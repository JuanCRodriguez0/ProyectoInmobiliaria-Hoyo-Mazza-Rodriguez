using MySql.Data.MySqlClient;
using ProyectoInmobiliaria_Hoyo_Mazza_Rodriguez.Models;

namespace ProyectoInmobiliaria_Hoyo_Mazza_Rodriguez.Models
{
    public class RepositorioTipoInmueble
    {
        private readonly string connectionString;

        public RepositorioTipoInmueble(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection")!;
        }

        // 1 OBTENER TODOS LOS TIPOS DE INMUEBLE
        public List<TipoInmueble> ObtenerTodos()
        {
            var tipos = new List<TipoInmueble>();

            using (var connection = new MySqlConnection(connectionString))
            {
                var sql = @"SELECT IdTipoInmueble, Descripcion
                            FROM tipos_inmueble
                            ORDER BY Descripcion";

                using (var command = new MySqlCommand(sql, connection))
                {
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            tipos.Add(new TipoInmueble
                            {
                                IdTipoInmueble = reader.GetInt32("IdTipoInmueble"),
                                Descripcion = reader.GetString("Descripcion")
                            });
                        }
                    }
                }
            }
            return tipos;
        }

        // 2 OBTENER UN TIPO DE INMUEBLE POR ID
        public TipoInmueble? ObtenerPorId(int id)
        {
            TipoInmueble? tipo = null;

            using (var connection = new MySqlConnection(connectionString))
            {
                var sql = @"SELECT IdTipoInmueble, Descripcion
                            FROM tipos_inmueble
                            WHERE IdTipoInmueble = @id";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    connection.Open();

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            tipo = new TipoInmueble
                            {
                                IdTipoInmueble = reader.GetInt32("IdTipoInmueble"),
                                Descripcion = reader.GetString("Descripcion")
                            };
                        }
                    }
                }
            }
            return tipo;
        }

        // 3 GUARDAR (ALTA DE TIPO DE INMUEBLE)
        public int Alta(TipoInmueble tipo)
        {
            int res = -1;

            using (var connection = new MySqlConnection(connectionString))
            {
                var sql = @"INSERT INTO tipos_inmueble (Descripcion)
                            VALUES (@descripcion);
                            SELECT LAST_INSERT_ID();";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@descripcion", tipo.Descripcion);

                    connection.Open();
                    res = Convert.ToInt32(command.ExecuteScalar());
                    tipo.IdTipoInmueble = res;
                }
            }
            return res;
        }

        // 4 MODIFICACIÓN DE TIPO DE INMUEBLE
        public int Modificacion(TipoInmueble tipo)
        {
            int res = -1;

            using (var connection = new MySqlConnection(connectionString))
            {
                var sql = @"UPDATE tipos_inmueble
                            SET Descripcion = @descripcion
                            WHERE IdTipoInmueble = @id";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@id", tipo.IdTipoInmueble);
                    command.Parameters.AddWithValue("@descripcion", tipo.Descripcion);

                    connection.Open();
                    res = command.ExecuteNonQuery();
                }
            }
            return res;
        }

        // 5 BAJA 
        public int Baja(int id)
        {
            int res = -1;

            using (var connection = new MySqlConnection(connectionString))
            {
                var sql = @"DELETE FROM tipos_inmueble WHERE IdTipoInmueble = @id";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@id", id);

                    connection.Open();
                    res = command.ExecuteNonQuery();
                }
            }
            return res;
        }

        // 6 VERIFICAR SI EL TIPO ESTÁ EN USO POR ALGÚN INMUEBLE
        public bool TieneInmuebles(int id)
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                var sql = @"SELECT COUNT(*) FROM inmuebles WHERE IdTipoInmueble = @id";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    connection.Open();
                    var cantidad = Convert.ToInt32(command.ExecuteScalar());
                    return cantidad > 0;
                }
            }
        }
    }
}