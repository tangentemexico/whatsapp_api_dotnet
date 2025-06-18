using Mgk.Commonsx;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Reflection;
using System.Text;

namespace Mgk.DataBasex
{
    /// <summary>
    /// Operaciones basicas de base de datos con Objetos.
    /// Insertar, Actualizar, Buscar, Leer, Borrar
    /// 
    /// </summary>
    public class MgkDataBaseObjT : MgkDataBaseOperations
    {
        public MgkDataBaseObjT(String ConnectionStr = null, DataBaseEngineEnum DbEngine = DataBaseEngineEnum.No, String PrefixConnectionStr = null) : base(ConnectionStr, DbEngine, PrefixConnectionStr)
        {

        }

        /// <summary>
        /// Estructura donde se agrupan datos para: 
        /// </summary>
        protected struct QueryParametersTable
        {
            /// <summary>
            /// Sentencia SQL
            /// </summary>
            public string StrQuery { get; set; }
            /// <summary>
            /// Lista de Parametros
            /// </summary>
            public List<DbParameter> ParameterList { get; set; }
            /// <summary>
            /// Nombre de tabla
            /// </summary>
            public string TableName { get; set; }
        }

        /// <summary>
        /// Estructura donde se agrupan datos para: 
        /// </summary>
        public class Tka
        {
            /// <summary>
            /// Nombre de tabla. 
            /// Este valor se busca en el objeto oObject
            /// </summary>
            public string TableName { get; set; } = "";
            /// <summary>
            /// Campos que forman la llave primaria de la tabla. 
            /// Este valor se busca en el objeto oObject
            /// </summary>
            public string[] TableKeys { get; set; }
            /// <summary>
            /// Bandera para saber si la llave primara es autoincrementable. 
            /// Este valor se busca en el objeto oObject
            /// </summary>
            public bool IsAutoIncrement { get; set; }

            /// <summary>
            /// Lista de parametros en caso de ser necesario en la sentencia SQL.
            /// </summary>
            public List<DbParameter> ParameterList { get; set; } = new List<DbParameter>();

            /// <summary>
            /// Lista de campos que se usaran en la lista de parametros para la sentencia SQL.
            /// </summary>
            public List<string> TableTableKeysList { get; set; } = new List<string>();

            /// <summary>
            /// Lista de campos que no son llaves primarias
            /// </summary>
            public List<string> TableNoKeysList { get; set; } = new List<string>();

            /// <summary>
            /// suma de comprobacion de valores tka encontrados en oObject para construir sentencia dinamica SQL
            /// </summary>
            public int _cheksum_tka { get; set; }
        }

        /// <summary>
        /// Buscar en propiedades estaticas publicas, no publicas, publicas del objeto 
        /// oObject para crear Sentencia dinamica.
        /// </summary>
        /// <param name="oObject"></param>
        /// <param name="t"></param>
        /// <param name="tkaRequired"></param>
        /// <param name="tkaI"></param>
        /// <returns></returns>
        protected Tka GetTka(object oObject, Type t, string tkaRequired = "tka", Tka tkaI = null, bool isInsert = false)
        //protected Tka GetTka(object oObject, PropertyInfo[] propertiesControl, string tkaRequired = "tka", Tka tkaI = new Tka())
        {
            if (tkaI == null)
                tkaI = new Tka();
            tkaI = _GetTka(oObject, t.GetProperties(BindingFlags.Public | BindingFlags.Static), tkaRequired, tkaI, isInsert);
            if (tkaI._cheksum_tka != -3)
            {
                tkaI = _GetTka(oObject, t.GetProperties(BindingFlags.NonPublic | BindingFlags.Instance), tkaRequired, tkaI, isInsert);
                if (tkaI._cheksum_tka != -3)
                    tkaI = _GetTka(oObject, t.GetProperties(), tkaRequired, tkaI, isInsert);
            }
            if (tkaRequired.IndexOf('c') >= 0)
                tkaI = _GetTka(oObject, t.GetProperties(), "c", tkaI, isInsert);
            return tkaI;
        }

        /// <summary>
        /// Obtener datos del objeto oObject para crear Sentencia dinamica
        /// </summary>
        /// <param name="oObject"></param>
        /// <param name="propertiesControl">Atributos del objeto oObject donde seran buscados:
        /// __table_name: Nombre de la tabla,
        /// __table_keys: campos que forman la llave primaria,
        /// __auto_increment: Bandera que indica si el campo llave es autoincrementable.
        /// </param>
        /// <param name="tkaRequired"></param>
        /// <param name="keysForQuery"></param>
        /// <returns></returns>
        protected Tka _GetTka(object oObject, PropertyInfo[] propertiesControl, string tkaRequired, Tka tka, bool isInsert = false)
        {
            Messages.Clear();
            tkaRequired = tkaRequired.ToLower();
            int iT = tkaRequired.LastIndexOf('t');
            int iK = tkaRequired.LastIndexOf('k');
            int iA = tkaRequired.LastIndexOf('a');

            string tmp_key = "";

            try
            {

                #region Obtener todas las columnas del objeto
                if (tkaRequired == "c")
                {
                    if (tka == null)
                        tka = new Tka();

                    tka.TableNoKeysList = new List<string>();
                    foreach (System.Reflection.PropertyInfo p in propertiesControl)
                    {
                        if (p.Name[0] == '_')
                            continue;

                        if (tka.IsAutoIncrement && tka.TableKeys != null) // esto sería para no incluir las TableKeys
                        {
                            int index = Array.IndexOf(tka.TableKeys, p.Name);
                            if (index >= 0)
                                continue;
                        }
                        tka.TableNoKeysList.Add(p.Name);
                    }
                    return tka;
                }
                #endregion

                foreach (System.Reflection.PropertyInfo p in propertiesControl)
                {
                    if (iT + iK + iA == -3)
                        break;
                    object PropertyValue = p.GetValue(oObject, null);
                    if (iT >= 0 && p.Name == "__table_name")
                    {
                        tka.TableName = (string)PropertyValue;
                        iT = -1;
                        continue;
                    }
                    if (iA >= 0 && p.Name == "__auto_increment")
                    {
                        tka.IsAutoIncrement = (bool)PropertyValue;
                        iA = -1;
                        continue;
                    }
                    //if (p.Name == "__table_keys" && keysForQuery == null)
                    if (iK >= 0 && p.Name == "__table_keys")
                    {
                        iK = -1;
                        tka.TableKeys = (string[])PropertyValue;
                    }
                }

                //
                {
                    if ((tka.TableKeys != null))
                        foreach (string key in tka.TableKeys)
                        {
                            tmp_key = key;
                            PropertyInfo NumberPropertyInfo = oObject.GetType().GetProperty(key);
                            if (NumberPropertyInfo != null)
                            {
                                var value = NumberPropertyInfo.GetValue(oObject, null);
                                tka.TableTableKeysList.Add(string.Format("{0}=@{1}", key, key));
                                if (!isInsert || (!isInsert && !tka.IsAutoIncrement))
                                {
                                    if (!tka.ParameterList.Exists(item => item.ParameterName == key))
                                        tka.ParameterList.Add(CreateParameter(key, value));
                                }
                            }
                        }
                }
            }
            catch (Exception e)
            {
                Messages.Add(new MgkMessage
                {
                    Code = "EX-TKA",
                    Exception = e.ToString(),
                    Message = "Error en operaciones de base de datos con objetos",
                    Messagex = "Error al intentar obtener las propiedades del objeto para __table_name,__table_keys,__auto_increment",
                    Number = MgkResponseCode.SYS_EXCEPTION_UNKNOW,
                    OData = new
                    {
                        oObject = oObject,
                        propertiesControl = propertiesControl,
                        tkaRequired = tkaRequired,
                        tmp_key = tmp_key,
                        //keysForQuery= keysForQuery,
                        tka = tka
                    }
                });
                MgkLog.Error(Messages.GetLastMessage());
            }
            tka._cheksum_tka = iT + iK + iA;
            return tka;
        }


