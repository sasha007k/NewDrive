using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace DAL.Models
{
    public class Folder
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Path { get; set; }

        public int ParentFolderId { get; set; }

        public IdentityUser Owner { get; set; }
        public string OwnerId { get; set; }

        public List<File> Files { get; set; }

        public List<Folder> Folders { get; set; }
    }
}
