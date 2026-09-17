using MySql.Data.MySqlClient;
using ProyectoInmobiliaria_Hoyo_Mazza_Rodriguez.Models;

namespace ProyectoInmobiliaria_Hoyo_Mazza_Rodriguez.Models
{
    public class RepositorioInquilino
    {
        private readonly string connectionString;

        public RepositorioInquilino(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection")!;
        }

        // 1. OBTENER TODOS LOS INQUILINOS
        public List<Inquilino> ObtenerTodos()
        {
            var inquilinos = new List<Inquilino>();

            using (var connection = new MySqlConnection(connectionString))
            {
                var sql = @"SELECT IdInquilino, Dni, Nombre, Apellido, FechaNacimiento, Telefono, Email, Garantes, Sueldo 
                            FROM inquilinos WHERE estado = 1";

                using (var command = new MySqlCommand(sql, connection))
                {
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            inquilinos.Add(new Inquilino
                            {
                                IdInquilino = reader.GetInt32("IdInquilino"),
                                Dni = reader.GetString("Dni"),
                                Nombre = reader.GetString("Nombre"),
                                Apellido = reader.GetString("Apellido"),
                                FechaNacimiento = reader.GetDateTime("FechaNacimiento"),
                                Telefono = reader.GetString("Telefono"),
                                Email = reader.GetString("Email"),
                                Garantes = reader.GetString("Garantes"),
                                Sueldo = reader.GetDecimal("Sueldo")
                            });
                        }
                    }
                }
            }
            return inquilinos;
        }

        // 2. OBTENER UN INQUILINO POR ID
        public Inquilino? ObtenerPorId(int id)
        {
            Inquilino? inquilino = null;

            using (var connection = new MySqlConnection(connectionString))
            {
                var sql = @"SELECT IdInquilino, Dni, Nombre, Apellido, FechaNacimiento, Telefono, Email, Garantes, Sueldo 
                            FROM inquilinos 
                            WHERE IdInquilino = @id";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    connection.Open();

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            inquilino = new Inquilino
                            {
                                IdInquilino = reader.GetInt32("IdInquilino"),
                                Dni = reader.GetString("Dni"),
                                Nombre = reader.GetString("Nombre"),
                                Apellido = reader.GetString("Apellido"),
                                FechaNacimiento = reader.GetDateTime("FechaNacimiento"),
                                Telefono = reader.GetString("Telefono"),
                                Email = reader.GetString("Email"),
                                Garantes = reader.GetString("Garantes"),
                                Sueldo = reader.GetDecimal("Sueldo")
                            };
                        }
                    }
                }
            }
            return inquilino;
        }

        // 3. GUARDAR (ALTA DE INQUILINO)
        public int Alta(Inquilino inquilino)
        {
            int res = -1;

            using (var connection = new MySqlConnection(connectionString))
            {
                var sql = @"INSERT INTO inquilinos (Dni, Nombre, Apellido, FechaNacimiento, Telefono, Email, Garantes, Sueldo, estado) 
                            VALUES (@dni, @nombre, @apellido, @fechaNacimiento, @telefono, @email, @garantes, @sueldo, @estado);
                            SELECT LAST_INSERT_ID();";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@dni", inquilino.Dni);
                    command.Parameters.AddWithValue("@nombre", inquilino.Nombre);
                    command.Parameters.AddWithValue("@apellido", inquilino.Apellido);
                    command.Parameters.AddWithValue("@fechaNacimiento", inquilino.FechaNacimiento);
                    command.Parameters.AddWithValue("@telefono", inquilino.Telefono);
                    command.Parameters.AddWithValue("@email", inquilino.Email);
                    command.Parameters.AddWithValue("@garantes", inquilino.Garantes);
                    command.Parameters.AddWithValue("@sueldo", inquilino.Sueldo);
                    command.Parameters.AddWithValue("@estado", true);

                    connection.Open();
                    res = Convert.ToInt32(command.ExecuteScalar());
                    inquilino.IdInquilino = res;
                }
            }
            return res;
        }

        // 4. MODIFICACIÓN DE INQUILINO
        public int Modificacion(Inquilino inquilino)
        {
            int res = -1;

            using (var connection = new MySqlConnection(connectionString))
            {
                var sql = @"UPDATE inquilinos 
                            SET Dni = @dni, 
                                Nombre = @nombre, 
                                Apellido = @apellido, 
                                FechaNacimiento = @fechaNacimiento, 
                                Telefono = @telefono, 
                                Email = @email, 
                                Garantes = @garantes, 
                                Sueldo = @sueldo 
                            WHERE IdInquilino = @id";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@id", inquilino.IdInquilino);
                    command.Parameters.AddWithValue("@dni", inquilino.Dni);
                    command.Parameters.AddWithValue("@nombre", inquilino.Nombre);
                    command.Parameters.AddWithValue("@apellido", inquilino.Apellido);
                    command.Parameters.AddWithValue("@fechaNacimiento", inquilino.FechaNacimiento);
                    command.Parameters.AddWithValue("@telefono", inquilino.Telefono);
                    command.Parameters.AddWithValue("@email", inquilino.Email);
                    command.Parameters.AddWithValue("@garantes", inquilino.Garantes);
                    command.Parameters.AddWithValue("@sueldo", inquilino.Sueldo);

                    connection.Open();
                    res = command.ExecuteNonQuery();
                }
            }
            return res;
        }

        // 5. BAJA (ELIMINACIÓN DE INQUILINO)
        public int Baja(int id)
        {
            int res = -1;

            using (var connection = new MySqlConnection(connectionString))
            {
                var sql = @"UPDATE inquilinos SET estado = 0 WHERE idInquilino = @id ";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@id", id);

                    connection.Open();
                    res = command.ExecuteNonQuery();
                }
            }
            return res;
        }

        public PaginadoResultado<Inquilino> ObtenerPaginado(int pagina, int tamanioPagina, string? busqueda)
        {
            var resultado = new PaginadoResultado<Inquilino>
            {
                PaginaActual = pagina < 1 ? 1 : pagina,
                TamanioPagina = tamanioPagina,
                Busqueda = busqueda
            };
            var inquilinos = new List<Inquilino>();

            using (var connection = new MySqlConnection(connectionString))
            {
                var where = "WHERE estado = 1";
                if (!string.IsNullOrWhiteSpace(busqueda))
                    where += " AND (Nombre LIKE @busqueda OR Apellido LIKE @busqueda OR Dni LIKE @busqueda)";

                var sqlCount = $"SELECT COUNT(*) FROM inquilinos {where}";
                var sqlDatos = $@"SELECT IdInquilino, Dni, Nombre, Apellido, FechaNacimiento, Telefono, Email, Garantes, Sueldo
                          FROM inquilinos {where}
                          ORDER BY Apellido, Nombre
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
                            inquilinos.Add(new Inquilino
                            {
                                IdInquilino = reader.GetInt32("IdInquilino"),
                                Dni = reader.GetString("Dni"),
                                Nombre = reader.GetString("Nombre"),
                                Apellido = reader.GetString("Apellido"),
                                FechaNacimiento = reader.GetDateTime("FechaNacimiento"),
                                Telefono = reader.GetString("Telefono"),
                                Email = reader.GetString("Email"),
                                Garantes = reader.GetString("Garantes"),
                                Sueldo = reader.GetDecimal("Sueldo")
                            });
                        }
                    }
                }
            }

            resultado.Items = inquilinos;
            return resultado;
        }

        public List<Inquilino> BuscarPorTexto(string term, int limite = 10)
        {
            var inquilinos = new List<Inquilino>();

            using (var connection = new MySqlConnection(connectionString))
            {
                var sql = @"SELECT IdInquilino, Dni, Nombre, Apellido, FechaNacimiento, Telefono, Email, Garantes, Sueldo
                    FROM inquilinos
                    WHERE estado = 1 AND (Nombre LIKE @term OR Apellido LIKE @term OR Dni LIKE @term)
                    ORDER BY Apellido, Nombre
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
                            inquilinos.Add(new Inquilino
                            {
                                IdInquilino = reader.GetInt32("IdInquilino"),
                                Dni = reader.GetString("Dni"),
                                Nombre = reader.GetString("Nombre"),
                                Apellido = reader.GetString("Apellido"),
                                FechaNacimiento = reader.GetDateTime("FechaNacimiento"),
                                Telefono = reader.GetString("Telefono"),
                                Email = reader.GetString("Email"),
                                Garantes = reader.GetString("Garantes"),
                                Sueldo = reader.GetDecimal("Sueldo")
                            });
                        }
                    }
                }
            }
            return inquilinos;
        }
    }
}