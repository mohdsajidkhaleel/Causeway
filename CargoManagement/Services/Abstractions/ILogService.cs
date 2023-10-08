
namespace CargoManagement.Services.Abstractions
{
    public interface ILogService 
    {
        Task LogException(string? functionName, string? controllerName, string exceptionDetails);
    }
}
