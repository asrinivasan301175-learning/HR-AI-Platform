using ModelContextProtocol.Server;
using System.ComponentModel;

namespace HR.McpServer.Tools;

[McpServerToolType]
public class EmployeeTools
{
    private static readonly HttpClient Client = new()
    {
        BaseAddress = new Uri("http://localhost:5001")
    };

    [McpServerTool]
    [Description("Gets employee details using employee ID")]
    public static async Task<string> GetEmployee(
        int employeeId)
    {
        var response = await Client.GetAsync(
            $"/api/employees/{employeeId}");

        if (!response.IsSuccessStatusCode)
            return "Employee not found";

        return await response.Content.ReadAsStringAsync();
    }


    [McpServerTool]
    [Description("Gets available leave balance for an employee")]
    public static async Task<string> GetLeaveBalance(
        int employeeId)
    {
        var response = await Client.GetAsync(
            $"/api/leaves/{employeeId}/balance");

        if (!response.IsSuccessStatusCode)
            return "Leave balance not found";

        return await response.Content.ReadAsStringAsync();
    }


    [McpServerTool]
    [Description("Gets the company annual leave policy")]
    public static async Task<string> GetLeavePolicy()
    {
        var response = await Client.GetAsync(
            "/api/policies/leave");

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadAsStringAsync();
    }
}