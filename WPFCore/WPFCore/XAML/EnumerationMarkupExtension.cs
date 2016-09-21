using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Markup;

namespace WPFCore.XAML
{
    public class EnumerationMarkupExtension : MarkupExtension
    {
        private Type _enumType;

        public EnumerationMarkupExtension(Type enumType)
        {
            if (enumType == null)
                throw new ArgumentNullException("enumType");

            this.EnumType = enumType;
        }

        public Type EnumType
        {
            get { return this._enumType; }
            private set
            {
                if (this._enumType == value)
                    return;

                var enumType = Nullable.GetUnderlyingType(value) ?? value;

                if (enumType.IsEnum == false)
                    throw new ArgumentException("Type must be an Enum.");

                this._enumType = value;
            }
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            var enumValues = Enum.GetValues(this.EnumType);

            return (
              from object enumValue in enumValues
              select new EnumerationMember
              {
                  Value = enumValue,
                  Description = this.GetDescription(enumValue)
              }).ToArray();
        }

        private string GetDescription(object enumValue)
        {
            var descriptionAttribute = this.EnumType
              .GetField(enumValue.ToString())
              .GetCustomAttributes(typeof(DescriptionAttribute), false)
              .FirstOrDefault() as DescriptionAttribute;

            return descriptionAttribute != null
              ? descriptionAttribute.Description
              : enumValue.ToString();
        }


        internal static string GetDescription(Type enumType, object enumValue)
        {
            var descriptionAttribute = enumType
              .GetField(enumValue.ToString())
              .GetCustomAttributes(typeof(DescriptionAttribute), false)
              .FirstOrDefault() as DescriptionAttribute;

            return descriptionAttribute != null
              ? descriptionAttribute.Description
              : enumValue.ToString();
        }

        public static ObservableCollection<EnumerationMember> AsCollection(Type enumType)
        {
            return new ObservableCollection<EnumerationMember>
            (
            Enum.GetValues(enumType).Cast<object>().Select(enumValue => new EnumerationMember
            {
                Value = enumValue,
                Description=GetDescription(enumType, enumValue)
            }).ToList());
        }

        public class EnumerationMember
        {
            public string Description { get; set; }
            public object Value { get; set; }
        }
    }
}
