namespace FritzBoxClient.Exceptions.NasExceptions
{
    public class FritzBoxFileSystemException : Exception
    {
        public int ErrorCode { get; }
        public string UserMessage { get; }
        public List<FritzBoxFileErrorDetail> ErrorDetails { get; } = new List<FritzBoxFileErrorDetail>();

        public FritzBoxFileSystemException(string message, int errorCode, List<FritzBoxFileErrorDetail> errorDetails)
            : base(message)
        {
            ErrorCode = errorCode;
            UserMessage = message;
            ErrorDetails = errorDetails ?? new List<FritzBoxFileErrorDetail>();
        }

        public override string ToString()
        {
            var details = string.Join("\n", ErrorDetails.Select(d => d.ToString()));
            return $"{base.ToString()}\nErrorCode: {ErrorCode}\nDetails:\n{details}";
        }
    }

    public class FritzBoxFileErrorDetail
    {
        public string Path { get; }
        public string Message { get; }
        public int Code { get; }

        public FritzBoxFileErrorDetail(string path, string message, int code)
        {
            Path = path;
            Message = message;
            Code = code;
        }

        public override string ToString()
        {
            return $"Path: {Path}, Message: {Message}, Code: {Code}";
        }
    }

}
