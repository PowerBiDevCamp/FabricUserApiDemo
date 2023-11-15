using FabricUserApiDemo.Models;
using Microsoft.PowerBI.Api;
using Microsoft.PowerBI.Api.Models;
using Microsoft.PowerBI.Api.Models.Credentials;
using Microsoft.Rest;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Formats.Asn1.AsnWriter;

namespace FabricUserApiDemo.Services {

  public class PowerBiUserApi {

    private static PowerBIClient pbiClient;

    static PowerBiUserApi() {
      string accessToken = AzureAdTokenManager.GetAccessToken(PowerBiPermissionScopes.TenantProvisioning);
      string urlPowerBiServiceApiRoot = "https://api.powerbi.com/";
      var tokenCredentials = new TokenCredentials(accessToken, "Bearer");
      pbiClient = new PowerBIClient(new Uri(urlPowerBiServiceApiRoot), tokenCredentials);
    }

    public static void RefreshDataset(string WorkspaceId, string DatasetId) {

      var refreshRequest = new DatasetRefreshRequest {
        NotifyOption = NotifyOption.NoNotification,
        Type = DatasetRefreshType.Automatic
      };

      var responseStartFresh = pbiClient.Datasets.RefreshDatasetInGroup(new Guid(WorkspaceId), DatasetId, refreshRequest);

      var responseStatusCheck = pbiClient.Datasets.GetRefreshExecutionDetails(new Guid(DatasetId), new Guid(responseStartFresh.XMsRequestId));

      Console.Write(".");

      while (responseStatusCheck.Status == "Unknown") {
        Thread.Sleep(2000);
        Console.Write(".");
        responseStatusCheck = pbiClient.Datasets.GetRefreshExecutionDetails(new Guid(DatasetId), new Guid(responseStartFresh.XMsRequestId));
      }
      Console.WriteLine();
    }

    public static IList<Datasource> GetDatasourcesForDataset(string WorkspaceId, string DatasetId) {
      return pbiClient.Datasets.GetDatasourcesInGroup(new Guid(WorkspaceId), DatasetId).Value;
    }

    public static void PatchAnonymousAccessWebCredentials(string WorkspaceId, string DatasetId) {

      // get datasources for dataset
      var datasources = pbiClient.Datasets.GetDatasourcesInGroup(new Guid(WorkspaceId), DatasetId).Value;

      foreach (var datasource in datasources) {

        // check to ensure datasource use Web connector
        if (datasource.DatasourceType.ToLower() == "web") {

          // get DatasourceId and GatewayId
          var datasourceId = datasource.DatasourceId;
          var gatewayId = datasource.GatewayId;

          // Initialize UpdateDatasourceRequest object with AnonymousCredentials
          UpdateDatasourceRequest req = new UpdateDatasourceRequest {
            CredentialDetails = new CredentialDetails(
              new AnonymousCredentials(),
              PrivacyLevel.Organizational,
              EncryptedConnection.NotEncrypted)
          };

          // Update datasource credentials through Gateways - UpdateDatasource
          pbiClient.Gateways.UpdateDatasource((Guid)gatewayId, (Guid)datasourceId, req);

        }
      }
    }

    public static void PatchDirectLakeDatasetCredentials(string WorkspaceId, string DatasetId) {

      var datasources = pbiClient.Datasets.GetDatasourcesInGroup(new Guid(WorkspaceId), DatasetId).Value;

      // update credentials for all SQL datasources
      foreach (var datasource in datasources) {
        if (datasource.DatasourceType.ToLower() == "sql") {

          var datasourceId = datasource.DatasourceId;
          var gatewayId = datasource.GatewayId;

          // create credential details
          var CredentialDetails = new CredentialDetails();
          CredentialDetails.CredentialType = CredentialType.OAuth2;
          CredentialDetails.UseCallerAADIdentity = true;
          CredentialDetails.EncryptedConnection = EncryptedConnection.Encrypted;
          CredentialDetails.EncryptionAlgorithm = EncryptionAlgorithm.None;
          CredentialDetails.PrivacyLevel = PrivacyLevel.Organizational;

          // create UpdateDatasourceRequest 
          UpdateDatasourceRequest req = new UpdateDatasourceRequest(CredentialDetails);

          // Execute Patch command to update Azure SQL datasource credentials
          pbiClient.Gateways.UpdateDatasource((Guid)gatewayId, (Guid)datasourceId, req);

        }
      }


    }

