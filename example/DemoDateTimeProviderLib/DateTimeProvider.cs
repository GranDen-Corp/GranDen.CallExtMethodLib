using System;
using GranDen.CallExtMethodLib;

namespace DemoDateTimeProviderLib
{
    public class DateTimeProvider
    {
        private readonly ExtMethodInvoker _extMethodInvoker = new ExtMethodInvoker("Iso8601ExtMethodLib");
        private readonly DateTime _storedDateTime;

        public DateTimeProvider(DateTime dateTime)
        {
            if (dateTime.Kind == DateTimeKind.Utc)
            {
                _storedDateTime = dateTime;
            }
            else
            {
                _storedDateTime = dateTime.ToUniversalTime();
            }
        }

        public DateTimeProvider(string iso8601String)
        {
            _storedDateTime = _extMethodInvoker.Invoke<DateTime>("FromIso8601String", iso8601String);
        }

        public DateTimeProvider() : this(DateTime.UtcNow)
        {
        }

        public DateTime GetStoredDateTime()
        {
            return _storedDateTime;
        }

        public string GetIso8601Format()
        {
            var ret =  _extMethodInvoker.Invoke<string>("ToIso8601String", _storedDateTime);
            return ret;
        }
    }
}
