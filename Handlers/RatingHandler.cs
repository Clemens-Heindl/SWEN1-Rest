using System.Net;
using System.Text.Json.Nodes;

using Clemens.SWEN1.System;
using Clemens.SWEN1.Server;



namespace Clemens.SWEN1.Handlers;

/// <summary>This class implements a Handler for media entry endpoints.</summary>
public sealed class RatingHandler: Handler, IHandler
{

    
    /// <summary>Handles a request if possible.</summary>
    /// <param name="e">Event arguments.</param>
    public override void Handle(HttpRestEventArgs e)
    {
        if(e.Path.StartsWith("/ratings"))
        {
            if((e.Path == "/ratings") && (e.Method == HttpMethod.Post))
            {
                try
                
                {
                    string? authHeader = Request.Headers["Authorization"];
                    Session? session = verifyToken(authHeader);
                    Rating rating = new()
                    {
                        owner = session.UserName;
                        Comment = e.Content?["comment"]?.GetValue<string>() ?? string.Empty,
                        Stars = e.Content?["stars"]?.GetValue<int>() ?? 0,
                    };
                    rating.Save();
                    e.Respond(HttpStatusCode.OK, new JsonObject() { ["success"] = true, ["message"] = "Rating created." });

                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.WriteLine($"[{nameof(RatingHandler)} Handled {e.Method.ToString()} {e.Path}.");
                }
                catch(Exception ex)
                {
                    e.Respond(HttpStatusCode.InternalServerError, new JsonObject(){ ["success"] = false, ["reason"] = ex.Message });
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"[{nameof(RatingHandler)} Exception creating rating. {e.Method.ToString()} {e.Path}: {ex.Message}");
                }
            }
            else 
            if((e.Path == "/ratings") && (e.Method == HttpMethod.Post))
            {
                try
                {
                    Session? session = Session.Create(e.Content["username"]?.GetValue<string>() ?? string.Empty, e.Content["password"]?.GetValue<string>() ?? string.Empty);

                    if(session is null)
                    {
                        e.Respond(HttpStatusCode.Unauthorized, new JsonObject() { ["success"] = false, ["reason"] = "Invalid username or password." });
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"[{nameof(RatingHandler)} Invalid login attempt. {e.Method.ToString()} {e.Path}.");
                    }
                    else
                    {
                        e.Respond(HttpStatusCode.OK, new JsonObject() { ["success"] = true, ["token"] = session.Token });
                        Console.ForegroundColor = ConsoleColor.Blue;
                        Console.WriteLine($"[{nameof(RatingHandler)} Handled {e.Method.ToString()} {e.Path}.");
                    }
                }
                catch(Exception ex)
                {
                    e.Respond(HttpStatusCode.InternalServerError, new JsonObject() { ["success"] = false, ["reason"] = ex.Message });
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"[{nameof(RatingHandler)} Exception creating session. {e.Method.ToString()} {e.Path}: {ex.Message}");
                }
            }

            else
            {
                e.Respond(HttpStatusCode.BadRequest, new JsonObject(){ ["success"] = false, ["reason"] = "Invalid media entry endpoint." });

                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"[{nameof(RatingHandler)} Invalid ratings endpoint.");
            }

            e.Responded = true;
        }
    }
}
