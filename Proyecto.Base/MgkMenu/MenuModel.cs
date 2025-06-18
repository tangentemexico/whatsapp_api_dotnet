using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mgk.Base.Menu
{
    public class MenuModel
    {
        public static string __table_name { get; } = "_menu";
        public static string[] __table_keys { get; } = { "Menu_id" };
        public static bool __auto_increment { get; } = false;

        public int Menu_id { get; set; }
        public String Titulo { get; set; }
        public String Descripcion { get; set; }
        public String Imagen { get; set; }
        public String Ventana { get; set; }
        public String Enlace { get; set; }
        public Boolean? Es_activo { get; set; }
        public Boolean? Es_top { get; set; }
        public String I18n { get; set; }
        public int Orden { get; set; }
        public int Grupo { get; set; }
        public int Menu_id_padre { get; set; }
        public String Sistema { get; set; }

        public static Boolean Existe(String I18n,List<MenuModel> MenuLista)
        {
            if (MenuLista!=null && MenuLista.Count > 0)
                return MenuLista.Exists(item => item.I18n== I18n);
            return false;
        }

        public static Boolean Existe(string[] I18n, List<MenuModel> MenuLista,Boolean Todos=true)
        {
            if (MenuLista != null && MenuLista.Count > 0 && I18n!=null && I18n.Length > 0)
            {
                int Tot = I18n.Length;
                int Encontrados = 0;
                foreach(string s in I18n)
                {
                    if (Existe(s, MenuLista))
                        Encontrados++;

                    if (Encontrados > 0 && Todos == false)
                        return true;
                }
                return Encontrados==Tot;
            }
            return false;
        }
    }
}

/**
 * 
create table _menu (
Menu_id int not null primary key identity,
Titulo varchar(50) not null,
Descripcion varchar(50),
Imagen varchar(100),
Ventana varchar(100),
Enlace varchar(250),
Es_activo bit not null default 1,
I18n varchar(50) not null
 * 
 */
