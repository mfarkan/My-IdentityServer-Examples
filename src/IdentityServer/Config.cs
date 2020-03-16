using IdentityModel;
using IdentityServer4.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityServer
{
    public static class Config
    {
        public static IEnumerable<IdentityResource> GetIdentityResources() =>
            new List<IdentityResource>
            {
                new  IdentityResources.OpenId(),
                new  IdentityResources.Profile(),
                new IdentityResource
                {
                    Name="rc.scope",
                    UserClaims =
                    {
                        "basic.claim"
                    }
                }
            };
        public static IEnumerable<ApiResource> GetApis() => new List<ApiResource> { new ApiResource("apiOne"),
            new ApiResource("apiTwo") };
        public static IEnumerable<Client> GetClients() => new List<Client> {
            new Client{
            // this is the client information about the requesting for token these infos.
            ClientId = "clientId",
            ClientSecrets = { new Secret("client_secret_".ToSha256())},
            AllowedGrantTypes = GrantTypes.ClientCredentials, // this is the retrive the access_token
            AllowedScopes = {"apiOne" }
            },
            new Client
            {
                ClientId="Client_Id_Mvc",
                ClientSecrets={new Secret("client_secret_mvc".ToSha256())},
                AllowedGrantTypes=GrantTypes.Code,
                RequireConsent=false,
                AllowedScopes=
                {
                    IdentityServer4.IdentityServerConstants.StandardScopes.OpenId,
                    "apiOne",
                    "apiTwo",
                    "rc.scope",
                    IdentityServer4.IdentityServerConstants.StandardScopes.Profile
                },
                // puts all the claims id_token.
                //AlwaysIncludeUserClaimsInIdToken=true,
                RedirectUris={ "https://localhost:5001/signin-oidc" }
            }
        };
    }
}
