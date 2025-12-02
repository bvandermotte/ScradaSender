namespace ScradaSender.Api.Jobs
{
    public class FileReaderJob : IFileReaderJob
    {
        public void ReadFiles()
        {
            Console.WriteLine("Read");
        }
    }
}
