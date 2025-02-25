namespace LeerUitkomst2.WebApi.Models
{
    public class DataBoolean
    {
        public bool Value { get; }
        public string Message { get; }
        public string LoggerMessage { get; }
        public object ExtraData = null;

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
        public DataBoolean CloneDataBoolean()
        {
            return this.CloneDataBoolean(this.Value, this.Message, this.LoggerMessage);
        }
        public DataBoolean CloneDataBoolean(string message)
        {
            return this.CloneDataBoolean(this.Value, message, message);
        }
        public DataBoolean CloneDataBoolean(string message, string loggerMessage)
        {
            return this.CloneDataBoolean(this.Value, message, loggerMessage);
        }
        public DataBoolean CloneDataBoolean(bool value)
        {
            return this.CloneDataBoolean(value, this.Message, this.LoggerMessage);
        }
        public DataBoolean CloneDataBoolean(bool value, string message)
        {
            return this.CloneDataBoolean(value, message, message);
        }
        public DataBoolean CloneDataBoolean(bool value, string message, string loggerMessage)
        {
            return this.CloneDataBoolean(value, message, loggerMessage, this.ExtraData);
        }
        public DataBoolean CloneDataBoolean(bool value, string message, string loggerMessage, object data)
        {
            var dataBoolean = new DataBoolean(value, message, loggerMessage);
            return dataBoolean.SetData(data);
        }

        public DataBoolean SetData(object data)
        {
            this.ExtraData = data;
            return this;
        }
    }
}
