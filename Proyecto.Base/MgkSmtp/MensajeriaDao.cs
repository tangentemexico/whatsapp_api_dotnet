using Mgk.Base.ControlAcceso;
using Mgk.Commonsx;
using Mgk.DataBasex;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mgk.Base.MgkSmtp
{
    public class MensajeriaDao : MgkDataBaseObjT
    {
        public MgkMessage Message { get; set; }

        public MensajeriaDao()
        {
            Message = new MgkMessage();
        }

        public MgkMessage Insert(MensajeriaModel MensajeriaMo, AccesoModel AccesoM)
        {
            Messages.Clear();
            try
            {
                if (AccesoM == null)
                    AccesoM = new AccesoModel { Usuario_id = "system" };
                MensajeriaMo.Fecha_inserta = DateTime.Now;
                MensajeriaMo.Usuario_id_inserta = AccesoM.Usuario_id;
                MensajeriaMo.Remitente = AccesoM.Usuario_id??"";
                    int iRetVal = this.InsertObject(MensajeriaMo);

                if (iRetVal >= 0)
                {
                    Messages.Message = "Registro Insertado exitosamente";
                    Messages.OData = MensajeriaMo;
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
                    OData = new { MensajeriaMo = MensajeriaMo }
                });
                MgkLog.Error(Messages.GetLastMessage());
            }
            return Messages;
        }

        public MgkMessage Update(MensajeriaModel MensajeriaMo, AccesoModel AccesoM)
        {
            Messages.Clear();
            try
            {
                this.UpdateObject(MensajeriaMo);
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
                    OData = new { MensajeriaMo = MensajeriaMo }
                });
                MgkLog.Error(Messages.GetLastMessage());
            }

            return Messages.GetLastMessage();
        }


        public MgkMessage Delete(MensajeriaModel MensajeriaMo, AccesoModel AccesoM)
        {
            Messages.Clear();
            try
            {
                this.DeleteObject(MensajeriaMo);
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
                    OData = new { MensajeriaMo = MensajeriaMo }
                });
                MgkLog.Error(Messages.GetLastMessage());
            }

            return Messages.GetLastMessage();
        }


        public MensajeriaModel GetItem(MensajeriaModel MensajeriaMo, AccesoModel AccesoM)
        {
            Messages.Clear();
            MensajeriaModel MensajeriaItem = null;
            try
            {
                MensajeriaItem = this.ReadNew<MensajeriaModel>(MensajeriaMo);              
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
                    //OData = new { MensajeriaMo = MensajeriaMo, QueryB = QueryB }
                });
                MgkLog.Error(Messages.GetLastMessage());
            }

            return MensajeriaItem;
        }

        public List<MensajeriaModel> GetItems(MensajeriaModel MensajeriaMo, AccesoModel AccesoM)
        {
            Messages.Clear();
            MgkQueryBuilder QueryB = new MgkQueryBuilder();
            if (MensajeriaMo == null)
                MensajeriaMo = new MensajeriaModel();

            try
            {
                List<DbParameter> Parameters = new List<DbParameter>();
                QueryB.SetQueryBase("select * from " + MensajeriaModel.__table_name);
                if ((MensajeriaMo.Remitente ?? "") != "")
                {
                    Parameters.Add((DbParameter)GetParameter("@Remitente", MensajeriaMo.Remitente));
                    QueryB.AddAnd("Remitente=@Remitente");
                }
                if ((MensajeriaMo.Destinatario ?? "") != "")
                {
                    Parameters.Add((DbParameter)GetParameter("@Destinatario", MensajeriaMo.Destinatario));
                    QueryB.AddAnd("Destinatario=@Destinatario");
                }


                var Itemsd = this.ReadObjListByQuery<MensajeriaModel>(QueryB.GetQuery(), Parameters);
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
                    OData = new { MensajeriaMo = MensajeriaMo, QueryB = QueryB }
                });
                MgkLog.Error(Messages.GetLastMessage());
            }

            Messages.Number = 0;
            return null;
        }

    }
}