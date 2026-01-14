using Clemens.SWEN1.Server;
using Clemens.SWEN1.System;
using System.Net;
using System.Text.Json;
using System.Text.Json.Nodes;



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
                    MediaEntry? entry = new()
                    {
                        ID = e.Content?["entry"]?.GetValue<int>() ?? 0
                    };
                    entry.Refresh();
                    Rating rating = new()
                    {
                        Owner = session!.UserName,
                        Entry = entry,
                        Comment = e.Content?["comment"]?.GetValue<string>() ?? string.Empty,
                        Stars = e.Content?["stars"]?.GetValue<int>() ?? 0,
                    };
                    if (rating.Exists()) throw new InvalidOperationException("Rating already exists");
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
                    Rating? entry = new(session)
                    {
                        ID = ID
                    };
                    entry.Refresh();
                    if (entry == null)
                    {
                        throw new InvalidOperationException("Rating doesnt exist");
                    }
                    entry.Delete();
                    e.Respond(HttpStatusCode.OK, new JsonObject() { ["success"] = true, ["message"] = "Rating deleted." });

                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.WriteLine($"[{nameof(RatingHandler)} Handled {e.Method.ToString()} {e.Path}.");
                }
                catch(Exception ex)
                {
                    e.Respond(HttpStatusCode.InternalServerError, new JsonObject() { ["success"] = false, ["reason"] = ex.Message });
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"[{nameof(RatingHandler)} Exception deleting rating. {e.Method.ToString()} {e.Path}: {ex.Message}");
                }
            } else 
            if((e.Path == "/ratings") && (e.Method == HttpMethod.Put))
            {
                try
                
                {
                    string? authHeader = e.Context.Request.Headers["Authorization"];
                    Session? session = Session.verifyToken(authHeader);
                    int ID = e.Content?["id"]?.GetValue<int>() ?? 0;
                    Rating? entry = new(session)
                    {
                        ID = ID
                    };
                    entry.Refresh();
                    if (entry == null)
                    {
                        throw new InvalidOperationException("Rating doesnt exist");
                    }
                    Rating newData = new()
                    {
                        Comment = e.Content?["comment"]?.GetValue<string>() ?? string.Empty,
                        Stars = e.Content?["stars"]?.GetValue<int>() ?? 0,
                    };
                    Rating.Repo.Edit(ID, newData);
                    e.Respond(HttpStatusCode.OK, new JsonObject() { ["success"] = true, ["message"] = "Rating edited." });

                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.WriteLine($"[{nameof(RatingHandler)} Handled {e.Method.ToString()} {e.Path}.");
                }
                catch(Exception ex)
                {
                    e.Respond(HttpStatusCode.InternalServerError, new JsonObject(){ ["success"] = false, ["reason"] = ex.Message });
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"[{nameof(RatingHandler)} Exception updating rating. {e.Method.ToString()} {e.Path}: {ex.Message}");
                }
            }
            else
            if ((e.Path == "/ratings") && (e.Method == HttpMethod.Get))
            {
                try

                {
                    string? authHeader = e.Context.Request.Headers["Authorization"];
                    Session? session = Session.verifyToken(authHeader);
                    IEnumerable<Rating> entries = Rating.Repo.GetRatingHistory(session);
                    if (!entries.Any())
                    {
                        throw new InvalidOperationException("No ratings found");
                    }

                    e.Respond(HttpStatusCode.OK, new JsonObject() { ["success"] = true, ["ratings"] = JsonSerializer.SerializeToNode(entries) });

                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.WriteLine($"[{nameof(RatingHandler)} Handled {e.Method.ToString()} {e.Path}.");
                }
                catch (Exception ex)
                {
                    e.Respond(HttpStatusCode.InternalServerError, new JsonObject() { ["success"] = false, ["reason"] = ex.Message });
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"[{nameof(RatingHandler)} Exception viewing ratings. {e.Method.ToString()} {e.Path}: {ex.Message}");
                }
            }
            else
            if ((e.Path == "/ratings/confirm") && (e.Method == HttpMethod.Put))
            {
                try

                {
                    string? authHeader = e.Context.Request.Headers["Authorization"];
                    Session? session = Session.verifyToken(authHeader);
                    int ID = e.Content?["id"]?.GetValue<int>() ?? 0;
                    Rating? entry = new(session)
                    {
                        ID = ID
                    };
                    entry.Refresh();
                    entry.Entry.Refresh();
                    if (entry == null)
                    {
                        throw new InvalidOperationException("Rating doesnt exist");
                    }
                    entry._Confirmation = true;
                    Rating.Repo.Edit(ID, entry);
                    e.Respond(HttpStatusCode.OK, new JsonObject() { ["success"] = true, ["message"] = "Rating confirmed." });

                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.WriteLine($"[{nameof(RatingHandler)} Handled {e.Method.ToString()} {e.Path}.");
                }
                catch (Exception ex)
                {
                    e.Respond(HttpStatusCode.InternalServerError, new JsonObject() { ["success"] = false, ["reason"] = ex.Message });
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"[{nameof(RatingHandler)} Exception confirming rating. {e.Method.ToString()} {e.Path}: {ex.Message}");
                }
            }
            else
            if ((e.Path == "/ratings/like") && (e.Method == HttpMethod.Post))
            {
                try

                {
                    string? authHeader = e.Context.Request.Headers["Authorization"];
                    Session? session = Session.verifyToken(authHeader);
                    int ID = e.Content?["id"]?.GetValue<int>() ?? 0;
                    Rating.Repo.Like(ID, session);
                    e.Respond(HttpStatusCode.OK, new JsonObject() { ["success"] = true, ["message"] = "Rating liked." });

                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.WriteLine($"[{nameof(RatingHandler)} Handled {e.Method.ToString()} {e.Path}.");
                }
                catch (Exception ex)
                {
                    e.Respond(HttpStatusCode.InternalServerError, new JsonObject() { ["success"] = false, ["reason"] = ex.Message });
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"[{nameof(RatingHandler)} Exception liking rating. {e.Method.ToString()} {e.Path}: {ex.Message}");
                }
            }
            else
            if ((e.Path == "/ratings/like") && (e.Method == HttpMethod.Delete))
            {
                try

                {
                    string? authHeader = e.Context.Request.Headers["Authorization"];
                    Session? session = Session.verifyToken(authHeader);
                    int ID = e.Content?["id"]?.GetValue<int>() ?? 0;
                    Rating.Repo.UnLike(ID, session);
                    e.Respond(HttpStatusCode.OK, new JsonObject() { ["success"] = true, ["message"] = "Rating unliked." });

                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.WriteLine($"[{nameof(RatingHandler)} Handled {e.Method.ToString()} {e.Path}.");
                }
                catch (Exception ex)
                {
                    e.Respond(HttpStatusCode.InternalServerError, new JsonObject() { ["success"] = false, ["reason"] = ex.Message });
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"[{nameof(RatingHandler)} Exception unliking rating. {e.Method.ToString()} {e.Path}: {ex.Message}");
                }
            }


            else
            {
                e.Respond(HttpStatusCode.BadRequest, new JsonObject() { ["success"] = false, ["reason"] = "Invalid ratings endpoint." });

                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"[{nameof(RatingHandler)} Invalid ratings endpoint.");
            }

            e.Responded = true;
        }
    }
}
