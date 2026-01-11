using System.Net;
using System.Text.Json.Nodes;

using Clemens.SWEN1.System;
using Clemens.SWEN1.Server;



namespace Clemens.SWEN1.Handlers;

/// <summary>This class implements a Handler for media entry endpoints.</summary>
public sealed class MediaHandler: Handler, IHandler
{

    
    /// <summary>Handles a request if possible.</summary>
    /// <param name="e">Event arguments.</param>
    public override void Handle(HttpRestEventArgs e)
    {
        if(e.Path.StartsWith("/media"))
        {
            if((e.Path == "/media") && (e.Method == HttpMethod.Post))
            {
                try
                
                {
                    MediaEntry entry = new()
                    {
                        MediaType= e.Content?["type"]?.GetValue<string>() ?? string.Empty,
                        Title = e.Content?["title"]?.GetValue<string>() ?? string.Empty,
                        ReleaseYear = e.Content?["release"]?.GetValue<int>() ?? 0,
                        AgeRestriction = e.Content?["restriction"]?.GetValue<int>() ?? 0,
                        Genres = e.Content?["genres"]?.GetValue<string[]>() ?? [],
                    };

                    e.Respond(HttpStatusCode.OK, new JsonObject() { ["success"] = true, ["message"] = "Media Entry created." });

                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.WriteLine($"[{nameof(MediaHandler)} Handled {e.Method.ToString()} {e.Path}.");
                }
                catch(Exception ex)
                {
                    e.Respond(HttpStatusCode.InternalServerError, new JsonObject(){ ["success"] = false, ["reason"] = ex.Message });
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"[{nameof(MediaHandler)} Exception creating media entry. {e.Method.ToString()} {e.Path}: {ex.Message}");
                }
            }
            else              
            if((e.Path == "/media") && (e.Method == HttpMethod.Post))
            {
                try
                {
                    Session? session = Session.Create(e.Content["username"]?.GetValue<string>() ?? string.Empty, e.Content["password"]?.GetValue<string>() ?? string.Empty);

                    if(session is null)
                    {
                        e.Respond(HttpStatusCode.Unauthorized, new JsonObject() { ["success"] = false, ["reason"] = "Invalid username or password." });
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"[{nameof(MediaHandler)} Invalid login attempt. {e.Method.ToString()} {e.Path}.");
                    }
                    else
                    {
                        e.Respond(HttpStatusCode.OK, new JsonObject() { ["success"] = true, ["token"] = session.Token });
                        Console.ForegroundColor = ConsoleColor.Blue;
                        Console.WriteLine($"[{nameof(MediaHandler)} Handled {e.Method.ToString()} {e.Path}.");
                    }
                }
                catch(Exception ex)
                {
                    e.Respond(HttpStatusCode.InternalServerError, new JsonObject() { ["success"] = false, ["reason"] = ex.Message });
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"[{nameof(MediaHandler)} Exception creating session. {e.Method.ToString()} {e.Path}: {ex.Message}");
                }
            }

            else
            {
                e.Respond(HttpStatusCode.BadRequest, new JsonObject(){ ["success"] = false, ["reason"] = "Invalid media entry endpoint." });

                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"[{nameof(MediaHandler)} Invalid media entry endpoint.");
            }

            e.Responded = true;
        }
    }
}
