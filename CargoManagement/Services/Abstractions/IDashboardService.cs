using CargoManagement.Models.Dashboard;

namespace CargoManagement.Services.Abstractions
{
    public interface IDashboardService
    {
        Task<BookingDashboardResponseDTO> TodaysNewBookings();
        Task<DashboardResponseDTO> TodaysClosedBookings();
        Task<DashboardResponseDTO> TodaysScheduledJourneys();


        Task<DashboardResponseDTO> OutstandingBookings();
        decimal? OutstandingCustomerCredit();
    }
}
