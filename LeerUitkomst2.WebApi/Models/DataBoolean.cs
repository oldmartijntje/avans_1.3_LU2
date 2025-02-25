namespace LeerUitkomst2.WebApi.Models
{
    public class DataBoolean
    {
        public bool Value { get; }
        public string Message { get; }
        public string LoggerMessage { get; }

        public DataBoolean(bool value, string message)
        {
            Value = value;
            Message = message;
            LoggerMessage = message;
        }
        public DataBoolean(bool value, string message, string loggerMessage)
        {
            Value = value;
            Message = message;
            LoggerMessage = loggerMessage;
        }
        public DataBoolean ChangeDataBoolean()
        {
            return this.ChangeDataBoolean(this.Value, this.Message, this.LoggerMessage);
        }
        public DataBoolean ChangeDataBoolean(string message)
        {
            return this.ChangeDataBoolean(this.Value, message, message);
        }
        public DataBoolean ChangeDataBoolean(string message, string loggerMessage)
        {
            return this.ChangeDataBoolean(this.Value, message, loggerMessage);
        }
        public DataBoolean ChangeDataBoolean(bool value)
        {
            return this.ChangeDataBoolean(value, this.Message, this.LoggerMessage);
        }
        public DataBoolean ChangeDataBoolean(bool value, string message)
        {
            return this.ChangeDataBoolean(value, message, message);
        }
        public DataBoolean ChangeDataBoolean(bool value, string message, string loggerMessage)
        {
            return new DataBoolean(value, message, loggerMessage);
        }
    }
}
