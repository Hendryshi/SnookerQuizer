using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using SnookerQuizer.Model;
using Serilog;

namespace SnookerQuizer.Helper
{
	public class HttpHelper
	{
		public static List<T> GetData<T>(string url, string urlParam)
		{
			List<T> result = null;
			HttpClient client = new HttpClient();
			try
			{
				client.BaseAddress = new Uri(url);
				client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

				// List data response.
				HttpResponseMessage response = client.GetAsync(urlParam).Result;

				if (response.IsSuccessStatusCode)
					result = response.Content.ReadAsAsync<IEnumerable<T>>().Result.ToList();
				else
					Log.Error("{0} ({1})", (int)response.StatusCode, response.ReasonPhrase);

				return result;
			}
			catch(Exception ex)
			{
				Log.Fatal(string.Format("Error when getting data from url: [{0}], urlParam: [{1}]", url, urlParam), ex);
				return result;
			}
			finally
			{
				client.Dispose();
			}
		}
	}
}
