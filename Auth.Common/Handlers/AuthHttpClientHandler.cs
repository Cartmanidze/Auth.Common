using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Auth.Common.Configurations;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Auth.Common.Handlers
{
    public class AuthHttpClientHandler : HttpClientHandler
    {
        private const string AuthorizationHeaderKey = "Authorization";

        private const string BearerHeader = "Bearer";

        private readonly IHttpContextAccessor _httpContextAccessor;

        private readonly TokenConfiguration _tokenSettings;

        public AuthHttpClientHandler(IHttpContextAccessor httpContextAccessor,
            IOptions<TokenConfiguration> tokenSettings)
        {
            ServerCertificateCustomValidationCallback = (_, _, _, _) => true;
            CheckCertificateRevocationList = false;

            _httpContextAccessor = httpContextAccessor;
            _tokenSettings = tokenSettings.Value;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var authHeader = request.Headers.Authorization;

            if (authHeader == null)
            {
                AddAuthorizationHeader(request);
            }

            return await base.SendAsync(request, cancellationToken);
        }

        private void AddAuthorizationHeader(HttpRequestMessage request)
        {
            if (_httpContextAccessor.HttpContext != null && _httpContextAccessor.HttpContext.Request.Headers.TryGetValue(AuthorizationHeaderKey, out var value))
            {
                var token = value.ToString().Split(" ").LastOrDefault();
                request.Headers.Authorization = new AuthenticationHeaderValue(BearerHeader, token);
            }
            else if (!string.IsNullOrWhiteSpace(_tokenSettings.Value))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue(BearerHeader, _tokenSettings.Value);
            }
        }
    }
}
