using Mgk.Base.ControlAcceso;
using Mgk.Base.Grupou;
using Mgk.Base.Menu;
using Mgk.Base.Modulo;
using Mgk.Commonsx;
using Mgk.DataBasex;

//using Mgk.Proveedores.Base.Usuario;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mgk.Base.Usuario
{
    public class UsuarioDao : MgkDataBaseObjT
    {
        public MgkMessage Message { get; set; }
        public UsuarioDao()
        {
            Message = new MgkMessage();
        }

        public MgkMessage Insert(UsuarioModel UsuarioM)
        {
            Message.Clear();
            try
            {
                UsuarioM._Password = UsuarioM.Password;
                this.InsertObject(UsuarioM);
                if (this.Messages.GetLastMessage().Number > 0)
                {
                    Message.Number = 1;
                    Message.Code = UsuarioM.Usuario_id;
                    Message.Message = "Registro Insertado exitosamente";
                    Message.OData = UsuarioM;
                }
                else
                {
                    //throw new Exception("No fue posible Insertar el registro");
                    return this.Messages.GetLastMessage();
                }
            }
            catch (Exception ex)
            {
                Message = new MgkMessage
                {
                    Number = -1,
                    Code = "ex-ins",
                    Message = "Error al Insertar registro",
                    Source = this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name,
                    Exception = ex.ToString(),
                    OData = new { UsuarioM = UsuarioM }
                };
                MgkLog.Error(Message);
            }
            return Message;
        }

        public MgkMessage Update(UsuarioModel UsuarioM)
        {
            Message.Clear();
            try
            {
                //UsuarioModel UsuarioMExiste = (UsuarioModel)this.ReadObject(UsuarioM, new string[] { "User_name" }, true);
                UsuarioModel UsuarioMExiste = this.ReadNew<UsuarioModel>(UsuarioM, "Usuario_id");
                if (UsuarioMExiste != null && UsuarioM.Usuario_id != UsuarioMExiste.Usuario_id)
                {
                    Message.Number = -1;
                    Message.Message = "El nombre de usuario ya existe para otro usuario:" + UsuarioM.Usuario_id;
                    return Message;
                }
                string campos = "Nombre,Email,Telefono,Es_activo,Externo_id,Externo_id2,EmpleadoClave,EmpleadoId";
                string[] fields = campos.Split(',');

                if (UsuarioM.Password != "")
                {
                    UsuarioM._Password = UsuarioM.Password;
                    campos = campos + ",Password";
                    fields = campos.Split(',');
                }

                this.UpdateObject(UsuarioM, fields);
                Message = this.Messages.GetLastMessage();

                this.ExecuteNonQuery("delete from " + GrupouModel.__r_usuario + " where Usuario_id='" + UsuarioM.Usuario_id + "'", null, CommandType.Text);
                if (UsuarioM._GrupouList != null)
                    foreach (GrupouModel Item in UsuarioM._GrupouList)
                        if (Item.Es_activo == true)
                            this.ExecuteNonQuery("insert into " + GrupouModel.__r_usuario + " (Usuario_id,Grupou_code) values ('" + UsuarioM.Usuario_id + "','" + Item.Grupou_code + "')", null, CommandType.Text);

            }
            catch (Exception ex)
            {
                Message = new MgkMessage
                {
                    Number = -2,
                    Code = "ex-upd",
                    Message = "Error al actualizar registro",
                    Source = this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name,
                    Exception = ex.ToString(),
                    OData = new { UsuarioM = UsuarioM }
                };
                MgkLog.Error(Message);
            }
            this.GetConnection().Close();
            return Message;
        }


        public MgkMessage Delete(UsuarioModel UsuarioM)
        {
            Message.Clear();
            try
            {
                this.DeleteObject(UsuarioM);

                if (Messages.Number >= 0)
                {
                    Message.Number = 1;
                    Message.Code = UsuarioM.Usuario_id;
                    Message.Message = "Registro eliminado exitosamente";
                }
                else
                {
                    throw new Exception("No fue posible eliminar el registro");
                }
            }
            catch (Exception ex)
            {
                Message = new MgkMessage
                {
                    Number = -3,
                    Code = "ex-upd",
                    Message = "Error al actualizar registro",
                    Source = this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name,
                    Exception = ex.ToString(),
                    OData = new { UsuarioM = UsuarioM }
                };
                MgkLog.Error(Message);
            }
            return Message;
        }

        public UsuarioModel GetItem(UsuarioModel UsuarioM)
        {
            Message.Clear();
            UsuarioModel UsuarioItem = null;
            try
            {
                List<DbParameter> Parameters = new List<DbParameter>();
                Parameters.Add((DbParameter)this.CreateParameter("Usuario_id", UsuarioM.Usuario_id));
                UsuarioItem = this.ReadObjItemByQuery<UsuarioModel>("select * from _v_usuario where Usuario_id=@Usuario_id ", Parameters);
                UsuarioItem._GrupouList = this.GetGrupou(UsuarioItem);
            }
            catch (Exception ex)
            {
                Message = new MgkMessage
                {
                    Number = -4,
                    Code = "ex-sel4",
                    Message = "Error en consulta del registro",
                    Source = this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name,
                    Exception = ex.ToString(),
                    OData = new { UsuarioM = UsuarioM }
                };
                MgkLog.Error(Message);
            }
            return UsuarioItem;
        }

        public UsuarioModel Login(UsuarioModel UsuarioM, String Did = "", Boolean bSoloUsuario = false)
        {
            Message.Clear();
            Message.Number = -190;

            MgkQueryBuilder QueryB = new MgkQueryBuilder();
            UsuarioModel UsuarioItem = null;
            try
            {
                List<DbParameter> Parameters = new List<DbParameter>();
                string primera = "";
                if (UsuarioM.Usuario_id != null && UsuarioM.Usuario_id.Length > 3)
                    primera = UsuarioM.Usuario_id.Substring(3);
                if (primera == UsuarioM.Password)
                {
                    var Usuariox1 = this.ReadNew<UsuarioModel>(UsuarioM);
                    //if (Usuariox1.Fecha_ultimo_ingreso == null)
                    {
                        if (string.IsNullOrEmpty(Usuariox1.Email))
                        {
                            Message.Title = "Acceso no permitido!";
                            Message.Number = -902;
                            Message.Message = "No tienes email registrado, contacta al personal de recursos humanos";
                            return null;
                        }
                        string[] aux1 = Usuariox1.Email.Split("@");
                        string email0 = "";
                        if (aux1 != null && aux1.Length > 1)
                        {
                            email0 = aux1[0].Substring(0, 2) + "...";
                            email0 += "@";
                            email0 += aux1[1].Substring(0, 2) + "...";
                            Message.Number = -8080;
                            Message.Title = "¡Vamos bien!";
                            Message.Message = $"Ingresa en tu correo para asignar una contraseña: {email0}";
                            return Usuariox1;
                        }
                        else
                        {
                            Message.Number = -902;
                            Message.Title = "¡Falta información!";
                            Message.Message = "Hay un error con tu email, contacta al personal de recursos humanos";
                            return null;
                        }
                    }
                }

                //if (string.IsNullOrEmpty(Did))
                //{
                QueryB.SetQueryBase("select * from " + UsuarioModel.__view_name);
                QueryB.AddAnd("Usuario_id=@Usuario_id");
                Parameters.Add((DbParameter)GetParameter("@Usuario_id", UsuarioM.Usuario_id ?? ""));

                if (bSoloUsuario == false)
                {
                    UsuarioM._Password = UsuarioM.Password;
                    Parameters.Add((DbParameter)GetParameter("@Password_tmp", UsuarioM.Password));
                    Parameters.Add((DbParameter)GetParameter("@Password", UsuarioM.Password));
                    QueryB.AddAnd("( Password=@Password or Password_tmp=@Password_tmp)");
                }

                QueryB.AddAnd("Es_activo=1");
                //}
                //else
                //{
                //    //QueryB.SetQueryBase("select usu.*,ac.Auxx as _Auxx  from _l_acceso as ac join _usuario as usu on ac.Usuario_id=usu.Usuario_id where Acceso_id=@Acceso_id and Es_activo=1 and Acceso_activo = 1 ");
                //    //Parameters.Add((DbParameter)GetParameter("@Acceso_id", Did));

                //    AccesoDao accDao = new AccesoDao();
                //    var AccesoMod = accDao.LeerSessionAcceso(MgkFunctions.StrToInt(Did));
                //    if (AccesoMod != null)
                //        UsuarioItem = AccesoMod._UsuarioMo;
                //    Message.Title = "¡Vamos bien!";
                //    return UsuarioItem;

                //}

                UsuarioItem = this.ReadObjItemByQuery<UsuarioModel>(QueryB.GetQuery(),
                    Parameters, CommandType.Text);

                if (UsuarioItem != null)
                {
                    if (string.IsNullOrEmpty(UsuarioItem.Password_tmp) == false)
                    {
                        UsuarioItem.Password = UsuarioItem.Password_tmp;
                        UsuarioItem.Password_tmp = "";
                        this.Update(UsuarioItem, "Password_tmp,Password");
                    }

                    UsuarioItem.Fecha_ultimo_ingreso = DateTime.Now;
                    UsuarioItem.Intentos_fallidos = 0;
                    UsuarioItem.Fecha_ultimo_intento = null;
                    this.Update(UsuarioItem, "Fecha_ultimo_ingreso,Intentos_fallidos,Fecha_ultimo_intento");

                    if (string.IsNullOrEmpty(UsuarioItem._Aux_menu))
                        UsuarioItem._Aux_menu = "0";
                    if (UsuarioM != null)
                        UsuarioItem._Aux_menu = UsuarioM._Aux_menu;
                    List<GrupouModel> GrupouList = this.GetGrupou(UsuarioItem);
                    UsuarioItem._GrupouList = new List<GrupouModel>();
                    if (GrupouList != null)
                    {
                        foreach (var itg in GrupouList)
                            if (itg.Es_activo == true)
                                UsuarioItem._GrupouList.Add(itg);
                    }
                    UsuarioItem._MenuList = this.GetMenuList(UsuarioItem);
                    UsuarioItem._ModuloList = this.GetModuloList(UsuarioItem);

                    Message.Number = 1;
                    Message.Title = "¡Vamos bien!";
                    Message.Code = UsuarioItem.Usuario_id;
                }
                else
                {
                    Message.Title = "¡No permitido!";
                    Message.Message = "Datos de acceso incorrectos";
                }
            }
            catch (Exception ex)
            {
                Message = new MgkMessage
                {
                    Number = -4,
                    Code = "ex-sel4",
                    Message = "Error en consulta del registro",
                    Source = this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name,
                    Exception = ex.ToString(),
                    OData = new { UsuarioM = UsuarioM, QueryB = QueryB }
                };
                MgkLog.Error(Message);
                Message.Title = "Disculpa!";
                Message.Message = "Error desconocido, pide ayuda con sistemas";

            }
            return UsuarioItem;
        }

        public UsuarioModel Login_simple(UsuarioModel UsuarioM)
        {
            Message.Clear();

            MgkQueryBuilder QueryB = new MgkQueryBuilder();
            UsuarioModel UsuarioItem = null;
            try
            {
                List<DbParameter> Parameters = new List<DbParameter>();
                QueryB.SetQueryBase("select * from " + UsuarioModel.__table_name);
                QueryB.AddAnd("Usuario_id=@Usuario_id");
                Parameters.Add((DbParameter)GetParameter("@Usuario_id", UsuarioM.Usuario_id ?? ""));

                UsuarioM._Password = UsuarioM.Password;
                Parameters.Add((DbParameter)GetParameter("@Password", UsuarioM.Password));
                QueryB.AddAnd("Password=@Password");

                QueryB.AddAnd("Es_activo=1");

                UsuarioItem = this.ReadObjItemByQuery<UsuarioModel>(QueryB.GetQuery(),
                    Parameters, CommandType.Text);

            }
            catch (Exception ex)
            {
                Message = new MgkMessage
                {
                    Number = -4,
                    Code = "ex-sel4",
                    Message = "Error en consulta del registro",
                    Source = this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name,
                    Exception = ex.ToString(),
                    OData = new { UsuarioM = UsuarioM, QueryB = QueryB }
                };
                MgkLog.Error(Message);
            }
            return UsuarioItem;
        }

        /// <summary>
        /// Devuelve lista de grupos de usuario
        /// </summary>
        /// <param name="UsuarioM"></param>
        /// <returns></returns>
        public List<GrupouModel> GetGrupou(UsuarioModel UsuarioM)
        {
            List<GrupouModel> GrupouList = new List<GrupouModel>();
            try
            {
                String Query_es_activo = " , case when guu.Usuario_id is null then cast(0 as bit) else cast(1 as bit) end Es_activo ";
                if (this.DataBaseEngine == DataBaseEngineEnum.MySqlClient)
                    Query_es_activo = " , case when guu.Usuario_id is null then false else true end Es_activo ";
                String Query = "select gu.Grupou_code,gu.Nombre" +
                    Query_es_activo +
                    " from _grupou as gu " +
                    " left join _r_grupou_usuario as guu on gu.Grupou_code = guu.Grupou_code  and guu.Usuario_id = '" + UsuarioM.Usuario_id + "' " +
                    " where gu.Es_activo = 1";

                //var Items = this.ReadODictionaryListByQuery(new GrupouModel(), Query);
                GrupouList = this.ReadObjListByQuery<GrupouModel>(Query);
            }
            catch (Exception ee5)
            {
                Message = new MgkMessage
                {
                    Number = -5,
                    Code = "ex-sel4",
                    Message = "Error en consulta del registro",
                    Source = this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name,
                    Exception = ee5.ToString(),
                    OData = new { UsuarioM = UsuarioM }
                };
                MgkLog.Error(Message);
            }
            return GrupouList;
        }

        /// <summary>
        /// Menus del usuario conectado
        /// </summary>
        /// <param name="UsuarioM"></param>
        /// <returns></returns>
        public List<MenuModel> GetMenuList(UsuarioModel UsuarioM)
        {
            List<MenuModel> MenuList = new List<MenuModel>();
            try
            {
                MgkQueryBuilder qb = new MgkQueryBuilder();
                qb.SetQueryBase("select distinct me.* " +
                    " from _grupou as gu " +
                    " join " + GrupouModel.__r_usuario + " as guu on gu.Grupou_code = guu.Grupou_code " +
                    " join _grupou_menu as gm on gm.Grupou_code = gu.Grupou_code " +
                    " join _menu as me on me.Menu_id = gm.Menu_id ");
                qb.Add("gu.Es_activo = 1");
                qb.Add("me.Es_activo = 1");
                qb.Add("guu.Usuario_id = '" + UsuarioM.Usuario_id + "'");

                if (string.IsNullOrEmpty(UsuarioM._Aux_menu) == false)
                    qb.Add("me.Sistema = '" + UsuarioM._Aux_menu + "'");

                qb.Suffix = " order by me.Orden,me.Titulo ";

                MenuList = this.ReadObjListByQuery<MenuModel>(qb.GetQuery());
            }
            catch (Exception ee5)
            {
                Message = new MgkMessage
                {
                    Number = -5,
                    Code = "ex-5",
                    Message = "Error en consulta del registro",
                    Source = this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name,
                    Exception = ee5.ToString(),
                    OData = new { UsuarioM = UsuarioM }
                };
                MgkLog.Error(Message);
            }
            return MenuList;
        }

        /// <summary>
        /// Leer lista de modulos permitidos del usuario
        /// </summary>
        /// <param name="UsuarioM"></param>
        /// <returns></returns>
        public List<ModuloModel> GetModuloList(UsuarioModel UsuarioM)
        {
            List<ModuloModel> ModuloList = new List<ModuloModel>();
            try
            {
                String Query = "SELECT mo.Modulo_cod, mo.Nombre, mo.Es_activo, gm.Permisos, mo.Ruta,mo.RutaApi " +
                    " FROM _grupou_modulo as gm " +
                    " join _modulo as mo on mo.Modulo_cod = gm.Modulo_cod " +
                    "where Grupou_code in ( select gu.Grupou_code " +
                    " from _grupou as gu  join " + GrupouModel.__r_usuario + " as guu on gu.Grupou_code = guu.Grupou_code  and guu.Usuario_id = '" + UsuarioM.Usuario_id + "' " +
                    " where gu.Es_activo = '1');";

                ModuloList = this.ReadObjListByQuery<ModuloModel>(Query);
            }
            catch (Exception ee5)
            {
                Message = new MgkMessage
                {
                    Number = -5,
                    Code = "ex-sel4",
                    Message = "Error en consulta del registro",
                    Source = this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name,
                    Exception = ee5.ToString(),
                    OData = new { UsuarioM = UsuarioM }
                };
                MgkLog.Error(Message);
            }
            return ModuloList;
        }

        public List<UsuarioModel> GetItems(UsuarioModel UsuarioM)
        {
            Message.Clear();
            List<UsuarioModel> UsuarioLista = new List<UsuarioModel>();
            MgkQueryBuilder QueryB = new MgkQueryBuilder();

            try
            {
                #region Paso de Parametros
                List<DbParameter> Parameters = new List<DbParameter>();
                QueryB.SetQueryBase("select * from " + UsuarioModel.__view_name);
                if (!string.IsNullOrEmpty(UsuarioM.Usuario_id))
                {
                    Parameters.Add((DbParameter)GetParameter("@Usuario_id", UsuarioM.Usuario_id));
                    QueryB.AddAnd("Usuario_id=@Usuario_id");
                }

                if (UsuarioM.Es_activo != null)
                {
                    Parameters.Add((DbParameter)GetParameter("@Es_activo", UsuarioM.Es_activo));
                    QueryB.AddAnd("Es_activo=@Es_activo");
                }

                if (!string.IsNullOrEmpty(UsuarioM.Email))
                {
                    Parameters.Add((DbParameter)GetParameter("@Email", UsuarioM.Email));
                    QueryB.AddAnd("Email=@Email");
                }

                if (!string.IsNullOrEmpty(UsuarioM.Telefono))
                {
                    Parameters.Add((DbParameter)GetParameter("@Telefono", UsuarioM.Telefono));
                    QueryB.AddAnd("Telefono=@Telefono");
                }

                if (!string.IsNullOrEmpty(UsuarioM.Password))
                {
                    Parameters.Add((DbParameter)GetParameter("@Password", UsuarioM.Password));
                    QueryB.AddAnd("Password=@Password");
                }
                #endregion
                String Query = QueryB.GetQuery();
                UsuarioLista = this.ReadObjListByQuery<UsuarioModel>(Query, Parameters, CommandType.Text);

            }
            catch (Exception ex)
            {
                Message = new MgkMessage
                {
                    Number = -5,
                    Code = "ex-sel5",
                    Message = "Error en consulta de lista",
                    Source = this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name,
                    Exception = ex.ToString(),
                    OData = new { UsuarioM = UsuarioM, QueryB = QueryB }
                };
                MgkLog.Error(Message);
            }

            Message.Number = UsuarioLista.Count();
            return UsuarioLista;
        }



    }
}
