namespace TechnicalTestDOT.Payloads.Response
{
    public class CommonResponse
    {
        public int StatusCode { get; set; }
        public string Message { get; set; }
        public object Data { get; set; }
    }
}
