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

    public int Stars {
        get; set;
    } = 0;

    public int ID
    {
        get; set;
    }

    private User[]? _LikedBy = null;


    private static RatingDatabase _Repository = new();

    public static Rating? Get(string id, Session? session = null)
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
            if(string.IsNullOrWhiteSpace(value.Title)) { throw new ArgumentException("Title of media entry must not be empty."); }
            
            _Entry = value; 
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
        _EndEdit();
    }
}