using DAL.Data;
using DAL.Models;

namespace DAL.Repository
{
    public class FolderRepository : BaseRepository<Folder, ApplicationDbContext>, IFolderRepository
    {
        public FolderRepository(ApplicationDbContext context) : base(context)
        {

        }
    }
}
