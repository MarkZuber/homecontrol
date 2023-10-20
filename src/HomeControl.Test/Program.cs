using HomeControl.Web;
using HomeControl.Web.Devices.Epson;
using HomeControl.Web.Devices.Denon;
using HomeControl.Web.Services;

public static class Program
{
#pragma warning disable UseAsyncSuffix
    public static async Task Main(string[] args)
    {
        var logger = LoggerFactory.CreateLogger("/tmp", "HomeControl.Test_");
        logger.Information("Starting HomeControl.Test");
        try
        {
            if (args.Length == 0)
            {
                Console.WriteLine("No arguments provided");
                return;
            }

            Console.WriteLine(args[0]);

            EpsonNetworkProjector epsonProj = new(new NetworkSettings());
            DenonNetworkReceiver denonRcvr = new(new DenonHttpReceiverDevice(new NetworkSettings()));
            switch (args[0])
            {
                case "projon":
                    Console.WriteLine("Turning on Epson Projector");
                    await epsonProj.TurnOnAsync();
                    break;
                case "projoff":
                    Console.WriteLine("Turning off Epson Projector");
                    await epsonProj.TurnOffAsync();
                    break;
                case "rcvron":
                    Console.WriteLine("Turning on Denon Receiver");
                    await denonRcvr.TurnOnAsync();
                    break;
                case "rcvroff":
                    Console.WriteLine("Turning off Denon Receiver");
                    await denonRcvr.TurnOffAsync();
                    break;
                default:
                    Console.WriteLine("Unknown arguments provided");
                    break;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"ERROR: {ex.Message}");
        }
    }
}