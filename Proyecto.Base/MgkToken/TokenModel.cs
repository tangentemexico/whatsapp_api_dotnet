using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mgk.Base.Token
{
    public class TokenModel
    {
        public static int TokenId = 1;

        public static string __table_name { get; } = "_Token";
        public static string[] __table_keys { get; } = { "Token_id" };
        public static bool __auto_increment { get; } = true;

        public int Token_id { get; set; }
        public String Usuario_id { get; set; }
        public String Token { get; set; }
        public DateTime Fecha { get; set; }
    }
}

/**
 * 
create table _Token(
Token_id serial,
Usuario_id int not null,
Token varchar(50) not null,
Fecha datetime)
 * 
 */
