using System.Net.Http;
using Microsoft.AspNetCore.Http;

namespace DMS.UI.Helper
{
    public static class HttpClientHelper
    {
        public static void AddBearerToken(HttpClient client, HttpContext httpContext)
        {
            var token = httpContext.Request.Cookies["jwtToken"];
            if (!string.IsNullOrEmpty(token))
            {
                client.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }
        }
    }
}