        /// <summary>
        /// Crear la consulta SQL y lista de parametros DbParameter de un objeto, las devuelve una estructura con los valores
        /// </summary>
        /// <param name="objeto"></param>
        /// <param name="keysForQuery"></param>
        /// <returns>Array[string,List<DbParameter>]</returns>
        protected QueryParametersTable CreateQptByKeys(object oObject, string[] keysForQuery = null)
        {
            QueryParametersTable qpt = new QueryParametersTable();
            Tka tka;
            var setD = new Dictionary<string, object>();
            string StrQuery = "";
            string txtWhere = "";
            try
            {
                Type t = oObject.GetType();
                tka = this.GetTka(oObject, t, "t" + (keysForQuery == null ? "k" : ""));

                if (tka.TableName == "")
                {
                    MgkLog.Error(new MgkMessage
                    {
                        Code = "Mgk.Gca.TableName",
                        Message = "Error en operaciones de base de datos con objetos",
                        Messagex = "atributo '__table_name' no encontrado en objeto enviado",
                        OData = new
                        {
                            oObject = oObject,
                            propertiesPublic = t.GetProperties(),
                            propertiesNonPublic = t.GetProperties(BindingFlags.NonPublic | BindingFlags.Instance),
                        }
                    });
                    return qpt;
                }

                if (keysForQuery != null)
                {
                    if (keysForQuery.Length == 0)
                        tka.ParameterList = null;
                    tka.TableTableKeysList.Clear();
                    foreach (string key in keysForQuery)
                    {
                        try
                        {
                            PropertyInfo NumberPropertyInfo = oObject.GetType().GetProperty(key);
                            var value = NumberPropertyInfo.GetValue(oObject, null);
                            tka.TableTableKeysList.Add(string.Format("{0}=@{1}", key, key));
                            if (!tka.ParameterList.Exists(item => item.ParameterName == key))
                                tka.ParameterList.Add(CreateParameter(key, value));
                        }
                        catch (Exception ex)
                        {
                            this.Messages.Add(new MgkMessage
                            {
                                Source = "CreateQueryParameters",
                                Exception = ex.ToString(),
                                Message = "Error en operaciones de base de datos con objetos",
                                Messagex = "Error al crear consulta dinamica.Alguna de las TableKeys no existe en la clase para la tabla " + tka.TableName,
                                //Messagex = "alguna de las TableKeys no existe en la clase"
                            });
                            MgkLog.Error(Messages.GetLastMessage());
                        }
                    }
                }

                for (int i = 0; i < tka.TableTableKeysList.Count; i++)
                    txtWhere += tka.TableTableKeysList[i] + ((i + 1 < tka.TableTableKeysList.Count) ? " and " : "");

                if (txtWhere != "")
                    txtWhere = "where " + txtWhere;
                StrQuery = string.Format("select * from {0} {1}", tka.TableName, txtWhere);
                qpt.StrQuery = StrQuery;
                qpt.ParameterList = tka.ParameterList;
                qpt.TableName = tka.TableName;
            }
            catch (Exception ex)
            {
                this.Messages.Add(new MgkMessage
                {
                    Source = "MgkDataBaseObj.CreateQueryParameters",
                    Exception = ex.ToString(),
                    Message = "Error en operaciones de base de datos con objetos",
                    OData = new
                    {
                        oObject = oObject,
                        keysForQuery = keysForQuery
                    },
                    Messagex = "Error al crear sentencia sql dinamica, desconocido requiere debug"
                });
                MgkLog.Error(Messages.GetLastMessage());
                // Ocurrio algun Error, se debe resolver...
            }
            return qpt;
        }

        /// <summary>
        /// Crear consulta SQL y lista de parametros con los valores no nulos del objeto
        /// </summary>
        /// <param name="objeto"></param>
        /// <returns></returns>
        protected QueryParametersTable CreateQptByValues(object oObject)
        {
            QueryParametersTable qpt = new QueryParametersTable();
            Tka tka;
            string StrQuery = "";
            string txtWhere = "";

            try
            {
                Type t = oObject.GetType();
                System.Reflection.PropertyInfo[] properties = t.GetProperties();
                System.Reflection.PropertyInfo[] propertiesControl = t.GetProperties(BindingFlags.NonPublic | BindingFlags.Instance);
                tka = this.GetTka(oObject, t);

                if (tka.TableName == "")
                {
                    Messages.Add(new MgkMessage
                    {
                        Code = "Mgk.Gca.TableName",
                        Message = "Error en operaciones de base de datos con objetos",
                        Messagex = "atributo '__table_name' no encontrado en objeto enviado, No se puede construir el query dinamicamente",
                        OData = new
                        {
                            oObject = oObject,
                            propertiesControl = propertiesControl
                        }
                    });
                    MgkLog.Error(Messages.GetLastMessage());
                    return qpt;
                }

                foreach (System.Reflection.PropertyInfo p in properties)
                {
                    object PropertyValue = p.GetValue(oObject, null);
                    if (p.Name[0] == '_')
                        continue;
                    if (!IsInitial(PropertyValue))
                    {
                        PropertyInfo NumberPropertyInfo = oObject.GetType().GetProperty(p.Name);
                        var value = NumberPropertyInfo.GetValue(oObject, null);
                        tka.TableTableKeysList.Add(string.Format("{0}=@{1}", p.Name, p.Name));
                        if (!tka.ParameterList.Exists(item => item.ParameterName == p.Name))
                            tka.ParameterList.Add(CreateParameter(p.Name, value));
                    }
                }

                for (int i = 0; i < tka.TableTableKeysList.Count; i++)
                    txtWhere += tka.TableTableKeysList[i] + ((i + 1 < tka.TableTableKeysList.Count) ? " and " : "");

                if (txtWhere != "")
                    txtWhere = "where " + txtWhere;
                StrQuery = string.Format("select * from {0} {1}", tka.TableName, txtWhere);
                qpt.StrQuery = StrQuery;
                qpt.ParameterList = tka.ParameterList;
                qpt.TableName = tka.TableName;
            }
            catch (Exception ex)
            {
                this.Messages.Add(new MgkMessage
                {
                    Source = "MgkDataBaseObj.CreateQueryParameters",
                    Exception = ex.ToString(),
                    Message = "Error en operaciones de base de datos con objetos",
                    Messagex = "Error no identificado al crear consulta dinamica. Requiere debug",
                    OData = new
                    {
                        oObject = oObject,
                    },
                });
                MgkLog.Error(Messages.GetLastMessage());
                // Ocurrio algun Error, se debe resolver...
            }
            return qpt;
        }

        /// <summary>
        /// Crear consulta dinamica SQL sobre la tabla del objeto @oObject, agregando al final una condicion @condition
        /// </summary>
        /// <param name="oObject"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        protected QueryParametersTable CreateQptByCondition(object oObject, string condition)
        {
            Messages.Clear();
            Tka tka;
            QueryParametersTable qpt = new QueryParametersTable();
            string StrQuery = "";
            try
            {

                Type t = oObject.GetType();
                tka = this.GetTka(oObject, t, "t");

                if (tka.TableName == "")
                {
                    Messages.Add(new MgkMessage
                    {
                        Code = "Mgk.Gca.TableName",
                        Message = "Error en operaciones de base de datos con objetos",
                        Messagex = "atributo '__table_name' no encontrado en objeto enviado, No se puede construir el query dinamicamente",
                        OData = new
                        {
                            oObject = oObject,
                            PropertiesNonPublic = t.GetProperties(BindingFlags.NonPublic | BindingFlags.Instance)
                            //propertiesControl = propertiesControl
                        }
                    });
                    MgkLog.Error(Messages.GetLastMessage());
                    return qpt;
                }

                if (condition != "")
                    condition = " where " + condition;
                StrQuery = string.Format("select * from {0} {1}", tka.TableName, condition);
                qpt.StrQuery = StrQuery;
                qpt.TableName = tka.TableName;
            }
            catch (Exception ex)
            {
                this.Messages.Add(new MgkMessage
                {
                    Source = "MgkDataBaseObj.CreateQptByCondition",
                    Exception = ex.ToString(),
                    Message = "Error en operaciones de base de datos con objetos",
                    Messagex = "Error desconocido al crear consulta dinamica. Requiere debug",
                    OData = new
                    {
                        oObject = oObject,
                        condition = condition
                    },
                });
                MgkLog.Error(Messages.GetLastMessage());
                // Ocurrio algun Error, se debe resolver...
            }
            return qpt;
        }

