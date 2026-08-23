using Barman.Domain.Entities;
using Barman.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Barman.Persistence.Seed;

public static class PermissionSeeder
{
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        var permissions = new List<Permission>
        {
            // Customer
            new() { Code = "Customer.View", Name = "مشاهده مشتری" },
            new() { Code = "Customer.Create", Name = "ایجاد مشتری" },
            new() { Code = "Customer.Edit", Name = "ویرایش مشتری" },
            new() { Code = "Customer.Delete", Name = "حذف مشتری" },

            // Department
            new() { Code = "Department.View", Name = "مشاهده بخش" },
            new() { Code = "Department.Create", Name = "ایجاد بخش" },
            new() { Code = "Department.Edit", Name = "ویرایش بخش" },
            new() { Code = "Department.Delete", Name = "حذف بخش" },

            // Employee
            new() { Code = "Employee.View", Name = "مشاهده پرسنل" },
            new() { Code = "Employee.Create", Name = "ایجاد پرسنل" },
            new() { Code = "Employee.Edit", Name = "ویرایش پرسنل" },
            new() { Code = "Employee.Deactivate", Name = "غیرفعال‌سازی پرسنل" },

            // Reception
            new() { Code = "Reception.View", Name = "مشاهده پذیرش" },
            new() { Code = "Reception.Create", Name = "ثبت پذیرش" },
            new() { Code = "Reception.Edit", Name = "ویرایش پذیرش" },
            new() { Code = "Reception.Delete", Name = "حذف پذیرش" },

            // Sample
            new() { Code = "Sample.View", Name = "مشاهده نمونه" },
            new() { Code = "Sample.Create", Name = "ایجاد نمونه" },
            new() { Code = "Sample.Edit", Name = "ویرایش نمونه" },

            // Sample Category
            new() { Code = "SampleCategory.View", Name = "مشاهده دسته‌بندی نمونه" },
            new() { Code = "SampleCategory.Create", Name = "ایجاد دسته‌بندی نمونه" },
            new() { Code = "SampleCategory.Edit", Name = "ویرایش دسته‌بندی نمونه" },
            new() { Code = "SampleCategory.Delete", Name = "حذف دسته‌بندی نمونه" },

            // Matrix
            new() { Code = "Matrix.View", Name = "مشاهده ماتریس" },
            new() { Code = "Matrix.Create", Name = "ایجاد ماتریس" },
            new() { Code = "Matrix.Edit", Name = "ویرایش ماتریس" },
            new() { Code = "Matrix.Delete", Name = "حذف ماتریس" },

            // Test
            new() { Code = "Test.View", Name = "مشاهده آزمون" },
            new() { Code = "Test.Create", Name = "ایجاد آزمون" },
            new() { Code = "Test.Edit", Name = "ویرایش آزمون" },
            new() { Code = "Test.Delete", Name = "حذف آزمون" },

            // Test Panel
            new() { Code = "TestPanel.View", Name = "مشاهده پانل آزمون" },
            new() { Code = "TestPanel.Create", Name = "ایجاد پانل آزمون" },
            new() { Code = "TestPanel.Edit", Name = "ویرایش پانل آزمون" },
            new() { Code = "TestPanel.Delete", Name = "حذف پانل آزمون" },

            // Workflow
            new() { Code = "Workflow.Assign", Name = "ارجاع آزمون" },
            new() { Code = "Workflow.Inbox.View", Name = "مشاهده کارتابل" },
            new() { Code = "Workflow.AssignToSection", Name = "ارجاع به مسئول بخش" },
            new() { Code = "Workflow.AssignToAnalyst", Name = "ارجاع به کارشناس" },

            // Result
            new() { Code = "Result.View", Name = "مشاهده نتیجه" },
            new() { Code = "Result.Entry", Name = "ثبت نتیجه" },
            new() { Code = "Result.Edit", Name = "ویرایش نتیجه" },
            new() { Code = "Result.SectionApprove", Name = "تأیید نتیجه توسط مسئول بخش" },
            new() { Code = "Result.TechnicalApprove", Name = "تأیید نتیجه توسط مسئول فنی" },
            new() { Code = "Result.FinalApprove", Name = "تأیید نهایی نتیجه" },

            // Report
            new() { Code = "Report.View", Name = "مشاهده گزارش" },
            new() { Code = "Report.Generate", Name = "صدور گزارش نهایی" },

            // Dashboard
            new() { Code = "Dashboard.View", Name = "مشاهده داشبورد" },

            // System
            new() { Code = "Admin", Name = "دسترسی کامل" }
        };

        var existingCodes = await context.Permissions
            .Select(x => x.Code)
            .ToHashSetAsync();

        var newPermissions = permissions
            .Where(x => !existingCodes.Contains(x.Code))
            .ToList();

        if (newPermissions.Count == 0)
            return;

        context.Permissions.AddRange(newPermissions);

        await context.SaveChangesAsync();
    }
}