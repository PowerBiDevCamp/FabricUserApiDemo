using System.Diagnostics;
using FabricUserApiDemo.Models;

namespace FabricUserApiDemo.Services {

  public class CustomerTenantBuilder {

    public static void CreateCustomerTenant(string WorkspaceName) {

      Console.WriteLine("Provision a new Fabric customer tenant");
      FabricWorkspace workspace = FabricUserApi.CreateWorkspace(WorkspaceName, AppSettings.PremiumCapacityId);

      Console.WriteLine();
      Console.WriteLine("Mission complete");
      Console.WriteLine();

      Console.Write("Press ENTER to open workspace in the browser");
      Console.ReadLine();

      OpenWorkspaceInBrowser(workspace.id);

    }

    public static void CreateCustomerTenantWithUsers(string WorkspaceName) {

      Console.WriteLine("Provision a new Fabric custom tenant");
      FabricWorkspace workspace = FabricUserApi.CreateWorkspace(WorkspaceName, AppSettings.PremiumCapacityId, "Demo workspace");

      FabricUserApi.AddWorkspaceUser(workspace.id, AppSettings.TestUser1Id, WorkspaceRole.Admin);
      FabricUserApi.AddWorkspaceUser(workspace.id, AppSettings.TestUser2Id, WorkspaceRole.Viewer);
      FabricUserApi.AddWorkspaceGroup(workspace.id, AppSettings.TestADGroup1, WorkspaceRole.Member);
      FabricUserApi.AddWorkspaceServicePrincipal(workspace.id, AppSettings.ServicePrincipalObjectId, WorkspaceRole.Admin);

      FabricUserApi.ViewWorkspaceRoleAssignments(workspace.id);

      Console.WriteLine();
      Console.WriteLine("Mission complete");
      Console.WriteLine();

      Console.Write("Press ENTER to open workspace in the browser");
      Console.ReadLine();

      OpenWorkspaceInBrowser(workspace.id);

    }

    public static void CreateCustomerTenantWithImportedSalesModel(string WorkspaceName) {

      Console.WriteLine("Provision a new Fabric customer tenant with import-mode semantic model");
      FabricWorkspace workspace = FabricUserApi.CreateWorkspace(WorkspaceName);

      FabricItemCreateRequest modelCreateRequest =
        FabricItemTemplateManager.GetImportedSalesModelCreateRequest("Product Sales");


      var model = FabricUserApi.CreateItem(workspace.id, modelCreateRequest);

      Console.WriteLine(" - Preparing " + model.displayName + " semantic model");

      Console.WriteLine("   > Patching datasource credentials for semantic model");
      PowerBiUserApi.PatchAnonymousAccessWebCredentials(workspace.id, model.id);

      Console.Write("   > Refreshing semantic model");
      PowerBiUserApi.RefreshDataset(workspace.id, model.id);
      Console.WriteLine();

      FabricItemCreateRequest createRequestReport =
        FabricItemTemplateManager.GetSalesReportCreateRequest(model.id, "Product Sales");

      var report = FabricUserApi.CreateItem(workspace.id, createRequestReport);

      Console.WriteLine();
      Console.WriteLine("Customer tenant provisioning complete");
      Console.WriteLine();

      Console.Write("Press ENTER to open workspace in the browser");
      Console.ReadLine();

      WebPageGenerator.GenerateReportPageUserOwnsData(workspace.id, report.id);

      WebPageGenerator.GenerateReportPageAppOwnsData(workspace.id, report.id);

      OpenWorkspaceInBrowser(workspace.id);

    }

