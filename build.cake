#tool nuget:?package=Cake.Bakery&Version=0.4.1

var target = Argument("target", "Default");

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
        CsProjLocalPath = $"{LocalPath}/{projName}.csproj";
        LinuxArmReleasePath = $"{LocalPath}/bin/{coreBuildInformation.BuildConfiguration}/{coreBuildInformation.DotnetFramework}/{coreBuildInformation.Architecture}";
        RemoteStagingPath = $"{CoreBuildInfo.RemoteServerSshAddress}:/home/pi/{ServiceName}_staging";
    }

    public CoreBuildInformation CoreBuildInfo {get;}

    public string ServiceName {get;}
    public string LocalPath { get; }
    public string CsProjLocalPath { get; }
    public string LinuxArmReleasePath { get; }
    public string RemoteStagingPath { get; }
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
    var coreBuildInformation = new CoreBuildInformation("Release", "192.168.2.203", "netcoreapp3.0", "linux-arm");
    return new BuildData(coreBuildInformation);
});

public void RunWslCommand(string command)
{
    string cmd = "wsl.exe";
    string args = command;

    if (Environment.OSVersion.Platform == PlatformID.Unix)
    {
        cmd = command.Substring(0, command.IndexOf(' '));
        args = command.Substring(command.IndexOf(' '));
    }

    StartProcess(cmd, new ProcessSettings
    {
        Arguments = args,
    });
}

public void RunRemoteCommand(string remoteServerSshAddress, string command)
{
    if (Environment.OSVersion.Platform == PlatformID.Unix)
    {
        return;
    }

    if (!command.StartsWith("'"))
    {
        command = $"'{command}'";
    }
    RunWslCommand($"ssh {remoteServerSshAddress} {command}");
}

public void DeploySystemdService(ServiceData serviceData)
{
    // Build and publish the bits for linux-arm architecture
    DotNetCorePublish(
        serviceData.CsProjLocalPath,
        new DotNetCorePublishSettings
        {
            Configuration = serviceData.CoreBuildInfo.BuildConfiguration,
            Runtime = serviceData.CoreBuildInfo.Architecture
        });

    // rsync the bits over to the staging path on the pi
    RunWslCommand($"rsync -rvzh {serviceData.LinuxArmReleasePath}/ {serviceData.RemoteStagingPath}");

    // Ensure user / environment/paths are setup on the pi for the systemd execution
    RunRemoteCommand(serviceData.CoreBuildInfo.RemoteServerSshAddress, $"'bash -s' < ./scripts/configure_{serviceData.ServiceName}_systemd.sh");
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
