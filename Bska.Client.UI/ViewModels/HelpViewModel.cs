
namespace Bska.Client.UI.ViewModels
{
    using Bska.Client.UI.API;
    using Bska.Client.UI.Views;
    using MoonPdfLib;
    using System;
    using System.Windows.Input;

    public sealed class HelpViewModel:BaseViewModel
    {
        #region ctor
        public HelpViewModel(string fileNam,int page)
        {
            this.FileName = fileNam;
            this.Page = page;
        }

        #endregion

        #region properties
        public string FileName
        {
            get { return GetValue(() => FileName); }
            private set
            {
                SetValue(() => FileName, value);
            }
        }

        public int Page
        {
            get { return GetValue(() => Page); }
            private set
            {
                SetValue(() => Page, value);
            }
        }

        #endregion

        #region methods
        #endregion

        #region commands
        
        public DelegateCommand RotateRightCommand { get; private set; }
        public DelegateCommand RotateLeftCommand { get; private set; }

        public DelegateCommand NextPageCommand { get; private set; }
        public DelegateCommand PreviousPageCommand { get; private set; }
        public DelegateCommand FirstPageCommand { get; private set; }
        public DelegateCommand LastPageCommand { get; private set; }

        public DelegateCommand SinglePageCommand { get; private set; }
        public DelegateCommand FacingCommand { get; private set; }
        public DelegateCommand BookViewCommand { get; private set; }

        public DelegateCommand TogglePageDisplayCommand { get; private set; }

        public DelegateCommand ZoomInCommand { get; private set; }
        public DelegateCommand ZoomOutCommand { get; private set; }
        public DelegateCommand FitWidthCommand { get; private set; }
        public DelegateCommand FitHeightCommand { get; private set; }
        public DelegateCommand CustomZoomCommand { get; private set; }

        internal void initializCommands(HelpWindow wnd,MoonPdfPanel panel)
        {
            var pdfPanel = panel;
            Predicate<object> isPdfLoaded = f => panel!=null;
            this.PreviousPageCommand = new DelegateCommand("صفحه قبلی", f => pdfPanel.GotoPreviousPage(), isPdfLoaded, new KeyGesture(Key.Left));
            this.NextPageCommand = new DelegateCommand("صفحه بعدی", f => pdfPanel.GotoNextPage(), isPdfLoaded, new KeyGesture(Key.Right));
            this.FirstPageCommand = new DelegateCommand("صفحه نخست", f => pdfPanel.GotoFirstPage(), isPdfLoaded, new KeyGesture(Key.Home));
            this.LastPageCommand = new DelegateCommand("صفحه آخر", f => pdfPanel.GotoLastPage(), isPdfLoaded, new KeyGesture(Key.End));
         
            this.RotateRightCommand = new DelegateCommand("چرخش به راست", f => pdfPanel.RotateRight(), isPdfLoaded, new KeyGesture(Key.Add, ModifierKeys.Control | ModifierKeys.Shift));
            this.RotateLeftCommand = new DelegateCommand("چرخش به چپ", f => pdfPanel.RotateLeft(), isPdfLoaded, new KeyGesture(Key.Subtract, ModifierKeys.Control | ModifierKeys.Shift));

            this.ZoomInCommand = new DelegateCommand("بزرگنمایی", f => pdfPanel.ZoomIn(), isPdfLoaded, new KeyGesture(Key.Add));
            this.ZoomOutCommand = new DelegateCommand("کوچک نمایی", f => pdfPanel.ZoomOut(), isPdfLoaded, new KeyGesture(Key.Subtract));

            this.FitWidthCommand = new DelegateCommand("عرض متناسب", f => pdfPanel.ZoomToWidth(), isPdfLoaded, new KeyGesture(Key.D4, ModifierKeys.Control));
            this.FitHeightCommand = new DelegateCommand("ارتفاع مناسب", f => pdfPanel.ZoomToHeight(), isPdfLoaded, new KeyGesture(Key.D5, ModifierKeys.Control));
            this.CustomZoomCommand = new DelegateCommand("Custom zoom", f => pdfPanel.SetFixedZoom(), isPdfLoaded, new KeyGesture(Key.D6, ModifierKeys.Control));

            this.TogglePageDisplayCommand = new DelegateCommand("نمایش صفحات به طور مداوم", f => pdfPanel.TogglePageDisplay(), isPdfLoaded, new KeyGesture(Key.D7, ModifierKeys.Control));
            
            this.SinglePageCommand = new DelegateCommand("نمایش یک صفحه", f => { pdfPanel.ViewType = MoonPdfLib.ViewType.SinglePage; }, isPdfLoaded, new KeyGesture(Key.D1, ModifierKeys.Control));
            this.FacingCommand = new DelegateCommand("Facing", f => { pdfPanel.ViewType = MoonPdfLib.ViewType.Facing; }, isPdfLoaded, new KeyGesture(Key.D2, ModifierKeys.Control));
            this.BookViewCommand = new DelegateCommand("نمایش کتابی", f => { pdfPanel.ViewType = MoonPdfLib.ViewType.BookView; }, isPdfLoaded, new KeyGesture(Key.D3, ModifierKeys.Control));


            this.RegisterInputBindings(wnd);
        }

        private void RegisterInputBindings(HelpWindow wnd)
        {
            // register inputbindings for all commands (properties)
            foreach (var pi in typeof(HelpViewModel).GetProperties())
            {
                var cmd = pi.GetValue(this, null) as BaseCommand;
                if (cmd != null)
                {
                    if (cmd.InputBinding != null)
                        wnd.InputBindings.Add(cmd.InputBinding);
                }
            }
        }

        #endregion

        #region fields
        #endregion
    }
}
