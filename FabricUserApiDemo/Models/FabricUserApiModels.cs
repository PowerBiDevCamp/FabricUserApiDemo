
namespace FabricUserApiDemo.Models {

  public class FabricWorkspaceList {
    public List<FabricWorkspace> value { get; set; }
  }

  public class FabricWorkspace {
    public string id { get; set; }
    public string displayName { get; set; }
    public string description { get; set; }
    public string type { get; set; }
    public string capacityId { get; set; }
  } 

  public class FabricWorkspaceCreateRequest {
    public string displayName { get; set; }
    public string capacityId { get; set; }
    public string description { get; set; }
  }

  public class FabricWorkspaceUpdateRequest {
    public string displayName { get; set; }
    public string description { get; set; }
  }

  public class FabricCapacity {
    public string id { get; set; }
    public string displayName { get; set; }
    public string sku { get; set; }
    public string region { get; set; }
    public string state { get; set; }
  }

  public class FabricCapacityList {
    public List<FabricCapacity> value { get; set; }
  }

  public class FabricWorkspaceRoleAssignmentList {
    public List<FabricWorkspaceRoleAssignment> value { get; set; }
  }

  public class FabricWorkspaceRoleAssignment {
    public Principal principal { get; set; }
    public string role { get; set; }
  }

  public class WorkspaceRole {
    public const string Admin = "Admin";
    public const string Contributor = "Contributor";
    public const string Member = "Member";
    public const string Viewer = "Viewer";
  }

  public class Principal {
    public string id { get; set; }
    public string displayName { get; set; }
    public string type { get; set; }
    public UserDetails userDetails { get; set; }
    public GroupDetails groupDetails { get; set; }
    public ServicePrincipalDetails servicePrincipalDetails { get; set; }
    public ServicePrincipalProfileDetails servicePrincipalProfileDetails { get; set; }
  }

  public class PrincipalType {
    public const string User = "User";
    public const string Group = "Group";
    public const string ServicePrincipal = "ServicePrincipal";
    public const string ServicePrincipalProfile = "ServicePrincipalProfile";
  }

  public class UserDetails {
    public string userPrincipalName { get; set; }
  }

  public class GroupDetails {
    public string groupType { get; set; }
  }

  public class ServicePrincipalDetails {
    public string aadAppId { get; set; }
  }

  public class ServicePrincipalProfileDetails {
    public ParentPrincipal parentPrincipal { get; set; }
  }

  public class ParentPrincipal {
    public string id { get; set; }
    public string type { get; set; }
  }

  public class FabricItemList {
    public List<FabricItem> value { get; set; }
  }

  public class FabricItem {
    public string id { get; set; }
    public string type { get; set; }
    public string displayName { get; set; }
    public string description { get; set; }
    public string workspaceId { get; set; }
  }

  public class FabricItemUpdateRequest {
    public string displayName { get; set; }
    public string description { get; set; }
  }

  public class FabricItemType {
    public const string SemanticModel = "SemanticModel";
    public const string Report = "Report";
    public const string PaginatedReport = "PaginatedReport";
    public const string Dashboard = "Dashboard";
    public const string Datamart = "Datamart";
    public const string Lakehouse = "Lakehouse";
    public const string SQLEndpoint = "SQLEndpoint";
    public const string Notebook = "Notebook";
    public const string SparkJobDefinition = "SparkJobDefinition";
    public const string MLModel = "MLModel";
    public const string MLExperiment = "MLExperiment";
    public const string Warehouse = "Warehouse";
    public const string MirroredWarehouse = "MirroredWarehouse";
    public const string DataPipeline = "DataPipeline";
    public const string KQLDatabase = "KQLDatabase";
    public const string KQLDataConnection = "KQLDataConnection";
    public const string KQLQueryset = "KQLQueryset";
    public const string Eventstream = "Eventstream";

    // create collection of all possible types for testing
    public static readonly List<string> AllTypes = new List<string> {
      SemanticModel, Report, PaginatedReport, Dashboard, Datamart,
      Lakehouse, SQLEndpoint, Notebook, SparkJobDefinition, MLModel, MLExperiment,
      Warehouse, MirroredWarehouse, DataPipeline,
      KQLDatabase, KQLDataConnection, KQLQueryset, Eventstream
    };

  }

  public class FabricItemCreateRequest {
    public string type { get; set; }
    public string displayName { get; set; }
    public string description { get; set; }
    public FabricItemDefinition definition { get; set; }
  }

  public class FabricItemUpdateDefinitionRequest {
    public FabricItemDefinition definition { get; set; }
  }

  public class FabricItemDefinition {
    public string format { get; set; }
    public List<FabricItemDefinitionPart> parts { get; set; }
  }

  public class FabricItemDefinitionFormat {
    public const string ipynb = "ipynb";
    public const string SparkJobDefinitionV1 = "SparkJobDefinitionV1";
  }

  public class FabricItemDefinitionPart {
    public string path { get; set; }
    public string payload { get; set; }
    public string payloadType { get; set; }
  }

  public class SparkJobCreateData {
    public string workspaceId { get; set; }
    public string lakehouseId { get; set; }
    public string displayName { get; set; }
    public string description { get; set; }
    public string codeContent { get; set; }
    public bool runAfterCreate { get; set; }
  }

  public class FabricSparkJobDefinitionCreatePart {
    public object executableFile { get; set; }
    public string defaultLakehouseArtifactId { get; set; }
    public string mainClass { get; set; }
    public List<object> additionalLakehouseIds { get; set; }
    public object retryPolicy { get; set; }
    public string commandLineArguments { get; set; }
    public List<object> additionalLibraryUris { get; set; }
    public string language { get; set; }
    public object environmentArtifactId { get; set; }
  }

  public class FabricItemDefinitionResponse {
    public FabricItemDefinition definition { get; set; }
  }

  public class FabricLakehouse : FabricItem {
    public FabricLakehouseProperties properties { get; set; }
  }

  public class FabricLakehouseProperties {
    public string oneLakeTablesPath { get; set; }
    public string oneLakeFilesPath { get; set; }
    public SqlEndpointProperties sqlEndpointProperties { get; set; }
  }

  public class SqlEndpointProperties {
    public string connectionString { get; set; }
    public string id { get; set; }
    public string provisioningStatus { get; set; }
  }

  public class FabricSqlEndpoint {
    public string server { get; set; }
    public string database { get; set; }
  }

  public class FabricTableLoadRequest {
    public string relativePath { get; set; }
    public string loadType { get; set; }
    public string mode { get; set; }
    public bool recursive { get; set; }
    public FabricTableLoadRequestFormatOptions formatOptions { get; set; }
  }

  public class FabricTableLoadRequestFormatOptions {
    public string format { get; set; }
    public bool header { get; set; }
    public string delimiter { get; set; }
  }

  public class FabricOperation {
    public string status { get; set; }
    public DateTime createdTimeUtc { get; set; }
    public DateTime lastUpdatedTimeUtc { get; set; }
    public object percentComplete { get; set; }
    public FabricErrorResponse error { get; set; }
  }

  public class FabricErrorResponse {
    public string errorCode { get; set; }
    public string message { get; set; }
    public string requestId { get; set; }
    public object moreDetails { get; set; }
    public object relatedResource { get; set; }

  }

  public class FabricReportEmbeddingData {
    public string reportId;
    public string reportName;
    public string embedUrl;
    public string accessToken;
    public string accessTokenType;
  }

}
