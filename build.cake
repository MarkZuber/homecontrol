///////////////////////////////////////////////////////////////////////////////
// ARGUMENTS
///////////////////////////////////////////////////////////////////////////////

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");

public class CoreBuildInformation
{
    public CoreBuildInformation(
        string buildConfiguration,
        string remoteServerAddress,
        string dotnetFramework,
        string architecture)
    {
        BuildConfiguration = buildConfiguration;
        RemoteServerAddress = remoteServerAddress;
        DotnetFramework = dotnetFramework;
        Architecture = architecture;
    }

    public string BuildConfiguration {get;}
    public string RemoteServerAddress {get;}
    public string RemoteServerSshAddress => $"pi@{RemoteServerAddress}";
    public string DotnetFramework {get;}
    public string Architecture {get;}
}

public class ServiceData
{
    public ServiceData(CoreBuildInformation coreBuildInformation, string projName, string serviceName)
    {
        ServiceName = serviceName;
        CoreBuildInfo = coreBuildInformation;
        LocalPath = $"./src/{projName}";
        LocalScriptPath = "./scripts";
        CsProjLocalPath = $"{LocalPath}/{projName}.csproj";
        LinuxArmReleasePath = $"{LocalPath}/bin/{coreBuildInformation.BuildConfiguration}/{coreBuildInformation.DotnetFramework}/{coreBuildInformation.Architecture}";
        RemoteStagingPath = $"{CoreBuildInfo.RemoteServerSshAddress}:/home/pi/{ServiceName}_staging";
        RemoteLocalScriptPath = "/home/pi/scripts_staging";
        RemoteScriptPath = $"{CoreBuildInfo.RemoteServerSshAddress}:/home/pi/scripts_staging";
    }

    public CoreBuildInformation CoreBuildInfo {get;}

    public string ServiceName {get;}
    public string LocalPath { get; }
    public string CsProjLocalPath { get; }
    public string LinuxArmReleasePath { get; }
    public string RemoteStagingPath { get; }
    public string LocalScriptPath { get; }
    public string RemoteLocalScriptPath { get; }
    public string RemoteScriptPath { get; }
}

public class BuildData
{
    public BuildData(CoreBuildInformation coreBuildInformation)
    {
        StreamDeckControllerService = new ServiceData(
            coreBuildInformation,
            "HomeControl.StreamDeck",
            "streamdeck");

        HomeControlWebService = new ServiceData(
            coreBuildInformation,
            "HomeControl.Web",
            "homecontrolweb");
    }

    public ServiceData StreamDeckControllerService {get;}
    public ServiceData HomeControlWebService {get;}
}

Setup<BuildData>(setupContext=> {
    var coreBuildInformation = new CoreBuildInformation("Release", "192.168.2.203", "net5.0", "linux-arm");
    return new BuildData(coreBuildInformation);
});

public void RunLocalCommand(string command)
{
    string cmd;
    string args;

    if (Environment.OSVersion.Platform == PlatformID.Unix)
    {
        cmd = command.Substring(0, command.IndexOf(' '));
        args = command.Substring(command.IndexOf(' '));
    }
    else
    {
        cmd = "wsl.exe";
        args = command;
    }

    StartProcess(cmd, new ProcessSettings
    {
        Arguments = args,
    });
}

public void RunServiceStartScript(ServiceData serviceData)
{
    RunRsync(serviceData.LocalScriptPath, serviceData.RemoteScriptPath);
    string scriptCommand = $"{serviceData.RemoteLocalScriptPath}/configure_{serviceData.ServiceName}_systemd.sh";
    RunLocalCommand($"ssh -t {serviceData.CoreBuildInfo.RemoteServerSshAddress} {scriptCommand}");
}

public void RunRsync(string sourcePath, string targetPath)
{
    RunLocalCommand($"rsync -rvzh --delete {sourcePath}/ {targetPath}");
}

public void DeploySystemdService(ServiceData serviceData)
{
    // Build and publish the bits for linux-arm architecture
    DotNetCorePublish(
        serviceData.CsProjLocalPath,
        new DotNetCorePublishSettings
        {
            Configuration = serviceData.CoreBuildInfo.BuildConfiguration,
            Runtime = serviceData.CoreBuildInfo.Architecture,
        });

    RunRsync(serviceData.LinuxArmReleasePath, serviceData.RemoteStagingPath);
    RunServiceStartScript(serviceData);
}

Task("DeployStreamDeckController")
    .Does<BuildData>((buildData) => {

        DeploySystemdService(buildData.StreamDeckControllerService);
    });

Task("DeployHomeControlWeb")
    .Does<BuildData>((buildData) => {
        DeploySystemdService(buildData.HomeControlWebService);
    });

Task("DeployAll")
    .IsDependentOn("DeployHomeControlWeb")
    .IsDependentOn("DeployStreamDeckController")
    .Does(() =>
    {
    });

Task("Default")
    .IsDependentOn("DeployAll")
    .Does(() =>
    {
    });

RunTarget(target);
