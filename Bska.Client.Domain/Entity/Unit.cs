
namespace Bska.Client.Domain.Entity
{
    using Bska.Client.API.EF6;
    using Bska.Client.Common;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;
    public class Unit : Entity, IEquatable<Unit>, IFormattable
    {
        public Unit()
        {
            this.Childeren = new List<Unit>();
        }
        public Int32 UnitId { get; set; }
        public String Name { get; set; }
        public StuffType StuffId { get; set; }

        [NotMapped]
        public String Desctiption { get; set; }
        public UnitMathType MathType { get; set; }
        public Int32? MathNum { get; set; }
        public virtual Unit Parent { get; set; }
        public virtual ICollection<Unit> Childeren { get; set; }
        public override int GetHashCode()
        {
            return UnitId.GetHashCode();
        }
        public override string ToString()
        {
            return string.Format("{0} مربوط به {1}", Name, StuffId);
        }
        public override bool Equals(object obj)
        {
            if (obj == null)
                return base.Equals(obj);
            return Equals(obj as Unit);
        }
        public bool Equals(Unit other)
        {
            if (other == null)
                return base.Equals(other);

            return this.StuffId == other.StuffId && this.Name == other.Name;
        }

        public string ToString(string format, IFormatProvider formatProvider)
        {
            if (format == null)
            {
                return ToString();
            }
            string formatUpper = format.ToUpper();
            switch (formatUpper)
            {
                case "URM":
                    return setMathFormat();
                default:
                    return ToString();
            }
        }

        private string setMathFormat()
        {
            switch (MathType)
            {
                case UnitMathType.None:
                    return "بدون رابطه";
                case UnitMathType.Divide:
                    return string.Format("رابطه با واحد بالایی ( {0} / 1 )", MathNum);
                case UnitMathType.Multiple:
                    return string.Format("رابطه با واحد بالایی ( X {0} )", MathNum);
                default:
                    return "ناشناخته";
            }
        }
    }
}
