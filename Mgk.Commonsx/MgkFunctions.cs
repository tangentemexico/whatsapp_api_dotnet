using System;
using System.Collections.Generic;
//using System.Configuration;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Data.Common;
using System.IO;
using System.Reflection;

namespace Mgk.Commonsx
{
    public class MgkFunctions
    {
        public static string WORK_ENVIRONMENT = null;
        private static string ConfigJSON = null;
        private static Dictionary<string, object> ConfigDict = null;

        public static string MD5Hash(string input)
        {
            using (var md5 = MD5.Create())
            {
                var result = md5.ComputeHash(Encoding.ASCII.GetBytes(input));
                return Encoding.ASCII.GetString(result);
            }
        }

        public static string Reverse(string s)
        {
            char[] charArray = s.ToCharArray();
            Array.Reverse(charArray);
            return new string(charArray);
        }

        public static string Md5i2(string str)
        {
            string md5i = Md5(str);
            md5i = Md5(Reverse(md5i));
            return Reverse(md5i);
        }

        public static string Md5(string str, int tot) {
            if (tot<0)
                return str;
            String RetVal = str;
            for (int i = 0; i < tot && i < 20; i++)
                RetVal = Md5(RetVal);
            return RetVal;
        }
        public static string Md5(string str)
        {
            MgkStaticMessage.Clear();
            try
            {
                MD5 md5 = MD5CryptoServiceProvider.Create();
                ASCIIEncoding encoding = new ASCIIEncoding();
                byte[] stream = null;
                StringBuilder sb = new StringBuilder();
                stream = md5.ComputeHash(encoding.GetBytes(str));
                for (int i = 0; i < stream.Length; i++) sb.AppendFormat("{0:x2}", stream[i]);
                return sb.ToString();
            }
            catch (Exception e)
            {
                MgkLog.Warning(new MgkMessage
                {
                    Source = "MgkFunctions.MD5",
                    Exception = e.ToString(),
                    OData = new { str = str }
                });
                return "";
            }
        }

        public static bool EsNumeroEntero(String Cadena)
        {
            if (Cadena == null)
                return false;
            Regex patronNumerico = new Regex("[^0-9]");
            return !patronNumerico.IsMatch(Cadena);
        }

        public static bool EsNumeroFlotante(String Cadena)
        {
            if (Cadena == null)
                return false;
            Regex patronNumerico = new Regex(@"^(\d|-)?(\d|,)*\.?\d*$");
            return patronNumerico.IsMatch(Cadena);
        }

        public bool EsAlfabetico(String Cadena)
        {
            Regex patronAlfabetico = new Regex("[^a-zA-Z]");
            return !patronAlfabetico.IsMatch(Cadena);
        }

        public bool EsAlfanumerico(String Cadena)
        {
            if (Cadena == null)
                return false;
            Regex patronAlfanumerico = new Regex("[^a-zA-Z0-9]");
            return !patronAlfanumerico.IsMatch(Cadena);
        }

        public static object StrToDynamic(String strValue, String TargetType)
        {
            if (TargetType == null)
                return strValue;
            switch (TargetType.ToLower())
            {
                case "int":
                    return StrToInt(strValue);
                case "decimal":
                    return StrToDecimal(strValue);
                case "double":
                    return StrToDouble(strValue);
                case "datetime":
                    return StrToDateTime(strValue);
                case "boolean":
                    return StrToBoolean(strValue);
                case "float":
                    return StrToFloat(strValue);
                default:
                    return strValue;
            }
        }

        /// <summary>
        /// Convertir Texto a Entero
        /// </summary>
        /// <param name="valor"></param>
        /// <returns></returns>
        public static int StrToInt(String strVal, int defaultValor = 0)
        {
            MgkStaticMessage.Clear();
            try
            {
                if (strVal != null && strVal != "")
                    defaultValor = Int32.Parse(strVal);
            }
            catch (Exception e)
            {
                MgkLog.Warning(new MgkMessage
                {
                    Number = -120000,
                    Code = "EX-WAR",
                    Source = "MgkFunctions.StrToInt",
                    Exception = e.ToString(),
                    OData = new
                    {
                        strVal = strVal
                    }
                });
            }
            return defaultValor;
        }

