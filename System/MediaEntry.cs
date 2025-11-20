using System.Text;



namespace FHTW.Swen1.Forum.System;

public sealed class MediaEntry: Atom, IAtom
{
    private string? _MediaType = null;

    private bool _New;

    private string? _Title = null;

    private int? _ReleaseYear = null;

    private int? _AgeRestriction = null;

    private string[]? _Genres = null;



    public MediaEntry(Session? session = null)
    {
        _EditingSession = session;
        _New = true;
    }


    public override void Save()
    {
        _EndEdit();
    }

    public override void Delete()
    {
        _EndEdit();
    }

    public override void Refresh()
    {
        _EndEdit();
    }
}