using DAL.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BLL.Services
{
    public interface IFolderService
    {
        Folder GetBaseFolderByUserId(string userId);
        Task CreateFolder(string name, int currentFolderId, string userId);
        List<Folder> GetAllFoldersInCurrentFolder(int currentFolderId);

        Task DeleteFolder(int id);
    }
}
