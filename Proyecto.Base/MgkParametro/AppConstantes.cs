using Mgk.Commonsx;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mgk.Base.Parametro
{
    public class AppConstantes
    {
        /// <summary>
        /// Tiempo de espera sin actividad antes de invalidar el acceso
        /// </summary>
        public static int SESION_SEGUNDOS = 900;

        /// <summary>
        /// Permitir acceso mismo usuario desde diferentes lugares
        /// </summary>
        public static Boolean SESION_MULTIPLE = false;

        /// <summary>
        /// Nombre de sesion para guardar identificador de acceso
        /// </summary>
        public static String SESION_NOMBRE = "_MGK_SESS_";
        public static String SESION_NOMBRE_CAPTCHA = "_MGK_CAP_";
        public static String SESION_MSG_INICIO = "_MSG_INICIO_";

        public static string PARAM_LICENCIA = "_licencia_mgk_";

        public static MgkMessage MESSAGE_NO_PERMITIDO = new MgkMessage { Number = -600, Message = "Acceso no permitido" };

        public static MgkMessage MESSAGE_TERMINADO = new MgkMessage { Number = -601, Message = "Acceso terminado" };
        public static MgkMessage MESSAGE_ERROR = new MgkMessage { Number = -602, Message = "Error desconocido" };


        private static String ROUND { get; set; }
        private static int ROUND_ { get; set; }
        public static int _ROUND
        {
            get
            {
                if (ROUND != null)
                    return ROUND_;

                ROUND = MgkFunctions.AppSettings("ROUND", "2", true);
                ROUND_ = MgkFunctions.StrToInt(ROUND );
                return ROUND_;
            }
        }
    }
}
