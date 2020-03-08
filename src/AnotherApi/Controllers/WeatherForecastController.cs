using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using IdentityModel.Client;
namespace AnotherApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly IHttpClientFactory _factory;

        public WeatherForecastController(IHttpClientFactory factory)
        {
            _factory = factory;
        }
        public async Task<IActionResult> Index()
        {
            // retrieve access_token
            var client = _factory.CreateClient();

            var discoveryDocument = await client.GetDiscoveryDocumentAsync("http://localhost:16855/");

            var tokenResponse = await client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
            {
                Address = discoveryDocument.TokenEndpoint,
                ClientId = "clientId",
                Scope = "apiOne",
                ClientSecret = "client_secret_",
            });

            var apiClient = _factory.CreateClient();

            apiClient.SetBearerToken(tokenResponse.AccessToken);

            var response = await apiClient.GetAsync("http://localhost:33038/WeatherForecast/List");
            //retrieve secret_data

            var content = await response.Content.ReadAsStringAsync();

            return Ok(new
            {
                token = tokenResponse,
                message = content,
            });
        }
    }
}
