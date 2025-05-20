namespace FrontEndDevExtreme.Models
{
    public class ResponseModel<T>
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public int? ErrorCode { get; set; }
        public int TotalCount { get; set; }
        public T? Data { get; set; }
    }

}
