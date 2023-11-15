
namespace FabricUserApiDemo.Models {

  public class PowerBiPermissionScopes {

    public const string resourceUri = "https://analysis.windows.net/powerbi/api";

    public static readonly string[] Default = new string[] {
      "https://analysis.windows.net/powerbi/api/.default"
    };

    // these are the only permissions needed for using Power BI REST APIs
    public static readonly string[] TenantProvisioning = new string[] {
      "https://analysis.windows.net/powerbi/api/Workspace.ReadWrite.All",
      "https://analysis.windows.net/powerbi/api/Dataset.ReadWrite.All",
      "https://analysis.windows.net/powerbi/api/Report.ReadWrite.All" 
    };
  }

}
