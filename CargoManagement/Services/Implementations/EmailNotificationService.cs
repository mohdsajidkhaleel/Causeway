using CargoManagement.Repository;
using CargoManagement.Services.Abstractions;
using MailService.WebApi.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Data;
using System.Net.Mail;
using System.Net.Mime;

namespace CargoManagement.Services.Implementations
{
    public class EmailNotificationService : IEmailNotificationService
    {
        private readonly cmspartialdeliveryContext _context;
        private readonly ILogService _logService;
        private readonly MailSettings _mailSettings;
        public EmailNotificationService(IOptions<MailSettings> mailSettings, cmspartialdeliveryContext context, ILogService logService)
        {
            _mailSettings = mailSettings.Value;
            _context = context;
            _logService = logService;
        }

        public async Task CheckNewBookings()
        {
            if (_mailSettings.IsEmailEnabled)
            {
                #region For BackgroundService
                var BookingDetails = await _context.Bookings
                    .Include(x => x.Bookingitems)
                    .Include(x => x.Customer)
                    .Include(x => x.ReceipientCustomer)
                    .Where(x => x.IsEmailNotificationSent == false && x.ReceipientCustomer.Email != null)
                    .Select(x => new BookingMailDetails
                    {
                        Id = x.Id,
                        BookingId = x.BookingId,
                        ReceipientCustomer = x.ReceipientCustomer.Name,
                        ConsignorName = x.Customer.Name,
                        ConsigneeName = x.ReceipientCustomer.Name,
                        Packages = x.Bookingitems.FirstOrDefault(y => y.BookingId == x.Id).Quantity,
                        Description = x.Bookingitems.FirstOrDefault(y => y.BookingId == x.Id).Description,
                        BookingDate = x.CreateDate,
                        ShipmentMode = x.ShipmentMode == "RO" ? "Road" : x.ShipmentMode == "RA" ? "Sea" : "",
                        ReceipientCustomerMailId = x.ReceipientCustomer.Email
                    }).ToListAsync();

                if (BookingDetails.Count() > 0)
                {
                    foreach (var booking in BookingDetails)
                    {
                        try
                        {
                            var isSentSuccessfull = await SentEmailNotification(booking);
                            if (isSentSuccessfull)
                            {
                                await updateBookingEmailNotification(booking.Id);
                            }
                        }
                        catch (Exception ex)
                        {
                            await _logService.LogException("CheckNewBookings()", "EmailController", ex.ToString());
                        }
                    }
                }
                #endregion
            }


            //var isSentSuccessfull = await SentEmailNotification(new BookingMailDetails());
            //if (isSentSuccessfull)
            //{
            //    var data = await updateBookingEmailNotification(654);
            //}
        }

        public async Task<bool> SentEmailNotification(BookingMailDetails BookingDetails)
        {
            try
            {
                //string logopath = Directory.GetCurrentDirectory() + "\\emailtemplate\\causewaylogopng.png";
                //string filepath = Directory.GetCurrentDirectory() + "\\emailtemplate\\bookingtemplate.html";

                string logopath = _mailSettings.BookingHTMLtemplateLogoURL;
                string filepath = _mailSettings.BookingHTMLtemplateURL;

                StreamReader str = new StreamReader(filepath);
                string MailText = str.ReadToEnd();
                str.Close();

                MailText = MailText.Replace("[BookingID]", BookingDetails.BookingId).Replace("[ConsignorName]", BookingDetails.ConsignorName)
                                   .Replace("[ConsigneeName]", BookingDetails.ConsigneeName).Replace("[Packages]", BookingDetails.Packages.ToString())
                                   .Replace("[Description]", BookingDetails.Description).Replace("[ShipmentMode]", BookingDetails.ShipmentMode)
                                   .Replace("[BookingDate]", BookingDetails.BookingDate.ToString());

                var view = AlternateView.CreateAlternateViewFromString(MailText, null, "text/html");
                var inlineLogo = new LinkedResource(logopath, MediaTypeNames.Image.Jpeg);
                inlineLogo.ContentId = "sample";
                view.LinkedResources.Add(inlineLogo);

                MailMessage newMail = new MailMessage();
                newMail.To.Add(new System.Net.Mail.MailAddress(BookingDetails.ReceipientCustomerMailId));
                //newMail.To.Add(new System.Net.Mail.MailAddress("jojijayakumar549@gmail.com"));
                newMail.From = new System.Net.Mail.MailAddress(_mailSettings.Mail);
                newMail.Subject = $"CAUSEWAY CARGO - Shipment Details for Booking ID - " + BookingDetails.BookingId;
                newMail.IsBodyHtml = true;
                newMail.AlternateViews.Add(view);               

                try
                {
                    using (var smtpClient = new SmtpClient())
                    {
                        smtpClient.Host = _mailSettings.Host;
                        smtpClient.Port = _mailSettings.Port;
                        smtpClient.EnableSsl = true;
                        smtpClient.Credentials = new System.Net.NetworkCredential(_mailSettings.Mail, _mailSettings.Password);
                        await smtpClient.SendMailAsync(newMail);
                        smtpClient.Dispose();
                        return true;
                    }
                }
                catch (Exception ex)
                {
                    await _logService.LogException("SentEmailNotification()", "Email", ex.ToString());
                    return false;
                }
            }
            catch (Exception ex)
            {
                await _logService.LogException("SentEmailNotification", "Email", ex.ToString());
                return false;
            }
        }

        public async Task<bool> updateBookingEmailNotification(int BookingID)
        {
            try
            {
                var BookingData = await _context.Bookings.Where(x => x.Id == BookingID).FirstOrDefaultAsync();
                if (BookingData != null)
                {
                    BookingData.IsEmailNotificationSent = true;
                    _context.Update(BookingData);
                    await _context.SaveChangesAsync();
                }
                return true;
            }
            catch (Exception ex)
            {
                await _logService.LogException("updateBookingEmailNotification","Email", ex.ToString());
                return false;
            }           
        }
    }
}
