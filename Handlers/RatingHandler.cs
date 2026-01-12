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
                    string? authHeader = e.Context.Request.Headers["Authorization"];
                    Session? session = Session.verifyToken(authHeader);
                    Rating rating = new()
                    {
                        Owner = session!.UserName,
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
            if((e.Path == "/ratings") && (e.Method == HttpMethod.Delete))
            {
                try
                {
                    string? authHeader = e.Context.Request.Headers["Authorization"];
                    Session? session = Session.verifyToken(authHeader);
                    int ID = e.Content?["id"]?.GetValue<int>() ?? 0;
                    Rating? rating = Rating.Get(ID, session);
                    if (rating == null)
                    {
                        throw new InvalidOperationException("Rating doesnt exist");
                    }
                    rating.Delete();
                    e.Respond(HttpStatusCode.OK, new JsonObject() { ["success"] = true, ["message"] = "Rating created." });

                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.WriteLine($"[{nameof(RatingHandler)} Handled {e.Method.ToString()} {e.Path}.");
                }
                catch(Exception ex)
                {
                    e.Respond(HttpStatusCode.InternalServerError, new JsonObject() { ["success"] = false, ["reason"] = ex.Message });
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"[{nameof(RatingHandler)} Exception creating session. {e.Method.ToString()} {e.Path}: {ex.Message}");
                }
            } else 
            if((e.Path == "/ratings") && (e.Method == HttpMethod.Put))
            {
                try
                
                {
                    string? authHeader = e.Context.Request.Headers["Authorization"];
                    Session? session = Session.verifyToken(authHeader);
                    Rating rating = new(session)
                    {
                        Owner = session!.UserName,
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
            {
                e.Respond(HttpStatusCode.BadRequest, new JsonObject(){ ["success"] = false, ["reason"] = "Invalid media entry endpoint." });

                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"[{nameof(RatingHandler)} Invalid ratings endpoint.");
            }

            e.Responded = true;
        }
    }
}
