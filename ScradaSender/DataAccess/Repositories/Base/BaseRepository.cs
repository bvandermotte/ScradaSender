namespace ScradaSender.DataAccess.Repositories.Base
{
    public abstract class BaseRepository(ScradaSenderDbContext baseDbContext) : IBaseRepository
    {
        protected readonly ScradaSenderDbContext dbContext = baseDbContext;

        public async Task SaveChangesAsync()
        {
            await dbContext.SaveChangesAsync();
        }
    }
}
