using System;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Formatting;

namespace RESTAppDemo
{
	/// <summary>
	/// Provides extension methods on the HttpClient class
	/// </summary>
	public static class HttpClientExtensions
	{
		public static Task<HttpResponseMessage> PatchAsJsonAsync<T>(this HttpClient client, string requestUri, T value)
		{
			if (client == null) throw new ArgumentNullException(nameof(client));
			if (value == null) throw new ArgumentNullException(nameof(value));

			var content = new ObjectContent<T>(value, new JsonMediaTypeFormatter());
			var request = new HttpRequestMessage(new HttpMethod("PATCH"), requestUri) { Content = content };

			return client.SendAsync(request);
		}
	}
}
