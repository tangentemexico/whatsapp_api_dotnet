using Mgk.Base.Usuario;
using Mgk.DataBasex;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mgk.Base.ControlAcceso
{
    public class MgkSessionAccesoDao: MgkDataBaseObjT
    {
        public List<object> Sessiones_activas()
        {
            return this.ReadDictionaryList("select * from _v_session", null);
        }

        public void Cortar_session(UsuarioModel UsuarioM,String Front="")
        {
            //String Query = String.Format("delete from {0} where Usuario_id='{1}'"
            //    , MgkSessionAccesoModel.__table_name
            //    , UsuarioM.Usuario_id );
            //if (string.IsNullOrEmpty(Front) == false)
            //    Query += " and Front='"+ Front + "'";

            //this.ExecuteNonQuery(Query, null, CommandType.Text);
        }

    }
}
