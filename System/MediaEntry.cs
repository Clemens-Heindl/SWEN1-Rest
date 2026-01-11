using System;



namespace Clemens.SWEN1.System;

public sealed class MediaEntry: Atom, IAtom
{
    private bool _New;

    private string _Creator = string.Empty;

    private Rating[]? _Ratings = null;

    private static MediaDatabase _Repository = new();

    public MediaEntry? Get(string id, Session? session = null)
    {
        return _Repository.Get(id, session);
        
    }

    public int ID {
        get; set;
    };

    public string MediaType {
        get; set;
    } = string.Empty;

    public string Title {
        get; set;
    } = string.Empty;

    public string Description {
        get; set;
    } = string.Empty;

    public int ReleaseYear {
        get; set;
    } = 0;

    public int AgeRestriction {
        get; set;
    } = 0;

    public string[] Genres {
        get; set;
    } = [];

    public MediaEntry()
    {
        _New = true;
    }

    public MediaEntry(Session? session = null)
    {
        _EditingSession = session;
        _New = true;
    }

    public string Creator
    {
        get { return _Creator; }
        set 
        {
            if(!_New) { throw new InvalidOperationException("Creator cannot be changed."); }
            if(string.IsNullOrWhiteSpace(value)) { throw new ArgumentException("User name of creator must not be empty."); }
            
            _Creator = value; 
        }
    }


    public override void Save()
    {
        if(!_New) { _EnsureAdminOrOwner(Creator); }
        _Repository.Save(this );
        _EndEdit();
    }

    public override void Delete()
    {   
        _EnsureAdminOrOwner(Creator);
        _EndEdit();
    }

    public override void Refresh()
    {
        _EndEdit();
    }
}