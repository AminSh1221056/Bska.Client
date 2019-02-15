
namespace Bska.Client.UI.Helper
{
    using Bska.Client.Common;
    using Bska.Client.Domain.Entity;
    using System;
    public class UnitHelper
    {
        public Unit mainparentRecovery(Unit parent)
        {
            Unit mparent = parent;

            if (parent.Parent != null)
            {
                mparent = this.mainparentRecovery(parent.Parent);
            }
            return mparent;
        }

        public Double CalculateUnitNum(Unit unit, Double val)
        {
            switch (unit.MathType)
            {
                case UnitMathType.Multiple:
                    val = (val * unit.MathNum.Value);
                    break;
                case UnitMathType.Divide:
                    val = (val / unit.MathNum.Value);
                    break;
            }
            if (unit.Parent != null)
            {
                val = CalculateUnitNum(unit.Parent, val);
            }
            return val;
        }

        public Double ReverseCalculateUnitNum(Unit unit, Double val)
        {
            switch (unit.MathType)
            {
                case UnitMathType.Divide:
                    val = (val * unit.MathNum.Value);
                    break;
                case UnitMathType.Multiple:
                    val = (val / unit.MathNum.Value);
                    break;
            }
            if (unit.Parent != null)
            {
                val = ReverseCalculateUnitNum(unit.Parent, val);
            }
            return val;
        }
    }
}
