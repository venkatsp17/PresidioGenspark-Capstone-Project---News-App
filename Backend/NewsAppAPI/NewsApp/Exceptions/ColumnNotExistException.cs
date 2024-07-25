using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace NewsApp.Exceptions
{
    public class ColumnNotExistException : Exception
    {
        string message;
        public ColumnNotExistException(string key, string model)
        {
            message = $"The column '{key}' does not exist in the 'Comment' entity.";
        }
        public override string Message => message;
    }
}
