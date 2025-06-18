using Mgk.Commonsx;
//using MySql.Data.MySqlClient;
using MySqlConnector;
using System;
using System.Configuration;
using System.Data;
using System.Data.Common;

namespace Mgk.DataBasex
{
    public class MgkMysqlConnect
    {
        public string StringConnection { get; set; } = null;
        public static MgkMessages Messages { get; set; } = new MgkMessages();

        public MgkMysqlConnect(String stringConnection = "")
        {
            if (stringConnection == "")
                ReadStrConnection();
            else
                this.StringConnection = stringConnection;
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
        public MySqlConnection GetConnection()
        {
            MySqlConnection connection = new MySqlConnection(this.StringConnection);
            if (connection.State != ConnectionState.Open)
                connection.Open();
            return connection;
        }


        public static DbCommand GetCommand(DbConnection connection, string commandText, CommandType commandType)
        {
            MySqlCommand command = new MySqlCommand(commandText, connection as MySqlConnection);
            command.CommandType = commandType;
            return command;
        }

        public static MySqlParameter GetParameter(string parameter, object value)
        {
            MySqlParameter parameterObject = new MySqlParameter(parameter, value != null ? value : DBNull.Value);
            parameterObject.Direction = ParameterDirection.Input;
            return parameterObject;
        }

        public static MySqlParameter GetParameterOut(string parameter, SqlDbType type, object value = null, ParameterDirection parameterDirection = ParameterDirection.InputOutput)
        {
            try
            {
                MySqlParameter parameterObject = new MySqlParameter(parameter, type); ;

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
                Messages.AddException(ex, "ConexionMySQL", "GetParameterOut");
            }
            return null;
        }

    }
}