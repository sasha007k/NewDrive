using DAL.Data;
using DAL.Models;
using System.Threading.Tasks;

namespace DAL.Repository
{
    public class FolderRepository : BaseRepository<Folder, ApplicationDbContext>, IFolderRepository
    {
        public FolderRepository(ApplicationDbContext context) : base(context)
        {

        }

        public async Task Delete(int id)
        {
            var entity = await _context.Folders.FindAsync(id);
            if (entity != null)
            {
                entity.IsDeleted = true;
            }

            await _context.SaveChangesAsync();
        }

        public async Task RestoreFile(int id)
        {
            var entity = await _context.Folders.FindAsync(id);
            if (entity != null)
            {
                entity.IsDeleted = false;
            }

            await _context.SaveChangesAsync();
        }        
    }
}
