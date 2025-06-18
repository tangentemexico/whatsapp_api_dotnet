using Mgk.Commonsx;
using Mgk.Base.Parametro;
using Mgk.Base.Token;
using Mgk.Base.Usuario;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mgk.Base.ControlAcceso;
using Mgk.Base.MgkSmtp;
using RestSharp;
using static System.Net.Mime.MediaTypeNames;

namespace Mgk.Base.Usuario
{
    public class UsuarioCtrl
    {
        protected UsuarioDao UsuarioD { get; set; }

        public MgkMessage Message { get; set; }
        public UsuarioModel UsuarioItem { get; set; }

        public UsuarioCtrl()
        {
            Message = new MgkMessage();
            UsuarioD = new UsuarioDao();
        }

        public MgkMessage Guardar(UsuarioModel UsuarioM, AccesoModel AccesoM)
        {
            UsuarioModel UsuarioMx = UsuarioD.ReadNew<UsuarioModel>(UsuarioM);
            if (UsuarioMx==null)
                Message = this.Insert(UsuarioM, AccesoM);
            else
                Message = this.Update(UsuarioM, AccesoM);
            return Message;
        }

        /// <summary>
        /// Insertar Usuario
        /// </summary>
        /// <param name="UsuarioM"></param>
        /// <returns></returns>
        public MgkMessage Insert(UsuarioModel UsuarioM, AccesoModel AccesoM)
        {
            Message = UsuarioD.Insert(UsuarioM);
            return Message;
        }

        /// <summary>
        /// Actualizar Usuario
        /// </summary>
        /// <param name="UsuarioM"></param>
        /// <returns></returns>
        public MgkMessage Update(UsuarioModel UsuarioM, AccesoModel AccesoM)
        {
            Message = UsuarioD.Update(UsuarioM);

            return Message;
        }

        /// <summary>
        /// Eliminar Usuario
        /// </summary>
        /// <param name="UsuarioM"></param>
        /// <param name="UsuarioM"></param>
        /// <returns></returns>
        public MgkMessage Delete(UsuarioModel UsuarioM, AccesoModel AccesoM)
        {
            Message = UsuarioD.Delete(UsuarioM);
            return Message;
        }


        public MgkMessage Get(UsuarioModel UsuarioM, AccesoModel AccesoM)
        {
            UsuarioModel UsuarioItem = GetUsuario(UsuarioM, AccesoM);
            Message = UsuarioD.Message;
            Message.OData = UsuarioItem;
            return Message;
        }

        public UsuarioModel GetUsuario(UsuarioModel UsuarioM, AccesoModel AccesoM)
        {
            UsuarioModel UsuarioItem = UsuarioD.GetItem(UsuarioM);
            Message = UsuarioD.Message;
            return UsuarioItem;
        }

        public MgkMessage GetLogin(UsuarioModel UsuarioM)
        {
            UsuarioItem = UsuarioD.Login(UsuarioM);
            Message = UsuarioD.Message;
            Message.OData = UsuarioItem;
            return Message;
        }

        /// <summary>
        /// Leer usuario para login
        /// </summary>
        /// <param name="UsuarioM"></param>
        /// <returns></returns>
        public MgkMessage GetForLogin(UsuarioModel UsuarioM, String Did = "", Boolean bSoloUsuario = false)
        {
            Message.Number = -190;
            String Password = "";
            if (UsuarioM!=null)
                Password = UsuarioM.Password;

            UsuarioModel UsuarioMx = UsuarioD.Login(UsuarioM, Did,bSoloUsuario);
            if (UsuarioD.Message.Number == -8080)
            {
                string mess = UsuarioD.Message.Message;
                MgkMessage mess1 = UsuarioD.Message.GetCopy();
                //EnviarCredenciales(UsuarioM);
                CorreoAsignarNuevoPassword(UsuarioM);
                UsuarioD.Message=mess1;
                return mess1;
            }

            if (UsuarioMx == null && UsuarioM!=null) {
                // validar acceso por TOKEN
                TokenCtrl oCtrl = new TokenCtrl();
                TokenModel TokenM = new TokenModel {Usuario_id= UsuarioM.Usuario_id,Token= Password };
                TokenM=oCtrl.BuscarToken(TokenM);
                if (TokenM != null)
                {
                    UsuarioMx = UsuarioD.Login(UsuarioM, Did,true);
                    oCtrl.Delete(TokenM,0);
                }
            }

            Message = UsuarioD.Message;
            Message.OData = UsuarioMx;
            this.UsuarioItem = UsuarioMx;
            return Message;
        }

