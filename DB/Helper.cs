namespace Clemens.SWEN1.Database;

using global::System.Data;



internal static class _DBHelper
{
    public static IDbCommand BindParam(this IDbCommand cmd, string param, object? value)
    {
        IDataParameter p = cmd.CreateParameter();
        p.ParameterName = param;
        p.Value = value;
        cmd.Parameters.Add(p);

        return cmd;
    }


    public static string GetString(this IDataReader re, string fieldName)
    {
        int idx = re.GetOrdinal(fieldName);
        if(re.IsDBNull(idx)) { return string.Empty; }
        return re.GetString(idx);
    }


    public static int GetInt(this IDataReader re, string fieldName)
    {
        return re.GetInt32(re.GetOrdinal(fieldName));
    }


    public static bool GetBool(this IDataReader re, string fieldName)
    {
        return re.GetBoolean(re.GetOrdinal(fieldName));
    }
}