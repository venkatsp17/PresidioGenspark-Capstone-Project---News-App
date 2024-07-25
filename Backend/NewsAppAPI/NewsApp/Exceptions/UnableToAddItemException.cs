namespace NewsApp.Exceptions
{
    public class UnableToAddItemException : Exception
    {
        string message;
        public UnableToAddItemException()
        {
            message = $"Unable to add item!";
        }
        public override string Message => message;
    }
}
