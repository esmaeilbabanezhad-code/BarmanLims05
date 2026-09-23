using Npgsql;
using System.Text.Json;



const string PgRestorePath =
    @"C:\Program Files\PostgreSQL\18\bin\pg_restore.exe";

const string PostgreSqlConnectionString =
    "Host=localhost;Port=5432;Database=postgres;Username=postgres;Password=Bab@09126432744";


if (args.Length != 1)
{
    Console.WriteLine(
        "Usage: Barman.DatabaseRestoreWorker <restoreRequest.json>");

    return 1;
}

var requestPath = args[0];

if (!File.Exists(requestPath))
{
    Console.WriteLine(
        $"ERROR: Restore request not found: {requestPath}");

    return 2;
}

if (!File.Exists(PgRestorePath))
{
    Console.WriteLine(
        $"ERROR: pg_restore.exe not found: {PgRestorePath}");

    return 3;
}

var jsonOptions =
    new JsonSerializerOptions
    {
        WriteIndented = true
    };

DatabaseRestoreRequest? request;

try
{
    var json =
        await File.ReadAllTextAsync(requestPath);

    request =
        JsonSerializer.Deserialize<DatabaseRestoreRequest>(
            json,
            jsonOptions);
}
catch (Exception ex)
{
    Console.WriteLine(
        $"ERROR: Cannot read restore request: {ex.Message}");

    return 4;
}

if (request is null)
{
    Console.WriteLine(
        "ERROR: Restore request is empty or invalid.");

    return 5;
}

if (!string.Equals(
        request.Status,
        "Pending",
        StringComparison.OrdinalIgnoreCase))
{
    Console.WriteLine(
        $"ERROR: Restore request status is '{request.Status}'. Expected 'Pending'.");

    return 6;
}

if (string.IsNullOrWhiteSpace(request.BackupPath))
{
    Console.WriteLine(
        "ERROR: BackupPath is empty.");

    return 7;
}

if (!File.Exists(request.BackupPath))
{
    Console.WriteLine(
        $"ERROR: Backup file not found: {request.BackupPath}");

    return 8;
}



Console.WriteLine("========================================");
Console.WriteLine(" Barman LIMS Database Restore Worker");
Console.WriteLine("========================================");
Console.WriteLine();

Console.WriteLine(
    $"Request: {requestPath}");

Console.WriteLine(
    $"Backup: {request.BackupPath}");

Console.WriteLine(
    $"Target database: {request.TargetDatabase}");

Console.WriteLine();

// ------------------------------------------------------------
// Mark request as Running
// ------------------------------------------------------------

request.Status = "Running";
request.StartedAt = DateTime.Now;
request.ErrorMessage = null;
request.ExitCode = null;

await File.WriteAllTextAsync(
    requestPath,
    JsonSerializer.Serialize(
        request,
        jsonOptions));

