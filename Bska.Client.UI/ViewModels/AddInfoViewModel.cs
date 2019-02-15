
namespace Bska.Client.UI.ViewModels
{
    using Bska.Client.Domain.Entity.AssetEntity;
    using Bska.Client.UI.API;
    using System;
    using System.Windows;
    using System.Windows.Input;
    public sealed class AddInfoViewModel : BaseDetailsViewModel<UnConsumption>
    {
        #region ctor

        public AddInfoViewModel(UnConsumption currentEntity)
            : base(currentEntity)
        {
            this.initalizCommand();
        }

        #endregion

        #region properties

        public Byte[] Image1
        {
            get { return CurrentEntity.Image1; }
            set
            {
                CurrentEntity.Image1 = value;
                OnPropertyChanged("Image1");
            }
        }

        public Byte[] Image2
        {
            get { return CurrentEntity.Image2; }
            set
            {
                CurrentEntity.Image2 = value;
                OnPropertyChanged("Image2");
            }
        }

        public Byte[] Image3
        {
            get { return CurrentEntity.Image3; }
            set
            {
                CurrentEntity.Image3 = value;
                OnPropertyChanged("Image3");
            }
        }

        public Byte[] Image4
        {
            get { return CurrentEntity.Image4; }
            set
            {
                CurrentEntity.Image4 = value;
                OnPropertyChanged("Image4");
            }
        }

        public byte[] GuaranteeImage
        {
            get { return CurrentEntity.GuaranteeImage; }
            set
            {
                CurrentEntity.GuaranteeImage = value;
                OnPropertyChanged("GuaranteeImage");
            }
        }
        
        #endregion

        #region methods

        private void confirm(object parameter)
        {
            var window = parameter as Window;
            if (window != null)
            {
                window.DialogResult = true;
            }
        }

        #endregion

        #region command

        public ICommand ConfirmCommand { get; private set; }

        private void initalizCommand()
        {
            ConfirmCommand = new MvvmCommand(
                (parameter) =>
                {
                    this.confirm(parameter);
                },
                (parameter) =>
                {
                    return true;
                }
                );
        }

        #endregion
    }
}
