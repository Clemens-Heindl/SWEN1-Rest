using System.Text;



namespace FHTW.Swen1.Forum.System;

public sealed class Rating: Atom, IAtom
{
    private string? _Owner = null;

    private bool _New;

    private bool _Confirmation = false;

    private string? _Comment = null;

    private int? _Stars = null;

    private int? _AgeRestriction = null;

    private string[]? _LikedBy = null;



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