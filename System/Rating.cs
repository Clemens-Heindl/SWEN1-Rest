using System.Text;



namespace FHTW.Swen1.Forum.System;

public sealed class Rating: Atom, IAtom
{   
    private MediaEntry? _Entry = null;

    private User? _Owner = null;

    private bool _New;

    private bool _Confirmation = false;

    private string? _Comment = null;

    private int? _Stars = null;

    private User[]? _LikedBy = null;



    public Rating(Session? session = null)
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