    public static void ViewWorkspaces() {
      var workspaces = pbiClient.Groups.GetGroups().Value;
      foreach (var workspace in workspaces) {
        Console.WriteLine(workspace.Name);
      }
    }

    public static FabricReportEmbeddingData GetUserOwnsDataReportEmbeddingData(string workspaceId, string reportId) {

      var report = pbiClient.Reports.GetReportInGroup(new Guid(workspaceId), new Guid(reportId));
      var embedUrl = "https://app.powerbi.com/reportEmbed";
      var reportName = report.Name;
      var accessToken = AzureAdTokenManager.GetAccessToken();

      return new FabricReportEmbeddingData {
        reportId = reportId.ToString(),
        reportName = reportName,
        embedUrl = embedUrl,
        accessToken = accessToken,
        accessTokenType = "models.TokenType.Aad"
      };

    }

    public static FabricReportEmbeddingData GetAppOwnsDataReportEmbeddingData(string workspaceId, string reportId) {

      var report = pbiClient.Reports.GetReportInGroup(new Guid(workspaceId), new Guid(reportId));
      var embedUrl = "https://app.powerbi.com/reportEmbed";
      var reportName = report.Name;
      var accessToken = GetEmbedToken(workspaceId, report.DatasetId, reportId);

      return new FabricReportEmbeddingData {
        reportId = reportId.ToString(),
        reportName = reportName,
        embedUrl = embedUrl,
        accessToken = accessToken,
        accessTokenType = "models.TokenType.Embed"
      };

    }

    public static string GetEmbedToken(string WorkspaceId, string DatasetId, string ReportId) {


      IList<GenerateTokenRequestV2Dataset> datasetRequests = new List<GenerateTokenRequestV2Dataset>();
      datasetRequests.Add(new GenerateTokenRequestV2Dataset(DatasetId));
      
      IList<GenerateTokenRequestV2Report> reportRequests = new List<GenerateTokenRequestV2Report>();
      reportRequests.Add(new GenerateTokenRequestV2Report(new Guid(ReportId), allowEdit: false));

      var workspaceRequests = new List<GenerateTokenRequestV2TargetWorkspace>();
      workspaceRequests.Add(new GenerateTokenRequestV2TargetWorkspace(new Guid(WorkspaceId)));

      GenerateTokenRequestV2 tokenRequest =
        new GenerateTokenRequestV2 {
          Datasets = datasetRequests,
          Reports = reportRequests,
          TargetWorkspaces = workspaceRequests
        };

      // call to Power BI Service API and pass GenerateTokenRequest object to generate embed token
      var EmbedTokenResult = pbiClient.EmbedToken.GenerateToken(tokenRequest);

      return EmbedTokenResult.Token;

    }

    public static FabricReportEmbeddingData GetReportEmbeddingDataUserOwnsData(string workspaceId, string reportId) {

      var report = pbiClient.Reports.GetReportInGroup(new Guid(workspaceId), new Guid(reportId));
      var embedUrl = "https://app.powerbi.com/reportEmbed";
      var reportName = report.Name;
      var accessToken = AzureAdTokenManager.GetAccessToken();

      return new FabricReportEmbeddingData {
        reportId = reportId.ToString(),
        reportName = reportName,
        embedUrl = embedUrl,
        accessToken = accessToken,
        accessTokenType = "models.TokenType.Aad"
      };

    }

  }

}