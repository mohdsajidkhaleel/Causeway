using WhatsappBusiness.CloudApi.Messages.Requests;
using WhatsappBusiness.CloudApi.Response;

namespace CargoManagement.Services.Abstractions
{
    public interface IWhatsAppBusinessClient
    {
        Task<WhatsAppResponse> SendTextMessageTemplateAsync(SendTemplateMessageViewModel Data, CancellationToken cancellationToken = default);

    }
}
