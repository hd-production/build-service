using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using HdProduction.BuildService.Synchronization;

namespace HdProduction.BuildService.Services
{
  public interface IHelpdeskBuildService
  {
    string BuildApp(int buildConfiguration);
  }

  public class HelpdeskBuildService : IHelpdeskBuildService
  {
    private const string ProjectPrefix = "ConfigTool.App.";
    private readonly string _sourcesPath;

    public HelpdeskBuildService(string sourcesPath)
    {
      _sourcesPath = sourcesPath;
    }

    public string BuildApp(int buildConfiguration)
    {
      var appBuildName = ProjectPrefix + buildConfiguration;
      var buildKey = $"{appBuildName}_{DateTime.UtcNow.ToFileTimeUtc()}";
      var pathToProject = Path.Combine(_sourcesPath, $"{appBuildName}/{appBuildName}.csproj");
      var outputPath = Path.Combine(_sourcesPath, buildKey);
      var zippedBuild = outputPath + ".zip";

      if (!File.Exists(pathToProject))
      {
        throw new ApplicationException("Selected build configuration is not supported");
      }

      Process.Start("dotnet", $"publish {pathToProject} -c Release -o {outputPath}")
        ?.WaitForExit();

      ZipFile.CreateFromDirectory(outputPath, zippedBuild);
      Directory.Delete(outputPath, true);

      return zippedBuild;
    }
  }
}