        /// <summary>
        /// Convierte un straing a un valor decimal
        /// </summary>
        /// <param name="strInt">Valor a convertir</param>
        /// <param name="name">Nombre de la variable origen para rastraear error</param>
        /// <param name="defaultValue">Valor en caso de error</param>
        /// <returns></returns>
        public static Decimal StrToDecimal(String strVal, String name = "", Decimal defaultValue = 0)
        {
            MgkStaticMessage.Clear();
            try
            {
                if (strVal != null && strVal != "")
                    defaultValue = (Decimal)Convert.ToDecimal(strVal);
            }
            catch (Exception e)
            {
                MgkLog.Warning(new MgkMessage
                {
                    Number = -120000,
                    Code = "EX-WAR",
                    Source = "MgkFunctions.StrToDecimal",
                    Exception = e.ToString(),
                    OData = new
                    {
                        strVal = strVal,
                        name = name
                    }
                });
                defaultValue = 0;
            }
            return defaultValue;
        }

        public static Double StrToDouble(String strVal, String name = "", Double defaultValue = 0)
        {
            MgkStaticMessage.Clear();
            try
            {
                if (strVal != null && strVal != "")
                    defaultValue = (Double)Convert.ToDouble(strVal);
            }
            catch (Exception e)
            {
                MgkLog.Warning(new MgkMessage
                {
                    Number = -120000,
                    Code = "EX-WAR",
                    Source = "MgkFunctions.StrToDouble",
                    Exception = e.ToString(),
                    OData = new
                    {
                        strVal = strVal,
                        name = name
                    }
                });
                defaultValue = 0;
            }
            return defaultValue;
        }

        public static bool BStrToDateTime(DateTime dateTime, string sDateTime)
        {
            MgkStaticMessage.Clear();
            try
            {
                dateTime = DateTime.Parse(sDateTime);
                return true;
            }
            catch (Exception e)
            {
                MgkLog.Warning(new MgkMessage
                {
                    Number = -120000,
                    Code = "EX-WAR",
                    Source = "MgkFunctions.bStrToDateTime",
                    Exception = e.ToString(),
                    OData = new
                    {
                        dateTime = dateTime,
                        sDateTime = sDateTime

                    }
                });
                return false;
            }
        }

        public static DateTime StrToDateTime(string sDateTime, DateTime? DefaultValue =null)
        {
            MgkStaticMessage.Clear();
            try
            {
                if (string.IsNullOrEmpty(sDateTime)==false)
                    return DateTime.Parse(sDateTime);
                else 
                    return DefaultValue==null?DateTime.Parse("1900-01-01"): DefaultValue.Value;
            }
            catch (Exception e)
            {
                MgkLog.Warning(new MgkMessage
                {
                    Number = -120000,
                    Code = "EX-WAR",
                    Source = "MgkFunctions.StrToDateTime",
                    Exception = e.ToString(),
                    OData = new
                    {
                        sDateTime = sDateTime
                    }
                });
                //return new DateTime();
                return DefaultValue == null ? DateTime.Parse("1900-01-01") : DefaultValue.Value;
            }
        }

        public static DateTime? StrToDateTimeNull(string sDateTime)
        {
            MgkStaticMessage.Clear();
            try
            {
                if (string.IsNullOrEmpty(sDateTime) == false)
                    return DateTime.Parse(sDateTime);
                else
                    return null;
            }
            catch (Exception e)
            {
                MgkLog.Warning(new MgkMessage
                {
                    Number = -120000,
                    Code = "EX-WAR",
                    Source = "MgkFunctions.StrToDateTime",
                    Exception = e.ToString(),
                    OData = new
                    {
                        sDateTime = sDateTime
                    }
                });
                return null;
            }
        }

        public static Boolean StrToBoolean(String strBool, bool vnull)
        {
            return strBool == null ? vnull : StrToBoolean(strBool);
        }

        public static String Substring(String Str,int Begin,int Length)
        {
            if (Str == null)
                return "";
            if (Length < 0)
                Length = 1;
            if (Begin+ Length >= Str.Length)
                Length = Str.Length - Begin-1;
            if (Begin < 0)
                Begin = 0;
            return Str.Substring(Begin, Length);
        }

