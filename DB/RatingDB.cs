using System.Data;
using Npgsql;
using Clemens.SWEN1.System;
using System.Diagnostics.Tracing;

namespace Clemens.SWEN1.Database;



public sealed class RatingDatabase: Database<Rating>, IDatabase<Rating>
{

    protected override Rating _CreateObject(IDataReader re)
    {
        Rating rval = new();
        return _RefreshObject(re, rval);
    }


    protected override Rating _RefreshObject(IDataReader re, Rating obj)
    {
        if(re.Read()){
            obj.Owner = re.GetString(0);
            obj.Comment = re.GetString(1);
            obj.Stars = re.GetInt32(3);
            obj.Entry = MediaEntry.Get(re.GetString(4));
        }
        return obj;
    }
    public override Rating? Get(string id, Session? session = null)
    {   
        if(session ==  null) return null;
        var sql = "SELECT OWNER, COMMENT, STARS, ENTRY FROM RATINGS WHERE ID = @u";
        using var cmd = new NpgsqlCommand(sql, _Cn);
        cmd.Parameters.AddWithValue("u", id); 
        using var reader = cmd.ExecuteReader();
        if (!reader.HasRows)
        {
            Console.WriteLine("Rating not found");
            return null;
        }
        return _CreateObject(reader);

        
    }


    public override IEnumerable<Rating> GetAll(Session? session = null)
    {
        var sql = "SELECT OWNER, COMMENT, STARS, ENTRY FROM RATINGS";
        using var cmd = new NpgsqlCommand(sql, _Cn);
        using var reader = cmd.ExecuteReader();

        List<Rating> rval = new List<Rating>();
        while(reader.Read())
        {
            rval.Add(_CreateObject(reader));
        }

        return rval;
    }


    public override void Refresh(Rating obj)
    {
        var sql = "SELECT OWNER, COMMENT, STARS, ENTRY FROM RATINGS WHERE ID = @u";
        using var cmd = new NpgsqlCommand(sql, _Cn);
        cmd.Parameters.AddWithValue("u", obj.ID);
        using var reader = cmd.ExecuteReader();
        _RefreshObject(reader, obj);

    }


    public override void Delete(Rating obj)
    {

        String sql = "DELETE FROM RATINGS WHERE ID = @u";
        using var cmd = new NpgsqlCommand(sql, _Cn);
        cmd.Parameters.AddWithValue("u", obj.ID);
        cmd.ExecuteNonQuery();
    }


    public override void Save(Rating obj)
    {
        if(obj != null)
        {

            String sql = "INSERT INTO RATINGS (OWNER, COMMENT, CONFIRMATION, STARS, ENTRY) VALUES (@u, @n, @p, @e, @a)";
            using var cmd = new NpgsqlCommand(sql, _Cn);
            cmd.Parameters.AddWithValue("u", obj.Owner);
            cmd.Parameters.AddWithValue("n", obj.Comment);
            cmd.Parameters.AddWithValue("p", obj._Confirmation);
            cmd.Parameters.AddWithValue("e", obj.Stars);
            cmd.Parameters.AddWithValue("a", obj.Entry);
            cmd.ExecuteNonQuery();
            Console.WriteLine("Rating saved successfully!");


        }
        else
        {
            throw new InvalidOperationException("Rating must not be null.");
        }
    }
}