        public int DictionaryToObject(object Obj, object oObject)
        {
            return DictionaryToObject((Dictionary<string, object>)Obj, oObject);
        }

        public int DictionaryToObject(Dictionary<string, object> Obj, object oObject)
        {
            string tmpName = "";
            object PropertyValue = null;
            Messages.Clear();

            try
            {
                if (Obj.Count == 0)
                {
                    this.Messages.AddError(this.ToString(), "DictionaryToObject", "Obj es vacio");
                    return -1;
                }
                Type t = oObject.GetType();
                System.Reflection.PropertyInfo[] properties = t.GetProperties();
                foreach (System.Reflection.PropertyInfo p in properties)
                {
                    tmpName = p.Name;
                    //if (this.DataBaseEngine == DataBaseEngineEnum.Npgsql)
                    //    tmpName = tmpName.ToLower();
                    if (Obj.ContainsKey(tmpName) == false)
                        continue;
                    PropertyInfo propertyInfo = oObject.GetType().GetProperty(p.Name);
                    PropertyValue = propertyInfo.GetValue(oObject, null);
                    var DicItemValue = Obj[tmpName];

                    try
                    {
                        
                        if (propertyInfo.PropertyType.Name.IndexOf("Null") >= 0) { 

                        }  

                        if (DicItemValue is bool || PropertyValue is bool|| propertyInfo.PropertyType.AssemblyQualifiedName.ToLower().IndexOf("bool") >= 0)
                            if (DicItemValue is int || DicItemValue is short || DicItemValue is Int16 || DicItemValue is Int32 || DicItemValue is UInt64)
                                propertyInfo.SetValue(oObject, ((UInt64)DicItemValue) == 1);
                            else
                                propertyInfo.SetValue(oObject, DicItemValue);
                        else if (DicItemValue is Boolean || PropertyValue is Boolean)
                            if (DicItemValue is int || DicItemValue is short || DicItemValue is Int16 || DicItemValue is Int32 || DicItemValue is UInt64)
                                propertyInfo.SetValue(oObject, ((UInt64)DicItemValue) == 1);
                            else
                                propertyInfo.SetValue(oObject, DicItemValue);
                        else if (DicItemValue is DateTime || PropertyValue is DateTime || propertyInfo.PropertyType.AssemblyQualifiedName.ToLower().IndexOf("datetime") >= 0)
                        {
                            if (DicItemValue != null)
                            {
                                if (propertyInfo.PropertyType.Name == "DateTime")
                                {
                                    propertyInfo.SetValue(oObject, Convert.ChangeType(DicItemValue, propertyInfo.PropertyType), null);
                                }
                                else
                                {
                                    Type u = Nullable.GetUnderlyingType(propertyInfo.PropertyType);
                                    propertyInfo.SetValue(oObject, Convert.ChangeType(DicItemValue, u), null);
                                }
                            }
                        }
                        else if (PropertyValue is int || PropertyValue is short || PropertyValue is float || PropertyValue is Decimal || PropertyValue is Double)
                        {
                            propertyInfo.SetValue(oObject, Convert.ChangeType(DicItemValue ?? "0", propertyInfo.PropertyType), null);
                        }
                        else if (DicItemValue != null)
                            ///propertyInfo.SetValue(objeto, Convert.ChangeType(Obj[p.Name].ToString(), propertyInfo.PropertyType), null);
                            propertyInfo.SetValue(oObject, Convert.ChangeType(DicItemValue, propertyInfo.PropertyType), null);
                    }
                    catch (Exception ex1)
                    {
                        Messages.Add(new MgkMessage
                        {
                            Code = "EX-DO1",
                            Number = MgkResponseCode.DB_EXCEPTION_UNKNOW,
                            Exception = ex1.ToString(),
                            Message = "### Error AL CONVERTIR DICCIONARIO EN OBJETO",
                            Messagex = "Error al convertir diccionario a objeto",
                            Source = this.ToString() + ".DictionaryToObject",
                            OData = new
                            {
                                Obj = Obj,
                                oObject = oObject,
                                tmpName = tmpName
                            }
                        });
                        MgkLog.Error(Messages.GetLastMessage());
                    }

                }
                return 1;
            }
            catch (Exception ex)
            {
                Messages.Add(new MgkMessage
                {
                    Code = "EX-DO",
                    Number = MgkResponseCode.DB_EXCEPTION_UNKNOW,
                    Exception = ex.ToString(),
                    Message = "Error en operaciones de base de datos con objetos",
                    Messagex = "Error al convertir diccionario a objeto",
                    Source = this.ToString() + ".DictionaryToObject",
                    OData = new
                    {
                        Obj = Obj,
                        oObject = oObject,
                        tmpName = tmpName
                    }
                });
                MgkLog.Error(Messages.GetLastMessage());
            }
            return -1;
        }

        /// <summary>
        /// Llena una lista de parametros desde un objeto omitiendo las propiedades que su nombre inician con guion bajo '_'
        /// </summary>
        /// <param name="objeto"></param>
        /// <returns></returns>
        protected List<DbParameter> ParametersFromObject(object oObject)
        {
            List<DbParameter> ParameterList = new List<DbParameter>();
            try
            {
                Type t = oObject.GetType();
                System.Reflection.PropertyInfo[] properties = t.GetProperties();

                foreach (System.Reflection.PropertyInfo p in properties)
                {
                    if (p.Name[0] == '_')
                        continue;
                    object PropertyValue = p.GetValue(oObject, null);
                    if (PropertyValue is string)
                        ParameterList.Add((DbParameter)GetParameterOut(p.Name, SqlDbType.Text, (string)PropertyValue));
                    else if (PropertyValue is int)
                        ParameterList.Add((DbParameter)GetParameterOut(p.Name, SqlDbType.Int, (int)PropertyValue));
                    else if (PropertyValue is double)
                        ParameterList.Add((DbParameter)GetParameterOut(p.Name, SqlDbType.Decimal, (Decimal)PropertyValue));
                    else if (PropertyValue is decimal)
                        ParameterList.Add((DbParameter)GetParameterOut(p.Name, SqlDbType.Decimal, (Decimal)PropertyValue));
                    else if (PropertyValue is DateTime)
                        ParameterList.Add((DbParameter)GetParameterOut(p.Name, SqlDbType.DateTime, (DateTime)PropertyValue));
                    else if (PropertyValue is short)
                        ParameterList.Add((DbParameter)GetParameterOut(p.Name, SqlDbType.SmallInt, (short)PropertyValue));
                    else if (PropertyValue is byte)
                        ParameterList.Add((DbParameter)GetParameterOut(p.Name, SqlDbType.Binary, (byte)PropertyValue));
                    else if (PropertyValue is bool)
                        ParameterList.Add((DbParameter)GetParameterOut(p.Name, SqlDbType.Bit, (bool)PropertyValue));
                    else if (PropertyValue == null)
                    {
                        //no se que hacer en este caso
                        //ParameterList.Add(GetParameterOut(p.Name, SqlDbType.Binary, (byte)PropertyValue));
                    }
                    else
                    {
                        // falta incluir este type
                        Messages.Add(new MgkMessage
                        {
                            Code = "ERR-PFO",
                            Message = "Error en operaciones de base de datos con objetos",
                            Messagex = "Tipo de dato no identificado",
                            Number = MgkResponseCode.SYS_ERROR_UNKNOW,
                            Source = this.ToString() + ".parametersFromObject",
                            OData = new
                            {
                                oObject = oObject,
                                type = t
                            }
                        });
                        MgkLog.Error(Messages.GetLastMessage());
                    }
                }
            }
            catch (Exception ex)
            {
                Messages.Add(new MgkMessage
                {
                    Code = "EX-PFO",
                    Exception = ex.ToString(),
                    Message = "Error en operaciones de base de datos con objetos",
                    Messagex = "Error al crear parametros desde objeto",
                    Number = MgkResponseCode.SYS_ERROR_UNKNOW,
                    Source = this.ToString() + ".parametersFromObject",
                    OData = new
                    {
                        oObject = oObject,
                    }
                });
                MgkLog.Error(Messages.GetLastMessage());
                // Ocurrio algun Error, se debe resolver...
            }
            return ParameterList;
        }

