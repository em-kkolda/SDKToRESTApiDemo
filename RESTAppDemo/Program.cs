using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Net.Http;
using Newtonsoft.Json.Linq;

namespace RESTAppDemo
{
	class Program
	{
		/// <summary>
		/// Entry point for REST API Client Application
		/// </summary>
		static void Main(string[] args)
		{
			// Create the HttpClient to be used to make request calls
			HttpClient apiClient = new HttpClient();
			apiClient.BaseAddress = new Uri("https://api.elliemae.com");

			// Encode the OAuth Client ID and secret
			string authToken = Convert.ToBase64String(
				Encoding.UTF8.GetBytes(String.Format("{0}:{1}", 
                    ConfigurationManager.AppSettings["OAuthClientID"],
                    ConfigurationManager.AppSettings["OAuthSecret"])));

			var authParams = new Dictionary<string, string>
			{
				{ "grant_type", "password" },
				{ "username", String.Format("{0}@encompass:{1}", 
					ConfigurationManager.AppSettings["UserID"], 
					ConfigurationManager.AppSettings["InstanceID"]) },
				{ "password", ConfigurationManager.AppSettings["Password"] }
			}.ToList();

			// Authenticate with my OAuth credential using Resource Owner flow
			HttpRequestMessage authMsg = new HttpRequestMessage(HttpMethod.Post, "/oauth2/v1/token");
			authMsg.Headers.Add("Authorization", "Basic " + authToken);
			authMsg.Content = new FormUrlEncodedContent(authParams);

			// Invoke the Token endpoint to authenticate
			HttpResponseMessage msg = apiClient.SendAsync(authMsg).Result;
			dynamic resp = msg.Content.ReadAsAsync<dynamic>().Result;

			// Set the Authorization header for all subsequent calls using the returned access token
			apiClient.DefaultRequestHeaders.Add("Authorization",
				"Bearer " + resp.access_token);

			// Create the query criterion
			JObject cri = JObject.FromObject(new 
			{
				// Define the filter for the Loan Number
				filter = new
				{
					canonicalName = "Loan.LoanNumber",
					value = "1801EM000070",
					matchType = "exact"
				}
			});

			// Invoke the pipeline API
			msg = apiClient.PostAsJsonAsync("/encompass/v1/loanPipeline", cri).Result;
			resp = msg.Content.ReadAsAsync<dynamic>().Result;

			// Check the count
			if (resp.Count != 1)
			{
				Console.WriteLine("Loan number did not return a unique match");
				return;
			}

			// Loop over the result set, each of which has a loanGuid attribute
			string loanGuid = resp[0].loanGuid;

			// Create the Loan contract to update the new document signing date (Loan.closingDocument.documentSigningDate)
			JObject req = JObject.FromObject(new
			{
				closingDocument = new
				{
					documentSigningDate = DateTime.Today
				}
			});

			// Send the update request to the server
			msg = apiClient.PatchAsJsonAsync("/encompass/v1/loans/" + loanGuid, req).Result;

			if (msg.IsSuccessStatusCode)
            {
                Console.WriteLine("Loan updated successfully");
            }
            else
			{
				resp = msg.Content.ReadAsAsync<dynamic>().Result;
				Console.WriteLine("Loan update failed with response " + resp.details);
			}

            // In the debugger, don't close the window right away
            if (System.Diagnostics.Debugger.IsAttached) Console.ReadLine();
        }
    }
}
