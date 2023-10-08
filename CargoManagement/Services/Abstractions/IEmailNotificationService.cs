using WhatsappBusiness.CloudApi.Messages.Requests;
using WhatsappBusiness.CloudApi.Response;

namespace CargoManagement.Services.Abstractions
{
    public interface IEmailNotificationService
    {
        Task CheckNewBookings();
        Task<bool> updateBookingEmailNotification(int Id);
    }
}