        public MgkMessage GetForLogin_simple(UsuarioModel UsuarioM)
        {
            UsuarioModel UsuarioMx = UsuarioD.Login_simple(UsuarioM);
            Message = UsuarioD.Message;
            Message.OData = UsuarioMx;
            this.UsuarioItem = UsuarioMx;
            if (UsuarioMx == null)
                Message.Number = -1;
            return Message;
        }


        /// <summary>
        /// Leer Lista de Usuarios
        /// </summary>
        /// <param name="UsuarioM"></param>
        /// <param name="UsuarioM"></param>
        /// <returns></returns>
        public List<UsuarioModel> GetAll(UsuarioModel UsuarioM, int Acceso_id)
        {
            List<UsuarioModel> UsuarioItems = UsuarioD.GetItems(UsuarioM);
            Message = UsuarioD.Message;

            return UsuarioItems;
        }

        public List<UsuarioModel> GetAll(int Acceso_id)
        {
            List<UsuarioModel> UsuarioItems = UsuarioD.GetItems(new UsuarioModel());
            Message = UsuarioD.Message;

            return UsuarioItems;
        }

        /// <summary>
        /// Leer usuario para login
        /// </summary>
        /// <param name="UsuarioM"></param>
        /// <returns></returns>
        public MgkMessage GetForLogin(UsuarioModel UsuarioM)
        {
            UsuarioM = UsuarioD.Login(UsuarioM);
            Message = UsuarioD.Message;
            Message.OData = UsuarioM;
            return Message;
        }

