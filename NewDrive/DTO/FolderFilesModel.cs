using DAL.Models;
using System.Collections.Generic;

namespace NewDrive.DTO
{
    public class FolderFilesModel
    {
        public int CurrentFolderId { get; set; }

        public int ParentFolderId { get; set; } // -1 if base folder

        public List<File> FilesInFolder { get; set; }

        public List<Folder> FoldersInFolder { get; set; }

        public string Search { get; set; }
    }
}