        /// <summary>
        /// Leer objeto desde base de datos devolviendo un nuevo objeto de la misma clase. 
        /// En caso de no encontrar, devuelve null
        /// </summary>
        /// <param name="oObject"></param>
        /// <param name="keysForQuery"></param>        
        /// <returns></returns>
        public T ReadNew<T>(object oObject/*, string[] keysForQuery = null*/)
        {
            return ReadObject<T>(oObject, null, true);
        }

        public T ReadNew<T>(object oObject, string keysForQuery = null)
        {
            return ReadObject<T>(oObject, keysForQuery == null ? null : (keysForQuery.Split(',')), true);
        }

        /// <summary>
        /// Leer objeto desde base de datos, los valores encontrados sobre el mismo objeto recibido
        /// </summary>
        /// <param name="oObject"></param>
        /// <param name="keysForQuery"></param>
        /// <returns></returns>
        public T ReadByKeys<T>(object oObject, string[] keysForQuery = null)
        {
            return ReadObject<T>(oObject, keysForQuery);
        }

        public T Read<T>(object oObject, string keysForQuery = null)
        {
            return ReadObject<T>(oObject, keysForQuery == null ? null : (keysForQuery.Split(',')));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="oObject"></param>
        /// <param name="keysForQuery"></param>
        /// <param name="newObject">true: El resultado en un nuevo objeto, false: los valores sobre el mismo objeto</param>
        /// <returns></returns>
        public T ReadObject<T>(object oObject, string[] keysForQuery = null, bool newObject = false)
        {
            Messages.Clear();
            T newInstance = default(T);
            try
            {
                QueryParametersTable qpt = CreateQptByKeys(oObject, keysForQuery);
                if (MgkStaticMessage.Message.Number < 0)
                {
                    MgkStaticMessage.Message.SetMessage(new MgkMessage
                    {
                        Number = 0,
                        Code = "-1",
                        Message = "No se encontro informacion",
                    });
                    return default(T);
                    //if (newObject)
                    //    return default(T);
                    //else
                    //    return -1;
                }
                Dictionary<string, object> Obj = (Dictionary<string, object>)ReadDictionaryItem(qpt.StrQuery, qpt.ParameterList);
                if (Obj != null)
                {
                    if (newObject)
                    {
                        newInstance = (T)Activator.CreateInstance(oObject.GetType());
                        DictionaryToObject(Obj, newInstance);
                    }
                    else
                    {
                        DictionaryToObject(Obj, oObject);
                    }
                    if (MgkStaticMessage.Message.Number == 0)
                    {
                        MgkStaticMessage.Message.SetMessage(new MgkMessage
                        {
                            Number = 1,
                            Message = "Registro encontrado",
                            OData = new
                            {
                                keysForQuery = keysForQuery
                            }
                        });
                    }
                }
                else
                {
                    MgkStaticMessage.Message.SetMessage(new MgkMessage
                    {
                        Number = 0,
                        Code = "-1",
                        Message = "No se encontro informacion",
                    });
                    return default(T);
                    //if (newObject)
                    //    return null;
                    //else
                    //    return -1;
                }
            }
            catch (Exception ex)
            {
                Messages.Add(new MgkMessage
                {
                    Code = "EX-RO",
                    Message = "Error en operaciones de base de datos con objetos",
                    Messagex = "Error al leer objeto de base de datos. ReadObject",
                    Number = MgkResponseCode.DB_EXCEPTION_UNKNOW,
                    Exception = ex.ToString(),
                    Source = this.ToString() + ".ReadObject",
                    OData = new
                    {
                        oObject = oObject,
                        keysForQuery = keysForQuery,
                    }
                });
                MgkLog.Error(Messages.GetLastMessage());
                MgkStaticMessage.Message.SetMessage(new MgkMessage
                {
                    Number = MgkResponseCode.DB_NOT_FOUND,
                    Code = "DB_NOT_FOUND",
                    Message = "No se encontro informacion",
                });
                return default(T);
                //if (newObject)
                //    return null;
                //else
                //    return -1;
                // Ocurrio algun Error, se debe resolver...
            }
            return newInstance;
            //if (newObject)
            //    return newInstance;
            //else
            //    return 1;
        }

        public int Insert(object oObject, string[] fields = null, bool IsAutoIncremental = false)
        {
            return InsertObject(oObject, fields, IsAutoIncremental);
        }

        /// <summary>
        /// Construye y ejecuta Sentencia SQL dinamico para Insertar registro en base de datos
        /// </summary>
        /// <param name="oObject"></param>
        /// <param name="fields"></param>
        /// <param name="IsAutoIncrement"></param>
        /// <returns></returns>
        public int InsertObject(object oObject, string[] fields = null, bool IsAutoIncrement = false)
        {
            Messages.Clear();
            Tka tka = new Tka { IsAutoIncrement = IsAutoIncrement };

            List<string> partesSet = new List<string>();
            //var setD = new Dictionary<string, object>();
            string StrQuery = "";
            string txtfields = "";
            string txtValores = "";

            try
            {
                Type t = oObject.GetType();
                System.Reflection.PropertyInfo[] properties = t.GetProperties();
                //tka = this.GetTka(oObject, t.GetProperties(BindingFlags.NonPublic | BindingFlags.Instance), "tka", tka);
                tka = this.GetTka(oObject, t, "tka", tka, true);

                if (tka.TableName == "")
                {
                    Messages.Add(new MgkMessage
                    {
                        Number = -100000,
                        Message = "Error en operaciones de base de datos con objetos",
                        Messagex = "Error al Insertar registro,El objeto no tiene la propiedad '__table_name' para crear la consulta dinamica",
                        OData = new
                        {
                            oObject = oObject,
                            fields = fields,
                            tka = tka,
                            PropertiesNonPublic = t.GetProperties(BindingFlags.NonPublic | BindingFlags.Instance)
                        }
                    });
                    MgkLog.Error(Messages.GetLastMessage());
                }

                foreach (System.Reflection.PropertyInfo p in properties)
                {
                    object PropertyValue = p.GetValue(oObject, null);
                    if (p.Name[0] == '_')
                        continue;
                    if (fields != null)
                    {
                        int index = Array.IndexOf(fields, p.Name);
                        if (index < 0)
                            continue;
                    }
                    //if (tka.TableKeys != null) // esto sería para no incluir las TableKeys
                    if (tka.IsAutoIncrement && tka.TableKeys != null) // esto sería para no incluir las TableKeys
                    {
                        int index = Array.IndexOf(tka.TableKeys, p.Name);
                        if (index >= 0)
                            continue;
                    }
                    if (PropertyValue != null)
                    {
                        var par = CreateParameter(p.Name, PropertyValue);
                        if (par == null)
                            throw new Exception(string.Format("No fue posible crear Parameter para Insert variable {0} en tabla {1}", p.Name, tka.TableName));
                        if (!tka.ParameterList.Exists(item => item.ParameterName == p.Name))
                            tka.ParameterList.Add(par);
                        partesSet.Add(string.Format("{0}", p.Name));
                    }

                }
                for (int i = 0; i < partesSet.Count; i++)
                    txtfields += partesSet[i] + ((i + 1 < partesSet.Count) ? "," : "");
                for (int i = 0; i < partesSet.Count; i++)
                    txtValores += "@" + partesSet[i] + ((i + 1 < partesSet.Count) ? " , " : "");

                
                StrQuery = string.Format("Insert into {0} ({1}) values ({2})", tka.TableName, txtfields, txtValores);
                if (tka.IsAutoIncrement)
                {
                    StrQuery += ";" + this.LastIdQuery(tka.TableName, tka.TableKeys[0]);

                    int resultado = this.ExecuteScalarInt(StrQuery, tka.ParameterList, System.Data.CommandType.Text);
                    if (tka.TableKeys != null && tka.TableKeys.Length > 0)
                    {
                        PropertyInfo propertyInfo = oObject.GetType().GetProperty(tka.TableKeys[0]);
                        propertyInfo.SetValue(oObject, Convert.ChangeType(resultado, propertyInfo.PropertyType), null);
                    }
                    if (resultado > 0)
                    {
                        this.Messages.Message = "Registro Insertado correctamente " + resultado;
                    }

                    return resultado;
                }
                return this.ExecuteNonQuery(StrQuery, tka.ParameterList, System.Data.CommandType.Text);
            }
            catch (Exception ex)
            {
                Messages.Add(new MgkMessage
                {
                    Code = "EX-INS",
                    Exception = ex.ToString(),
                    //Message = "Error en operaciones de base de datos con objetos."+ ex.Message,
                    Message = "Error al insertar objeto DBOT." + ex.Message,
                    //Messagex = "Error al Insertar objeto",
                    Number = MgkResponseCode.SYS_ERROR_UNKNOW,
                    Source = this.ToString() + ".InsertObject",
                    OData = new
                    {
                        StrQuery = StrQuery,
                        oObject = oObject,
                        fields = fields,
                        tka = tka
                    }
                });
                MgkLog.Error(Messages.GetLastMessage());
                // Ocurrio algun Error, se debe resolver...
            }
            return -1;
        }

        public int InsertObjectTka(object oObject, Tka tka,string[] fields = null, bool IsAutoIncrement = false)
        {
            Messages.Clear();

            List<string> partesSet = new List<string>();
            //var setD = new Dictionary<string, object>();
            string StrQuery = "";
            string txtfields = "";
            string txtValores = "";

            try
            {
                Type t = oObject.GetType();
                System.Reflection.PropertyInfo[] properties = t.GetProperties();
                //tka = this.GetTka(oObject, t.GetProperties(BindingFlags.NonPublic | BindingFlags.Instance), "tka", tka);

                if (tka.TableName == "")
                {
                    Messages.Add(new MgkMessage
                    {
                        Number = -100000,
                        Message = "Error en operaciones de base de datos con objetos",
                        Messagex = "Error al Insertar registro,El objeto no tiene la propiedad '__table_name' para crear la consulta dinamica",
                        OData = new
                        {
                            oObject = oObject,
                            fields = fields,
                            tka = tka,
                            PropertiesNonPublic = t.GetProperties(BindingFlags.NonPublic | BindingFlags.Instance)
                        }
                    });
                    MgkLog.Error(Messages.GetLastMessage());
                }

                foreach (System.Reflection.PropertyInfo p in properties)
                {
                    object PropertyValue = p.GetValue(oObject, null);
                    if (p.Name[0] == '_')
                        continue;
                    if (fields != null)
                    {
                        int index = Array.IndexOf(fields, p.Name);
                        if (index < 0)
                            continue;
                    }
                    //if (tka.TableKeys != null) // esto sería para no incluir las TableKeys
                    if (tka.IsAutoIncrement && tka.TableKeys != null) // esto sería para no incluir las TableKeys
                    {
                        int index = Array.IndexOf(tka.TableKeys, p.Name);
                        if (index >= 0)
                            continue;
                    }
                    if (PropertyValue != null)
                    {
                        var par = CreateParameter(p.Name, PropertyValue);
                        if (par == null)
                            throw new Exception(string.Format("No fue posible crear Parameter para Insert variable {0} en tabla {1}", p.Name, tka.TableName));
                        if (!tka.ParameterList.Exists(item => item.ParameterName == p.Name))
                            tka.ParameterList.Add(par);
                        partesSet.Add(string.Format("{0}", p.Name));
                    }

                }
                for (int i = 0; i < partesSet.Count; i++)
                    txtfields += partesSet[i] + ((i + 1 < partesSet.Count) ? "," : "");
                for (int i = 0; i < partesSet.Count; i++)
                    txtValores += "@" + partesSet[i] + ((i + 1 < partesSet.Count) ? " , " : "");

                StrQuery = string.Format("Insert into {0} ({1}) values ({2})", tka.TableName, txtfields, txtValores);
                if (tka.IsAutoIncrement)
                {
                    StrQuery += ";" + this.LastIdQuery(tka.TableName, tka.TableKeys[0]);

                    int resultado = this.ExecuteScalarInt(StrQuery, tka.ParameterList, System.Data.CommandType.Text);
                    if (tka.TableKeys != null && tka.TableKeys.Length > 0)
                    {
                        PropertyInfo propertyInfo = oObject.GetType().GetProperty(tka.TableKeys[0]);
                        propertyInfo.SetValue(oObject, Convert.ChangeType(resultado, propertyInfo.PropertyType), null);
                    }
                    if (resultado > 0)
                    {
                        this.Messages.Message = "Registro Insertado correctamente " + resultado;
                    }

                    return resultado;
                }
                return this.ExecuteNonQuery(StrQuery, tka.ParameterList, System.Data.CommandType.Text);
            }
            catch (Exception ex)
            {
                Messages.Add(new MgkMessage
                {
                    Code = "EX-INS",
                    Exception = ex.ToString(),
                    Message = "Error en operaciones de base de datos con objetos",
                    Messagex = "Error al Insertar objeto",
                    Number = MgkResponseCode.SYS_ERROR_UNKNOW,
                    Source = this.ToString() + ".InsertObject",
                    OData = new
                    {
                        StrQuery = StrQuery,
                        oObject = oObject,
                        fields = fields,
                        tka = tka
                    }
                });
                MgkLog.Error(Messages.GetLastMessage());
                // Ocurrio algun Error, se debe resolver...
            }
            return -1;
        }

        public int UpdateByFields(object oObject, string[] fields = null)
        {
            return UpdateObject(oObject, fields);
        }

        public int Update(object oObject, String fields = null)
        {
            return UpdateObject(oObject, (fields == null) ? null : (fields.Split(',')));
        }

        protected Boolean FieldInArray(string[] fields, string field)
        {
            if (fields == null)
                return false;
            foreach (string s in fields)
                if (s == field)
                    return true;
            return false;
        }

        public int UpdateObject(object oObject, string[] fields = null, Boolean UpdateNull = false)
        {
            Messages.Clear();
            Tka tka = new Tka();
            List<string> partesSet = new List<string>();
            var setD = new Dictionary<string, object>();
            //string[] TableKeys = null;
            string StrQuery = "";
            string txtSet = "";
            string txtWhere = "";
            try
            {
                Type t = oObject.GetType();
                System.Reflection.PropertyInfo[] properties = t.GetProperties();
                tka = this.GetTka(oObject, t, "tka");

                if (tka.TableName == "")
                {
                    Messages.Add(new MgkMessage
                    {
                        Code = "Mgk.Gca.TableName",
                        Message = "Error en operaciones de base de datos con objetos",
                        Messagex = "atributo '__table_name' no encontrado en objeto enviado, No se puede construir el query dinamicamente",
                        Source = "MgkDataBaseObj.UpdateObject",
                        OData = new
                        {
                            oObject = oObject,
                            fields = fields,
                            //propertiesControl = propertiesControl
                        }
                    });
                    MgkLog.Error(Messages.GetLastMessage());
                    return -1;
                }

                foreach (System.Reflection.PropertyInfo p in properties)
                {
                    object PropertyValue = p.GetValue(oObject, null);
                    if (p.Name[0] == '_')
                        continue;
                    if (fields != null)
                    {
                        int index = Array.IndexOf(fields, p.Name);
                        if (index < 0)
                            continue;
                    }
                    if (tka.IsAutoIncrement)
                    {
                        if (tka.TableKeys != null) // esto sería para no incluir las TableKeys
                        {
                            int index = Array.IndexOf(tka.TableKeys, p.Name);
                            if (index >= 0)
                            {
                                if (!tka.ParameterList.Exists(item => item.ParameterName == p.Name))
                                    tka.ParameterList.Add(CreateParameter(p.Name, PropertyValue));
                                continue;
                            }
                        }
                    }
                    if (PropertyValue != null || FieldInArray(fields, p.Name) || UpdateNull)
                    {
                        int index = Array.IndexOf(tka.TableKeys, p.Name);
                        if (index < 0)
                            tka.ParameterList.Add(CreateParameter(p.Name, PropertyValue));
                        setD.Add(p.Name, string.Format("{0}=@{1}", p.Name, p.Name));
                        partesSet.Add(string.Format("{0}=@{1}", p.Name, p.Name));
                    }

                }
                for (int i = 0; i < partesSet.Count; i++)
                    txtSet += partesSet[i] + ((i + 1 < partesSet.Count) ? "," : "");
                for (int i = 0; i < tka.TableTableKeysList.Count; i++)
                    txtWhere += tka.TableTableKeysList[i] + ((i + 1 < tka.TableTableKeysList.Count) ? " and " : "");

            }
            catch (Exception ex)
            {
                Messages.Add(new MgkMessage
                {
                    Code = "EX-UPD",
                    Number = MgkResponseCode.DB_EXCEPTION_UNKNOW,
                    Exception = ex.ToString(),
                    Source = "MgkDataBaseObj.UpdateObject",
                    OData = oObject
                });
                MgkLog.Error(Messages.GetLastMessage());
                // Ocurrio algun Error, se debe resolver...
            }
            StrQuery = string.Format("Update {0} set {1} where {2}", tka.TableName, txtSet, txtWhere);
            MgkMessage MessageU = new MgkMessage();
            try
            {
                MessageU.Number = this.ExecuteNonQuery(StrQuery, tka.ParameterList, System.Data.CommandType.Text);
                MessageU.Message = "Actualización exitosa";
            }
            catch(Exception ex)
            {
                MessageU.Number = -1;
                MessageU.Message = "Error en actualización del registro";
                MessageU.Exception = ex.ToString();
                MgkLog.Error(MessageU);
                //MessageU.Exception = "";
            }

            Messages.Add(MessageU);
            return MessageU.Number;
        }

        public int DeleteObject(object oObject)
        {
            Messages.Clear();
            Tka tka = new Tka();
            //string[] TableKeys = null;

            List<string> partesSet = new List<string>();
            var setD = new Dictionary<string, object>();
            string StrQuery = "";
            string txtWhere = "";
            try
            {
                Type t = oObject.GetType();
                tka = this.GetTka(oObject, t, "tk");

                if (tka.TableName == "")
                {
                    Messages.Add(new MgkMessage
                    {
                        Code = "Mgk.Gca.TableName",
                        Message = "Error en operaciones de base de datos con objetos",
                        Messagex = "atributo '__table_name' no encontrado en objeto enviado,No se puede construir el query dinamicamente",
                        Source = "MgkDataBaseObj.DeleteObject",
                        OData = new
                        {
                            oObject = oObject,
                            tka = tka,
                            PropertiesNonPublic = t.GetProperties(BindingFlags.NonPublic | BindingFlags.Instance)
                        }
                    });
                    MgkLog.Error(Messages.GetLastMessage());
                    return -1;
                }


                for (int i = 0; i < tka.TableTableKeysList.Count; i++)
                    txtWhere += tka.TableTableKeysList[i] + ((i + 1 < tka.TableTableKeysList.Count) ? " and " : "");
            }
            catch (Exception ex)
            {
                Messages.Add(new MgkMessage
                {
                    Code = "EX-DEL",
                    Number = MgkResponseCode.DB_EXCEPTION_UNKNOW,
                    Exception = ex.ToString(),
                    Source = this.ToString() + ".DeleteObject",
                    OData = oObject
                });
                MgkLog.Error(Messages.GetLastMessage());
            }
            StrQuery = string.Format("Delete from {0} where {1}", tka.TableName, txtWhere);
            return this.ExecuteNonQuery(StrQuery, tka.ParameterList, System.Data.CommandType.Text);
        }

        public List<T> ReadObjListByKeys<T>(object oObject, String keysForQuery)
        {
            return ReadObjListByKeys<T>(oObject, (keysForQuery == null ? null : (keysForQuery.Split(','))));
        }

        /// <summary>
        /// Recuperar lista de objectList
        /// </summary>
        /// <param name="objeto"></param>
        /// <param name="keysForQuery"></param>
        /// <returns></returns>
        public List<T> ReadObjListByKeys<T>(object oObject, string[] keysForQuery)
        {
            Type type = typeof(T);
            List<T> objectListResult = new List<T>();
            Messages.Clear();
            try
            {
                QueryParametersTable qpt = CreateQptByKeys(oObject, keysForQuery);

                if (MgkStaticMessage.Message.Number < 0)
                    return objectListResult;
                List<object> objectList = ReadDictionaryList(qpt.StrQuery, qpt.ParameterList);
                /*if (objectList.Count > 0)
                    objectList.Remove(objectList[0]);*/
                if (objectList != null && objectList.Count > 0)
                    foreach (Object ob1 in objectList)
                    {
                        Object intance = Activator.CreateInstance(type);
                        DictionaryToObject(ob1, intance);
                        objectListResult.Add((T)intance);
                    }
            }
            catch (Exception ex)
            {
                Messages.Add(new MgkMessage
                {
                    Code = "EX-READ",
                    Message = "Error en operaciones de base de datos con objetos",
                    Messagex = "Error al leer lista de objetos",
                    Number = MgkResponseCode.DB_EXCEPTION_UNKNOW,
                    Exception = ex.ToString(),
                    Source = this.ToString() + ".ReadObjects",
                    OData = objectListResult
                });
                MgkLog.Error(Messages.GetLastMessage());
                // Ocurrio algun Error, se debe resolver...
            }
            return objectListResult;
        }

        /// <summary>
        /// Consulta por valores que tenga asignados el objeto
        /// </summary>
        /// <param name="oObject"></param>
        /// <returns></returns>
        public List<T> ReadObjListByValues<T>(object oObject)
        {
            Type type = typeof(T);
            List<T> objectListResult = new List<T>();
            try
            {
                QueryParametersTable qpt = CreateQptByValues(oObject);
                if (MgkStaticMessage.Message.Number < 0)
                    return null;
                List<object> objectList = ReadDictionaryList(qpt.StrQuery, qpt.ParameterList);
                if (objectList != null && objectList.Count > 0)
                    objectList.Remove(objectList[0]);
                if (objectList != null && objectList.Count > 0)
                    foreach (Object ob1 in objectList)
                    {
                        Object intance = Activator.CreateInstance(type);
                        DictionaryToObject(ob1, intance);
                        objectListResult.Add((T)intance);
                    }
            }
            catch (Exception ex)
            {
                Messages.Add(new MgkMessage
                {
                    Code = "EX-RDLV",
                    Exception = ex.ToString(),
                    Number = MgkResponseCode.SYS_ERROR_UNKNOW,
                    Source = this.ToString() + ".ReadObjListByValues",
                    OData = new
                    {
                        oObject = oObject,
                    }
                });
                MgkLog.Error(Messages.GetLastMessage());
                // Ocurrio algun Error, se debe resolver...
            }
            return objectListResult;
        }

        public List<T> ReadObjListAll<T>(/*object oObject*/)
        {
            return ReadObjListByCondition<T>(/*oObject,*/"");
        }

        /// <summary>
        /// Leer lista de objectList por condition SQL
        /// </summary>
        /// <param name="objeto"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        public List<T> ReadObjListByCondition<T>(
            string condition = ""
            , List<DbParameter> Parameters = null
            , CommandType commandType = CommandType.Text)
        {
            Type type = typeof(T);
            List<T> objectListResult = new List<T>();
            try
            {
                //QueryParametersTable qpt = CreateQptByCondition(objectListResult, condition);
                QueryParametersTable qpt = CreateQptByCondition(Activator.CreateInstance(type), condition);
                List<object> objectList = ReadDictionaryList(qpt.StrQuery, Parameters, false, commandType);
                if (MgkStaticMessage.Message.Number < 0)
                    return null;
                if (objectList != null && objectList.Count > 0)
                    foreach (Object ob1 in objectList)
                    {
                        Object intance = Activator.CreateInstance(type);
                        DictionaryToObject(ob1, intance);
                        objectListResult.Add((T)intance);
                    }
                MgkStaticMessage.Message.Number = objectListResult.Count;
            }
            catch (Exception ex)
            {
                Messages.Add(new MgkMessage
                {
                    Code = "EX-RDLC",
                    Exception = ex.ToString(),
                    Message = "Error en operaciones de base de datos con objetos",
                    Messagex = "Error al leer lista de objetos por condicion",
                    Number = MgkResponseCode.SYS_ERROR_UNKNOW,
                    Source = this.ToString() + ".ReadObjListByCondition",
                    OData = new
                    {
                        oObject = objectListResult,
                    }
                });
                MgkLog.Error(Messages.GetLastMessage());

                // Ocurrio algun Error, se debe resolver...
            }
            return objectListResult;
        }

