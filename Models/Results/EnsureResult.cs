namespace Decibel.Models.Results
{
    public struct EnsureResult
    {
        public bool IsSuccess
        {
            get; init;
        }

        public string? Path
        {
            get; init;
        }

        public string Message
        {
            get; init;
        }

        public EnsureResult(bool isSucess, string? path, string message)
        {
            this.IsSuccess = isSucess;
            this.Path = path;
            this.Message = message;
        }

        public static EnsureResult Success(string path, string message = "Ambiente garantido com sucesso")
            => new EnsureResult(true, path, message);

        public static EnsureResult Failure(string message)
            => new EnsureResult(false, null, message);
    }
}
