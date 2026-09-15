using MySql.Data.MySqlClient;

namespace ProyectoInmobiliaria_Hoyo_Mazza_Rodriguez.Models
{
    public class RepositorioUsuario
    {
        private readonly string connectionString;

        public RepositorioUsuario(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection")!;
        }

        public List<Usuario> ObtenerTodos()
        {
            var usuarios = new List<Usuario>();
            using (var connection = new MySqlConnection(connectionString))
            {
                var sql = @"SELECT idUsuario, email, clave, nombre, apellido, rol, avatar, estado
                            FROM usuarios WHERE estado = 1";
                using (var command = new MySqlCommand(sql, connection))
                {
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            usuarios.Add(LeerUsuario(reader));
                        }
                    }
                }
            }
            return usuarios;
        }

        public Usuario? ObtenerPorId(int id)
        {
            Usuario? usuario = null;
            using (var connection = new MySqlConnection(connectionString))
            {
                var sql = @"SELECT idUsuario, email, clave, nombre, apellido, rol, avatar, estado
                            FROM usuarios WHERE idUsuario = @id";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read()) usuario = LeerUsuario(reader);
                    }
                }
            }
            return usuario;
        }

        public Usuario? ObtenerPorEmail(string email)
        {
            Usuario? usuario = null;
            using (var connection = new MySqlConnection(connectionString))
            {
                var sql = @"SELECT idUsuario, email, clave, nombre, apellido, rol, avatar, estado
                            FROM usuarios WHERE email = @email AND estado = 1";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@email", email);
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read()) usuario = LeerUsuario(reader);
                    }
                }
            }
            return usuario;
        }

        public int Alta(Usuario usuario)
        {
            int res = -1;
            using (var connection = new MySqlConnection(connectionString))
            {
                var sql = @"INSERT INTO usuarios (email, clave, nombre, apellido, rol, avatar, estado)
                            VALUES (@email, @clave, @nombre, @apellido, @rol, @avatar, 1);
                            SELECT LAST_INSERT_ID();";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@email", usuario.Email);
                    command.Parameters.AddWithValue("@clave", usuario.Clave);
                    command.Parameters.AddWithValue("@nombre", usuario.Nombre);
                    command.Parameters.AddWithValue("@apellido", usuario.Apellido);
                    command.Parameters.AddWithValue("@rol", usuario.Rol);
                    command.Parameters.AddWithValue("@avatar", (object?)usuario.Avatar ?? DBNull.Value);

                    connection.Open();
                    res = Convert.ToInt32(command.ExecuteScalar());
                    usuario.IdUsuario = res;
                }
            }
            return res;
        }

        public int Modificacion(Usuario usuario)
        {
            int res = -1;
            using (var connection = new MySqlConnection(connectionString))
            {
                var sql = @"UPDATE usuarios
                            SET nombre = @nombre, apellido = @apellido, avatar = @avatar, rol = @rol
                            WHERE idUsuario = @id";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@id", usuario.IdUsuario);
                    command.Parameters.AddWithValue("@nombre", usuario.Nombre);
                    command.Parameters.AddWithValue("@apellido", usuario.Apellido);
                    command.Parameters.AddWithValue("@avatar", (object?)usuario.Avatar ?? DBNull.Value);
                    command.Parameters.AddWithValue("@rol", usuario.Rol);

                    connection.Open();
                    res = command.ExecuteNonQuery();
                }
            }
            return res;
        }

        public int CambiarClave(int idUsuario, string claveHasheada)
        {
            int res = -1;
            using (var connection = new MySqlConnection(connectionString))
            {
                var sql = @"UPDATE usuarios SET clave = @clave WHERE idUsuario = @id";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@id", idUsuario);
                    command.Parameters.AddWithValue("@clave", claveHasheada);
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
                var sql = @"UPDATE usuarios SET estado = 0 WHERE idUsuario = @id";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    connection.Open();
                    res = command.ExecuteNonQuery();
                }
            }
            return res;
        }

        public bool ExisteEmail(string email, int idExcluir = 0)
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                var sql = @"SELECT COUNT(*) FROM usuarios WHERE email = @email AND idUsuario <> @idExcluir";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@email", email);
                    command.Parameters.AddWithValue("@idExcluir", idExcluir);
                    connection.Open();
                    return Convert.ToInt32(command.ExecuteScalar()) > 0;
                }
            }
        }

        private Usuario LeerUsuario(MySqlDataReader reader)
        {
            return new Usuario
            {
                IdUsuario = reader.GetInt32("idUsuario"),
                Email = reader.GetString("email"),
                Clave = reader.GetString("clave"),
                Nombre = reader.GetString("nombre"),
                Apellido = reader.GetString("apellido"),
                Rol = reader.GetString("rol"),
                Avatar = reader.IsDBNull(reader.GetOrdinal("avatar")) ? null : reader.GetString("avatar"),
                Estado = reader.GetBoolean("estado")
            };
        }
    }
}