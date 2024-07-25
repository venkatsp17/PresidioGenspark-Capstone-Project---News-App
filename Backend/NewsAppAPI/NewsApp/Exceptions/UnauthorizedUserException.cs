namespace NewsApp.Exceptions
{
    public class UnauthorizedUserException : Exception
    {
        string message;

        public UnauthorizedUserException()
        {
            message = "Invalid email or password!";
        }
        public override string Message => message;
    }
}
