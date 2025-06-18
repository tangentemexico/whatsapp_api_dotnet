using Mgk.Commonsx;
using Mgk.DataBasex;
using Mgk.Base.ControlAcceso;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mgk.Commonsx;

namespace Mgk.Base.HistoriaTabla
{
    public class HistoriaTablaDao : MgkDataBaseObjT
    {
        public MgkMessage Message { get; set; }

        public HistoriaTablaDao()
        {
            Message = new MgkMessage();
        }

        public MgkMessage Insert(HistoriaTablaModel HistoriaTablaMo, AccesoModel AccesoM)
        {
            Message.Clear();
            try
            {
                this.InsertObject(HistoriaTablaMo);
                int iRetVal = HistoriaTablaMo.Historia_tabla_id;

                if (iRetVal > 0)
                {
                    HistoriaTablaMo.Historia_tabla_id = iRetVal;
                    Message.Number = HistoriaTablaMo.Historia_tabla_id;
                    Message.Message = "Registro Insertado exitosamente";
                    Message.OData = HistoriaTablaMo;
                }
                else
                {
                    throw new Exception("No fue posible Insertar el registro");
                }
            }
            catch (Exception ex)
            {
                Message = new MgkMessage
                {
                    Number = -1,
                    Code = "ex-ins",
                    Message = "Error al Insertar registro",
                    Source = this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name,
                    Exception = ex.ToString(),
                    OData = new { HistoriaTablaMo = HistoriaTablaMo }
                };
                MgkLog.Error(Message);
            }
            
            return Message;
        }

        public MgkMessage Update(HistoriaTablaModel HistoriaTablaMo, AccesoModel AccesoM)
        {
            Message.Clear();
            try
            {
                this.UpdateObject(HistoriaTablaMo);
                Message = this.Messages.GetLastMessage();
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
                    OData = new { HistoriaTablaMo = HistoriaTablaMo }
                };
                MgkLog.Error(Message);
            }
            
            return Message;
        }


        public MgkMessage Delete(HistoriaTablaModel HistoriaTablaMo, AccesoModel AccesoM)
        {
            Message.Clear();
            try
            {
                this.DeleteObject(HistoriaTablaMo);
                Message = this.Messages.GetLastMessage();
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
                    OData = new { HistoriaTablaMo = HistoriaTablaMo }
                };
                MgkLog.Error(Message);
            }
            
            return Message;
        }


        public HistoriaTablaModel GetItem(HistoriaTablaModel HistoriaTablaMo, AccesoModel AccesoM)
        {
            Message.Clear();
            HistoriaTablaModel HistoriaTablaItem = null;
            try
            {
                HistoriaTablaItem = this.ReadObject<HistoriaTablaModel>(HistoriaTablaMo, null, true);

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
                    //OData = new { HistoriaTablaMo = HistoriaTablaMo, QueryB = QueryB }
                };
                MgkLog.Error(Message);
            }
            
            return HistoriaTablaItem;
        }

        public List<HistoriaTablaModel> GetItems(HistoriaTablaModel HistoriaTablaMo, AccesoModel AccesoM)
        {
            Message.Clear();
            MgkQueryBuilder QueryB = new MgkQueryBuilder();

            try
            {
                #region Paso de Parametros
                List<DbParameter> Parameters = new List<DbParameter>();
                QueryB.SetQueryBase("select * from sitio");
                if (HistoriaTablaMo.Historia_tabla_id > 0)
                {
                    Parameters.Add((DbParameter)GetParameter("@Historia_tabla_id", HistoriaTablaMo.Historia_tabla_id));
                    QueryB.AddAnd("Historia_tabla_id=@Historia_tabla_id");
                }

                if (HistoriaTablaMo.Nombre != null)
                {
                    Parameters.Add((DbParameter)GetParameter("@Nombre", HistoriaTablaMo.Nombre));
                    QueryB.AddAnd("Nombre=@Nombre");
                }

                #endregion

                var Itemsd = this.ReadObjListByQuery<HistoriaTablaModel>(QueryB.GetQuery(), Parameters);
                Message.Number = Itemsd.Count;
                
                return Itemsd;
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
                    OData = new { HistoriaTablaMo = HistoriaTablaMo, QueryB = QueryB }
                };
                MgkLog.Error(Message);
            }
            
            Message.Number = 0;
            return null;
        }
    }
}