namespace NewsApp.Exceptions
{
    public class UnableToRegisterException : Exception
    {
        string message;

        public UnableToRegisterException()
        {
            message = "Unable to Register at this moment!";
        }

        public override string Message => message;
    }
}
