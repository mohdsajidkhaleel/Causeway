using CargoManagement.Repository;
using CargoManagement.Services.Abstractions;
using MailService.WebApi.Settings;
using Microsoft.Extensions.Options;

namespace CargoManagement.Services.Implementations
{
    public class LoggerService : ILogService
    {
        private readonly cmspartialdeliveryContext _context;
        private readonly MailSettings _mailSettings;
        public LoggerService(IOptions<MailSettings> mailSettings, cmspartialdeliveryContext context)
        {
            _context = context;
            _mailSettings = mailSettings.Value;
        }
        public async Task LogException(string? functionName, string? controllerName, string exceptionDetails)
        {
            Exceptionlog logentry = new Exceptionlog()
            {
                ControllerName = controllerName,
                ExceptionDetails = exceptionDetails,
                FunctionName = functionName,
                CreatedDate = DateTime.UtcNow
            };
            await _context.Exceptionlogs.AddAsync(logentry);
            await _context.SaveChangesAsync();
        }

    }
}
