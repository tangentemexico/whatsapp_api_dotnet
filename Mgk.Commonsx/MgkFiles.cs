using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mgk.Commonsx.Net6
{

    public class MgkFiles
    {
        public static int TARGET_NOT_EXITS = -101;
        public static int WRITE_ERROR = -201;
        public static int SUCCESS = 0;

        /// <summary>
        /// Escribir texto en ruta especifica
        /// </summary>
        /// <param name="path"></param>
        /// <param name="txt"></param>
        /// <returns></returns>
        public static int WriteString(string path, String txt)
        {
            try
            {
                StreamWriter sw = new StreamWriter(path, false);
                sw.WriteLine(txt);
                sw.Close();
            }
            catch (Exception ee203)
            {
                MgkLog.Error(new MgkMessage
                {
                    Exception = ee203.ToString(),
                    Number = WRITE_ERROR,
                    Message = "Error al escribir Texto en directorio",
                    Source = "MgkFiles.WriteString",
                    OData = new
                    {
                        path = path,
                    }
                });
                return -1;
            }

            return 0;
        }

        /// <summary>
        /// Escribir arreglo de bytes en ruta
        /// </summary>
        /// <param name="FullPath">Nombre completo del archivo</param>
        /// <param name="Bytes">Arreglo de bytes</param>
        /// <param name="Directory">Directorio opcional cuando el nombre completo no incluye la ruta</param>
        /// <param name="CreateDirectoryBase">Opcional, crear directorio</param>
        /// <returns></returns>
        public static int WriteAllBytes(string FullPath, byte[] Bytes,String Directory=null,Boolean CreateDirectoryBase=false)
        {
            try
            {
                if (Directory != null)
                    FullPath = Directory + FullPath;

                if (CreateDirectoryBase)
                    if (Directory != null)
                        CreateDirectory(Directory);
                    else 
                        CreateDirectory(System.IO.Path.GetDirectoryName(FullPath) + "\\");                        

                File.WriteAllBytes(FullPath, Bytes);
                return 0;
            }
            catch (Exception ee201)
            {
                MgkLog.Debug(new MgkMessage
                {
                    Exception = ee201.ToString(),
                    Number = WRITE_ERROR,
                    Message = "Error al escribir bytes en directorio",
                    Source = "MgkFiles.WriteAllBytes",
                    OData = new
                    {
                        FullPath = FullPath,
                        Directory= Directory,
                        CreateDirectoryBase= CreateDirectoryBase
                    }
                });
                return WRITE_ERROR;
            }
        }

        public static void CreateDirectory(String Directorio_nombre)
        {
            if (Directory.Exists(Directorio_nombre) == false)
            {
                Directory.CreateDirectory(Directorio_nombre);
            }
        }

        public static string ValidName(String FileName) 
        {
            string validChars = "qwertyuiopasdfghjklmnbvcxzPOIUYTREWQLKJHGFDSAMNBVCXZ1234567890_";
            string RetVal = "";
            if (FileName!=null)
                for(int i=0;i< FileName.Length;i++)
                {
                    if (validChars.IndexOf(FileName[i]) >= 0)
                        RetVal += FileName[i];
                }
            if (RetVal == "")
                RetVal = DateTime.Now.ToString("yyyy_MM_dd_HHmmss");
            return RetVal;
        }


        /// <summary>
        /// Contenido binario de un archivo
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static byte[] GetFile(string filePath)
        {
            return MgkFunctions.GetFile(filePath);
        }

        /// <summary>
        /// Lista de nombres de archivos dentro de un directorio
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static String[] GetFiles(string path)
        {
            return MgkFunctions.GetFiles(path);
        }

        public static MgkFilesModel CreateFilesModel(String FullName)
        {
            MgkFilesModel MgkFilesMo = new MgkFilesModel();
            MgkFilesMo._FullName = FullName;
            return MgkFilesMo;
        }

        public static String ReadAllText(string path)
        {
            try
            {
                return System.IO.File.ReadAllText(path);
            }
            catch (Exception ee205)
            {
                MgkLog.Error(new MgkMessage
                {
                    Exception = ee205.ToString(),
                    Number = WRITE_ERROR,
                    Message = "Error al LEER Texto en directorio",
                    Source = "MgkFiles.ReadAllText",
                    Messagex = path
                });
                return "";
            }
        }

        public static MgkFilesModel CreateFilesModel(String Path,String Name,String Extension)
        {
            MgkFilesModel MgkFilesMo = new MgkFilesModel();
            Path = Path ?? "";
            Name = Name ?? "";
            Extension = Extension ?? "";

            if (Path.Length > 0 && Path[Path.Length - 1] != '\\')
                Path += "\\";
            if (Extension.Length > 0 && Extension[0] != '.')
                Extension = "." + Extension;

            MgkFilesMo._FullName = Path+Name+Extension;
            return MgkFilesMo;
        }

        public static int Copy(String Archivo_origen, String Ruta_destino, bool overwrite = false)
        {
            return Copy(Archivo_origen, Ruta_destino, "", overwrite);
        }

        public static MgkMessage DeleteFiles(String Path) {
            MgkMessage Message = new MgkMessage();
            if (Directory.Exists(Path))
            {
                string[] files = MgkFiles.GetFiles(Path);
                if (files!=null)
                foreach (var itemf in files)
                    DeleteFile(itemf);
            }
            return Message;
        }

        public static MgkMessage DeleteFile(String Path)
        {
            MgkMessage Message = new MgkMessage();
            try
            {
                if (File.Exists(Path))
                {
                    File.Delete(Path);
                }
            }
            catch (Exception ex001)
            {
                MgkFilesModel MgkFilesMo = CreateFilesModel(Path);
                Message.Number = -201;
                Message.Message = "Error al intentar borrar archivo :" + MgkFilesMo._FileName;
                Message.Exception = ex001.ToString();
            }
            return Message;
        }

        public static MgkMessage DeleteDirectory(String Path)
        {
            MgkMessage Message = new MgkMessage();
            try
            {
                if (Directory.Exists(Path))
                {
                    Directory.Delete(Path);
                }
            }
            catch (Exception ex001)
            {                
                Message.Number = -2013;
                Message.Message = "Error al intentar borrar directorio :" + Path;
                Message.Exception = ex001.ToString();
            }
            return Message;
        }

        /// <summary>
        /// Copiar archivo hacia el directorio destino, nombre de archivo con prefijo definido
        /// </summary>
        /// <param name="Archivo_origen"></param>
        /// <param name="Ruta_destino"></param>
        /// <param name="Prefijo"></param>
        public static int Copy(String Archivo_origen, String Ruta_destino, String Prefijo = "", bool overwrite = false)
        {
            try
            {
                if (File.Exists(Archivo_origen))
                {
                    CreateDirectory(Ruta_destino);
                    MgkFilesModel ArchivoMo = CreateFilesModel(Archivo_origen);

                    if (Prefijo != "")
                        ArchivoMo.FileName = Prefijo + "_" + ArchivoMo.FileName;
                    ArchivoMo.TrgtPath = Ruta_destino;
                    File.Copy(Archivo_origen, ArchivoMo._FullName, overwrite);
                    return 0;
                }
                return -1;
            }
            catch (Exception ee202)
            {
                MgkLog.Error(new MgkMessage
                {
                    Exception = ee202.ToString(),
                    Code = "EX-COPY",
                    Number = -202,
                    Source = "MgkFiles.Copy",
                    Message = "Error al copiar archivo",
                    Message2 = "Archivo_origen:" + Archivo_origen,
                    Message3 = "Ruta_destino:" + Ruta_destino,
                    Messagex = "Prefijo:" + Prefijo
                });
                return -202;
            }
        }

        public static byte[] StreamToByteArray(Stream st)
        {
            try
            {
                st.Seek(0, SeekOrigin.Begin);
                byte[] bytes = new byte[st.Length];
                st.Read(bytes, 0, bytes.Length);
                st.Seek(0, SeekOrigin.Begin);
                return bytes;
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Mover archivo hacia el directorio destino, nombre de archivo con prefijo definido
        /// </summary>
        /// <param name="Archivo_origen"></param>
        /// <param name="Directorio_destino"></param>
        /// <param name="Prefijo"></param>
        public static int MoveTo(String Archivo_origen, String Directorio_destino, String Prefijo = "", bool overwrite = false)
        {
            try
            {
                if (File.Exists(Archivo_origen))
                {
                    CreateDirectory(Directorio_destino);
                    MgkFilesModel ArchivoMo = CreateFilesModel(Archivo_origen);

                    if (Prefijo != "")
                        ArchivoMo.FileName = Prefijo + "_" + ArchivoMo.FileName;
                    ArchivoMo.TrgtPath = Directorio_destino;
                    return MgkFiles.Move(Archivo_origen, ArchivoMo._FullName, overwrite);
                }
                return -1;
            }
            catch (Exception ee202)
            {
                MgkLog.Error(new MgkMessage
                {
                    Exception = ee202.ToString(),
                    Code = "EX-MOVETO",
                    Number = -202,
                    Source = "JuxFiles.MoveTo",
                    Message = "Error al mover archivo",
                    Message2 = "Archivo_origen:" + Archivo_origen,
                    Message3 = "Directorio_destino:" + Directorio_destino,
                    Messagex = "Prefijo:" + Prefijo
                });
                return -202;
            }
        }

        public static int Move(String Archivo_origen, String Archivo_destino, bool overwrite = false)
        {
            try
            {
                if (File.Exists(Archivo_origen))
                {
                    MgkFilesModel ArchivoMo = CreateFilesModel(Archivo_destino);
                    CreateDirectory(ArchivoMo.TrgtPath);

                    if (File.Exists(Archivo_destino) && overwrite)
                    {
                        File.Delete(Archivo_destino);
                    }

                    File.Move(Archivo_origen, Archivo_destino);
                    return 0;
                }
                return -1;
            }
            catch (Exception ee202)
            {
                MgkLog.Error(new MgkMessage
                {
                    Exception = ee202.ToString(),
                    Code = "EX-COPY",
                    Number = -202,
                    Source = "MgkFiles.Move",
                    Message = "Error al mover archivo",
                    Message2 = "Archivo_origen:" + Archivo_origen,
                    Message3 = "Archivo_destino:" + Archivo_destino,
                });
                return -202;
            }
        }
    }
}