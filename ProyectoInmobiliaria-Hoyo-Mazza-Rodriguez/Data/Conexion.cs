using MySql.Data.MySqlClient;

namespace ProyectoInmobiliaria_Hoyo_Mazza_Rodriguez.Data
{
    public class Conexion
    {
        private static string? connectionString;

        public static void Configurar(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public static MySqlConnection ObtenerConexion()
        {
            return new MySqlConnection(connectionString);
        }
    }
}