        public static Boolean StrToBoolean(String strBool)
        {
            if (strBool == null)
                return false;
            return strBool.ToUpper().Equals("TRUE") || strBool.ToUpper().Equals("1");
        }

        public static float StrToFloat(String strVal, String name = "", float retval = 0)
        {
            MgkStaticMessage.Clear();
            try
            {
                if (strVal != null && strVal != "")
                    retval = (float)Convert.ToDouble(strVal);
            }
            catch (Exception e)
            {
                MgkLog.Warning(new MgkMessage
                {
                    Number = -120000,
                    Code = "EX-WAR",
                    Source = "MgkFunctions.StrToFloat",
                    Exception = e.ToString(),
                    OData = new
                    {
                        strVal = strVal,
                        name = name
                    }
                });
                retval = 0;
            }
            return retval;
        }

        public static string Base64DeCode(string base64EnCodedData)
        {
            try
            {
                var base64EnCodedBytes = System.Convert.FromBase64String(base64EnCodedData);
                return System.Text.Encoding.UTF8.GetString(base64EnCodedBytes);
            }
            catch(Exception ex)
            {
                return "";
            }
        }

        public static string Base64EnCode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }

        /// <summary>
        /// Leer nombre de ambiente de trabajo
        /// </summary>
        public static string ReadWorkEnvironment()
        {
            return AppSettings("Jux.workEnvironment");
            //if (MgkFunctions.WORK_ENVIRONMENT == null)
            //{
            //    MgkFunctions.WORK_ENVIRONMENT = ConfigurationManager.AppSettings["Mgk.workEnvironment"];
            //    if (MgkFunctions.WORK_ENVIRONMENT != null && MgkFunctions.WORK_ENVIRONMENT.Trim() != "")
            //        MgkFunctions.WORK_ENVIRONMENT = MgkFunctions.WORK_ENVIRONMENT.Trim() + ".";
            //}
            //if (MgkFunctions.WORK_ENVIRONMENT == null)
            //    MgkFunctions.WORK_ENVIRONMENT = "";
            //return MgkFunctions.WORK_ENVIRONMENT;
        }

        private static void SetWorkEnvironment(string env)
        {
            if (MgkFunctions.WORK_ENVIRONMENT == null)
                MgkFunctions.WORK_ENVIRONMENT = env ?? "";
        }
        public static string ReadAllText(String FileName)
        {
            return ReadAllText(FileName, Encoding.UTF8);
        }
        public static string ReadAllText(String FileName, Encoding encoding)
        {
            try
            {
                if (File.Exists(FileName))
                    return File.ReadAllText(FileName, encoding);
            }
            catch
            {

            }
            return null;
        }

