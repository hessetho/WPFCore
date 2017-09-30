using System;

namespace WPFCore.Data.StructuredDataReader
{
    public class DataRowError
    {
        public DataRowError(StructuredDataRow dataRow, Exception internalException, string propertyName, string description)
            : this(dataRow, internalException, propertyName, description, string.Empty)
        {
        }

        public DataRowError(StructuredDataRow dataRow, Exception internalException, string propertyName, string description, string readValue)
        {
            this.InternalException = internalException;
            this.PropertyName = propertyName;
            this.Description = description;
            this.ReadValue = readValue;
            this.DataRow = dataRow;
        }

        public Exception InternalException { get; private set; }
        public string PropertyName { get; private set; }
        public string Description { get; private set; }
        public string ReadValue { get; private set; }

        public StructuredDataRow DataRow { get; private set; }
    }
}
