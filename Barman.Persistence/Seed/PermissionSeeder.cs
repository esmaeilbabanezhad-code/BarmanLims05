using Barman.Domain.Entities;
using Barman.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Barman.Persistence.Seed;

public static class PermissionSeeder
{
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        if (await context.Permissions.AnyAsync())
            return;

        var permissions = new List<Permission>
        {
            new() { Code="Customer.View", Name="مشاهده مشتری" },
            new() { Code="Customer.Create", Name="ایجاد مشتری" },
            new() { Code="Customer.Edit", Name="ویرایش مشتری" },
            new() { Code="Customer.Delete", Name="حذف مشتری" },

            new() { Code="Reception.View", Name="مشاهده پذیرش" },
            new() { Code="Reception.Create", Name="ثبت پذیرش" },
            new() { Code="Reception.Edit", Name="ویرایش پذیرش" },
            new() { Code="Reception.Delete", Name="حذف پذیرش" },

            new() { Code="Sample.View", Name="مشاهده نمونه" },
            new() { Code="Sample.Create", Name="ایجاد نمونه" },

            new() { Code="Test.View", Name="مشاهده آزمون" },
            new() { Code="Test.Create", Name="ایجاد آزمون" },
            new() { Code="Test.Edit", Name="ویرایش آزمون" },

            new() { Code="Workflow.Assign", Name="ارجاع آزمون" },

            new() { Code="Result.Entry", Name="ثبت نتیجه" },

            new() { Code="Report.Approve", Name="تأیید گزارش" },

            new() { Code="Dashboard.View", Name="مشاهده داشبورد" },

            new() { Code="Admin", Name="دسترسی کامل" }
        };

        context.Permissions.AddRange(permissions);

        await context.SaveChangesAsync();
    }
}