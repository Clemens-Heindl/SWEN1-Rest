using System.Reflection;

using Clemens.SWEN1.Server;



namespace Clemens.SWEN1.Handlers;

/// <summary>This class provides a base implementation of the <see cref="IHandler"/> interface and a static event handler for the <see cref="HttpRestServer.RequestReceived"/> event.</summary>
public abstract class Handler: IHandler
{

    /// <summary>Available handlers.</summary>
    private static List<IHandler>? _Handlers = null;


    
    /// <summary>Gets a list of available handlers.</summary>
    /// <returns>Returns a list of handlers.</returns>
    private static List<IHandler> _GetHandlers()
    {
        List<IHandler> rval = new();

        foreach(Type i in Assembly.GetExecutingAssembly().GetTypes()
            .Where(m => m.IsAssignableTo(typeof(IHandler)) && !m.IsAbstract))
        {
            IHandler? h = (IHandler?) Activator.CreateInstance(i);
            if(h is not null) { rval.Add(h); }
        }

        return rval;
    }


    
    /// <summary>Provides an event handler for the <see cref="HttpRestServer.RequestReceived"/> event.</summary>
    /// <param name="sender">Object that raised the event.</param>
    /// <param name="e">Event arguments.</param>
    public static void HandleEvent(object? sender, HttpRestEventArgs e)
    {
        foreach(IHandler i in (_Handlers ??= _GetHandlers()))
        {
            i.Handle(e);
            if(e.Responded) break;
        }
    }


    
    /// <summary>Handles a request if possible.</summary>
    /// <param name="e">Event arguments.</param>
    public abstract void Handle(HttpRestEventArgs e);
}
