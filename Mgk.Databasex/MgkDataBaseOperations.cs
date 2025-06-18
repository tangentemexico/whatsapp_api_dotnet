using Mgk.Commonsx;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;

/// <summary>
/// agustinistmo@gmail.com
/// </summary>
namespace Mgk.DataBasex
{
    public class MgkDataBaseOperations : MgkDataBase
    {
        public MgkDataBaseOperations(String ConnectionStr = null, DataBaseEngineEnum DbEngine = DataBaseEngineEnum.No, String PrefixConnectionStr = null) : base(ConnectionStr,DbEngine, PrefixConnectionStr)
        {
            //Console.WriteLine(this.ToString());
        }


        /// <summary>
        /// Lista de objectList Diccionaro de la consulta
        /// </summary>
        /// <param name="query">Consulta SQL</param>
        /// <param name="parameters">Parametros opcionales para la consulta</param>
        /// <param name="fieldDescriptions">La primera fila contiene la descripcion de los campos</param>
        /// <returns></returns>
        public List<object> ReadDictionaryList(String query, List<DbParameter> parameterList = null, bool fieldDescriptions = false, CommandType commandType = CommandType.Text)
        {
            List<object> items = new List<object>();
            try
            {
                using (DbDataReader dataReader = this.GetDataReader(query, parameterList, commandType))
                {
                    if (MgkStaticMessage.Message.Number < 0)
                    {
                        if (dataReader!=null)
                            dataReader.Close();
                        return null;
                    }
                    if (dataReader != null && dataReader.HasRows)
                    {
                        int ix = 0;
                        while (dataReader.HasRows && dataReader.Read())
                        {
                            if (ix++ == 0 && fieldDescriptions)
                            {
                                var item0 = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
                                for (int i = 0; i < dataReader.FieldCount; i++)
                                    item0.Add(dataReader.GetName(i), dataReader.GetDataTypeName(i));
                                items.Add(item0);
                            }
                            var item = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
                            for (int i = 0; i < dataReader.FieldCount; i++)
                                item.Add(dataReader.GetName(i), dataReader.IsDBNull(i) ? null : dataReader.GetValue(i));
                            items.Add(item);
                        }
                        dataReader.Close();
                        Messages.Add(new MgkMessage { Number = ix });
                    }
                    else
                    {
                        if (dataReader!=null)
                            dataReader.Close();
                        Messages.Add(new MgkMessage { Number = 0 });
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                Messages.Add(new MgkMessage
                {
                    Code = "EX-FDL",
                    Number = MgkResponseCode.DB_EXCEPTION_UNKNOW,
                    Exception = ex.ToString(),
                    Source = this.ToString() + ".ReadDictionaryList",
                    OData = new
                    {
                        query = query,
                        //parameterList = parameterList
                        parameterList = ParametersToString(parameterList),
                    }
                });
                MgkLog.Error(Messages.GetLastMessage());
            }
            return items;
        }


        /// <summary>
        /// Obtener consulta listado, diccionario para data table javascript
        /// </summary>
        /// <param name="query"></param>
        /// <param name="parameterList"></param>
        /// <returns></returns>
        public Dictionary<string, object> ReadDictionaryListDT(String query, List<DbParameter> parameterList = null)
        {
            //string jsonResult = "";
            var dict = new Dictionary<string, object>();
            try
            {
                List<object> items = ReadDictionaryList(query, parameterList, true);
                if (MgkStaticMessage.Message.Number < 0)
                {
                    //return null;
                }
                if (items != null && items.Count > 0)
                {
                    dict.Add("fields", items[0]);
                    items.Remove(items[0]);
                }
                else
                {
                    dict.Add("fields", null);
                }
                dict.Add("recordsTotal", items == null ? 0 : items.Count);
                dict.Add("recordsFiltered", items == null ? 0 : items.Count);
                dict.Add("data", items);

                Messages.Clear();
            }
            catch (Exception ex)
            {
                Messages.Add(new MgkMessage
                {
                    Code = "EX-FDLDT",
                    Number = MgkResponseCode.DB_EXCEPTION_UNKNOW,
                    Exception = ex.ToString(),
                    Source = this.ToString() + ".ReadDictionaryListDT",
                    OData = new
                    {
                        query = query,
                        parameterList = ParametersToString(parameterList),
                    }
                });
                MgkLog.Error(Messages.GetLastMessage());
            }
            return dict;
        }

        /// <summary>
        /// Consulta un registro en base de datos, devuelve un Dictionary
        /// </summary>
        /// <param name="query"></param>
        /// <param name="parameterList"></param>
        /// <returns></returns>
        public Dictionary<string, object> ReadDictionaryItem(String query, List<DbParameter> parameterList = null, CommandType commandType = CommandType.Text)
        {
            List<object> items = this.ReadDictionaryList(query, parameterList,false,commandType);
            if (MgkStaticMessage.Message.Number == 0)
            {
                return null;
            }
            else
            {
                if (items != null && items.Count > 0)
                    return (Dictionary<string, object>)items[0];
            }
            return null;
        }

        /// <summary>
        /// Consulta un registro en base de datos, devuelve un json del registro
        /// </summary>
        /// <param name="query"></param>
        /// <param name="parameterList"></param>
        /// <returns></returns>
        public String ReadDictionaryItemJson(String query, List<DbParameter> parameterList = null)
        {
            string output = "";
            var item = ReadDictionaryItem(query, parameterList);
            if (MgkStaticMessage.Message.Number <= 0)
                return output;
            try
            {
                output = Newtonsoft.Json.JsonConvert.SerializeObject(item);
            }
            catch (Exception ex)
            {
                Messages.Add(new MgkMessage
                {
                    Code = "EX-JQ",
                    Number = MgkResponseCode.DB_EXCEPTION_UNKNOW,
                    Exception = ex.ToString(),
                    Source = this.ToString() + ".JsonConsulta",
                    OData = new
                    {
                        query = query,
                        parameterList = ParametersToString(parameterList),
                    }
                });
                MgkLog.Error(Messages.GetLastMessage());
            }
            return output;
        }

        /// <summary>
        /// De una consulta SQL, obtiene una lista con los types de dato de la consulta proporcionada
        /// </summary>
        /// <param name="query"></param>
        /// <param name="parameterList"></param>
        /// <returns></returns>
        public List<object> DataTypesListFromQuery(String query, List<DbParameter> parameterList = null)
        {
            List<object> items = new List<object>();
            try
            {
                using (DbDataReader dataReader = this.GetDataReader(query, parameterList, CommandType.Text))
                {
                    if (MgkStaticMessage.Message.Number < 0)
                    {
                        dataReader.Close();
                        return null;
                    }
                    if (dataReader != null && dataReader.HasRows)
                    {
                        while (dataReader.HasRows && dataReader.Read())
                        {
                            var item = new Dictionary<string, object>();
                            string type = "";
                            for (int i = 0; i < dataReader.FieldCount; i++)
                            {
                                type = dataReader.GetDataTypeName(i);
                                item.Add(dataReader.GetName(i), dataReader.GetDataTypeName(i));
                            }
                            items.Add(item);
                            break;
                        }
                    }
                    dataReader.Close();
                }
            }
            catch (Exception ex)
            {
                Messages.Add(new MgkMessage
                {
                    Code = "EX-OLTD",
                    Number = MgkResponseCode.DB_EXCEPTION_UNKNOW,
                    Exception = ex.ToString(),
                    Source = this.ToString() + ".dataTypesListFromQuery",
                    OData = new
                    {
                        query = query,
                        parameterList = ParametersToString(parameterList)
                    }
                });
                MgkLog.Error(Messages.GetLastMessage());
            }
            return items;
        }

        /// <summary>
        /// Devuelve verdadero si el valor es nulo o inicial de acuerdo al type de dato
        /// </summary>
        /// <param name="valor"></param>
        /// <returns></returns>
        protected bool IsInitial(object value)
        {
            try
            {
                if (value == null)
                    return true;
                else if (value is string)
                    return (string)(value??"") == "";
                else if (value is int)
                    return (int)value == 0;
                else if (value is double)
                    return (double)value == 0;
                else if (value is decimal)
                    return (Decimal)value == 0;
                else if (value is DateTime)
                    return value == null;
                else if (value is short)
                    return (short)value == 0;
                else if (value is byte)
                    return (byte)value == 0;
                else if (value is bool)
                    return value==null;
                else if (value is Boolean)
                    return value == null;
                else if (value is Boolean?)
                    return value == null;
                else
                {
                    // falta incluir este type
                    Messages.Add(new MgkMessage
                    {
                        Code = "EX-II1",
                        Message = "No se puede identificar el tipo de dato del objeto para determinar si tiene valor inicial",
                        Number = MgkResponseCode.DB_EXCEPTION_UNKNOW,
                        Source = this.ToString() + ".isInitial",
                        OData = new
                        {
                            value = value
                        }
                    });
                    MgkLog.Error(Messages.GetLastMessage());
                }
                return true;
            }
            catch (Exception ex)
            {
                Messages.Add(new MgkMessage
                {
                    Code = "EX-II",
                    Number = MgkResponseCode.DB_EXCEPTION_UNKNOW,
                    Exception = ex.ToString(),
                    Source = this.ToString() + ".isInitial",
                    OData = new
                    {
                        value = value
                    }
                });
                MgkLog.Error(Messages.GetLastMessage());
            }
            return true;
        }

        /// <summary>
        /// Crear DbParameter con el type de dato del objeto recibido y su valor
        /// </summary>
        /// <param name="nombre"></param>
        /// <param name="valor"></param>
        /// <returns></returns>
        protected DbParameter CreateParameter(string name, object value)
        {
            try
            {
                return (DbParameter)GetParameter(name, value);
            }
            catch (Exception ex)
            {
                Messages.Add(new MgkMessage
                {
                    Code = "EXP-CP1",
                    Message = "No se puede identificar el tipo de dato del objeto para crear parametro de base de datos",
                    Number = MgkResponseCode.DB_EXCEPTION_UNKNOW,
                    Source = this.ToString() + ".createParameter",
                    Exception = ex.ToString(),
                    OData = new
                    {
                        value = value,
                        name = name
                    }
                });
                MgkLog.Error(Messages.GetLastMessage());
            }
            return null;
        }

        protected DbParameter CreateParameterOut(string name, object value)
        {
            try
            {
                if (value is string)
                    return (DbParameter)GetParameterOut(name, SqlDbType.VarChar, value);
                else if (value is int)
                    return (DbParameter)GetParameterOut(name, SqlDbType.Int, value);
                else if (value is double)
                    return (DbParameter)GetParameterOut(name, SqlDbType.Decimal, value);
                else if (value is decimal)
                    return (DbParameter)GetParameterOut(name, SqlDbType.Decimal, value);
                else if (value is float)
                    return (DbParameter)GetParameterOut(name, SqlDbType.Float, value);
                else if (value is DateTime)
                    return (DbParameter)GetParameterOut(name, SqlDbType.DateTime, value);
                else if (value is short)
                    return (DbParameter)GetParameterOut(name, SqlDbType.SmallInt, value);
                else if (value is byte)
                    return (DbParameter)GetParameterOut(name, SqlDbType.Binary, value);
                else if (value is bool)
                    return (DbParameter)GetParameterOut(name, SqlDbType.Bit, value);
                else if (value == null)
                {
                    Messages.Add(new MgkMessage
                    {
                        Code = "ERR-CP2",
                        Message = "Valor es NULL, no se puede identificar el tipo de dato",
                        Number = MgkResponseCode.SYS_ERROR_UNKNOW,
                        Source = this.ToString() + ".createParameterOut",
                        OData = new
                        {
                            value = value,
                            name = name
                        }
                    });
                    MgkLog.Error(Messages.GetLastMessage());
                    //no se que hacer en este caso
                    //parameterList.Add(GetParameterOut(p.nombre, SqlDbType.Binary, (byte)valor));
                }
                else
                {
                    // falta incluir este type
                    Messages.Add(new MgkMessage
                    {
                        Code = "ERR-CP1",
                        Message = "No se puede identificar el tipo de dato del objeto para crear parametro de base de datos",
                        Number = MgkResponseCode.SYS_ERROR_UNKNOW,
                        Source = this.ToString() + ".createParameterOut",
                        OData = new
                        {
                            value = value,
                            name = name
                        }
                    });
                    MgkLog.Error(Messages.GetLastMessage());
                }
                return (DbParameter)GetParameterOut(name, SqlDbType.VarChar, (string)value);
            }
            catch (Exception ex)
            {
                Messages.Add(new MgkMessage
                {
                    Code = "EXP-CP1",
                    Message = "No se puede identificar el tipo de dato del objeto para crear parametro de base de datos",
                    Number = MgkResponseCode.DB_EXCEPTION_UNKNOW,
                    Source = this.ToString() + ".createParameterOut",
                    Exception = ex.ToString(),
                    OData = new
                    {
                        value = value,
                        name = name
                    }
                });
                MgkLog.Error(Messages.GetLastMessage());
            }
            return null;
        }

        /// <summary>
        /// Retorna el valor maximo de una columna en una tabla
        /// </summary>
        /// <param name="Table_name"></param>
        /// <param name="Colum_name"></param>
        /// <returns></returns>
        public int GetMax(String Table_name,String Colum_name,String Where_condition="")
        {
            if ((Where_condition??"") != "")
                Where_condition = " where "+ Where_condition;
            int Maximo = ExecuteScalarInt("select IFNULL( max(" + Colum_name + "),0) as Maximo from " + Table_name+ Where_condition, null, CommandType.Text);
            return Maximo;
        }

    }
}