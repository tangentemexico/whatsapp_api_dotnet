using Mgk.Base.Menu;
using Mgk.Base.Modulo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mgk.Base.Grupou
{
    public class GrupouModel
    {
        public static string __r_usuario { get; } = "_r_grupou_usuario";
        public static string __table_name { get; } = "_Grupou";
        public static string[] __table_keys { get; } = { "Grupou_code" };
        public static bool __auto_increment { get; } = false;

        public String Grupou_code { get; set; }
        public String Nombre { get; set; }
        public Boolean? Es_activo { get; set; }
        public Boolean? Acceso_movil { get; set; }
        public Boolean? Acceso_web { get; set; }

        public List<MenuModel> _ListaMenu { get; set; }

        public List<ModuloModel> _ListaModulo { get; set; }


        public static Boolean Existe(String Codigo, List<GrupouModel> GrupoLista)
        {
            Boolean RetVal = false;
            string[] CodigoArray = Codigo.Split(',');
            foreach(string ItemCod in CodigoArray)
            {
                if (GrupoLista != null && GrupoLista.Count > 0)
                {
                    RetVal = GrupoLista.Exists(item => item.Es_activo == true && item.Grupou_code.ToLower() == ItemCod.ToLower());
                    if (RetVal == true)
                        return RetVal;
                }
            }
            return RetVal;
        }

    }
}

/**
 * 
create table _Grupou (
Grupou_id int not null primary key identity,
Titulo varchar(50) not null,
Descripcion varchar(50),
Imagen varchar(100),
Ventana varchar(100),
Enlace varchar(250),
Es_activo bit not null default 1,
I18n varchar(50) not null
 * 
 */
