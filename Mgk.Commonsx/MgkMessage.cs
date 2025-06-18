using System;
using System.Collections.Generic;
using System.Text;

namespace Mgk.Commonsx
{
    public class MgkMessage : ICloneable
    {
        public virtual object Clone()
        {
            return this.MemberwiseClone();
        }

        public const string TYPE_DANGER = "danger";
        public const string TYPE_WARNING = "warning";
        public const string TYPE_INFO = "info";
        public const string TYPE_SUCCESS = "success";

        public int Number { get; set; } = 0;
        public string Code { get; set; } = "";
        public string Message { get; set; } = "";
        public string Title { get; set; } = "";
        private string type { get; set; } = TYPE_SUCCESS;
        public string Type { 
            get {
                if (Number < 0 && (type == MgkMessage.TYPE_SUCCESS || type == MgkMessage.TYPE_INFO))
                {
                    type = MgkMessage.TYPE_WARNING;
                    return type;
                }
                if (Number >= 0 && type != MgkMessage.TYPE_SUCCESS && type != MgkMessage.TYPE_INFO)
                {
                    type = MgkMessage.TYPE_INFO;
                    return type;
                }
                return type;
            }
            set { type = value; } 
        }
        public string SubType { get; set; } = "";

        public string Message2 { get; set; } = "";
        public string Message3 { get; set; } = "";
        public string Messagex { get; set; } = "";

        public string Source { get; set; } = "";
        public string Exception { get; set; } = "";
        public object OData { get; set; } = null;
        public string _Mgk_secret { get; set; } = "";

        public string _MessageFull { get { return this.Message + " " + this.Message2 + " " + this.Message3; } }

        private static bool _clear { get; set; } = true;

        //public MgkMessage()
        //{
        //    clear();
        //}

        /// <summary>
        /// Limpiar los valores del objeto
        /// </summary>
        public void Clear()
        {
            Number = 0;
            Code = "";
            Message = "";
            Title = "";
            Type = TYPE_SUCCESS;
            SubType = "";

            Message2 = "";
            Message3 = "";
            Messagex = "";

            Source = "";
            Exception = "";
            OData = null;
            _Mgk_secret = "";


            if (MgkMessage._clear)
                return;
            MgkMessage._clear = true;

            MgkStaticMessage.Message.Number = 0;
            MgkStaticMessage.Message.Code = "";
            MgkStaticMessage.Message.Message = "";
            MgkStaticMessage.Message.Title = "";
            MgkStaticMessage.Message.Type = TYPE_SUCCESS;
            MgkStaticMessage.Message.SubType = "";

            MgkStaticMessage.Message.Message2 = "";
            MgkStaticMessage.Message.Message3 = "";
            MgkStaticMessage.Message.Messagex = "";

            MgkStaticMessage.Message.Source = "";
            MgkStaticMessage.Message.Exception = "";
            MgkStaticMessage.Message.OData = null;
            MgkStaticMessage.Message._Mgk_secret = "";
        }


        /// <summary>
        /// Cambiar el Message actual
        /// </summary>
        /// <param name="Message"></param>
        public MgkMessage SetMessage(MgkMessage myMessage)
        {
            MgkMessage._clear = false;
            if (myMessage.Number < 0 && (myMessage.Type == MgkMessage.TYPE_INFO || myMessage.Type == MgkMessage.TYPE_SUCCESS))
                myMessage.Type = MgkMessage.TYPE_DANGER;

            MgkStaticMessage.Message = (MgkMessage)myMessage.Clone();
            this.Number = myMessage.Number;
            this.Type = myMessage.Type;
            this.Code = myMessage.Code;
            this.Title = myMessage.Title;
            this.Message = myMessage.Message;
            this.Messagex = myMessage.Messagex == "" ? myMessage.Message : myMessage.Messagex;
            this.Message2 = myMessage.Message2;
            this.Source = myMessage.Source;
            this.Exception = myMessage.Exception;
            return MgkStaticMessage.Message;
        }

        public void SetError1(string Message, string Source = "", object OData = null)
        {
            this.SetMessage(new MgkMessage
            {
                Message = Message,
                Source = Source,
                OData = OData
            });
        }

        /// <summary>
        /// Devuelve el objeto en un string formato JSON
        /// </summary>
        /// <returns></returns>
        public string ToJson()
        {
            try
            {
                return Newtonsoft.Json.JsonConvert.SerializeObject(this);
            }catch(Exception)
            {
                try
                {
                    return Newtonsoft.Json.JsonConvert.SerializeObject(
                        new MgkMessage { 
                            Number=this.Number,
                            Message=this.Message,
                            Message2 = this.Message2,
                            Message3 = this.Message3,
                            Title =this.Title,
                            Code=this.Code,
                            Type=this.Type,
                            Messagex=this.Messagex,
                            Exception= this.Exception
                        }
                        );
                }
                catch (Exception)
                {
                    return "";
                }
            }
        }

        /// <summary>
        /// Crear una copia del objeto a traves de una conversion JSON
        /// </summary>
        /// <returns></returns>
        public MgkMessage GetCopy()
        {
            try
            {
                return Newtonsoft.Json.JsonConvert.DeserializeObject<MgkMessage>(Newtonsoft.Json.JsonConvert.SerializeObject(this));
            }
            catch (Exception e)
            {
                MgkLog.Error(new MgkMessage
                {
                    Number = -100000,
                    Source = "MgkMessage",
                    Message = "Erro al intentar copia del objeto",
                    Exception = e.ToString(),
                    OData = new
                    {
                        thisToJson = this.ToJson()
                    }
                });
            }
            return null;
        }

        /// <summary>
        /// Imprime en consola valores del objeto
        /// </summary>
        public void WriteLine()
        {
            Console.WriteLine("------------ MgkMessage-----------<<");
            Console.WriteLine("Number\t\t:" + this.Number);
            Console.WriteLine("Code\t\t:" + this.Code);
            Console.WriteLine("Message\t\t:" + this.Message);
            Console.WriteLine("Messagex\t:" + this.Messagex);
            Console.WriteLine("Exception\t:" + this.Exception);
            Console.WriteLine("------------ MgkMessage----------->>");
        }
    }
}
