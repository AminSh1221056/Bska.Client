
namespace Bska.Client.UI.CustomAttributes
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Text.RegularExpressions;

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    public sealed class MobileAttribute : DataTypeAttribute
    {
        private static Regex _regex = new Regex(@"^(\+\s?)?((?<!\+.*)\(\+?\d+([\s\-\.]?\d+)?\)|\d+)([\s\-\.]?(\(\d+([\s\-\.]?\d+)?\)|\d+))*(\s?(x|ext\.?)\s?\d+)?$", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture);

        public MobileAttribute() : base(DataType.PhoneNumber) { ErrorMessage = "Invalied Phone number"; }

        public override bool IsValid(object value)
        {
            if (string.IsNullOrEmpty(value.ToString()))
            {
                return true;
            }

            string valueAsString = value as string;
            return valueAsString != null && _regex.Match(valueAsString).Length > 0 && valueAsString.StartsWith("09");
        }
    }
}