        public T ReadObjItemByQuery<T>(string StrQuery, List<DbParameter> Parameters = null, CommandType commandType = CommandType.Text)
        {
            List<T> items = ReadObjListByQuery<T>(StrQuery, Parameters, commandType);
            if (items.Count > 0)
            {
                return items[0];
            }
            else
                return default(T);
        }

        /// <summary>
        /// Lista de objectList por consulta SQL
        /// </summary>
        /// <param name="objeto"></param>
        /// <param name="StrQuery"></param>
        /// <returns></returns>
        public List<T> ReadObjListByQuery<T>(string StrQuery, List<DbParameter> Parameters = null, CommandType commandType = CommandType.Text, bool fieldDescriptions = false)
        {
            Messages.ClearMessages();
            Type type = typeof(T);
            List<T> objectListResult = new List<T>();

            try
            {
                List<object> objectList = ReadDictionaryList(StrQuery, Parameters, fieldDescriptions, commandType);
                if (objectList != null && objectList.Count > 0)
                    foreach (Object ob1 in objectList)
                    {
                        Object intance = Activator.CreateInstance(type);
                        DictionaryToObject(ob1, intance);
                        objectListResult.Add((T)intance);
                    }
            }
            catch (Exception ex)
            {
                Messages.Add(new MgkMessage
                {
                    Code = "EX-RDLQ",
                    Exception = ex.ToString(),
                    Number = MgkResponseCode.SYS_ERROR_UNKNOW,
                    Message = "Error en operaciones de base de datos con objetos",
                    Messagex = "Error al leer lista de objetos por query",
                    Source = this.ToString() + ".ReadObjListByQuery",
                    OData = new
                    {
                        oObject = objectListResult,
                    }
                });
                MgkLog.Error(Messages.GetLastMessage());
                // Ocurrio algun Error, se debe resolver...
            }
            return objectListResult;
        }