        /// <summary>
        /// Enviar correo con liga para asignar nuevo password
        /// </summary>
        /// <param name="UsuarioM"></param>
        /// <returns></returns>
        public MgkMessage SolicitaRecuperarPassword(UsuarioModel UsuarioM)
        {
            bool recuperap_email = ParametroCtrl.GetValueBool("recuperap_email",true).Value;
            bool recuperap_whasapp = ParametroCtrl.GetValueBool("recuperap_whasapp",false).Value;

            if (MgkFunctions.EsNumeroFlotante(UsuarioM.Email))
            {
                UsuarioM.Telefono = UsuarioM.Email;
                UsuarioM.Email = "";
            }

            List<UsuarioModel> Items = UsuarioD.GetItems(UsuarioM);
            if (Items.Count > 0)
            {
                TokenModel TokenMo = new TokenModel();
                TokenMo.Usuario_id = Items[0].Usuario_id;
                UsuarioD.ReadObject<TokenModel>(TokenMo, new string[] { "Usuario_id" });
                if (TokenMo.Token_id > 0)
                    UsuarioD.DeleteObject(TokenMo);

                TokenMo.Token_id = 0;
                TokenMo.Usuario_id = Items[0].Usuario_id;
                TokenMo.Fecha = DateTime.Now;
                TokenMo.Token = MgkFunctions.RandomLength(6).ToString();
                UsuarioD.Insert(TokenMo);

                String Token = ParametroCtrl.GetValue("g_url_rp");
                string[] partes = new[] { Items[0].Usuario_id.ToString(), TokenMo.Token_id.ToString() };
                Token += "Inicio/NuevoPassword/?token=";

                partes[0] = (Newtonsoft.Json.JsonConvert.SerializeObject(partes));
                partes[0] = TokenMo.Token;
                Token += partes[0];
                //Token += "/AsignarNuevoPassword/";

                StringBuilder Txt = new StringBuilder();
                Txt.Append("Hola: ["+ Items[0].Usuario_id + "] " + Items[0].Nombre + " <br>");
                Txt.Append("Utiliza este código de acceso temporal :" + TokenMo.Token + "<br>");
                //Txt.Append("Utiliza este enlace para asignar una nueva contraseña <br>");
                //Txt.Append("<a href=\"" + Token + "\">" + Token + "</a><br>");
                Txt.Append("<br>Si tu no solicitaste la recuperación de contraseña, por favor ignora este correo<br>");

                //TokenMo.Token = partes[0];
                Items[0]._Password = TokenMo.Token;
                Items[0].Password_tmp = Items[0].Password;
                UsuarioD.Update(Items[0], "Password_tmp");

                if (recuperap_email)
                {
                    MgkSmtpCtrl MgkSmtp = new MgkSmtpCtrl();
                    Message = MgkSmtp.Send(new MgkSmtpModel
                    {
                        To = Items[0].Email,
                        Subject = "Recuperación de contraseña",
                        Body = Txt.ToString(),
                        IsBodyHtml = true
                    });
                    if (Message.Number >= 0)
                        Message.Message = "Se ha enviado un correo con indicaciones para recuperar acceso.";
                    else
                        Message.Message = "El servicio de correo presenta fallos para enviar las indicaciones a su correo, contacte al administrador del sistema.";
                }

                if (recuperap_whasapp)
                {
                    Txt.Clear();
                    Txt.Append("Recuperación de contraseña:\n"+ TokenMo.Token);
                    //NotificacionesCtrl.EnviarWhatsapp(Items[0].Telefono, Txt.ToString() );
                    if (Message.Number >= 0)
                        Message.Message += "Utilice el token que le ha llegado por whatsapp.";
                    else
                        Message.Message += "El servicio de whatsapp tiene falla, contacta al administrador para recuperar tu contraseña.";
                }

            }
            else
            {
                Message.Number = -1;
                Message.Message = "Información no corresponde a usuarios registrados";
            }
            return Message;
        }

        public MgkMessage CorreoAsignarNuevoPassword(UsuarioModel UsuarioM)
        {
            UsuarioM.Es_activo = true;
            UsuarioM.Password = null;
            List<UsuarioModel> Items = UsuarioD.GetItems(UsuarioM);
            if (Items.Count > 0)
            {
                UsuarioM = Items[0];
                if (Items[0].Es_activo != true)
                {
                    Message.Number = -1;
                    Message.Message = "Datos incorrectos";
                    return Message;
                }
                TokenModel TokenMo = new TokenModel();
                TokenMo.Usuario_id = Items[0].Usuario_id;
                UsuarioD.Read<TokenModel>(TokenMo, "Usuario_id");
                if (TokenMo.Token_id > 0)
                    UsuarioD.DeleteObject(TokenMo);

                TokenMo.Token_id = 0;
                TokenMo.Usuario_id = Items[0].Usuario_id;
                TokenMo.Fecha = DateTime.Now;
                TokenMo.Token = "";
                UsuarioD.Insert(TokenMo);

                String Token = ParametroCtrl.GetValue("g_url");
                string[] partes = new[] { Items[0].Usuario_id.ToString(), TokenMo.Token_id.ToString() };
                Token += "Inicio/NuevoPassword/?token=";
                partes[0] = (Newtonsoft.Json.JsonConvert.SerializeObject(partes));
                Token += partes[0];
                //Token += "/AsignarNuevoPassword/";
                StringBuilder Txt = new StringBuilder();
                string Usuario_id = Items[0].Usuario_id;
                if (!string.IsNullOrEmpty(Items[0].EmpleadoClave))
                {
                    Usuario_id = Items[0].EmpleadoClave.Substring(3);
                }
                Txt.Append("Hola:" + Items[0].Nombre + " <br>");
                Txt.Append("Nombre de usuario: " + Usuario_id + "<br>");
                Txt.Append("Utiliza esta liga para asignar una nueva contraseña <br>");
                Txt.Append("<a href=\"" + Token + "\">" + Token + "</a><br>");
                Txt.Append("Si tu no solicitaste la recuperación de contraseña, por favor ignora este correo<br>");

                TokenMo.Token = partes[0];
                UsuarioD.Update(TokenMo);


                MgkSmtpCtrl JuxSmtp = new MgkSmtpCtrl();

                Message = JuxSmtp.Send(new MgkSmtpModel
                {
                    To = UsuarioM.Email,
                    Subject = "Recuperación de contraseña",
                    Body = Txt.ToString(),
                    IsBodyHtml = true
                });
                // Enviar correo
                // Remitente:  ParametroCtrl.GetValue("smtp.from");
                // Destinatario: UsuarioM.Email
                // Asunto: Recuperación de contraseña
                // Mensaje: Txt.ToString()
                if (Message.Number >= 0)
                    Message.Message = "Se ha enviado un correo con indicaciones para recuperar acceso";
                else
                    Message.Message = "El servicio de correo presenta fallos para enviar las indicaciones a su correo, contacte al administrador del sistema";
            }
            else
            {
                Message.Number = -1;
                Message.Message = "Correo no existe";
            }
            return Message;
        }

