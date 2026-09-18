using MySql.Data.MySqlClient;

namespace ProyectoInmobiliaria_Hoyo_Mazza_Rodriguez.Models
{
    public class RepositorioReserva
    {
        private readonly string connectionString;

        public RepositorioReserva(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection")!;
        }

        private const string SELECT_BASE = @"
            SELECT r.idReserva, r.idInquilino, r.idInmueble, r.montoPorDia, r.fechaDesde, r.fechaHasta,
                   r.fechaHastaOriginal, r.fechaTerminacionEfectiva, r.multa, r.terminada,
                   r.idUsuarioCreador, r.idUsuarioTerminador, r.idReservaOrigen,
                   CONCAT(q.nombre, ' ', q.apellido) AS NombreInquilino,
                   i.direccion AS DireccionInmueble,
                   CONCAT(uc.nombre, ' ', uc.apellido) AS NombreUsuarioCreador,
                   CONCAT(ut.nombre, ' ', ut.apellido) AS NombreUsuarioTerminador
            FROM reservas r
            INNER JOIN inquilinos q ON r.idInquilino = q.idInquilino
            INNER JOIN inmuebles i ON r.idInmueble = i.idInmueble
            LEFT JOIN usuarios uc ON r.idUsuarioCreador = uc.idUsuario
            LEFT JOIN usuarios ut ON r.idUsuarioTerminador = ut.idUsuario";

