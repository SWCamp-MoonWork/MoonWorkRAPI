namespace MoonWorkRAPI.Models
{
    public class UserModel
    {
        public long? UserId { get; set; }
        public string? UserName { get; set; }
        public string? Password { get; set;}
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Note { get; set; }
        public Boolean? IsActive { get; set; }
/*        public DateTime? LastLogin { get; set; }*/
    }
}
