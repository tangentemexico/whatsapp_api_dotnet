using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mgk.Base.Modulo
{
    public class ModuloModel
    {
        public static string __table_name { get; } = "_Modulo";
        public static string[] __table_keys { get; } = { "Modulo_cod" };
        public static bool __auto_increment { get; } = false;

        /// <summary>
        /// Codigo amigable del modulo
        /// </summary>
        public String Modulo_cod { get; set; }
        /// <summary>
        /// Nombre descriptivo del modulo
        /// </summary>
        public String Nombre { get; set; }
        /// <summary>
        /// Estatus de uso
        /// </summary>
        public Boolean? Es_activo { get; set; }

        /// <summary>
        /// Posibles permisos permitidos
        /// </summary>
        public int Permisos { get; set; }

        /// <summary>
        /// Ruta de acceso MVC. Se usa para la validacion de acceso
        /// </summary>
        public String Ruta { get; set; }

        public String RutaApi { get; set; }


        public int _Es_activo
        {
            get
            {
                if (Es_activo == null)
                    return 0;
                else if (Es_activo.Value == false)
                    return 0;
                else
                    return 1;
            }
            set
            {
                Es_activo = (value == 0) ? false : true;
            }
        }

        //public Boolean _Borrar
        //{
        //    get { return _Borrar_==1; }
        //    set { _Borrar_ = value ? 1 : 0; }
        //}
        //public Boolean _Modificar
        //{
        //    get { return _Modificar_ == 1; }
        //    set { _Modificar_ = value ? 1 : 0; }
        //}

        //public Boolean _Leer
        //{
        //    get { return _Leer_ == 1; }
        //    set { _Leer_ = value ? 1 : 0; }
        //}
        //public Boolean _Escribir
        //{
        //    get { return _Escribir_ == 1; }
        //    set { _Escribir_ = value ? 1 : 0; }
        //}

        public Boolean _Borrar
        {
            get { return (Permisos & 8) > 0 ; }
            set { if (value ) Permisos = Permisos |  8; }
        }
        public Boolean _Modificar
        {
            get { return (Permisos & 4) > 0 ; }
            set { if (value ) Permisos = Permisos |  4; }
        }
        public Boolean _Escribir
        {
            get { return (Permisos & 2) > 0 ; }
            set { if (value ) Permisos = Permisos | 2; }
        }
        public Boolean _Leer
        {
            get { return (Permisos & 1) > 0 ; }
            set { if (value ) Permisos = Permisos |  1; }
        }

        /// <summary>
        /// Permisos asignados sobre este modulo
        /// </summary>
        public int _Permisos_ { get; set; }

        //public int _Borrar_
        //{
        //    get { return (_Permisos_ & 8) > 0 ? 1 : 0; }
        //    set { if (value > 0) _Permisos_ = _Permisos_| value | 8; }
        //}
        //public int _Modificar_
        //{
        //    get { return (_Permisos_ & 4) > 0 ? 1 : 0; }
        //    set { if (value > 0) _Permisos_ = _Permisos_ | value | 4; }
        //}
        //public int _Escribir_
        //{
        //    get { return (_Permisos_ & 2) > 0 ? 1 : 0; }
        //    set { if (value > 0) _Permisos_ = _Permisos_ | value | 2; }
        //}
        //public int _Leer_
        //{
        //    get { return (_Permisos_ & 1) > 0 ? 1 : 0; }
        //    set { if (value > 0) _Permisos_ = _Permisos_ | value | 1; }
        //}

        public static Boolean Existe(String Codigo, List<ModuloModel> ModuloLista)
        {
            Boolean RetVal = false;
            string[] CodigoArray = Codigo.Split(',');
            foreach (string ItemCod in CodigoArray)
            {
                if (ModuloLista != null && ModuloLista.Count > 0)
                {
                    RetVal = ModuloLista.Exists(item => item.Es_activo == true && item.Modulo_cod.ToLower() == ItemCod.ToLower());
                    if (RetVal == true)
                        return RetVal;
                }
            }
            return RetVal;
        }

        public enum EnumPermisos
        {
            TODO = 15,
            BME=14,
            Borrar = 8,
            Modificar = 4,
            Escritura = 2,
            Lectura = 1,
        }
        public static Boolean PermisoPara(String Codigo, List<ModuloModel> ModuloLista, EnumPermisos ePermiso)
        {
            return PermisoPara(Codigo,ModuloLista, (int)ePermiso);
        }
        public static Boolean PermisoPara(String Codigo, List<ModuloModel> ModuloLista, int iPermiso)
        {
            Boolean RetVal = false;
            if (ModuloLista != null && ModuloLista.Count > 0)
            {
                foreach (var item in ModuloLista)
                {
                    if (Codigo.ToLower() == item.Modulo_cod.ToLower())
                    {
                        int r = item.Permisos & iPermiso;
                        if (r > 0)
                            return true;
                    }
                }
            }
            return RetVal;
        }

    }
}

/**
 * 
 * 
create table _modulo(
Modulo_cod varchar(20) not null primary key,
Nombre varchar(20),
Es_activo tinyint(1));
 * 
 */
