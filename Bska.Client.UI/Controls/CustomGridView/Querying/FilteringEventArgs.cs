﻿using System;

namespace Bska.Client.UI.Controls.CustomGridView.Querying
{
    public class FilteringEventArgs : EventArgs
    {
        public Exception Error { get; private set; }

        public FilteringEventArgs(Exception ex)
        {
            Error = ex;
        }
    }
}
