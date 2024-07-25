namespace NewsApp.Exceptions
{
    public class ItemNotFoundException : Exception
    {
        string message;
        public ItemNotFoundException()
        {
            message = $"Item with given value not found!";
        }
        public override string Message => message;
    }
}
