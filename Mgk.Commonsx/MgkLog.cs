using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Text;
using System.Threading;

namespace Mgk.Commonsx
{
    public class MgkLog
    {
        public enum Type { LOG, DEBUG, WARNING, ERROR,SQL,ERROR_DB,STEEP };
        public static String LOG_DIRECTORY = "0";
        public static String LOG_SEPARATOR = "\t";
        public static String LOG_FORMAT_NAME_DATETIME = "";
        private static string prefixName = "";

        public static String WRITE_LOG = "";
        public static String WRITE_STEEP = "";
        public static String WRITE_DEBUG = "";
        public static String WRITE_WARNING = "";
        public static String WRITE_ERROR = "";
        public static String WRITE_SQL = "";
        public static String WRITE_ERROR_DB = "";

        public static String CONSOLE_LOG = "";
        public static String CONSOLE_DEBUG = "";
        public static String CONSOLE_STEEP = "";
        public static String CONSOLE_WARNING = "";
        public static String CONSOLE_ERROR = "";

        public void Agregar(String Source, String Message, Type Yype)
        {
        }

        public static void Steep(String Source, String Message)
        {
            ReadWriteLog();
            if (WRITE_LOG == "1")
                Write(Message, Type.STEEP.ToString());
        }
        public static void Steep(MgkMessage MgkMessage)
        {
            ReadWriteLog();
            if (WRITE_ERROR == "1")
                Write(MgkMessage.ToJson(), Type.STEEP.ToString());
        }

        public static void Log(String Source, String Message)
        {
            ReadWriteLog();
            if (WRITE_LOG == "1")
                Write(Message, Type.LOG.ToString());
        }

        public static void Debug(String Source, String Message)
        {
            ReadWriteLog();
            if (WRITE_DEBUG== "1")
                Write(Message, Type.DEBUG.ToString());
        }

        public static void Error(String Source, String Message)
        {
            ReadWriteLog();
            if (WRITE_ERROR == "1")
                Write(Message, Type.ERROR.ToString());
        }

        public static void Error(MgkMessage MgkMessage)
        {
            ReadWriteLog();
            if (WRITE_ERROR == "1")
                Write(MgkMessage.ToJson(), Type.ERROR.ToString());
        }

        public static void ErrorDB(MgkMessage MgkMessage)
        {
            ReadWriteLog();
            if (WRITE_ERROR_DB == "1")
                Write(MgkMessage.ToJson(), Type.ERROR_DB.ToString());
        }

        public static void Sql(String Source, String Message)
        {
            ReadWriteLog();
            if (WRITE_SQL == "1")
                Write(Source + LOG_SEPARATOR + Message, Type.SQL.ToString());
        }

        public static void Sql(MgkMessage MgkMessage)
        {
            ReadWriteLog();
            if (WRITE_SQL=="1")
                Write(MgkMessage.ToJson(), Type.SQL.ToString());
        }

        public static void Warning(String Source, String Message)
        {
            ReadWriteLog();
            if (WRITE_WARNING == "1")
                Write(Message, Type.WARNING.ToString());
        }

        public static void Log(MgkMessage Message)
        {
            ReadWriteLog();
            if (WRITE_LOG == "1")
                Write(Message.ToJson(), Type.LOG.ToString());
        }

        public static void Debug(MgkMessage Message)
        {
            ReadWriteLog();
            if (WRITE_DEBUG == "1")
                Write(Message.ToJson(), Type.DEBUG.ToString());
        }

        public static void Warning(MgkMessage Message, bool bWrite = false)
        {
            ReadWriteLog();
            if (WRITE_WARNING == "1")
                Write(Message.ToJson(), Type.WARNING.ToString());
            if (bWrite)
                Message.WriteLine();
        }
       
        public static void ReadWriteLog() {
            if (WRITE_LOG == "")
            {
                try
                {
                    WRITE_LOG = MgkFunctions.AppSettings("Mgk.WRITE_LOG","0",true);
                    WRITE_STEEP = MgkFunctions.AppSettings("Mgk.WRITE_STEEP", "0", true);
                    WRITE_SQL = MgkFunctions.AppSettings("Mgk.WRITE_SQL", "0", true);
                    WRITE_DEBUG = MgkFunctions.AppSettings("Mgk.WRITE_DEBUG", "0", true);
                    WRITE_ERROR = MgkFunctions.AppSettings("Mgk.WRITE_ERROR", "0", true);
                    WRITE_ERROR_DB = MgkFunctions.AppSettings("Mgk.WRITE_ERROR_DB", "0", true);
                    WRITE_WARNING = MgkFunctions.AppSettings("Mgk.WRITE_WARNING", "0", true);

                    if (string.IsNullOrEmpty(WRITE_LOG))
                        WRITE_LOG = "0";
                    if (string.IsNullOrEmpty(WRITE_SQL))
                        WRITE_SQL = "0";
                    if (string.IsNullOrEmpty(WRITE_DEBUG))
                        WRITE_DEBUG = "0";
                    if (string.IsNullOrEmpty(WRITE_ERROR))
                        WRITE_ERROR = "0";
                    if (string.IsNullOrEmpty(WRITE_ERROR_DB))
                        WRITE_ERROR_DB = "0";
                    if (string.IsNullOrEmpty(WRITE_WARNING))
                        WRITE_WARNING = "0";
                }
                catch (Exception exx)
                {
                }
            }
        }

