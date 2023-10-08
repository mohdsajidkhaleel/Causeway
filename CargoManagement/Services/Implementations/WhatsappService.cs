using WhatsappBusiness.CloudApi;
using WhatsappBusiness.CloudApi.Messages.Requests;
using WhatsappBusiness.CloudApi.Response;
using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;
using WhatsappBusiness.CloudApi.Configurations;
using WhatsappBusiness.CloudApi.Exceptions;
using CargoManagement.Services.Abstractions;
using Microsoft.Extensions.Options;
using CargoManagement.Repository;
using Microsoft.EntityFrameworkCore;



namespace CargoManagement.Services.Implementations
{
    public class WhatsappService : IWhatsAppBusinessClient
    {
        private readonly cmspartialdeliveryContext _context;
        private readonly JsonSerializer _serializer = new JsonSerializer();
        private readonly WhatsAppBusinessCloudApiConfig _whatsAppConfig;
        public WhatsappService(IOptions<WhatsAppBusinessCloudApiConfig> whatsAppConfig, cmspartialdeliveryContext context)
        {
            _whatsAppConfig = whatsAppConfig.Value;
            _context = context;
        }

        #region sending whatsapp message coding
        public async Task<TextTemplateMessageRequest> SetTemplateValues(SendTemplateMessageViewModel Data)
        {
            var BookingDetails = await _context.Bookings
                .Include(x => x.Bookingitems)
                .Include(x => x.Customer)
                .Include(x => x.ReceipientCustomer)
                .Select(x => new
                {
                    Id = x.Id,
                    BookingId = x.BookingId,
                    ConsigneeMobile = x.ReceipientCustomer.Mobile,
                    ConsignorName = x.Customer.Name,
                    Packages = x.Bookingitems.Select(x => x.Quantity).FirstOrDefault()
                })
                .Where(x => x.Id == Data.BookingID)
                .SingleOrDefaultAsync();

            if (BookingDetails != null)
            {
                TextTemplateMessageRequest textTemplateMessage = new TextTemplateMessageRequest();
                //textTemplateMessage.To = "974" + BookingDetails.ConsigneeMobile.Trim();
                textTemplateMessage.To = "917293286899";
                textTemplateMessage.Template = new TextMessageTemplate();
                textTemplateMessage.Template.Name = _whatsAppConfig.WhatsappTemplateName == null ? WhatsAppBusinessRequestEndpoint.WhatsappTemplateName : _whatsAppConfig.WhatsappTemplateName;
                textTemplateMessage.Template.Language = new TextMessageLanguage();
                textTemplateMessage.Template.Language.Code = "en_US";
                textTemplateMessage.Template.Components = new List<TextMessageComponent>();
                textTemplateMessage.Template.Components.Add(new TextMessageComponent()
                {
                    Type = "body",
                    Parameters = new List<TextMessageParameter>()
                    {
                        new TextMessageParameter(){Type = "text",Text = Convert.ToString(BookingDetails.Packages)},
                        new TextMessageParameter(){Type = "text",Text = BookingDetails.ConsignorName},
                        new TextMessageParameter(){Type = "text",Text = BookingDetails.BookingId}
                    }
                });
                return textTemplateMessage;
            }
            else
            {
                return null;
            }                      
        }
                
        public async Task<WhatsAppResponse> SendTextMessageTemplateAsync(SendTemplateMessageViewModel Data, CancellationToken cancellationToken = default)
        {
            if (_whatsAppConfig.IsWhatsappEnabled)
            {
                var formattedWhatsAppEndpoint = WhatsAppBusinessRequestEndpoint.WhatsappEndPoint;
                TextTemplateMessageRequest textTemplateMessageRequest = await SetTemplateValues(Data);
                if (textTemplateMessageRequest == null)
                {
                    return null;
                }
                return await WhatsAppBusinessPostAsync<WhatsAppResponse>(textTemplateMessageRequest, formattedWhatsAppEndpoint, cancellationToken);
            }
           else
                return null;
        }

        private async Task<T> WhatsAppBusinessPostAsync<T>(object whatsAppDto, string whatsAppBusinessEndpoint, CancellationToken cancellationToken = default) where T : new()
        {
            HttpClient _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", WhatsAppBusinessRequestEndpoint.WhatsappPermanentAuthCode);
            T result = new();
            string json = JsonConvert.SerializeObject(whatsAppDto);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            cancellationToken.ThrowIfCancellationRequested();
            var response = await _httpClient.PostAsync(whatsAppBusinessEndpoint, content, cancellationToken).ConfigureAwait(false);

            if (response.IsSuccessStatusCode)
            {
                await response.Content.ReadAsStreamAsync(cancellationToken).ContinueWith((Task<Stream> stream) =>
                {
                    using var reader = new StreamReader(stream.Result);
                    using var json = new JsonTextReader(reader);
                    result = _serializer.Deserialize<T>(json);
                }, cancellationToken);
            }
            else
            {
                WhatsAppErrorResponse whatsAppErrorResponse = new WhatsAppErrorResponse();
                await response.Content.ReadAsStreamAsync(cancellationToken).ContinueWith((Task<Stream> stream) =>
                {
                    using var reader = new StreamReader(stream.Result);
                    using var json = new JsonTextReader(reader);
                    whatsAppErrorResponse = _serializer.Deserialize<WhatsAppErrorResponse>(json);
                }, cancellationToken);
                throw new WhatsappBusinessCloudAPIException(new HttpRequestException(whatsAppErrorResponse.Error.Message), response.StatusCode, whatsAppErrorResponse);
                                
            }
            return result;
        }

        #endregion

        #region sending text message coding
        //Below for sending text sms
        //using System;
        //using System.Collections.Generic;
        //using Twilio;
        //using Twilio.Rest.Api.V2010.Account;
        //using Twilio.Types;

        //static void Main(string[] args)
        //{
        //    var accountSid = "AC6b0aa6868f76027ad1422f8d2c3e1702";
        //    var authToken = "b1bff52153ae423ff7de6437e0a7a3bb";
        //    TwilioClient.Init(accountSid, authToken);

        //    var messageOptions = new CreateMessageOptions(
        //      new PhoneNumber("+917293286899"));
        //    messageOptions.From = new PhoneNumber("+14406893810");
        //    messageOptions.Body = "Your package received template , ID - 005645";


        //    var message = MessageResource.Create(messageOptions);
        //    Console.WriteLine(message.Body);
        //}

        #endregion

    }
}
