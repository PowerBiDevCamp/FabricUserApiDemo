using FabricUserApiDemo.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;

using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FabricUserApiDemo.Services {

  public class FabricUserApi {

    #region "Utility methods for executing HTTP requests"

    private static string AccessToken = AzureAdTokenManager.GetAccessToken(FabricPermissionScopes.TenantProvisioning);

    private static string ExecuteGetRequest(string endpoint) {

      string restUri = AppSettings.FabricUserApiBaseUrl + endpoint;

      HttpClient client = new HttpClient();
      client.DefaultRequestHeaders.Add("Authorization", "Bearer " + AccessToken);
      client.DefaultRequestHeaders.Add("Accept", "application/json");

      HttpResponseMessage response = client.GetAsync(restUri).Result;

      if (response.IsSuccessStatusCode) {
        return response.Content.ReadAsStringAsync().Result;
      }
      else {
        throw new ApplicationException("ERROR executing HTTP GET request " + response.StatusCode);
      }
    }

    private static string ExecutePostRequest(string endpoint, string postBody = "") {

      string restUri = AppSettings.FabricUserApiBaseUrl + endpoint;

      HttpContent body = new StringContent(postBody);
      body.Headers.ContentType = new MediaTypeWithQualityHeaderValue("application/json");

      HttpClient client = new HttpClient();
      client.DefaultRequestHeaders.Add("Accept", "application/json");
      client.DefaultRequestHeaders.Add("Authorization", "Bearer " + AccessToken);

      HttpResponseMessage response = client.PostAsync(restUri, body).Result;

      // switch to handle responses with different status codes
      switch (response.StatusCode) {

        // handle case when sync call succeeds with OK (200) or CREATED (201)
        case HttpStatusCode.OK:
        case HttpStatusCode.Created:
          Console.WriteLine();
          // return result to caller
          return response.Content.ReadAsStringAsync().Result;

        // handle case where call started async operation with ACCEPTED (202)
        case HttpStatusCode.Accepted:
          Console.Write(".");

          // get headers in response with URL for operation status and retry interval
          string operationUrl = response.Headers.GetValues("Location").First();
          //int retryAfter = int.Parse(response.Headers.GetValues("Retry-After").First());
          int retryAfter = 10; // hard-coded during testing - use what's above instead 

          // execute GET request with operation url until it returns OK (200)
          string jsonOperation;
          FabricOperation operation;

          do {
            Thread.Sleep(retryAfter * 1000);  // wait for retry interval 
            Console.Write(".");
            response = client.GetAsync(operationUrl).Result;
            jsonOperation = response.Content.ReadAsStringAsync().Result;
            operation = JsonSerializer.Deserialize<FabricOperation>(jsonOperation);

          } while (response.StatusCode == HttpStatusCode.Accepted ||
                   response.StatusCode == HttpStatusCode.TooManyRequests ||
                   operation.status == "InProgress");


          if (response.StatusCode == HttpStatusCode.OK) {
            // handle 2 cases where operation completed successfully
            if (!response.Headers.Contains("Location")) {
              // (1) handle case where operation has no result
              Console.WriteLine();
              return string.Empty;
            }
            else {
              Console.Write(".");
              // (2) handle case where operation has result by retrieving it
              response = client.GetAsync(operationUrl + "/result").Result;
              Console.WriteLine();
              return response.Content.ReadAsStringAsync().Result;
            }
          }
          else {
            // handle case where operation experienced error
            jsonOperation = response.Content.ReadAsStringAsync().Result;
            operation = JsonSerializer.Deserialize<FabricOperation>(jsonOperation);
            string errorMessage = operation.error.errorCode + " - " + operation.error.message;
            throw new ApplicationException(errorMessage);
          }

        default: // handle exeception where HTTP status code indicates failure
          throw new ApplicationException("ERROR executing HTTP POST request " + response.StatusCode);
      }

    }

    private static string ExecutePatchRequest(string endpoint, string postBody = "") {

      string restUri = AppSettings.FabricUserApiBaseUrl + endpoint;

      HttpContent body = new StringContent(postBody);
      body.Headers.ContentType = new MediaTypeWithQualityHeaderValue("application/json");

      HttpClient client = new HttpClient();
      client.DefaultRequestHeaders.Add("Accept", "application/json");
      client.DefaultRequestHeaders.Add("Authorization", "Bearer " + AccessToken);

      HttpResponseMessage response = client.PatchAsync(restUri, body).Result;

      if (response.IsSuccessStatusCode) {
        return response.Content.ReadAsStringAsync().Result;
      }
      else {
        throw new ApplicationException("ERROR executing HTTP PATCH request " + response.StatusCode);
      }
    }

    private static string ExecuteDeleteRequest(string endpoint) {
      string restUri = AppSettings.FabricUserApiBaseUrl + endpoint;

      HttpClient client = new HttpClient();
      client.DefaultRequestHeaders.Add("Accept", "application/json");
      client.DefaultRequestHeaders.Add("Authorization", "Bearer " + AccessToken);
      HttpResponseMessage response = client.DeleteAsync(restUri).Result;

      if (response.IsSuccessStatusCode) {
        return response.Content.ReadAsStringAsync().Result;
      }
      else {
        throw new ApplicationException("ERROR executing HTTP DELETE request " + response.StatusCode);
      }
    }

    private static JsonSerializerOptions jsonOptions = new JsonSerializerOptions {
      DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
    };

    #endregion

    public static List<FabricWorkspace> GetWorkspaces() {
      string jsonResult = ExecuteGetRequest("/workspaces");
      return JsonSerializer.Deserialize<List<FabricWorkspace>>(jsonResult);
    }

    public static FabricWorkspace GetWorkspaceByName(string WorkspaceName) {

      string jsonResult = ExecuteGetRequest("/workspaces");
      List<FabricWorkspace> workspaces = JsonSerializer.Deserialize<List<FabricWorkspace>>(jsonResult);

      foreach (FabricWorkspace workspace in workspaces) {
        if (workspace.displayName.Equals(WorkspaceName)) {
          return workspace;
        }
      }

      return null;
    }

    public static FabricWorkspace CreateWorkspace(string WorkspaceName, string CapacityId = AppSettings.PremiumCapacityId, string Description = null) {

      FabricWorkspace workspace = GetWorkspaceByName(WorkspaceName);


      Console.WriteLine();
      Console.Write(" - Creating " + WorkspaceName + " workspace");

      // delete workspace with same name if it exists
      if (workspace != null) {
        ExecuteDeleteRequest("/workspaces/" + workspace.id);
        workspace = null;
      }

      var workspaceCreateRequest = new FabricWorkspaceCreateRequest {
        displayName = WorkspaceName,
        description = Description,
        capacityId = CapacityId
      };

      string requestBody = JsonSerializer.Serialize(workspaceCreateRequest, jsonOptions);

      string jsonResult = ExecutePostRequest("/workspaces", requestBody);

      workspace = JsonSerializer.Deserialize<FabricWorkspace>(jsonResult);

      Console.WriteLine("   > Workspace created with capacity id " + workspaceCreateRequest.capacityId);
      Console.WriteLine();

      return workspace;
    }

    public static FabricWorkspace UpdateWorkspace(string WorkspaceId, string WorkspaceName, string Description = null) {

      var workspaceUpdateRequest = new FabricWorkspaceUpdateRequest {
        displayName = WorkspaceName,
        description = Description
      };

      string requestBody = JsonSerializer.Serialize(workspaceUpdateRequest, jsonOptions);

      string jsonResult = ExecutePatchRequest("/workspaces/" + WorkspaceId, requestBody);

      FabricWorkspace workspace = JsonSerializer.Deserialize<FabricWorkspace>(jsonResult);

      Console.WriteLine("   > Workspace uodated wtith new name of " + workspace.displayName);
      Console.WriteLine();

      return workspace;
    }

    public static void AssignWorkspaceToCapacity(string WorkspaceId, string CapacityId) {

      Console.WriteLine(" - Assigning workspace to capacity with id of " + CapacityId);

      string restUrl = "/workspaces/" + WorkspaceId + "/assignToCapacity";

      string postBody = "{ \"capacityId\": \"" + CapacityId + "\" }";

      // this call returns async 202 ACCEPTED
      string jsonResult = ExecutePostRequest(restUrl, postBody);

    }

    public static void AddWorkspaceUser(string WorkspaceId, string UserId, string RoleAssignment) {
      Console.WriteLine(" - Adding workspace role assignment to user with id of " + UserId);

      string restUrl = "/workspaces/" + WorkspaceId + "/roleAssignments";

      FabricWorkspaceRoleAssignment roleAssignment =
        new FabricWorkspaceRoleAssignment {
          role = RoleAssignment,
          principal = new Principal {
            id = UserId,
            type = PrincipalType.User
          },
        };


      string postBody = JsonSerializer.Serialize(roleAssignment, jsonOptions);

      ExecutePostRequest(restUrl, postBody);

    }

    public static void AddWorkspaceGroup(string WorkspaceId, string GroupId, string RoleAssignment) {
      Console.WriteLine(" - Adding workspace role assignment to AAD Group with id of " + GroupId);

      string restUrl = "/workspaces/" + WorkspaceId + "/roleAssignments";

      FabricWorkspaceRoleAssignment roleAssignment =
        new FabricWorkspaceRoleAssignment {
          role = RoleAssignment,
          principal = new Principal {
            id = GroupId,
            type = PrincipalType.Group
          },
        };

      string postBody = JsonSerializer.Serialize(roleAssignment, jsonOptions);

      ExecutePostRequest(restUrl, postBody);

    }

    public static void AddWorkspaceServicePrincipal(string WorkspaceId, string ServicePrincipalObjectId, string RoleAssignment) {
      Console.WriteLine(" - Adding workspace role assignment to service principal with id of " + ServicePrincipalObjectId);

      string restUrl = "/workspaces/" + WorkspaceId + "/roleAssignments";

      FabricWorkspaceRoleAssignment roleAssignment =
        new FabricWorkspaceRoleAssignment {
          role = RoleAssignment,
          principal = new Principal {
            id = ServicePrincipalObjectId,
            type = PrincipalType.ServicePrincipal
          },
        };

      string postBody = JsonSerializer.Serialize(roleAssignment, jsonOptions);

      ExecutePostRequest(restUrl, postBody);

    }

    public static void ExportItemDefinitionsFromWorkspace(string WorkspaceName) {

      FabricItemTemplateManager.DeleteAllTemplateFiles(WorkspaceName);

      FabricWorkspace workspace = GetWorkspaceByName(WorkspaceName);
      string jsonResult = ExecuteGetRequest("/workspaces/" + workspace.id + "/items");

      List<FabricItem> items = JsonSerializer.Deserialize<List<FabricItem>>(jsonResult);

      //List<string> unsupportedItems = new List<string>() { "dashboard", "datamart", "rdlreport", "Lakehouse" };
      foreach (var item in items) {
        // if (!unsupportedItems.Contains(item.type)) {

        if(item.type != FabricItemType.Dashboard &&
           item.type != FabricItemType.PaginatedReport) {

          try {
            string jsonResultItem = ExecutePostRequest("/workspaces/" + workspace.id + "/items/" + item.id + "/getDefinition");
            FabricItemDefinitionResponse definitionResponse = JsonSerializer.Deserialize<FabricItemDefinitionResponse>(jsonResultItem);
            FabricItemDefinition definition = definitionResponse.definition;
            string targetFolder = item.displayName + "." + item.type;
            Console.WriteLine("Exporting " + targetFolder);
            foreach (var part in definition.parts) {
              FabricItemTemplateManager.WriteFile(WorkspaceName, targetFolder, part.path, part.payload);
            }
            var ItemMetadata = JsonSerializer.Serialize(new {
              type = item.type,
              displayName = item.displayName
            }, jsonOptions);

            FabricItemTemplateManager.WriteFile(WorkspaceName, targetFolder, "item.metadata.json", ItemMetadata, false);


          }
          catch (Exception ex) {
            Console.WriteLine(" *** Error exporting " + item.type + " named " + item.displayName);
          }

          Thread.Sleep(7000);

        }


        // slow up calls so it doesn't trigger throttleing for more than 10+ calls per minute

        //}
      }


    }

    public static FabricItem CreateItem(string WorkspaceId, FabricItemCreateRequest ItemCreateRequest) {

      string displayMessage = string.Format(" - Creating {0} {1}", ItemCreateRequest.displayName, ItemCreateRequest.type);
      Console.Write(displayMessage);


      string postBody = JsonSerializer.Serialize(ItemCreateRequest, jsonOptions);
      string jsonResponse = ExecutePostRequest("/workspaces/" + WorkspaceId + "/items", postBody);
      FabricItem newItem = JsonSerializer.Deserialize<FabricItem>(jsonResponse);

      Console.WriteLine("   > " + newItem.displayName + " " + newItem.type + " created with Id " + newItem.id);
      Console.WriteLine();

      // return new item object to caller
      return newItem;
    }

    public static FabricItem UpdateItem(string WorkspaceId, string ItemId, string ItemName, string Description = null) {
      // NOTE: UpdateItem API does not work in initial Public preview release 

      var itemUpdateRequest = new FabricItemUpdateRequest {
        displayName = ItemName,
        description = Description
      };

      string requestBody = JsonSerializer.Serialize(itemUpdateRequest, jsonOptions);

      string jsonResult = ExecutePatchRequest("/workspaces/" + WorkspaceId + "/items/" + ItemId, requestBody);

      FabricItem item = JsonSerializer.Deserialize<FabricItem>(jsonResult);

      Console.WriteLine("   > Item uodated wtith new name of " + item.displayName);
      Console.WriteLine();

      return item;

    }

    public static FabricItem CreateLakehouse(string WorkspaceId, string LakehouseName) {

      FabricItemCreateRequest createRequestLakehouse = new FabricItemCreateRequest {
        displayName = LakehouseName,
        type = "Lakehouse"
      };

      FabricItem lakehouse = CreateItem(WorkspaceId, createRequestLakehouse);

      return lakehouse;
    }

    public static FabricLakehouse GetLakehouse(string WorkspaceId, string LakehousId) {
      string jsonResponse = ExecuteGetRequest("/workspaces/" + WorkspaceId + "/lakehouses/" + LakehousId);
      FabricLakehouse lakehouse = JsonSerializer.Deserialize<FabricLakehouse>(jsonResponse);
      return lakehouse;
    }

    public static FabricSqlEndpoint GetSqlEndpointForLakehouse(string WorkspaceId, string LakehouseId) {

      var lakehouse = GetLakehouse(WorkspaceId, LakehouseId);

      while (lakehouse.properties.sqlEndpointProperties.provisioningStatus != "Success") {
        lakehouse = GetLakehouse(WorkspaceId, LakehouseId);
        Thread.Sleep(10000);
        Console.Write(".");
      }
      Console.WriteLine();

      return new FabricSqlEndpoint {
        server = lakehouse.properties.sqlEndpointProperties.connectionString,
        database = lakehouse.properties.sqlEndpointProperties.id
      };

    }

    public static void RunNotebook(string WorkspaceId, FabricItem Notebook) {
      Console.Write(" - Starting job to execute " + Notebook.displayName + " notebook");

      string restUrl = "/workspaces/" + WorkspaceId + "/items/" + Notebook.id + "/jobs/instances?jobType=RunNotebook";
      string jsonResponse = ExecutePostRequest(restUrl, "{ \"executionData\": {} }");

      Console.WriteLine("   > Notebook execution job completed");
      Console.WriteLine();
    }

    private static void SaveJsonAsFile(string FileName, string JsonText, bool OpenInNotepad = false) {

      string FilePath = AppSettings.ExportToFilePath + FileName;
      Console.WriteLine(" - Generating output file " + FileName);

      File.WriteAllText(FilePath, JsonText);

      if (OpenInNotepad) {
        Process.Start("notepad", FilePath);
      }
    }

    public static void ViewWorkspaces() {

      string jsonResult = ExecuteGetRequest("/workspaces");
      SaveJsonAsFile("workspaces.json", jsonResult);

      List<FabricWorkspace> workspaces = JsonSerializer.Deserialize<List<FabricWorkspace>>(jsonResult);

      foreach (FabricWorkspace workspace in workspaces) {
        Console.WriteLine(workspace.displayName);
      }

    }

    public static void ViewCapacities() {

      string jsonResult = ExecuteGetRequest("/capacities");

      List<FabricCapacity> capacities = JsonSerializer.Deserialize<List<FabricCapacity>>(jsonResult);

      foreach (var capacity in capacities) {
        Console.WriteLine(capacity.displayName + " : " + capacity.sku);
      }

    }

    public static void ViewWorkspaceRoleAssignments(string WorkspaceId) {
      string restUrl = "/workspaces/" + WorkspaceId + "/roleAssignments";
      string jsonResult = ExecuteGetRequest(restUrl);
      SaveJsonAsFile("WorkspaceMemebership.json", jsonResult);
    }

    public static void ViewWorkspaceItems(string WorkspaceName) {

      FabricWorkspace workspace = GetWorkspaceByName(WorkspaceName);
      string jsonResult = ExecuteGetRequest("/workspaces/" + workspace.id + "/items");

      List<FabricItem> items = JsonSerializer.Deserialize<List<FabricItem>>(jsonResult);

      foreach (var item in items) {
        Console.WriteLine(item.type + " - " + item.id + " - " + item.displayName);
      }


    }

    public static void ViewAllWorkspaceItems() {

      string jsonResult = ExecuteGetRequest("/workspaces");
      SaveJsonAsFile("workspaces.json", jsonResult);

      List<FabricWorkspace> workspaces = JsonSerializer.Deserialize<List<FabricWorkspace>>(jsonResult);

      foreach (FabricWorkspace workspace in workspaces) {
        jsonResult = ExecuteGetRequest("/workspaces/" + workspace.id + "/items");
        List<FabricItem> items = JsonSerializer.Deserialize<List<FabricItem>>(jsonResult);
        foreach (var item in items) {
          Console.WriteLine(item.type + " - " + item.displayName);
        }
        Thread.Sleep(6000);
      }

    }

    public static void ViewWorkspaceItemDefinitions(string WorkspaceName) {

      FabricWorkspace workspace = GetWorkspaceByName(WorkspaceName);
      string jsonResult = ExecuteGetRequest("/workspaces/" + workspace.id + "/items");

      List<FabricItem> items = JsonSerializer.Deserialize<List<FabricItem>>(jsonResult);

      List<string> unsupportedItems = new List<string>() { "dashboard", "datamart" };
      foreach (var item in items) {
        if (!unsupportedItems.Contains(item.type)) {
          string jsonResultItem = ExecutePostRequest("/workspaces/" + workspace.id + "/items/" + item.id + "/getDefinition");
          FabricItemDefinitionResponse definitionResponse = JsonSerializer.Deserialize<FabricItemDefinitionResponse>(jsonResultItem);
          FabricItemDefinition definition = definitionResponse.definition;
          Console.WriteLine(item.type + ": " + item.displayName);
          foreach (var part in definition.parts) {
            Console.WriteLine(" - " + part.path);
            byte[] bytes = Convert.FromBase64String(part.payload);
            string definitionText = Encoding.UTF8.GetString(bytes, 0, bytes.Length);
            Console.WriteLine(definitionText);
          }
        }
      }
    }
  }

}
