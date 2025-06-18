using Mgk.Commonsx;
using Mgk.Base.ControlAcceso;
using Mgk.Base.HistoriaTabla;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mgk.Base.Parametro
{
    public class ParametroCtrl
    {
        private ParametroDao ParametroD { get; set; }

        public MgkMessage Message { get; set; }

        private static List<ParametroModel> ParametroList { get; set; }

        public ParametroCtrl()
        {
            Message = new MgkMessage();
            ParametroD = new ParametroDao();
        }
        public void ReCargarParametros()
        {
            CargarParametros(true);
        }


        public List<ParametroModel> LeerParametros(ParametroModel ParametroMod)
        {
            return (new ParametroDao()).GetItems(ParametroMod, new AccesoModel());
        }

        public List<ParametroModel> ConexionesSAP()
        {
            CargarParametros();
            return ParametroList.Where(x => x.Parametro_id.Length>3 && x.Parametro_id.Substring(0,4) == "sap_").ToList();
        }
              
        private static void CargarParametros(bool Recargar = false)
        {
            if (ParametroList == null)
            {
                //ParametroList = new List<ParametroModel>();
                Recargar = true;
            }
            //Recargar = true;
            if (Recargar)
            {
                // EnviarWhatsapp( MgkMessage.Source + LOG_SEPARATOR + MgkMessage.Message);
                MgkLog.Debug("ParametroCtrl", "CargarParametros");
                //MgkLog.DebugX("ParametroCtrl", "CargarParametros","PARAM");
                //ParametroList.Clear();
                ParametroList = (new ParametroDao()).GetItems(null,new AccesoModel());
                if (ParametroList == null)
                {
                    ParametroList = new List<ParametroModel>();
                }
            }               
        }

        public static String GetValueCurrent(String Clave, String Default = null)
        {
            ParametroDao dao = new ParametroDao();
            ParametroModel mod = new ParametroModel { Parametro_id = Clave };
            mod = dao.GetItem(mod, new AccesoModel());
            if (mod != null)
            {
                int indice = ParametroList.FindIndex(p => p.Parametro_id == Clave);
                if (indice >= 0)
                {
                    ParametroList[indice] = mod;
                }
                return mod.Valor;
            }                
            return Default;
        }

        public static ParametroModel GetParametro(String Clave)
        {
            CargarParametros();
            if (ParametroList.Exists(p => p.Parametro_id == Clave))
                return ParametroList.Find(p => p.Parametro_id == Clave);
            return null;
        }
        public static String GetValue(String Clave,String Default=null)
        {
            CargarParametros();
            if (ParametroList.Exists(p => p.Parametro_id == Clave))
                return ParametroList.Find(p => p.Parametro_id == Clave).Valor;
            return Default;
        }

        public static String GetSectreto(String Clave, String Default = null)
        {
            CargarParametros();
            String Secreto = Default;
            if (ParametroList.Exists(p => p.Parametro_id == Clave))
            {
                Secreto = ParametroList.Find(p => p.Parametro_id == Clave).Valor;
            }
            return Secreto;
        }

        public static String GetPath(String Clave, String Default = null)
        {
            CargarParametros();
            if (ParametroList.Exists(p => p.Parametro_id == Clave))
                Default = ParametroList.Find(p => p.Parametro_id == Clave).Valor;
            if ((Default ?? "") != "" && Default.Substring(Default.Length - 1, 1) != "\\")
                Default += "\\";
            return Default;
        }
		
		/*
        public static void SetValue(String Clave, String Valor, AccesoModel AccesoM)
        {
            CargarParametros();
            if (ParametroList != null && ParametroList.Exists(p => p.Parametro_id == Clave))
            {
                ParametroModel ParametroMod = ParametroList.Find(p => p.Parametro_id == Clave);
                ParametroMod.Valor = Valor;
                (new ParametroCtrl()).Update(ParametroMod, AccesoM);
            }
        }		*/

        public static String GetValuePassword(String Clave)
        {
            CargarParametros();
            if (ParametroList != null && ParametroList.Exists(p => p.Parametro_id == Clave))
                return ParametroList.Find(p => p.Parametro_id == Clave).Valor;
            return null;
        }

        public static Boolean? GetValueBool(String Clave, Boolean? Default = null)
        {
            CargarParametros();
            if (ParametroList.Exists(p => p.Parametro_id == Clave))
                return MgkFunctions.StrToBoolean(ParametroList.Find(p => p.Parametro_id == Clave).Valor);
            return Default;
        }

        public static int GetValueInt(String Clave, int Default = 0)
        {
            CargarParametros();
            if (ParametroList.Exists(p => p.Parametro_id == Clave))
                return MgkFunctions.StrToInt(ParametroList.Find(p => p.Parametro_id == Clave).Valor);
            return Default;
        }

        /// <summary>
        /// Antes de guardar veririfica si existe para hacer update o se trata de un registro nuevo
        /// </summary>
        /// <param name="ParametroMod"></param>
        /// <param name="AccesoM"></param>
        /// <returns></returns>
        public MgkMessage Save(ParametroModel ParametroMod, AccesoModel AccesoM)
        {
            ParametroModel ParametroModNew = (ParametroModel)ParametroD.ReadObject<ParametroModel>(ParametroMod, null, true);
            if (ParametroModNew != null && ParametroModNew.Parametro_id != "")
                return this.Update(ParametroMod, AccesoM);
            else
                return this.Insert(ParametroMod, AccesoM);
        }

        /// <summary>
        /// Insertar Parametro
        /// </summary>
        /// <param name="ParametroM"></param>
        /// <returns></returns>
        public MgkMessage Insert(ParametroModel ParametroM, AccesoModel AccesoM)
        {
            Message = ParametroD.Insert(ParametroM, AccesoM);

            if (Message.Number >= 0)
                HistoriaTablaCtrl.Insert(new HistoriaTablaModel
                {
                    Nombre = ParametroModel.__table_name,
                    Llave = ParametroM.Parametro_id,
                    Operacion = HistoriaTablaModel.OPERACION_INSERTAR
                }, AccesoM);
            CargarParametros(true);
            return Message;
        }

        /// <summary>
        /// Actualizar Parametro
        /// </summary>
        /// <param name="ParametroM"></param>
        /// <returns></returns>
        public MgkMessage Update(ParametroModel ParametroM, AccesoModel AccesoM)
        {
            Message = ParametroD.Update(ParametroM, AccesoM);
            if (Message.Number >= 0)
                HistoriaTablaCtrl.Insert(new HistoriaTablaModel
                {
                    Nombre = ParametroModel.__table_name,
                    Llave = ParametroM.Parametro_id,
                    Operacion = HistoriaTablaModel.OPERACION_ACTUALIZAR
                }, AccesoM);
            CargarParametros(true);
            return Message;
        }

        public MgkMessage UpdateValor(ParametroModel ParametroM, AccesoModel AccesoM)
        {
            Message = ParametroD.UpdateValor(ParametroM);
            if (Message.Number >= 0)
                HistoriaTablaCtrl.Insert(new HistoriaTablaModel
                {
                    Nombre = ParametroModel.__table_name,
                    Llave = ParametroM.Parametro_id,
                    Operacion = HistoriaTablaModel.OPERACION_ACTUALIZAR
                }, AccesoM);
            CargarParametros(true);
            return Message;
        }

        //

        /// <summary>
        /// Eliminar Parametro
        /// </summary>
        /// <param name="ParametroM"></param>
        /// <param name="ParametroM"></param>
        /// <returns></returns>
        public MgkMessage Delete(ParametroModel ParametroM, AccesoModel AccesoM)
        {
            Message = ParametroD.Delete(ParametroM, AccesoM);
            CargarParametros(true);
            if (Message.Number >= 0)
                HistoriaTablaCtrl.Insert(new HistoriaTablaModel
                {
                    Nombre = ParametroModel.__table_name,
                    Llave = ParametroM.Parametro_id,
                    Operacion = HistoriaTablaModel.OPERACION_BORRAR
                }, AccesoM);

            return Message;
        }


        public MgkMessage Get(ParametroModel ParametroM, AccesoModel AccesoM)
        {
            ParametroModel ParametroItem = ParametroD.GetItem(ParametroM, AccesoM);
            Message = ParametroD.Messages.GetLastMessage();
            Message.OData = ParametroItem;
            return Message;
        }


        /// <summary>
        /// Leer Lista de Parametros
        /// </summary>
        /// <param name="ParametroM"></param>
        /// <param name="ParametroM"></param>
        /// <returns></returns>
        public List<ParametroModel> GetAll(ParametroModel ParametroM, AccesoModel AccesoM)
        {
            var ParametroItems = ParametroD.GetItems(ParametroM, AccesoM);
            Message = ParametroD.Messages.GetLastMessage();
            return ParametroItems;
        }

        public List<ParametroModel> GetAll(AccesoModel AccesoM)
        {
            var ParametroItems = ParametroD.GetItems(new ParametroModel(), AccesoM);
            Message = ParametroD.Messages.GetLastMessage();
            return ParametroItems;
        }
    }
}