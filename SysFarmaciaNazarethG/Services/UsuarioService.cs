
using SysFarmaciaNazarethG.Models;
using System.Security.Cryptography;
using System.Text;

namespace  SysFarmaciaNazarethG.Services
{
    public class UsuarioService
    {
        private readonly BDContext _context;

        // Constructor que inyecta el contexto de la base de datos
        public UsuarioService(BDContext context)
        {
            _context = context;
        }

        // Método para validar las credenciales del usuario (login y contraseña)
        public Usuario? ValidarUsuario(string login, string password)
        {
            // Encriptar la contraseña ingresada con MD5
            string passwordEncriptada = ConvertirMD5(password);

            // Buscar el usuario en la base de datos con el login y contraseña encriptada
            var usuario = _context.Usuario
                .FirstOrDefault(u => u.Login == login && u.Password == passwordEncriptada);

            return usuario;
        }

        // Método para convertir la contraseña encriptada en formato MD5
        private string ConvertirMD5(string input)
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[] inputBytes = Encoding.ASCII.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                // Convertir el array de bytes en una cadena hexadecimal
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("X2")); // Convertir a hexadecimal en mayúsculas
                }
                return sb.ToString();
            }
        }

        // Método para obtener un usuario por su ID
        public Usuario? ObtenerUsuarioPorId(int id)
        {
            return _context.Usuario.Find(id);
        }

        // Método para agregar un nuevo usuario
        public void AgregarUsuario(Usuario usuario)
        {
            // Encriptar la contraseña antes de guardar
            usuario.Password = ConvertirMD5(usuario.Password);

            _context.Usuario.Add(usuario);
            _context.SaveChanges();
        }

        // Método para actualizar un usuario existente
        public void ActualizarUsuario(Usuario usuario)
        {
            var usuarioExistente = _context.Usuario.Find(usuario.Id);
            if (usuarioExistente != null)
            {
                usuarioExistente.Nombre = usuario.Nombre;
                usuarioExistente.Apellido = usuario.Apellido;
                usuarioExistente.Login = usuario.Login;

                // Encriptar la nueva contraseña si ha sido modificada
                if (usuario.Password != usuarioExistente.Password)
                {
                    usuarioExistente.Password = ConvertirMD5(usuario.Password);
                }

                _context.SaveChanges();
            }
        }

        // Método para eliminar un usuario
        public void EliminarUsuario(int id)
        {
            var usuario = _context.Usuario.Find(id);
            if (usuario != null)
            {
                _context.Usuario.Remove(usuario);
                _context.SaveChanges();
            }
        }
    }
}