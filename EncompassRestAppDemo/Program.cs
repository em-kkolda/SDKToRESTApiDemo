using System;
using System.Configuration;
using System.Threading.Tasks;
using EncompassRest;
using EncompassRest.Filters;
using EncompassRest.LoanPipeline;
using EncompassRest.Loans;

namespace EncompassRestAppDemo
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // Generate the access token and client for the API
            using (var client = await EncompassRestClient.CreateFromUserCredentialsAsync(
                new ClientParameters(
                    ConfigurationManager.AppSettings["ApiClientID"],
                    ConfigurationManager.AppSettings["ApiClientSecret"]),
                ConfigurationManager.AppSettings["InstanceID"],
                ConfigurationManager.AppSettings["UserID"],
                ConfigurationManager.AppSettings["Password"]))
            {
                // Invoke the Pipeline API to get the matching loans
                var items = await client.Pipeline.ViewPipelineAsync(
                    new PipelineParameters(
                        new StringFieldFilter(
                            CanonicalLoanField.LoanNumber,
                            StringFieldMatchType.Exact,
                            "1801EM000070")));

                // Check the count
                if (items.Count != 1)
                {
                    Console.WriteLine("Loan number did not return a unique match");
                    return;
                }

                // Loop over the result set, each of which has a loanGuid attribute
                var loanGuid = items[0].LoanGuid;

                // Create the Loan contract to update the new document signing date (Loan.ClosingDocument.DocumentSigningDate)
                var loan = new Loan(client, loanGuid);
                if (bool.Parse(ConfigurationManager.AppSettings["UseFieldID"]))
                {
                    loan.Fields["1887"].Value = DateTime.Today;
                }
                else
                {
                    loan.ClosingDocument.DocumentSigningDate = DateTime.Today;
                }

                try
                {
                    await client.Loans.UpdateLoanAsync(loan);
                    Console.WriteLine("Loan updated successfully");
                }
                catch (EncompassRestException ex)
                {
                    Console.WriteLine("Loan update failed with response "
                        + ex.ResponseContent);
                }

                // In the debugger, don't close the window right away
                if (System.Diagnostics.Debugger.IsAttached) Console.ReadLine();
            }
        }
    }
}
