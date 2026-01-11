using System.Collections;
using System.Data;
using Npgsql;

using Clemens.SWEN1.System;

namespace Clemens.SWEN1.Database;


public abstract class Database<T>: IDatabase<T> where T: IAtom, new()
{
    private static NpgsqlConnection? _DbConnection;
    

    protected static NpgsqlConnection _Cn
    {
        get
        {
            if(_DbConnection == null) 
            {   
                var conectionString =  "Host=localhost;Port=5432;Username=postgres;Password=mysecretpassword;Database=postgres";
                _DbConnection = new NpgsqlConnection(conectionString);
                _DbConnection.Open();
                Console.WriteLine("Connected to PostgreSQL!");
            }

            return _DbConnection;
        }
    }

    protected abstract T _RefreshObject(IDataReader re, T rval);
    
    protected abstract T _CreateObject(IDataReader re);

    public abstract T? Get(string id, Session? session = null);

    public abstract IEnumerable<T> GetAll(Session? session = null);

    public abstract void Refresh(T obj);

    public abstract void Save(T obj);

    public abstract void Delete(T obj);


    T? IDatabase<T>.Get(string id, Session? session)
    {
        return Get(id, session);
    }

    IEnumerable<T> IDatabase<T>.GetAll(Session? session)
    {
        return GetAll(session);
    }

    void IDatabase<T>.Refresh(T obj)
    {
        Refresh((T) obj);
    }

    void IDatabase<T>.Save(T obj)
    {
        Save((T) obj);
    }

    void IDatabase<T>.Delete(T obj)
    {
        Delete((T) obj);
    }
}