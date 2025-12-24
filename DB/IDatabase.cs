using System.Collections;
using System.Data;
using System.Data.SQLite;

using Clemens.SWEN1.System;

namespace Clemens.SWEN1.Database;

public interface IDatabase<T> where T: IAtom, new()
{
    public new T? Get(string id, Session? session = null);

    public new IEnumerable<T> GetAll(Session? session = null);

    public void Refresh(T obj);

    public void Save(T obj);

    public void Delete(T obj);
}