using System;
using System.Configuration;
using EllieMae.Encompass.Client;
using EllieMae.Encompass.BusinessObjects.Loans;
using EllieMae.Encompass.Query;
using EllieMae.Encompass.Collections;

namespace SDKDemo
{
	class Program
	{
		/// <summary>
		/// Entry point for the SDK Application
		/// </summary>
		static void Main(string[] args)
		{
			// Start the Encompass Session using a user's credentials
			Session session = new Session();
			session.Start(
				String.Format("https://{0}.ea.elliemae.net${0}", ConfigurationManager.AppSettings["InstanceID"]),
				ConfigurationManager.AppSettings["UserID"],
				ConfigurationManager.AppSettings["Password"]
			);

			// Query for a Loan
			StringFieldCriterion cri = new StringFieldCriterion()
			{
				FieldName = "Loan.LoanNumber",
				Value = "1801EM000070"
			};
			
			// Run the query to get the Loan Identifier
			LoanIdentityList loanIds = session.Loans.Query(cri);

			if (loanIds.Count != 1)
			{
				Console.WriteLine("Loan number did not return a unique match");
				return;
			}

			// Open and lock the Loan
			string loanGuid = loanIds[0].Guid;

			using (Loan loan = session.Loans.Open(loanGuid, true, true))
			{
				loan.Fields["1887"].Value = DateTime.Today;
				loan.Commit();
			}

			// Close the session
			session.End();
		}
	}
}
