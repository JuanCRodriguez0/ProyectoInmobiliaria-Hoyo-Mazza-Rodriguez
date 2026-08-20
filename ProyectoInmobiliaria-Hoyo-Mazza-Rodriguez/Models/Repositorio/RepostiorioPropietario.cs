using MySql.Data.MySqlClient;
using ProyectoInmobiliaria_Hoyo_Mazza_Rodriguez.Models;

namespace ProyectoInmobiliaria_Hoyo_Mazza_Rodriguez.Models
{
    public class RepositorioPropietario
    {
        private readonly string connectionString;

        public RepositorioPropietario(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection")!;
        }

        // 1. OBTENER TODOS LOS PROPIETARIOS
        public List<Propietario> ObtenerTodos()
        {
            var propietarios = new List<Propietario>();

            using (var connection = new MySqlConnection(connectionString))
            {
                var sql = @"SELECT IdPropietario, Dni, Nombre, Apellido, FechaNacimiento, Direccion, Telefono, Email 
                            FROM propietarios";

                using (var command = new MySqlCommand(sql, connection))
                {
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            propietarios.Add(new Propietario
                            {
                                IdPropietario = reader.GetInt32("IdPropietario"),
                                Dni = reader.GetString("Dni"),
                                Nombre = reader.GetString("Nombre"),
                                Apellido = reader.GetString("Apellido"),
                                FechaNacimiento = reader.GetDateTime("FechaNacimiento"),
                                Direccion = reader.GetString("Direccion"),
                                Telefono = reader.GetString("Telefono"),
                                Email = reader.GetString("Email")
                            });
                        }
                    }
                }
            }
            return propietarios;
        }

        // 2. OBTENER UN PROPIETARIO POR ID
        public Propietario? ObtenerPorId(int id)
        {
            Propietario? propietario = null;

            using (var connection = new MySqlConnection(connectionString))
            {
                var sql = @"SELECT IdPropietario, Dni, Nombre, Apellido, FechaNacimiento, Direccion, Telefono, Email 
                            FROM propietarios 
                            WHERE IdPropietario = @id";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    connection.Open();

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            propietario = new Propietario
                            {
                                IdPropietario = reader.GetInt32("IdPropietario"),
                                Dni = reader.GetString("Dni"),
                                Nombre = reader.GetString("Nombre"),
                                Apellido = reader.GetString("Apellido"),
                                FechaNacimiento = reader.GetDateTime("FechaNacimiento"),
                                Direccion = reader.GetString("Direccion"),
                                Telefono = reader.GetString("Telefono"),
                                Email = reader.GetString("Email")
                            };
                        }
                    }
                }
            }
            return propietario;
        }

        // 3. GUARDAR (ALTA DE PROPIETARIO)
        public int Alta(Propietario propietario)
        {
            int res = -1;

            using (var connection = new MySqlConnection(connectionString))
            {
                var sql = @"INSERT INTO propietarios (Dni, Nombre, Apellido, FechaNacimiento, Direccion, Telefono, Email) 
                            VALUES (@dni, @nombre, @apellido, @fechaNacimiento, @direccion, @telefono, @email);
                            SELECT LAST_INSERT_ID();";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@dni", propietario.Dni);
                    command.Parameters.AddWithValue("@nombre", propietario.Nombre);
                    command.Parameters.AddWithValue("@apellido", propietario.Apellido);
                    command.Parameters.AddWithValue("@fechaNacimiento", propietario.FechaNacimiento);
                    command.Parameters.AddWithValue("@direccion", propietario.Direccion);
                    command.Parameters.AddWithValue("@telefono", propietario.Telefono);
                    command.Parameters.AddWithValue("@email", propietario.Email);

                    connection.Open();
                    res = Convert.ToInt32(command.ExecuteScalar());
                    propietario.IdPropietario = res;
                }
            }
            return res;
        }

        // 4. MODIFICACIÓN DE PROPIETARIO
        public int Modificacion(Propietario propietario)
        {
            int res = -1;

            using (var connection = new MySqlConnection(connectionString))
            {
                var sql = @"UPDATE propietarios 
                            SET Dni = @dni, 
                                Nombre = @nombre, 
                                Apellido = @apellido, 
                                FechaNacimiento = @fechaNacimiento, 
                                Direccion = @direccion, 
                                Telefono = @telefono, 
                                Email = @email 
                            WHERE IdPropietario = @id";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@id", propietario.IdPropietario);
                    command.Parameters.AddWithValue("@dni", propietario.Dni);
                    command.Parameters.AddWithValue("@nombre", propietario.Nombre);
                    command.Parameters.AddWithValue("@apellido", propietario.Apellido);
                    command.Parameters.AddWithValue("@fechaNacimiento", propietario.FechaNacimiento);
                    command.Parameters.AddWithValue("@direccion", propietario.Direccion);
                    command.Parameters.AddWithValue("@telefono", propietario.Telefono);
                    command.Parameters.AddWithValue("@email", propietario.Email);

                    connection.Open();
                    res = command.ExecuteNonQuery();
                }
            }
            return res;
        }

        // 5. BAJA (ELIMINACIÓN DE PROPIETARIO)
        public int Baja(int id)
        {
            int res = -1;

            using (var connection = new MySqlConnection(connectionString))
            {
                var sql = @"UPDATE propietarios SET estado = 0 WHERE IdPropietario = @id ";

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