        /// <summary>
        /// Obtener valor en archivo de configuraciones
        /// </summary>
        /// <param name="nombre"></param>
        /// <param name="noexiste"></param>
        /// <param name="useWorkEnvironment">Usar variables por ambiente de trabajo</param>
        /// <returns></returns>
        public static string AppSettings(string nombre, string noexiste = null, bool useWorkEnvironment = false)
        {
            MgkStaticMessage.Clear();
            string valor = noexiste;
            string env = "";
            try
            {
                //Console.WriteLine("AppSettings. Variable={0}", nombre);
                if (ConfigDict == null && ConfigJSON == null)
                {
                    string BaseDirectory = AppContext.BaseDirectory;
                    MgkFilesModel configFile = new MgkFilesModel();
                    configFile.TrgtPath = AppContext.BaseDirectory;
                    configFile._FileName = "appsettings.json";

                    //string ConfigFileName = BaseDirectory + Path.DirectorySeparatorChar + "appsettings.json";
                    Console.WriteLine("BaseDirectory:" + (BaseDirectory ?? "-NULL-"));
                    Console.WriteLine("ConfigFileName:" + (configFile._FullName ?? "-NULL-"));
                    //ConfigJSON = ReadAllText(ConfigFileName);
                    try
                    {
                        if (File.Exists(configFile._FullName))
                        {
                            ConfigJSON = File.ReadAllText(configFile._FullName, Encoding.UTF8);
                            Console.WriteLine(ConfigJSON);
                        }                            
                        else
                            Console.WriteLine("Archivo Configuracion no encontrado:" + configFile._FullName);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error en lectura del archivo de configuracion");
                        Console.WriteLine(ex.ToString());
                    }

                    ConfigJSON = ConfigJSON ?? "";

                    ConfigDict = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(ConfigJSON);

                    if (ConfigDict != null)
                    {
                        ConfigDict.Add("_BaseDirectory_", BaseDirectory);
                        ConfigDict.Add("_DirectorySeparatorChar_", Path.DirectorySeparatorChar);
                        ConfigDict.Add("_IsLinux_", (OperatingSystem.IsLinux() ? "Si" : "No"));
                        ConfigDict.Add("_IsWindows_", (OperatingSystem.IsWindows() ? "Si" : "No"));
                        ConfigDict.Add("_IsMacOS_", (OperatingSystem.IsMacOS() ? "Si" : "No"));

                        if (ConfigDict.ContainsKey("Mgk.workEnvironment"))
                            env = ConfigDict["Mgk.workEnvironment"].ToString();
                        SetWorkEnvironment(env);
                    }
                }

                if (ConfigDict != null)
                {
                    env = "";
                    if (MgkFunctions.WORK_ENVIRONMENT != "" && useWorkEnvironment)
                        env = MgkFunctions.WORK_ENVIRONMENT + ".";
                    nombre = env + nombre;
                    if (ConfigDict.ContainsKey(nombre))
                        valor = ConfigDict[nombre].ToString();

                }

                //Console.WriteLine("AppSettings. valor={0}", valor);
                return valor;

                //if (useWorkEnvironment)
                //    nombre = ReadWorkEnvironment() + nombre;
                //valor = ConfigurationManager.AppSettings[nombre];
                //if (valor == null)
                //    valor = noexiste;
            }
            catch (Exception e)
            {
                MgkLog.Warning(new MgkMessage
                {
                    Number = -120000,
                    Code = "EX-WAR",
                    Source = "MgkFunctions.AppSettings",
                    Exception = e.ToString(),
                    OData = new
                    {
                        nombre = nombre,
                        noexiste = noexiste
                    }
                });
                valor = noexiste;
            }
            return valor;
        }

        public static object GetFromDicctionary(object dictionary, string key, object notFounded = null)
        {
            MgkStaticMessage.Clear();
            try
            {
                Dictionary<string, object> dic = (Dictionary<string, object>)dictionary;
                if (dic.ContainsKey(key))
                    return dic[key];
            }
            catch (Exception ex)
            {
                MgkStaticMessage.Message.SetMessage(new MgkMessage
                {
                    Number = -120000,
                    Code = "EX-WAR",
                    Source = "MgkFunctions.GetFromDicctionary",
                    Exception = ex.ToString(),
                    OData = new
                    {
                        dictionary = dictionary,
                        key = key,
                        notFounded = notFounded
                    }
                });
                MgkLog.Warning(MgkStaticMessage.Message);
            }
            return notFounded;
        }

        public static Dictionary<string, object> ObjectToDicctionary(Object oObject)
        {
            MgkStaticMessage.Clear();
            try
            {
                string txt = Newtonsoft.Json.JsonConvert.SerializeObject(oObject);
                Dictionary<string, object> dicx = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(txt);
                return dicx;
            }
            catch (Exception e)
            {
                MgkLog.Error(new MgkMessage
                {
                    Number = -100000,
                    Message = "Error al convertir Objeto en Diccionario",
                    Exception = e.ToString(),
                    OData = new
                    {
                        oObject = oObject
                    }
                });
            }
            return null;
        }


