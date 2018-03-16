using System;
using System.Collections.Generic;
using System.Linq;
using System.Dynamic;
using System.Net;
using System.Configuration;
using System.Threading.Tasks;
using System.Net.Http;
using System.Web.Http;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using WebhookReceiverDemo.Models;

namespace WebhookReceiverDemo.Controllers
{
    public class WebhookController : ApiController
    {
        // POST: api/Webhook
        public async Task<IHttpActionResult> Post()
        {
            // Parse the Webhook Notification from the HTTP request
            WebhookNotification notification = await WebhookNotification.FromRequest(Request);

            // Generate an event key to identify the event type. Event keys have the format
            // {ResourceType}.{EventType} or, if no resource type is present, simply {EventType}
            string eventKey = getEventKey(notification);

            // Add case statements for other events that you wish to capture and have
            // subscribed to.
            switch (eventKey)
            {
                case "Loan.Create": onLoanCreated(notification); break;
                    //case "Loan.Update": onLoanUpdated(notification); break;
                    //case "Loan.Submit": onLoanSubmitted(notification); break;
            }

            // Let the webhook sender know we received and processed the notification
            return Ok();
        }

        // Creates an event key for a webhook notification
        private string getEventKey(WebhookNotification notification)
        {
            return notification.ResourceType == null ? notification.EventType 
                : (notification.ResourceType + "." + notification.EventType);
        }

        // Handle the creation of a resource
        private void onLoanCreated(WebhookNotification notification)
        {
            // Do your work for the Loan Create event here,
            // but keep it fast -- the Webhook server may time out otherwise and re-call
            // your webhook again. For example, write the Loan ID to a database
            // or queue or run a background task to process this loan.

            System.Diagnostics.Debugger.Log(0, "LoanCreated",
                String.Format("Received Create notification for Loan {0} by User {1}",
                    notification.ResourceID, notification.UserID
                ));
        }
    }
}