        private static void ReadLogDirectory()
        {
            MgkFunctions.AppSettings("");

            if (LOG_DIRECTORY != "0")
                return;
            try
            {
                CONSOLE_LOG = MgkFunctions.AppSettings("Mgk.ConsoleLog", "0", true);
                CONSOLE_DEBUG = MgkFunctions.AppSettings("Mgk.ConsoleDebug", "0", true);
                CONSOLE_STEEP = MgkFunctions.AppSettings("Mgk.ConsoleSteep", "0", true);
                CONSOLE_ERROR = MgkFunctions.AppSettings("Mgk.ConsoleError", "0", true);
                CONSOLE_WARNING = MgkFunctions.AppSettings("Mgk.ConsoleWarning", "0", true);
                LOG_FORMAT_NAME_DATETIME = MgkFunctions.AppSettings("Mgk.LogFormatNameDateTime", "", true);
                LOG_FORMAT_NAME_DATETIME = LOG_FORMAT_NAME_DATETIME ?? "";

                if (MgkFunctions.AppSettings("Mgk.LogDirectory", "", true) == "")
                {
                    Console.WriteLine("--- MyLog.leerLogDirectory(): Algo paso con la lectura de LOG_DIRECTORY");
                    LOG_DIRECTORY = "1";
                }
                else
                {
                    LOG_DIRECTORY = MgkFunctions.AppSettings("Mgk.LogDirectory", "", true);
                    LOG_SEPARATOR = MgkFunctions.AppSettings(MgkFunctions.WORK_ENVIRONMENT + "Mgk.LogSeparator", LOG_SEPARATOR);
                    if (!Directory.Exists(LOG_DIRECTORY))
                        Directory.CreateDirectory(LOG_DIRECTORY);
                }
            }
            catch (Exception)
            {
                LOG_DIRECTORY = "";
            }
        }

        /// <summary>
        /// Escribir en archivo
        /// </summary>
        /// <param name="line"></param>
        /// <param name="tipo"></param>
        public static void Write(String line, String type)
        {
            ReadLogDirectory();
            if (LOG_DIRECTORY.Equals("1") || LOG_DIRECTORY.Equals("2"))
                return;

            try
            {
                Thread thread = new Thread(() => {
                    try
                    {
                        if (LOG_FORMAT_NAME_DATETIME == "")
                            prefixName = "";
                        else
                            prefixName = DateTime.Now.ToString(LOG_FORMAT_NAME_DATETIME) + "_";
                        String fileName = LOG_DIRECTORY + prefixName +
                            (type.Equals("") ? "" : type + "_") +
                            ".txt";
                        StreamWriter sw = new StreamWriter(fileName, true);
                        sw.WriteLine(DateTime.Now.ToString("s") + "\t[" + type + "]" + LOG_SEPARATOR + line);
                        sw.Close();

                        if ((type == "LOG" && MgkFunctions.StrToBoolean(CONSOLE_LOG) ==true) ||
                        (type == "ERROR" && MgkFunctions.StrToBoolean(CONSOLE_ERROR) == true) ||
                        (type == "DEBUG" && MgkFunctions.StrToBoolean(CONSOLE_DEBUG) == true) ||
                        (type == "WARNING" && MgkFunctions.StrToBoolean(CONSOLE_WARNING) == true )
                        )
                            Console.WriteLine(type + ">>" + line);
                    }
                    catch (Exception )
                    {

                    }
                });
                thread.Start();
            }
            catch (Exception)
            {
                //DIRECTORY = "2";
            }
        }


        /// <summary>
        /// Escribir log en cualquier archivo especificado por el usuario
        /// </summary>
        /// <param name="FileName"></param>
        /// <param name="Line"></param>
        public static void WriteLog(String FileName, String Line)
        {
            try
            {
                Thread thread = new Thread(() => {
                    try
                    {
                        StreamWriter sw = new StreamWriter(FileName, true);
                        sw.WriteLine(Line);
                        sw.Close();
                    }
                    catch (Exception )
                    {

                    }
                });
                thread.Start();
            }
            catch (Exception)
            {
                //DIRECTORY = "2";
            }
        }

    }
}