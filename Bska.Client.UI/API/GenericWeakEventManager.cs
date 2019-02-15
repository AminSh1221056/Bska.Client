
namespace Bska.Client.UI.API
{
    using System;
    using System.Windows;

    public sealed class GenericWeakEventManager<TEventArgs> : IWeakEventListener where TEventArgs : EventArgs
    {
        EventHandler<TEventArgs> realHander;

        public GenericWeakEventManager(EventHandler<TEventArgs> handler)
        {
            if (handler == null)
            {
                throw new ArgumentNullException("handler");
            }

            this.realHander = handler;
        }

        bool IWeakEventListener.ReceiveWeakEvent(Type managerType, Object sender, EventArgs
                                                 e)
        {
            TEventArgs realArgs = (TEventArgs)e;

            this.realHander(sender, realArgs);

            return true;
        }
    }
}
