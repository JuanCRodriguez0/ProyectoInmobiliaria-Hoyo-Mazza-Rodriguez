using MySql.Data.MySqlClient;

namespace ProyectoInmobiliaria_Hoyo_Mazza_Rodriguez.Models
{
    public class RepositorioInmueble
    {
        private readonly string connectionString;

        public RepositorioInmueble(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection")!;
        }

        // Trae todos los activos, con el nombre del propietario y la descripción del tipo (para el listado)
        public List<Inmueble> ObtenerTodos()
        {
            var inmuebles = new List<Inmueble>();

            using (var connection = new MySqlConnection(connectionString))
            {
                var sql = @"SELECT i.idInmueble, i.idPropietario, i.idTipoInmueble, i.direccion, i.cupo,
                                   i.ambientes, i.superficie, i.precioPorDia, i.latitud, i.longitud,
                                   i.disponible, i.estado, i.portada,
                                   CONCAT(p.nombre, ' ', p.apellido) AS NombrePropietario,
                                   t.descripcion AS DescripcionTipo
                            FROM inmuebles i
                            INNER JOIN propietarios p ON i.idPropietario = p.idPropietario
                            INNER JOIN tipos_inmueble t ON i.idTipoInmueble = t.idTipoInmueble
                            WHERE i.estado = 1";

                using (var command = new MySqlCommand(sql, connection))
                {
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            inmuebles.Add(LeerInmueble(reader));
                        }
                    }
                }
            }
            return inmuebles;
        }

        public Inmueble? ObtenerPorId(int id)
        {
            Inmueble? inmueble = null;

            using (var connection = new MySqlConnection(connectionString))
            {
                var sql = @"SELECT i.idInmueble, i.idPropietario, i.idTipoInmueble, i.direccion, i.cupo,
                                   i.ambientes, i.superficie, i.precioPorDia, i.latitud, i.longitud,
                                   i.disponible, i.estado, i.portada,
                                   CONCAT(p.nombre, ' ', p.apellido) AS NombrePropietario,
                                   t.descripcion AS DescripcionTipo
                            FROM inmuebles i
                            INNER JOIN propietarios p ON i.idPropietario = p.idPropietario
                            INNER JOIN tipos_inmueble t ON i.idTipoInmueble = t.idTipoInmueble
                            WHERE i.idInmueble = @id";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    connection.Open();

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            inmueble = LeerInmueble(reader);
                        }
                    }
                }
            }
            return inmueble;
        }

        // Lista de inmuebles de un propietario específico (para el informe pedido en el enunciado)
        public List<Inmueble> ObtenerPorPropietario(int idPropietario)
        {
            var inmuebles = new List<Inmueble>();

            using (var connection = new MySqlConnection(connectionString))
            {
                var sql = @"SELECT i.idInmueble, i.idPropietario, i.idTipoInmueble, i.direccion, i.cupo,
                                   i.ambientes, i.superficie, i.precioPorDia, i.latitud, i.longitud,
                                   i.disponible, i.estado, i.portada,
                                   CONCAT(p.nombre, ' ', p.apellido) AS NombrePropietario,
                                   t.descripcion AS DescripcionTipo
                            FROM inmuebles i
                            INNER JOIN propietarios p ON i.idPropietario = p.idPropietario
                            INNER JOIN tipos_inmueble t ON i.idTipoInmueble = t.idTipoInmueble
                            WHERE i.idPropietario = @idPropietario AND i.estado = 1";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@idPropietario", idPropietario);
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            inmuebles.Add(LeerInmueble(reader));
                        }
                    }
                }
            }
            return inmuebles;
        }

        public int Alta(Inmueble inmueble)
        {
            int res = -1;

            using (var connection = new MySqlConnection(connectionString))
            {
                var sql = @"INSERT INTO inmuebles 
                            (idPropietario, idTipoInmueble, direccion, cupo, ambientes, superficie, 
                             precioPorDia, latitud, longitud, disponible, estado, portada) 
                            VALUES 
                            (@idPropietario, @idTipoInmueble, @direccion, @cupo, @ambientes, @superficie,
                             @precioPorDia, @latitud, @longitud, @disponible, @estado, @portada);
                            SELECT LAST_INSERT_ID();";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@idPropietario", inmueble.IdPropietario);
                    command.Parameters.AddWithValue("@idTipoInmueble", inmueble.IdTipoInmueble);
                    command.Parameters.AddWithValue("@direccion", inmueble.Direccion);
                    command.Parameters.AddWithValue("@cupo", inmueble.Cupo);
                    command.Parameters.AddWithValue("@ambientes", inmueble.Ambientes);
                    command.Parameters.AddWithValue("@superficie", inmueble.Superficie);
                    command.Parameters.AddWithValue("@precioPorDia", inmueble.PrecioPorDia);
                    command.Parameters.AddWithValue("@latitud", (object?)inmueble.Latitud ?? DBNull.Value);
                    command.Parameters.AddWithValue("@longitud", (object?)inmueble.Longitud ?? DBNull.Value);
                    command.Parameters.AddWithValue("@disponible", true);
                    command.Parameters.AddWithValue("@estado", true);
                    command.Parameters.AddWithValue("@portada", (object?)inmueble.Portada ?? DBNull.Value);

                    connection.Open();
                    res = Convert.ToInt32(command.ExecuteScalar());
                    inmueble.IdInmueble = res;
                }
            }
            return res;
        }

        public int Modificacion(Inmueble inmueble)
        {
            int res = -1;

            using (var connection = new MySqlConnection(connectionString))
            {
                var sql = @"UPDATE inmuebles 
                            SET idPropietario = @idPropietario,
                                idTipoInmueble = @idTipoInmueble,
                                direccion = @direccion, 
                                cupo = @cupo,
                                ambientes = @ambientes,
                                superficie = @superficie,
                                precioPorDia = @precioPorDia, 
                                latitud = @latitud, 
                                longitud = @longitud, 
                                portada = @portada
                            WHERE idInmueble = @id";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@id", inmueble.IdInmueble);
                    command.Parameters.AddWithValue("@idPropietario", inmueble.IdPropietario);
                    command.Parameters.AddWithValue("@idTipoInmueble", inmueble.IdTipoInmueble);
                    command.Parameters.AddWithValue("@direccion", inmueble.Direccion);
                    command.Parameters.AddWithValue("@cupo", inmueble.Cupo);
                    command.Parameters.AddWithValue("@ambientes", inmueble.Ambientes);
                    command.Parameters.AddWithValue("@superficie", inmueble.Superficie);
                    command.Parameters.AddWithValue("@precioPorDia", inmueble.PrecioPorDia);
                    command.Parameters.AddWithValue("@latitud", (object?)inmueble.Latitud ?? DBNull.Value);
                    command.Parameters.AddWithValue("@longitud", (object?)inmueble.Longitud ?? DBNull.Value);
                    command.Parameters.AddWithValue("@portada", (object?)inmueble.Portada ?? DBNull.Value);

                    connection.Open();
                    res = command.ExecuteNonQuery();
                }
            }
            return res;
        }

        // Baja lógica (ABM) — distinta de la suspensión de oferta
        public int Baja(int id)
        {
            int res = -1;

            using (var connection = new MySqlConnection(connectionString))
            {
                var sql = @"UPDATE inmuebles SET estado = 0 WHERE idInmueble = @id";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    connection.Open();
                    res = command.ExecuteNonQuery();
                }
            }
            return res;
        }

        // Suspender/reactivar oferta (regla de negocio, NO es la baja del ABM)
        public int CambiarDisponibilidad(int id, bool disponible)
        {
            int res = -1;

            using (var connection = new MySqlConnection(connectionString))
            {
                var sql = @"UPDATE inmuebles SET disponible = @disponible WHERE idInmueble = @id";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    command.Parameters.AddWithValue("@disponible", disponible);
                    connection.Open();
                    res = command.ExecuteNonQuery();
                }
            }
            return res;
        }

        // Búsqueda de inmuebles NO ocupados entre dos fechas (para el alta de reserva y el informe pedido)
        public List<Inmueble> ObtenerDisponiblesEntreFechas(DateTime desde, DateTime hasta)
        {
            var inmuebles = new List<Inmueble>();

            using (var connection = new MySqlConnection(connectionString))
            {
                var sql = @"SELECT i.idInmueble, i.idPropietario, i.idTipoInmueble, i.direccion, i.cupo,
                                   i.ambientes, i.superficie, i.precioPorDia, i.latitud, i.longitud,
                                   i.disponible, i.estado, i.portada,
                                   CONCAT(p.nombre, ' ', p.apellido) AS NombrePropietario,
                                   t.descripcion AS DescripcionTipo
                            FROM inmuebles i
                            INNER JOIN propietarios p ON i.idPropietario = p.idPropietario
                            INNER JOIN tipos_inmueble t ON i.idTipoInmueble = t.idTipoInmueble
                            WHERE i.estado = 1 AND i.disponible = 1
                              AND i.idInmueble NOT IN (
                                  SELECT r.idInmueble FROM reservas r
                                  WHERE r.fechaDesde <= @hasta AND r.fechaHasta >= @desde
                              )";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@desde", desde);
                    command.Parameters.AddWithValue("@hasta", hasta);
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            inmuebles.Add(LeerInmueble(reader));
                        }
                    }
                }
            }
            return inmuebles;
        }

        private Inmueble LeerInmueble(MySqlDataReader reader)
        {
            return new Inmueble
            {
                IdInmueble = reader.GetInt32("idInmueble"),
                IdPropietario = reader.GetInt32("idPropietario"),
                IdTipoInmueble = reader.GetInt32("idTipoInmueble"),
                Direccion = reader.GetString("direccion"),
                Cupo = reader.GetInt32("cupo"),
                Ambientes = reader.GetInt32("ambientes"),
                Superficie = reader.GetDecimal("superficie"),
                PrecioPorDia = reader.GetDecimal("precioPorDia"),
                Latitud = reader.IsDBNull(reader.GetOrdinal("latitud")) ? null : reader.GetDecimal("latitud"),
                Longitud = reader.IsDBNull(reader.GetOrdinal("longitud")) ? null : reader.GetDecimal("longitud"),
                Disponible = reader.GetBoolean("disponible"),
                Estado = reader.GetBoolean("estado"),
                Portada = reader.IsDBNull(reader.GetOrdinal("portada")) ? null : reader.GetString("portada"),
                NombrePropietario = reader.GetString("NombrePropietario"),
                DescripcionTipo = reader.GetString("DescripcionTipo")
            };
        }
    }
}