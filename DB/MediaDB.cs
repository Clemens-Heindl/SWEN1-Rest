using System.Data;
using Npgsql;
using Clemens.SWEN1.System;
using System.Diagnostics.Tracing;

namespace Clemens.SWEN1.Database;



public sealed class MediaDatabase: Database<MediaEntry>, IDatabase<MediaEntry>
{

    protected override MediaEntry _CreateObject(IDataReader re)
    {
        MediaEntry rval = new();
        return _RefreshObject(re, rval);
    }


    protected override MediaEntry _RefreshObject(IDataReader re, MediaEntry obj)
    {
        if(re.Read()){
            obj.Creator = re.GetString(0);
            obj.Title = re.GetString(1);
            obj.MediaType = re.GetString(2);
            obj.Description = re.GetString(3);
            obj.ReleaseYear = re.GetInt32(4);
            obj.AgeRestriction = re.GetInt32(5);
        }
        return obj;
    }
    public override MediaEntry? Get(string id, Session? session = null)
    {   
        if(session ==  null) return null;
        var sql = "SELECT CREATOR, TITLE, MEDIATYPE, DESCRIPTION, RELEASEYEAR, AGERESTRICTION FROM MEDIA WHERE ID = @u";
        using var cmd = new NpgsqlCommand(sql, _Cn);
        cmd.Parameters.AddWithValue("u", id); 
        using var reader = cmd.ExecuteReader();
        if (!reader.HasRows)
        {
            Console.WriteLine("Media not found");
            return null;
        }
        return _CreateObject(reader);

        
    }


    public override IEnumerable<MediaEntry> GetAll(Session? session = null)
    {
        var sql = "SELECT CREATOR, TITLE, MEDIATYPE, DESCRIPTION, RELEASEYEAR, AGERESTRICTION FROM MEDIA";
        using var cmd = new NpgsqlCommand(sql, _Cn);
        using var reader = cmd.ExecuteReader();

        List<MediaEntry> rval = new List<MediaEntry>();
        while(reader.Read())
        {
            rval.Add(_CreateObject(reader));
        }

        return rval;
    }


    public override void Refresh(MediaEntry obj)
    {
        var sql = "SELECT CREATOR, TITLE, MEDIATYPE, DESCRIPTION, RELEASEYEAR, AGERESTRICTION FROM MEDIA WHERE ID = @u";
        using var cmd = new NpgsqlCommand(sql, _Cn);
        cmd.Parameters.AddWithValue("u", obj.ID);
        using var reader = cmd.ExecuteReader();
        _RefreshObject(reader, obj);

    }


    public override void Delete(MediaEntry obj)
    {

        String sql = "DELETE FROM MEDIA WHERE ID = @u";
        using var cmd = new NpgsqlCommand(sql, _Cn);
        cmd.Parameters.AddWithValue("u", obj.ID);
        cmd.ExecuteNonQuery();
    }


    public override void Save(MediaEntry obj)
    {
        if(obj != null)
        {
            if(string.IsNullOrWhiteSpace(obj?.Title))
            {
                throw new InvalidOperationException("MediaEntry name must not be empty.");
            }

            String sql = "INSERT INTO MEDIA (CREATOR, TITLE, MEDIATYPE, DESCRIPTION, RELEASEYEAR, AGERESTRICTION) VALUES (@u, @t, @n, @p, @e, @a)";
            using var cmd = new NpgsqlCommand(sql, _Cn);
            cmd.Parameters.AddWithValue("u", obj.Creator);
            cmd.Parameters.AddWithValue("t", obj.Title);
            cmd.Parameters.AddWithValue("n", obj.MediaType);
            cmd.Parameters.AddWithValue("p", obj.Description);
            cmd.Parameters.AddWithValue("e", obj.ReleaseYear);
            cmd.Parameters.AddWithValue("a", obj.AgeRestriction);
            cmd.ExecuteNonQuery();
            Console.WriteLine("MediaEntry saved successfully!");


        }
        else
        {
            throw new InvalidOperationException("MediaEntry must not be null.");
        }
    }
}