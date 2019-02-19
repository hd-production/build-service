using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using HdProduction.BuildService.Synchronization;

namespace HdProduction.BuildService.Services
{
  public interface IHelpdeskBuildService1
  {
    string BuildApp(int buildConfiguration);
  }

  public class HelpdeskBuildService1 : IHelpdeskBuildService1
  {
    private const string ProjectPrefix = "ConfigTool.App.";
    private readonly string _sourcesPath;

    public HelpdeskBuildService1(string sourcesPath)
    {
      _sourcesPath = sourcesPath;
    }

    public string BuildApp(int buildConfiguration)
    {
      var appBuildName = ProjectPrefix + buildConfiguration;
      var pathToProject = Path.Combine(_sourcesPath, $"{appBuildName}/{appBuildName}.csproj");
      var outputPath = Path.Combine(_sourcesPath, $"AppBuilds/{appBuildName}");
      var zippedBuild = outputPath + ".zip";

      using (new AppBuildLocker(buildConfiguration))
      {
        if (File.Exists(zippedBuild))
        {
          return zippedBuild;
        }

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
}