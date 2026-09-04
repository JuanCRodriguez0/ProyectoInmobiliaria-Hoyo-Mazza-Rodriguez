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
        public List<Reserva> ObtenerTodos()
        {
            var reservas = new List<Reserva>();

            using (var connection = new MySqlConnection(connectionString))
            {
                var sql = @"SELECT r.idReserva, r.idInquilino, r.idInmueble, r.montoPorDia, r.fechaDesde, r.fechaHasta,
                                   CONCAT(q.nombre, ' ', q.apellido) AS NombreInquilino,
                                   i.direccion AS DireccionInmueble
                            FROM reservas r
                            INNER JOIN inquilinos q ON r.idInquilino = q.idInquilino
                            INNER JOIN inmuebles i ON r.idInmueble = i.idInmueble
                            WHERE r.estado = 1
                            ORDER BY r.fechaDesde DESC";

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
                var sql = @"SELECT r.idReserva, r.idInquilino, r.idInmueble, r.montoPorDia, r.fechaDesde, r.fechaHasta,
                                   CONCAT(q.nombre, ' ', q.apellido) AS NombreInquilino,
                                   i.direccion AS DireccionInmueble
                            FROM reservas r
                            INNER JOIN inquilinos q ON r.idInquilino = q.idInquilino
                            INNER JOIN inmuebles i ON r.idInmueble = i.idInmueble
                            WHERE r.idReserva = @id";

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
                var sql = @"SELECT r.idReserva, r.idInquilino, r.idInmueble, r.montoPorDia, r.fechaDesde, r.fechaHasta,
                                   CONCAT(q.nombre, ' ', q.apellido) AS NombreInquilino,
                                   i.direccion AS DireccionInmueble
                            FROM reservas r
                            INNER JOIN inquilinos q ON r.idInquilino = q.idInquilino
                            INNER JOIN inmuebles i ON r.idInmueble = i.idInmueble
                            WHERE r.idInmueble = @idInmueble AND r.estado = 1
                            ORDER BY r.fechaDesde DESC";

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

        public int Alta(Reserva reserva)
        {
            int res = -1;

            using (var connection = new MySqlConnection(connectionString))
            {
                var sql = @"INSERT INTO reservas (idInquilino, idInmueble, montoPorDia, fechaDesde, fechaHasta, estado)
                            VALUES (@idInquilino, @idInmueble, @montoPorDia, @fechaDesde, @fechaHasta, @estado);
                            SELECT LAST_INSERT_ID();";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@idInquilino", reserva.IdInquilino);
                    command.Parameters.AddWithValue("@idInmueble", reserva.IdInmueble);
                    command.Parameters.AddWithValue("@montoPorDia", reserva.MontoPorDia);
                    command.Parameters.AddWithValue("@fechaDesde", reserva.FechaDesde);
                    command.Parameters.AddWithValue("@fechaHasta", reserva.FechaHasta);
                    command.Parameters.AddWithValue("@estado", true);

                    connection.Open();
                    res = Convert.ToInt32(command.ExecuteScalar());
                    reserva.IdReserva = res;
                }
            }
            return res;
        }

        public int Modificacion(Reserva reserva)
        {
            int res = -1;

            using (var connection = new MySqlConnection(connectionString))
            {
                var sql = @"UPDATE reservas
                            SET idInquilino = @idInquilino,
                                idInmueble = @idInmueble,
                                montoPorDia = @montoPorDia,
                                fechaDesde = @fechaDesde,
                                fechaHasta = @fechaHasta
                            WHERE idReserva = @id";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@id", reserva.IdReserva);
                    command.Parameters.AddWithValue("@idInquilino", reserva.IdInquilino);
                    command.Parameters.AddWithValue("@idInmueble", reserva.IdInmueble);
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
                NombreInquilino = reader.GetString("NombreInquilino"),
                DireccionInmueble = reader.GetString("DireccionInmueble")
            };
        }
    }
}
