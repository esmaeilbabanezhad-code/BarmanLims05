using Barman.Domain.Entities;
using Barman.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Barman.Persistence.Seed;

public static class RolePermissionSeeder
{
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        var rolePermissions = new Dictionary<string, string[]>
        {
            ["ANALYST"] =
            [
                "Result.Edit",
                "Result.Entry",
                "Result.View",
                "Sample.View",
                "Test.View"
            ],

            ["SECTION_MANAGER"] =
            [
                "Result.SectionApprove",
                "Result.View",
                "Sample.View",
                "Test.View",
                "Workflow.AssignToAnalyst",
                "Workflow.Inbox.View"
            ],

            ["TECH_MANAGER"] =
                [
                    "Result.TechnicalApprove",
                    "Result.View",
                    "Sample.View",
                    "Test.View",
                    "Workflow.AssignToSection",
                    "Workflow.Inbox.View",

                    "LimitReference.View",
                    "LimitReference.Create",
                    "LimitReference.Edit",
                    "LimitReference.Delete"
                ],

            ["DIRECTOR"] =
                [
                    "Report.Generate",
                    "Report.View",
                    "Result.FinalApprove",
                    "Result.View",

                    "LimitReference.View"
                ],

            ["LIMS_ADMINISTRATOR"] =
[
    "Customer.Create",
    "Customer.Delete",
    "Customer.Edit",
    "Customer.View",

    "Department.Create",
    "Department.Delete",
    "Department.Edit",
    "Department.View",

    "Employee.Create",
    "Employee.Edit",
    "Employee.View",

    "Reception.Create",
    "Reception.Delete",
    "Reception.Edit",
    "Reception.View",
    "Reception.Correction",

    "Sample.Create",
    "Sample.Edit",
    "Sample.View",

    "SampleCategory.Create",
    "SampleCategory.Delete",
    "SampleCategory.Edit",
    "SampleCategory.View",

    "Matrix.Create",
    "Matrix.Delete",
    "Matrix.Edit",
    "Matrix.View",

    "Test.Create",
    "Test.Delete",
    "Test.Edit",
    "Test.View",

    "TestPanel.Create",
    "TestPanel.Delete",
    "TestPanel.Edit",
    "TestPanel.View",

    "LimitReference.View",
    "LimitReference.Create",
    "LimitReference.Edit",
    "LimitReference.Delete",

    "Workflow.Assign",
    "Workflow.Inbox.View",
    "Workflow.AssignToSection",
    "Workflow.AssignToAnalyst",

    "Result.View",
    "Result.Entry",
    "Result.Edit",
    "Result.SectionApprove",
    "Result.TechnicalApprove",
    "Result.FinalApprove",

    "Report.View",
    "Report.Generate",

    "MasterData.View",
    "MasterData.Import",
    "MasterData.Export"
],



            ["RECEPTION"] =
                [
                    "Customer.Create",
                    "Customer.Delete",
                    "Customer.Edit",
                    "Customer.View",
                    "Reception.Create",
                    "Reception.Delete",
                    "Reception.Edit",
                    "Reception.View",
                    "Reception.Correction",
                    "Sample.Create",
                    "Sample.Edit",
                    "Sample.View"
                ]
        };

        foreach (var item in rolePermissions)
        {
            var role = await context.Roles
                .FirstOrDefaultAsync(x => x.Code == item.Key);

            if (role == null)
                continue;

            var permissionCodes = item.Value;

            var permissions = await context.Permissions
                .Where(x => permissionCodes.Contains(x.Code))
                .ToListAsync();

            var existingPermissionIds = await context.RolePermissions
                .Where(x => x.RoleId == role.Id)
                .Select(x => x.PermissionId)
                .ToListAsync();

            foreach (var permission in permissions)
            {
                if (!existingPermissionIds.Contains(permission.Id))
                {
                    context.RolePermissions.Add(
                        new RolePermission
                        {
                            RoleId = role.Id,
                            PermissionId = permission.Id
                        });
                }
            }
        }

        await context.SaveChangesAsync();
    }
}