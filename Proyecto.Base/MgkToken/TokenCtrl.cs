using Mgk.Commonsx;
using Mgk.Base.Usuario;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mgk.Base.Token
{
    public class TokenCtrl
    {
        protected TokenDao TokenD { get; set; }

        public MgkMessage Message { get; set; }

        public TokenCtrl()
        {
            Message = new MgkMessage();
            TokenD = new TokenDao();
        }

        public MgkMessage CrearToken(int Acceso_id)
        {
            TokenModel TokenM = new TokenModel();
            Message = TokenD.Insert(TokenM, Acceso_id);
            return Message;
        }

        /// <summary>
        /// Insertar Token
        /// </summary>
        /// <param name="TokenM"></param>
        /// <returns></returns>
        public MgkMessage Insert(TokenModel TokenM, int Acceso_id)
        {
            Message = TokenD.Insert(TokenM, Acceso_id);
            return Message;
        }

        /// <summary>
        /// Actualizar Token
        /// </summary>
        /// <param name="TokenM"></param>
        /// <returns></returns>
        public MgkMessage Update(TokenModel TokenM, int Acceso_id)
        {
            Message = TokenD.Update(TokenM, Acceso_id);
            return Message;
        }

        /// <summary>
        /// Eliminar Token
        /// </summary>
        /// <param name="TokenM"></param>
        /// <param name="UsuarioM"></param>
        /// <returns></returns>
        public MgkMessage Delete(TokenModel TokenM, int Acceso_id)
        {
            Message = TokenD.Delete(TokenM, Acceso_id);
            return Message;
        }


        public MgkMessage Get(TokenModel TokenM, int Acceso_id)
        {
            TokenModel TokenItem = TokenD.GetItem(TokenM, Acceso_id);
            Message = TokenD.Message;
            Message.OData = TokenItem;
            return Message;
        }

        public TokenModel BuscarToken(TokenModel TokenM)
        {
            TokenModel TokenItem = TokenD.BuscarToken(TokenM);
            return TokenItem;
        }

        /// <summary>
        /// Leer Lista de Tokens
        /// </summary>
        /// <param name="TokenM"></param>
        /// <param name="UsuarioM"></param>
        /// <returns></returns>
        public List<TokenModel> GetAll(TokenModel TokenM, int Acceso_id)
        {
            List<TokenModel> TokenItems = TokenD.GetItems(TokenM, Acceso_id);
            Message = TokenD.Message;

            return TokenItems;
        }

        public List<TokenModel> GetAll(int Acceso_id)
        {
            List<TokenModel> TokenItems = TokenD.GetItems(new TokenModel(), Acceso_id);
            Message = TokenD.Message;

            return TokenItems;
        }

    }
}