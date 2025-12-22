using System.Text;



namespace FHTW.Swen1.Forum.System;

public sealed class Rating: Atom, IAtom
{   
    private MediaEntry? _Entry = null;

    private string _Owner = string.empty;

    private bool _New;

    private bool _Confirmation = false;

    public string Comment {
        get; set;
    } = string.empty;

    public int Stars {
        get; set;
    } = 0;

    private User[]? _LikedBy = null;



    public Rating(Session? session = null)
    {
        _EditingSession = session;
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
            if(!_New) { throw new InvalidOperationException("Owner cannot be changed."); }
            if(string.IsNullOrWhiteSpace(value.title)) { throw new ArgumentException("Title of media entry must not be empty."); }
            
            _Entry = value; 
        }
    }

    public override void Save()
    {
        if(!_New) { _EnsureAdminOrOwner(Owner); }
        _EndEdit();
    }

    public override void Delete()
    {   
        _EnsureAdminOrOwner(Owner);
        _EndEdit();
    }

    public override void Refresh()
    {
        _EndEdit();
    }
}