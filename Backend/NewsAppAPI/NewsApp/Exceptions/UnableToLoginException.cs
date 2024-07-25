namespace NewsApp.Exceptions
{
    public class UnableToLoginException: Exception
    {
        string message;

        public UnableToLoginException()
        {
            message = "Unable to Login at this moment!";
        }

        public override string Message => message;
    }
}
