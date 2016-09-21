using WPFCore.ViewModelSupport;

namespace WPFCore.Data
{

    internal class ValidateDateRangeAttribute : InstanceValidationAttribute
    {
        public override bool IsValid(object instance, object value)
        {
            var dateRange = (DateRange)instance;

            return dateRange.IsValidRange;
        }
    }
}
