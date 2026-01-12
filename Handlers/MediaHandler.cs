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
                    string? authHeader = e.Context.Request.Headers["Authorization"];
                    Session? session = Session.verifyToken(authHeader);
                    MediaEntry entry = new()
                    {
                        Creator = session!.UserName,
                        MediaType= e.Content?["type"]?.GetValue<string>() ?? string.Empty,
                        Title = e.Content?["title"]?.GetValue<string>() ?? string.Empty,
                        ReleaseYear = e.Content?["release"]?.GetValue<int>() ?? 0,
                        AgeRestriction = e.Content?["restriction"]?.GetValue<int>() ?? 0,
                        Genres = e.Content?["genres"]?.GetValue<string[]>() ?? [],
                    };
                    entry.Save();

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
            if((e.Path == "/media") && (e.Method == HttpMethod.Delete))
            {
                try
                {
                    string? authHeader = e.Context.Request.Headers["Authorization"];
                    Session? session = Session.verifyToken(authHeader);
                    MediaEntry entry = new()
                    {
                        Creator = session!.UserName,
                        ID = e.Content?["id"]?.GetValue<int>() ?? 0,
                    };
                    entry.Delete();

                    e.Respond(HttpStatusCode.OK, new JsonObject() { ["success"] = true, ["message"] = "Media Entry deleted." });

                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.WriteLine($"[{nameof(MediaHandler)} Handled {e.Method.ToString()} {e.Path}.");
                }
                catch(Exception ex)
                {
                    e.Respond(HttpStatusCode.InternalServerError, new JsonObject() { ["success"] = false, ["reason"] = ex.Message });
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"[{nameof(MediaHandler)} Exception deleting media entry. {e.Method.ToString()} {e.Path}: {ex.Message}");
                }
            } else 
            if((e.Path == "/media") && (e.Method == HttpMethod.Put))
            {
                try
                
                {
                    string? authHeader = e.Context.Request.Headers["Authorization"];
                    Session? session = Session.verifyToken(authHeader);
                    MediaEntry entry = new(session)
                    {
                        Creator = session!.UserName,
                        MediaType= e.Content?["type"]?.GetValue<string>() ?? string.Empty,
                        Title = e.Content?["title"]?.GetValue<string>() ?? string.Empty,
                        ReleaseYear = e.Content?["release"]?.GetValue<int>() ?? 0,
                        AgeRestriction = e.Content?["restriction"]?.GetValue<int>() ?? 0,
                        Genres = e.Content?["genres"]?.GetValue<string[]>() ?? [],
                    };
                    entry.Save();

                    e.Respond(HttpStatusCode.OK, new JsonObject() { ["success"] = true, ["message"] = "Media Entry edited." });

                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.WriteLine($"[{nameof(MediaHandler)} Handled {e.Method.ToString()} {e.Path}.");
                }
                catch(Exception ex)
                {
                    e.Respond(HttpStatusCode.InternalServerError, new JsonObject(){ ["success"] = false, ["reason"] = ex.Message });
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"[{nameof(MediaHandler)} Exception editing media entry. {e.Method.ToString()} {e.Path}: {ex.Message}");
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
