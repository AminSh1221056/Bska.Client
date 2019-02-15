
namespace Bska.Client.UI.CustomAttributes
{
    using System;
    using System.ComponentModel.DataAnnotations;

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    public sealed class PositiveNumberAttribute : DataTypeAttribute
    {
        public PositiveNumberAttribute() : base(DataType.Custom) { }

        public override bool IsValid(object value)
        {
            Double val;
            bool isvalid = false;
            isvalid = Double.TryParse(value.ToString(), out val);
            if (isvalid)
            {
                if (val <= 0) isvalid = false;
            }

            return isvalid;
        }
    }

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    public sealed class PositiveIntNumberAttribute : DataTypeAttribute
    {
        public PositiveIntNumberAttribute() : base(DataType.Custom) { }

        public override bool IsValid(object value)
        {
            int val;
            bool isvalid = false;
            isvalid = int.TryParse(value.ToString(), out val);
            if (isvalid)
            {
                if (val <= 0) isvalid = false;
            }

            return isvalid;
        }
    }
}
