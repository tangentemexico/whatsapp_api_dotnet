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

namespace Mgk.Base.Menu
{
    public class MenuDao : MgkDataBaseObjT
    {
        public MgkMessage Message { get; set; }
        public MenuDao()
        {
            Message = new MgkMessage();
        }

        public MgkMessage Insert(MenuModel MenuM, int Acceso_id)
        {
            Message.Clear();
            try
            {
                Message.Number = this.InsertObject(MenuM);
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
                    OData = new { MenuM = MenuM }
                };
                MgkLog.Error(Message);
            }
            this.GetConnection().Close();
            return Message;
        }

        public MgkMessage Update(MenuModel MenuM, int Acceso_id)
        {
            Message.Clear();
            GetConnection();
            try
            {
                Message.Number = this.UpdateObject(MenuM);
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
                    OData = new { MenuM = MenuM }
                };
                MgkLog.Error(Message);
            }
            this.GetConnection().Close();
            return Message;
        }


        public MgkMessage Delete(MenuModel MenuM, int Acceso_id)
        {
            Message.Clear();
            GetConnection();
            try
            {
                Message.Number= this.DeleteObject(MenuM);
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
                    OData = new { MenuM = MenuM }
                };
                MgkLog.Error(Message);
            }
            this.GetConnection().Close();
            return Message;
        }

        public MenuModel GetItem(MenuModel MenuM, int Acceso_id)
        {
            Message.Clear();
            MenuModel MenuItem = null;
            try
            {
                MenuItem = this.ReadNew<MenuModel>(MenuM);
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
                    OData = new { MenuM = MenuM}
                };
                MgkLog.Error(Message);
            }
            this.GetConnection().Close();
            return MenuItem;
        }


        public List<MenuModel> GetItems(MenuModel MenuM, int Acceso_id)
        {
            Message.Clear();
            GetConnection();
            DbDataReader DataR = null;
            List<MenuModel> MenuLista = new List<MenuModel>();
            MgkQueryBuilder QueryB = new MgkQueryBuilder();

            try
            {
                #region Paso de Menu
                List<DbParameter> Parameters = new List<DbParameter>();
                QueryB.SetQueryBase("select * from "+ MenuModel.__table_name);
                if (MenuM.Menu_id >0 )
                {
                    Parameters.Add((DbParameter)GetParameter("@Menu_id", MenuM.Menu_id));
                    QueryB.AddAnd("Menu_id=@Menu_id");
                }
                
                #endregion
                String Query = QueryB.GetQuery();
                MenuLista = this.ReadObjListByQuery<MenuModel>(Query,Parameters);
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
                    OData = new { MenuM = MenuM, QueryB = QueryB }
                };
                MgkLog.Error(Message);
            }
            this.GetConnection().Close();
            Message.Number = MenuLista.Count();
            return MenuLista;
        }

    }
}
