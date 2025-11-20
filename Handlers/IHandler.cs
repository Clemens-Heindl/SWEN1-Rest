using FHTW.Swen1.Forum.Server;



namespace FHTW.Swen1.Forum.Handlers;

/// <summary>Classes capable of handling request implement this interface.</summary>
public interface IHandler
{
    
    /// <summary>Handles a request if possible.</summary>
    /// <param name="e">Event arguments.</param>
    public void Handle(HttpRestEventArgs e);
}
