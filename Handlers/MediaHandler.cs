using Clemens.SWEN1.Server;
using Clemens.SWEN1.System;
using System.Net;
using System.Text.Json;
using System.Text.Json.Nodes;



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
                        Genre = e.Content?["genre"]?.GetValue<string>() ?? string.Empty,
                        Description = e.Content?["description"]?.GetValue<string>() ?? string.Empty,
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
                    int ID = e.Content?["id"]?.GetValue<int>() ?? 0;
                    MediaEntry? entry = new(session)
                    {
                        ID = ID
                    };
                    entry.Refresh();
                    if (entry == null) 
                    { 
                        throw new InvalidOperationException("Entry doesnt exist"); 
                    }
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
                    int ID = e.Content?["id"]?.GetValue<int>() ?? 0;
                    MediaEntry? entry = new(session)
                    {
                        ID = ID
                    };
                    entry.Refresh();
                    if (entry == null)
                    {
                        throw new InvalidOperationException("Entry doesnt exist");
                    }
                    MediaEntry newData = new()
                    {
                        MediaType = e.Content?["type"]?.GetValue<string>() ?? string.Empty,
                        Title = e.Content?["title"]?.GetValue<string>() ?? string.Empty,
                        ReleaseYear = e.Content?["release"]?.GetValue<int>() ?? 0,
                        AgeRestriction = e.Content?["restriction"]?.GetValue<int>() ?? 0,
                        Genre = e.Content?["genre"]?.GetValue<string>() ?? string.Empty,
                        Description = e.Content?["description"]?.GetValue<string>() ?? string.Empty,
                    };
                    MediaEntry.Repo.Edit(ID, newData);
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
            } else 
            if ((e.Path == "/media") && (e.Method == HttpMethod.Get))
            {
                try
                {
                    string? authHeader = e.Context.Request.Headers["Authorization"];
                    Session? session = Session.verifyToken(authHeader);
                    string filter = e.Content?["filter"]?.GetValue<string>() ?? string.Empty;
                    string keyword = e.Content?["keyword"]?.GetValue<string>() ?? string.Empty;
                    IEnumerable<MediaEntry> entries = MediaEntry.Repo.Search(filter, keyword);
                    if (!entries.Any())
                    {
                        throw new InvalidOperationException("Search Query yielded no results");
                    }


                    e.Respond(HttpStatusCode.OK, new JsonObject() { ["success"] = true, ["result"] = JsonSerializer.SerializeToNode(entries) });

                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.WriteLine($"[{nameof(MediaHandler)} Handled {e.Method.ToString()} {e.Path}.");
                }
                catch (Exception ex)
                {
                    e.Respond(HttpStatusCode.InternalServerError, new JsonObject() { ["success"] = false, ["reason"] = ex.Message });
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"[{nameof(MediaHandler)} Exception searching media entry. {e.Method.ToString()} {e.Path}: {ex.Message}");
                }
            } else
            if ((e.Path == "/media/ratings") && (e.Method == HttpMethod.Get))
            {
                try
                {
                    string? authHeader = e.Context.Request.Headers["Authorization"];
                    Session? session = Session.verifyToken(authHeader);
                    int ID = e.Content?["id"]?.GetValue<int>() ?? 0;
                    IEnumerable<Rating> entries = Rating.Repo.Search(ID);
                    if (!entries.Any())
                    {
                        throw new InvalidOperationException("Media Entry has not yet been rated");
                    }

                    int totalStars = 0;
                    int ratingsCount = 0;
                    foreach(var rating in entries)
                    {
                        totalStars += rating.Stars;
                        ratingsCount++;
                    }


                    e.Respond(HttpStatusCode.OK, new JsonObject() { ["success"] = true, ["averageRating"] = (totalStars/ratingsCount), ["ratings"] = JsonSerializer.SerializeToNode(entries) });

                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.WriteLine($"[{nameof(MediaHandler)} Handled {e.Method.ToString()} {e.Path}.");
                }
                catch (Exception ex)
                {
                    e.Respond(HttpStatusCode.InternalServerError, new JsonObject() { ["success"] = false, ["reason"] = ex.Message });
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"[{nameof(MediaHandler)} Exception viewing media ratings. {e.Method.ToString()} {e.Path}: {ex.Message}");
                }
            } else
            if ((e.Path == "/media/favorites") && (e.Method == HttpMethod.Post))
            {
                try
                {
                    string? authHeader = e.Context.Request.Headers["Authorization"];
                    Session? session = Session.verifyToken(authHeader);
                    int ID = e.Content?["id"]?.GetValue<int>() ?? 0;
                    MediaEntry.Repo.Favorite(ID, session);


                    e.Respond(HttpStatusCode.OK, new JsonObject() { ["success"] = true, ["message"] = "Media Entry favorited." });

                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.WriteLine($"[{nameof(MediaHandler)} Handled {e.Method.ToString()} {e.Path}.");
                }
                catch (Exception ex)
                {
                    e.Respond(HttpStatusCode.InternalServerError, new JsonObject() { ["success"] = false, ["reason"] = ex.Message });
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"[{nameof(MediaHandler)} Exception favoriting media entry. {e.Method.ToString()} {e.Path}: {ex.Message}");
                }
            }
            else
            if ((e.Path == "/media/favorites") && (e.Method == HttpMethod.Delete))
            {
                try
                {
                    string? authHeader = e.Context.Request.Headers["Authorization"];
                    Session? session = Session.verifyToken(authHeader);
                    int ID = e.Content?["id"]?.GetValue<int>() ?? 0;
                    MediaEntry.Repo.UnFavorite(ID, session);


                    e.Respond(HttpStatusCode.OK, new JsonObject() { ["success"] = true, ["message"] = "Media Entry unfavorited." });

                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.WriteLine($"[{nameof(MediaHandler)} Handled {e.Method.ToString()} {e.Path}.");
                }
                catch (Exception ex)
                {
                    e.Respond(HttpStatusCode.InternalServerError, new JsonObject() { ["success"] = false, ["reason"] = ex.Message });
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"[{nameof(MediaHandler)} Exception unfavoriting media entry. {e.Method.ToString()} {e.Path}: {ex.Message}");
                }
            }
            else
            if ((e.Path == "/media/favorites") && (e.Method == HttpMethod.Get))
            {
                try
                {
                    string? authHeader = e.Context.Request.Headers["Authorization"];
                    Session? session = Session.verifyToken(authHeader);
                    IEnumerable<MediaEntry> entries = MediaEntry.Repo.GetFavorites(session);
                    if (!entries.Any())
                    {
                        throw new InvalidOperationException("No favorites found");
                    }


                    e.Respond(HttpStatusCode.OK, new JsonObject() { ["success"] = true, ["favorites"] = JsonSerializer.SerializeToNode(entries) });

                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.WriteLine($"[{nameof(MediaHandler)} Handled {e.Method.ToString()} {e.Path}.");
                }
                catch (Exception ex)
                {
                    e.Respond(HttpStatusCode.InternalServerError, new JsonObject() { ["success"] = false, ["reason"] = ex.Message });
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"[{nameof(MediaHandler)} Exception getting media favorites. {e.Method.ToString()} {e.Path}: {ex.Message}");
                }
            }
            else
            if ((e.Path == "/media/recommendation") && (e.Method == HttpMethod.Get))
            {
                try
                {
                    string? authHeader = e.Context.Request.Headers["Authorization"];
                    Session? session = Session.verifyToken(authHeader);
                    IEnumerable<MediaEntry> entries = MediaEntry.Repo.GetRecommendations(session);
                    if (!entries.Any())
                    {
                        throw new InvalidOperationException("No good Recommendations found");
                    }


                    e.Respond(HttpStatusCode.OK, new JsonObject() { ["success"] = true, ["Recommendation"] = JsonSerializer.SerializeToNode(entries) });

                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.WriteLine($"[{nameof(MediaHandler)} Handled {e.Method.ToString()} {e.Path}.");
                }
                catch (Exception ex)
                {
                    e.Respond(HttpStatusCode.InternalServerError, new JsonObject() { ["success"] = false, ["reason"] = ex.Message });
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"[{nameof(MediaHandler)} Exception getting recommendations. {e.Method.ToString()} {e.Path}: {ex.Message}");
                }
            }

            else
            {
                e.Respond(HttpStatusCode.BadRequest, new JsonObject() { ["success"] = false, ["reason"] = "Invalid media entry endpoint." });

                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"[{nameof(MediaHandler)} Invalid media entry endpoint.");
            }

            e.Responded = true;
        }
    }
}