        /// <summary>
        /// Insert Object Store Procedure
        /// </summary>
        /// <param name="oObject"></param>
        /// <param name="fields"></param>
        /// <param name="IsAutoIncrement"></param>
        /// <returns></returns>
        //public int InsertObject_sp(object oObject, string[] fields = null, bool IsAutoIncrement = false, String Sp_name = "")
        //{
        //    Messages.Clear();
        //    Tka tka = new Tka { IsAutoIncrement = IsAutoIncrement };
        //    List<string> partesSet = new List<string>();
        //    string StrQuery = "";

        //    try
        //    {
        //        Type t = oObject.GetType();
        //        System.Reflection.PropertyInfo[] properties = t.GetProperties();
        //        tka = this.GetTka(oObject, t, "tka", tka, true);

        //        if (tka.TableName == "")
        //        {
        //            Messages.Add(new MgkMessage
        //            {
        //                Number = -100000,
        //                Message = "Error en operaciones de base de datos con objetos",
        //                Messagex = "Error al Insertar registro,El objeto no tiene la propiedad '__table_name' para crear la consulta dinamica",
        //                OData = new
        //                {
        //                    oObject = oObject,
        //                    fields = fields,
        //                    tka = tka,
        //                    PropertiesNonPublic = t.GetProperties(BindingFlags.NonPublic | BindingFlags.Instance)
        //                }
        //            });
        //            MgkLog.Error(Messages.GetLastMessage());
        //        }

