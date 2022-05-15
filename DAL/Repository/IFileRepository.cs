using DAL.Models;
using System.Threading.Tasks;

namespace DAL.Repository
{
    public interface IFileRepository : IRepository<File>
    {
        Task Delete(int id);

        Task RestoreFile(int id);        
    }
}
