using DAL.Data;
using DAL.Models;

namespace DAL.Repository
{
    public class FileRepository : BaseRepository<File, ApplicationDbContext>, IFileRepository
    {
        public FileRepository(ApplicationDbContext context) : base(context)
        {

        }
    }
}
