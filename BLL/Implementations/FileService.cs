using BLL.Services;
using DAL.Repository;
using Microsoft.AspNetCore.Http;
using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace BLL.Implementations
{
    public class FileService : IFileService
    {
        private readonly IFileRepository _fileRepository;
        public FileService(IFileRepository fileRepository)
        {
            _fileRepository = fileRepository;
        }

        public async Task SaveFile(IFormFile file, int currentFolderId, string userId)
        {
            if (file == null || currentFolderId < 1 || string.IsNullOrWhiteSpace(userId))
            {
                return;
            }

            var uniqueName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            var storagePath = Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).FullName, "Storage");
            var fullPath = Path.Combine(storagePath, uniqueName);

            if (!Directory.Exists(storagePath))
            {
                Directory.CreateDirectory(storagePath);
            }            

            using (Stream fileStream = new FileStream(fullPath, FileMode.OpenOrCreate))
            {
                await file.CopyToAsync(fileStream);
            }

            var newFile = new DAL.Models.File()
            {
                Name = file.FileName,
                FolderId = currentFolderId,
                OwnerId = userId,
                Path = fullPath
            };

            await _fileRepository.Add(newFile);
        }

        public List<DAL.Models.File> GetAllFilesInCurrentFolder(int currentFolderId)
        {
            if (currentFolderId < 1)
            {
                return new List<DAL.Models.File>();
            }

            return _fileRepository.GetAll().Where(x => x.FolderId == currentFolderId).ToList();
        }

        public async Task<(byte[], string)> GetFile(string path)
        {
            var file =  _fileRepository.GetAll().Where(x => x.Path == path).FirstOrDefault();

            byte[] bytes = System.IO.File.ReadAllBytes(path);

            return (bytes, file.Name);
        }

        public async Task DeleteFile(int id)
        {
          await  _fileRepository.Delete(id);
        }

        public async Task UpdateFile(IFormFile file, string path)
        {
            File.Delete(path);

            using (Stream fileStream = new FileStream(path, FileMode.OpenOrCreate))
            {
                await file.CopyToAsync(fileStream);
            }
        }
    }
}
