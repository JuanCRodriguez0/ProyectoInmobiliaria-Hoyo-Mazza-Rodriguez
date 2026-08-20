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
        public List<Inquilinos> ObtenerTodos()
        {
            var inquilinos = new List<Inquilinos>();

            using (var connection = new MySqlConnection(connectionString))
            {
                var sql = @"SELECT IdInquilino, Dni, Nombre, Apellido, FechaNacimiento, Telefono, Email, Garantes, Sueldo 
                            FROM inquilinos";

                using (var command = new MySqlCommand(sql, connection))
                {
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            inquilinos.Add(new Inquilinos
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
        public Inquilinos? ObtenerPorId(int id)
        {
            Inquilinos? inquilino = null;

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
                            inquilino = new Inquilinos
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
        public int Alta(Inquilinos inquilino)
        {
            int res = -1;

            using (var connection = new MySqlConnection(connectionString))
            {
                var sql = @"INSERT INTO inquilinos (Dni, Nombre, Apellido, FechaNacimiento, Telefono, Email, Garantes, Sueldo) 
                            VALUES (@dni, @nombre, @apellido, @fechaNacimiento, @telefono, @email, @garantes, @sueldo);
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

                    connection.Open();
                    res = Convert.ToInt32(command.ExecuteScalar());
                    inquilino.IdInquilino = res;
                }
            }
            return res;
        }

        // 4. MODIFICACIÓN DE INQUILINO
        public int Modificacion(Inquilinos inquilino)
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
                var sql = @"DELETE FROM inquilinos WHERE IdInquilino = @id";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@id", id);

                    connection.Open();
                    res = command.ExecuteNonQuery();
                }
            }
            return res;
        }
    }
}