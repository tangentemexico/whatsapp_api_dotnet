using Mgk.Base.ControlAcceso;
using Mgk.Base.Usuario;
using Mgk.Commonsx;
using Microsoft.AspNetCore.Mvc;
using WhatsappControl.Enviar;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WhatsappWebNet6.Controllers.apirest
{
    [Route("api/[controller]")]
    [ApiController]
    public class WhatsappController : ControllerBase
    {
        // GET: api/<WhatsappController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<WhatsappController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<WhatsappController>
        [HttpPost]
        public MgkMessage Post([FromBody] EnviarModelRequest value)
        {
            MgkMessage message = new MgkMessage { 
                Number = 0,
                Message = "Mensaje recibido" 
            };

            if (value.Usuario_id == "***") {
                message.OData = new EnviarModelRequest();
                return message;
            }
            ControlAccesoCtrl accesoCtrl = new ControlAccesoCtrl();
            message = accesoCtrl.Login_simple(new UsuarioModel { Usuario_id=value.Usuario_id,Password=value.Password });
            if (message.Number < 0)
                return message;

            //var ipremoto = HttpContext.Request.HttpContext.Connection.RemoteIpAddress;
            value.Origen = HttpContext.Request.HttpContext.Connection.RemoteIpAddress.ToString();
            EnviarCtrl enviarCtrl = new EnviarCtrl();
            message = enviarCtrl.EnviarMensaje(value);
            message.OData = value;

            return message;
        }

        // PUT api/<WhatsappController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<WhatsappController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
