﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace Bska.Client.UI.Controls.CustomGridView
{
    public class DataGridFilterCommand : ICommand
    {
        private readonly Action<object> action;

        public DataGridFilterCommand(Action<object> action)
        {
            this.action = action;
        }

        public void Execute(object parameter)
        {
            if (action != null) action(parameter);
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

       public event EventHandler CanExecuteChanged
       {
           add { CommandManager.RequerySuggested += value; }
           remove { CommandManager.RequerySuggested -= value; }
       }
    }
}