        public List<Reserva> ObtenerTodos()
        {
            var reservas = new List<Reserva>();

            using (var connection = new MySqlConnection(connectionString))
            {
                var sql = SELECT_BASE + " WHERE r.estado = 1 ORDER BY r.fechaDesde DESC";

                using (var command = new MySqlCommand(sql, connection))
                {
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            reservas.Add(LeerReserva(reader));
                        }
                    }
                }
            }
            return reservas;
        }

        public Reserva? ObtenerPorId(int id)
        {
            Reserva? reserva = null;

            using (var connection = new MySqlConnection(connectionString))
            {
                var sql = SELECT_BASE + " WHERE r.idReserva = @id";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    connection.Open();

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            reserva = LeerReserva(reader);
                        }
                    }
                }
            }
            return reserva;
        }

        public List<Reserva> ObtenerPorInmueble(int idInmueble)
        {
            var reservas = new List<Reserva>();

            using (var connection = new MySqlConnection(connectionString))
            {
                var sql = SELECT_BASE + " WHERE r.idInmueble = @idInmueble AND r.estado = 1 ORDER BY r.fechaDesde DESC";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@idInmueble", idInmueble);
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            reservas.Add(LeerReserva(reader));
                        }
                    }
                }
            }
            return reservas;
        }

        public PaginadoResultado<Reserva> ObtenerVigentes(int pagina, int tamanioPagina)
        {
            var resultado = new PaginadoResultado<Reserva>
            {
                PaginaActual = pagina < 1 ? 1 : pagina,
                TamanioPagina = tamanioPagina
            };
            var reservas = new List<Reserva>();

            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                var hoy = DateTime.Today;

                using (var comandoCount = new MySqlCommand(
                    "SELECT COUNT(*) FROM reservas WHERE estado = 1 AND fechaDesde <= @hoy AND fechaHasta >= @hoy", connection))
                {
                    comandoCount.Parameters.AddWithValue("@hoy", hoy);
                    resultado.TotalRegistros = Convert.ToInt32(comandoCount.ExecuteScalar());
                }

                var sql = SELECT_BASE + @"
                    WHERE r.estado = 1
                      AND r.fechaDesde <= @hoy
                      AND r.fechaHasta >= @hoy
                    ORDER BY r.fechaHasta
                    LIMIT @tamanio OFFSET @offset";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@hoy", hoy);
                    command.Parameters.AddWithValue("@tamanio", resultado.TamanioPagina);
                    command.Parameters.AddWithValue("@offset", (resultado.PaginaActual - 1) * resultado.TamanioPagina);

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            reservas.Add(LeerReserva(reader));
                        }
                    }
                }
            }
            resultado.Items = reservas;
            return resultado;
        }

        public PaginadoResultado<Reserva> ObtenerQueTerminanEn(int dias, int pagina, int tamanioPagina)
        {
            var resultado = new PaginadoResultado<Reserva>
            {
                PaginaActual = pagina < 1 ? 1 : pagina,
                TamanioPagina = tamanioPagina
            };
            var reservas = new List<Reserva>();

            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                var hoy = DateTime.Today;
                var limite = hoy.AddDays(dias);

                using (var comandoCount = new MySqlCommand(
                    "SELECT COUNT(*) FROM reservas WHERE estado = 1 AND terminada = 0 AND fechaHasta BETWEEN @hoy AND @limite", connection))
                {
                    comandoCount.Parameters.AddWithValue("@hoy", hoy);
                    comandoCount.Parameters.AddWithValue("@limite", limite);
                    resultado.TotalRegistros = Convert.ToInt32(comandoCount.ExecuteScalar());
                }

                var sql = SELECT_BASE + @"
                    WHERE r.estado = 1
                      AND r.terminada = 0
                      AND r.fechaHasta BETWEEN @hoy AND @limite
                    ORDER BY r.fechaHasta
                    LIMIT @tamanio OFFSET @offset";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@hoy", hoy);
                    command.Parameters.AddWithValue("@limite", limite);
                    command.Parameters.AddWithValue("@tamanio", resultado.TamanioPagina);
                    command.Parameters.AddWithValue("@offset", (resultado.PaginaActual - 1) * resultado.TamanioPagina);

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            reservas.Add(LeerReserva(reader));
                        }
                    }
                }
            }
            resultado.Items = reservas;
            return resultado;
        }

        public PaginadoResultado<Reserva> ObtenerPaginado(int pagina, int tamanioPagina, string? busqueda)
        {
            var resultado = new PaginadoResultado<Reserva>
            {
                PaginaActual = pagina < 1 ? 1 : pagina,
                TamanioPagina = tamanioPagina,
                Busqueda = busqueda
            };
            var reservas = new List<Reserva>();

            using (var connection = new MySqlConnection(connectionString))
            {
                var where = "WHERE r.estado = 1";
                if (!string.IsNullOrWhiteSpace(busqueda))
                    where += " AND (q.nombre LIKE @busqueda OR q.apellido LIKE @busqueda OR i.direccion LIKE @busqueda)";

                var sqlCount = $@"SELECT COUNT(*)
                          FROM reservas r
                          INNER JOIN inquilinos q ON r.idInquilino = q.idInquilino
                          INNER JOIN inmuebles i ON r.idInmueble = i.idInmueble
                          {where}";

                var sqlDatos = SELECT_BASE + $@" {where}
                          ORDER BY r.fechaDesde DESC
                          LIMIT @tamanio OFFSET @offset";

                connection.Open();

                using (var comandoCount = new MySqlCommand(sqlCount, connection))
                {
                    if (!string.IsNullOrWhiteSpace(busqueda)) comandoCount.Parameters.AddWithValue("@busqueda", $"%{busqueda}%");
                    resultado.TotalRegistros = Convert.ToInt32(comandoCount.ExecuteScalar());
                }

                using (var command = new MySqlCommand(sqlDatos, connection))
                {
                    if (!string.IsNullOrWhiteSpace(busqueda)) command.Parameters.AddWithValue("@busqueda", $"%{busqueda}%");
                    command.Parameters.AddWithValue("@tamanio", resultado.TamanioPagina);
                    command.Parameters.AddWithValue("@offset", (resultado.PaginaActual - 1) * resultado.TamanioPagina);

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            reservas.Add(LeerReserva(reader));
                        }
                    }
                }
            }

            resultado.Items = reservas;
            return resultado;
        }

        public int Alta(Reserva reserva, int? idUsuarioCreador = null)
        {
            if (reserva.FechaHasta <= reserva.FechaDesde)
                throw new InvalidOperationException("La fecha hasta debe ser posterior a la fecha desde.");

            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                using (var transaccion = connection.BeginTransaction())
                {
                    try
                    {
                        decimal porcentajeSenia = 0;

                        using (var cmdInmueble = new MySqlCommand(
                            "SELECT disponible, estado, porcentajeSenia FROM inmuebles WHERE idInmueble = @id FOR UPDATE",
                            connection, transaccion))
                        {
                            cmdInmueble.Parameters.AddWithValue("@id", reserva.IdInmueble);
                            using (var reader = cmdInmueble.ExecuteReader())
                            {
                                if (!reader.Read())
                                    throw new InvalidOperationException("El inmueble seleccionado no existe.");

                                bool estado = reader.GetBoolean("estado");
                                bool disponible = reader.GetBoolean("disponible");
                                porcentajeSenia = reader.IsDBNull(reader.GetOrdinal("porcentajeSenia"))
                                    ? 0 : reader.GetDecimal("porcentajeSenia");

                                if (!estado)
                                    throw new InvalidOperationException("El inmueble seleccionado ya no está disponible en el sistema.");
                                if (!disponible)
                                    throw new InvalidOperationException("El inmueble está suspendido por el propietario y no puede reservarse.");
                            }
                        }

                        using (var cmdSolape = new MySqlCommand(
                            @"SELECT COUNT(*) FROM reservas
                              WHERE idInmueble = @idInmueble AND estado = 1
                                AND fechaDesde <= @hasta AND fechaHasta >= @desde",
                            connection, transaccion))
                        {
                            cmdSolape.Parameters.AddWithValue("@idInmueble", reserva.IdInmueble);
                            cmdSolape.Parameters.AddWithValue("@desde", reserva.FechaDesde);
                            cmdSolape.Parameters.AddWithValue("@hasta", reserva.FechaHasta);

                            var cantidad = Convert.ToInt32(cmdSolape.ExecuteScalar());
                            if (cantidad > 0)
                                throw new InvalidOperationException("El inmueble ya se encuentra reservado en esas fechas.");
                        }

                        int idReservaNueva;
                        using (var command = new MySqlCommand(
                            @"INSERT INTO reservas
                                (idInquilino, idInmueble, montoPorDia, fechaDesde, fechaHasta,
                                 fechaHastaOriginal, idUsuarioCreador, idReservaOrigen, terminada, estado)
                              VALUES
                                (@idInquilino, @idInmueble, @montoPorDia, @fechaDesde, @fechaHasta,
                                 @fechaHastaOriginal, @idUsuarioCreador, @idReservaOrigen, 0, 1);
                              SELECT LAST_INSERT_ID();",
                            connection, transaccion))
                        {
                            command.Parameters.AddWithValue("@idInquilino", reserva.IdInquilino);
                            command.Parameters.AddWithValue("@idInmueble", reserva.IdInmueble);
                            command.Parameters.AddWithValue("@montoPorDia", reserva.MontoPorDia);
                            command.Parameters.AddWithValue("@fechaDesde", reserva.FechaDesde);
                            command.Parameters.AddWithValue("@fechaHasta", reserva.FechaHasta);
                            command.Parameters.AddWithValue("@fechaHastaOriginal", reserva.FechaHasta);
                            command.Parameters.AddWithValue("@idUsuarioCreador", (object?)idUsuarioCreador ?? DBNull.Value);
                            command.Parameters.AddWithValue("@idReservaOrigen", (object?)reserva.IdReservaOrigen ?? DBNull.Value);

                            idReservaNueva = Convert.ToInt32(command.ExecuteScalar());
                        }
                        reserva.IdReserva = idReservaNueva;

                        
                        if (porcentajeSenia > 0)
                        {
                            var dias = (reserva.FechaHasta.Date - reserva.FechaDesde.Date).Days;
                            var totalReserva = dias * reserva.MontoPorDia;
                            var importeSenia = Math.Round(totalReserva * (porcentajeSenia / 100m), 2);

                            if (importeSenia > 0)
                            {
                                using (var cmdPago = new MySqlCommand(
                                    @"INSERT INTO pagos (idReserva, concepto, fechaPago, importe, anulado, idUsuarioCreador)
                                      VALUES (@idReserva, @concepto, @fechaPago, @importe, 0, @idUsuario)",
                                    connection, transaccion))
                                {
                                    cmdPago.Parameters.AddWithValue("@idReserva", idReservaNueva);
                                    cmdPago.Parameters.AddWithValue("@concepto", $"Seña inicial ({porcentajeSenia:0.##}% del total)");
                                    cmdPago.Parameters.AddWithValue("@fechaPago", DateTime.Today);
                                    cmdPago.Parameters.AddWithValue("@importe", importeSenia);
                                    cmdPago.Parameters.AddWithValue("@idUsuario", (object?)idUsuarioCreador ?? DBNull.Value);
                                    cmdPago.ExecuteNonQuery();
                                }
                            }
                        }

                        transaccion.Commit();
                        return idReservaNueva;
                    }
                    catch
                    {
                        transaccion.Rollback();
                        throw;
                    }
                }
            }
        }

        public int Modificacion(Reserva reserva)
        {
            if (reserva.FechaHasta <= reserva.FechaDesde)
                throw new InvalidOperationException("La fecha hasta debe ser posterior a la fecha desde.");

            if (ExisteSolapamiento(reserva.IdInmueble, reserva.FechaDesde, reserva.FechaHasta, reserva.IdReserva))
                throw new InvalidOperationException("El inmueble ya se encuentra reservado en esas fechas.");

            int res = -1;

            using (var connection = new MySqlConnection(connectionString))
            {
                var sql = @"UPDATE reservas
                            SET montoPorDia = @montoPorDia,
                                fechaDesde = @fechaDesde,
                                fechaHasta = @fechaHasta
                            WHERE idReserva = @id";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@id", reserva.IdReserva);
                    command.Parameters.AddWithValue("@montoPorDia", reserva.MontoPorDia);
                    command.Parameters.AddWithValue("@fechaDesde", reserva.FechaDesde);
                    command.Parameters.AddWithValue("@fechaHasta", reserva.FechaHasta);

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
                var sql = @"UPDATE reservas SET estado = 0 WHERE idReserva = @id";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    connection.Open();
                    res = command.ExecuteNonQuery();
                }
            }
            return res;
        }

        public bool ExisteSolapamiento(int idInmueble, DateTime desde, DateTime hasta, int idReservaExcluir = 0)
        {
            bool existe = false;

            using (var connection = new MySqlConnection(connectionString))
            {
                var sql = @"SELECT COUNT(*) FROM reservas
                            WHERE idInmueble = @idInmueble
                              AND estado = 1
                              AND idReserva <> @idReservaExcluir
                              AND fechaDesde <= @hasta
                              AND fechaHasta >= @desde";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@idInmueble", idInmueble);
                    command.Parameters.AddWithValue("@idReservaExcluir", idReservaExcluir);
                    command.Parameters.AddWithValue("@desde", desde);
                    command.Parameters.AddWithValue("@hasta", hasta);

                    connection.Open();
                    var count = Convert.ToInt32(command.ExecuteScalar());
                    existe = count > 0;
                }
            }
            return existe;
        }

        public decimal CalcularMulta(Reserva reserva, DateTime fechaTerminacionEfectiva)
        {
            var diasOriginales = (reserva.FechaHastaOriginal.Date - reserva.FechaDesde.Date).Days;
            var diasCumplidos = (fechaTerminacionEfectiva.Date - reserva.FechaDesde.Date).Days;
            var diasRestantes = (reserva.FechaHastaOriginal.Date - fechaTerminacionEfectiva.Date).Days;

            if (diasRestantes < 0) diasRestantes = 0;

            var montoRestante = diasRestantes * reserva.MontoPorDia;
            var porcentaje = diasCumplidos < (diasOriginales / 2m) ? 0.50m : 0.25m;

            return Math.Round(montoRestante * porcentaje, 2);
        }

        public decimal TerminarAnticipadamente(int idReserva, DateTime fechaTerminacionEfectiva, int idUsuarioTerminador)
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                using (var transaccion = connection.BeginTransaction())
                {
                    try
                    {
                        Reserva reserva;

                        using (var comandoSelect = new MySqlCommand(
                            @"SELECT idReserva, idInquilino, idInmueble, montoPorDia, fechaDesde,
                                     fechaHasta, fechaHastaOriginal, terminada
                              FROM reservas
                              WHERE idReserva = @id
                              FOR UPDATE",
                            connection, transaccion))
                        {
                            comandoSelect.Parameters.AddWithValue("@id", idReserva);

                            using (var reader = comandoSelect.ExecuteReader())
                            {
                                if (!reader.Read())
                                    throw new InvalidOperationException("La reserva no existe.");

                                reserva = new Reserva
                                {
                                    IdReserva = reader.GetInt32("idReserva"),
                                    IdInquilino = reader.GetInt32("idInquilino"),
                                    IdInmueble = reader.GetInt32("idInmueble"),
                                    MontoPorDia = reader.GetDecimal("montoPorDia"),
                                    FechaDesde = reader.GetDateTime("fechaDesde"),
                                    FechaHasta = reader.GetDateTime("fechaHasta"),
                                    FechaHastaOriginal = reader.GetDateTime("fechaHastaOriginal"),
                                    Terminada = reader.GetBoolean("terminada")
                                };
                            }
                        }

                        if (reserva.Terminada)
                            throw new InvalidOperationException("La reserva ya fue terminada anteriormente.");

                        if (fechaTerminacionEfectiva.Date < reserva.FechaDesde.Date)
                            throw new InvalidOperationException("La fecha de terminación no puede ser anterior al inicio de la reserva.");

                        if (fechaTerminacionEfectiva.Date >= reserva.FechaHastaOriginal.Date)
                            throw new InvalidOperationException("Esa fecha no adelanta el fin de la reserva; no corresponde terminarla anticipadamente.");

                        var multa = CalcularMulta(reserva, fechaTerminacionEfectiva);

                        using (var comandoPago = new MySqlCommand(
                            @"INSERT INTO pagos (idReserva, concepto, fechaPago, importe, anulado, idUsuarioCreador)
                              VALUES (@idReserva, @concepto, @fechaPago, @importe, 0, @idUsuario)",
                            connection, transaccion))
                        {
                            comandoPago.Parameters.AddWithValue("@idReserva", idReserva);
                            comandoPago.Parameters.AddWithValue("@concepto", "Multa por terminación anticipada");
                            comandoPago.Parameters.AddWithValue("@fechaPago", DateTime.Today);
                            comandoPago.Parameters.AddWithValue("@importe", multa);
                            comandoPago.Parameters.AddWithValue("@idUsuario", idUsuarioTerminador);
                            comandoPago.ExecuteNonQuery();
                        }

                        using (var comandoUpdate = new MySqlCommand(
                            @"UPDATE reservas
                              SET terminada = 1,
                                  fechaTerminacionEfectiva = @fechaEfectiva,
                                  fechaHasta = @fechaEfectiva,
                                  multa = @multa,
                                  idUsuarioTerminador = @idUsuario
                              WHERE idReserva = @id",
                            connection, transaccion))
                        {
                            comandoUpdate.Parameters.AddWithValue("@fechaEfectiva", fechaTerminacionEfectiva.Date);
                            comandoUpdate.Parameters.AddWithValue("@multa", multa);
                            comandoUpdate.Parameters.AddWithValue("@idUsuario", idUsuarioTerminador);
                            comandoUpdate.Parameters.AddWithValue("@id", idReserva);
                            comandoUpdate.ExecuteNonQuery();
                        }

                        transaccion.Commit();
                        return multa;
                    }
                    catch
                    {
                        transaccion.Rollback();
                        throw;
                    }
                }
            }
        }

        private Reserva LeerReserva(MySqlDataReader reader)
        {
            return new Reserva
            {
                IdReserva = reader.GetInt32("idReserva"),
                IdInquilino = reader.GetInt32("idInquilino"),
                IdInmueble = reader.GetInt32("idInmueble"),
                MontoPorDia = reader.GetDecimal("montoPorDia"),
                FechaDesde = reader.GetDateTime("fechaDesde"),
                FechaHasta = reader.GetDateTime("fechaHasta"),
                FechaHastaOriginal = reader.GetDateTime("fechaHastaOriginal"),
                FechaTerminacionEfectiva = reader.IsDBNull(reader.GetOrdinal("fechaTerminacionEfectiva"))
                    ? null : reader.GetDateTime("fechaTerminacionEfectiva"),
                Multa = reader.IsDBNull(reader.GetOrdinal("multa"))
                    ? null : reader.GetDecimal("multa"),
                Terminada = reader.GetBoolean("terminada"),
                IdUsuarioCreador = reader.IsDBNull(reader.GetOrdinal("idUsuarioCreador"))
                    ? null : reader.GetInt32("idUsuarioCreador"),
                IdUsuarioTerminador = reader.IsDBNull(reader.GetOrdinal("idUsuarioTerminador"))
                    ? null : reader.GetInt32("idUsuarioTerminador"),
                IdReservaOrigen = reader.IsDBNull(reader.GetOrdinal("idReservaOrigen"))
                    ? null : reader.GetInt32("idReservaOrigen"),
                NombreInquilino = reader.GetString("NombreInquilino"),
                DireccionInmueble = reader.GetString("DireccionInmueble"),
                NombreUsuarioCreador = reader.IsDBNull(reader.GetOrdinal("NombreUsuarioCreador"))
                    ? null : reader.GetString("NombreUsuarioCreador"),
                NombreUsuarioTerminador = reader.IsDBNull(reader.GetOrdinal("NombreUsuarioTerminador"))
                    ? null : reader.GetString("NombreUsuarioTerminador")
            };
        }
    }
}