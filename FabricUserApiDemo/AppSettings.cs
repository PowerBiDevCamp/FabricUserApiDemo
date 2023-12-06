
namespace FabricUserApiDemo {

  class AppSettings {

    public const string FabricUserApiBaseUrl = "https://api.fabric.microsoft.com/v1";

    // Azure AD public client application metadata for user authentication
    public const string ApplicationId = "11111111-1111-1111-1111-111111111111";
    public const string RedirectUri = "http://localhost";

    // add Capacity Id for Premium capacity
    public const string PremiumCapacityId = "22222222-2222-2222-2222-222222222222";

    // Add Azure AD object Ids for 2 users, a group and a service principal for testing role assignments
    public const string TestUser1Id = "33333333-3333-3333-3333-333333333333";
    public const string TestUser2Id = "44444444-4444-4444-4444-444444444444";
    public const string TestADGroup1 = "55555555-5555-5555-5555-555555555555";
    public const string ServicePrincipalObjectId = "66666666-6666-6666-6666-666666666666";

    // these constants point to folders paths in the project folder where files are created
    public const string LocalTemplatesFolder = @"..\..\..\ExportedDefinitions\";
    public const string ExportToFilePath = @"..\..\..\ExportedJson\";
    public const string LocalWebPageFolder = @"..\..\..\WebPages\";

    // full support for running as a service principal using confidential application not supported in Public preview
    public const bool RunAsServicePrincipal = false;
    public const string TenantId = "";
    public const string ConfidentialApplicationId = "";
    public const string ConfidentialApplicationSecret = "";
    public const string TenantSpecificAuthority = "https://login.microsoftonline.com/" + TenantId;

  }



}
