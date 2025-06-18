using Mgk.Base.Grupou;
using Mgk.Base.Menu;
using Mgk.Base.Modulo;
using Mgk.Commonsx;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mgk.Base.Usuario
{
    public class UsuarioModel
    {
        public static string __table_name { get; } = "_Usuario";
        public static string __view_name { get; } = "_v_usuario";

        public static string[] __table_keys { get; } = { "Usuario_id" };
        public static bool __auto_increment { get; } = false;

        public String Usuario_id { get; set; }
        public String Password { get; set; }
        public String Password_tmp { get; set; }
        public String Nombre { get; set; }
        public String Email { get; set; }
        public String Telefono { get; set; }
        public Boolean? Es_activo { get; set; }

        public int EmpleadoId { get; set; }
        public String EmpleadoClave { get; set; }

        public String Externo_id { get; set; }
        public String Externo_id2 { get; set; }

        public Boolean? Aviso_aceptado { get; set; }
        public DateTime? Fecha_aviso_aceptado { get; set; }
        public DateTime? Fecha_aviso_rechazado { get; set; }

        public DateTime? Fecha_ultimo_ingreso { get; set; }
        public DateTime? Fecha_ultimo_intento {  get; set; }    
        public int Intentos_fallidos {  get; set; } 


        /// <summary>
        /// Auxiliar para identificar el menu que se requiere desplegar
        /// </summary>
        public String _Aux_menu { get; set; }

        /// <summary>
        /// Auxiliar para identificar quien es el front que invica al back
        /// </summary>
        public String _Aux_front { get; set; }
        public String _Device { get; set; }
        public String _Token_push { get; set; }
        public String _Grupo { get; set; }



        public String _Password{
            get { return this.Password; }
            set { if (value!=null && value!="")this.Password = MgkFunctions.Md5(value); }
        }

        public List<GrupouModel> _GrupouList { get; set; }
        public List<MenuModel> _MenuList { get; set; }
        public List<ModuloModel> _ModuloList { get; set; }

    }
}
