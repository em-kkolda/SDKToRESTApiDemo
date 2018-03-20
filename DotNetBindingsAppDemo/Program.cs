using System;
using System.Collections.Generic;
using System.Configuration;
using Elli.Api.Base;
using Elli.Api.Loans.Api;
using Elli.Api.Loans.Client;
using Elli.Api.Loans.Model;
using Elli.Api.Loans.Pipeline.Api;
using Elli.Api.Loans.Pipeline.Model;

namespace DotNetBindingsAppDemo
{
    public class Program
    {
		static void Main(string[] args)
		{
			// Create the user credentials for the login
			var credentials = new UserCredential
			{
				IdentityType = IdentityType.Lender,
				InstanceId = ConfigurationManager.AppSettings["InstanceID"],
				UserName = ConfigurationManager.AppSettings["UserID"],
				Password = ConfigurationManager.AppSettings["Password"]
			};

			// Generate the access token for the API
			var accessToken = AccessToken.GetAccessToken(credentials);

			// Now invoke the Pipeline API to query based on LoanNumber
			LoanPipelineViewContract view = new LoanPipelineViewContract()
			{
				Filter = new LoanPipelineFilterContract()
				{
					Terms = new List<LoanPipelineFilterContractTerms>()
					{
						new LoanPipelineFilterContractTerms()
						{
							CanonicalName = "Loan.LoanNumber",
							Value = "1801EM000070",
							MatchType = "exact"
						}
					}
				}
			};

			// Invoke the Pipeline API to get the matching loans
			var pipelineClient = ApiClientProvider.GetApiClient<LoanPipelineApi>(accessToken);
			var items = pipelineClient.PipelineRequest(contract: view);

			// Check the count
			if (items.Count != 1)
			{
				Console.WriteLine("Loan number did not return a unique match");
				return;
			}

			// Loop over the result set, each of which has a loanGuid attribute
			string loanGuid = items[0].LoanGuid;

			// Create the Loan contract to update the new document signing date (Loan.ClosingDocument.DocumentSigningDate)
			LoanContract loan = new LoanContract()
			{
				ClosingDocument = new LoanContractClosingDocument()
				{
					DocumentSigningDate = DateTime.Today
				}
			};

            try
            {
                // Create the Loan API client
                var loanClient = ApiClientProvider.GetApiClient<LoansApi>(accessToken);
                loanClient.UpdateLoan(loanGuid, loanContract: loan);
                Console.WriteLine("Loan updated successfully");
            }
            catch (ApiException ex)
            {
                Console.WriteLine("Loan update failed with response "
                    + ex.ErrorContent);
            }

            // In the debugger, don't close the window right away
            if (System.Diagnostics.Debugger.IsAttached) Console.ReadLine();
		}
	}
}
