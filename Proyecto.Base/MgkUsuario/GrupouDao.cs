using Mgk.Commonsx;
using Mgk.DataBasex;
using Mgk.Base.Usuario;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mgk.Base.Menu;
using Mgk.Base.Modulo;

namespace Mgk.Base.Grupou
{
    public class GrupouDao : MgkDataBaseObjT
    {
        public MgkMessage Message { get; set; }
        public GrupouDao()
        {
            Message = new MgkMessage();
        }

        public MgkMessage Insert(GrupouModel GrupouM, int Acceso_id)
        {
            Message.Clear();
            try
            {
                Message.Number = this.InsertObject(GrupouM);
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
                    OData = new { GrupouM = GrupouM }
                };
                MgkLog.Error(Message);
            }
            this.GetConnection().Close();
            return Message;
        }

        public MgkMessage Update(GrupouModel GrupouM, int Acceso_id)
        {
            Message.Clear();
            GetConnection();
            try
            {
                Message.Number = this.UpdateObject(GrupouM);
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
                    OData = new { GrupouM = GrupouM }
                };
                MgkLog.Error(Message);
            }
            this.GetConnection().Close();
            return Message;
        }

        public MgkMessage UpdateMenuLista(GrupouModel GrupouM) {
            try
            {
                this.ExecuteNonQuery("delete from _grupou_menu where Grupou_code='" + GrupouM.Grupou_code + "'", null, CommandType.Text);
                if (GrupouM._ListaMenu!=null)
                    foreach (var item in GrupouM._ListaMenu)
                        if (item.Es_activo == true)
                            this.ExecuteNonQuery("insert into _grupou_menu values ('" + GrupouM.Grupou_code + "','" + item.Menu_id + "')", null, CommandType.Text);
            }
            catch (Exception ex)
            {
                this.Message.Clear();
                this.Message.Message = "Error Actulizar Lista de Menus en Grupo de usuario";
                this.Message.Number = -4340;
                this.Message.Exception = ex.ToString();
                this.Message.OData = new { ListaMenu = GrupouM._ListaMenu };
                MgkLog.Error(this.Message);
            }


            return this.Messages.GetLastMessage();
        }

        public MgkMessage UpdateModuloLista(GrupouModel GrupouM)
        {
            try
            {
                this.ExecuteNonQuery("delete from _grupou_modulo where Grupou_code='" + GrupouM.Grupou_code + "'", null, CommandType.Text);
                if (GrupouM._ListaModulo!=null)
                    foreach (var item in GrupouM._ListaModulo)
                        if (item.Permisos > 0)
                            this.ExecuteNonQuery("insert into _grupou_modulo values ('" + GrupouM.Grupou_code + "','" + item.Modulo_cod + "'," + item.Permisos + ")", null, CommandType.Text);
            }
            catch (Exception ex)
            {
                this.Message.Clear();
                this.Message.Message = "Error Actulizar Lista de Módulos en Grupo de usuario";
                this.Message.Number = -4341;
                this.Message.Exception = ex.ToString();
                this.Message.OData = new { ListaModulo = GrupouM._ListaModulo };
                MgkLog.Error(this.Message);
            }
            return this.Messages.GetLastMessage();
        }

   

        public MgkMessage Delete(GrupouModel GrupouM, int Acceso_id)
        {
            Message.Clear();
            try
            {
                this.ExecuteNonQuery("delete from _grupou_menu where Grupou_code='" + GrupouM.Grupou_code + "'", null, CommandType.Text);
                this.ExecuteNonQuery("delete from _grupou_modulo where Grupou_code='" + GrupouM.Grupou_code + "'", null, CommandType.Text);

                Message.Number= this.DeleteObject(GrupouM);
            }
            catch (Exception ex)
            {
                Message = new MgkMessage
                {
                    Number = -3,
                    Code = "ex-upd",
                    Message = "Error al actualizar registro."+ ex.ToString(),
                    Source = this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name,
                    Exception = ex.ToString(),
                    OData = new { GrupouM = GrupouM }
                };
                MgkLog.Error(Message);
            }
            return Message;
        }

        public GrupouModel GetItem(GrupouModel GrupouM, int Acceso_id)
        {
            Message.Clear();
            GrupouModel GrupouItem = null;
            try
            {
                GrupouItem = this.ReadNew<GrupouModel>(GrupouM);
                String QueryMenu = @"select 
	me.Menu_id	
	,me.Titulo	
	,me.Descripcion	
	,me.Imagen	
	,me.Ventana	
	,me.Enlace	
	, cast((case when gm.Grupou_code is null then 0 else 1 end) as bit) Es_activo	
	,me.I18n	
	,me.Orden	
	,me.Grupo	
	,me.Menu_id_padre	
	,me.Auxx
from _grupou_menu as gm
right join  _menu as me on me.Menu_id = gm.Menu_id and gm.Grupou_code = '"+ GrupouM .Grupou_code+ @"'
	where me.Es_activo = 1 ";
                GrupouItem._ListaMenu = this.ReadObjListByQuery<MenuModel>(QueryMenu);

                QueryMenu = @"select 
	mo.Modulo_cod	
	,mo.Nombre	
	,case when gm.Grupou_code is null then 0 else gm.Permisos end as Permisos
	,mo.Ruta	
	,mo.RutaApi
	, cast((case when gm.Grupou_code is null then 0 else 1 end) as bit) Es_activo	
from _grupou_modulo as gm
right join  _modulo as mo on mo.Modulo_cod = gm.Modulo_cod and gm.Grupou_code = '" + GrupouM.Grupou_code + @"'
	where mo.Es_activo = 1 ";

                GrupouItem._ListaModulo = this.ReadObjListByQuery<ModuloModel>(QueryMenu);
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
                    OData = new { GrupouM = GrupouM}
                };
                MgkLog.Error(Message);
            }
            this.GetConnection().Close();
            return GrupouItem;
        }


        public List<GrupouModel> GetItems(GrupouModel GrupouM, int Acceso_id)
        {
            Message.Clear();
            GetConnection();
            DbDataReader DataR = null;
            List<GrupouModel> GrupouLista = new List<GrupouModel>();
            MgkQueryBuilder QueryB = new MgkQueryBuilder();

            try
            {
                #region Paso de Grupou
                List<DbParameter> Parameters = new List<DbParameter>();
                QueryB.SetQueryBase("select * from "+ GrupouModel.__table_name);
                if (!string.IsNullOrEmpty(GrupouM.Grupou_code) )
                {
                    Parameters.Add((DbParameter)GetParameter("@Grupou_code", GrupouM.Grupou_code));
                    QueryB.AddAnd("Grupou_code=@Grupou_code");
                }
                
                #endregion
                String Query = QueryB.GetQuery();
                GrupouLista = this.ReadObjListByQuery<GrupouModel>(Query,Parameters);
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
                    OData = new { GrupouM = GrupouM, QueryB = QueryB }
                };
                MgkLog.Error(Message);
            }
            this.GetConnection().Close();
            Message.Number = GrupouLista.Count();
            return GrupouLista;
        }

    }
}
