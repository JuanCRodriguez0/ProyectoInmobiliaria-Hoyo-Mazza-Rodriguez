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

        private const string SELECT_BASE = @"
            SELECT i.idInmueble, i.idPropietario, i.idTipoInmueble, i.direccion, i.cupo,
                   i.ambientes, i.superficie, i.precioPorDia, i.porcentajeSenia, i.latitud, i.longitud,
                   i.disponible, i.estado, i.portada,
                   CONCAT(p.nombre, ' ', p.apellido) AS NombrePropietario,
                   t.descripcion AS DescripcionTipo
            FROM inmuebles i
            INNER JOIN propietarios p ON i.idPropietario = p.idPropietario
            INNER JOIN tipos_inmueble t ON i.idTipoInmueble = t.idTipoInmueble";

        public List<Inmueble> ObtenerTodos()
        {
            var inmuebles = new List<Inmueble>();

            using (var connection = new MySqlConnection(connectionString))
            {
                var sql = SELECT_BASE + " WHERE i.estado = 1";

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

        public PaginadoResultado<Inmueble> ObtenerPaginado(int pagina, int tamanioPagina, string? busqueda, bool? disponible)
        {
            var resultado = new PaginadoResultado<Inmueble>
            {
                PaginaActual = pagina < 1 ? 1 : pagina,
                TamanioPagina = tamanioPagina,
                Busqueda = busqueda
            };
            var inmuebles = new List<Inmueble>();

            using (var connection = new MySqlConnection(connectionString))
            {
                var where = "WHERE i.estado = 1";
                if (disponible.HasValue) where += " AND i.disponible = @disponible";
                if (!string.IsNullOrWhiteSpace(busqueda))
                    where += " AND (i.direccion LIKE @busqueda OR p.nombre LIKE @busqueda OR p.apellido LIKE @busqueda OR t.descripcion LIKE @busqueda)";

                var sqlCount = $@"SELECT COUNT(*) FROM inmuebles i
                          INNER JOIN propietarios p ON i.idPropietario = p.idPropietario
                          INNER JOIN tipos_inmueble t ON i.idTipoInmueble = t.idTipoInmueble
                          {where}";

                var sqlDatos = SELECT_BASE + $@" {where}
                          ORDER BY i.idInmueble DESC
                          LIMIT @tamanio OFFSET @offset";

                connection.Open();

                using (var comandoCount = new MySqlCommand(sqlCount, connection))
                {
                    if (disponible.HasValue) comandoCount.Parameters.AddWithValue("@disponible", disponible.Value);
                    if (!string.IsNullOrWhiteSpace(busqueda)) comandoCount.Parameters.AddWithValue("@busqueda", $"%{busqueda}%");
                    resultado.TotalRegistros = Convert.ToInt32(comandoCount.ExecuteScalar());
                }

                using (var command = new MySqlCommand(sqlDatos, connection))
                {
                    if (disponible.HasValue) command.Parameters.AddWithValue("@disponible", disponible.Value);
                    if (!string.IsNullOrWhiteSpace(busqueda)) command.Parameters.AddWithValue("@busqueda", $"%{busqueda}%");
                    command.Parameters.AddWithValue("@tamanio", resultado.TamanioPagina);
                    command.Parameters.AddWithValue("@offset", (resultado.PaginaActual - 1) * resultado.TamanioPagina);

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            inmuebles.Add(LeerInmueble(reader));
                        }
                    }
                }
            }

            resultado.Items = inmuebles;
            return resultado;
        }

        // Usado por el autocompletado de Inmueble en Reserva/Edit (no filtra fechas)
        public List<Inmueble> BuscarPorTexto(string term, int limite = 10)
        {
            var inmuebles = new List<Inmueble>();

            using (var connection = new MySqlConnection(connectionString))
            {
                var sql = SELECT_BASE + @"
                    WHERE i.estado = 1 AND i.direccion LIKE @term
                    ORDER BY i.direccion
                    LIMIT @limite";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@term", $"%{term}%");
                    command.Parameters.AddWithValue("@limite", limite);
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

        public List<Inmueble> BuscarDisponiblesPorTexto(string term, DateTime desde, DateTime hasta, int limite = 10)
        {
            var inmuebles = new List<Inmueble>();

            using (var connection = new MySqlConnection(connectionString))
            {
                var sql = SELECT_BASE + @"
                    WHERE i.estado = 1 AND i.disponible = 1 AND i.direccion LIKE @term
                      AND i.idInmueble NOT IN (
                          SELECT r.idInmueble FROM reservas r
                          WHERE r.estado = 1 AND r.fechaDesde <= @hasta AND r.fechaHasta >= @desde
                      )
                    ORDER BY i.direccion
                    LIMIT @limite";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@term", $"%{term}%");
                    command.Parameters.AddWithValue("@desde", desde.Date);
                    command.Parameters.AddWithValue("@hasta", hasta.Date);
                    command.Parameters.AddWithValue("@limite", limite);
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
                var sql = SELECT_BASE + " WHERE i.idInmueble = @id";

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

        public PaginadoResultado<Inmueble> ObtenerPorPropietario(int idPropietario, int pagina, int tamanioPagina)
        {
            var resultado = new PaginadoResultado<Inmueble>
            {
                PaginaActual = pagina < 1 ? 1 : pagina,
                TamanioPagina = tamanioPagina
            };
            var inmuebles = new List<Inmueble>();

            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                using (var comandoCount = new MySqlCommand(
                    "SELECT COUNT(*) FROM inmuebles WHERE idPropietario = @idPropietario AND estado = 1", connection))
                {
                    comandoCount.Parameters.AddWithValue("@idPropietario", idPropietario);
                    resultado.TotalRegistros = Convert.ToInt32(comandoCount.ExecuteScalar());
                }

                var sql = SELECT_BASE + @" WHERE i.idPropietario = @idPropietario AND i.estado = 1
                                            ORDER BY i.direccion
                                            LIMIT @tamanio OFFSET @offset";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@idPropietario", idPropietario);
                    command.Parameters.AddWithValue("@tamanio", resultado.TamanioPagina);
                    command.Parameters.AddWithValue("@offset", (resultado.PaginaActual - 1) * resultado.TamanioPagina);

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            inmuebles.Add(LeerInmueble(reader));
                        }
                    }
                }
            }

            resultado.Items = inmuebles;
            return resultado;
        }

        public PaginadoResultado<InformeInmuebleReservas> ObtenerMasReservados(int dias, int pagina, int tamanioPagina)
        {
            var resultado = new PaginadoResultado<InformeInmuebleReservas>
            {
                PaginaActual = pagina < 1 ? 1 : pagina,
                TamanioPagina = tamanioPagina
            };
            var items = new List<InformeInmuebleReservas>();

            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                var desdeFecha = DateTime.Today.AddDays(-dias);

                using (var comandoCount = new MySqlCommand(@"
                    SELECT COUNT(*) FROM (
                        SELECT i.idInmueble
                        FROM inmuebles i
                        INNER JOIN reservas r ON r.idInmueble = i.idInmueble
                                              AND r.estado = 1 AND r.fechaDesde >= @desde
                        WHERE i.estado = 1
                        GROUP BY i.idInmueble
                    ) sub", connection))
                {
                    comandoCount.Parameters.AddWithValue("@desde", desdeFecha);
                    resultado.TotalRegistros = Convert.ToInt32(comandoCount.ExecuteScalar());
                }

                var sql = @"SELECT i.idInmueble, i.idPropietario, i.idTipoInmueble, i.direccion, i.cupo,
                           i.ambientes, i.superficie, i.precioPorDia, i.porcentajeSenia, i.latitud, i.longitud,
                           i.disponible, i.estado, i.portada,
                           CONCAT(p.nombre, ' ', p.apellido) AS NombrePropietario,
                           t.descripcion AS DescripcionTipo,
                           COUNT(r.idReserva) AS CantidadReservas
                    FROM inmuebles i
                    INNER JOIN propietarios p ON i.idPropietario = p.idPropietario
                    INNER JOIN tipos_inmueble t ON i.idTipoInmueble = t.idTipoInmueble
                    INNER JOIN reservas r ON r.idInmueble = i.idInmueble
                                          AND r.estado = 1
                                          AND r.fechaDesde >= @desde
                    WHERE i.estado = 1
                    GROUP BY i.idInmueble
                    ORDER BY CantidadReservas DESC
                    LIMIT @tamanio OFFSET @offset";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@desde", desdeFecha);
                    command.Parameters.AddWithValue("@tamanio", resultado.TamanioPagina);
                    command.Parameters.AddWithValue("@offset", (resultado.PaginaActual - 1) * resultado.TamanioPagina);

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            items.Add(new InformeInmuebleReservas
                            {
                                Inmueble = LeerInmueble(reader),
                                CantidadReservas = reader.GetInt32("CantidadReservas")
                            });
                        }
                    }
                }
            }

            resultado.Items = items;
            return resultado;
        }

        public PaginadoResultado<Inmueble> ObtenerSinReservasEn(int dias, int pagina, int tamanioPagina)
        {
            var resultado = new PaginadoResultado<Inmueble>
            {
                PaginaActual = pagina < 1 ? 1 : pagina,
                TamanioPagina = tamanioPagina
            };
            var inmuebles = new List<Inmueble>();
            var desdeFecha = DateTime.Today.AddDays(-dias);

            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                var whereSub = @"i.estado = 1
                      AND i.idInmueble NOT IN (
                          SELECT r.idInmueble FROM reservas r
                          WHERE r.estado = 1 AND r.fechaDesde >= @desde
                      )";

                using (var comandoCount = new MySqlCommand(
                    $"SELECT COUNT(*) FROM inmuebles i WHERE {whereSub}", connection))
                {
                    comandoCount.Parameters.AddWithValue("@desde", desdeFecha);
                    resultado.TotalRegistros = Convert.ToInt32(comandoCount.ExecuteScalar());
                }

                var sql = SELECT_BASE + $@" WHERE {whereSub}
                                            ORDER BY i.direccion
                                            LIMIT @tamanio OFFSET @offset";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@desde", desdeFecha);
                    command.Parameters.AddWithValue("@tamanio", resultado.TamanioPagina);
                    command.Parameters.AddWithValue("@offset", (resultado.PaginaActual - 1) * resultado.TamanioPagina);

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            inmuebles.Add(LeerInmueble(reader));
                        }
                    }
                }
            }

            resultado.Items = inmuebles;
            return resultado;
        }

        public int Alta(Inmueble inmueble)
        {
            int res = -1;

            using (var connection = new MySqlConnection(connectionString))
            {
                var sql = @"INSERT INTO inmuebles 
                            (idPropietario, idTipoInmueble, direccion, cupo, ambientes, superficie, 
                             precioPorDia, porcentajeSenia, latitud, longitud, disponible, estado, portada) 
                            VALUES 
                            (@idPropietario, @idTipoInmueble, @direccion, @cupo, @ambientes, @superficie,
                             @precioPorDia, @porcentajeSenia, @latitud, @longitud, @disponible, @estado, @portada);
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
                    command.Parameters.AddWithValue("@porcentajeSenia", inmueble.PorcentajeSenia);
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
                                porcentajeSenia = @porcentajeSenia,
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
                    command.Parameters.AddWithValue("@porcentajeSenia", inmueble.PorcentajeSenia);
                    command.Parameters.AddWithValue("@latitud", (object?)inmueble.Latitud ?? DBNull.Value);
                    command.Parameters.AddWithValue("@longitud", (object?)inmueble.Longitud ?? DBNull.Value);
                    command.Parameters.AddWithValue("@portada", (object?)inmueble.Portada ?? DBNull.Value);

                    connection.Open();
                    res = command.ExecuteNonQuery();
                }
            }
            return res;
        }

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

        public PaginadoResultado<Inmueble> ObtenerDisponiblesEntreFechas(DateTime desde, DateTime hasta, int pagina, int tamanioPagina)
        {
            var resultado = new PaginadoResultado<Inmueble>
            {
                PaginaActual = pagina < 1 ? 1 : pagina,
                TamanioPagina = tamanioPagina
            };
            var inmuebles = new List<Inmueble>();

            var whereSub = @"i.estado = 1 AND i.disponible = 1
                              AND i.idInmueble NOT IN (
                                  SELECT r.idInmueble FROM reservas r
                                  WHERE r.estado = 1 AND r.fechaDesde <= @hasta AND r.fechaHasta >= @desde
                              )";

            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                using (var comandoCount = new MySqlCommand($"SELECT COUNT(*) FROM inmuebles i WHERE {whereSub}", connection))
                {
                    comandoCount.Parameters.AddWithValue("@desde", desde);
                    comandoCount.Parameters.AddWithValue("@hasta", hasta);
                    resultado.TotalRegistros = Convert.ToInt32(comandoCount.ExecuteScalar());
                }

                var sql = SELECT_BASE + $@" WHERE {whereSub}
                                            ORDER BY i.direccion
                                            LIMIT @tamanio OFFSET @offset";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@desde", desde);
                    command.Parameters.AddWithValue("@hasta", hasta);
                    command.Parameters.AddWithValue("@tamanio", resultado.TamanioPagina);
                    command.Parameters.AddWithValue("@offset", (resultado.PaginaActual - 1) * resultado.TamanioPagina);

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            inmuebles.Add(LeerInmueble(reader));
                        }
                    }
                }
            }
            resultado.Items = inmuebles;
            return resultado;
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
                PorcentajeSenia = reader.GetDecimal("porcentajeSenia"),
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