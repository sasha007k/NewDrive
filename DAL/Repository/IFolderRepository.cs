using DAL.Models;
using System.Threading.Tasks;

namespace DAL.Repository
{
    public interface IFolderRepository : IRepository<Folder>
    {
        Task Delete(int id);

        Task RestoreFile(int id);        
    }
}
