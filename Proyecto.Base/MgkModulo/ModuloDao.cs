using Mgk.Commonsx;
using Mgk.DataBasex;
using Mgk.Base.ControlAcceso;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mgk.Base.Modulo
{
    public class ModuloDao : MgkDataBaseObjT
    {
        public MgkMessage Message { get; set; }

        public ModuloDao()
        {
            Message = new MgkMessage();
        }

        public MgkMessage Insert(ModuloModel ModuloMo, AccesoModel AccesoM)
        {
            Message.Clear();
            try
            {
                if (ModuloMo.Es_activo == null)
                    ModuloMo.Es_activo = false;
                int iRetVal = this.InsertObject(ModuloMo);

                if (iRetVal >= 0)
                {
                    Message.Message = "Registro Insertado exitosamente";
                    Message.OData = ModuloMo;
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
                    OData = new { ModuloMo = ModuloMo }
                };
                MgkLog.Error(Message);
            }
            return Message;
        }

        public MgkMessage Update(ModuloModel ModuloMo, AccesoModel AccesoM)
        {
            Message.Clear();
            try
            {
                this.UpdateObject(ModuloMo);
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
                    OData = new { ModuloMo = ModuloMo }
                };
                MgkLog.Error(Message);
            }
            return Message;
        }


        public MgkMessage Delete(ModuloModel ModuloMo, AccesoModel AccesoM)
        {
            Message.Clear();
            try
            {
                this.DeleteObject(ModuloMo);
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
                    OData = new { ModuloMo = ModuloMo }
                };
                MgkLog.Error(Message);
            }
            return Message;
        }


        public ModuloModel GetItem(ModuloModel ModuloMo, AccesoModel AccesoM)
        {
            Message.Clear();
            ModuloModel ModuloItem = null;
            try
            {
                ModuloItem = this.ReadObject<ModuloModel>(ModuloMo, null, true);

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
                    //OData = new { ModuloMo = ModuloMo, QueryB = QueryB }
                };
                MgkLog.Error(Message);
            }
            return ModuloItem;
        }

        public List<ModuloModel> GetItems(ModuloModel ModuloMo, AccesoModel AccesoM)
        {
            Message.Clear();
            MgkQueryBuilder QueryB = new MgkQueryBuilder();

            try
            {
                #region Paso de Parametros
                List<DbParameter> Parameters = new List<DbParameter>();
                QueryB.SetQueryBase("select * from "+ ModuloModel.__table_name);

                if ((ModuloMo.Modulo_cod ?? "") != "")
                {
                    Parameters.Add((DbParameter)GetParameter("@Modulo_cod", ModuloMo.Modulo_cod));
                    QueryB.AddAnd("Modulo_cod=@Modulo_cod");
                }

                if (ModuloMo.Es_activo != null)
                {
                    Parameters.Add((DbParameter)GetParameter("@Es_activo", ModuloMo.Es_activo));
                    QueryB.AddAnd("Es_activo=@Es_activo");
                }
                if ((ModuloMo.Nombre??"") != "")
                {
                    Parameters.Add((DbParameter)GetParameter("@Nombre", ModuloMo.Nombre));
                    QueryB.AddAnd("Nombre=@Nombre");
                }
               

                #endregion

                var Itemsd = this.ReadObjListByQuery<ModuloModel>(QueryB.GetQuery(), Parameters);
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
                    OData = new { ModuloMo = ModuloMo, QueryB = QueryB }
                };
                MgkLog.Error(Message);
            }
            Message.Number = 0;
            return null;
        }

    }
}