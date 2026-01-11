namespace Clemens.SWEN1.System;

static string? verifyToken(string authHeader){
        
    if (authHeader != null && authHeader.StartsWith("Bearer "))
    {
        // Extract token
        string token = authHeader["Bearer ".Length..].Trim();
        return Session.Get(token);
    } else
    {
        throw new UnauthorizedAccessException("Invalid Session Token.");
    }
}