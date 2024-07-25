namespace NewsApp.Exceptions
{
    public class NoAvailableItemException : Exception
    {
        string message;
        public NoAvailableItemException()
        {
            message = $"No Items Available!";
        }
        public override string Message => message;
    }
}
