using Mgk.Base.Parametro;
using Mgk.Base.Usuario;
using Mgk.Commonsx;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Mgk.Base.MgkSmtp
{
    public class MgkSmtpCtrl
    {      
        EmailParameters emailParameters { get; set; }
        MgkMessage Message { get; set; }

        public MgkSmtpCtrl()
        {
            Message = new MgkMessage();
        }

        public void SetParameters(EmailParameters emp)
        {
            SetEmailParameters(emp);
        }
        private void SetEmailParameters(EmailParameters emp = null)
        {
            if (emp != null)
            {
                emailParameters = emp;
                return;
            }
            
            if (emailParameters == null)
                emailParameters = new EmailParameters();

            emailParameters.bSmtpStatus = ParametroCtrl.GetValueBool("smtp_status", true).Value;
            emailParameters.sSmtpHost = ParametroCtrl.GetValue("smtp_host");
            emailParameters.iSmtpPort = ParametroCtrl.GetValueInt("smtp_port");
            emailParameters.sSmtpUser = ParametroCtrl.GetValue("smtp_user");
            emailParameters.sSmtpPassword = ParametroCtrl.GetValue("smtp_password");
            emailParameters.bSmtpSsl = ParametroCtrl.GetValueBool("smtp_ssl", false).Value;
            emailParameters.sSmtpFromName = ParametroCtrl.GetValue("smtp_FromName");
            emailParameters.sSmtpFrom = ParametroCtrl.GetValue("smtp_From");

            emailParameters.email_sign = ParametroCtrl.GetValue("g_firma", "MGK");
            emailParameters.owner = ParametroCtrl.GetValue("g_propietario");

            //emailParameters.bSmtpStatus = true;
            //emailParameters.sSmtpHost = "smtp.gmail.com";
            //emailParameters.iSmtpPort = 587;
            //emailParameters.sSmtpUser = "ccp@dimeca.com.mx";
            //emailParameters.sSmtpPassword = "Dimeca09";
            //emailParameters.bSmtpSsl = true;
            //emailParameters.sSmtpFromName = "ccp@dimeca.com.mx";
            //emailParameters.sSmtpFrom = "ccp@dimeca.com.mx";

            //emailParameters.email_sign = ParametroCtrl.GetValue("g_firma", "MGK");
            //emailParameters.owner = ParametroCtrl.GetValue("g_propietario");
        }

        /// <summary>
        /// Enviar correo a un solo destinatario
        /// </summary>
        /// <param name="usuarioPara"></param>
        /// <param name="subject"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        public MgkMessage Send(UsuarioModel usuarioPara, String subject, String body)
        {
            if (Message == null)
                Message = new MgkMessage();
            List<UsuarioModel> ToList = new List<UsuarioModel>();
            ToList.Add(usuarioPara);
            Message = this.Send(new MgkSmtpModel
            {
                ToList = ToList,
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            });
            return Message;
        }

        public MgkMessage SendViaApi(MgkSmtpModel emailSendModel)
        {
            if (Message == null)
                Message = new MgkMessage();
            else
                Message.Clear();

            try
            {
                string smtp_api_url = ParametroCtrl.GetValue("smtp_apiurl", "");
                if (smtp_api_url == "")
                {
                    Message.Number = -1000;
                    Message.Message = "Error en confirugación de mesajeria [url api]";
                    return Message;
                }

                var emailSendModelJson = Newtonsoft.Json.JsonConvert.SerializeObject(emailSendModel);
                MgkSmtpModelApi eModApi = Newtonsoft.Json.JsonConvert.DeserializeObject<MgkSmtpModelApi>(emailSendModelJson);

                if (emailParameters == null)
                    SetEmailParameters();

                if (!(emailSendModel.ToList != null && emailSendModel.ToList.Count > 0))
                {
                    Message.Number = -1;
                    Message.Message = "No se ha definido destinatario para envio de email";
                    return Message;
                }

                eModApi.Parameters = this.emailParameters;

                var client = new RestClient(smtp_api_url);                
                var request = new RestRequest("api/SMTP", Method.Post);
                request.AddHeader("Content-Type", "application/json");
                var body = Newtonsoft.Json.JsonConvert.SerializeObject(eModApi);
                request.AddParameter("application/json", body, ParameterType.RequestBody);
                var response = client.Execute(request);
                //Console.WriteLine(response.Content);
            }
            catch(Exception ex)
            {
                Message.Number = -1000;
                Message.Message = "Error inesperado enviar email";
                Message.Exception = ex.ToString();
            }

            return Message;
        }

        /// <summary>
        /// Enviar uno o varios correos
        /// </summary>
        /// <param name="emailSendModel"></param>
        /// <returns></returns>
        public MgkMessage Send(MgkSmtpModel emailSendModel,String mode=null)
        {
            if (Message == null)
                Message = new MgkMessage();
            else
                Message.Clear();
            emailSendModel.SetMgkSmtpModel();

            //bool smtp_via_api = ParametroCtrl.GetValueBool("smtp_via_api",false ).Value;
            //if (smtp_via_api)
            //{
            //    return SendViaApi(emailSendModel);
            //}

            if (mode == null)
                mode = ParametroCtrl.GetValue("smtp_mode", "smtp").ToLower();
            if (mode == "api") {
                return SendViaApi(emailSendModel);
            }

            String mailBcc = "";
            try
            {
                if (emailParameters == null)
                    SetEmailParameters();
                if (!(emailSendModel.ToList != null && emailSendModel.ToList.Count > 0))
                {
                    return Message;
                }
                String txtTo = emailSendModel.ToList[0].Email;
                if (emailParameters.bSmtpStatus == false)
                {
                    Message.SetMessage(new MgkMessage
                    {
                        Type = MgkMessage.TYPE_WARNING,
                        Number = -100000,
                        Message = "Servicio de correo inactivo"
                    });
                    return Message;
                }
                txtTo = txtTo.Replace(';', ',');
                string[] arr1 = txtTo.Split(',');
                if (arr1.Length > 1)
                {
                    emailSendModel.ToList.Clear();
                    foreach (string txt in arr1)
                        emailSendModel.ToList.Add(new UsuarioModel { Email = txt, Nombre = "Usuario" });
                }

                if (emailSendModel.BccList != null && emailSendModel.BccList.Count > 0)
                {
                    String txtBcc = emailSendModel.BccList[0].Email;
                    txtBcc = txtBcc.Replace(';', ',');
                    string[] arr2 = txtBcc.Split(',');
                    if (arr1.Length > 1)
                    {
                        emailSendModel.BccList.Clear();
                        foreach (string txt in arr1)
                            emailSendModel.BccList.Add(new UsuarioModel { Email = txt, Nombre = "Usuario Oculto" });
                    }
                }

                MailAddress mailFrom = new MailAddress(emailParameters.sSmtpFrom, emailParameters.sSmtpFromName ?? "");
                MailAddress mailTo = new MailAddress(emailSendModel.ToList[0].Email, emailSendModel.ToList[0].Nombre == "" ? emailSendModel.ToList[0].Email : emailSendModel.ToList[0].Nombre);


                MailMessage mail = new MailMessage(mailFrom, mailTo);
                mail.IsBodyHtml = emailSendModel.IsBodyHtml;
                mail.Subject = emailSendModel.Subject;
                if (emailSendModel.Encoding != null)
                    mail.BodyEncoding = Encoding.GetEncoding(emailSendModel.Encoding);
                mail.Body = emailSendModel.Body;
                if ((this.emailParameters.email_sign ?? "" + emailParameters.owner ?? "").Length > 0)
                    mail.Body += "<br/><br/>" + this.emailParameters.email_sign + string.Format("<br/><strong>{0}</strong><br/>", emailParameters.owner);

                if (emailSendModel.ToList.Count > 1)
                {
                    for (int i = 1; i < emailSendModel.ToList.Count; i++)
                        mail.To.Add(new MailAddress(emailSendModel.ToList[i].Email, emailSendModel.ToList[i].Nombre));
                }

                try
                {
                    String mailCC = MgkFunctions.AppSettings("smtp_cc", null, true);
                    if (mailCC != null)
                    {
                        string[] mailCCArr = mailCC.Split(',');
                        foreach (string direccion in mailCCArr)
                            mail.CC.Add(direccion);
                    }
                }
                catch (Exception e)
                {
                    MgkLog.Error(new MgkMessage
                    {
                        Number = -100000,
                        Message = "Error al obtener destinatarios CC",
                        Exception = e.ToString(),
                    });
                    //MgkFunctions.warning(this.ToString(), "Error el obtener destinatarios CC:" + e.ToString());
                }

                try
                {
                    mailBcc = MgkFunctions.AppSettings("smtp_bcc", null, true);
                    if (mailBcc != null && mailBcc != "")
                    {
                        string[] mailBccArr = mailBcc.Split(',');
                        foreach (string direccion in mailBccArr)
                            mail.Bcc.Add(direccion);
                    }
                }
                catch (Exception e)
                {
                    Message.SetMessage(new MgkMessage
                    {
                        Source = this.ToString(),
                        Number = -100000,
                        Exception = e.ToString(),
                        Message = "Error el obtener destinatarios BccList:",
                        OData = new
                        {
                            mailBcc = mailBcc
                        }
                    });
                    MgkLog.Error(Message);
                }

                if (emailSendModel.CcList != null && emailSendModel.CcList.Count > 0)
                    foreach (UsuarioModel user in emailSendModel.CcList)
                        mail.CC.Add(user.Email);

                if (emailSendModel.BccList != null && emailSendModel.BccList.Count > 0)
                    foreach (UsuarioModel user in emailSendModel.BccList)
                        mail.Bcc.Add(user.Email);

                SmtpClient client = new SmtpClient(emailParameters.sSmtpHost);

                if (emailParameters.iSmtpPort != 0)
                    client.Port = emailParameters.iSmtpPort;

                if (emailParameters.sSmtpUser != null && emailParameters.sSmtpUser != "")
                {
                    client.Credentials = new System.Net.NetworkCredential(emailParameters.sSmtpUser, emailParameters.sSmtpPassword);
                    client.EnableSsl = emailParameters.bSmtpSsl;
                }

                if (emailSendModel.AttachPathList != null)
                    foreach (String s in emailSendModel.AttachPathList)
                    {
                        if (System.IO.File.Exists(s))
                        {
                            Attachment att = new Attachment(s);
                            mail.Attachments.Add(att);
                        }
                    }

                client.Send(mail);
                mail.Dispose();
                client.Dispose();
                Message.SetMessage(new MgkMessage
                {
                    Number = 1,
                    Message = "Correo enviado",
                });
            }
            catch (Exception e)
            {
                Message.SetMessage(new MgkMessage
                {
                    Source = this.ToString(),
                    Number = -100000,
                    Exception = e.ToString(),
                    Message = "Error en envio de correo",
                    OData = new
                    {
                        emailSendModel = emailSendModel,
                        emailParameters = emailParameters
                    }
                });
                MgkLog.Error(Message);
            }


            return Message;
        }
    }
}