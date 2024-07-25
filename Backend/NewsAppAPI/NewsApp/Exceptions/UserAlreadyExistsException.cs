namespace NewsApp.Exceptions
{
    public class UserAlreadyExistsException : Exception
    {
        string message;
        public UserAlreadyExistsException()
        {
            message = "Account already exists with email!";
        }
        public override string Message => message;
    }
}
