using Duende.IdentityServer;
using Duende.IdentityServer.Models;

namespace DsShop.IdentityServer.Configuration;

public class IdentityConfiguration
{
    public const string Admin = "Admin";
    public const string Client = "Client";

    public static IEnumerable<IdentityResource> IdentityResources =>
        new List<IdentityResource>
        {
            new IdentityResources.OpenId(),
            new IdentityResources.Email(),
            new IdentityResources.Profile()
        };

    public static IEnumerable<ApiScope> ApiScopes =>
        new List<ApiScope>
        {
            new ApiScope("dsshop", "DsShop Server"),
            new ApiScope(name: "read", "Read Data."),
            new ApiScope(name: "write", "Write Data."),
            new ApiScope(name: "delete", "Delete Data.")
        };

    public static IEnumerable<Client> Clients =>
    new List<Client>
    {
            //cliente genérico
            new Client
            {
                ClientId = "client",
                ClientSecrets = { new Secret("abracadabra#simsalabim".Sha256())},
                AllowedGrantTypes = GrantTypes.ClientCredentials, //precisa das credenciais do usuário
                AllowedScopes = { "read", "write", "profile" }
            },
            new Client
            {
                ClientId = "dsshop",
                ClientSecrets = { new Secret("abracadabra#simsalabim".Sha256())},
                AllowedGrantTypes = GrantTypes.Code, //via codigo
                RedirectUris = {"https://localhost:7239/signin-oidc"},//login
                PostLogoutRedirectUris = {"https://localhost:7239/signout-callback-oidc"},//logout
                AllowedScopes = new List<string>
                {
                    IdentityServerConstants.StandardScopes.OpenId,
                    IdentityServerConstants.StandardScopes.Profile,
                    IdentityServerConstants.StandardScopes.Email,
                    "dsshop"
                }
            }
    };
}