        //        foreach (System.Reflection.PropertyInfo p in properties)
        //        {
        //            object PropertyValue = p.GetValue(oObject, null);
        //            if (p.Name[0] == '_')
        //                continue;
        //            if (fields != null)
        //            {
        //                int index = Array.IndexOf(fields, p.Name);
        //                if (index < 0)
        //                    continue;
        //            }

        //            if (PropertyValue != null)
        //            {
        //                var par = CreateParameter("p_" + p.Name, PropertyValue);
        //                if (par == null)
        //                    throw new Exception(string.Format("No fue posible crear Parameter para Insert variable {0} en tabla {1}", p.Name, tka.TableName));
        //                if (!tka.ParameterList.Exists(item => item.ParameterName == p.Name))
        //                    tka.ParameterList.Add(par);
        //                partesSet.Add(string.Format("{0}", p.Name));
        //            }

        //        }

        //        if (Sp_name == "")
        //            Sp_name = "sp_" + tka.TableName + "_ins";
        //        StrQuery = string.Format("{0}", Sp_name);


        //        MgkMessage oMessage = this.ReadObjItemByQuery<MgkMessage>(StrQuery, tka.ParameterList, System.Data.CommandType.StoredProcedure);
        //        if (oMessage != null)
        //            Messages.Add(oMessage);
        //        else
        //            oMessage = Messages.GetLastMessage();

        //        if (tka.IsAutoIncrement)
        //        {
        //            if (tka.TableKeys != null && tka.TableKeys.Length > 0)
        //            {
        //                PropertyInfo propertyInfo = oObject.GetType().GetProperty(tka.TableKeys[0]);
        //                propertyInfo.SetValue(oObject, Convert.ChangeType(oMessage.Number, propertyInfo.PropertyType), null);
        //            }
        //            if (oMessage.Number > 0)
        //            {
        //                this.Messages.Message = "Registro Insertado correctamente " + oMessage.Number;
        //            }
        //        }
        //        return oMessage.Number;
        //    }
        //    catch (Exception ex)
        //    {
        //        Messages.Add(new MgkMessage
        //        {
        //            Code = "EX-INS",
        //            Exception = ex.ToString(),
        //            //Message = "Error en operaciones de base de datos con objetos."+ ex.Message,
        //            Message = "Error al insertar objeto DBOT." + ex.Message,
        //            //Messagex = "Error al Insertar objeto",
        //            Number = MgkResponseCode.SYS_ERROR_UNKNOW,
        //            Source = this.ToString() + ".InsertObject_sp",
        //            OData = new
        //            {
        //                StrQuery = StrQuery,
        //                oObject = oObject,
        //                fields = fields,
        //                tka = tka
        //            }
        //        });
        //        MgkLog.Error(Messages.GetLastMessage());
        //    }
        //    return -1;
        //}

        /// <summary>
        /// Update Object Store Procedure
        /// </summary>
        /// <param name="oObject"></param>
        /// <param name="fields"></param>
        /// <param name="IsAutoIncrement"></param>
        /// <returns></returns>
        //public int UpdateObject_sp(object oObject, string[] fields = null, Boolean UpdateNull = false, String Sp_name = "")
        //{
        //    Messages.Clear();
        //    Tka tka = new Tka();
        //    List<string> partesSet = new List<string>();
        //    var setD = new Dictionary<string, object>();
        //    //string[] TableKeys = null;
        //    string StrQuery = "";
        //    string txtSet = "";
        //    string txtWhere = "";
        //    try
        //    {
        //        Type t = oObject.GetType();
        //        System.Reflection.PropertyInfo[] properties = t.GetProperties();
        //        tka = this.GetTka(oObject, t, "tka");

        //        if (tka.TableName == "")
        //        {
        //            Messages.Add(new MgkMessage
        //            {
        //                Code = "Mgk.Gca.TableName",
        //                Message = "Error en operaciones de base de datos con objetos",
        //                Messagex = "atributo '__table_name' no encontrado en objeto enviado, No se puede construir el query dinamicamente",
        //                Source = "MgkDataBaseObj.UpdateObject",
        //                OData = new
        //                {
        //                    oObject = oObject,
        //                    fields = fields,
        //                    //propertiesControl = propertiesControl
        //                }
        //            });
        //            MgkLog.Error(Messages.GetLastMessage());
        //            return -1;
        //        }

