
namespace Bska.Client.UI.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Linq.Expressions;
    using System.Reflection;

    public abstract class ObservableObject : INotifyPropertyChanged
    {
        #region Implementation of INotifyPropertyChanged
        /// <summary> 
        /// Occurs when a property value changes. 
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        private Dictionary<string, object> propertyValueStorage;

        #endregion

        public ObservableObject()
        {
            this.propertyValueStorage = new Dictionary<string, object>();
        }

        /// <summary> 
        /// Called when [property changed]. 
        /// </summary> 
        /// <param name="propertyName">The property.</param>
        public void OnPropertyChanged(String propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        protected void SetValue<T>(Expression<Func<T>> property, T value)
        {
            LambdaExpression lambdaExpression = property;

            if (lambdaExpression == null)
            {
                throw new ArgumentException("Invalid lambda expression", new Exception("Lambda expression return value can't be null"));
            }

            string propertyName = this.getPropertyName(lambdaExpression);

            T storedValue = this.getValue<T>(propertyName);

            if (!Equals(storedValue, value))
            {
                this.propertyValueStorage[propertyName] = value;

                this.OnPropertyChanged(propertyName);
            }
        }

        protected T GetValue<T>(Expression<Func<T>> property)
        {
            LambdaExpression lambdaExpression = property;

            if (lambdaExpression == null)
            {
                throw new ArgumentException("Invalid lambda expression", new Exception("Lambda expression return value can't be null"));
            }

            string propertyName = this.getPropertyName(lambdaExpression);

            return getValue<T>(propertyName);
        }

        private T getValue<T>(string propertyName)
        {
            object value;

            if (propertyValueStorage.TryGetValue(propertyName, out value))
            {
                return (T)value;
            }

            return default(T);
        }

        private string getPropertyName(LambdaExpression lambdaExpression)
        {
            MemberExpression memberExpression = default(MemberExpression);

            if (lambdaExpression.Body is UnaryExpression)
            {
                var unaryExpression = lambdaExpression.Body as UnaryExpression;
                if (unaryExpression != null)
                {
                    memberExpression = unaryExpression.Operand as MemberExpression;
                }
            }
            else
            {
                memberExpression = lambdaExpression.Body as MemberExpression;
            }

            if (memberExpression != null)
            {
                return memberExpression.Member.Name;
            }

            return String.Empty;
        }

        protected void RaisePropertyChangedEvent()
        {
            // Get the call stack
            StackTrace stackTrace = new StackTrace();

            // Get the calling method name
            string callingMethodName = stackTrace.GetFrame(1).GetMethod().Name;

            // Check if the callingMethodName contains an underscore like in "set_SomeProperty"
            if (callingMethodName.Contains("_"))
            {
                // Extract the property name
                string propertyName = callingMethodName.Split('_')[1];

                if (this.PropertyChanged != null && propertyName != String.Empty)
                {
                    this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
                }
            }
        }

        protected void RaisePropertyChangedEvent(Expression<Func<object>> lambdaExpression)
        {
            // The changed property is not identified through a string but rather through the property itself
            if (this.PropertyChanged != null)
            {
                // Extract the body of the lambda expression
                var lambdaBody = lambdaExpression.Body as MemberExpression;

                if (lambdaBody == null)
                {
                    // If the Property is a primitive data type (i.e. bool) the body of the lambda expression
                    // have to be converted to an "UnaryExpression" to get the desired "MemberExpression"
                    var unaryExpression = (lambdaExpression.Body as UnaryExpression);
                    if (unaryExpression != null)
                    {
                        lambdaBody = unaryExpression.Operand as MemberExpression;
                    }
                }

                // "Raise" the PropertyChanged-Event with the "real name" of the Property
                if (lambdaBody != null)
                {
                    this.PropertyChanged(this, new PropertyChangedEventArgs(lambdaBody.Member.Name));
                }
                else
                {
                    Debug.WriteLine("Could not resolve the name of the Property");
                }
            }
        }

        protected void RaisePropertyChangedEvent(string propertyName)
        {
            // This is an improved "string based" version to "raise" the PropertyChanged-Event
            // Bevor raising the PropertyChanged Event, the property name is being evaluated !
            this.checkPropertyName(propertyName);

            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #region Private Helpers

        [Conditional("DEBUG")]
        private void checkPropertyName(string propertyName)
        {
            PropertyDescriptor propertyDescriptor = TypeDescriptor.GetProperties(this)[propertyName];
            if (propertyDescriptor == null)
            {
                // ReSharper disable PossiblyMistakenUseOfParamsMethod
                string message = string.Format(null, "The property with the propertyName '{0}' doesn't exist.", propertyName);
                // ReSharper restore PossiblyMistakenUseOfParamsMethod
                Debug.Fail(message);
            }
        }

        #endregion // Private Helpers

        protected void RaisePropertyChangedEvent_Deprecated(string propertyName)
        {
            // This is the old "string based" version to raise the PropertyChanged-Event
            // There is no evaluation whether the property name is valid or not !!!
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
