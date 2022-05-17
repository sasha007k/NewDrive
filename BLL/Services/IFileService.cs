using DAL.Models;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace BLL.Services
{
    public interface IFileService
    {
        Task SaveFile(IFormFile file, int currentFolderId, string userId);

        List<DAL.Models.File> GetAllFilesInCurrentFolder(int currentFolderId);

        List<DAL.Models.File> GetAllFiles();

        Task<(byte[], string)> GetFile(string path);

        Task UpdateFile(IFormFile file, string path);

        Task DeleteFile(int id);

        Task<DAL.Models.File> GetAsync(int id);

        Task RestoreFile(int id);
    }
}
