using System.Diagnostics;
using System.IO.Compression;

if (args.Length < 2)
{
    Console.WriteLine("Usage:");
    Console.WriteLine("BarmanUpdater <zipPath> <appDirectory> [--no-start]");
    return;
}

var zipPath = Path.GetFullPath(args[0]);
var appDirectory = Path.GetFullPath(args[1]);

var noStart =
    args.Any(x =>
        string.Equals(
            x,
            "--no-start",
            StringComparison.OrdinalIgnoreCase));

if (!File.Exists(zipPath))
{
    Console.WriteLine($"ZIP not found: {zipPath}");
    Environment.ExitCode = 1;
    return;
}

if (!Directory.Exists(appDirectory))
{
    Console.WriteLine($"Application directory not found: {appDirectory}");
    Environment.ExitCode = 1;
    return;
}

var tempDirectory = Path.Combine(
    Path.GetTempPath(),
    "BarmanLimsUpdate",
    Guid.NewGuid().ToString("N"));

Directory.CreateDirectory(tempDirectory);

try
{
    Console.WriteLine("========================================");
    Console.WriteLine(" Barman LIMS Updater");
    Console.WriteLine("========================================");
    Console.WriteLine();

    Console.WriteLine($"ZIP: {zipPath}");
    Console.WriteLine($"APP: {appDirectory}");
    Console.WriteLine($"START AFTER UPDATE: {!noStart}");
    Console.WriteLine();

    // ----------------------------------------
    // 1. Extract update
    // ----------------------------------------

    Console.WriteLine("Extracting update...");

    ZipFile.ExtractToDirectory(
        zipPath,
        tempDirectory,
        overwriteFiles: true);

    Console.WriteLine("Extraction completed.");
    Console.WriteLine();

    // ----------------------------------------
    // 2. Wait for Barman LIMS to exit
    // ----------------------------------------

    Console.WriteLine("Waiting for Barman LIMS to close...");

    var processes =
        Process.GetProcessesByName("BarmanLims");

    if (processes.Length == 0)
    {
        Console.WriteLine("Barman LIMS is not running.");
    }
    else
    {
        foreach (var process in processes)
        {
            try
            {
                Console.WriteLine(
                    $"Found process PID {process.Id}.");

                if (!process.HasExited)
                {
                    Console.WriteLine(
                        "Waiting up to 30 seconds for application exit...");

                    if (!process.WaitForExit(30000))
                    {
                        Console.WriteLine(
                            "Application did not exit within 30 seconds.");

                        Console.WriteLine(
                            "The update cannot continue while Barman LIMS is running.");

                        Environment.ExitCode = 2;
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    $"Unable to wait for process: {ex.Message}");

                Environment.ExitCode = 2;
                return;
            }
            finally
            {
                process.Dispose();
            }
        }
    }

    // ----------------------------------------
    // 3. Verify process is really gone
    // ----------------------------------------

    var remainingProcesses =
        Process.GetProcessesByName("BarmanLims");

    if (remainingProcesses.Length > 0)
    {
        Console.WriteLine();
        Console.WriteLine(
            "Barman LIMS is still running.");

        Console.WriteLine(
            "Update cancelled.");

        foreach (var process in remainingProcesses)
        {
            process.Dispose();
        }

        Environment.ExitCode = 2;
        return;
    }

    Console.WriteLine("Barman LIMS is closed.");
    Console.WriteLine();

    // ----------------------------------------
    // 4. Replace application files
    // ----------------------------------------

    Console.WriteLine("Replacing application files...");

    var files =
        Directory.GetFiles(
            tempDirectory,
            "*",
            SearchOption.AllDirectories);

    var copiedFiles = 0;
    var skippedFiles = 0;

    var updaterFileName = "BarmanUpdater.exe";

    foreach (var sourceFile in files)
    {
        var relativePath =
            Path.GetRelativePath(
                tempDirectory,
                sourceFile);

        // The updater cannot overwrite itself while it is running.
        if (
            string.Equals(
                Path.GetFileName(relativePath),
                updaterFileName,
                StringComparison.OrdinalIgnoreCase)
            &&
            relativePath.StartsWith(
                "Updater" + Path.DirectorySeparatorChar,
                StringComparison.OrdinalIgnoreCase))
        {
            Console.WriteLine(
                $"Skipping running updater: {relativePath}");

            skippedFiles++;
            continue;
        }

        var destinationFile =
            Path.Combine(
                appDirectory,
                relativePath);

        var destinationDirectory =
            Path.GetDirectoryName(destinationFile);

        if (!string.IsNullOrWhiteSpace(destinationDirectory))
        {
            Directory.CreateDirectory(
                destinationDirectory);
        }

        File.Copy(
            sourceFile,
            destinationFile,
            overwrite: true);

        copiedFiles++;
    }

    Console.WriteLine(
        $"Copied {copiedFiles} file(s).");

    Console.WriteLine(
        $"Skipped {skippedFiles} file(s).");

    Console.WriteLine();
    Console.WriteLine("Update completed.");

    // ----------------------------------------
    // 5. Start Barman LIMS
    // ----------------------------------------

    if (noStart)
    {
        Console.WriteLine();
        Console.WriteLine(
            "START SKIPPED (--no-start).");

        return;
    }

    var appExe =
        Path.Combine(
            appDirectory,
            "BarmanLims.exe");

    if (!File.Exists(appExe))
    {
        Console.WriteLine();
        Console.WriteLine(
            "ERROR: BarmanLims.exe was not found:");

        Console.WriteLine(appExe);

        Environment.ExitCode = 3;
        return;
    }

    Console.WriteLine();
    Console.WriteLine("Starting Barman LIMS...");

    Process.Start(new ProcessStartInfo
    {
        FileName = appExe,
        WorkingDirectory = appDirectory,
        UseShellExecute = true
    });

    Console.WriteLine("Barman LIMS started successfully.");
}
catch (Exception ex)
{
    Console.Error.WriteLine();
    Console.Error.WriteLine("========================================");
    Console.Error.WriteLine(" UPDATE FAILED");
    Console.Error.WriteLine("========================================");
    Console.Error.WriteLine();
    Console.Error.WriteLine(ex);

    Environment.ExitCode = 1;
}
finally
{
    try
    {
        if (Directory.Exists(tempDirectory))
        {
            Directory.Delete(
                tempDirectory,
                recursive: true);
        }
    }
    catch
    {
        // Ignore temporary directory cleanup errors.
    }
}
