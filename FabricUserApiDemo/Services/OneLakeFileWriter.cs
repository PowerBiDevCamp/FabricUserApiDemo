using Azure.Core;
using Azure.Storage.Files.DataLake;
using Microsoft.Identity.Client;
using Microsoft.Rest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace FabricUserApiDemo.Services {

  public class OneLakeTokenCredentials : TokenCredential {

    private static readonly string[] requiredScoped = new string[] {
      "https://storage.azure.com/user_impersonation"
    };

    private static AuthenticationResult accessTokenResult = AzureAdTokenManager.GetAccessTokenResult(requiredScoped);

    private readonly AccessToken accessToken;


    public OneLakeTokenCredentials() {
      accessToken = new AccessToken(accessTokenResult.AccessToken, accessTokenResult.ExpiresOn);
    }

    public override AccessToken GetToken(TokenRequestContext requestContext, CancellationToken cancellationToken) => accessToken;

    public override ValueTask<AccessToken> GetTokenAsync(TokenRequestContext requestContext, CancellationToken cancellationToken) =>
        new ValueTask<AccessToken>(Task.FromResult(accessToken));
  }

  public class OneLakeFileWriter {

    private const string oneLakeUrl = "https://onelake.blob.fabric.microsoft.com";
    private static readonly Uri oneLakeUri = new Uri(oneLakeUrl);

    private string workspaceId;
    private string lakehouseId;

    private DataLakeServiceClient dataLakeServiceClient;
    private DataLakeFileSystemClient fileSystemClient;
    private DataLakeDirectoryClient filesFolder;

    public OneLakeFileWriter(string WorkspaceId, string LakehouseId) {
      this.workspaceId = WorkspaceId;
      this.lakehouseId = LakehouseId;
      this.dataLakeServiceClient = new DataLakeServiceClient(oneLakeUri, new OneLakeTokenCredentials());
      this.fileSystemClient = dataLakeServiceClient.GetFileSystemClient(workspaceId);
      filesFolder = this.fileSystemClient.GetDirectoryClient(lakehouseId + @"\Files");
    }

    public void UploadMainFileForSparkJobDefinition(string SparkJobDefinitionId, string FileContent) {

      var sjdFolder = this.fileSystemClient.GetDirectoryClient(SparkJobDefinitionId + @"\Main");

      var file = sjdFolder.GetFileClient("Main.py");
      file.Create();

      var fileContentStream = new MemoryStream(Encoding.UTF8.GetBytes(FileContent));

      file.Append(fileContentStream, 0);
      file.Flush(fileContentStream.Length);      

    }

    public DataLakeDirectoryClient CreateTopLevelFolder(string FolderName) {
      var folder = filesFolder.GetSubDirectoryClient(FolderName);
      folder.CreateIfNotExists();
      return folder;
    }

    public DataLakeFileClient CreateFile(DataLakeDirectoryClient Folder, string FileName, Stream FileContent) {

      var file = Folder.GetFileClient(FileName);
      file.Create();

      file.Append(FileContent, 0);
      file.Flush(FileContent.Length);

      return file;
    }



  }

}
