using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Update.Internal;
using ScradaSender.DataAccess.Entities;
using ScradaSender.DataAccess.Repositories.Base;
using ScradaSender.DataAccess.Repositories.Interfaces;
using static ScradaSender.Shared.Enums.Enums;

namespace ScradaSender.DataAccess.Repositories
{
    public class FileStatussesRepository(ScradaSenderDbContext dbContext) : BaseRepository(dbContext), IFileStatussesRepository
    {
        public async Task AddFileStatus(List<FileStatusses> fileStatusses)
        {
            await dbContext.AddRangeAsync(fileStatusses);
        }

        public async Task<List<FileStatusses>> GetFilesFromNames(List<string> fileNames)
        {
            return await dbContext.Set<FileStatusses>().AsNoTracking().Where(i => fileNames.Contains(i.FileName)).ToListAsync();
        }

        public async Task<List<FileStatusses>> GetFilesWithStatusSent()
        {
            return await dbContext.Set<FileStatusses>().Where(i => i.Status == nameof(Status.Sent)).ToListAsync();
        }
    }
}
