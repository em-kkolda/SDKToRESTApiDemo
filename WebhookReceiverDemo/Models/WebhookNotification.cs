using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Http;
using System.Net.Http;
using Newtonsoft.Json.Linq;

namespace WebhookReceiverDemo.Models
{
    // Provides a wrapper for a webhook notification
    public class WebhookNotification
    {
        // Constructor for the notifiction object
        private WebhookNotification(JObject message)
        {
            this.Message = message;
        }

        // Accessor for the raw message content of the notification
        public JObject Message { get; private set; }

        public JObject Metadata
        {
            get { return (JObject) this.Message["meta"]; }
        }

        // Accessors for the basic Webhook attributes
        public string EventID
        {
            get { return this.Message.Value<string>("eventId"); }
        }

        public DateTime EventTime
        {
            get { return DateTime.Parse(this.Message.Value<string>("eventTime")); }
        }

        public string EventType
        {
            get { return this.Message.Value<string>("eventType"); }
        }

        public string ResourceType
        {
            get { return this.Metadata.Value<string>("resourceType");  }
        }

        public string ResourceID
        {
            get { return this.Metadata.Value<string>("resourceId"); }
        }

        public string UserID
        {
            get { return this.Metadata.Value<string>("userId"); }
        }

        // Parses and validates an incoming request and converts it to a WebhookNotification object
        public async static Task<WebhookNotification> FromRequest(HttpRequestMessage request)
        {
            // Retrieve the Elli-Signature header to validate the message
            var sig = request.Headers.GetValues("Elli-Signature").FirstOrDefault();
            if (sig == null) throw new HttpResponseException(
                new HttpResponseMessage(HttpStatusCode.BadRequest)
                {
                    Content = new StringContent("Missing required Elli-Signature")
                });

            // Retrieve the raw bytes of the request body
            var msgBody = await request.Content.ReadAsByteArrayAsync();

            // Validate the signature
            if (!EncompassApi.ValidateSignature(sig, msgBody))
                throw new HttpResponseException(
                new HttpResponseMessage(HttpStatusCode.BadRequest)
                {
                    Content = new StringContent("Invalid Elli-Signature")
                });

            try
            {
                // Parse the body of the request as a JSON object
                string jsonText = Encoding.UTF8.GetString(msgBody);
                return new WebhookNotification(JObject.Parse(jsonText));
            }
            catch (Exception ex)
            {
                // Recast the exception to an HttpResponseException
                throw new HttpResponseException(
                new HttpResponseMessage(HttpStatusCode.BadRequest)
                {
                    Content = new StringContent(ex.Message)
                });
            }
        }

    }
}