        public static string StrRequestFromObject(Object oObject)
        {
            MgkStaticMessage.Clear();
            StringBuilder strRequest = new StringBuilder("");
            try
            {
                Dictionary<string, object> dictio = MgkFunctions.ObjectToDicctionary(oObject);
                int i = 1;
                foreach (var item in dictio)
                {
                    if (item.Value == null)
                        continue;
                    if (item.Value is DateTime)
                        strRequest.Append(string.Format("{0}={1}", item.Key, ((DateTime)item.Value).ToString("yyyy-MM-dd")));
                    else
                        strRequest.Append(string.Format("{0}={1}", item.Key, item.Value.ToString()));
                    if (i++ < dictio.Count)
                        strRequest.Append("&");
                }
            }
            catch (Exception e)
            {
                MgkLog.Error(new MgkMessage
                {
                    Number = -100000,
                    Message = "Error al convertir Objeto stringRequest",
                    Exception = e.ToString(),
                    Messagex = "strRequestFromObject",
                    OData = new
                    {
                        oObject = oObject
                    }
                });
            }
            return strRequest.ToString();
        }

        public static byte[] GetFile(string filePath)
        {
            MgkStaticMessage.Clear();
            try
            {
                System.IO.FileStream fs = System.IO.File.OpenRead(filePath);
                byte[] data = new byte[fs.Length];
                int br = fs.Read(data, 0, data.Length);
                if (br != fs.Length)
                    throw new System.IO.IOException(filePath);
                fs.Close();
                return data;
            }
            catch (Exception e)
            {
                MgkLog.Error(new MgkMessage
                {
                    Number = -100000,
                    Message = "Error al obtener bytes del archivo",
                    Exception = e.ToString(),
                    Messagex = "GetFile",
                    OData = new
                    {
                        filePath = filePath
                    }
                });
            }
            return null;
        }

        //public static String ColumnToString(DbDataReader oDbDataReader, String nombreColumna)
        //{
        //    String sRetVal;

        //    if (!oDbDataReader.IsDBNull(oDbDataReader.GetOrdinal(nombreColumna)))
        //        sRetVal = oDbDataReader[nombreColumna].ToString();
        //    else
        //        sRetVal = String.Empty;

        //    return sRetVal;
        //}

        //public static DateTime ColumnToDateTime(DbDataReader oDbDataReader, String nombreColumna, DateTime Fecha)
        //{
        //    DateTime sRetVal = Fecha;

        //    if (!oDbDataReader.IsDBNull(oDbDataReader.GetOrdinal(nombreColumna)))
        //        sRetVal = StrToDateTime(oDbDataReader[nombreColumna].ToString());

        //    return sRetVal;
        //}

        //public static int ColumnToInt(DbDataReader oDbDataReader, String nombreColumna)
        //{
        //    int iRetVal;

        //    if (!oDbDataReader.IsDBNull(oDbDataReader.GetOrdinal(nombreColumna)))
        //        iRetVal = Convert.ToInt32(oDbDataReader[nombreColumna]);
        //    else
        //        iRetVal = 0;

        //    return iRetVal;
        //}

        //public static decimal ColumnToDecimal(DbDataReader oDbDataReader, String nombreColumna)
        //{
        //    decimal iRetVal;

        //    if (!oDbDataReader.IsDBNull(oDbDataReader.GetOrdinal(nombreColumna)))
        //        iRetVal = Convert.ToDecimal(oDbDataReader[nombreColumna]);
        //    else
        //        iRetVal = 0;

        //    return iRetVal;
        //}

        //public static Double ColumnToDouble(DbDataReader oDbDataReader, String nombreColumna)
        //{
        //    Double iRetVal;

        //    if (!oDbDataReader.IsDBNull(oDbDataReader.GetOrdinal(nombreColumna)))
        //        iRetVal = Convert.ToDouble(oDbDataReader[nombreColumna]);
        //    else
        //        iRetVal = 0;

        //    return iRetVal;
        //}

        public static String[] GetFiles(string path)
        {
            String[] files = null;

            if (Directory.Exists(path))
            {
                // This path is a directory
                files = Directory.GetFiles(path);
            }
            else
            {
                Console.WriteLine("{0} is not a valid file or directory.", path);
            }

            return files;
        }
		
        public static System.IO.Stream GetStream(byte[] Bytes)
        {
            try
            {
                System.IO.Stream stream = new MemoryStream(Bytes);
                return stream;
            }
            catch (Exception )
            {

            }
            return null;
        }

        public static byte[] GetBytes(System.IO.Stream stream)
        {
            try
            {
                byte[] Bytes = new byte[stream.Length];
                stream.Read(Bytes, 0, Bytes.Length);
                stream.Seek(0, SeekOrigin.Begin);
                return Bytes;
            }
            catch (Exception )
            {

            }
            return null;
        }

