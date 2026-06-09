using Npgsql;
using System;
using System.Collections.Generic;
using System.Text;

namespace ZeemanSport.Runtime.Helpers
{
    public static class DataReaderHelper
    {
        public static string? ReadNullableString(NpgsqlDataReader reader, string columnName)
        {
            int ordinal = reader.GetOrdinal(columnName);

            if (reader.IsDBNull(ordinal))
                return null;

            return reader.GetString(ordinal);
        }

        public static int? ReadNullableInt32(NpgsqlDataReader reader, string columnName)
        {
            int ordinal = reader.GetOrdinal(columnName);

            if (reader.IsDBNull(ordinal))
                return null;

            return reader.GetInt32(ordinal);
        }

        public static decimal? ReadNullableDecimal(NpgsqlDataReader reader, string columnName)
        {
            int ordinal = reader.GetOrdinal(columnName);

            if (reader.IsDBNull(ordinal))
                return null;

            return reader.GetDecimal(ordinal);
        }

        public static DateTime? ReadNullableDateTime(NpgsqlDataReader reader, string columnName)
        {
            int ordinal = reader.GetOrdinal(columnName);

            if (reader.IsDBNull(ordinal))
                return null;

            return reader.GetDateTime(ordinal);
        }
    }
}
