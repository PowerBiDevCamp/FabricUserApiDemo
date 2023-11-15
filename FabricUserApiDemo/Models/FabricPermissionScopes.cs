using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FabricUserApiDemo.Models {

  public class FabricPermissionScopes {

    public const string resourceUri = "https://api.fabric.microsoft.com/";

    // used for user token acquisition
    public static readonly string[] TenantProvisioning = new string[] {
      "https://api.fabric.microsoft.com/Capacity.ReadWrite.All",
      "https://api.fabric.microsoft.com/Workspace.ReadWrite.All",
      "https://api.fabric.microsoft.com/Item.ReadWrite.All",
      "https://api.fabric.microsoft.com/Item.Read.All",
      "https://api.fabric.microsoft.com/Item.Execute.All",
      "https://api.fabric.microsoft.com/Content.Create",
      "https://api.fabric.microsoft.com/Dataset.ReadWrite.All ",
      "https://api.fabric.microsoft.com/Report.ReadWrite.All",
    };

    // used for service principal token acquisition
    public static readonly string[] Default = new string[] {
      "https://api.fabric.microsoft.com/.default"
    };

    // not used in this sample application
    public static readonly string[] AdminScopes = new string[] {
      "https://api.fabric.microsoft.com/Capacity.ReadWrite.All",
      "https://api.fabric.microsoft.com/Workspace.ReadWrite.All",
      "https://api.fabric.microsoft.com/Item.ReadWrite.All",
      "https://api.fabric.microsoft.com/Content.Create",
      "https://api.fabric.microsoft.com/Dashboard.ReadWrite.All",
      "https://api.fabric.microsoft.com/Dataflow.ReadWrite.All ",
      "https://api.fabric.microsoft.com/Dataset.ReadWrite.All ",
      "https://api.fabric.microsoft.com/Report.ReadWrite.All",
      "https://api.fabric.microsoft.com/UserState.ReadWrite.All",
      "https://api.fabric.microsoft.com/Tenant.ReadWrite.All"
    };

  }
}




