using Clemens.SWEN1.Handlers;
using Clemens.SWEN1.Server;

namespace Clemens.SWEN1;

/// <summary>Program class.</summary>
internal static class Program
{
    
    /// <summary>Main entry point of the application.</summary>
    /// <param name="args">Command line arguments.</param>
    static void Main(string[] args)
    {
        HttpRestServer svr = new();
        svr.RequestReceived += Handler.HandleEvent;
        svr.Run();
    }
}
