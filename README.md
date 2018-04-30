# SDKToRESTApiDemo
This solution provides four sample projects, three of which are meant to illustrate how code written 
against the Encompass SDK can be converted to the Encompass NG REST API, either using the native .NET HttpClient, using [Ellie Mae's .NET Language Bindings](https://github.com/EllieMae/developerconnect-dotnet-bindings), or using [EncompassRest](https://github.com/EncompassRest/EncompassRest). The four projects are:

* **SDKAppDemo:** A simple application based on the SmartClient-based Encompass SDK. The demo uses the Query capabilities of the SDK
to look up a Loan from the Encompass system, open that loan and then modify and save the loan.
* **RESTAppDemo:** A functionally-equivalent project that uses the .NET HttpClient object to interact with the Encompass NG Lending 
Platform's REST API. You will need an OAuth token and secret to use this code.
* **DotNetBindingsAppDemo:** A functionally-equivalent project that uses the Ellie Mae .NET Language Bindings to access the Encompass 
NG Lending Platform. The Language Bindings provide a convenient way build type-safe code that obscures much of the complexity of 
using a REST API.
* **EncompassRestAppDemo:** A functionally-equivalent project that uses EncompassRest to access the Encompass 
NG Lending Platform. EncompassRest provides a convenient way build type-safe code that obscures much of the complexity of 
using a REST API.

Finally, the solution includes a project named **WebhookReceiverDemo**, which provides a sample implementation of a WebAPI 2-based API 
that can receive wwebhook calls from the Encompass NG Lending Platform. Using webhooks, you can be notified of key business or data
events within your Encompass system, such as a Loan being created or modified.

In each project, you will need to modify the app.config or web.config file in order to run the code. Within the config file, look for
replacement variables which look like `_your_oauth_clientid` and replace the placeholder with the appropriate value for your Encompass
environment.
