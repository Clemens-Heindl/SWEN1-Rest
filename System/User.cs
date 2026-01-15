using Clemens.SWEN1.Database;
using System.Collections;
using System.Data;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Serialization;

namespace Clemens.SWEN1.System;

public sealed class User: Atom, IAtom
{
    private string? _UserName = null;

    private bool _New;

    private string? _PasswordHash = null;

    private static UserDatabase _Repository = new();
    public static UserDatabase Repo
    {
        get { return _Repository; }
    }
    public User(Session? session = null)
    {
        _EditingSession = session;
        _New = false;
    }
    public User()
    {
        _New = true;
    }


    public User? Get(string userName, Session? session = null)
    {
        return _Repository.Get<string>(userName, session);
        
    }


    public string UserName
    {
        get { return _UserName ?? string.Empty; }
        set 
        {
            if(!_New) { throw new InvalidOperationException("User name cannot be changed."); }
            if(string.IsNullOrWhiteSpace(value)) { throw new ArgumentException("User name must not be empty."); }
            
            _UserName = value; 
        }
    }

    internal static string _HashPassword(string userName, string password)
    {
        StringBuilder rval = new();
        foreach(byte i in SHA256.HashData(Encoding.UTF8.GetBytes(userName + password)))
        {
            rval.Append(i.ToString("x2"));
        }
        return rval.ToString();
    }

    public string FullName
    {
        get; set;
    } = string.Empty;

    [JsonIgnore]
    public string PasswordHash
    {
        get => _PasswordHash ?? string.Empty;
    }

    [JsonIgnore]
    public bool isAdmin
    {
        get; set;
    } = false;



    public string EMail
    {
        get; set;
    } = string.Empty;

    public int Score
    {
        get; set;
    } = 0;


    public void SetPassword(string password)
    {
        _PasswordHash = _HashPassword(UserName, password);
    }

    public override void Save()
    {
        if(!_New) { _EnsureAdminOrOwner(UserName); }

        _Repository.Save(this);

        _PasswordHash = null;
        _EndEdit();
    }

    public override void Delete()
    {
        _EnsureAdminOrOwner(UserName);

        _EndEdit();
    }

    public override void Refresh()
    {
        _EndEdit();
    }
}
