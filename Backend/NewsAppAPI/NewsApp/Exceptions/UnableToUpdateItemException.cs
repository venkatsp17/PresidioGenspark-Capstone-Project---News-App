namespace NewsApp.Exceptions
{
    public class UnableToUpdateItemException : Exception
    {
        string message;
        public UnableToUpdateItemException()
        {
            message = $"Unable to update item!";
        }
        public override string Message => message;
    }
}
