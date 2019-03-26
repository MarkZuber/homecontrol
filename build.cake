#tool nuget:?package=Cake.Bakery&Version=0.4.1
#addin nuget:?package=Cake.Docker&Version=0.9.9

var target = Argument("target", "Default");

var BuildConfiguration = "Release";
var RemoteServerAddress = "192.168.2.203";
var RemoteServerSshAddress = $"pi@{RemoteServerAddress}";

var StreamDeckControllerLocalPath = "./src/HomeControl.StreamDeck";
var StreamDeckControllerCsProjLocalPath = $"{StreamDeckControllerLocalPath}/HomeControl.StreamDeck.csproj";
var StreamDeckControllerLinuxArmReleasePath = $"{StreamDeckControllerLocalPath}/bin/Release/netcoreapp3.0/linux-arm";
var StreamDeckControllerRemotePath = $"{RemoteServerSshAddress}:/home/pi/streamdeckcon";

var HomeControlWebLocalPath = "./src/HomeControl.Web";
var HomeControlWebDockerfile = $"{HomeControlWebLocalPath}/Dockerfile";


Task("DeployStreamDeckController")
    .Does(() => {
    DotNetCorePublish(
        StreamDeckControllerCsProjLocalPath,
        new DotNetCorePublishSettings
        {
            Configuration = BuildConfiguration,
            Runtime = "linux-arm"
        });
    StartProcess("wsl.exe", new ProcessSettings
    {
        Arguments = $"rsync -rvzh {StreamDeckControllerLinuxArmReleasePath}/* {StreamDeckControllerRemotePath}"
    });
});

Task("DeployHomeControlWeb")
    .Does(() => {
        DockerBuild(
            new DockerImageBuildSettings
            {
                File = HomeControlWebDockerfile,
                Tag = new string[] { "homecontrol/latest" }
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
