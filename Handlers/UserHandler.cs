using System.Net;
using System.Text.Json.Nodes;

using FHTW.Swen1.Forum.System;
using FHTW.Swen1.Forum.Server;



namespace FHTW.Swen1.Forum.Handlers;

/// <summary>This class implements a Handler for user endpoints.</summary>
public sealed class UserHandler: Handler, IHandler
{

    
    /// <summary>Handles a request if possible.</summary>
    /// <param name="e">Event arguments.</param>
    public override void Handle(HttpRestEventArgs e)
    {
        if(e.Path.StartsWith("/users"))
        {
            if((e.Path == "/users/register") && (e.Method == HttpMethod.Post))
            {
                try
                
                {
                    User user = new()
                    {
                        UserName = e.Content?["username"]?.GetValue<string>() ?? string.Empty,
                        FullName = e.Content?["fullname"]?.GetValue<string>() ?? string.Empty,
                        EMail = e.Content?["email"]?.GetValue<string>() ?? string.Empty
                    };
                    user.SetPassword(e.Content?["password"]?.GetValue<string>() ?? string.Empty);

                    e.Respond(HttpStatusCode.OK, new JsonObject() { ["success"] = true, ["message"] = "User created." });

                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.WriteLine($"[{nameof(VersionHandler)} Handled {e.Method.ToString()} {e.Path}.");
                }
                catch(Exception ex)
                {
                    e.Respond(HttpStatusCode.InternalServerError, new JsonObject(){ ["success"] = false, ["reason"] = ex.Message });
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"[{nameof(VersionHandler)} Exception creating user. {e.Method.ToString()} {e.Path}: {ex.Message}");
                }
            }
            else              
            if((e.Path == "/users/login") && (e.Method == HttpMethod.Post))
            {
                try
                {
                    Session? session = Session.Create(e.Content["username"]?.GetValue<string>() ?? string.Empty, e.Content["password"]?.GetValue<string>() ?? string.Empty);

                    if(session is null)
                    {
                        e.Respond(HttpStatusCode.Unauthorized, new JsonObject() { ["success"] = false, ["reason"] = "Invalid username or password." });
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"[{nameof(VersionHandler)} Invalid login attempt. {e.Method.ToString()} {e.Path}.");
                    }
                    else
                    {
                        e.Respond(HttpStatusCode.OK, new JsonObject() { ["success"] = true, ["token"] = session.Token });
                        Console.ForegroundColor = ConsoleColor.Blue;
                        Console.WriteLine($"[{nameof(VersionHandler)} Handled {e.Method.ToString()} {e.Path}.");
                    }
                }
                catch(Exception ex)
                {
                    e.Respond(HttpStatusCode.InternalServerError, new JsonObject() { ["success"] = false, ["reason"] = ex.Message });
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"[{nameof(VersionHandler)} Exception creating session. {e.Method.ToString()} {e.Path}: {ex.Message}");
                }
            }

            else
            {
                e.Respond(HttpStatusCode.BadRequest, new JsonObject(){ ["success"] = false, ["reason"] = "Invalid user endpoint." });

                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"[{nameof(VersionHandler)} Invalid user endpoint.");
            }

            e.Responded = true;
        }
    }
}
