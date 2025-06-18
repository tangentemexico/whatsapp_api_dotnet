namespace Proyecto.Web.Models
{
    public class RecibeArchivosModel
    {

        public IFormFile[] PostFile {  get; set; }
        public string Usuario_id {  get; set; }
        public string Password { get; set; }
        public string Txt { get; set; }
        public string Fecha_desde { get; set; }
        public string Operacion { get; set; }
    }
}
