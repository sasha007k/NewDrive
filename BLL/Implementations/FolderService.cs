using BLL.Services;
using DAL.Models;
using DAL.Repository;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using System.IO;

namespace BLL.Implementations
{
    public class FolderService : IFolderService
    {
        private readonly IFolderRepository _folderRepository;
        private readonly IFileRepository _fileRepository;

        public FolderService(
            IFolderRepository folderRepository,
            IFileRepository fileRepository)
        {
            _folderRepository = folderRepository;
            _fileRepository = fileRepository;
        }

        public Folder GetBaseFolderByUserId(string userId)
        {
            return _folderRepository.GetAll(x => x.Files, x => x.Folders)
                .Where(x => x.OwnerId == userId).Where(x => x.ParentFolderId == -1).FirstOrDefault();
        }

        public async Task CreateFolder(string name, int currentFolderId, string userId)
        {
            var folder = new Folder()
            {
                Name = name,
                ParentFolderId = currentFolderId,
                Path = string.Empty,
                OwnerId = userId
            };

            await _folderRepository.Add(folder);
        }

        public List<Folder> GetAllFoldersInCurrentFolder(int currentFolderId)
        {
            if (currentFolderId < 1)
            {
                return new List<Folder>();
            }

            return _folderRepository.GetAll().Where(x => x.ParentFolderId == currentFolderId).ToList();
        }

        public async Task DeleteFolder(int id)
        {
            var dir = _folderRepository.GetAll(x => x.Files, x => x.Folders).Where(x => x.Id == id).FirstOrDefault();
            if (dir == null) return;

            if (dir.Folders != null)
            {
                foreach (var folder in dir.Folders)
                {
                    foreach (var file in folder.Files)
                    {
                        await _fileRepository.Delete(file.Id);
                    }
                    await _folderRepository.Delete(folder.Id);
                }
            }

            if (dir.Files != null)
            {
                foreach (var file in dir.Files)
                {
                    await _fileRepository.Delete(file.Id);
                }
            }

            await _folderRepository.Delete(dir.Id);
        }

        public async Task RestoreFolder(int id)
        {
            var dir = _folderRepository.GetAll(x => x.Files, x => x.Folders).Where(x => x.Id == id).FirstOrDefault();
            if (dir == null) return;

            if (dir.Folders != null)
            {
                foreach (var folder in dir.Folders)
                {
                    foreach (var file in folder.Files)
                    {
                        await _fileRepository.RestoreFile(file.Id);
                    }
                    await _folderRepository.RestoreFile(folder.Id);
                }
            }

            if (dir.Files != null)
            {
                foreach (var file in dir.Files)
                {
                    await _fileRepository.RestoreFile(file.Id);
                }
            }

            await _folderRepository.RestoreFile(dir.Id);
        }
    }
}
