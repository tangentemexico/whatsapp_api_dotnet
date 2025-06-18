using Mgk.Base.Parametro;
using Mgk.Base.Usuario;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Common;
using Mgk.Commonsx;

namespace Mgk.Base.ControlAcceso
{
    public class ControlAccesoCtrl
    {
        //protected AccesoDao AccesoD { get; set; }
        public AccesoModel AccesoMo { get; set; }

        public MgkMessage Message { get; set; }

        public ControlAccesoCtrl()
        {
            Message = new MgkMessage();
        }

        public static AccesoModel GetAccesoInvitado()
        {
            UsuarioModel UsuarioMod = new UsuarioModel
            {
                Usuario_id = "_invitado_",
                _MenuList = new List<Menu.MenuModel>(),
                _ModuloList = new List<Modulo.ModuloModel>(),
                _GrupouList = new List<Grupou.GrupouModel>()
            };
            UsuarioMod._GrupouList.Add(new Grupou.GrupouModel { Grupou_code="ADM",Es_activo=true});
            AccesoModel AccesoMod = new AccesoModel
            {
                Usuario_id = UsuarioMod.Usuario_id,
                _UsuarioMo = UsuarioMod,
            };
            return AccesoMod;
        }

        /// <summary>
        /// Validar acceso usuario
        /// </summary>
        /// <param name="UsuarioM"></param>
        /// <returns></returns>
        public MgkMessage Login(UsuarioModel UsuarioM, String Origen = "", String Did = "", Boolean bSoloUsuario = false, String OsName="")
        {
            if (Message == null)
                Message = new MgkMessage();
            AccesoDao AccesoD = null;
            if (AccesoD == null)
                AccesoD = new AccesoDao();

            if (UsuarioM == null)
            {
                Message = new MgkMessage
                {
                    Number = -100,
                    Title = "Ops!",
                    Message = "Error de acceso. Falta definir datos"
                };
                return Message;
            }

            String Password = UsuarioM.Password;

            UsuarioCtrl UsuarioC = new UsuarioCtrl();
            MgkMessage Msg = UsuarioC.GetForLogin(UsuarioM, Did, bSoloUsuario );


            if (string.IsNullOrEmpty(UsuarioM.Usuario_id) == false)
            {
                AccesoMo = new AccesoModel
                {
                    Usuario_id = UsuarioM.Usuario_id,
                    Origen = Origen,
                    Did = Did,
                    OsName = OsName,
                };

                AccesoMo.Auxx = UsuarioM._Aux_menu;
                Message = AccesoD.Insert(AccesoMo, 0, UsuarioM);
                if (Message.Number > 0)
                {
                    AccesoMo = (AccesoModel)Message.OData;
                    AccesoMo._UsuarioMo._Password = Password;
                    Message.Message = "Acceso correcto";
                    Message.Code = AccesoMo._Acceso_id;
                }
                else
                    Message.Message = "Error al registrar el acceso";
            }
            else
            {
                Message.Number = -300;
                Message.Message = "Datos incorrectos de acceso";
            }
            return Message;
        }

        /// <summary>
        /// Validar acceso usuario
        /// </summary>
        /// <param name="UsuarioM"></param>
        /// <returns></returns>
        public MgkMessage Login_simple(UsuarioModel UsuarioM)
        {
            if (Message == null)
                Message = new MgkMessage();
            AccesoDao AccesoD = null;
            if (AccesoD == null)
                AccesoD = new AccesoDao();

            if (UsuarioM == null)
            {
                Message = new MgkMessage
                {
                    Number = -100,
                    Message = "Error de acceso. Falta definir datos"
                };
                return Message;
            }

            UsuarioCtrl UsuarioC = new UsuarioCtrl();
            MgkMessage Msg = UsuarioC.GetForLogin_simple(UsuarioM);
            if (Msg.Number >= 0)
            {
                UsuarioC.UsuarioItem._Aux_front = UsuarioM._Aux_front??"";                
            }
            else
            {
                Message.Number = -300;
                Message.Message = "Datos incorrectos de acceso";
                return Message;
            }
            return Message;
        }


