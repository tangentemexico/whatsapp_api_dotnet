namespace Mgk.Databasex
{
    public class Class1
    {

    }

    public class DemoModel
    {
        public static string __table_name { get; } = "t_demo";
        public static string[] __table_keys { get; } = { "Mensaje_id" };
        public static bool __auto_increment { get; } = true;
        public int Mensaje_id { get; set; }
        public String Remitente { get; set; }
        public String Fecha_inserta { get; set; }
        public String Destinatarios { get; set; }
        public String Mensaje { get; set; }
    }
}
