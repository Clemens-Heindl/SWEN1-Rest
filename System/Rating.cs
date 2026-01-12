using System.Text;
using Clemens.SWEN1.System;
using Clemens.SWEN1.Database;


namespace Clemens.SWEN1.System;

public sealed class Rating: Atom, IAtom
{   
    private MediaEntry? _Entry = null;

    private string _Owner = string.Empty;

    private bool _New;

    public bool _Confirmation{
        get; set;
    } = false;

    public string Comment {
        get; set;
    } = string.Empty;

    private int _stars = 0;

    public int Stars
    {
        get => _stars;
        set => _stars = Math.Clamp(value, 0, 5);
    }

    public int ID
    {
        get; set;
    }

    private User[]? _LikedBy = null;


    private static RatingDatabase _Repository = new();

    public static RatingDatabase Repo
    {
        get { return _Repository; }
    }

    public static Rating? Get(int id, Session? session = null)
    {
        return _Repository.Get(id, session);
        
    }
    public Rating(Session? session = null)
    {
        _EditingSession = session;
        _New = true;
    }
    public Rating()
    {
        _New = true;
    }

    public string Owner
    {
        get { return _Owner; }
        set 
        {
            if(!_New) { throw new InvalidOperationException("Owner cannot be changed."); }
            if(string.IsNullOrWhiteSpace(value)) { throw new ArgumentException("User name of owner must not be empty."); }
            
            _Owner = value; 
        }
    }

    public MediaEntry Entry
    {
        get { return _Entry; }
        set 
        {
            if(!_New) { throw new InvalidOperationException("Entry cannot be changed."); }
            
            _Entry = value; 
        }
    }

    protected override void _EnsureAdminOrOwner(string owner)
    {
        _VerifySession();
        bool confirmed = (_EditingSession.UserName == (this.Entry?.Creator ?? ""));
        if (!(_EditingSession!.IsAdmin || (_EditingSession.UserName == owner) || confirmed))
        {
            throw new UnauthorizedAccessException("Admin or owner privileges required.");
        }
    }

    public override void Save()
    {
        if(!_New) { _EnsureAdminOrOwner(Owner); }
        _Repository.Save(this);
        _EndEdit();
    }

    public override void Delete()
    {   
        _EnsureAdminOrOwner(Owner);
        _Repository.Delete(this);
        _EndEdit();
    }

    public override void Refresh()
    {
        _Repository.Refresh(this);
        _EnsureAdminOrOwner(Owner);
    }

    public bool Exists()
    {
        return _Repository.Exists(this);
    }
}