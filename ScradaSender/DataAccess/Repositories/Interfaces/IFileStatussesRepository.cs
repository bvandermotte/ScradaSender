using ScradaSender.DataAccess.Entities;
using ScradaSender.DataAccess.Repositories.Base;

namespace ScradaSender.DataAccess.Repositories.Interfaces
{
    public interface IFileStatussesRepository : IBaseRepository
    {
        Task<List<FileStatusses>> GetFilesFromNames(List<string> fileNames);
        Task AddFileStatus(List<FileStatusses> fileStatusses);
        Task<List<FileStatusses>> GetFilesWithStatusSent();
    }
}