namespace ScradaSender.Api.Jobs.Interface
{
    public interface IStatusCheckerJob
    {
        Task CheckStatusAsync();
    }
}