    public static void CreateCustomerTenantWithDirectLakeSalesModel(string WorkspaceName) {

      string LakehouseName = "sales";

      Console.WriteLine("Provision new customer tenant with Lakehouse");
      FabricWorkspace workspace = FabricUserApi.CreateWorkspace(WorkspaceName, AppSettings.PremiumCapacityId);

      FabricItem lakehouse = FabricUserApi.CreateLakehouse(workspace.id, LakehouseName);

      var notebookCreateRequest = FabricItemTemplateManager.GetNotebookCreateRequest(workspace.id, lakehouse);

      var notebook = FabricUserApi.CreateItem(workspace.id, notebookCreateRequest);

      FabricUserApi.RunNotebook(workspace.id, notebook);

      Console.Write(" - Getting SQL endpoint connection information");
      var sqlEndpoint = FabricUserApi.GetSqlEndpointForLakehouse(workspace.id, lakehouse.id);

      Console.WriteLine("   > Server: " + sqlEndpoint.server);
      Console.WriteLine("   > Database: " + sqlEndpoint.database);
      Console.WriteLine();

      var modelCreateRequest =
        FabricItemTemplateManager.GetDirectLakeSalesModelCreateRequest("Product Sales", sqlEndpoint.server, sqlEndpoint.database);

      var model = FabricUserApi.CreateItem(workspace.id, modelCreateRequest);

      Console.WriteLine(" - Preparing " + model.displayName + " semantic model");

      Console.WriteLine("   > Patching datasource credentials for semantic model");
      PowerBiUserApi.PatchDirectLakeDatasetCredentials(workspace.id, model.id);

      Console.Write("   > Refreshing semantic model");
      PowerBiUserApi.RefreshDataset(workspace.id, model.id);
      Console.WriteLine();

      FabricItemCreateRequest createRequestReport =
        FabricItemTemplateManager.GetSalesReportCreateRequest(model.id, "Product Sales");

      var report = FabricUserApi.CreateItem(workspace.id, createRequestReport);

      Console.WriteLine();
      Console.WriteLine("Customer tenant provisioning complete");
      Console.WriteLine();

      Console.Write("Press ENTER to open workspace in the browser");
      Console.ReadLine();

      WebPageGenerator.GenerateReportPageUserOwnsData(workspace.id, report.id);

      OpenWorkspaceInBrowser(workspace.id);

    }

    public static void CreateCustomerTenantFromFolder(string TenantName, string TemplateFolder) {

      string WorkspaceName = TenantName;

      FabricWorkspace workspace = FabricUserApi.CreateWorkspace(WorkspaceName);

      List<FabricItemCreateRequest> createRequests = FabricItemTemplateManager.GetItemCreateRequests(TemplateFolder);

      foreach (var createRequest in createRequests) {
        FabricItem newItem = FabricUserApi.CreateItem(workspace.id, createRequest);
      }

      OpenWorkspaceInBrowser(workspace.id);

    }

    private static void OpenWorkspaceInBrowser(string WorkspaceId) {

      string url = "https://app.powerbi.com/groups/" + WorkspaceId;

      var process = new Process();
      process.StartInfo = new ProcessStartInfo(@"C:\Program Files\Google\Chrome\Application\chrome.exe");
      process.StartInfo.Arguments = url + " --profile-directory=\"Profile 1\" ";
      process.Start();

    }

    // testing - throw away
    public static void CreateCustomerTenantWithAllTypes(string WorkspaceName) {

      Console.WriteLine("Provision a new Fabric customer tenant");
      FabricWorkspace workspace = FabricUserApi.CreateWorkspace(WorkspaceName, AppSettings.PremiumCapacityId);

      foreach (string ItemType in FabricItemType.AllTypes) {

        FabricItemCreateRequest createRequest = new FabricItemCreateRequest {
          displayName = "Test " + ItemType,
          type = ItemType
        };

        try {
          FabricUserApi.CreateItem(workspace.id, createRequest);
          Console.WriteLine(ItemType + " created");
        }
        catch (Exception ex) {
          Console.WriteLine("Error creating " + ItemType);
        }

      }

      Console.WriteLine();
      Console.WriteLine("Mission complete");
      Console.WriteLine();

      Console.Write("Press ENTER to open workspace in the browser");
      Console.ReadLine();

      OpenWorkspaceInBrowser(workspace.id);

    }


  }

}
