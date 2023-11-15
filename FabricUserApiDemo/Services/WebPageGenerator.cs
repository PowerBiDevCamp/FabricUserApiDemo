using System;
using System.IO;
using System.Diagnostics;
using System.Configuration;

namespace FabricUserApiDemo.Services {
  public class WebPageGenerator {

    private static readonly string rootFolder = AppSettings.LocalWebPageFolder;

    static WebPageGenerator() {
      Directory.CreateDirectory(rootFolder);
      Directory.CreateDirectory(rootFolder + "css");
      File.WriteAllText(rootFolder + "css/styles.css", Properties.Resources.styles_css);
      Directory.CreateDirectory(rootFolder + "js");
      File.WriteAllText(rootFolder + "js/jquery.js", Properties.Resources.jquery_js);
      File.WriteAllText(rootFolder + "js/powerbi.js", Properties.Resources.powerbi_js);
    }

    private static void LaunchPageInBrowser(string pagePath) {
      var process = new Process();
      process.StartInfo = new ProcessStartInfo(@"C:\Program Files\Google\Chrome\Application\chrome.exe");
      process.StartInfo.Arguments = pagePath + " --profile-directory=\"Profile 1\" ";
      process.Start();
    }

    public static void GenerateReportPageUserOwnsData(string workspaceId, string reportId, bool LaunchInBrowser = true) {

      // get Power BI embedding data
      var embeddingData = PowerBiUserApi.GetUserOwnsDataReportEmbeddingData(workspaceId, reportId);

      // parse embedding data into page template
      string htmlSource = Properties.Resources.EmbedReport_html;
      string htmlOutput = htmlSource.Replace("@AppName", "Sales Report - User-Owns-Data")
                                    .Replace("@EmbedReportId", embeddingData.reportId)
                                    .Replace("@EmbedUrl", embeddingData.embedUrl)
                                    .Replace("@EmbedToken", embeddingData.accessToken)
                                    .Replace("EmbedTokenType", "models.TokenType.Aad");


      // generate page file on local har drive
      string pagePath = rootFolder + "SalesReport-UserOwnsData.html";
      File.WriteAllText(pagePath, htmlOutput);

      // launch page in browser if requested
      if (LaunchInBrowser) {
        LaunchPageInBrowser(Path.GetFullPath(pagePath));
      }
    }

    public static void GenerateReportPageAppOwnsData(string workspaceId, string reportId, bool LaunchInBrowser = true) {

      // get Power BI embedding data
      var embeddingData = PowerBiUserApi.GetAppOwnsDataReportEmbeddingData(workspaceId, reportId);

      // parse embedding data into page template
      string htmlSource = Properties.Resources.EmbedReport_html;
      string htmlOutput = htmlSource.Replace("@AppName", "Sales Report - App-Owns-Data")
                                    .Replace("@EmbedReportId", embeddingData.reportId)
                                    .Replace("@EmbedUrl", embeddingData.embedUrl)
                                    .Replace("@EmbedToken", embeddingData.accessToken)
                                    .Replace("EmbedTokenType", "models.TokenType.Embed");


      // generate page file on local har drive
      string pagePath = rootFolder + "SalesReport-AppOwnsData.html";
      File.WriteAllText(pagePath, htmlOutput);

      // launch page in browser if requested
      if (LaunchInBrowser) {
        LaunchPageInBrowser(Path.GetFullPath(pagePath));
      }
    }


  }
}