        /// <summary>
        /// Validar token para recuperacion de contraseña. Vigencia 24 horas desde petición
        /// </summary>
        /// <param name="UsuarioM"></param>
        /// <returns></returns>
        public MgkMessage ValidarActualizarPassword(String Token)
        {
            try
            {
                String xToken = Token;
                string[] partes = Newtonsoft.Json.JsonConvert.DeserializeObject<string[]>(xToken);
                if (!(partes != null && partes.Length > 1))
                    return Message;

                Message.Number = 1;
                Message.Code = partes[1];

                TokenModel TokenMo = new TokenModel();
                TokenMo.Token_id = MgkFunctions.StrToInt(Message.Code);
                TokenMo = UsuarioD.ReadNew<TokenModel>(TokenMo);

                if (TokenMo!=null && TokenMo.Token_id > 0 && TokenMo.Token == Token)
                    Message.Message = "Token correcto";
                else
                {
                    Message.Number = -1;
                    Message.Message = "Token Incorrecto!";
                }
            }
            catch (Exception e)
            {
                Message.Number = -1;
                Message.Code = "0";
                Message.Message = "Enlace incorrecto";
            }
            return Message;
        }

        /// <summary>
        /// Actualizar password por solicitud de recuperacion
        /// </summary>
        /// <param name="UsuarioMo"></param>
        /// <returns></returns>
        public MgkMessage ActualizarPasswordRecuperado(UsuarioModel UsuarioMo)
        {
            try
            {
                // Validar existencia de token para recuperacion de contraseña
                Message = ValidarActualizarPassword(UsuarioMo.Usuario_id);
                if (Message.Number > 0)
                {
                    TokenModel TokenMo = new TokenModel();
                    TokenMo.Token_id = MgkFunctions.StrToInt(Message.Code);
                    TokenMo = UsuarioD.ReadNew<TokenModel>(TokenMo);

                    // actualizar password de usuario
                    UsuarioMo.Usuario_id = TokenMo.Usuario_id;
                    //UsuarioMo._Password = UsuarioMo.Password;
                    //UsuarioD.Update(UsuarioMo, "Password" );
                    ActualizarPassword(UsuarioMo, new AccesoModel { _UsuarioMo=UsuarioMo});

                    // eliminar token
                    UsuarioD.DeleteObject(TokenMo);
                }
                else
                {
                    Message.Message = "Enlace incorrecto";
                }
            }
            catch (Exception e)
            {
                Message.Number = -1;
                Message.Message = "Error de enlace incorrecto";
            }
            return Message;
        }

