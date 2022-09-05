using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using Microsoft.Azure.Functions.Worker;
using wl= Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Azure.Functions.Worker.Http;
using Newtonsoft.Json.Linq;

namespace IsolatedFunctionApp.AuthenticationAzureAd
{
    public class Function1
    {
        private readonly ILogger _logger;

        public Function1(ILoggerFactory loggerFactory, IConfiguration configuration)
        {
            _logger = loggerFactory.CreateLogger<Function1>();
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        [Function("Function1")]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post")] wl.HttpRequestData req)
        {
            string token = "eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiIsIng1dCI6IjJaUXBKM1VwYmpBWVhZR2FYRUpsOGxWMFRPSSIsImtpZCI6IjJaUXBKM1VwYmpBWVhZR2FYRUpsOGxWMFRPSSJ9.eyJhdWQiOiJhMDcyOTEwYi04ZWViLTQ2MzYtYmI3Ny1hNDEzZmI3Zjg1MzIiLCJpc3MiOiJodHRwczovL3N0cy53aW5kb3dzLm5ldC8wNmUyNzc1ZS05ZDNkLTQ5ZGUtYWQzNi1kYTgyZTI5NWZhNjcvIiwiaWF0IjoxNjYyNDA2MjI2LCJuYmYiOjE2NjI0MDYyMjYsImV4cCI6MTY2MjQxMDEyNiwiYWlvIjoiRTJaZ1lDamcrUGlpTTlFK2Z0WWV2VHRNUmlmS0FRPT0iLCJhcHBpZCI6IjhlMzNhOWNjLWJjNTUtNDM1ZC1iYzQ3LTU4YmVkMDIzZDlmMyIsImFwcGlkYWNyIjoiMSIsImlkcCI6Imh0dHBzOi8vc3RzLndpbmRvd3MubmV0LzA2ZTI3NzVlLTlkM2QtNDlkZS1hZDM2LWRhODJlMjk1ZmE2Ny8iLCJvaWQiOiI2NWQzNzI5MS1hNjZkLTQ0NzgtOWU4Ny01NzY4NGRiZTVjODMiLCJyaCI6IjAuQVNzQVhuZmlCajJkM2ttdE50cUM0cFg2Wnd1UmNxRHJqalpHdTNla0VfdF9oVElyQUFBLiIsInJvbGVzIjpbIkRlbW8iXSwic3ViIjoiNjVkMzcyOTEtYTY2ZC00NDc4LTllODctNTc2ODRkYmU1YzgzIiwidGlkIjoiMDZlMjc3NWUtOWQzZC00OWRlLWFkMzYtZGE4MmUyOTVmYTY3IiwidXRpIjoiOHhPN3pDRjAzRUdaLVpLNHpPaF9BQSIsInZlciI6IjEuMCJ9.mHJJAmNkapYbLsSSkwYN9JiTEJh6NP04elA08lFEonO7BuD3Ry5EfNrXcUdnpCsZ14_pA63KD8KP1Whmkd5fTEHXdf7Pa8tdhEKr4eoqKTGMLF6V0fAykgKq6YXC3-QLsmxP2kEyPlWNOUbtq1JB0hzy4VCSNE_9AIWiLL-1gHvLrLnEpRQKm2uzyrJYZ_VX6jFhI1XU-Y6y3iVepXiJYG5LDLEVi1V3eyVPkyQHGuXO04clfcGxNHF8i6-Bawd34p3bFoPinu_BdwL9GXZIYCvB6mvC_0g-t_--83LzBFWyU2MfQzTCS9NqprMrSkhKa18XRXkDpZZG0QErnrgNVA";
            
            try
            {
                //var authority = $"{Configuration["AzureAd:Instance"]}/{Configuration["AzureAd:TenantId"]}";
                var authority = "https://login.microsoftonline.com/06e2775e-9d3d-49de-ad36-da82e295fa67/";
                var audience = Configuration["AzureAd:ClientId"];
                var _tokenValidator = new JwtSecurityTokenHandler();
                var _tokenValidationParameters = new TokenValidationParameters
                {
                    ValidAudience = audience
                };
                var _configurationManager = new ConfigurationManager<OpenIdConnectConfiguration>(
                    $"{authority}/.well-known/openid-configuration",
                    new OpenIdConnectConfigurationRetriever());
                var validationParameters = _tokenValidationParameters.Clone();
                var openIdConfig = await _configurationManager.GetConfigurationAsync(default);
                validationParameters.ValidIssuer = openIdConfig.Issuer;
                validationParameters.IssuerSigningKeys = openIdConfig.SigningKeys;



                // Validate token
                var principal = _tokenValidator.ValidateToken(
                        token, validationParameters, out _);

                // Set principal + token in Features collection
                // They can be accessed from here later in the call chain
            }
            catch (SecurityTokenException)
            {
                // Token is not valid (expired etc.)
               return req.CreateResponse(HttpStatusCode.Unauthorized);
            }




            return req.CreateResponse(HttpStatusCode.OK);
        }
    }
}
