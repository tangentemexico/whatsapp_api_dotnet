using Mgk.Commonsx;
//using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using Npgsql;
using System.Threading;
using System.Net.Mail;
using MySqlConnector;
using Microsoft.Data.SqlClient;

namespace Mgk.DataBasex
{
    public class MgkDataBase
    {
        public enum DataBaseEngineEnum { No, SqlClient, MySqlClient, Npgsql };
        public DataBaseEngineEnum DataBaseEngine { get; set; }
        public string ConnectionString { get; set; }
		public string PrefixConnectionString { get; set; }		
        public MgkMessages Messages { get; set; }
        private SqlConnection ConnectionSql;
        private MySqlConnection ConnectionMySql;
        private NpgsqlConnection ConnectionPostgreSql;
        public int CommandTimeout = -1;

        string _procedureName = "";
        List<DbParameter> _parameters = null;
        CommandType _commandType;

        public string GetProcedureName()
        {
            return this._procedureName;
        }

        public List<DbParameter> GetParameters()
        {
            return this._parameters;
        }

        public CommandType GetCommandType()
        {
            return this._commandType;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ConnectionStr"></param>
        /// <param name="DbEngine"></param>
        /// <param name="PrefixConnectionStr">Prefijo para buscar cadena de conexion</param>
        public MgkDataBase(String ConnectionStr=null, DataBaseEngineEnum DbEngine= DataBaseEngineEnum.No,String PrefixConnectionStr=null)
        {
            //DataBaseEngine = DataBaseEngineEnum.PostgresSqlClient;
            //DataBaseEngine = DataBaseEngineEnum.MySqlClient;

			PrefixConnectionString = PrefixConnectionStr;
            DataBaseEngine = DbEngine;

            ConnectionString = ConnectionStr;
            Messages = new MgkMessages();

            if (ConnectionString==null)
                ReadStrConnection();
        }

        /// Identificar motor de base de datos
        private void _identifyDataBaseEngine(string ProviderName)
        {
            Messages.Clear();
            if (DataBaseEngine != DataBaseEngineEnum.No)
                return;
            try
            {
                string[] partes = ProviderName.Split('.');
                if (ProviderName != null && ProviderName != "")
                    DataBaseEngine = (DataBaseEngineEnum)Enum.Parse(typeof(DataBaseEngineEnum), partes[partes.Length - 1]);
            }
            catch (Exception ex)
            {
                Messages.Add(new MgkMessage
                {
                    Number = MgkResponseCode.DB_EXCEPTION_UNKNOW,
                    Code = "EX-IDE",
                    Message = "Error en operaciones de base de datos [IDE]",
                    Messagex = "Error al identificar motor de base de datos",
                    Source = this.ToString(),
                    Message2 = "_identifyDataBaseEngine",
                    Exception = ex.ToString()
                });
                MailSupport(Messages.GetLastMessage());
                MgkLog.Error(Messages.GetLastMessage());
                throw;
            }
        }

        /*public void _iniciar_conexion()
		{
			if (DataBaseEngine == DataBaseEngineEnum.MySQL)
				operadorBaseDatos = new ConexionMySQL();
			if (DataBaseEngine == DataBaseEngineEnum.Mssql)
				operadorBaseDatos = new ConexionMssql();
		}*/
        public void _ReadStrConnection() {
            this.DataBaseEngine= DataBaseEngineEnum.No;
            this.ConnectionString = null;
            this.ReadStrConnection();
        }
        private void ReadStrConnection()
        {
            Messages.Clear();
            try
            {
                if (ConnectionString == null)
                {
                    string providerName = "";
                    MgkFunctions.ReadWorkEnvironment();
                    string env = MgkFunctions.WORK_ENVIRONMENT;
                    if (string.IsNullOrWhiteSpace(env) == false)
                        env += ".";

                    string keyConStr = "connectionString";

                    string ConnStr = MgkFunctions.AppSettings(keyConStr, "", true);
                    Dictionary<string, string> keyValuePairs = new Dictionary<string, string>();
                    keyValuePairs = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(ConnStr);
                    if (keyValuePairs != null)
                    {
                        if (keyValuePairs.ContainsKey("connectionString"))
                            ConnectionString = keyValuePairs["connectionString"];
                        if (keyValuePairs.ContainsKey("providerName"))
                            providerName = keyValuePairs["providerName"];
                    }

                    _identifyDataBaseEngine(providerName);


                    //String csencode = MgkFunctions.AppSettings("Mgk.csencode");
                    //if (MgkFunctions.StrToBoolean(csencode,true) && MgkFunctions.AppSettings("Mgk.Seed") != null)
                    //    ConnectionString = Mgk.Secrets.MgkSecret1._decode64(ConnectionString, Mgk.Secrets.MgkSecret1.MD5(ConnectionString, 2));
                }
            }
            catch (Exception e)
            {
                Messages.Add(new MgkMessage
                {
                    Code = "DB_ERROR_LEER_CONEXION",
                    Number = MgkResponseCode.DB_ERROR_CONNECTION_READ,
                    Message = "Error al obtener cadena de conexión con base de datos",
                    Messagex = "Error en lectura de ConnectionString",
                    Source = this.ToString(),
                    Message2 = "ReadStrConnection",
                    Exception = e.ToString()
                });
                MailSupport(Messages.GetLastMessage());
                MgkLog.Error(Messages.GetLastMessage());
                //throw;
            }
        }

        /// <summary>
        /// Crea una conexion y devuelve una conexion MySQL
        /// </summary>
        /// <returns></returns>
        public MySqlConnection GetConnectionMySql()
        {
            if (ConnectionMySql == null)
                ConnectionMySql = new MySqlConnection(this.ConnectionString);

            try
            {
                if (ConnectionMySql.State != ConnectionState.Open)
                    ConnectionMySql.Open();
            }
            catch(Exception ex1)
            {
                try
                {
                    ConnectionMySql = null;
                    ConnectionMySql = new MySqlConnection(this.ConnectionString);
                    ConnectionMySql.Open();
                }
                catch(Exception ex2)
                {

                }

                //ConnectionMySql = new MySqlConnection(this.ConnectionString);
            }

            return ConnectionMySql;
        }

        /// <summary>
        /// Crea una conexion y devuelve una conexion MSSQL
        /// </summary>
        /// <returns></returns>
        public SqlConnection GetConnectionMSSQL()
        {
            if (ConnectionSql == null)
                ConnectionSql = new SqlConnection(this.ConnectionString);
            if (ConnectionSql.State != ConnectionState.Open)
            {
                if (ConnectionSql.ConnectionString == null)
                    ConnectionSql.ConnectionString = this.ConnectionString;
                else if (ConnectionSql.ConnectionString == "")
                    ConnectionSql.ConnectionString = this.ConnectionString;
                ConnectionSql.Open();
            }

            return ConnectionSql;
        }

        /// <summary>
        /// Crea una conexion y devuelve una conexion MySQL
        /// </summary>
        /// <returns></returns>
        public NpgsqlConnection GetConnectionPostgreSql()
        {
            if (ConnectionPostgreSql == null)
                ConnectionPostgreSql = new NpgsqlConnection(this.ConnectionString);
            if (ConnectionPostgreSql.State != ConnectionState.Open)
            {
                ConnectionPostgreSql = new NpgsqlConnection(this.ConnectionString);
                ConnectionPostgreSql.Open();
            }
            return ConnectionPostgreSql;
        }

        /// <summary>
        /// Cerrar conexion actual
        /// </summary>
        public void ConnectionClose()
        {
            try
            {
                if (DataBaseEngine == DataBaseEngineEnum.MySqlClient)
                    ConnectionMySql.Close();
                else if (DataBaseEngine == DataBaseEngineEnum.SqlClient)
                    ConnectionSql.Close();
                else if (DataBaseEngine == DataBaseEngineEnum.Npgsql)
                    ConnectionPostgreSql.Close();
            }
            catch (Exception )
            {

            }
        }

        public DbConnection GetConnection()
        {
            try
            {
                if (DataBaseEngine == DataBaseEngineEnum.MySqlClient)
                    return GetConnectionMySql();
                else if (DataBaseEngine == DataBaseEngineEnum.SqlClient)
                    return GetConnectionMSSQL();
                else if (DataBaseEngine == DataBaseEngineEnum.Npgsql)
                    return GetConnectionPostgreSql();

                else
                {
                    if (this.ConnectionString == null)
                    {
                        Messages.Add(new MgkMessage
                        {
                            Code = "ERR-GC1",
                            Number = MgkResponseCode.DB_ERROR_CONNECTION_CONNECT,
                            Message = "Error en operaciones con base de datos [GC]",
                            Messagex = "No se ha identificado ConnectionString"
                        });
                        MgkLog.Error(Messages.GetLastMessage());
                        return null;
                    }
                    Messages.Add(new MgkMessage
                    {
                        Code = "ERR-GC2",
                        Number = MgkResponseCode.DB_ERROR_CONNECTION_CONNECT,
                        Message = "Error en operaciones con base de datos [GC]",
                        Messagex = "Error al intentar conexion con base de datos. GetConnection"
                    });
                    MgkLog.Error(Messages.GetLastMessage());
                }
            }
            catch (Exception ex)
            {
                Messages.Add(new MgkMessage
                {
                    Exception = ex.ToString(),
                    Code = "EX-GC3",
                    Number = MgkResponseCode.DB_ERROR_CONNECTION_CONNECT,
                    Message = "Error en operaciones con base de datos [GC]",
                    Messagex = "Error al intentar conexion con base de datos. GetConnection",
                    Message2 = this.ConnectionString
                });
                MailSupport(Messages.GetLastMessage());
                MgkLog.Error(Messages.GetLastMessage());
            }
            return null;
        }

        public DbCommand GetCommand(DbConnection Connection, string commandText, CommandType commandType = CommandType.Text)
        {
            Messages.Clear();
            try
            {
                if (DataBaseEngine == DataBaseEngineEnum.MySqlClient)
                    return MgkMysqlConnect.GetCommand(Connection, commandText, commandType);
                if (DataBaseEngine == DataBaseEngineEnum.SqlClient)
                    return MgkMssqlConnect.GetCommand(Connection, commandText, commandType);
                if (DataBaseEngine == DataBaseEngineEnum.Npgsql)
                    return MgkPostgreSqlConnect.GetCommand(Connection, commandText, commandType);
            }
            catch (Exception ex)
            {
                Messages.Add(new MgkMessage
                {
                    Code = "EX-DBC",
                    Message = "Error con operaciones de base de datos [DBC]",
                    Messagex = "Error al crear DbCommand",
                    Number = MgkResponseCode.DB_EXCEPTION_UNKNOW,
                    Exception = ex.ToString(),
                    Source = this.ToString() + ".GetCommand",
                    OData = new
                    {
                        commandText = commandText,
                        commandType = commandType
                    }
                });
                MailSupport(Messages.GetLastMessage());
                MgkLog.Error(Messages.GetLastMessage());
                throw;
            }
            return null;
        }

        /// <summary>
        /// SqlParameter|MySqlParameter
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public object GetParameter(string parameter, object value)
        {
            if (DataBaseEngine == DataBaseEngineEnum.MySqlClient)
                return MgkMysqlConnect.GetParameter(parameter, value);
            if (DataBaseEngine == DataBaseEngineEnum.SqlClient)
                return MgkMssqlConnect.GetParameter(parameter, value);
            if (DataBaseEngine == DataBaseEngineEnum.Npgsql)
                return MgkPostgreSqlConnect.GetParameter(parameter, value);
            return null;
        }

        /// <summary>
        /// SqlParameter|MySqlParameter
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="type"></param>
        /// <param name="value"></param>
        /// <param name="parameterDirection"></param>
        /// <returns></returns>
        public object GetParameterOut(string parameter, SqlDbType type, object value = null, ParameterDirection parameterDirection = ParameterDirection.InputOutput)
        {
            if (DataBaseEngine == DataBaseEngineEnum.MySqlClient)
                return MgkMysqlConnect.GetParameterOut(parameter, type, value, parameterDirection);
            if (DataBaseEngine == DataBaseEngineEnum.SqlClient)
                return MgkMssqlConnect.GetParameterOut(parameter, type, value, parameterDirection);
            return null;
        }

        public int ExecuteNonQuery(string procedureName, List<DbParameter> parameters, CommandType commandType = CommandType.StoredProcedure)
        {
            this._procedureName = procedureName;
            this._parameters = parameters;
            this._commandType = commandType;

            /*if (operadorBaseDatos == null)
				return -1;*/
            int returnValue = -1;
            try
            {
                MgkLog.Sql("ExecuteNonQuery", procedureName + ";" + ParametersToString(parameters));

                using (DbConnection Connection = GetConnection())
                {
                    DbCommand cmd = this.GetCommand(Connection, procedureName, commandType);

                    if (parameters != null && parameters.Count > 0)
                    {
                        cmd.Parameters.AddRange(parameters.ToArray());
                    }
                    returnValue = cmd.ExecuteNonQuery();
                    Messages.Add(new MgkMessage
                    {
                        Message = string.Format("{0} registro(s) modificado(s)", returnValue),
                        Type = (returnValue > 0) ? MgkMessage.TYPE_SUCCESS : MgkMessage.TYPE_DANGER,
                        Number = returnValue
                    });
                }
            }
            catch (Exception ex)
            {
                Messages.Add(new MgkMessage
                {
                    Code = "EX-ENQ",
                    Number = MgkResponseCode.DB_EXCEPTION_ExecuteNonQuery,
                    Message = "Error en operaciones de base de datos [ENQ]",
                    Messagex = "Error al intentar ExecuteNonQuery",
                    Exception = ex.ToString(),
                    Source = this.ToString() + ".ExecuteNonQuery",
                    OData = new
                    {
                        procedureName = procedureName,
                        //parameters = parameters,
                        parameters = ParametersToString(parameters),
                        commandType = commandType
                    }
                });
                MailSupport(Messages.GetLastMessage());
                MgkLog.Error(Messages.GetLastMessage());
                throw;
            }
            return returnValue;
        }

        public int ExecuteScalarInt(string procedureName, List<DbParameter> parameters, CommandType commandType = CommandType.StoredProcedure)
        {
            this._procedureName = procedureName;
            this._parameters = parameters;
            this._commandType = commandType;

            int returnValue = -1;
            try
            {
                MgkLog.Sql("ExecuteScalarInt", procedureName + ";" + ParametersToString(parameters));

                using (DbConnection Connection = this.GetConnection())
                {
                    if (MgkStaticMessage.Message.Number < 0)
                        return -1;
                    DbCommand cmd = this.GetCommand(Connection, procedureName, commandType);

                    //foreach(DbParameter par in parameters)
                    //{
                    //    cmd.Parameters.Add(par);
                    //}

                    if (parameters != null && parameters.Count > 0)
                    {
                        cmd.Parameters.AddRange(parameters.ToArray());
                    }

                    var lastId = cmd.ExecuteScalar();
                    returnValue = Convert.ToInt32(lastId);
                    Messages.Add(new MgkMessage
                    {
                        Message = string.Format("Resultado de base de datos: {0}", returnValue),
                        Type = returnValue > 0 ? MgkMessage.TYPE_SUCCESS : MgkMessage.TYPE_WARNING,
                        Number = returnValue
                    });
                }
            }
            catch (Exception ex)
            {
                Messages.Add(new MgkMessage
                {
                    Code = "EX-ESI",
                    Number = MgkResponseCode.DB_EXCEPTION_UNKNOW,
                    //Message = "Error con operaciones de base de datos [ESI]",
                    Message = "Error DB."+ex.Message,
                    Messagex = "Error ExecuteScalarInt",
                    Exception = ex.ToString(),
                    Source = this.ToString() + ".ExecuteScalarInt",
                    OData = new
                    {
                        procedureName = procedureName,
                        //parameters = parameters,
                        parameters = ParametersToString(parameters),
                        commandType = commandType
                    }
                });
                MailSupport(Messages.GetLastMessage());
                MgkLog.Error(Messages.GetLastMessage());
                throw ex;
            }
            return returnValue;
        }

        public object ExecuteScalar(string procedureName, List<DbParameter> parameters, CommandType commandType = CommandType.StoredProcedure)
        {
            this._procedureName = procedureName;
            this._parameters = parameters;
            this._commandType = commandType;

            object returnValue = null;
            try
            {
                MgkLog.Sql("ExecuteScalarInt", procedureName + ";" + ParametersToString(parameters));

                using (DbConnection Connection = GetConnection())
                {
                    DbCommand cmd = this.GetCommand(Connection, procedureName, commandType);

                    if (parameters != null && parameters.Count > 0)
                    {
                        cmd.Parameters.AddRange(parameters.ToArray());
                    }

                    returnValue = cmd.ExecuteScalar();
                    Messages.Add(new MgkMessage
                    {
                        Message = string.Format("{0} registro(s) modificado(s)", returnValue),
                        Type = MgkMessage.TYPE_INFO,
                        Number = 0
                    });
                }
            }
            catch (Exception ex)
            {
                Messages.Add(new MgkMessage
                {
                    Code = "EX-ES",
                    Number = MgkResponseCode.DB_EXCEPTION_ExecuteScalar,
                    Message = "Error con operaciones de base de datos [ES]",
                    Messagex = "Error con ExecuteScalar",
                    Exception = ex.ToString(),
                    Source = this.ToString() + ".ExecuteScalar",
                    OData = new
                    {
                        procedureName = procedureName,
                        //parameters = parameters,
                        parameters = ParametersToString(parameters),
                        commandType = commandType
                    }
                });
                MailSupport(Messages.GetLastMessage());
                MgkLog.Error(Messages.GetLastMessage());
                throw;
            }
            return returnValue;
        }

        public object ExecuteScalar(string procedureName, List<DbParameter> parameters)
        {
            this._procedureName = procedureName;
            this._parameters = parameters;
            this._commandType = CommandType.StoredProcedure;

            object returnValue = null;
            try
            {
                MgkLog.Sql("ExecuteScalarInt", procedureName + ";" + ParametersToString(parameters));

                using (DbConnection Connection = GetConnection())
                {
                    DbCommand cmd = this.GetCommand(Connection, procedureName, CommandType.StoredProcedure);

                    if (parameters != null && parameters.Count > 0)
                    {
                        cmd.Parameters.AddRange(parameters.ToArray());
                    }
                    returnValue = cmd.ExecuteScalar();
                }
            }
            catch (Exception ex)
            {
                Messages.Add(new MgkMessage
                {
                    Code = "EX-ES2",
                    Number = MgkResponseCode.DB_EXCEPTION_ExecuteScalar,
                    Message = "Error con operaciones de base de datos [ES2]",
                    Messagex = "Error Base de datos ExecuteScalar. Funcion 2",
                    Exception = ex.ToString(),
                    Source = this.ToString() + ".ExecuteScalar",
                    OData = new
                    {
                        procedureName = procedureName,
                        //parameters = parameters
                        parameters = ParametersToString(parameters),
                    }
                });
                MailSupport(Messages.GetLastMessage());
                MgkLog.Error(Messages.GetLastMessage());
                throw;
            }
            return returnValue;
        }

        public DbDataReader GetDataReader(string procedureName, List<DbParameter> parameters, CommandType commandType = CommandType.StoredProcedure)
        {
            this._procedureName = procedureName;
            this._parameters = parameters;
            this._commandType = commandType;

            DbDataReader ds;
            try
            {
                
                MgkLog.Sql("GetDataReader", procedureName+";"+ ParametersToString(parameters));

                DbConnection Connection = GetConnection();
                if (MgkStaticMessage.Message.Number < 0)
                    return null;
                if (Connection == null)
                    return null;
                {
                    DbCommand cmd = this.GetCommand(Connection, procedureName, commandType);
					if (this.CommandTimeout>=0)
                        cmd.CommandTimeout = this.CommandTimeout;					
                    if (parameters != null && parameters.Count > 0)
                    {
                        if (cmd == null)
                        {
                            MgkLog.Error(Messages.GetLastMessage());
                        }

                        if (cmd.Parameters == null)
                        {
                            MgkLog.Error(Messages.GetLastMessage());
                        }
                        int tam = parameters.ToArray().Length;
                        Array myArr = parameters.ToArray();
                        String par = myArr.GetValue(0).ToString();

                        cmd.Parameters.AddRange(parameters.ToArray());
                    }
                    ds = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                }
            }
            catch (Exception ex)
            {
                Messages.Add(new MgkMessage
                {
                    Code = "EX-GR",
                    Number = MgkResponseCode.DB_EXCEPTION_ExecuteReader,
                    Message = "Error con operaciones de base de datos [GR]",
                    Messagex = "Error ExecuteReader",
                    Exception = ex.ToString(),
                    Source = this.ToString() + ".GetDataReader",
                    OData = new
                    {
                        procedureName = procedureName,
                        //parameters = parameters,
                        parameters = ParametersToString(parameters),
                        commandType = commandType
                    }
                });
                MailSupport(Messages.GetLastMessage());
                MgkLog.Error(Messages.GetLastMessage());
                throw;
            }
            return ds;
        }

        public String ExecuteProcedure(string procedureName, List<DbParameter> parameters, CommandType commandType = CommandType.StoredProcedure)
        {
            this._procedureName = procedureName;
            this._parameters = parameters;
            this._commandType = commandType;

            String sRetVal;
            try
            {
                MgkLog.Sql("ExecuteProcedure", procedureName + ";" + ParametersToString(parameters));

                DbConnection connection = GetConnection();
                if (MgkStaticMessage.Message.Number < 0)
                    return null;
                if (connection == null)
                    return null;
                {
                    DbCommand cmd = this.GetCommand(connection, procedureName, commandType);
                    if (parameters != null && parameters.Count > 0)
                    {
                        if (cmd == null)
                        {
                            MgkLog.Error(Messages.GetLastMessage());
                        }

                        if (cmd.Parameters == null)
                        {
                            MgkLog.Error(Messages.GetLastMessage());
                        }
                        int tam = parameters.ToArray().Length;
                        Array myArr = parameters.ToArray();
                        String par = myArr.GetValue(0).ToString();

                        cmd.Parameters.AddRange(parameters.ToArray());
                    }
                    // ds = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                    cmd.Prepare();
                    if (CommandTimeout>=0)
                        cmd.CommandTimeout = CommandTimeout;
                    Object result = cmd.ExecuteScalar();
                    // Object result = cmd.ExecuteNonQuery();
                    sRetVal = (String)result;
                }
            }
            catch (Exception ex)
            {
                Messages.Add(new MgkMessage
                {
                    Code = "EX-GR",
                    Number = MgkResponseCode.DB_EXCEPTION_ExecuteReader,
                    Message = "Error con operaciones de base de datos [GR]",
                    Messagex = "Error ExecuteReader",
                    Exception = ex.ToString(),
                    Source = this.ToString() + ".GetDataReader",
                    OData = new
                    {
                        procedureName = procedureName,
                        //parameters = parameters,
                        parameters = ParametersToString(parameters),
                        commandType = commandType
                    }
                });
                MailSupport(Messages.GetLastMessage());
                MgkLog.Error(Messages.GetLastMessage());
                throw;
            }
            return sRetVal;
        }

        /// <summary>
        /// Query para obtener el ultimo registro insertado cuando es auto incrementable
        /// </summary>
        /// <param name="Table_name"></param>
        /// <param name="Fiel_name_key"></param>
        /// <returns></returns>
        public String LastIdQuery(String Table_name = "", String Fiel_name_key = "")
        {
            if (this.DataBaseEngine == DataBaseEngineEnum.MySqlClient)
                return "SELECT LAST_INSERT_ID();";
            if (this.DataBaseEngine == DataBaseEngineEnum.SqlClient)
                return "SELECT Scope_Identity();";
            if (this.DataBaseEngine == DataBaseEngineEnum.Npgsql)
                return "SELECT currval(pg_get_serial_sequence('" + Table_name.ToLower() + "','" + Fiel_name_key.ToLower() + "'));";
            return "";
        }

        public void MailSupport(String Subject, MgkMessage Message, String Smtp_mail_to = "") {
            if (Subject == "" || Subject == null)
                Subject = "Error con base de datos";
            MailSupport(Subject, Message.Message + "<br>\n" + Message.Exception, Smtp_mail_to);
        }

        public void MailSupport(MgkMessage Message, String Smtp_mail_to = "")
        {
            MailSupport(Message.Message, Message._MessageFull + "<br>\n" + Message.Exception, Smtp_mail_to);
        }

        public void MailSupport(String Subject, String Message, String Smtp_mail_to = "")
        {
            Thread thread = new Thread(() => {

                String Smtp_host = MgkFunctions.AppSettings("MAIL_HOSTNAME_SMTP", "");
                int Smtp_port = 0;
                String Smtp_ssl = "";
                String Smtp_username = "";
                String Smtp_password = "";
                String Smtp_mail_bcc = "";
                String Smtp_from_name = "";

                try
                {
                    if (Smtp_host != "")
                    {
                        Smtp_port = MgkFunctions.StrToInt(MgkFunctions.AppSettings("MAIL_PORT_SMTP", "0"));
                        Smtp_mail_to = MgkFunctions.AppSettings("MAIL_TO", "");
                        Smtp_mail_bcc = MgkFunctions.AppSettings("MAIL_BCC", "");
                        Smtp_ssl = MgkFunctions.AppSettings("MAIL_USESSL_SEND", "0");
                        Smtp_username = MgkFunctions.AppSettings("MAIL_USERNAME_SMTP", "");
                        Smtp_password = MgkFunctions.AppSettings("MAIL_PASSWORD_SMTP", "");
                        Smtp_from_name = MgkFunctions.AppSettings("MAIL_FROM_NAME", "Soporte DataBase");
                        

                        MailAddress mailFrom = new MailAddress(Smtp_username, Smtp_from_name);
                        MailAddress mailTo = new MailAddress(Smtp_mail_to, Smtp_mail_to);

                        MailMessage mail = new MailMessage(mailFrom, mailTo);
                        mail.IsBodyHtml = true;
                        mail.Subject = Subject;
                        mail.Body = Message;

                        if ((Smtp_mail_bcc ?? "") != "")
                        {
                            string[] mailBccArr = Smtp_mail_bcc.Split(',');
                            foreach (string direccion in mailBccArr)
                                mail.Bcc.Add(direccion);
                        }


                        SmtpClient client = new SmtpClient(Smtp_host);
                        if (Smtp_port != 0)
                            client.Port = Smtp_port;

                        if ((Smtp_username ?? "") != "")
                        {
                            client.Credentials = new System.Net.NetworkCredential(Smtp_username, Smtp_password);
                            client.EnableSsl = MgkFunctions.StrToBoolean(Smtp_ssl);
                        }

                        client.Send(mail);
                        mail.Dispose();
                        client.Dispose();
                    }
                }
                catch (Exception ee)
                {
                    MgkLog.Error(new MgkMessage
                    {
                        Exception = ee.ToString(),
                        Number = -100031,
                        Messagex = String.Format("host={0},port={1},to={2},ssl={3},user={4},pwd={5}", Smtp_host, Smtp_port, Smtp_mail_to, Smtp_ssl, Smtp_username, Smtp_password),
                        Message = "Error al enviar correo por error en base de datos",
                    });
                }
            });
            thread.Start();
        }

        public String ParametersToString(List<DbParameter> parameters)
        {
            if (parameters != null && parameters.Count > 0)
            {
                string[] str = new string[parameters.Count];
                for (int i = 0; i < parameters.Count; i++)
                {
                    str[i] = parameters[i].ParameterName + "=" + parameters[i].Value.ToString();
                }
                return Newtonsoft.Json.JsonConvert.SerializeObject(str);
            }
            else
                return "";
        }

    }
}