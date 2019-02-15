using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bska.Client.UI.Controls.CustomGridView.Support
{
    public enum FilterOperator
    {
        Undefined,
        LessThan,
        LessThanOrEqual,
        GreaterThan,
        GreaterThanOrEqual,
        Equals,
        Like,
        Between
    }

    public enum FilterStringOperator
    {
        IndexOf,
        StartWith,
        EndWith
    }
}
