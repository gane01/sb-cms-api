using System.Net.Http.Headers;

namespace SbContentManager.ContentstackApi
{
    public class HttpHeaderHandler : DelegatingHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            /*
            if (request.Method == HttpMethod.Post)
            {
                request.Content!.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            }*/
            request.Headers.Add("api_key", "bltc35dce36417d764b");
            request.Headers.Add("access_token", "cs53aab06da08c2b6f4725c1d8");
            request.Headers.Authorization = new AuthenticationHeaderValue("cs64851cfb88c1be460d0d84e2");

            return await base.SendAsync(request, cancellationToken).ConfigureAwait(false);

        }
    }
}