        public static Decimal TruncateDecimal(decimal dec, int tot)
        {
            string sdec = dec.ToString();
            string[] tmp = sdec.Split('.');
            if (tmp.Length > 1 && tot > 0)
            {
                if (tot < tmp[1].Length)
                    tmp[1] = tmp[1].Substring(0, tot);
                else
                    tmp[1] = tmp[1] + new String('0', tot - tmp[1].Length);

                sdec = string.Join(".", tmp);
                dec = StrToDecimal(sdec);
            }
            else if (tot > 0)
            {
                sdec += "." + new String('0', tot);
                dec = StrToDecimal(sdec);
            }

            return dec;
        }

        public static string Replace(String Origen, string[] Buscar, string[] Reemplazar)
        {
            try
            {
                for (int i = 0; i < Buscar.Length && i < Reemplazar.Length; i++)
                    Origen = Origen.Replace(Buscar[i], Reemplazar[i]);
            }
            catch (Exception)
            {

            }
            return Origen;
        }

        public static string ReplaceFor(String Origen, string Buscar_, string Reemplazar_,String Separator="|")
        {
            string[] Buscar = Buscar_.Split(Separator);
            string[] Reemplazar = Reemplazar_.Split(Separator);
            return Replace(Origen, Buscar, Reemplazar);
        }

        public static string Replace(String Origen, Object oObject,String Prefijo="{",String Sufijo="}")
        {
            try
            {
                Type t = oObject.GetType();
                String tmpName = "";
                object PropertyValue = null;

                System.Reflection.PropertyInfo[] properties = t.GetProperties();
                foreach (System.Reflection.PropertyInfo p in properties)
                {
                    PropertyInfo propertyInfo = oObject.GetType().GetProperty(p.Name);
                    PropertyValue = propertyInfo.GetValue(oObject, null);
                    tmpName = p.Name;

                    if (PropertyValue != null)
                        Origen = Origen.Replace(Prefijo + p.Name + Sufijo, PropertyValue.ToString());
                    else
                        Origen = Origen.Replace(Prefijo + p.Name + Sufijo, "" );
                }

                //   for (int i = 0; i < Buscar.Length && i < Reemplazar.Length; i++)
                //Origen = Origen.Replace(Buscar[i], Reemplazar[i]);
            }
            catch (Exception)
            {

            }
            return Origen;
        }


        /// <summary>
        /// Devuelve fecha actual formato YYYY-MM-DD
        /// </summary>
        /// <returns></returns>
        public static string CurrentDateYMD() {
            return DateTime.Now.ToString("s").Substring(0, 10);
        }

        public static string DateToYMD(DateTime? Dtime=null)
        {
            if (Dtime==null)
                return DateTime.Now.ToString("s").Substring(0, 10);
            else
                return Dtime.Value.ToString("s").Substring(0, 10);
        }

        public static string NewGuid()
        {
            Guid g = Guid.NewGuid();
            return g.ToString();
        }

        public static string NameXfromDateTime()
        {
            return DateTime.Now.ToString("s").Replace(" ", "").Replace(":", "").Replace("-", "").Replace(".", "");
        }

        public static int RandomLength(int Length)
        {
            int x = 0;
            string sx = "";
            Random ran = new Random();
            for (int i = 0; i < Length; i++)
                sx += ran.Next().ToString().Substring(0, 1);
            x = MgkFunctions.StrToInt(sx);
            sx = (x * DateTime.Now.Millisecond).ToString().Substring(0,Length);
            x = MgkFunctions.StrToInt(sx);

            return x;
        }

        public static Dictionary<string, object> GetAllSettings()
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                if (ConfigDict!=null)
                foreach (var item in ConfigDict)
                {
                    sb.Append(String.Format("{0}\t{1}", item.Key, item.Value));
                }
                MgkLog.Debug("GetAllSettings\n", sb.ToString());
            }
            catch (Exception ex)
            {
                MgkLog.Error("GetAllSettings\n", "Error inesperado, obtener todos los parametros." + ex.ToString());
            }
            return ConfigDict;
        }

    }
}
