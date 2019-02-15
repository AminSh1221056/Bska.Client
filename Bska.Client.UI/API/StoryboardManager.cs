
namespace Bska.Client.UI.API
{
    using System;
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Media.Animation;
    using System.Linq;

    /// <summary>
    /// The purpose of this class is to provide a means of handling storyboard notifications
    /// without violation of proper decoupling precepts.
    /// </summary>
    public static class StoryboardManager
    {
        // **************** Class dependency properties ************************************* //

        /// <summary>
        /// This is the attached property that allows a storyboard declared in XAML to
        /// specify a string that can be used as it unique ID. The act of it passing over
        /// its ID also gives this static class the opportunity to remember the storyboard
        /// object as well.
        /// </summary>
        public static DependencyProperty IDProperty =
            DependencyProperty.RegisterAttached(
                "ID",
                typeof(string),
                typeof(StoryboardManager),
                new PropertyMetadata(null, IDChanged));


        // **************** Class data members ********************************************** //

        public static Dictionary<string, Storyboard> _storyboards = new Dictionary<string, Storyboard>();

        public static Dictionary<string, Queue<Action>> _storyboardsCompletedHandlers = new Dictionary<string, Queue<Action>>();


        // **************** Support of accessing storyboard ID ****************************** //

        /// <summary>
        /// This method is called when the storyboard's ID changes.
        /// </summary>
        /// <param name="obj">Storyboard object that is sending in an ID.</param>
        /// <param name="e">Object that wraps the changed ID.</param>
        private static void IDChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            do
            {
                // Sanity check. Ensure that the caller is a storyboard.
                Storyboard sb = obj as Storyboard;

                if (sb == null)
                {
                    // Not a storyboard. Do not process.
                    break;
                }

                // Convert the incoming changed data to a string key that is supposed to
                // identify the storyboard.
                string key = e.NewValue as string;

                // Create an end of storyboard event that will remove any other storyboard
                // complete handler that may be associated with this particular storyboard.
                EventHandler completedHandler =
                    (sender, args) =>
                    {
                        // Does the storyboard have an associated completed handler?

                        if (!_storyboardsCompletedHandlers[key].Any())
                        {
                            // No.
                            return;
                        }

                        // Extract the existing completed handler from the list, if any.
                        var action = _storyboardsCompletedHandlers[key].Dequeue();

                        // Completed handler found?
                        if (action != null)
                        {
                            // Yes. Execute it.
                            action();
                        }
                    };

                // Is this particular storyboard present in our collection of storyboards?
                if (!_storyboards.ContainsKey(key))
                {
                    // No. Add it.
                    try
                    {
                        sb.Completed += completedHandler;
                    }
                    catch /*(Exception ex)*/
                    {
                        //throw new ApplicationException( "Cannot register completed event handler for the story board. Is it frozen?", ex );
                    }

                    // Add the storyboard to the collection of storyboards and make sure
                    // that the completed handler is saved as well.
                    _storyboards.Add(key, sb);
                    _storyboardsCompletedHandlers.Add(key, new Queue<Action>());
                }
            } while (false);
        }


        /// <summary>
        /// Save the ID to the attached property storage.
        /// </summary>
        /// <param name="obj">Object that owns the property. In this case it is a storyboard.</param>
        /// <param name="id">ID to be saved.</param>
        public static void SetID(DependencyObject obj, string id)
        {
            obj.SetValue(IDProperty, id);
        }


        /// <summary>
        /// Retrieve the ID for the specified storyboard.
        /// </summary>
        /// <param name="obj">Storyboard that owns the ID.</param>
        /// <returns>
        /// The string that represents this storyboard's ID is returned.
        /// </returns>
        public static string GetID(DependencyObject obj)
        {
            return obj.GetValue(IDProperty) as string;
        }


        // **************** Class members *************************************************** //

        /// <summary>
        /// Called by a client to start a particular storyboard.
        /// </summary>
        /// <param name="id">Specifies the storyboard to be started.</param>
        /// <param name="onCompleted">Optional callback to be invoked when storyboard finishes running.</param>
        public static void PlayStoryboard(string id, FrameworkElement frameWorkElement, Action onCompleted = null)
        {
            if (frameWorkElement != null)
            {
                if (_storyboards.ContainsKey(id))
                {
                    var sb = _storyboards[id];
                    _storyboardsCompletedHandlers[id].Enqueue(onCompleted);
                    sb.Begin(frameWorkElement);
                }
            }
            else
            {
                if (_storyboards.ContainsKey(id))
                {
                    var sb = _storyboards[id];
                    _storyboardsCompletedHandlers[id].Enqueue(onCompleted);
                    sb.Begin();
                }
            }
        }
    }
}
