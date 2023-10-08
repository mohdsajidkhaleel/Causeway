using Newtonsoft.Json;
using System.Collections.Generic;

namespace WhatsappBusiness.CloudApi.Messages.Requests
{
    public class TextTemplateMessageRequest
    {
        [JsonProperty("messaging_product")]
        public string MessagingProduct { get; private set; } = "whatsapp";

        [JsonProperty("recipient_type")]
        public string RecipientType { get; private set; } = "individual";

        [JsonProperty("to")]
        public string To { get; set; }

        [JsonProperty("type")]
        public string Type { get; private set; } = "template";

        [JsonProperty("template")]
        public TextMessageTemplate Template { get; set; }
    }

    public class TextMessageTemplate
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("language")]
        public TextMessageLanguage Language { get; set; }

        [JsonProperty("components")]
        public List<TextMessageComponent> Components { get; set; }
    }

    public class TextMessageComponent
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("parameters")]
        public List<TextMessageParameter> Parameters { get; set; }
    }

    public class TextMessageParameter
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("image", NullValueHandling = NullValueHandling.Ignore)]
        public TemplateImage Image { get; set; }

        [JsonProperty("text", NullValueHandling = NullValueHandling.Ignore)]
        public string Text { get; set; }
    }

    public class TemplateImage
    {
        [JsonProperty("link")]
        public string Link { get; set; }
    }

    public class TextMessageLanguage
    {
        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("policy")]
        public string Policy { get; private set; } = "deterministic";
    }

    public class SendTemplateMessageViewModel
    {
        public int BookingID { get; set; }
    }

}
