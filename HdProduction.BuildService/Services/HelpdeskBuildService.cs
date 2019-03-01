using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using HdProduction.BuildService.Models;

namespace HdProduction.BuildService.Services
{
  public interface IHelpdeskBuildService
  {
    string BuildApp(SelfHostBuildConfiguration buildConfiguration);
  }

  public class HelpdeskBuildService : IHelpdeskBuildService
  {
    private const string ProjectPrefix = "HelpDesk.App.";

    private static readonly Dictionary<SelfHostBuildConfiguration, string> AppBuildNames = new Dictionary<SelfHostBuildConfiguration, string>
    {
      [SelfHostBuildConfiguration.MySql] = "MySql",
      [SelfHostBuildConfiguration.PostgresSql] = "PostgresSql",
      [SelfHostBuildConfiguration.SqlServer] = "SqlServer",
      [SelfHostBuildConfiguration.Sqlite] = "Sqlite",
    };

    private readonly string _sourcesPath;

    public HelpdeskBuildService(string sourcesPath)
    {
      _sourcesPath = sourcesPath;
    }

    public string BuildApp(SelfHostBuildConfiguration buildConfiguration)
    {
      var appBuildName = ProjectPrefix + AppBuildNames[buildConfiguration];
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