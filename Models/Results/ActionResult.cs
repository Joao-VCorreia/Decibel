namespace Decibel.Models.Results
{
    public struct ActionResult
    {
        public bool IsSuccess
        {
            get; init;
        }
        public string Message
        {
            get; init;
        }

        public ActionResult(bool isSuccess, string message)
        {
            this.IsSuccess = isSuccess;
            this.Message = message;
        }

        public static ActionResult Success(string message = "Sucesso ao executar função")
            => new ActionResult(true, message);

        public static ActionResult Failure(string message)
            => new ActionResult(false, message);
    }
}
