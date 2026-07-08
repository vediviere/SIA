using System.Xml.Linq;

namespace SIA.ArchitectureTests;

public class ProjectReferenceRulesTests
{
    [Fact]
    public void Domain_projects_should_not_depend_on_application_infrastructure_api_or_other_services()
    {
        var violations = GetServiceProjects()
            .Where(p => p.Layer == "Domain")
            .SelectMany(project =>
                project.ProjectReferences
                    .Where(reference =>
                        !IsBuildingBlock(reference, "SIA.BuildingBlocks.Domain"))
                    .Select(reference =>
                        $"""El proyecto Domain '{project.Name}' no debe referenciar '{reference}'."""))
            .ToList();

        Assert.Empty(violations);
    }

    [Fact]
    public void Application_projects_should_not_depend_on_api_or_infrastructure()
    {
        var violations = GetServiceProjects()
            .Where(p => p.Layer == "Application")
            .SelectMany(project =>
                project.ProjectReferences
                    .Where(reference =>
                        IsLayer(reference, "Api") ||
                        IsLayer(reference, "Infrastructure"))
                    .Select(reference =>
                        $"""El proyecto Application '{project.Name}' no debe referenciar '{reference}'."""))
            .ToList();

        Assert.Empty(violations);
    }

    [Fact]
    public void Contracts_projects_should_not_depend_on_domain_application_infrastructure_or_api()
    {
        var violations = GetServiceProjects()
            .Where(p => p.Layer == "Contracts")
            .SelectMany(project =>
                project.ProjectReferences
                    .Where(reference =>
                        IsLayer(reference, "Domain") ||
                        IsLayer(reference, "Application") ||
                        IsLayer(reference, "Infrastructure") ||
                        IsLayer(reference, "Api"))
                    .Select(reference =>
                        $"""El proyecto Contracts '{project.Name}' no debe referenciar capas internas: '{reference}'."""))
            .ToList();

        Assert.Empty(violations);
    }

    [Fact]
    public void Services_should_not_reference_internal_layers_from_other_services()
    {
        var violations = GetServiceProjects()
            .SelectMany(project =>
                project.ProjectReferences
                    .Where(reference =>
                        IsOtherServiceReference(project, reference) &&
                        !IsLayer(reference, "Contracts"))
                    .Select(reference =>
                        $"""El proyecto '{project.Name}' no debe referenciar capas internas de otro servicio: '{reference}'."""))
            .ToList();

        Assert.Empty(violations);
    }

    [Fact]
    public void Api_projects_should_not_reference_domain_directly()
    {
        var violations = GetServiceProjects()
            .Where(p => p.Layer == "Api")
            .SelectMany(project =>
                project.ProjectReferences
                    .Where(reference => IsLayer(reference, "Domain"))
                    .Select(reference =>
                        $"""El proyecto Api '{project.Name}' no debe referenciar Domain directamente: '{reference}'."""))
            .ToList();

        Assert.Empty(violations);
    }

    private static List<ProjectInfo> GetServiceProjects()
    {
        var root = FindRepositoryRoot();

        var serviceProjectFiles = Directory
            .GetFiles(Path.Combine(root, "src", "services"), "*.csproj", SearchOption.AllDirectories);

        return serviceProjectFiles
            .Select(projectPath =>
            {
                var projectName = Path.GetFileNameWithoutExtension(projectPath);
                var serviceDirectory = projectPath
                    .Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)
                    .First(part => part.StartsWith("SIA.") && part.EndsWith("Service"));

                var layer = GetLayer(projectName);

                var document = XDocument.Load(projectPath);

                var references = document
                    .Descendants("ProjectReference")
                    .Select(element => element.Attribute("Include")?.Value)
                    .Where(value => !string.IsNullOrWhiteSpace(value))
                    .Select(value => Path.GetFullPath(Path.Combine(Path.GetDirectoryName(projectPath)!, value!)))
                    .ToList();

                return new ProjectInfo(
                    projectPath,
                    projectName,
                    serviceDirectory,
                    layer,
                    references
                );
            })
            .ToList();
    }

    private static string FindRepositoryRoot()
    {
        var current = new DirectoryInfo(AppContext.BaseDirectory);

        while (current is not null)
        {
            var slnx = Path.Combine(current.FullName, "SIA.Platform.slnx");
            var props = Path.Combine(current.FullName, "Directory.Build.props");

            if (File.Exists(slnx) || File.Exists(props))
            {
                return current.FullName;
            }

            current = current.Parent;
        }

        throw new DirectoryNotFoundException("No se encontró la raíz del repositorio SIA.");
    }

    private static string GetLayer(string projectName)
    {
        if (projectName.EndsWith(".Api")) return "Api";
        if (projectName.EndsWith(".Application")) return "Application";
        if (projectName.EndsWith(".Domain")) return "Domain";
        if (projectName.EndsWith(".Infrastructure")) return "Infrastructure";
        if (projectName.EndsWith(".Contracts")) return "Contracts";
        if (projectName.EndsWith(".Tests")) return "Tests";

        return "Unknown";
    }

    private static bool IsLayer(string projectReferencePath, string layer)
    {
        var projectName = Path.GetFileNameWithoutExtension(projectReferencePath);
        return projectName.EndsWith($".{layer}");
    }

    private static bool IsBuildingBlock(string projectReferencePath, string projectName)
    {
        return projectReferencePath.Contains("building-blocks", StringComparison.OrdinalIgnoreCase) &&
               Path.GetFileNameWithoutExtension(projectReferencePath) == projectName;
    }

    private static bool IsOtherServiceReference(ProjectInfo project, string projectReferencePath)
    {
        var normalizedPath = projectReferencePath.Replace('\\', '/');

        if (!normalizedPath.Contains("/src/services/", StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        return !normalizedPath.Contains($"/src/services/{project.ServiceDirectory}/", StringComparison.OrdinalIgnoreCase);
    }

    private sealed record ProjectInfo(
        string Path,
        string Name,
        string ServiceDirectory,
        string Layer,
        List<string> ProjectReferences
    );
}
