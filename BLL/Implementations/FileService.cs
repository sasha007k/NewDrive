using BLL.Services;
using DAL.Repository;
using Microsoft.AspNetCore.Http;
using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Diagnostics.CodeAnalysis;

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
                Path = fullPath,
                Stared = false
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

        public List<DAL.Models.File> GetAllFiles()
        {
            return _fileRepository.GetAll().ToList();
        }

        [ExcludeFromCodeCoverage]
        public async Task<(byte[], string)> GetFile(string path)
        {
            var file =  _fileRepository.GetAll().Where(x => x.Path == path).FirstOrDefault();

            byte[] bytes = System.IO.File.ReadAllBytes(path);

            return (bytes, file.Name);
        }

        public async Task DeleteFile(int id)
        {
            await _fileRepository.Delete(id);
        }

        [ExcludeFromCodeCoverage]
        public async Task UpdateFile(IFormFile file, string path)
        {
            File.Delete(path);

            using (Stream fileStream = new FileStream(path, FileMode.OpenOrCreate))
            {
                await file.CopyToAsync(fileStream);
            }
        }

        public async Task<DAL.Models.File> GetAsync(int id)
        {
            return await _fileRepository.GetById(id);
        }

        public async Task RestoreFile(int id)
        {
            await _fileRepository.RestoreFile(id);
        }

        public List<DAL.Models.File> GetStared()
        {
            return _fileRepository.GetAll()
                .Where(x => x.Stared == true).ToList();
        }

        public async Task<DAL.Models.File> StarFile(int id)
        {
            var file = await _fileRepository.GetById(id);

            file.Stared = !file.Stared;

            await _fileRepository.Update(file);

            return file;
        }
    }
}
