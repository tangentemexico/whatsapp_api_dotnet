using Mgk.Base.Usuario;
using Mgk.Commonsx;
using Mgk.DataBasex;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mgk.Base.ControlAcceso
{
    public class AccesoDao : MgkDataBaseObjT
    {
        public MgkMessage Message { get; set; }
        public AccesoDao()
        {
            Message = new MgkMessage();
        }

        private AccesoModel SetObject(DbDataReader DataR)
        {
            AccesoModel AccesoItem = new AccesoModel();
            AccesoItem.Acceso_id = MgkFunctions.StrToInt(DataR["Acceso_id"].ToString());
            AccesoItem.Usuario_id = DataR["Usuario_id"].ToString();
            AccesoItem.Fecha_inicio = MgkFunctions.StrToDateTime(DataR["Fecha_inicio"].ToString());
            AccesoItem.Fecha_actualiza = MgkFunctions.StrToDateTime(DataR["Fecha_actualiza"].ToString());
            AccesoItem.Fecha_fin = MgkFunctions.StrToDateTime(DataR["Fecha_fin"].ToString());
            AccesoItem.Origen = DataR["Origen"].ToString();
            //AccesoItem._User_name = DataR["_User_name"].ToString();
            return AccesoItem;
        }

        private List<DbParameter> SetParameteres(AccesoModel AccesoM)
        {
            List<DbParameter> Parameters = new List<DbParameter>();
            if (AccesoM.Acceso_id > 0)
                Parameters.Add((DbParameter)GetParameter("@Acceso_id", AccesoM.Acceso_id));
            Parameters.Add((DbParameter)GetParameter("@Usuario_id", AccesoM.Usuario_id));
            if (AccesoM.Fecha_inicio != null)
                Parameters.Add((DbParameter)GetParameter("@Fecha_inicio", AccesoM.Fecha_inicio));
            Parameters.Add((DbParameter)GetParameter("@Fecha_actualiza", AccesoM.Fecha_actualiza));
            if (AccesoM.Fecha_fin != null)
                Parameters.Add((DbParameter)GetParameter("@Fecha_fin", AccesoM.Fecha_fin.Value));
            if (AccesoM.Origen != null)
                Parameters.Add((DbParameter)GetParameter("@Origen", AccesoM.Origen));
            return Parameters;
        }

        public MgkMessage Insert(AccesoModel AccesoM, int Acceso_id, UsuarioModel UsuarioMo = null)
        {
            Message.Clear();
            AccesoM.Fecha_inicio = DateTime.Now;
            AccesoM.Fecha_actualiza = AccesoM.Fecha_inicio;
            AccesoM.Fecha_fin = AccesoM.Fecha_inicio;
            base.Insert(AccesoM);
            if (AccesoM.Acceso_id > 0)
            {
                AccesoM._UsuarioMo = UsuarioMo;
                Message.Number = AccesoM.Acceso_id;
                Message.Message = "Registro Insertado exitosamente";
                Message.OData = AccesoM;

                InsertSessionAcceso(AccesoM);
            }
            else
            {
                throw new Exception("No fue posible Insertar el registro");
            }
            //this.ConnectionClose();
            return Message;
        }

        public void InsertSessionAcceso(AccesoModel AccesoM)
        {
            MgkSessionAccesoModel sesaMod = new MgkSessionAccesoModel();
            sesaMod.Acceso_id = AccesoM.Acceso_id;
            sesaMod.Usuario_id = AccesoM.Usuario_id;
            sesaMod.Front = AccesoM._UsuarioMo._Aux_front;
            sesaMod.Session_json = Newtonsoft.Json.JsonConvert.SerializeObject(AccesoM);

            //base.ExecuteNonQuery( string.Format("delete from {0} where Usuario_id='{1}' and Front='{2}'", 
            //    MgkSessionAccesoModel.__table_name, 
            //    AccesoM.Usuario_id ,
            //    sesaMod.Front), null, CommandType.Text);

            base.Insert(sesaMod);
        }

        public AccesoModel LeerSessionAcceso(int Acceso_id)
        {
            MgkSessionAccesoModel sesaMod = new MgkSessionAccesoModel();
            sesaMod.Acceso_id = Acceso_id;
            ReadObject<MgkSessionAccesoModel>(sesaMod);
            if (sesaMod != null && sesaMod.Session_json!=null)
            {
                AccesoModel AccesoM = Newtonsoft.Json.JsonConvert.DeserializeObject<AccesoModel>(sesaMod.Session_json);
                return AccesoM;
            }
            return null;
        }

        public MgkMessage Update(AccesoModel AccesoM, int Acceso_id)
        {
            Message.Clear();
            GetConnection();
            AccesoM.Fecha_actualiza = DateTime.Now;
            AccesoM.Fecha_fin = AccesoM.Fecha_actualiza;

            try
            {
                #region Paso de Parametros
                List<DbParameter> Parameters = SetParameteres(AccesoM);
                #endregion

                ExecuteNonQuery("Update Acceso set Fecha_fin=@Fecha_fin,Fecha_actualiza=@Fecha_actualiza where Acceso_id=@Acceso_id",
                    Parameters, CommandType.Text);
                if (Messages.Number >= 0)
                {
                    Message.Number = AccesoM.Acceso_id;
                    Message.Message = "Registro actualizado exitosamente";
                }
                else
                {
                    throw new Exception("No fue posible actualizar el registro");
                }
            }
            catch (Exception ex)
            {
                Message = new MgkMessage
                {
                    Number = -2,
                    Code = "ex-upd",
                    Message = "Error al actualizar registro",
                    Source = this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name,
                    Exception = ex.ToString(),
                    OData = new { AccesoM = AccesoM }
                };
                MgkLog.Error(Message);
            }
            this.ConnectionClose();
            return Message;
        }

        public MgkMessage Delete(AccesoModel AccesoM, int Acceso_id)
        {
            Message.Clear();
            GetConnection();
            try
            {
                #region Paso de Parametros
                List<DbParameter> Parameters = new List<DbParameter>();
                Parameters.Add((DbParameter)GetParameter("@Acceso_id", AccesoM.Acceso_id));
                #endregion

                ExecuteNonQuery("Delete from "+ AccesoModel.__table_name + " where Acceso_id=@Acceso_id",
                    Parameters, CommandType.Text);
                if (Messages.Number >= 0)
                {
                    Message.Number = AccesoM.Acceso_id;
                    Message.Message = "Registro actualizado exitosamente";
                }
                else
                {
                    throw new Exception("No fue posible actualizar el registro");
                }
            }
            catch (Exception ex)
            {
                Message = new MgkMessage
                {
                    Number = -3,
                    Code = "ex-upd",
                    Message = "Error al actualizar registro",
                    Source = this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name,
                    Exception = ex.ToString(),
                    OData = new { AccesoM = AccesoM }
                };
                MgkLog.Error(Message);
            }
            this.ConnectionClose();
            return Message;
        }

        public AccesoModel GetItem(AccesoModel AccesoM, int Acceso_id)
        {
            Message.Clear();
            MgkQueryBuilder QueryB = new MgkQueryBuilder();
            AccesoModel AccesoItem = null;
            try
            {
                #region Paso de Parametros
                QueryB.SetQueryBase("select ha.*,se.Session_json as _Session_json from "
                    + AccesoModel.__table_name + " as ha join " + Usuario.UsuarioModel.__table_name + " as us on us.Usuario_id = ha.Usuario_id and us.Es_activo='1' " +
                    " join _session as se on se.Acceso_id = ha.Acceso_id ");
                QueryB.AddAnd("ha.Acceso_id=" + AccesoM.Acceso_id);
                #endregion

                AccesoItem = this.ReadObjItemByQuery<AccesoModel>( QueryB.GetQuery());
                if (AccesoItem != null)
                {
                    Message.Number = AccesoItem.Acceso_id;
                    AccesoItem = Newtonsoft.Json.JsonConvert.DeserializeObject<AccesoModel>(AccesoItem._Session_json);
                }

            }
            catch (Exception ex)
            {
                Message = new MgkMessage
                {
                    Number = -4,
                    Code = "ex-sel4",
                    Message = "Error en consulta del registro",
                    Source = this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name,
                    Exception = ex.ToString(),
                    OData = new { AccesoM = AccesoM, QueryB = QueryB }
                };
                MgkLog.Error(Message);
            }
            this.ConnectionClose();
            return AccesoItem;
        }


        public List<AccesoModel> GetItems(AccesoModel AccesoM, int Acceso_id)
        {
            Message.Clear();
            GetConnection();
            DbDataReader DataR = null;
            List<AccesoModel> AccesoLista = new List<AccesoModel>();
            MgkQueryBuilder QueryB = new MgkQueryBuilder();

            try
            {
                #region Paso de Parametros
                List<DbParameter> Parameters = new List<DbParameter>();
                QueryB.SetQueryBase("select * from "+ AccesoModel.__table_name);
                if (AccesoM.Acceso_id > 0)
                {
                    Parameters.Add((DbParameter)GetParameter("@Acceso_id", AccesoM.Acceso_id));
                    QueryB.AddAnd("Acceso_id=@Acceso_id");
                }
                if (AccesoM.Usuario_id != null)
                {
                    Parameters.Add((DbParameter)GetParameter("@Usuario_id", AccesoM.Usuario_id));
                    QueryB.AddAnd("Usuario_id=@Usuario_id");
                }
                if (AccesoM.Fecha_inicio != null && AccesoM.Fecha_fin != null)
                {
                    Parameters.Add((DbParameter)GetParameter("@Fecha_inicio", AccesoM.Fecha_inicio));
                    Parameters.Add((DbParameter)GetParameter("@Fecha_fin", AccesoM.Fecha_fin));
                    QueryB.AddAnd("cast(Fecha_inicio as Date) between @Fecha_inicio and @Fecha_fin");
                }

                #endregion
                String Query = QueryB.GetQuery();
                DataR = this.GetDataReader(Query,
                    Parameters, CommandType.Text);
                while (DataR.HasRows && DataR.Read())
                    AccesoLista.Add(SetObject(DataR));

            }
            catch (Exception ex)
            {
                Message = new MgkMessage
                {
                    Number = -5,
                    Code = "ex-sel5",
                    Message = "Error en consulta de lista",
                    Source = this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name,
                    Exception = ex.ToString(),
                    OData = new { AccesoM = AccesoM, QueryB = QueryB }
                };
                MgkLog.Error(Message);
            }
            this.ConnectionClose();
            Message.Number = AccesoLista.Count();
            return AccesoLista;
        }

        public void UpdateUltimoAcceso(AccesoModel AccesoM)
        {
            try
            {
                AccesoM.Fecha_actualiza = DateTime.Now;
                UpdateObject(AccesoM, new string[] { "Fecha_actualiza" });
            }
            catch (Exception ex)
            {
                Message = new MgkMessage
                {
                    Number = -3,
                    Code = "ex-upd",
                    Message = "Error al actualizar registro",
                    Source = this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name,
                    Exception = ex.ToString(),
                    OData = new { AccesoM = AccesoM }
                };
                MgkLog.Error(Message);
            }
        }
        public void UpdateSalida(AccesoModel AccesoM)
        {
            try
            {
                AccesoM.Fecha_fin = DateTime.Now;
                UpdateObject(AccesoM, new string[] { "Fecha_fin" });
            }
            catch (Exception ex)
            {
                Message = new MgkMessage
                {
                    Number = -3,
                    Code = "ex-upd",
                    Message = "Error al actualizar registro",
                    Source = this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name,
                    Exception = ex.ToString(),
                    OData = new { AccesoM = AccesoM }
                };
                MgkLog.Error(Message);
            }
        }
    }
}
