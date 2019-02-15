
namespace Bska.Client.UI.Services
{
    public interface IDialogService
    {
        void ShowInfo(string title, string message);

        void ShowError(string title, string message);

        void ShowAlert(string title, string message);

        bool AskConfirmation(string title, string message);
    }
}
