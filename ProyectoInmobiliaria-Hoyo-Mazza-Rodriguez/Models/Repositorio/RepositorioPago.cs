using MySql.Data.MySqlClient;

namespace ProyectoInmobiliaria_Hoyo_Mazza_Rodriguez.Models
{
    public class RepositorioPago
    {
        private readonly string connectionString;

        public RepositorioPago(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection")!;
        }

        private const string SELECT_BASE = @"
            SELECT p.idPago, p.idReserva, p.concepto, p.fechaPago, p.importe, p.anulado,
                   p.idUsuarioCreador, p.idUsuarioAnulador,
                   CONCAT(uc.nombre, ' ', uc.apellido) AS NombreCreador,
                   CONCAT(ua.nombre, ' ', ua.apellido) AS NombreAnulador,
                   CONCAT(q.nombre, ' ', q.apellido)   AS NombreInquilino,
                   i.direccion                         AS DireccionInmueble
            FROM pagos p
            INNER JOIN reservas   r  ON p.idReserva = r.idReserva
            INNER JOIN inquilinos q  ON r.idInquilino = q.idInquilino
            INNER JOIN inmuebles  i  ON r.idInmueble = i.idInmueble
            LEFT  JOIN usuarios   uc ON p.idUsuarioCreador  = uc.idUsuario
            LEFT  JOIN usuarios   ua ON p.idUsuarioAnulador = ua.idUsuario";


        public List<Pago> ObtenerPorReserva(int idReserva, int pagina = 1, int tamPagina = 10)
        {
            var pagos = new List<Pago>();

            if (pagina < 1) pagina = 1;
            if (tamPagina < 1) tamPagina = 10;

            using (var connection = new MySqlConnection(connectionString))
            {
                var sql = SELECT_BASE + @"
                    WHERE p.idReserva = @idReserva
                    ORDER BY p.fechaPago DESC, p.idPago DESC
                    LIMIT @tamPagina OFFSET @offset";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@idReserva", idReserva);
                    command.Parameters.AddWithValue("@tamPagina", tamPagina);
                    command.Parameters.AddWithValue("@offset", (pagina - 1) * tamPagina);

                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            pagos.Add(LeerPago(reader));
                        }
                    }
                }
            }
            return pagos;
        }


        public int ContarPorReserva(int idReserva)
        {
            int total = 0;

            using (var connection = new MySqlConnection(connectionString))
            {
                var sql = "SELECT COUNT(*) FROM pagos WHERE idReserva = @idReserva";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@idReserva", idReserva);
                    connection.Open();
                    total = Convert.ToInt32(command.ExecuteScalar());
                }
            }
            return total;
        }

        public Pago? ObtenerPorId(int id)
        {
            Pago? pago = null;

            using (var connection = new MySqlConnection(connectionString))
            {
                var sql = SELECT_BASE + " WHERE p.idPago = @id";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    connection.Open();

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            pago = LeerPago(reader);
                        }
                    }
                }
            }
            return pago;
        }


        public decimal TotalPagadoPorReserva(int idReserva)
        {
            decimal total = 0;

            using (var connection = new MySqlConnection(connectionString))
            {
                var sql = @"SELECT COALESCE(SUM(importe), 0)
                            FROM pagos
                            WHERE idReserva = @idReserva AND anulado = 0";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@idReserva", idReserva);
                    connection.Open();
                    total = Convert.ToDecimal(command.ExecuteScalar());
                }
            }
            return total;
        }

        public int Alta(Pago pago)
        {
            int res = -1;

            using (var connection = new MySqlConnection(connectionString))
            {
                var sql = @"INSERT INTO pagos (idReserva, concepto, fechaPago, importe, anulado, idUsuarioCreador)
                            VALUES (@idReserva, @concepto, @fechaPago, @importe, 0, @idUsuarioCreador);
                            SELECT LAST_INSERT_ID();";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@idReserva", pago.IdReserva);
                    command.Parameters.AddWithValue("@concepto", pago.Concepto);
                    command.Parameters.AddWithValue("@fechaPago", pago.FechaPago);
                    command.Parameters.AddWithValue("@importe", pago.Importe);
                    command.Parameters.AddWithValue("@idUsuarioCreador", (object?)pago.IdUsuarioCreador ?? DBNull.Value);

                    connection.Open();
                    res = Convert.ToInt32(command.ExecuteScalar());
                    pago.IdPago = res;
                }
            }
            return res;
        }


        public int ModificarConcepto(int idPago, string concepto)
        {
            int res = -1;

            using (var connection = new MySqlConnection(connectionString))
            {
                var sql = @"UPDATE pagos 
                    SET concepto = @concepto 
                    WHERE idPago = @id 
                      AND anulado = 0";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@id", idPago);
                    command.Parameters.AddWithValue("@concepto", concepto);

                    connection.Open();
                    res = command.ExecuteNonQuery();
                }
            }

            return res;
        }


        public int Anular(int idPago, int idUsuarioAnulador)
        {
            int res = -1;

            using (var connection = new MySqlConnection(connectionString))
            {
                var sql = @"UPDATE pagos
                            SET anulado = 1, idUsuarioAnulador = @idUsuarioAnulador
                            WHERE idPago = @id AND anulado = 0";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@id", idPago);
                    command.Parameters.AddWithValue("@idUsuarioAnulador", idUsuarioAnulador);

                    connection.Open();
                    res = command.ExecuteNonQuery();
                }
            }
            return res;
        }

        private Pago LeerPago(MySqlDataReader reader)
        {
            return new Pago
            {
                IdPago = reader.GetInt32("idPago"),
                IdReserva = reader.GetInt32("idReserva"),
                Concepto = reader.GetString("concepto"),
                FechaPago = reader.GetDateTime("fechaPago"),
                Importe = reader.GetDecimal("importe"),
                Anulado = reader.GetBoolean("anulado"),
                IdUsuarioCreador = reader.IsDBNull(reader.GetOrdinal("idUsuarioCreador"))
                    ? null : reader.GetInt32("idUsuarioCreador"),
                IdUsuarioAnulador = reader.IsDBNull(reader.GetOrdinal("idUsuarioAnulador"))
                    ? null : reader.GetInt32("idUsuarioAnulador"),
                NombreCreador = reader.IsDBNull(reader.GetOrdinal("NombreCreador"))
                    ? null : reader.GetString("NombreCreador"),
                NombreAnulador = reader.IsDBNull(reader.GetOrdinal("NombreAnulador"))
                    ? null : reader.GetString("NombreAnulador"),
                NombreInquilino = reader.GetString("NombreInquilino"),
                DireccionInmueble = reader.GetString("DireccionInmueble")
            };
        }
    }
}