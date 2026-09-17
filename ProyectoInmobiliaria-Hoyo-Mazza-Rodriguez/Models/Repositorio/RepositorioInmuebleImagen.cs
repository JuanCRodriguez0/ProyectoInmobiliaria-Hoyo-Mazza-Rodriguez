using MySql.Data.MySqlClient;

namespace ProyectoInmobiliaria_Hoyo_Mazza_Rodriguez.Models
{
    public class RepositorioInmuebleImagen
    {
        private readonly string connectionString;

        public RepositorioInmuebleImagen(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection")!;
        }

        public List<InmuebleImagen> ObtenerPorInmueble(int idInmueble)
        {
            var imagenes = new List<InmuebleImagen>();

            using (var connection = new MySqlConnection(connectionString))
            {
                var sql = @"SELECT idImagen, idInmueble, ruta FROM inmueble_imagenes WHERE idInmueble = @idInmueble";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@idInmueble", idInmueble);
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            imagenes.Add(new InmuebleImagen
                            {
                                IdImagen = reader.GetInt32("idImagen"),
                                IdInmueble = reader.GetInt32("idInmueble"),
                                Ruta = reader.GetString("ruta")
                            });
                        }
                    }
                }
            }
            return imagenes;
        }

        public int Alta(InmuebleImagen imagen)
        {
            int res = -1;

            using (var connection = new MySqlConnection(connectionString))
            {
                var sql = @"INSERT INTO inmueble_imagenes (idInmueble, ruta) VALUES (@idInmueble, @ruta);
                            SELECT LAST_INSERT_ID();";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@idInmueble", imagen.IdInmueble);
                    command.Parameters.AddWithValue("@ruta", imagen.Ruta);
                    connection.Open();
                    res = Convert.ToInt32(command.ExecuteScalar());
                    imagen.IdImagen = res;
                }
            }
            return res;
        }

        public int Baja(int idImagen)
        {
            int res = -1;

            using (var connection = new MySqlConnection(connectionString))
            {
                var sql = @"DELETE FROM inmueble_imagenes WHERE idImagen = @id";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@id", idImagen);
                    connection.Open();
                    res = command.ExecuteNonQuery();
                }
            }
            return res;
        }
    }
}