using System;
using System.Text;
using System.Configuration;
using System.Security.Cryptography;
using Elli.Api.Base;


namespace WebhookReceiverDemo
{
    // Helper functions for the Encompass REST API
    public static class EncompassApi
    {

        // Validates a Webhook signature
        public static bool ValidateSignature(string signature, byte[] msgBody)
        {
            // The EM Servers use HMAC to digitally sign the message
            var emKey = Encoding.ASCII.GetBytes(ConfigurationManager.AppSettings["EMHMACKey"]);
            var hmac = new HMACSHA256(emKey);

            // Compute the hash of the message body
            var hash = Convert.ToBase64String(hmac.ComputeHash(msgBody));

            // Compare the hash to the signature provided
            return hash == signature;
        }

    }
}