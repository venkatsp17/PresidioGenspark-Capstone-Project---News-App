namespace NewsApp.Exceptions
{
    public class UnableToAddUserException : Exception
    {
        string message;
        public UnableToAddUserException()
        {
            message = $"Unable to add user!";
        }
        public override string Message => message;
    }
}
