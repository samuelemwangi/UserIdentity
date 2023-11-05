using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;

namespace UserIdentity.IntegrationTests.Presentation.Helpers
{
  internal static class SerDe
  {
    public static HttpContent ConvertToHttpContent<T>(T data)
    {
      var jsonQuery = JsonConvert.SerializeObject(data);
      HttpContent httpContent = new StringContent(jsonQuery, Encoding.UTF8);
      httpContent.Headers.Remove("content-type");
      httpContent.Headers.Add("content-type", "application/json; charset=utf-8");

      return httpContent;
    }

    public static T? Deserialize<T>(String jsonString)
    {
      return JsonConvert.DeserializeObject<T>(jsonString);
    }
  }
}
