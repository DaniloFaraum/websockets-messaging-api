namespace WebKafka.Models
{
    public class ErrorResponse
    {
        public int Code { get; set; }
        public string Message { get; set; }

        public ErrorResponse(int code, string message)
        {
            this.Code = code;
            this.Message = message;
        }
    }
}