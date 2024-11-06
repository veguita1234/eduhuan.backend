using System.ComponentModel.DataAnnotations;

namespace EduHuan.backend.Repositories.Entities
{
    public class User
    {
        [Key]
        public Guid UserId { get; set; }
        public string Tipo { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
        public string Celular { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }

        public ICollection<Calificaciones> Calificaciones { get; set; }
    }
}
