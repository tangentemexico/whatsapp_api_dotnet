using Mgk.Commonsx;
//using MySql.Data.MySqlClient;
using System;
using System.Configuration;
using System.Data;
using System.Data.Common;
using Npgsql;

namespace Mgk.DataBasex
{
    public class MgkPostgreSqlConnect
    {
        public string StringConnection { get; set; } = null;
        public static MgkMessages Messages { get; set; } = new MgkMessages();

        public MgkPostgreSqlConnect(String StringConnection = "")
        {
            if (StringConnection == "")
                ReadStrConnection();
            else
                this.StringConnection = StringConnection;
        }

        private void ReadStrConnection()
        {
            try
            {
                if (StringConnection == null)
                {
                    MgkFunctions.ReadWorkEnvironment();
                    StringConnection = MgkFunctions.AppSettings("connectionString", "", true);
                }
            }
            catch (Exception e)
            {
                Messages.Add(new MgkMessage
                {
                    Code = "DB_ERROR_LEER_CONEXION",
                    Number = MgkResponseCode.DB_ERROR_CONNECTION_READ,
                    Message = "Error al obtener cadena de conexión con base de datos",
                    Source = this.ToString() + ".readStrConnection",
                });
                Console.WriteLine(e.ToString());
            }
        }

        /// <summary>
        /// Crea una conexion y devuelve una conexion MySQL
        /// </summary>
        /// <returns></returns>
        public NpgsqlConnection GetConnection()
        {
            NpgsqlConnection connection = new NpgsqlConnection(this.StringConnection);
            if (connection.State != ConnectionState.Open)
                connection.Open();
            return connection;
        }


        public static DbCommand GetCommand(DbConnection connection, string commandText, CommandType commandType)
        {
            NpgsqlCommand command = new NpgsqlCommand(commandText, connection as NpgsqlConnection);
            command.CommandType = commandType;
            return command;
        }

        public static NpgsqlParameter GetParameter(string parameter, object value)
        {
            NpgsqlParameter parameterObject = new NpgsqlParameter(parameter, value != null ? value : DBNull.Value);
            parameterObject.Direction = ParameterDirection.Input;
            return parameterObject;
        }

        public static NpgsqlParameter GetParameterOut(string parameter, SqlDbType type, object value = null, ParameterDirection parameterDirection = ParameterDirection.InputOutput)
        {
            try
            {
                NpgsqlParameter parameterObject = new NpgsqlParameter(parameter, type); ;

                if (type == SqlDbType.NVarChar || type == SqlDbType.VarChar || type == SqlDbType.NText || type == SqlDbType.Text)
                {
                    parameterObject.Size = -1;
                }

                parameterObject.Direction = parameterDirection;

                if (value != null)
                {
                    parameterObject.Value = value;
                }
                else
                {
                    parameterObject.Value = DBNull.Value;
                }

                return parameterObject;
            }
            catch (Exception ex)
            {
                Messages.AddException(ex, "ConexionPostgreSql", "GetParameterOut");
            }
            return null;
        }

    }
}