try
{
    var maintenanceBuilder =
        new NpgsqlConnectionStringBuilder(
            PostgreSqlConnectionString);

    maintenanceBuilder.Database = "postgres";

    await using var connection =
        new NpgsqlConnection(
            maintenanceBuilder.ConnectionString);

    await connection.OpenAsync();

    Console.WriteLine(
        "Connected to PostgreSQL maintenance database.");

    // --------------------------------------------------------
    // Terminate existing connections
    // --------------------------------------------------------

    await using (
        var terminateCommand =
            new NpgsqlCommand(
                """
                SELECT pg_terminate_backend(pid)
                FROM pg_stat_activity
                WHERE datname = $1
                  AND pid <> pg_backend_pid();
                """,
                connection))
    {
        terminateCommand.Parameters.AddWithValue(
            request.TargetDatabase);

        await terminateCommand.ExecuteNonQueryAsync();
    }

    Console.WriteLine(
        "Existing database connections terminated.");

    // --------------------------------------------------------
    // Quote database name safely
    // --------------------------------------------------------

    var quotedDatabaseName =
        new NpgsqlCommandBuilder()
            .QuoteIdentifier(
                request.TargetDatabase);

    // --------------------------------------------------------
    // Drop database
    // --------------------------------------------------------

    await using (
        var dropCommand =
            new NpgsqlCommand(
                $"DROP DATABASE IF EXISTS {quotedDatabaseName};",
                connection))
    {
        await dropCommand.ExecuteNonQueryAsync();
    }

    Console.WriteLine(
        "Target database dropped.");

    // --------------------------------------------------------
    // Recreate database
    // --------------------------------------------------------

    await using (
        var createCommand =
            new NpgsqlCommand(
                $"CREATE DATABASE {quotedDatabaseName};",
                connection))
    {
        await createCommand.ExecuteNonQueryAsync();
    }

    Console.WriteLine(
        "Target database recreated.");

    // --------------------------------------------------------
    // Restore
    // --------------------------------------------------------

    var restoreConnectionBuilder =
        new NpgsqlConnectionStringBuilder(
            PostgreSqlConnectionString);

    restoreConnectionBuilder.Database =
        request.TargetDatabase;

    var restoreStartInfo =
        new System.Diagnostics.ProcessStartInfo
        {
            FileName = PgRestorePath,
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true
        };

    restoreStartInfo.ArgumentList.Add("--host");
    restoreStartInfo.ArgumentList.Add(
        restoreConnectionBuilder.Host ?? "localhost");

    restoreStartInfo.ArgumentList.Add("--port");
    restoreStartInfo.ArgumentList.Add(
        restoreConnectionBuilder.Port.ToString());

    restoreStartInfo.ArgumentList.Add("--username");
    restoreStartInfo.ArgumentList.Add(
        restoreConnectionBuilder.Username ?? "postgres");

    restoreStartInfo.ArgumentList.Add("--dbname");
    restoreStartInfo.ArgumentList.Add(
        request.TargetDatabase);

    restoreStartInfo.ArgumentList.Add("--no-owner");
    restoreStartInfo.ArgumentList.Add("--exit-on-error");
    restoreStartInfo.ArgumentList.Add(request.BackupPath);

    restoreStartInfo.Environment["PGPASSWORD"] =
        restoreConnectionBuilder.Password ?? string.Empty;

    using var restoreProcess =
        new System.Diagnostics.Process
        {
            StartInfo = restoreStartInfo
        };

    Console.WriteLine();
    Console.WriteLine(
        "Starting pg_restore...");

    restoreProcess.Start();

    var outputTask =
        restoreProcess.StandardOutput.ReadToEndAsync();

    var errorTask =
        restoreProcess.StandardError.ReadToEndAsync();

    await restoreProcess.WaitForExitAsync();

    var output =
        await outputTask;

    var error =
        await errorTask;

    if (!string.IsNullOrWhiteSpace(output))
    {
        Console.WriteLine();
        Console.WriteLine(
            "pg_restore output:");

        Console.WriteLine(output);
    }

    if (!string.IsNullOrWhiteSpace(error))
    {
        Console.WriteLine();
        Console.WriteLine(
            "pg_restore error output:");

        Console.WriteLine(error);
    }

    request.ExitCode =
        restoreProcess.ExitCode;

    if (restoreProcess.ExitCode != 0)
    {
        request.Status = "Failed";
        request.CompletedAt = DateTime.Now;
        request.ErrorMessage =
            string.IsNullOrWhiteSpace(error)
                ? "pg_restore failed."
                : error.Trim();

        await File.WriteAllTextAsync(
            requestPath,
            JsonSerializer.Serialize(
                request,
                jsonOptions));

        Console.WriteLine();
        Console.WriteLine(
            $"RESTORE FAILED. ExitCode: {restoreProcess.ExitCode}");

        return 10;
    }

    request.Status = "Success";
    request.CompletedAt = DateTime.Now;
    request.ErrorMessage = null;

    await File.WriteAllTextAsync(
        requestPath,
        JsonSerializer.Serialize(
            request,
            jsonOptions));

    Console.WriteLine();
    Console.WriteLine(
        "========================================");

    Console.WriteLine(
        " RESTORE COMPLETED SUCCESSFULLY");

    Console.WriteLine(
        "========================================");

    return 0;
}
catch (Exception ex)
{
    request.Status = "Failed";
    request.CompletedAt = DateTime.Now;
    request.ErrorMessage = ex.ToString();

    await File.WriteAllTextAsync(
        requestPath,
        JsonSerializer.Serialize(
            request,
            jsonOptions));

    Console.WriteLine();
    Console.WriteLine(
        "RESTORE FAILED:");

    Console.WriteLine(ex);

    return 20;
}


// ------------------------------------------------------------
// Local request model
// ------------------------------------------------------------

sealed class DatabaseRestoreRequest
{
    public string BackupPath { get; set; } = string.Empty;

    public string EmergencyBackupPath { get; set; } = string.Empty;

    public string TargetDatabase { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }

    public DateTime? StartedAt { get; set; }

    public DateTime? CompletedAt { get; set; }

    public string Status { get; set; } = "Pending";

    public string? ErrorMessage { get; set; }

    public int? ExitCode { get; set; }
}