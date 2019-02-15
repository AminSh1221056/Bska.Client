
namespace Bska.Client.UI.API
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Linq;
    using System.Reflection;

    public abstract class ChangeListener : INotifyPropertyChanged, IDisposable
    {
        #region *** Members ***
        protected string _propertyName;
        #endregion


        #region *** Abstract Members ***
        protected abstract void Unsubscribe();
        #endregion


        #region *** INotifyPropertyChanged Members and Invoker ***
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void RaisePropertyChanged(string propertyName)
        {
            var temp = PropertyChanged;
            if (temp != null)
                temp(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion


        #region *** Disposable Pattern ***

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                Unsubscribe();
            }
        }

        ~ChangeListener()
        {
            Dispose(false);
        }

        #endregion


        #region *** Factory ***
        public static ChangeListener Create(INotifyPropertyChanged value)
        {
            return Create(value, null);
        }

        public static ChangeListener Create(INotifyPropertyChanged value, string propertyName)
        {
            if (value is INotifyCollectionChanged)
            {
                return new CollectionChangeListener(value as INotifyCollectionChanged, propertyName);
            }
            else if (value is INotifyPropertyChanged)
            {
                return new ChildChangeListener(value as INotifyPropertyChanged, propertyName);
            }
            else
                return null;
        }
        #endregion
    }

    public class ChildChangeListener : ChangeListener
    {
        #region *** Members ***
        protected static readonly Type _inotifyType = typeof(INotifyPropertyChanged);

        private readonly INotifyPropertyChanged _value;
        private readonly Type _type;
        private readonly Dictionary<string, ChangeListener> _childListeners = new Dictionary<string, ChangeListener>();

        #endregion


        #region *** Constructors ***

        public ChildChangeListener(INotifyPropertyChanged instance)
        {
            if (instance == null)
                throw new ArgumentNullException("instance");

            _value = instance;
            _type = _value.GetType();

            Subscribe();
        }

        public ChildChangeListener(INotifyPropertyChanged instance, string propertyName)
            : this(instance)
        {
            _propertyName = propertyName;
        }
        #endregion


        #region *** Private Methods ***
        private void Subscribe()
        {
            _value.PropertyChanged += new PropertyChangedEventHandler(value_PropertyChanged);

            var query =
                from property
                in _type.GetProperties(BindingFlags.Instance | BindingFlags.Public)
                where _inotifyType.IsAssignableFrom(property.PropertyType)
                select property;

            foreach (var property in query)
            {
                // Declare property as known "Child", then register it
                _childListeners.Add(property.Name, null);
                ResetChildListener(property.Name);
            }
        }


        /// <summary>
        /// Resets known (must exist in children collection) child event handlers
        /// </summary>
        /// <param name="propertyName">Name of known child property</param>
        private void ResetChildListener(string propertyName)
        {
            if (_childListeners.ContainsKey(propertyName))
            {
                // Unsubscribe if existing
                if (_childListeners[propertyName] != null)
                {
                    _childListeners[propertyName].PropertyChanged -= new PropertyChangedEventHandler(child_PropertyChanged);

                    // Should unsubscribe all events
                    _childListeners[propertyName].Dispose();
                    _childListeners[propertyName] = null;
                }

                var property = _type.GetProperty(propertyName);
                if (property == null)
                    throw new InvalidOperationException(string.Format("Was unable to get '{0}' property information from Type '{1}'", propertyName, _type.Name));

                object newValue = property.GetValue(_value, null);

                // Only recreate if there is a new value
                if (newValue != null)
                {
                    if (newValue is INotifyCollectionChanged)
                    {
                        _childListeners[propertyName] =
                            new CollectionChangeListener(newValue as INotifyCollectionChanged, propertyName);
                    }
                    else if (newValue is INotifyPropertyChanged)
                    {
                        _childListeners[propertyName] =
                            new ChildChangeListener(newValue as INotifyPropertyChanged, propertyName);
                    }

                    if (_childListeners[propertyName] != null)
                        _childListeners[propertyName].PropertyChanged += new PropertyChangedEventHandler(child_PropertyChanged);
                }
            }
        }
        #endregion


        #region *** Event Handler ***
        void child_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            RaisePropertyChanged(e.PropertyName);
        }

        void value_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            // First, reset child on change, if required...
            ResetChildListener(e.PropertyName);

            // ...then, notify about it
            RaisePropertyChanged(e.PropertyName);
        }

        protected override void RaisePropertyChanged(string propertyName)
        {
            // Special Formatting
            base.RaisePropertyChanged(string.Format("{0}{1}{2}",
                _propertyName, _propertyName != null ? "." : null, propertyName));
        }
        #endregion


        #region *** Overrides ***
        /// <summary>
        /// Release all child handlers and self handler
        /// </summary>
        protected override void Unsubscribe()
        {
            _value.PropertyChanged -= new PropertyChangedEventHandler(value_PropertyChanged);

            foreach (var binderKey in _childListeners.Keys)
            {
                if (_childListeners[binderKey] != null)
                    _childListeners[binderKey].Dispose();
            }

            _childListeners.Clear();

            System.Diagnostics.Debug.WriteLine("ChildChangeListener '{0}' unsubscribed", _propertyName);
        }
        #endregion
    }

    public class CollectionChangeListener : ChangeListener
    {
        #region *** Members ***
        private readonly INotifyCollectionChanged _value;
        private readonly Dictionary<INotifyPropertyChanged, ChangeListener> _collectionListeners = new Dictionary<INotifyPropertyChanged, ChangeListener>();
        #endregion


        #region *** Constructors ***
        public CollectionChangeListener(INotifyCollectionChanged collection, string propertyName)
        {
            _value = collection;
            _propertyName = propertyName;

            Subscribe();
        }
        #endregion


        #region *** Private Methods ***
        private void Subscribe()
        {
            _value.CollectionChanged += new NotifyCollectionChangedEventHandler(value_CollectionChanged);

            foreach (var item in (IEnumerable)_value)
            {
                var k = item as INotifyPropertyChanged;
                if (k != null)
                    ResetChildListener(k);
            }
        }

        private void ResetChildListener(INotifyPropertyChanged item)
        {
            if (item == null)
                throw new ArgumentNullException("item");

            RemoveItem(item);

            ChangeListener listener = null;

            // Add new
            if (item is INotifyCollectionChanged)
                listener = new CollectionChangeListener(item as INotifyCollectionChanged, _propertyName);
            else
                listener = new ChildChangeListener(item as INotifyPropertyChanged);

            listener.PropertyChanged += new PropertyChangedEventHandler(listener_PropertyChanged);
            _collectionListeners.Add(item, listener);
        }

        private void RemoveItem(INotifyPropertyChanged item)
        {
            // Remove old
            if (_collectionListeners.ContainsKey(item))
            {
                _collectionListeners[item].PropertyChanged -= new PropertyChangedEventHandler(listener_PropertyChanged);

                _collectionListeners[item].Dispose();
                _collectionListeners.Remove(item);
            }
        }


        private void ClearCollection()
        {
            foreach (var key in _collectionListeners.Keys)
            {
                _collectionListeners[key].Dispose();
            }

            _collectionListeners.Clear();
        }
        #endregion


        #region *** Event handlers ***
        void value_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                ClearCollection();
            }
            else
            {
                // Don't care about e.Action, if there are old items, Remove them...
                if (e.OldItems != null)
                {
                    foreach (INotifyPropertyChanged item in (IEnumerable)e.OldItems)
                        RemoveItem(item);
                }

                // ...add new items as well
                if (e.NewItems != null)
                {
                    foreach (var item in (IEnumerable)e.NewItems)
                    {
                        var k = item as INotifyPropertyChanged;
                        if (k != null)
                            ResetChildListener(k);
                    }
                }
            }
        }


        void listener_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            // ...then, notify about it
            RaisePropertyChanged(string.Format("{0}{1}{2}",
                _propertyName, _propertyName != null ? "[]." : null, e.PropertyName));
        }
        #endregion


        #region *** Overrides ***
        /// <summary>
        /// Releases all collection item handlers and self handler
        /// </summary>
        protected override void Unsubscribe()
        {
            ClearCollection();

            _value.CollectionChanged -= new NotifyCollectionChangedEventHandler(value_CollectionChanged);

            System.Diagnostics.Debug.WriteLine("CollectionChangeListener unsubscribed");
        }
        #endregion
    }
}
