namespace NewsApp.Exceptions
{
    public class ItemAlreadyExistException : Exception
    {
        string message;
        public ItemAlreadyExistException()
        {
            message = "Item already exists with that name!";
        }
        public override string Message => message;
    }
}
