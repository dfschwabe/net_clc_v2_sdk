﻿using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace CenturyLinkCloudSdk.Runtime.Client
{
    public class AuthenticationHandler : DelegatingHandler
    {
        private readonly ITokenProvider _tokenProvider;

        public AuthenticationHandler(ITokenProvider tokenProvider)
        {
            _tokenProvider = tokenProvider;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var token = await _tokenProvider.GetBearerToken();
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            return await base.SendAsync(request, cancellationToken);
        }
    }
}