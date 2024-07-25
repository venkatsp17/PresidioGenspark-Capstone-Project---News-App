namespace NewsApp.Exceptions
{
    public class UserNotFoundException : Exception
    {
        string message;
        public UserNotFoundException()
        {
            message = "User with given credential not found";
        }
        public override string Message => message;
    }
}
