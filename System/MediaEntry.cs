using System.Text;



namespace FHTW.Swen1.Forum.System;

public sealed class MediaEntry: Atom, IAtom
{
    private bool _New;

    private string _Creator = string.empty;

    private Rating[]? _Ratings = null;

    public string MediaType {
        get; set;
    } = string.empty;

    public string Title {
        get; set;
    } = string.empty;

    public int ReleaseYear {
        get; set;
    } = 0;

    public int AgeRestriction {
        get; set;
    } = 0;

    public string[] Genres {
        get; set;
    } = [];



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