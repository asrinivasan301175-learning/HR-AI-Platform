var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

// 1. Employee API
app.MapGet("/api/employees/{id:int}", (int id) =>
{
    if (id == 101)
    {
        return Results.Ok(new
        {
            EmployeeId = 101,
            Name = "John",
            Department = "IT",
            Email = "john@company.com"
        });
    }

    if (id == 102)
    {
        return Results.Ok(new
        {
            EmployeeId = 102,
            Name = "David",
            Department = "Finance",
            Email = "david@company.com"
        });
    }

    return Results.NotFound("Employee not found");
});


// 2. Leave Balance API
app.MapGet("/api/leaves/{employeeId:int}/balance",
    (int employeeId) =>
{
    if (employeeId != 101 && employeeId != 102)
        return Results.NotFound("Employee not found");

    return Results.Ok(new
    {
        EmployeeId = employeeId,
        TotalLeaves = 24,
        UsedLeaves = 8,
        AvailableLeaves = 16
    });
});


// 3. Leave Policy API
app.MapGet("/api/policies/leave", () =>
{
    return Results.Ok(new
    {
        PolicyName = "Annual Leave Policy",
        AnnualLeave = 24,
        CarryForward = 5,
        ApprovalRequired = true,
        Description =
            "Employees receive 24 annual leave days."
    });
});

app.Run();