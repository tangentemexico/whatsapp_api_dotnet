using Mgk.Base.Parametro;
using Mgk.Commonsx;
using Mgk.Commonsx.Net6;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WhatsappControl.Enviar
{
    public class EnviarCtrl
    {
        public EnviarDao enviarDao;
        public EnviarCtrl() {
            if (enviarDao == null)
                enviarDao = new EnviarDao();
        }
        public MgkMessage EnviarMensaje(EnviarModelRequest enviarMod)
        {

            MgkLog.Debug(new MgkMessage
            {
                Number=1,
                Message="Mensaje Recibido",
                Source="EnviarCtrl.EnviarMensaje",
                OData=new {recibido = enviarMod }
            });

            MgkMessage message = new MgkMessage
            {
                Number = 0,
                Message = "Mensaje recibido"
            };

            enviarMod.Destinatarios = enviarMod.Destinatarios ?? "";
            enviarMod.Destinatarios = enviarMod.Destinatarios.Replace(" ", "").Trim();

            if (string.IsNullOrEmpty(enviarMod.Destinatarios))
            {
                message.Number = -1;
                message.Message = "No se recibio numero destinatario";
            }

            if (enviarMod.Destinatarios.Length < 10)
            {
                message.Number = -11;
                message.Message = "Número incorrecto";
            }

            if (string.IsNullOrEmpty(enviarMod.Mensaje) && enviarMod.Archivo_local == null && string.IsNullOrEmpty(enviarMod.Archivo_url))
            {
                message.Number = -2;
                message.Message = "No se recibio mensaje";
            }
            if (message.Number < 0)
                return message;

            enviarMod.Destinatarios = enviarMod.Destinatarios.Replace(";", ",");

            string[] destinos_array = enviarMod.Destinatarios.Split(',');
            foreach (var numero_destino in destinos_array)
            {
                if (enviarMod.Mensaje == "###")
                    message.Message = "Comando recibido";
                else
                    message = _guarda_mensaje_(enviarMod, numero_destino);
            }

            try
            {
                (new Thread(() =>
                {
                    _enviar_pendientes();
                })).Start();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return message;
        }

        private async void _enviar_pendientes() 
        {
            var items = enviarDao.NoEnviados();
            if (items == null || items.Count == 0)
            {
                MgkLog.Debug("EnviarCtrl._enviar_pendientes", "No hay mensajes pendientes");
                return;
            }

            foreach (var item in items)
            {                
                MensajeModel item2=enviarDao.ReadNew<MensajeModel>(item);
                if ((item2.Es_enviado??false) !=true)
                {
                    item2._url_service = item._url_service;
                    var result = _enviar_mensaje_destino_unico(item2);
                    item2.Es_enviado = result.Number >= 0;  
                    enviarDao.ActualizarFechaEnvio(item2);
                }
                await Task.Delay(3000); // Espera 3 segundos (3000 ms)
            }
            _enviar_pendientes();
        }

        private MgkMessage _enviar_mensaje_destino_unico(MensajeModel mensajeMod)
        {
            MgkMessage message = new MgkMessage
            {
                Source = "EnviarCtrl._enviar_mensaje_destino_unico",
                Number = -1,
                Message = "Mensaje no enviado"
            };

            try
            {

                if (string.IsNullOrEmpty(mensajeMod._url_service) == false)
                {
                    MensajeNodeModel reqNode = new MensajeNodeModel();
                    reqNode.numero = mensajeMod.Destinatarios;
                    reqNode.mensaje = mensajeMod.Mensaje;
                    reqNode.Archivo_url = mensajeMod.Archivo_url;
                    reqNode.Archivo_local = mensajeMod.Archivo_local;
                    if (string.IsNullOrEmpty(reqNode.PaisDestino))
                        reqNode.PaisDestino = "52";

                    if (reqNode.numero.Length == 10)
                        reqNode.numero = reqNode.PaisDestino + "1" + reqNode.numero;           
                    
                    string jsontxt = Newtonsoft.Json.JsonConvert.SerializeObject(reqNode);

                    MgkLog.Debug("EnviarCtrl.EnviarMensaje=", mensajeMod._url_service + "\t"+ reqNode.numero + "\n" + jsontxt + "\n");
                    

                    var client = new RestClient(mensajeMod._url_service);
                    //client.time = -1;
                    var request = new RestRequest(mensajeMod._url_service, Method.Post);
                    request.AddHeader("Content-Type", "application/json");
                    var body = jsontxt;
                    request.AddParameter("application/json", body, ParameterType.RequestBody);
                    var response = client.Execute(request);
                    //Console.WriteLine(response.Content);

                    if (response != null)
                    {
                        //MgkLog.Debug(new MgkMessage { OData= response.Content ,Message="Response.Content",Source= "EnviarCtrl._enviar_mensaje_destino_unico" });
                        message = Newtonsoft.Json.JsonConvert.DeserializeObject<MgkMessage>(response.Content);
                        MgkLog.Debug(message);
                    }
                    else
                    {
                        MgkLog.Debug("EnviarCtrl.response.Content", "- no cachamos respuesta- ???");
                    }

                    if (message.Number>=0 && reqNode.Archivo_local != null)
                        MgkFiles.DeleteFile(reqNode.Archivo_local);
                }
            }
            catch (Exception ex2)
            {
                message.Number = -324;
                message.Message = "Error desconocido enviar mensaje destino:"+ mensajeMod.Destinatarios;
                message.Exception = ex2.ToString();
                MgkLog.Debug(message);
                MgkLog.Error(message);

                return message;
            }

            return message;
        }

        private MgkMessage _guarda_mensaje_(EnviarModelRequest enviarMod, string numero_destino, Boolean EsCopia = false)
        {
            MgkMessage message = new MgkMessage
            {
                Number = 0,
            };

            try
            {
                if (string.IsNullOrEmpty(numero_destino))
                    return message;

                enviarDao.Messages.ClearMessages();
                enviarMod.Destinatarios = numero_destino;

                MensajeModel mensajeMod = new MensajeModel();
                mensajeMod.Archivo_url = enviarMod.Archivo_url;
                mensajeMod.Mensaje = enviarMod.Mensaje;
                mensajeMod.Remitente = enviarMod.Remitente;
                mensajeMod.Destinatarios = enviarMod.Destinatarios;
                mensajeMod.PaisDestino = enviarMod.PaisDestino;
                mensajeMod.usuario_id = enviarMod.Usuario_id;
                mensajeMod.Origen = enviarMod.Origen;
                //if (EsCopia == false)
                    enviarDao.Guardar(mensajeMod);

                if (enviarMod.Archivo_local != null)
                {
                    enviarMod.Archivo_local.TrgtPath = MgkFunctions.AppSettings("Mgk.LogDirectory", @"c:\tmp\",true);
                    MgkFiles.CreateDirectory(enviarMod.Archivo_local.TrgtPath);
                    enviarMod.Archivo_local.FileName = mensajeMod.Mensaje_id.ToString()+"__"+ enviarMod.Archivo_local.FileName;
                    MgkFiles.WriteAllBytes(enviarMod.Archivo_local._FullName, enviarMod.Archivo_local._Bytes);
                    mensajeMod.Archivo_local = enviarMod.Archivo_local._FullName;
                    enviarDao.Update(mensajeMod, "Archivo_local");

                }
            }
            catch (Exception ex2)
            {
                message.Number = -324;
                message.Message = "Error desconocido enviar mensaje destino:" + numero_destino;
                message.Exception = ex2.ToString();
                MgkLog.Error(message);

                return message;
            }

            return message;
        }
    }
}