        /// <summary>
        /// Validar acceso
        /// </summary>
        /// <param name="User_name"></param>
        /// <param name="Password"></param>
        /// <param name="Origen"></param>
        /// <param name="Acceso_id"></param>
        /// <returns></returns>
        public MgkMessage Validar(String Usuario_id, String Password, String Origen, int Acceso_id = 0, String Did = "")
        {
            if (Message == null)
                Message = new MgkMessage();
            AccesoDao AccesoD = null;
            if (AccesoD == null)
                AccesoD = new AccesoDao();

            Message.Clear();
            if (Acceso_id > 0)
            {
                AccesoModel AccesoM = AccesoD.GetItem(new AccesoModel { Acceso_id = Acceso_id }, Acceso_id);
                if (AccesoM != null && AccesoM.Acceso_id == Acceso_id && (AccesoM.Origen == Origen || Did != ""))
                //if (AccesoM.Acceso_id == Acceso_id && AccesoM._User_name== User_name && AccesoM.Origen == Origen)
                {

                    //AccesoM.Fecha_actualiza = DateTime.Now;
                    //AccesoD.Update(AccesoM, new string[] { "Fecha_actualiza" });
                    //Message.OData = AccesoM;

                    if (DateTime.Now.Subtract(AccesoM.Fecha_actualiza.Value).TotalSeconds < AppConstantes.SESION_SEGUNDOS || Did != "")
                    {
                        AccesoM.Fecha_actualiza = DateTime.Now;
                        AccesoD.UpdateObject(AccesoM, new string[] { "Fecha_actualiza" });
                        Message.OData = AccesoM;
                    }
                    else
                    {
                        //Message.Number = -100;
                        //Message.Message = "Ha superado el tiempo de inactividad";

                        AccesoM.Fecha_actualiza = DateTime.Now;
                        AccesoD.UpdateObject(AccesoM, new string[] { "Fecha_actualiza" });
                        Message.OData = AccesoM;
                    }
                }
                else
                {
                    Message.Number = -200;
                    Message.Message = "Acceso invalido";
                }
            }
            else
            {
                Login(new UsuarioModel { Usuario_id = Usuario_id, Password = Password }, Origen, Did);
            }

            return Message;
        }

        /// <summary>
        /// Validar token para recuperacion de contraseña
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public MgkMessage ValidarActualizarPassword(String token)
        {
            if (Message == null)
                Message = new MgkMessage();
            AccesoDao AccesoD = null;
            if (AccesoD == null)
                AccesoD = new AccesoDao();

            UsuarioCtrl UsuarioC = new UsuarioCtrl();
            MgkMessage Msg = UsuarioC.ValidarActualizarPassword(token);
            return Msg;
        }

        public MgkMessage ActualizarPassword(UsuarioModel UsuarioMo)
        {
            if (Message == null)
                Message = new MgkMessage();
            AccesoDao AccesoD = null;
            if (AccesoD == null)
                AccesoD = new AccesoDao();

            UsuarioCtrl UsuarioC = new UsuarioCtrl();
            MgkMessage Msg = UsuarioC.ActualizarPasswordRecuperado(UsuarioMo);
            return Msg;
        }

        public static MgkMessage SolicitaRecuperarPassword(UsuarioModel UsuarioMo)
        {
            UsuarioCtrl UsuarioC = new UsuarioCtrl();
            //MgkMessage Msg = UsuarioC.SolicitaRecuperarPassword(UsuarioMo);
            MgkMessage Msg = UsuarioC.CorreoAsignarNuevoPassword(UsuarioMo);
            return Msg;
        }

        public MgkMessage ActualizarFechaAcceso(AccesoModel AccesoM, AccesoDetalleModel AccesoDetalleMod)
        {
            MgkMessage Msg=new MgkMessage { 
                Number=-123,
                Message="sin registro por validar"
            };

            if (AccesoM == null)
                return Msg;

            AccesoDao AccesoD = new AccesoDao();
            List<DbParameter> parameters = new List<DbParameter>();

            parameters.Add((DbParameter)AccesoD.GetParameter("Acceso_id", AccesoM.Acceso_id));
            parameters.Add((DbParameter)AccesoD.GetParameter("Ruta", AccesoDetalleMod.Ruta));
            parameters.Add((DbParameter)AccesoD.GetParameter("Method", AccesoDetalleMod.Method));
            parameters.Add((DbParameter)AccesoD.GetParameter("Datos", AccesoDetalleMod.Datos));
            try
            {
                Msg = AccesoD.ReadObjItemByQuery<MgkMessage>("_acceso_upd", parameters, System.Data.CommandType.StoredProcedure);
            }catch(Exception ex)
            {
                Msg.Number = -323;
                Msg.Message = "Error actualizar acceso detalle";
            }            

            return Msg;
        }

        public void ActualizarFechaAcceso(AccesoModel AccesoM)
        {
            AccesoDao AccesoD = null;
            if (AccesoD == null)
                AccesoD = new AccesoDao();
            AccesoD.UpdateUltimoAcceso(AccesoM);
        }
        public void ActualizarFechaSalida(AccesoModel AccesoM)
        {
            AccesoDao AccesoD = null;
            if (AccesoD == null)
                AccesoD = new AccesoDao();
            AccesoD.UpdateSalida(AccesoM);
        }

        public MgkMessage Sessiones_activas(AccesoModel AccesoM)
        {
            MgkMessage oMessage = new MgkMessage();
            MgkSessionAccesoDao oDao = new MgkSessionAccesoDao();
            oMessage.OData = oDao.Sessiones_activas();
            return oMessage;
        }

        public MgkMessage Cortar_session(List<UsuarioModel> Usuarios,AccesoModel AccesoM)
        {
            MgkMessage oMessage = new MgkMessage();
            MgkSessionAccesoDao oDao = new MgkSessionAccesoDao();
            foreach(var item in Usuarios)
                oDao.Cortar_session(item);
            return oMessage;
        }

    }
}