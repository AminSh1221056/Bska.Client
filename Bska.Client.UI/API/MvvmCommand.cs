
namespace Bska.Client.UI.API
{
    using System;
    using System.ComponentModel;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Windows.Input;

    public class MvvmCommand : ICommand
    {
        private Action<object> execute = null;

        private Func<object, bool> canExecute = null;

        // reference a specific WeakEvent manager 
        private GenericWeakEventManager<PropertyChangedEventArgs> weakEventListener;

        //create an handler for the INotifyPropertyChanged 
        private void RequeryCanExecute(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            OnCanExecuteChanged();
        }

        public MvvmCommand(Action<object> execute, Func<object, bool> canExecute)
        {
            this.weakEventListener = new GenericWeakEventManager<PropertyChangedEventArgs>(this.RequeryCanExecute);
            this.execute = execute;
            this.canExecute = canExecute;
        }

        //create a new constructor that will add a listener 
        public MvvmCommand(
     Action<object> execute,
     Func<object, bool> canExecute,
     INotifyPropertyChanged source, string propertyName)
            : this(execute, canExecute)
        {
            PropertyChangedEventManager.AddListener(source, this.weakEventListener, propertyName);
        }

        public bool CanExecute(object parameter)
        {
            if (this.canExecute == null)
            {
                return true;
            }
            return this.canExecute(parameter);
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            if (this.execute != null)
            {
                this.execute(parameter);
            }
        }

        public void OnCanExecuteChanged()
        {
            EventHandler handler = CanExecuteChanged;
            if (handler != null)
            {
                CanExecuteChanged(this, EventArgs.Empty);
            }
        }

        public MvvmCommand AddListener<TEntity>(INotifyPropertyChanged source, Expression<Func<TEntity, object>> property)
        {
            string propertyName = GetPropertyName(property);
            PropertyChangedEventManager.AddListener(source, weakEventListener, propertyName);
            return this;
        }

        private string GetPropertyName<TEntity>(Expression<Func<TEntity, object>> expression)
        {
            var lambda = expression as LambdaExpression;
            MemberExpression memberExpression;
            if (lambda.Body is UnaryExpression)
            {
                var unaryExpression = lambda.Body as UnaryExpression;
                memberExpression = unaryExpression.Operand as MemberExpression;
            }
            else
            {
                memberExpression = lambda.Body as MemberExpression;
            }
            var constantExpression = memberExpression.Expression as ConstantExpression;
            var propertyInfo = memberExpression.Member as PropertyInfo;
            return propertyInfo.Name;
        }
    }
}
