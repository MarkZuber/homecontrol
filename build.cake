#tool nuget:?package=Cake.Bakery&Version=0.4.1
#addin nuget:?package=Cake.Docker&Version=0.9.9

var target = Argument("target", "Default");

var BuildConfiguration = "Release";
var RemoteServerAddress = "192.168.2.203";
var RemoteServerSshAddress = $"pi@{RemoteServerAddress}";


public class StreamDeckData
{
    public StreamDeckData(string removeServerSshAddress)
    {
        RemoteServerSshAddress = removeServerSshAddress;
    }

    public string RemoteServerSshAddress { get; }

    public string LocalPath => "./src/HomeControl.StreamDeck";
    public string CsProjLocalPath => $"{LocalPath}/HomeControl.StreamDeck.csproj";
    public string LinuxArmReleasePath => $"{LocalPath}/bin/Release/netcoreapp3.0/linux-arm";
    public string RemoteStagingPath => $"{RemoteServerSshAddress}:/home/pi/streamdeck_staging";
}

Setup<StreamDeckData>(setupContext=> {
    return new StreamDeckData(RemoteServerSshAddress);
});

var HomeControlWebLocalPath = "./src/HomeControl.Web";
var HomeControlWebDockerfile = $"{HomeControlWebLocalPath}/Dockerfile";


public void RunWslCommand(string command)
{
    StartProcess("wsl.exe", new ProcessSettings
    {
        Arguments = command
    });
}

public void RunRemoteCommand(string command)
{
    if (!command.StartsWith("'"))
    {
        command = $"'{command}'";
    }
    RunWslCommand($"ssh {RemoteServerSshAddress} {command}");
}


Task("DeployStreamDeckController")
    .Does<StreamDeckData>((streamDeckData) => {
        // Build and publish the bits for linux-arm architecture
        DotNetCorePublish(
            streamDeckData.CsProjLocalPath,
            new DotNetCorePublishSettings
            {
                Configuration = BuildConfiguration,
                Runtime = "linux-arm"
            });

        // rsync the bits over to the staging path on the pi
        RunWslCommand($"rsync -rvzh {streamDeckData.LinuxArmReleasePath}/* {streamDeckData.RemoteStagingPath}");

        // Ensure user / environment/paths are setup on the pi for the systemd execution
        RunRemoteCommand($"'bash -s' < scripts/configure_streamdeck_systemd.sh");
    });

Task("DeployHomeControlWeb")
    .Does(() => {

        string dockerImageName = "homecontrolweb:dev";

        DockerBuild(
            new DockerImageBuildSettings
            {
                File = HomeControlWebDockerfile,
                Tag = new string[] { dockerImageName }
            },
            ".");

        // TODO:
        // https://stackoverflow.com/questions/23935141/how-to-copy-docker-images-from-one-host-to-another-without-using-a-repository
        // Transferring a Docker image via SSH, bzipping the content on the fly:

        // docker save <image> | bzip2 | \
        //     ssh user@host 'bunzip2 | docker load'
        // It's also a good idea to put pv in the middle of the pipe to see how the transfer is going:

        // docker save <image> | bzip2 | pv | \
        //     ssh user@host 'bunzip2 | docker load'
        RunWslCommand($"docker save {dockerImageName} | bzip2 | pv | ssh {RemoteServerSshAddress} 'bunzip2 | docker load'");
        RunRemoteCommand($"docker run {dockerImageName}");
    });

Task("DeployAll")
    .IsDependentOn("DeployHomeControlWeb")
    .IsDependentOn("DeployStreamDeckController")
    .Does(() =>
    {
    });

Task("Default")
    .Does(() =>
    {
    Information("Hello World!");
});

RunTarget(target);
