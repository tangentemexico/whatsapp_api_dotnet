using System;
using System.Collections.Generic;
using System.Text;

namespace Mgk.Commonsx
{
    public class MgkResponseCode
    {
        public const int SYS_ERROR_UNKNOW = -100000;
        public const int SYS_ERROR_SETTINGS_READ = -100001;
        public const int SYS_EXCEPTION_UNKNOW = -190001;

        public const int DB_EXCEPTION_UNKNOW = -290000;
        public const int DB_ERROR_CONNECTION_READ = -200001;
        public const int DB_ERROR_CONNECTION_CONNECT = -200002;
        public const int DB_NOT_FOUND = -210000;

        public const int DB_EXCEPTION_ExecuteNonQuery = -290002;
        public const int DB_EXCEPTION_ExecuteScalarInt = -290003;
        public const int DB_EXCEPTION_ExecuteScalar = -290004;
        public const int DB_EXCEPTION_ExecuteReader = -290005;


        public const String SESSION_END = "SESSION_END";
        public const String SESSION_START = "SESSION_START";


        // RJB
        public static int PROCESS_CODE_EXITO = 0;
        public static int PROCESS_CODE_NO_EXITO = -1;
        public static int EXCEPTION_CODE = 0;
        public static String EXCEPTION_MSG = "SIN EXCEPCION";

        public static int CFDI_VERSION_NO_SOPORTADO = -300001;
        public static int CFDI_AUN_NO_VALIDADO = -300002;

        public static int DIRECTORIO_NO_EXISTE = -400001;

        public static int CORREO_EXCEPTION = -500001;
        public static int CORREO_SIN_CORREOS = -500002;

        public static int APP_SIN_LICENCIA = -600001;


    }
}
