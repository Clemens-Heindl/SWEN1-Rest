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
        obj.ID = re.GetInt32(0);
        obj.Creator = re.GetString(1);
        obj.Title = re.GetString(2);
        obj.MediaType = re.GetString(3);
        obj.Description = re.GetString(4);
        obj.ReleaseYear = re.GetInt32(5);
        obj.AgeRestriction = re.GetInt32(6);
        obj.Genre = re.GetString(7);
        return obj;
    }
    public override MediaEntry? Get<Tid>(Tid id, Session? session = null)
    {   
        var sql = "SELECT ID, CREATOR, TITLE, MEDIATYPE, DESCRIPTION, RELEASEYEAR, AGERATING, GENRE FROM MEDIA WHERE ID = @u";
        using var cmd = new NpgsqlCommand(sql, _Cn);
        cmd.Parameters.AddWithValue("u", id); 
        using var reader = cmd.ExecuteReader();
        if (!reader.HasRows)
        {
            Console.WriteLine("Media not found");
            return null;
        }
        if(!reader.Read()) return null;
        return _CreateObject(reader);

        
    }


    public override IEnumerable<MediaEntry> GetAll(Session? session = null)
    {
        var sql = "SELECT ID, CREATOR, TITLE, MEDIATYPE, DESCRIPTION, RELEASEYEAR, AGERATING, GENRE FROM MEDIA";
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
        var sql = "SELECT ID, CREATOR, TITLE, MEDIATYPE, DESCRIPTION, RELEASEYEAR, AGERATING, GENRE FROM MEDIA WHERE ID = @u";
        using var cmd = new NpgsqlCommand(sql, _Cn);
        cmd.Parameters.AddWithValue("u", obj.ID);
        using var reader = cmd.ExecuteReader();
        if (!reader.Read()) return;
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

            String sql = "INSERT INTO MEDIA (CREATOR, TITLE, MEDIATYPE, DESCRIPTION, RELEASEYEAR, AGERATING, GENRE) VALUES (@u, @t, @n, @p, @e, @a, @g)";
            using var cmd = new NpgsqlCommand(sql, _Cn);
            cmd.Parameters.AddWithValue("u", obj.Creator);
            cmd.Parameters.AddWithValue("t", obj.Title);
            cmd.Parameters.AddWithValue("n", obj.MediaType);
            cmd.Parameters.AddWithValue("p", obj.Description);
            cmd.Parameters.AddWithValue("e", obj.ReleaseYear);
            cmd.Parameters.AddWithValue("a", obj.AgeRestriction);
            cmd.Parameters.AddWithValue("g", obj.Genre);
            cmd.ExecuteNonQuery();
            Console.WriteLine("MediaEntry saved successfully!");


        }
        else
        {
            throw new InvalidOperationException("MediaEntry must not be null.");
        }
    }
    public void Edit(int id, MediaEntry obj)
    {
        if (obj != null)
        {
            if (string.IsNullOrWhiteSpace(obj?.Title))
            {
                throw new InvalidOperationException("MediaEntry name must not be empty.");
            }

            String sql = "UPDATE MEDIA SET TITLE = @t, MEDIATYPE = @n, DESCRIPTION = @p, RELEASEYEAR = @e, AGERATING = @a, GENRE = @g WHERE ID = @u";
            using var cmd = new NpgsqlCommand(sql, _Cn);
            cmd.Parameters.AddWithValue("u", id);
            cmd.Parameters.AddWithValue("t", obj.Title);
            cmd.Parameters.AddWithValue("n", obj.MediaType);
            cmd.Parameters.AddWithValue("p", obj.Description);
            cmd.Parameters.AddWithValue("e", obj.ReleaseYear);
            cmd.Parameters.AddWithValue("a", obj.AgeRestriction);
            cmd.Parameters.AddWithValue("g", obj.Genre);
            cmd.ExecuteNonQuery();
            Console.WriteLine("MediaEntry edited successfully!");


        }
        else
        {
            throw new InvalidOperationException("MediaEntry must not be null.");
        }
    }
    public IEnumerable<MediaEntry> Search(string filter, string keyword)
    {
        var allowedFilters = new HashSet<string>
        {
            "id",
            "creator",
            "title",
            "mediatype",
            "releaseyear",
            "agerating",
            "genre"
        };

        if (!allowedFilters.Contains(filter.ToLower()))
            throw new ArgumentException("Invalid filter, use one of: id, creator, title, mediatype, releaseyear, agerating or genre");


        var sql = $"SELECT ID, CREATOR, TITLE, MEDIATYPE, DESCRIPTION, RELEASEYEAR, AGERATING, GENRE FROM MEDIA WHERE {filter} = @k";
        using var cmd = new NpgsqlCommand(sql, _Cn);

        //Convert to int
        int numKey = 0;
        if ((filter == "id") || (filter == "releaseyear") || (filter == "agerating"))
        {
            numKey = Int32.Parse(keyword);
            cmd.Parameters.AddWithValue("k", numKey);
        } else
        {
            cmd.Parameters.AddWithValue("k", keyword);
        }
            
        using var reader = cmd.ExecuteReader();

        List<MediaEntry> rval = new List<MediaEntry>();
        while (reader.Read())
        {
            rval.Add(_CreateObject(reader));
        }

        return rval;
    }
    public void Favorite(int id, Session? session)
    {
        if (session != null)
        {
            String sql1 = "SELECT * FROM MEDIA WHERE ID = @i";
            using var cmd1 = new NpgsqlCommand(sql1, _Cn);
            cmd1.Parameters.AddWithValue("i", id);
            var exists = cmd1.ExecuteScalar();
            if (exists == null)
            {
                throw new InvalidOperationException("Media you are trying to favorite doesnt exist");
            }

            String sql2 = "SELECT * FROM FAVORITES WHERE MEDIA = @i AND USERNAME = @u";
            using var cmd2 = new NpgsqlCommand(sql2, _Cn);
            cmd2.Parameters.AddWithValue("i", id);
            cmd2.Parameters.AddWithValue("u", session.UserName);
            var exists2 = cmd2.ExecuteScalar();
            if (exists2 != null)
            {
                throw new InvalidOperationException("Media already favorited");
            }


            String sql = "INSERT INTO FAVORITES (USERNAME, MEDIA) VALUES (@u, @i)";
            using var cmd = new NpgsqlCommand(sql, _Cn);
            cmd.Parameters.AddWithValue("u", session.UserName);
            cmd.Parameters.AddWithValue("i", id);
            cmd.ExecuteNonQuery();
            Console.WriteLine("MediaEntry favorited successfully!");


        }
        else
        {
            throw new InvalidOperationException("Invalid Session (favorite).");
        }
    }

    public void UnFavorite(int id, Session? session)
    {
        if (session != null)
        {


            String sql = "DELETE FROM FAVORITES WHERE USERNAME = @u AND MEDIA = @i";
            using var cmd = new NpgsqlCommand(sql, _Cn);
            cmd.Parameters.AddWithValue("u", session.UserName);
            cmd.Parameters.AddWithValue("i", id);
            cmd.ExecuteNonQuery();
            Console.WriteLine("MediaEntry unfavorited successfully!");


        }
        else
        {
            throw new InvalidOperationException("Invalid Session (unfavorite).");
        }
    }

    public IEnumerable<MediaEntry> GetFavorites(Session? session)
    {
        if (session != null)
        {


            String sql = "SELECT MEDIA.ID, CREATOR, TITLE, MEDIATYPE, DESCRIPTION, RELEASEYEAR, AGERATING, GENRE FROM MEDIA JOIN FAVORITES ON MEDIA.ID = FAVORITES.MEDIA WHERE FAVORITES.USERNAME = @u";
            using var cmd = new NpgsqlCommand(sql, _Cn);
            cmd.Parameters.AddWithValue("u", session.UserName);
            cmd.ExecuteNonQuery();
            using var reader = cmd.ExecuteReader();

            List<MediaEntry> rval = new List<MediaEntry>();
            while (reader.Read())
            {
                rval.Add(_CreateObject(reader));
            }

            return rval;


        }
        else
        {
            throw new InvalidOperationException("Invalid Session (getFavorites).");
        }
    }
    public IEnumerable<MediaEntry> GetRecommendations(Session? session)
    {
        if (session != null)
        {

            //get favorite genre
            String sql = "SELECT MEDIA.GENRE FROM RATINGS JOIN MEDIA ON RATINGS.ENTRY = MEDIA.ID WHERE RATINGS.OWNER = @u AND RATINGS.STARS >= 3 LIMIT 1";
            using var cmd = new NpgsqlCommand(sql, _Cn);
            cmd.Parameters.AddWithValue("u", session.UserName);
            cmd.ExecuteNonQuery();
            var genre = cmd.ExecuteScalar();
            if(genre == null)
            {
                throw new InvalidOperationException("No Ratings to base Recommendations on found");
            }

            String sql2 = "SELECT DISTINCT MEDIA.ID, CREATOR, TITLE, MEDIATYPE, DESCRIPTION, RELEASEYEAR, AGERATING, GENRE FROM MEDIA JOIN RATINGS ON RATINGS.ENTRY = MEDIA.ID WHERE GENRE = @g AND RATINGS.STARS >= 3 AND RATINGS.OWNER != @u";
            using var cmd2 = new NpgsqlCommand(sql2, _Cn);
            cmd2.Parameters.AddWithValue("g", genre);
            cmd2.Parameters.AddWithValue("u", session.UserName);
            cmd2.ExecuteNonQuery();
            using var reader = cmd.ExecuteReader();


            List<MediaEntry> rval = new List<MediaEntry>();
            while (reader.Read())
            {
                rval.Add(_CreateObject(reader));
            }

            return rval;


        }
        else
        {
            throw new InvalidOperationException("Invalid Session (getRecommendations).");
        }
    }

}