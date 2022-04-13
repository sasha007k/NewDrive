 using Microsoft.AspNetCore.Identity;

namespace DAL.Models
{
    public class File
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Path { get; set; }

        public int FolderId { get; set; }

        public IdentityUser Owner { get; set; }
        public string OwnerId { get; set; }

        public Permission Permission { get; set; }
    }
}
