using Nuke.Common;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.Npm;
using static Nuke.Common.Tools.DotNet.DotNetTasks;
using static Nuke.Common.Tools.Npm.NpmTasks;

partial class Build
{
    private Target TypescriptInstall => _ => _
        .Executes(() => NpmInstall(s => s
            .AddProcessEnvironmentVariable("npm_config_loglevel", "error")
            .SetProcessWorkingDirectory(Paths.Typescript)));

    private Target TypescriptSystemWorkspaceMeta => _ => _
    .After(TypescriptInstall)
    .DependsOn(AllorsDotnetCoreGenerate)
    .DependsOn(EnsureDirectories)
    .Executes(() => NpmRun(s => s
        .AddProcessEnvironmentVariable("npm_config_loglevel", "error")
        .SetProcessWorkingDirectory(Paths.Typescript)
        .SetCommand("system-workspace-meta:test")));


    private Target TypescriptSystemWorkspaceMetaJson => _ => _
        .After(TypescriptInstall)
        .DependsOn(AllorsDotnetCoreGenerate)
        .DependsOn(EnsureDirectories)
        .Executes(() => NpmRun(s => s
            .AddProcessEnvironmentVariable("npm_config_loglevel", "error")
            .SetProcessWorkingDirectory(Paths.Typescript)
            .SetCommand("system-workspace-meta-json:test")));

    private Target TypescriptSystemWorkspaceAdapters => _ => _
        .After(TypescriptInstall)
        .DependsOn(AllorsDotnetCoreGenerate)
        .DependsOn(EnsureDirectories)
        .Executes(() => NpmRun(s => s
            .AddProcessEnvironmentVariable("npm_config_loglevel", "error")
            .SetProcessWorkingDirectory(Paths.Typescript)
            .SetCommand("system-workspace-adapters:test")));

    private Target TypescriptSystemWorkspaceAdaptersJson => _ => _
        .After(TypescriptInstall)
        .DependsOn(EnsureDirectories)
        .DependsOn(AllorsDotnetCoreGenerate)
        .DependsOn(AllorsDotnetCorePublishServer)
        .DependsOn(AllorsDotnetCorePublishCommands)
        .DependsOn(AllorsDotnetCoreResetDatabase)
        .Executes(async () =>
        {
            DotNet("Commands.dll Populate", Paths.ArtifactsCoreCommands);
            using var server = new Server(Paths.ArtifactsCoreServer);
            await server.Ready();
            NpmRun(s => s
                .AddProcessEnvironmentVariable("npm_config_loglevel", "error")
                .SetProcessWorkingDirectory(Paths.Typescript)
                .SetCommand("system-workspace-adapters-json:test"));
        });

    private Target TypescriptWorkspaceTests => _ => _
         .After(TypescriptInstall)
         .DependsOn(TypescriptSystemWorkspaceMeta)
         .DependsOn(TypescriptSystemWorkspaceMetaJson)
         .DependsOn(TypescriptSystemWorkspaceAdapters);

    private Target TypescriptWorkspaceAdaptersJsonTests => _ => _
        .After(TypescriptInstall)
        .DependsOn(TypescriptSystemWorkspaceAdaptersJson);
}