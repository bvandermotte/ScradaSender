namespace ScradaSender.Agents.Models.Base
{
    public class Response<T>
    {
        public T ResponseObject { get; set; }
        public string Error { get; set; }
    }
}
