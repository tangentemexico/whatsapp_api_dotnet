using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proyecto.Base.MgkControlAcceso
{
    public class LoginRequestModel
    {
        public String Usuario_id { get; set; }
        public String Password { get; set; }
        public String Origen { get; set; }
        public String Did { get; set; }
        public String OsName { get; set; }
        public String Email { get; set; }
    }
}