        //        foreach (System.Reflection.PropertyInfo p in properties)
        //        {
        //            object PropertyValue = p.GetValue(oObject, null);
        //            if (p.Name[0] == '_')
        //                continue;
        //            if (fields != null)
        //            {
        //                int index = Array.IndexOf(fields, p.Name);
        //                if (index < 0)
        //                    continue;
        //            }
        //            if (tka.IsAutoIncrement)
        //            {
        //                if (tka.TableKeys != null) // esto sería para no incluir las TableKeys
        //                {
        //                    int index = Array.IndexOf(tka.TableKeys, p.Name);
        //                    if (index >= 0)
        //                    {
        //                        if (!tka.ParameterList.Exists(item => item.ParameterName == p.Name))
        //                            tka.ParameterList.Add(CreateParameter(p.Name, PropertyValue));
        //                        continue;
        //                    }
        //                }
        //            }
        //            if (PropertyValue != null || FieldInArray(fields, p.Name) || UpdateNull)
        //            {
        //                int index = Array.IndexOf(tka.TableKeys, p.Name);
        //                if (index < 0)
        //                    tka.ParameterList.Add(CreateParameter("@p_"+p.Name, PropertyValue));
        //                setD.Add(p.Name, string.Format("{0}=@{1}", p.Name, p.Name));
        //                partesSet.Add(string.Format("{0}=@{1}", p.Name, p.Name));
        //            }

        //        }
        //        for (int i = 0; i < partesSet.Count; i++)
        //            txtSet += partesSet[i] + ((i + 1 < partesSet.Count) ? "," : "");
        //        for (int i = 0; i < tka.TableTableKeysList.Count; i++)
        //            txtWhere += tka.TableTableKeysList[i] + ((i + 1 < tka.TableTableKeysList.Count) ? " and " : "");

        //    }
        //    catch (Exception ex)
        //    {
        //        Messages.Add(new MgkMessage
        //        {
        //            Code = "EX-UPD",
        //            Number = MgkResponseCode.DB_EXCEPTION_UNKNOW,
        //            Exception = ex.ToString(),
        //            Source = "MgkDataBaseObj.UpdateObject",
        //            OData = oObject
        //        });
        //        MgkLog.Error(Messages.GetLastMessage());
        //        // Ocurrio algun Error, se debe resolver...
        //    }
        //    if (Sp_name == "")
        //        Sp_name = "sp_" + tka.TableName + "_upd";
        //    //StrQuery = string.Format("Update {0} set {1} where {2}", tka.TableName, txtSet, txtWhere);
        //    MgkMessage MessageU = new MgkMessage();
        //    try
        //    {
        //        MessageU = this.ReadObjItemByQuery<MgkMessage>(Sp_name, tka.ParameterList, System.Data.CommandType.StoredProcedure);
        //        //MessageU.Message = "Actualización exitosa";
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageU.Number = -1;
        //        MessageU.Message = "Error en actualización del registro";
        //        MessageU.Exception = ex.ToString();
        //        MgkLog.Error(MessageU);
        //        //MessageU.Exception = "";
        //    }

        //    Messages.Add(MessageU);
        //    return MessageU.Number;
        //}

        public String _Create_parameters_insert_v1_nombres(String columnas, String prefix)
        {
            String retVal = "";
            retVal = columnas.Replace(",", ",@" + prefix);
            retVal = $"(@{prefix}{retVal})";
            return retVal;
        }

        public List<DbParameter> _Create_parameters_insert_v1(object oObject, string[] fields = null, bool IsAutoIncrement = false, String idparam = "p_")
        {
            Messages.Clear();
            Tka tka = new Tka { IsAutoIncrement = IsAutoIncrement };
            List<string> partesSet = new List<string>();
            string StrQuery = "";

            try
            {
                Type t = oObject.GetType();
                System.Reflection.PropertyInfo[] properties = t.GetProperties();

                foreach (System.Reflection.PropertyInfo p in properties)
                {
                    object PropertyValue = p.GetValue(oObject, null);
                    if (p.Name[0] == '_')
                        continue;
                    if (fields != null)
                    {
                        int index = Array.IndexOf(fields, p.Name);
                        if (index < 0)
                            continue;
                    }

                    // Asignar DBNull.Value si la propiedad es null
                    if (PropertyValue == null)
                    {
                        PropertyValue = DBNull.Value;
                    }

                    var par = CreateParameter(idparam + p.Name, PropertyValue);
                    if (par == null)
                        throw new Exception(string.Format("No fue posible crear Parameter para Insert variable {0} en tabla {1}", p.Name, tka.TableName));

                    if (!tka.ParameterList.Exists(item => item.ParameterName == p.Name))
                        tka.ParameterList.Add(par);

                    partesSet.Add(string.Format("{0}", p.Name));
                }

                return tka.ParameterList;
            }
            catch (Exception ex)
            {
                Messages.Add(new MgkMessage
                {
                    Code = "EX-INS",
                    Exception = ex.ToString(),
                    Message = "Error al crear parametros objeto DBOT." + ex.Message,
                    Number = MgkResponseCode.SYS_ERROR_UNKNOW,
                    Source = this.ToString() + ".InsertObject_sp",
                    OData = new
                    {
                        StrQuery = StrQuery,
                        oObject = oObject,
                        fields = fields,
                        tka = tka
                    }
                });
                MgkLog.Error(Messages.GetLastMessage());
            }
            return tka.ParameterList;
        }


        /// <summary>
        /// Inserta una lista de objetos
        /// </summary>
        /// <param name="Items">Lista de objetos</param>
        /// <param name="parciales">elementos por insert</param>
        /// <param name="cols">nombres de columnas en el insert</param>
        /// <param name="table_name">Nombre de tabla</param>
        /// <returns></returns>
        public MgkMessage InsertList(object Items, int parciales, String cols = "", String table_name = "")
        {
            int contador = 1;
            int eventos = 0;
            int eventos_error = 0;
            MgkMessage Message = new MgkMessage();
            try
            {
                string parametroidx = "";
                List<DbParameter> ParmeteresTodo = new List<DbParameter>();
                StringBuilder sb = new StringBuilder();
                String query_ins = $"insert into {table_name} ({cols}) values ";

                if (!string.IsNullOrEmpty(table_name))
                    sb.Append(query_ins);

                String linex = _Create_parameters_insert_v1_nombres(cols, "IDX_");

                if (Items is IEnumerable coleccion)
                {
                    foreach (object item1 in coleccion)
                    {
                        if (string.IsNullOrEmpty(table_name))
                        {
                            Type t = item1.GetType();
                            var tka = this.GetTka(item1, t, "tkac");

                            if (string.IsNullOrEmpty(cols) && tka.TableNoKeysList != null && tka.TableNoKeysList.Count > 0)
                                cols = string.Join(",", tka.TableNoKeysList);

                            table_name = tka.TableName;
                            query_ins = $"insert into {table_name} ({cols}) values ";
                            sb.Append(query_ins);
                        }

                        parametroidx = $"p{contador}_";
                        var itemsp = _Create_parameters_insert_v1(item1, null, false, parametroidx);
                        ParmeteresTodo.AddRange(itemsp);
                        string lx = linex.Replace("IDX_", parametroidx);
                        sb.Append(lx);
                        sb.Append(",");
                        if (contador == parciales)
                        {
                            contador = 0;
                            try
                            {
                                eventos++;
                                ExecuteNonQuery(sb.ToString().Substring(0, sb.ToString().Length - 1), ParmeteresTodo, System.Data.CommandType.Text);
                            }
                            catch (Exception e1)
                            {
                                eventos_error++;
                            }
                            ParmeteresTodo.Clear();
                            sb = new StringBuilder();
                            sb.Append(query_ins);
                        }
                        contador++;
                    }
                }

                if (ParmeteresTodo.Count > 0)
                {
                    try
                    {
                        eventos++;
                        ExecuteNonQuery(sb.ToString().Substring(0, sb.ToString().Length - 1), ParmeteresTodo, System.Data.CommandType.Text);
                    }
                    catch (Exception e1)
                    {
                        eventos_error++;
                    }
                }

                if (eventos_error > 0)
                    Message.Number = -eventos_error;
                else
                    Message.Number = eventos;
            }
            catch (Exception ex0)
            {
                Message.Message = "Error desconocido Insert Multiple";
                Message.Exception = ex0.ToString();
                Message.Number = MgkResponseCode.SYS_ERROR_UNKNOW;
                MgkLog.Error(Message);
            }
            return Message;
        }


    }
}