using Microsoft.AspNetCore.Identity;

namespace DAL.Models
{
    public class FileUser
    {
        public int Id { get; set; }

        public int FileId { get; set; }

        public string UserId { get; set; }
    }
}
