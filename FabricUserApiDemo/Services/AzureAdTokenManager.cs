using FabricUserApiDemo.Models;
using Microsoft.Identity.Client;
using System.Reflection;
using static System.Formats.Asn1.AsnWriter;


namespace FabricUserApiDemo.Services {

  class AzureAdTokenManager {

    public const string urlPowerBiServiceApiRoot = "https://api.powerbi.com/";
    private const string tenantCommonAuthority = "https://login.microsoftonline.com/organizations";

    // Azure AD Application Id for user authentication
    private static string applicationId = AppSettings.ApplicationId;
    private static string redirectUri = AppSettings.RedirectUri;

    // Azure AD Application Id for service principal authentication
    private static string confidentialApplicationId = AppSettings.ConfidentialApplicationId;
    private static string confidentialApplicationSecret = AppSettings.ConfidentialApplicationSecret;
    private static string tenantSpecificAuthority = AppSettings.TenantSpecificAuthority;

    public static string GetAccessTokenInteractive(string[] scopes) {

      // create new public client application
      var appPublic = PublicClientApplicationBuilder.Create(applicationId)
                    .WithAuthority(tenantCommonAuthority)
                    .WithRedirectUri(redirectUri)
                    .Build();

      AuthenticationResult authResult = appPublic.AcquireTokenInteractive(scopes).ExecuteAsync().Result;

      // return access token to caller
      return authResult.AccessToken;
    }

    public static string GetAccessToken(string[] scopes) {

      // create new public client application
      var appPublic = PublicClientApplicationBuilder.Create(applicationId)
                      .WithAuthority(tenantCommonAuthority)
                      .WithRedirectUri(redirectUri)
                      .Build();

      // connect application to token cache
      TokenCacheHelper.EnableSerialization(appPublic.UserTokenCache);

      AuthenticationResult authResult;
      try {
        // try to acquire token from token cache
        var user = appPublic.GetAccountsAsync().Result.FirstOrDefault();
        authResult = appPublic.AcquireTokenSilent(scopes, user).ExecuteAsync().Result;
      }
      catch {
        authResult = appPublic.AcquireTokenInteractive(scopes).ExecuteAsync().Result;
      }

      // return access token to caller
      return authResult.AccessToken;
    }

    public static string GetAccessToken() {
      return GetAccessToken(FabricPermissionScopes.TenantProvisioning);
    }

    static class TokenCacheHelper {

      private static readonly string CacheFilePath = Assembly.GetExecutingAssembly().Location + ".tokencache.json";
      private static readonly object FileLock = new object();

      public static void EnableSerialization(ITokenCache tokenCache) {
        tokenCache.SetBeforeAccess(BeforeAccessNotification);
        tokenCache.SetAfterAccess(AfterAccessNotification);
      }

      private static void BeforeAccessNotification(TokenCacheNotificationArgs args) {
        lock (FileLock) {
          // repopulate token cache from persisted store
          args.TokenCache.DeserializeMsalV3(File.Exists(CacheFilePath) ? File.ReadAllBytes(CacheFilePath) : null);
        }
      }

      private static void AfterAccessNotification(TokenCacheNotificationArgs args) {
        // if the access operation resulted in a cache update
        if (args.HasStateChanged) {
          lock (FileLock) {
            // write token cache changes to persistent store
            File.WriteAllBytes(CacheFilePath, args.TokenCache.SerializeMsalV3());
          }
        }
      }
    }

    public static string GetAccessTokenForServicePrincipal(string[] Scopes) {

      var appConfidential = 
        ConfidentialClientApplicationBuilder.Create(confidentialApplicationId)
          .WithClientSecret(confidentialApplicationSecret)
          .WithAuthority(tenantSpecificAuthority)
          .Build();

      var authResult = appConfidential.AcquireTokenForClient(Scopes)
                                      .ExecuteAsync().Result;
      
      return authResult.AccessToken;
    }


  }


}