        public MgkMessage ActualizarPassword(UsuarioModel UsuarioMo, AccesoModel AccesoM)
        {
            Message.Clear();
            try
            {
                // actualizar password de usuario
                UsuarioMo._Password = UsuarioMo.Password;
                UsuarioD.Update(UsuarioMo, "Password" );
                if (UsuarioD.Message.Number >= 0)
                {
                    Message.Message = "Actualización exitosa";
                    UsuarioMo = UsuarioD.GetItem(UsuarioMo);

                    StringBuilder Txt = new StringBuilder();
                    Txt.Append($"Estimado {UsuarioMo.Nombre}<br/>");
                    Txt.Append($"Le notificamos que su contraseña se actualizó exitosamente el día {DateTime.Now.ToString("dd-MM-yyyy hh:mm tt")}<br/><br/>");
                    Txt.Append("<i>Si usted no solicitó este cambio de contraseña, favor de comunicar con el departamento de sistemas</i><br/>");


                    MgkSmtpCtrl JuxSmtp = new MgkSmtpCtrl();

                    Message = JuxSmtp.Send(new MgkSmtpModel
                    {
                        To = UsuarioMo.Email,
                        Subject = "Cambio de contraseña exitoso",
                        Body = Txt.ToString(),
                        IsBodyHtml = true
                    });
                }
            }
            catch (Exception e)
            {
                Message.Number = -1;
                Message.Message = "Error de enlace incorrecto";
            }
            return Message;
        }

        public MgkMessage EnviarCredenciales(UsuarioModel UsuarioM)
        {
            string url = ParametroCtrl.GetValue("g_url");
            var UsuarioItem = UsuarioD.GetItem(UsuarioM);
            String newpassword= GenerateRandomPassword(8);
            StringBuilder Txt = new StringBuilder();
            Txt.Append("Hola: [" + UsuarioItem.Usuario_id + "] " + UsuarioItem.Nombre + " <br>");
            Txt.Append("Estos son tus datos para ingresar al sistema<br>");
            Txt.Append($"Usuario:{UsuarioItem.Usuario_id}<br>");
            Txt.Append($"Contraseña:{newpassword}<br><br>");
            Txt.Append($"Desde cualquier navegador de internet, utiliza:<a href='{url}'>{url}</a> <br>");
            Txt.Append("También puedes descargar la aplicación rhenlinea desde las tiendas oficiales de Android y Apple<br>");

            UsuarioItem._Password = newpassword;
            UsuarioD.Update(UsuarioItem, "Password");

            MgkSmtpCtrl MgkSmtp = new MgkSmtpCtrl();
            Message = MgkSmtp.Send(new MgkSmtpModel
            {
                To = UsuarioItem.Email,
                Subject = "RH en línea - Datos de acceso",
                Body = Txt.ToString(),
                IsBodyHtml = true
            });
            if (Message.Number >= 0)
                Message.Message = "Se ha enviado un correo con indicaciones para recuperar acceso.";
            else
                Message.Message = "El servicio de correo presenta fallos para enviar las indicaciones a su correo, contacte al administrador del sistema.";

            return Message;
        }
        public static string GenerateRandomPassword(int length)
        {
            if (length < 6)
            {
                throw new ArgumentException("La longitud mínima de la contraseña debe ser 6.");
            }

            // Caracteres permitidos
            string lowerCase = "abcdefghijklmnopqrstuvwxyz";
            string upperCase = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            string digits = "0123456789";

            // Se garantiza que haya al menos un caracter de cada tipo
            Random rand = new Random();

            // Seleccionar al menos 1 de cada tipo
            char lower = lowerCase[rand.Next(lowerCase.Length)];
            char upper = upperCase[rand.Next(upperCase.Length)];
            char digit = digits[rand.Next(digits.Length)];

            // Rellenar el resto de la contraseña con caracteres aleatorios
            string allChars = lowerCase + upperCase + digits;
            StringBuilder password = new StringBuilder();
            password.Append(lower);
            password.Append(upper);
            password.Append(digit);

            // Añadir caracteres aleatorios hasta completar la longitud
            for (int i = 3; i < length; i++)
            {
                password.Append(allChars[rand.Next(allChars.Length)]);
            }

            // Mezclar los caracteres para evitar un patrón predecible
            return new string(password.ToString().OrderBy(c => rand.Next()).ToArray());
        }
    }
}
