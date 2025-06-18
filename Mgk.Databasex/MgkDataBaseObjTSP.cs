using Mgk.Commonsx;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Reflection;
using static Mgk.DataBasex.MgkDataBaseObjT;

namespace Mgk.DataBasex
{
    /// <summary>
    /// Operaciones basicas de base de datos con Objetos. Usando Stored Procedures
    /// Insertar, Actualizar, Buscar, Leer, Borrar
    /// 
    /// </summary>
    public class MgkDataBaseObjTSP : MgkDataBaseObjT
    {
        public MgkDataBaseObjTSP(String ConnectionStr = null, DataBaseEngineEnum DbEngine = DataBaseEngineEnum.No, String PrefixConnectionStr = null) : base(ConnectionStr, DbEngine, PrefixConnectionStr)
        {

        }

        /// <summary>
        /// Insert Object Store Procedure
        /// </summary>
        /// <param name="oObject"></param>
        /// <param name="fields"></param>
        /// <param name="IsAutoIncrement"></param>
        /// <returns></returns>
        public int InsertObject(object oObject, string[] fields = null, bool IsAutoIncrement = false, String Sp_name = "")
        {
            Messages.Clear();
            Tka tka = new Tka { IsAutoIncrement = IsAutoIncrement };
            List<string> partesSet = new List<string>();
            string StrQuery = "";

            try
            {
                Type t = oObject.GetType();
                System.Reflection.PropertyInfo[] properties = t.GetProperties();
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

                    if (PropertyValue != null)
                    {
                        var par = CreateParameter("p_" + p.Name, PropertyValue);
                        if (par == null)
                            throw new Exception(string.Format("No fue posible crear Parameter para Insert variable {0} en tabla {1}", p.Name, tka.TableName));
                        if (!tka.ParameterList.Exists(item => item.ParameterName == p.Name))
                            tka.ParameterList.Add(par);
                        partesSet.Add(string.Format("{0}", p.Name));
                    }

                }

                if (Sp_name == "")
                    Sp_name = "sp_" + tka.TableName + "_ins";
                StrQuery = string.Format("{0}", Sp_name);


                MgkMessage oMessage = this.ReadObjItemByQuery<MgkMessage>(StrQuery, tka.ParameterList, System.Data.CommandType.StoredProcedure);
                if (oMessage != null)
                    Messages.Add(oMessage);
                else
                    oMessage = Messages.GetLastMessage();

                if (tka.IsAutoIncrement)
                {
                    if (tka.TableKeys != null && tka.TableKeys.Length > 0)
                    {
                        PropertyInfo propertyInfo = oObject.GetType().GetProperty(tka.TableKeys[0]);
                        propertyInfo.SetValue(oObject, Convert.ChangeType(oMessage.Number, propertyInfo.PropertyType), null);
                    }
                    if (oMessage.Number > 0)
                    {
                        this.Messages.Message = "Registro Insertado correctamente " + oMessage.Number;
                    }
                }
                return oMessage.Number;
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
            return -1;
        }

        public int UpdateObject(object oObject, string fields = null, Boolean UpdateNull = false, String Sp_name = "") {
            return UpdateObject(oObject, (fields == null) ? null : (fields.Split(',')), UpdateNull, Sp_name);
        }

        /// <summary>
        /// Update Object Store Procedure
        /// </summary>
        /// <param name="oObject"></param>
        /// <param name="fields"></param>
        /// <param name="IsAutoIncrement"></param>
        /// <returns></returns>
        public int UpdateObject(object oObject, string[] fields = null, Boolean UpdateNull = false, String Sp_name = "")
        {
            Messages.Clear();
            Tka tka = new Tka();
            List<string> partesSet = new List<string>();
            var setD = new Dictionary<string, object>();
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
                            tka.ParameterList.Add(CreateParameter("@p_"+p.Name, PropertyValue));
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
            if (Sp_name == "")
                Sp_name = "sp_" + tka.TableName + "_upd";
            MgkMessage MessageU = new MgkMessage();
            try
            {
                MessageU = this.ReadObjItemByQuery<MgkMessage>(Sp_name, tka.ParameterList, System.Data.CommandType.StoredProcedure);
                if (MessageU == null)
                    MessageU = this.Messages.GetLastMessage();
            }
            catch (Exception ex)
            {
                MessageU.Number = -1;
                MessageU.Message = "Error en actualización del registro";
                MessageU.Exception = ex.ToString();
                MgkLog.Error(MessageU);
            }

            Messages.Add(MessageU);
            return MessageU.Number;
        }


        public int DeleteObject(object oObject,String Sp_name="")
        {
            Messages.Clear();
            Tka tka = new Tka();
            //string[] TableKeys = null;

            List<string> partesSet = new List<string>();
            var setD = new Dictionary<string, object>();
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
            if (Sp_name == "")
                Sp_name = "sp_" + tka.TableName + "_del";
            MgkMessage MessageU = new MgkMessage();
            try
            {
                MessageU = this.ReadObjItemByQuery<MgkMessage>(Sp_name, tka.ParameterList, System.Data.CommandType.StoredProcedure);
                if (MessageU == null)
                    MessageU = this.Messages.GetLastMessage();
            }
            catch (Exception ex)
            {
                MessageU.Number = -21000;
                MessageU.Message = "Error en actualización del registro";
                MessageU.Exception = ex.ToString();
                MgkLog.Error(MessageU);
            }
            return MessageU.Number;
        }


        public T ReadObject<T>(object oObject, string[] keysForQuery = null, bool newObject = false,String Sp_name="")
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
                }
                if (Sp_name == "")
                    Sp_name = "sp_" + qpt.TableName + "_get";
                // qpt.StrQuery
                Dictionary<string, object> Obj = (Dictionary<string, object>)ReadDictionaryItem(Sp_name, qpt.ParameterList,CommandType.StoredProcedure);
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
            }
            return newInstance;
        }





    }
}