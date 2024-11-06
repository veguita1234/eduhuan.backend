namespace EduHuan.backend.DTOs
{
    public class UserDTO
    {
        public Guid UserId { get; set; }
        public string Tipo { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
        public string Celular { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}
