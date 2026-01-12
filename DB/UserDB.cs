using System.Data;
using Npgsql;
using Clemens.SWEN1.System;
using System.Diagnostics.Tracing;

namespace Clemens.SWEN1.Database;



public sealed class UserDatabase: Database<User>, IDatabase<User>
{

    protected override User _CreateObject(IDataReader re)
    {
        User rval = new();
        return _RefreshObject(re, rval);
    }


    protected override User _RefreshObject(IDataReader re, User obj)
    {
        if(re.Read()){
            obj.UserName = re.GetString(0);
            obj.FullName = re.GetString(1);
            obj.EMail = re.GetString(2);
            obj.isAdmin = re.GetBoolean(3);
        }
        return obj;
    }
    public override User? Get<Tid>(Tid id, Session? session = null)
    {   
        if(session ==  null) return null;
        var sql = "SELECT USERNAME, NAME, EMAIL, HADMIN FROM USERS WHERE USERNAME = @u AND PASSWD = @p";
        using var cmd = new NpgsqlCommand(sql, _Cn);
        cmd.Parameters.AddWithValue("u", id); 
        cmd.Parameters.AddWithValue("p", session.Hash);
        using var reader = cmd.ExecuteReader();
        if (!reader.HasRows)
        {
            Console.WriteLine("Invalid Password");
            return null;
        }
        return _CreateObject(reader);

        
    }


    public override IEnumerable<User> GetAll(Session? session = null)
    {
        var sql = "SELECT USERNAME, NAME, EMAIL, HADMIN FROM USERS";
        using var cmd = new NpgsqlCommand(sql, _Cn);
        using var reader = cmd.ExecuteReader();

        List<User> rval = new List<User>();
        while(reader.Read())
        {
            rval.Add(_CreateObject(reader));
        }

        return rval;
    }


    public override void Refresh(User obj)
    {
        var sql = "SELECT USERNAME, NAME, EMAIL, HADMIN FROM USERS WHERE USERNAME = @u";
        using var cmd = new NpgsqlCommand(sql, _Cn);
        cmd.Parameters.AddWithValue("u", obj.UserName);
        using var reader = cmd.ExecuteReader();
        _RefreshObject(reader, obj);

    }


    public override void Delete(User obj)
    {

        String sql = "DELETE FROM USERS WHERE USERNAME = @u";
        using var cmd = new NpgsqlCommand(sql, _Cn);
        cmd.Parameters.AddWithValue("u", obj.UserName);
        cmd.ExecuteNonQuery();
    }


    public override void Save(User obj)
    {
        if(obj != null)
        {
            if(string.IsNullOrWhiteSpace(obj?.UserName))
            {
                throw new InvalidOperationException("User name must not be empty.");
            }
            if(string.IsNullOrWhiteSpace(obj.PasswordHash))
            {
                throw new InvalidOperationException("Password must not be empty.");
            }

            String sql = "INSERT INTO USERS (USERNAME, NAME, PASSWD, EMAIL, HADMIN) VALUES (@u, @n, @p, @e, @a)";
            using var cmd = new NpgsqlCommand(sql, _Cn);
            cmd.Parameters.AddWithValue("u", obj.UserName);
            cmd.Parameters.AddWithValue("n", obj.FullName);
            cmd.Parameters.AddWithValue("p", obj.PasswordHash);
            cmd.Parameters.AddWithValue("e", obj.EMail);
            cmd.Parameters.AddWithValue("a", obj.isAdmin);
            cmd.ExecuteNonQuery();
            Console.WriteLine("User saved successfully!");


        }
        else
        {
            throw new InvalidOperationException("User must not be null.");
        }
    }
    public void Edit(string id, User obj)
    {
        if (obj != null)
        {
            if (string.IsNullOrWhiteSpace(obj.PasswordHash))
            {
                throw new InvalidOperationException("Password must not be empty.");
            }

            String sql = "UPDATE USERS SET NAME =  @n, PASSWD = @p, EMAIL = @e, HADMIN = @a WHERE USERNAME = @u";
            using var cmd = new NpgsqlCommand(sql, _Cn);
            cmd.Parameters.AddWithValue("u", id);
            cmd.Parameters.AddWithValue("n", obj.FullName);
            cmd.Parameters.AddWithValue("p", obj.PasswordHash);
            cmd.Parameters.AddWithValue("e", obj.EMail);
            cmd.Parameters.AddWithValue("a", obj.isAdmin);
            cmd.ExecuteNonQuery();
            Console.WriteLine("User updated successfully!");


        }
        else
        {
            throw new InvalidOperationException("User must not be null.");
        }
    }
}