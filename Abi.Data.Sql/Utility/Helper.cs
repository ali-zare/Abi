using System;
using System.Text;
using System.Reflection;
using System.Data.SqlClient;
using System.Collections.Generic;
using Abi.Types;

namespace Abi.Data.Sql
{
    internal static class Helper
    {
        static Helper()
        {
            hex_chrL = new[] { '0', '0', '0', '0', '0', '0', '0', '0', '0', '0', '0', '0', '0', '0', '0', '0', '1', '1', '1', '1', '1', '1', '1', '1', '1', '1', '1', '1', '1', '1', '1', '1', '2', '2', '2', '2', '2', '2', '2', '2', '2', '2', '2', '2', '2', '2', '2', '2', '3', '3', '3', '3', '3', '3', '3', '3', '3', '3', '3', '3', '3', '3', '3', '3', '4', '4', '4', '4', '4', '4', '4', '4', '4', '4', '4', '4', '4', '4', '4', '4', '5', '5', '5', '5', '5', '5', '5', '5', '5', '5', '5', '5', '5', '5', '5', '5', '6', '6', '6', '6', '6', '6', '6', '6', '6', '6', '6', '6', '6', '6', '6', '6', '7', '7', '7', '7', '7', '7', '7', '7', '7', '7', '7', '7', '7', '7', '7', '7', '8', '8', '8', '8', '8', '8', '8', '8', '8', '8', '8', '8', '8', '8', '8', '8', '9', '9', '9', '9', '9', '9', '9', '9', '9', '9', '9', '9', '9', '9', '9', '9', 'A', 'A', 'A', 'A', 'A', 'A', 'A', 'A', 'A', 'A', 'A', 'A', 'A', 'A', 'A', 'A', 'B', 'B', 'B', 'B', 'B', 'B', 'B', 'B', 'B', 'B', 'B', 'B', 'B', 'B', 'B', 'B', 'C', 'C', 'C', 'C', 'C', 'C', 'C', 'C', 'C', 'C', 'C', 'C', 'C', 'C', 'C', 'C', 'D', 'D', 'D', 'D', 'D', 'D', 'D', 'D', 'D', 'D', 'D', 'D', 'D', 'D', 'D', 'D', 'E', 'E', 'E', 'E', 'E', 'E', 'E', 'E', 'E', 'E', 'E', 'E', 'E', 'E', 'E', 'E', 'F', 'F', 'F', 'F', 'F', 'F', 'F', 'F', 'F', 'F', 'F', 'F', 'F', 'F', 'F', 'F' };
            hex_chrR = new[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F' };

            nonFixedSizeSqlType = new Dictionary<SqlType, string>
            {
               { SqlType.DateTime2, "datetime2"},
               { SqlType.Time, "time"},
               { SqlType.DateTimeOffset, "datetimeoffset"},

               { SqlType.Char, "char"},
               { SqlType.NChar, "nchar"},
               { SqlType.Binary, "binary"},

               { SqlType.VarChar, "varchar"},
               { SqlType.NVarChar, "nvarchar"},
               { SqlType.VarBinary, "varbinary"},
            };
            map_ClrType_SqlType = new Dictionary<Type, SqlType>
            {
                {typeof(byte[]), SqlType.VarBinary},
                {typeof(string), SqlType.NVarChar},

                {typeof(char), SqlType.NVarChar},
                {typeof(bool), SqlType.Bit},
                {typeof(byte), SqlType.TinyInt},
                {typeof(short), SqlType.SmallInt},
                {typeof(int), SqlType.Int},
                {typeof(long), SqlType.BigInt},
                {typeof(float), SqlType.Real},
                {typeof(double), SqlType.Float},
                {typeof(decimal), SqlType.Decimal},
                {typeof(Guid), SqlType.UniqueIdentifier},
                {typeof(TimeSpan), SqlType.Time},
                {typeof(DateTime), SqlType.DateTime2},
                {typeof(RowVersion), SqlType.Timestamp},

                {typeof(char?), SqlType.NVarChar},
                {typeof(bool?), SqlType.Bit},
                {typeof(byte?), SqlType.TinyInt},
                {typeof(short?), SqlType.SmallInt},
                {typeof(int?), SqlType.Int},
                {typeof(long?), SqlType.BigInt},
                {typeof(float?), SqlType.Real},
                {typeof(double?), SqlType.Float},
                {typeof(decimal?), SqlType.Decimal},
                {typeof(Guid?), SqlType.UniqueIdentifier},
                {typeof(TimeSpan?), SqlType.Time},
                {typeof(DateTime?), SqlType.DateTime2},
                {typeof(RowVersion?), SqlType.Timestamp},
            };
            map_SqlType_PrmTypeStr = new Dictionary<SqlType, string>
            {
                {SqlType.BigInt, "bigint"},
                {SqlType.Binary, "binary(max)"},
                {SqlType.Bit, "bit"},
                {SqlType.Char, "char(8000)"},
                {SqlType.DateTime, "datetime"},
                {SqlType.Decimal, "decimal"},
                {SqlType.Float, "float"},
                {SqlType.Image, "image"},
                {SqlType.Int, "int"},
                {SqlType.Money, "money"},
                {SqlType.NChar, "nchar(4000)"},
                {SqlType.NText, "ntext"},
                {SqlType.NVarChar, "nvarchar(4000)"},
                {SqlType.Real, "real"},
                {SqlType.UniqueIdentifier, "UniqueIdentifier"},
                {SqlType.SmallDateTime, "smalldatetime"},
                {SqlType.SmallInt, "smallint"},
                {SqlType.SmallMoney, "smallmoney"},
                {SqlType.Text, "text"},
                {SqlType.Timestamp, "rowversion"},
                {SqlType.TinyInt, "tinyint"},
                {SqlType.VarBinary, "varbinary(max)"},
                {SqlType.VarChar, "varchar(8000)"},
                {SqlType.Variant, "variant"},
                {SqlType.Xml, "xml"},
                {SqlType.Date, "date"},
                {SqlType.Time, "time(7)"},
                {SqlType.DateTime2, "datetime2(7)"},
                {SqlType.DateTimeOffset, "datetimeoffset(7)"},
                {SqlType.RowVersion, "rowversion"},
            };
            map_ClrType_PrmVal_MthInfo = new Dictionary<Type, MethodInfo>
            {
                {typeof(byte[]), typeof(Helper).GetMethod(nameof(SetParameterValue), BindingFlags.NonPublic | BindingFlags.Static, null, new[] { typeof(byte[]) }, null)},
                {typeof(string), typeof(Helper).GetMethod(nameof(SetParameterValue), BindingFlags.NonPublic | BindingFlags.Static, null, new[] { typeof(string) }, null)},

                {typeof(char), typeof(Helper).GetMethod(nameof(SetParameterValue), BindingFlags.NonPublic | BindingFlags.Static, null, new[] { typeof(char) }, null)},
                {typeof(bool), typeof(Helper).GetMethod(nameof(SetParameterValue), BindingFlags.NonPublic | BindingFlags.Static, null, new[] { typeof(bool) }, null)},
                {typeof(byte), typeof(Helper).GetMethod(nameof(SetParameterValue), BindingFlags.NonPublic | BindingFlags.Static, null, new[] { typeof(byte) }, null)},
                {typeof(short), typeof(Helper).GetMethod(nameof(SetParameterValue), BindingFlags.NonPublic | BindingFlags.Static, null, new[] { typeof(short) }, null)},
                {typeof(int), typeof(Helper).GetMethod(nameof(SetParameterValue), BindingFlags.NonPublic | BindingFlags.Static, null, new[] { typeof(int) }, null)},
                {typeof(long), typeof(Helper).GetMethod(nameof(SetParameterValue), BindingFlags.NonPublic | BindingFlags.Static, null, new[] { typeof(long) }, null)},
                {typeof(float), typeof(Helper).GetMethod(nameof(SetParameterValue), BindingFlags.NonPublic | BindingFlags.Static, null, new[] { typeof(float) }, null)},
                {typeof(double), typeof(Helper).GetMethod(nameof(SetParameterValue), BindingFlags.NonPublic | BindingFlags.Static, null, new[] { typeof(double) }, null)},
                {typeof(decimal), typeof(Helper).GetMethod(nameof(SetParameterValue), BindingFlags.NonPublic | BindingFlags.Static, null, new[] { typeof(decimal) }, null)},
                {typeof(Guid), typeof(Helper).GetMethod(nameof(SetParameterValue), BindingFlags.NonPublic | BindingFlags.Static, null, new[] { typeof(Guid) }, null)},
                {typeof(TimeSpan), typeof(Helper).GetMethod(nameof(SetParameterValue), BindingFlags.NonPublic | BindingFlags.Static, null, new[] { typeof(TimeSpan) }, null)},
                {typeof(DateTime), typeof(Helper).GetMethod(nameof(SetParameterValue), BindingFlags.NonPublic | BindingFlags.Static, null, new[] { typeof(DateTime) }, null)},
                {typeof(RowVersion), typeof(Helper).GetMethod(nameof(SetParameterValue), BindingFlags.NonPublic | BindingFlags.Static, null, new[] { typeof(RowVersion) }, null)},

                {typeof(char?), typeof(Helper).GetMethod(nameof(SetParameterValue), BindingFlags.NonPublic | BindingFlags.Static, null, new[] { typeof(char?) }, null)},
                {typeof(bool?), typeof(Helper).GetMethod(nameof(SetParameterValue), BindingFlags.NonPublic | BindingFlags.Static, null, new[] { typeof(bool?) }, null)},
                {typeof(byte?), typeof(Helper).GetMethod(nameof(SetParameterValue), BindingFlags.NonPublic | BindingFlags.Static, null, new[] { typeof(byte?) }, null)},
                {typeof(short?), typeof(Helper).GetMethod(nameof(SetParameterValue), BindingFlags.NonPublic | BindingFlags.Static, null, new[] { typeof(short?) }, null)},
                {typeof(int?), typeof(Helper).GetMethod(nameof(SetParameterValue), BindingFlags.NonPublic | BindingFlags.Static, null, new[] { typeof(int?) }, null)},
                {typeof(long?), typeof(Helper).GetMethod(nameof(SetParameterValue), BindingFlags.NonPublic | BindingFlags.Static, null, new[] { typeof(long?) }, null)},
                {typeof(float?), typeof(Helper).GetMethod(nameof(SetParameterValue), BindingFlags.NonPublic | BindingFlags.Static, null, new[] { typeof(float?) }, null)},
                {typeof(double?), typeof(Helper).GetMethod(nameof(SetParameterValue), BindingFlags.NonPublic | BindingFlags.Static, null, new[] { typeof(double?) }, null)},
                {typeof(decimal?), typeof(Helper).GetMethod(nameof(SetParameterValue), BindingFlags.NonPublic | BindingFlags.Static, null, new[] { typeof(decimal?) }, null)},
                {typeof(Guid?), typeof(Helper).GetMethod(nameof(SetParameterValue), BindingFlags.NonPublic | BindingFlags.Static, null, new[] { typeof(Guid?) }, null)},
                {typeof(TimeSpan?), typeof(Helper).GetMethod(nameof(SetParameterValue), BindingFlags.NonPublic | BindingFlags.Static, null, new[] { typeof(TimeSpan?) }, null)},
                {typeof(DateTime?), typeof(Helper).GetMethod(nameof(SetParameterValue), BindingFlags.NonPublic | BindingFlags.Static, null, new[] { typeof(DateTime?) }, null)},
                {typeof(RowVersion?), typeof(Helper).GetMethod(nameof(SetParameterValue), BindingFlags.NonPublic | BindingFlags.Static, null, new[] { typeof(RowVersion?) }, null)},
            };
            map_ClrType_DtRdrVal_MthInfo = new Dictionary<Type, MethodInfo>
            {
                {typeof(byte[]), typeof(Helper).GetMethod(nameof(GetBytes), BindingFlags.NonPublic | BindingFlags.Static, null, new[] { typeof(SqlDataReader), typeof(int) }, null)},
                {typeof(string), typeof(Helper).GetMethod(nameof(GetString), BindingFlags.NonPublic | BindingFlags.Static, null, new[] { typeof(SqlDataReader), typeof(int) }, null)},

                {typeof(char), typeof(Helper).GetMethod(nameof(GetChar), BindingFlags.NonPublic | BindingFlags.Static, null, new[] { typeof(SqlDataReader), typeof(int) }, null)},
                {typeof(bool), typeof(Helper).GetMethod(nameof(GetBool), BindingFlags.NonPublic | BindingFlags.Static, null, new[] { typeof(SqlDataReader), typeof(int) }, null)},
                {typeof(byte), typeof(Helper).GetMethod(nameof(GetByte), BindingFlags.NonPublic | BindingFlags.Static, null, new[] { typeof(SqlDataReader), typeof(int) }, null)},
                {typeof(short), typeof(Helper).GetMethod(nameof(GetShort), BindingFlags.NonPublic | BindingFlags.Static, null, new[] { typeof(SqlDataReader), typeof(int) }, null)},
                {typeof(int), typeof(Helper).GetMethod(nameof(GetInt), BindingFlags.NonPublic | BindingFlags.Static, null, new[] { typeof(SqlDataReader), typeof(int) }, null)},
                {typeof(long), typeof(Helper).GetMethod(nameof(GetLong), BindingFlags.NonPublic | BindingFlags.Static, null, new[] { typeof(SqlDataReader), typeof(int) }, null)},
                {typeof(float), typeof(Helper).GetMethod(nameof(GetFloat), BindingFlags.NonPublic | BindingFlags.Static, null, new[] { typeof(SqlDataReader), typeof(int) }, null)},
                {typeof(double), typeof(Helper).GetMethod(nameof(GetDouble), BindingFlags.NonPublic | BindingFlags.Static, null, new[] { typeof(SqlDataReader), typeof(int) }, null)},
                {typeof(decimal), typeof(Helper).GetMethod(nameof(GetDecimal), BindingFlags.NonPublic | BindingFlags.Static, null, new[] { typeof(SqlDataReader), typeof(int) }, null)},
                {typeof(Guid), typeof(Helper).GetMethod(nameof(GetGuid), BindingFlags.NonPublic | BindingFlags.Static, null, new[] { typeof(SqlDataReader), typeof(int) }, null)},
                {typeof(TimeSpan), typeof(Helper).GetMethod(nameof(GetTimeSpan), BindingFlags.NonPublic | BindingFlags.Static, null, new[] { typeof(SqlDataReader), typeof(int) }, null)},
                {typeof(DateTime), typeof(Helper).GetMethod(nameof(GetDateTime), BindingFlags.NonPublic | BindingFlags.Static, null, new[] { typeof(SqlDataReader), typeof(int) }, null)},
                {typeof(RowVersion), typeof(Helper).GetMethod(nameof(GetRowVersion), BindingFlags.NonPublic | BindingFlags.Static, null, new[] { typeof(SqlDataReader), typeof(int) }, null)},

                {typeof(char?), typeof(Helper).GetMethod(nameof(GetNullableChar), BindingFlags.NonPublic | BindingFlags.Static, null, new[] {typeof(SqlDataReader), typeof(int) }, null)},
                {typeof(bool?), typeof(Helper).GetMethod(nameof(GetNullableBool), BindingFlags.NonPublic | BindingFlags.Static, null, new[] {typeof(SqlDataReader), typeof(int) }, null)},
                {typeof(byte?), typeof(Helper).GetMethod(nameof(GetNullableByte), BindingFlags.NonPublic | BindingFlags.Static, null, new[] { typeof(SqlDataReader), typeof(int) }, null)},
                {typeof(short?), typeof(Helper).GetMethod(nameof(GetNullableShort), BindingFlags.NonPublic | BindingFlags.Static, null, new[] { typeof(SqlDataReader), typeof(int) }, null)},
                {typeof(int?), typeof(Helper).GetMethod(nameof(GetNullableInt), BindingFlags.NonPublic | BindingFlags.Static, null, new[] { typeof(SqlDataReader), typeof(int) }, null)},
                {typeof(long?), typeof(Helper).GetMethod(nameof(GetNullableLong), BindingFlags.NonPublic | BindingFlags.Static, null, new[] { typeof(SqlDataReader), typeof(int) }, null)},
                {typeof(float?), typeof(Helper).GetMethod(nameof(GetNullableFloat), BindingFlags.NonPublic | BindingFlags.Static, null, new[] { typeof(SqlDataReader), typeof(int) }, null)},
                {typeof(double?), typeof(Helper).GetMethod(nameof(GetNullableDouble), BindingFlags.NonPublic | BindingFlags.Static, null, new[] { typeof(SqlDataReader), typeof(int) }, null)},
                {typeof(decimal?), typeof(Helper).GetMethod(nameof(GetNullableDecimal), BindingFlags.NonPublic | BindingFlags.Static, null, new[] { typeof(SqlDataReader), typeof(int) }, null)},
                {typeof(Guid?), typeof(Helper).GetMethod(nameof(GetNullableGuid), BindingFlags.NonPublic | BindingFlags.Static, null, new[] { typeof(SqlDataReader), typeof(int) }, null)},
                {typeof(TimeSpan?), typeof(Helper).GetMethod(nameof(GetNullableTimeSpan), BindingFlags.NonPublic | BindingFlags.Static, null, new[] { typeof(SqlDataReader), typeof(int) }, null)},
                {typeof(DateTime?), typeof(Helper).GetMethod(nameof(GetNullableDateTime), BindingFlags.NonPublic | BindingFlags.Static, null, new[] { typeof(SqlDataReader), typeof(int) }, null)},
                {typeof(RowVersion?), typeof(Helper).GetMethod(nameof(GetNullableRowVersion), BindingFlags.NonPublic | BindingFlags.Static, null, new[] { typeof(SqlDataReader), typeof(int) }, null)},
            };
            map_ClrType_ExeSclrVal_MthInfo = new Dictionary<Type, MethodInfo>
            {
                {typeof(byte[]), typeof(Helper).GetMethod(nameof(GetBytes), BindingFlags.NonPublic | BindingFlags.Static, null, new[] { typeof(object) }, null)},
                {typeof(string), typeof(Helper).GetMethod(nameof(GetString), BindingFlags.NonPublic | BindingFlags.Static, null, new[] { typeof(object) }, null)},

                {typeof(char), typeof(Helper).GetMethod(nameof(GetChar), BindingFlags.NonPublic | BindingFlags.Static, null, new[] { typeof(object) }, null)},
                {typeof(bool), typeof(Helper).GetMethod(nameof(GetBool), BindingFlags.NonPublic | BindingFlags.Static, null, new[] { typeof(object) }, null)},
                {typeof(byte), typeof(Helper).GetMethod(nameof(GetByte), BindingFlags.NonPublic | BindingFlags.Static, null, new[] { typeof(object) }, null)},
                {typeof(short), typeof(Helper).GetMethod(nameof(GetShort), BindingFlags.NonPublic | BindingFlags.Static, null, new[] { typeof(object) }, null)},
                {typeof(int), typeof(Helper).GetMethod(nameof(GetInt), BindingFlags.NonPublic | BindingFlags.Static, null, new[] { typeof(object) }, null)},
                {typeof(long), typeof(Helper).GetMethod(nameof(GetLong), BindingFlags.NonPublic | BindingFlags.Static, null, new[] { typeof(object) }, null)},
                {typeof(float), typeof(Helper).GetMethod(nameof(GetFloat), BindingFlags.NonPublic | BindingFlags.Static, null, new[] { typeof(object) }, null)},
                {typeof(double), typeof(Helper).GetMethod(nameof(GetDouble), BindingFlags.NonPublic | BindingFlags.Static, null, new[] { typeof(object) }, null)},
                {typeof(decimal), typeof(Helper).GetMethod(nameof(GetDecimal), BindingFlags.NonPublic | BindingFlags.Static, null, new[] { typeof(object) }, null)},
                {typeof(Guid), typeof(Helper).GetMethod(nameof(GetGuid), BindingFlags.NonPublic | BindingFlags.Static, null, new[] { typeof(object) }, null)},
                {typeof(TimeSpan), typeof(Helper).GetMethod(nameof(GetTimeSpan), BindingFlags.NonPublic | BindingFlags.Static, null, new[] { typeof(object) }, null)},
                {typeof(DateTime), typeof(Helper).GetMethod(nameof(GetDateTime), BindingFlags.NonPublic | BindingFlags.Static, null, new[] { typeof(object) }, null)},
                {typeof(RowVersion), typeof(Helper).GetMethod(nameof(GetRowVersion), BindingFlags.NonPublic | BindingFlags.Static, null, new[] { typeof(object) }, null)},

                {typeof(char?), typeof(Helper).GetMethod(nameof(GetNullableChar), BindingFlags.NonPublic | BindingFlags.Static, null, new[] {typeof(object) }, null)},
                {typeof(bool?), typeof(Helper).GetMethod(nameof(GetNullableBool), BindingFlags.NonPublic | BindingFlags.Static, null, new[] {typeof(object) }, null)},
                {typeof(byte?), typeof(Helper).GetMethod(nameof(GetNullableByte), BindingFlags.NonPublic | BindingFlags.Static, null, new[] { typeof(object) }, null)},
                {typeof(short?), typeof(Helper).GetMethod(nameof(GetNullableShort), BindingFlags.NonPublic | BindingFlags.Static, null, new[] { typeof(object) }, null)},
                {typeof(int?), typeof(Helper).GetMethod(nameof(GetNullableInt), BindingFlags.NonPublic | BindingFlags.Static, null, new[] { typeof(object) }, null)},
                {typeof(long?), typeof(Helper).GetMethod(nameof(GetNullableLong), BindingFlags.NonPublic | BindingFlags.Static, null, new[] { typeof(object) }, null)},
                {typeof(float?), typeof(Helper).GetMethod(nameof(GetNullableFloat), BindingFlags.NonPublic | BindingFlags.Static, null, new[] { typeof(object) }, null)},
                {typeof(double?), typeof(Helper).GetMethod(nameof(GetNullableDouble), BindingFlags.NonPublic | BindingFlags.Static, null, new[] { typeof(object) }, null)},
                {typeof(decimal?), typeof(Helper).GetMethod(nameof(GetNullableDecimal), BindingFlags.NonPublic | BindingFlags.Static, null, new[] { typeof(object) }, null)},
                {typeof(Guid?), typeof(Helper).GetMethod(nameof(GetNullableGuid), BindingFlags.NonPublic | BindingFlags.Static, null, new[] { typeof(object) }, null)},
                {typeof(TimeSpan?), typeof(Helper).GetMethod(nameof(GetNullableTimeSpan), BindingFlags.NonPublic | BindingFlags.Static, null, new[] { typeof(object) }, null)},
                {typeof(DateTime?), typeof(Helper).GetMethod(nameof(GetNullableDateTime), BindingFlags.NonPublic | BindingFlags.Static, null, new[] { typeof(object) }, null)},
                {typeof(RowVersion?), typeof(Helper).GetMethod(nameof(GetNullableRowVersion), BindingFlags.NonPublic | BindingFlags.Static, null, new[] { typeof(object) }, null)},
            };

            map_ClrType_DcmlCnvrtTo_MthInfo = new Dictionary<Type, MethodInfo>
            {
                { typeof(short), typeof(decimal).GetMethod(nameof(decimal.ToInt16)) },
                { typeof(int), typeof(decimal).GetMethod(nameof(decimal.ToInt32)) },
                { typeof(long), typeof(decimal).GetMethod(nameof(decimal.ToInt64)) },
            };
        }

        private static char[] hex_chrL;
        private static char[] hex_chrR;
        private static Dictionary<SqlType, string> nonFixedSizeSqlType;
        private static Dictionary<Type, SqlType> map_ClrType_SqlType;
        private static Dictionary<SqlType, string> map_SqlType_PrmTypeStr;
        private static Dictionary<Type, MethodInfo> map_ClrType_PrmVal_MthInfo;
        private static Dictionary<Type, MethodInfo> map_ClrType_DtRdrVal_MthInfo;
        private static Dictionary<Type, MethodInfo> map_ClrType_ExeSclrVal_MthInfo;
        private static Dictionary<Type, MethodInfo> map_ClrType_DcmlCnvrtTo_MthInfo;




        internal static string ToHexString(this byte[] Buffer)
        {
            char[] chrs = new char[(Buffer.Length * 2) + 2];

            chrs[0] = '0';
            chrs[1] = 'x';

            int index = 0;

            for (int i = 2; i < chrs.Length; i += 2)
            {
                byte b = Buffer[index++];
                chrs[i] = hex_chrL[b];
                chrs[i + 1] = hex_chrR[b];
            }

            return new string(chrs);
        }

        internal static bool IsFixedSize(this SqlType SqlType)
        {
            return !nonFixedSizeSqlType.ContainsKey(SqlType);
        }
        internal static SqlType GetSqlType(this PropertyInfo Property)
        {
            return map_ClrType_SqlType[Property.PropertyType];
        }
        internal static string GetParameterType(this SqlType SqlType, int? Size)
        {
            return (Size != null && !SqlType.IsFixedSize()) ? $"{nonFixedSizeSqlType[SqlType]}({Size})" : map_SqlType_PrmTypeStr[SqlType];
        }

        internal static ParameterDefinition GetParameterDefinition(this PropertyInfo Property)
        {
            return DatabaseParameterDefinition.Default.GetParameterDefinition(Property);
        }
        internal static IEnumerable<ParameterDefinition> GetParameterDefinition(this IEnumerable<PropertyInfo> Properties)
        {
            foreach (PropertyInfo property in Properties)
                yield return property.GetParameterDefinition();
        }

        internal static ParameterDefinition GetParameterDefinition<TEntityContext>(this PropertyInfo Property) where TEntityContext : EntityContext<TEntityContext>
        {
            return DatabaseParameterDefinition<TEntityContext>.Default.GetParameterDefinition(Property);
        }
        internal static IEnumerable<ParameterDefinition> GetParameterDefinition<TEntityContext>(this IEnumerable<PropertyInfo> Properties) where TEntityContext : EntityContext<TEntityContext>
        {
            foreach (PropertyInfo property in Properties)
                yield return property.GetParameterDefinition<TEntityContext>();
        }

        internal static IEnumerable<PropertyInfo> Map(this IEnumerable<PropertyInfo> Source, IEnumerable<PropertyInfo> Target)
        {
            foreach (PropertyInfo source in Source)
                foreach (PropertyInfo target in Target)
                    if (source.Name == target.Name)
                        yield return target;
        }

        internal static MethodInfo GetDecimalConvertTo(this PropertyInfo Property)
        {
            return map_ClrType_DcmlCnvrtTo_MthInfo[Property.PropertyType];
        }

        private static int IndexOfFirstNonWhiteSpace(this string s)
        {
            int i = 0;

            while (i < s.Length)
            {
                if (!char.IsWhiteSpace(s[i]))
                    return i;

                i++;
            }

            return -1;
        }
        internal static string CommandTextDecorator(this string commandText, int indent)
        {
            StringBuilder builder = new StringBuilder();

            string[] lines = commandText.Split('\n');

            int index = 0;

            foreach (string line in lines)
            {
                if (index == 0)
                    index = line.IndexOfFirstNonWhiteSpace();

                if (line.Trim(' ', '\n', '\r') == string.Empty)
                    index = 0;

                int i = line.IndexOfFirstNonWhiteSpace();

                if (index > 0 && i >= index)
                    builder.Append(new string(' ', indent)).Append(line.Remove(0, index));
                else
                    builder.Append(line);
            }

            return builder.ToString();
        }

        internal static bool IsUserDefinedTableType(this PropertyInfo Property)
        {
            return Property.PropertyType.IsGenericType && Property.PropertyType.GetGenericTypeDefinition() == typeof(UserDefinedTableType<>);
        }








        internal static MethodInfo GetSetParameterValueMethod(this Type Type)
        {
            return map_ClrType_PrmVal_MthInfo[Type];
        }
        internal static MethodInfo GetSetParameterValueMethod(this PropertyInfo Property)
        {
            return Property.PropertyType.GetSetParameterValueMethod();
        }

        internal static string SetParameterValue(this byte[] value) => value == null ? "null" : $"{value.ToHexString()}";
        internal static string SetParameterValue(this string value) => value == null ? "null" : $"N'{value.Replace("'", "''")}'";

        internal static string SetParameterValue(this char value) => $"N'{value}'";
        internal static string SetParameterValue(this bool value) => value.ToString();
        internal static string SetParameterValue(this byte value) => value.ToString();
        internal static string SetParameterValue(this short value) => value.ToString();
        internal static string SetParameterValue(this int value) => value.ToString();
        internal static string SetParameterValue(this long value) => value.ToString();
        internal static string SetParameterValue(this float value) => value.ToString();
        internal static string SetParameterValue(this double value) => value.ToString();
        internal static string SetParameterValue(this decimal value) => value.ToString();
        internal static string SetParameterValue(this Guid value) => $"'{value.ToString()}'";
        internal static string SetParameterValue(this TimeSpan value) => $"'{value.ToString()}'";
        internal static string SetParameterValue(this DateTime value) => $"'{value.ToString()}'";
        internal static string SetParameterValue(this RowVersion value) => $"{((ulong)value).GetBytes().ToHexString()}";

        internal static string SetParameterValue(this char? value) => value == null ? "null" : $"N'{value}'";
        internal static string SetParameterValue(this bool? value) => value == null ? "null" : value.ToString();
        internal static string SetParameterValue(this byte? value) => value == null ? "null" : value.ToString();
        internal static string SetParameterValue(this short? value) => value == null ? "null" : value.ToString();
        internal static string SetParameterValue(this int? value) => value == null ? "null" : value.ToString();
        internal static string SetParameterValue(this long? value) => value == null ? "null" : value.ToString();
        internal static string SetParameterValue(this float? value) => value == null ? "null" : value.ToString();
        internal static string SetParameterValue(this double? value) => value == null ? "null" : value.ToString();
        internal static string SetParameterValue(this decimal? value) => value == null ? "null" : value.ToString();
        internal static string SetParameterValue(this Guid? value) => value == null ? "null" : $"'{value.ToString()}'";
        internal static string SetParameterValue(this TimeSpan? value) => value == null ? "null" : $"'{value.ToString()}'";
        internal static string SetParameterValue(this DateTime? value) => value == null ? "null" : $"'{value.ToString()}'";
        internal static string SetParameterValue(this RowVersion? value) => value == null ? "null" : $"{((ulong)value).GetBytes().ToHexString()}";








        internal static MethodInfo GetDataReaderGetValueMethod(this Type Type)
        {
            return map_ClrType_DtRdrVal_MthInfo[Type];
        }
        internal static MethodInfo GetDataReaderGetValueMethod(this PropertyInfo Property)
        {
            return Property.PropertyType.GetDataReaderGetValueMethod();
        }

        internal static byte[] GetBytes(this SqlDataReader DataReader, int Index)
        {
            if (DataReader.IsDBNull(Index))
                return null;
            else
            {
                byte[] Buffer = new byte[Convert.ToInt32(DataReader.GetBytes(Index, 0, null, 0, int.MaxValue))];
                DataReader.GetBytes(Index, 0, Buffer, 0, Buffer.Length);
                return Buffer;
            }
        }
        internal static string GetString(this SqlDataReader DataReader, int Index)
        {
            if (DataReader.IsDBNull(Index))
                return null;
            else
                return DataReader.GetString(Index);
        }

        internal static char GetChar(this SqlDataReader DataReader, int Index)
        {
            if (DataReader.IsDBNull(Index))
                throw new EntityDatabaseException($"GetChar Failed, Data Reader Return Null Value At {Index} Index");
            else
                return DataReader.GetString(Index)[0];
        }
        internal static bool GetBool(this SqlDataReader DataReader, int Index)
        {
            if (DataReader.IsDBNull(Index))
                throw new EntityDatabaseException($"GetBool Failed, Data Reader Return Null Value At {Index} Index");
            else
                return DataReader.GetBoolean(Index);
        }
        internal static byte GetByte(this SqlDataReader DataReader, int Index)
        {
            if (DataReader.IsDBNull(Index))
                throw new EntityDatabaseException($"GetByte Failed, Data Reader Return Null Value At {Index} Index");
            else
                return DataReader.GetByte(Index);
        }
        internal static short GetShort(this SqlDataReader DataReader, int Index)
        {
            if (DataReader.IsDBNull(Index))
                throw new EntityDatabaseException($"GetShort Failed, Data Reader Return Null Value At {Index} Index");
            else
                return DataReader.GetInt16(Index);
        }
        internal static int GetInt(this SqlDataReader DataReader, int Index)
        {
            if (DataReader.IsDBNull(Index))
                throw new EntityDatabaseException($"GetInt Failed, Data Reader Return Null Value At {Index} Index");
            else
                return DataReader.GetInt32(Index);
        }
        internal static long GetLong(this SqlDataReader DataReader, int Index)
        {
            if (DataReader.IsDBNull(Index))
                throw new EntityDatabaseException($"GetLong Failed, Data Reader Return Null Value At {Index} Index");
            else
                return DataReader.GetInt64(Index);
        }
        internal static float GetFloat(this SqlDataReader DataReader, int Index)
        {
            if (DataReader.IsDBNull(Index))
                throw new EntityDatabaseException($"GetFloat Failed, Data Reader Return Null Value At {Index} Index");
            else
                return DataReader.GetFloat(Index);
        }
        internal static double GetDouble(this SqlDataReader DataReader, int Index)
        {
            if (DataReader.IsDBNull(Index))
                throw new EntityDatabaseException($"GetDouble Failed, Data Reader Return Null Value At {Index} Index");
            else
                return DataReader.GetDouble(Index);
        }
        internal static decimal GetDecimal(this SqlDataReader DataReader, int Index)
        {
            if (DataReader.IsDBNull(Index))
                throw new EntityDatabaseException($"GetDecimal Failed, Data Reader Return Null Value At {Index} Index");
            else
                return DataReader.GetDecimal(Index);
        }
        internal static Guid GetGuid(this SqlDataReader DataReader, int Index)
        {
            if (DataReader.IsDBNull(Index))
                throw new EntityDatabaseException($"GetGuid Failed, Data Reader Return Null Value At {Index} Index");
            else
                return DataReader.GetGuid(Index);
        }
        internal static TimeSpan GetTimeSpan(this SqlDataReader DataReader, int Index)
        {
            if (DataReader.IsDBNull(Index))
                throw new EntityDatabaseException($"GetTimeSpan Failed, Data Reader Return Null Value At {Index} Index");
            else
                return DataReader.GetTimeSpan(Index);
        }
        internal static DateTime GetDateTime(this SqlDataReader DataReader, int Index)
        {
            if (DataReader.IsDBNull(Index))
                throw new EntityDatabaseException($"GetDateTime Failed, Data Reader Return Null Value At {Index} Index");
            else
                return DataReader.GetDateTime(Index);
        }
        internal static RowVersion GetRowVersion(this SqlDataReader DataReader, int Index)
        {
            if (DataReader.IsDBNull(Index))
                throw new EntityDatabaseException($"GetRowVersion Failed, Data Reader Return Null Value At {Index} Index");
            else
            {
                byte[] Buffer = new byte[8];
                DataReader.GetBytes(Index, 0, Buffer, 0, Buffer.Length);

                return new RowVersion(Buffer.RowVersionToUInt64());
            }
        }


        internal static char? GetNullableChar(this SqlDataReader DataReader, int Index)
        {
            if (DataReader.IsDBNull(Index))
                return null;
            else
                return DataReader.GetString(Index)[0];
        }
        internal static bool? GetNullableBool(this SqlDataReader DataReader, int Index)
        {
            if (DataReader.IsDBNull(Index))
                return null;
            else
                return DataReader.GetBoolean(Index);
        }
        internal static byte? GetNullableByte(this SqlDataReader DataReader, int Index)
        {
            if (DataReader.IsDBNull(Index))
                return null;
            else
                return DataReader.GetByte(Index);
        }
        internal static short? GetNullableShort(this SqlDataReader DataReader, int Index)
        {
            if (DataReader.IsDBNull(Index))
                return null;
            else
                return DataReader.GetInt16(Index);
        }
        internal static int? GetNullableInt(this SqlDataReader DataReader, int Index)
        {
            if (DataReader.IsDBNull(Index))
                return null;
            else
                return DataReader.GetInt32(Index);
        }
        internal static long? GetNullableLong(this SqlDataReader DataReader, int Index)
        {
            if (DataReader.IsDBNull(Index))
                return null;
            else
                return DataReader.GetInt64(Index);
        }
        internal static float? GetNullableFloat(this SqlDataReader DataReader, int Index)
        {
            if (DataReader.IsDBNull(Index))
                return null;
            else
                return DataReader.GetFloat(Index);
        }
        internal static double? GetNullableDouble(this SqlDataReader DataReader, int Index)
        {
            if (DataReader.IsDBNull(Index))
                return null;
            else
                return DataReader.GetDouble(Index);
        }
        internal static decimal? GetNullableDecimal(this SqlDataReader DataReader, int Index)
        {
            if (DataReader.IsDBNull(Index))
                return null;
            else
                return DataReader.GetDecimal(Index);
        }
        internal static Guid? GetNullableGuid(this SqlDataReader DataReader, int Index)
        {
            if (DataReader.IsDBNull(Index))
                return null;
            else
                return DataReader.GetGuid(Index);
        }
        internal static TimeSpan? GetNullableTimeSpan(this SqlDataReader DataReader, int Index)
        {
            if (DataReader.IsDBNull(Index))
                return null;
            else
                return DataReader.GetTimeSpan(Index);
        }
        internal static DateTime? GetNullableDateTime(this SqlDataReader DataReader, int Index)
        {
            if (DataReader.IsDBNull(Index))
                return null;
            else
                return DataReader.GetDateTime(Index);
        }
        internal static RowVersion? GetNullableRowVersion(this SqlDataReader DataReader, int Index)
        {
            if (DataReader.IsDBNull(Index))
                return null;
            else
            {
                byte[] Buffer = new byte[8];
                DataReader.GetBytes(Index, 0, Buffer, 0, Buffer.Length);

                return new RowVersion(Buffer.RowVersionToUInt64());
            }
        }








        internal static MethodInfo GetExecuteScalarGetValueMethod(this Type Type)
        {
            return map_ClrType_ExeSclrVal_MthInfo[Type];
        }

        internal static byte[] GetBytes(object Value)
        {
            if (Value == DBNull.Value)
                return null;
            else
                return (byte[])Value;
        }
        internal static string GetString(object Value)
        {
            if (Value == DBNull.Value)
                return null;
            else
                return (string)Value;
        }

        internal static char GetChar(object Value)
        {
            if (Value == DBNull.Value)
                throw new EntityDatabaseException($"GetChar Failed, Execute Scalar Return Null Value");
            else
                return ((string)Value)[0];
        }
        internal static bool GetBool(object Value)
        {
            if (Value == DBNull.Value)
                throw new EntityDatabaseException($"GetBool Failed, Execute Scalar Return Null Value");
            else
                return (bool)Value;
        }
        internal static byte GetByte(object Value)
        {
            if (Value == DBNull.Value)
                throw new EntityDatabaseException($"GetByte Failed, Execute Scalar Return Null Value");
            else
                return (byte)Value;
        }
        internal static short GetShort(object Value)
        {
            if (Value == DBNull.Value)
                throw new EntityDatabaseException($"GetShort Failed, Execute Scalar Return Null Value");
            else
                return (short)Value;
        }
        internal static int GetInt(object Value)
        {
            if (Value == DBNull.Value)
                throw new EntityDatabaseException($"GetInt Failed, Execute Scalar Return Null Value");
            else
                return (int)Value;
        }
        internal static long GetLong(object Value)
        {
            if (Value == DBNull.Value)
                throw new EntityDatabaseException($"GetLong Failed, Execute Scalar Return Null Value");
            else
                return (long)Value;
        }
        internal static float GetFloat(object Value)
        {
            if (Value == DBNull.Value)
                throw new EntityDatabaseException($"GetFloat Failed, Execute Scalar Return Null Value");
            else
                return (float)Value;
        }
        internal static double GetDouble(object Value)
        {
            if (Value == DBNull.Value)
                throw new EntityDatabaseException($"GetDouble Failed, Execute Scalar Return Null Value");
            else
                return (double)Value;
        }
        internal static decimal GetDecimal(object Value)
        {
            if (Value == DBNull.Value)
                throw new EntityDatabaseException($"GetDecimal Failed, Execute Scalar Return Null Value");
            else
                return (decimal)Value;
        }
        internal static Guid GetGuid(object Value)
        {
            if (Value == DBNull.Value)
                throw new EntityDatabaseException($"GetGuid Failed, Execute Scalar Return Null Value");
            else
                return (Guid)Value;
        }
        internal static TimeSpan GetTimeSpan(object Value)
        {
            if (Value == DBNull.Value)
                throw new EntityDatabaseException($"GetTimeSpan Failed, Execute Scalar Return Null Value");
            else
                return (TimeSpan)Value;
        }
        internal static DateTime GetDateTime(object Value)
        {
            if (Value == DBNull.Value)
                throw new EntityDatabaseException($"GetDateTime Failed, Execute Scalar Return Null Value");
            else
                return (DateTime)Value;
        }
        internal static RowVersion GetRowVersion(object Value)
        {
            if (Value == DBNull.Value)
                throw new EntityDatabaseException($"GetRowVersion Failed, Execute Scalar Return Null Value");
            else
            {
                byte[] Buffer = (byte[])Value;

                return new RowVersion(Buffer.RowVersionToUInt64());
            }
        }


        internal static char? GetNullableChar(object Value)
        {
            if (Value == DBNull.Value)
                return null;
            else
                return ((string)Value)[0];
        }
        internal static bool? GetNullableBool(object Value)
        {
            if (Value == DBNull.Value)
                return null;
            else
                return (bool)Value;
        }
        internal static byte? GetNullableByte(object Value)
        {
            if (Value == DBNull.Value)
                return null;
            else
                return (byte)Value;
        }
        internal static short? GetNullableShort(object Value)
        {
            if (Value == DBNull.Value)
                return null;
            else
                return (short)Value;
        }
        internal static int? GetNullableInt(object Value)
        {
            if (Value == DBNull.Value)
                return null;
            else
                return (int)Value;
        }
        internal static long? GetNullableLong(object Value)
        {
            if (Value == DBNull.Value)
                return null;
            else
                return (long)Value;
        }
        internal static float? GetNullableFloat(object Value)
        {
            if (Value == DBNull.Value)
                return null;
            else
                return (float)Value;
        }
        internal static double? GetNullableDouble(object Value)
        {
            if (Value == DBNull.Value)
                return null;
            else
                return (double)Value;
        }
        internal static decimal? GetNullableDecimal(object Value)
        {
            if (Value == DBNull.Value)
                return null;
            else
                return (decimal)Value;
        }
        internal static Guid? GetNullableGuid(object Value)
        {
            if (Value == DBNull.Value)
                return null;
            else
                return (Guid)Value;
        }
        internal static TimeSpan? GetNullableTimeSpan(object Value)
        {
            if (Value == DBNull.Value)
                return null;
            else
                return (TimeSpan)Value;
        }
        internal static DateTime? GetNullableDateTime(object Value)
        {
            if (Value == DBNull.Value)
                return null;
            else
                return (DateTime)Value;
        }
        internal static RowVersion? GetNullableRowVersion(object Value)
        {
            if (Value == DBNull.Value)
                return null;
            else
            {
                byte[] Buffer = (byte[])Value;

                return new RowVersion(Buffer.RowVersionToUInt64());
            }
        }
    }
}
