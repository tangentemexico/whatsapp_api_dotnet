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

namespace Mgk.Base.Token
{
    public class TokenDao : MgkDataBaseObjT
    {
        public MgkMessage Message { get; set; }
        public TokenDao()
        {
            Message = new MgkMessage();
        }

        private TokenModel SetObject(DbDataReader DataR)
        {
            TokenModel TokenItem = new TokenModel();
            TokenItem.Token_id = MgkFunctions.StrToInt(DataR["Token_id"].ToString());
            TokenItem.Token = DataR["Token"].ToString();
            TokenItem.Usuario_id = DataR["Usuario_id"].ToString();
            TokenItem.Fecha = MgkFunctions.StrToDateTime(DataR["Fecha"].ToString());
            return TokenItem;
        }

        private List<DbParameter> SetParameteres(TokenModel TokenM)
        {
            List<DbParameter> Parameters = new List<DbParameter>();
            Parameters.Add((DbParameter)GetParameter("@Token_id", TokenM.Token_id));
            Parameters.Add((DbParameter)GetParameter("@Token", TokenM.Token));
            Parameters.Add((DbParameter)GetParameter("@Usuario_id", TokenM.Usuario_id));
            Parameters.Add((DbParameter)GetParameter("@Fecha", TokenM.Fecha));
            return Parameters;
        }

        public MgkMessage Insert(TokenModel TokenM, int Acceso_id)
        {
            Message.Clear();
            try
            {
                #region Paso de Token
                List<DbParameter> Parameters = SetParameteres(TokenM);
                #endregion

                String Query= "Insert into "+ TokenModel.__table_name + "(Token,Usuario_id,Fecha) values (@Token,@Usuario_id,@Fecha);" +
                    this.LastIdQuery("Token", "Token_id");

                int iRetVal = ExecuteScalarInt(Query, Parameters, CommandType.Text);
                if (iRetVal > 0)
                {
                    TokenM.Token_id = iRetVal;
                    Message.Number = iRetVal;
                    Message.Message = "Registro Insertado exitosamente";
                    Message.OData = TokenM;
                }
                else
                {
                    throw new Exception("No fue posible Insertar el registro");
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
                    OData = new { TokenM = TokenM }
                };
                MgkLog.Error(Message);
            }
            return Message;
        }

        public MgkMessage Update(TokenModel TokenM, int Acceso_id)
        {
            Message.Clear();
            try
            {
                List<DbParameter> Parameters = SetParameteres(TokenM);

                ExecuteNonQuery("update "+ TokenModel.__table_name + " set Token=@Token,Usuario_id=@Usuario_id,Fecha=@Fecha where Token_id=@Token_id",
                    Parameters, CommandType.Text);
                if (Messages.Number >= 0)
                {
                    Message.Number = 1;
                    Message.Message = "Registro actualizado exitosamente";
                }
                else
                {
                    throw new Exception("No fue posible actualizar el registro");
                }
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
                    OData = new { TokenM = TokenM }
                };
                MgkLog.Error(Message);
            }
            return Message;
        }


        public MgkMessage Delete(TokenModel TokenM, int Acceso_id)
        {
            Message.Clear();
            try
            {
                #region Paso de Token
                List<DbParameter> Parameters = new List<DbParameter>();
                Parameters.Add((DbParameter)GetParameter("@Token_id", TokenM.Token_id));
                #endregion

                ExecuteNonQuery("Delete from "+ TokenModel.__table_name + " where Token_id=@Token_id",
                    Parameters, CommandType.Text);
                if (Messages.Number >= 0)
                {
                    Message.Number = 1;
                    Message.Message = "Registro actualizado exitosamente";
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
                    OData = new { TokenM = TokenM }
                };
                MgkLog.Error(Message);
            }
            return Message;
        }

        public TokenModel GetItem(TokenModel TokenM, int Acceso_id)
        {
            Message.Clear();
            MgkQueryBuilder QueryB = new MgkQueryBuilder();
            TokenModel TokenItem = null;

            try
            {
                List<DbParameter> Parameters = new List<DbParameter>();
                QueryB.SetQueryBase("select * from "+ TokenModel.__table_name);
                Parameters.Add((DbParameter)GetParameter("@Token_id", TokenM.Token_id));
                QueryB.AddAnd("Token_id=@Token_id");

                TokenItem = this.ReadObjItemByQuery<TokenModel>(QueryB.GetQuery(), Parameters, CommandType.Text);
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
                    OData = new { TokenM = TokenM, QueryB = QueryB }
                };
                MgkLog.Error(Message);
            }
            return TokenItem;
        }

        public TokenModel BuscarToken(TokenModel TokenM)
        {
            Message.Clear();
            TokenModel TokenItem = null;
            try
            {
                var items = GetItems(TokenM, 0);
                if (items!=null && items.Count > 0)
                {
                    TokenItem = items[0];
                }
            }
            catch (Exception ex)
            {
                Message = new MgkMessage
                {
                    Number = -40,
                    Code = "ex-sel4",
                    Message = "Error en consulta del registro",
                    Source = this.GetType().FullName + "." + System.Reflection.MethodBase.GetCurrentMethod().Name,
                    Exception = ex.ToString(),
                    OData = new { TokenM = TokenM }
                };
                MgkLog.Error(Message);
            }
            return TokenItem;
        }

        public List<TokenModel> GetItems(TokenModel TokenM, int Acceso_id)
        {
            Message.Clear();
            List<TokenModel> TokenLista = new List<TokenModel>();
            MgkQueryBuilder QueryB = new MgkQueryBuilder();

            try
            {
                #region Paso de Token
                List<DbParameter> Parameters = new List<DbParameter>();
                QueryB.SetQueryBase("select * from "+ TokenModel.__table_name);
                if (TokenM.Token_id >0 )
                {
                    Parameters.Add((DbParameter)GetParameter("@Token_id", TokenM.Token_id));
                    QueryB.AddAnd("Token_id=@Token_id");
                }
                if ((TokenM.Token ?? "") != "")
                {
                    Parameters.Add((DbParameter)GetParameter("@Token", TokenM.Token));
                    QueryB.AddAnd("Token=@Token");
                }
                if (!string.IsNullOrEmpty(TokenM.Usuario_id))
                {
                    Parameters.Add((DbParameter)GetParameter("@Usuario_id", TokenM.Usuario_id));
                    QueryB.AddAnd("Usuario_id=@Usuario_id");
                }
                #endregion
                String Query = QueryB.GetQuery();

                TokenLista = this.ReadObjListByQuery<TokenModel>(Query, Parameters);
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
                    OData = new { TokenM = TokenM, QueryB = QueryB }
                };
                MgkLog.Error(Message);
            }
            Message.Number = TokenLista.Count();
            return TokenLista;
        }

    }
}
