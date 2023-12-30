using IdentityServer4;
using IdentityServer4.Models;

namespace ChatApp.IdentityServer
{
    public class Config
    {
        public static IEnumerable<IdentityResource> GetIdentityResources() =>
            new List<IdentityResource>
            {
            new IdentityResources.OpenId(),
            new IdentityResources.Profile(),
                // Dodaj inne zasoby tożsamości według potrzeb
            };

        public static IEnumerable<ApiScope> GetApiScopes() =>
            new List<ApiScope>
            {
            new ApiScope("api1", "My API")
            };

        public static IEnumerable<Client> GetClients() =>
            new List<Client>
            {
            new Client
            {
                ClientId = "spa",
                ClientName = "SPA Client",
                AllowedGrantTypes = GrantTypes.Code,
                RequirePkce = true,
                RequireClientSecret = false,
                RedirectUris = { "http://localhost:3000/callback" }, // Dostosuj do swojego SPA
                PostLogoutRedirectUris = { "http://localhost:3000/" },
                AllowedCorsOrigins = { "http://localhost:3000" },
                AllowedScopes = { "openid", "profile", "api1" }, // Dostosuj do swojego API
                AllowAccessTokensViaBrowser = true
            }
            };
    }
}
