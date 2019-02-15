
namespace Bska.Client.UI.CustomAttributes
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    public sealed class NationalCodeAttribute : DataTypeAttribute
    {
        public NationalCodeAttribute() : base(DataType.Custom) { }

        public override bool IsValid(object value)
        {
            if (!String.IsNullOrEmpty(value.ToString()))
            {
                String nationalid = value.ToString();
                Boolean isdigit = nationalid.All(char.IsDigit);
                if (isdigit)
                {
                    if (nationalid.Length == 10)
                    {
                        int one = Int32.Parse(nationalid[0].ToString()) * 10;
                        int two = Int32.Parse(nationalid[1].ToString()) * 9;
                        int three = Int32.Parse(nationalid[2].ToString()) * 8;
                        int four = Int32.Parse(nationalid[3].ToString()) * 7;
                        int five = Int32.Parse(nationalid[4].ToString()) * 6;
                        int six = Int32.Parse(nationalid[5].ToString()) * 5;
                        int seven = Int32.Parse(nationalid[6].ToString()) * 4;
                        int eight = Int32.Parse(nationalid[7].ToString()) * 3;
                        int nine = Int32.Parse(nationalid[8].ToString()) * 2;

                        int controlnum = Int32.Parse(nationalid[9].ToString());

                        int valuenum = (one + two + three + four + five + six + seven + eight + nine) % 11;

                        if (valuenum < 2)
                        {
                            if (controlnum == valuenum)
                                return true;
                            else
                                return false;
                        }
                        else if (valuenum >= 2)
                        {
                            if (controlnum == 11 - valuenum)
                                return true;
                            else
                                return false;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            return true;
        }
    }
}
