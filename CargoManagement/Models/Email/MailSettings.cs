using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MailService.WebApi.Settings
{
    public class MailSettings
    {
        public string Mail { get; set; }
        public string DisplayName { get; set; }
        public string Password { get; set; }
        public string Host { get; set; }
        public string BookingHTMLtemplateURL { get; set; }
        public string BookingHTMLtemplateLogoURL { get; set; }
        public int Port { get; set; }
        public bool IsEmailEnabled { get; set; }
    }

    public class BookingMailDetails
    {
        public int Id { get; set; }
        public string BookingId { get; set; }
        public string ReceipientCustomer { get; set; }
        public string ConsignorName { get; set; }
        public string ConsigneeName { get; set; }
        public int? Packages { get; set; }
        public string? Description { get; set; }
        public DateTime? BookingDate { get; set; }
        public string? ShipmentMode { get; set; }
        public string? ReceipientCustomerMailId { get; set; }
    }
}
