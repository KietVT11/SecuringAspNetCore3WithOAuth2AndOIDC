using Duende.IdentityServer;
using Duende.IdentityServer.Models;

namespace Marvin.IDP;

public static class Config
{
	public static IEnumerable<IdentityResource> IdentityResources =>
		new IdentityResource[]
		{
			new IdentityResources.OpenId(),
			new IdentityResources.Profile()
		};

	public static IEnumerable<ApiScope> ApiScopes =>
		new ApiScope[]
			{ };

	public static IEnumerable<Client> Clients =>
		new Client[]
			{
				new Client
				{
					ClientName = "Image Gallery",
					ClientId = "imagegalleryclient",
					AllowedGrantTypes = GrantTypes.Code,
					RedirectUris =
					{
						"https://localhost:44389/signin-oidc"
					},
					PostLogoutRedirectUris =
					{
						"https://localhost:44389/signout-callback-oidc"
					},
					AllowedScopes =
					{
						IdentityServerConstants.StandardScopes.OpenId,
						IdentityServerConstants.StandardScopes.Profile
					},
					ClientSecrets =
					{
						new Secret("secret".Sha256())
					},
					RequireConsent = true

				}
			};
}