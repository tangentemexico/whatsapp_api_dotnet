using Mgk.Commonsx;
using Mgk.DataBasex;
using Mgk.Base.ControlAcceso;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mgk.Base.Parametro
{
    public class ParametroDao : MgkDataBaseObjT
    {
        //public MgkMessage Message { get; set; }

        public ParametroDao()
        {
            //Message = new MgkMessage();
        }

        public MgkMessage Insert(ParametroModel ParametroMo, AccesoModel AccesoM)
        {
            Messages.Clear();
            try
            {
                int iRetVal = this.InsertObject(ParametroMo);

                if (iRetVal >= 0)
                {
                    Messages.Message = "Registro Insertado exitosamente";
                    Messages.OData = ParametroMo;
                }
                else
                {
                    throw new Exception("No fue posible Insertar el registro");
                }
            }
            catch (Exception ex)
            {
                Messages.Add(new MgkMessage
                {
                    Number = -1,
                    Code = "ex-ins",
                    Message = "Error al Insertar registro",
                    Source = this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name,
                    Exception = ex.ToString(),
                    OData = new { ParametroMo = ParametroMo }
                });
                MgkLog.Error(Messages.GetLastMessage());
            }            
            return Messages;
        }

        public MgkMessage UpdateValor(ParametroModel ParametroMo)
        {
            Messages.Clear();
            try
            {
                this.UpdateObject(ParametroMo,new string[] { "Valor" });
            }
            catch (Exception ex)
            {
                Messages.Add(new MgkMessage
                {
                    Number = -2,
                    Code = "ex-upd",
                    Message = "Error al actualizar registro",
                    Source = this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name,
                    Exception = ex.ToString(),
                    OData = new { ParametroMo = ParametroMo }
                });
                MgkLog.Error(Messages.GetLastMessage());
            }

            return Messages.GetLastMessage();
        }

        public MgkMessage Update(ParametroModel ParametroMo, AccesoModel AccesoM)
        {
            Messages.Clear();
            try
            {
                this.UpdateObject(ParametroMo);
                //Message = this.Messages.GetLastMessage();
            }
            catch (Exception ex)
            {
                Messages.Add(new MgkMessage
                {
                    Number = -2,
                    Code = "ex-upd",
                    Message = "Error al actualizar registro",
                    Source = this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name,
                    Exception = ex.ToString(),
                    OData = new { ParametroMo = ParametroMo }
                });
                MgkLog.Error(Messages.GetLastMessage());
            }
            
            return Messages.GetLastMessage();
        }


        public MgkMessage Delete(ParametroModel ParametroMo, AccesoModel AccesoM)
        {
            Messages.Clear();
            try
            {
                this.DeleteObject(ParametroMo);
                //Message = this.Messages.GetLastMessage();
            }
            catch (Exception ex)
            {
                Messages.Add(new MgkMessage
                {
                    Number = -3,
                    Code = "ex-upd",
                    Message = "Error al actualizar registro",
                    Source = this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name,
                    Exception = ex.ToString(),
                    OData = new { ParametroMo = ParametroMo }
                });
                MgkLog.Error(Messages.GetLastMessage());
            }
            
            return Messages.GetLastMessage();
        }


        public ParametroModel GetItem(ParametroModel ParametroMo, AccesoModel AccesoM)
        {
            Messages.Clear();
            ParametroModel ParametroItem = null;
            try
            {
                ParametroItem = this.ReadObject<ParametroModel>(ParametroMo, null, true);
            }
            catch (Exception ex)
            {
                Messages.Add(new MgkMessage
                {
                    Number = -4,
                    Code = "ex-sel4",
                    Message = "Error en consulta del registro",
                    Source = this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name,
                    Exception = ex.ToString(),
                    //OData = new { ParametroMo = ParametroMo, QueryB = QueryB }
                });
                MgkLog.Error(Messages.GetLastMessage());
            }
            
            return ParametroItem;
        }

        public List<ParametroModel> GetItems(ParametroModel ParametroMo, AccesoModel AccesoM)
        {
            Messages.Clear();
            MgkQueryBuilder QueryB = new MgkQueryBuilder();
            if (ParametroMo == null)
                ParametroMo = new ParametroModel();

            try
            {
                #region Paso de Parametros
                List<DbParameter> Parameters = new List<DbParameter>();
                QueryB.SetQueryBase("select * from "+ ParametroModel.__table_name);
                if (!string.IsNullOrEmpty(ParametroMo.Parametro_id))
                {
                    Parameters.Add((DbParameter)GetParameter("@Parametro_id", ParametroMo.Parametro_id));
                    QueryB.AddAnd("Parametro_id=@Parametro_id");
                }

                if (!string.IsNullOrEmpty(ParametroMo._clave))
                {
//                    Parameters.Add((DbParameter)GetParameter("@Parametro_id", ParametroMo.Parametro_id));
                    QueryB.AddAnd(string.Format("Parametro_id like '%{0}%'", ParametroMo._clave));
                }

                #endregion

                var Itemsd = this.ReadObjListByQuery<ParametroModel>(QueryB.GetQuery(), Parameters);
                Messages.Number = Itemsd.Count;
                
                return Itemsd;
            }
            catch (Exception ex)
            {
                Messages.Add(new MgkMessage
                {
                    Number = -5,
                    Code = "ex-sel5",
                    Message = "Error en consulta de lista",
                    Source = this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name,
                    Exception = ex.ToString(),
                    OData = new { ParametroMo = ParametroMo, QueryB = QueryB }
                });
                MgkLog.Error(Messages.GetLastMessage());
            }
            
            Messages.Number = 0;
            return null;
        }

    }
}