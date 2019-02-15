
namespace Bska.Client.UI.Services
{
    using System.Windows;

    public class DialogService : IDialogService
    {
        /// <summary> 
        /// Shows the info. 
        /// </summary> 
        /// <param name="title">The title.</param> 
        /// <param name="message">The message.</param> 
        public void ShowInfo(string title, string message)
        {
            MessageBox.Show(message, title, MessageBoxButton.OK,MessageBoxImage.Asterisk);
        }

        /// <summary> 
        /// Shows the error. 
        /// </summary> 
        /// <param name="title">The title.</param> 
        /// <param name="message">The message.</param> 
        public void ShowError(string title, string message)
        {
            MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Error);
        }

        /// <summary> 
        /// Shows the alert. 
        /// </summary> 
        /// <param name="title">The title.</param> 
        /// <param name="message">The message.</param> 
        public void ShowAlert(string title, string message)
        {
            MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Exclamation);
        }

        /// <summary> 
        /// Asks the confirmation. 
        /// </summary> 
        /// <param name="title">The title.</param> 
        /// <param name="message">The message.</param> 
        /// <returns></returns>
        public bool AskConfirmation(string title, string message)
        {
            return
               MessageBox.Show(message, title, MessageBoxButton.YesNo,MessageBoxImage.Question)== MessageBoxResult.Yes;
        }
    }
}
