
namespace Bska.Client.UI.ViewModels
{
    using Microsoft.Practices.Unity;
    using Services;
    using Domain.Entity.AssetEntity;
    using System.Collections.Generic;
    using System.Linq;
    using System;
    using Data.Service;
    using System.Threading.Tasks;
    using Repository.Model;
    using Helper;
    using System.Windows.Input;
    using API;
    using Common;

    public sealed class MainAccountDocViewModel : BaseViewModel
    {
        #region ctor

        public MainAccountDocViewModel(IUnityContainer container,AccountDocumentMaster currentDoc)
        {
            this._container = container;
            this._dialogService = _container.Resolve<IDialogService>();
            this._navigationService = _container.Resolve<INavigationService>();
            this._employeeService = _container.Resolve<IEmployeeService>();
            this.CurrentDocument = currentDoc;
            this.initializObj();
            this.initializCommands();
        }
        #endregion

        #region properties

        public AccountDocumentMaster CurrentDocument
        {
            get { return GetValue(() => CurrentDocument); }
            set
            {
                SetValue(() => CurrentDocument, value);
            }
        }

        public List<AccountDocumentDetailsModel> AccountDocuments
        {
            get { return GetValue(() => AccountDocuments); }
            set
            {
                SetValue(() => AccountDocuments, value);
            }
        }

        public String DebtorDescription
        {
            get { return GetValue(() => DebtorDescription); }
            set
            {
                SetValue(() => DebtorDescription, value);
            }
        }

        public String CreditorDescription
        {
            get { return GetValue(() => CreditorDescription); }
            set
            {
                SetValue(() => CreditorDescription, value);
            }
        }
        #endregion

        #region methods

        private async void initializObj()
        {
            var item = await this.setAccountDetailsAsync();
            this.AccountDocuments = item.Item1;
            DebtorDescription = $"جمع بدهکار : {item.Item2.ToString("N0")}";
            CreditorDescription = $"جمع بستانکار : {item.Item3.ToString("N0")}";
        }

        private Task<Tuple<List<AccountDocumentDetailsModel>, decimal, decimal>> setAccountDetailsAsync()
        {
            var ts = new Task<Tuple<List<AccountDocumentDetailsModel>, decimal, decimal>>(() =>
            {
                var lsitem = new List<AccountDocumentDetailsModel>();
                var coings = _employeeService.GetAccountCodings().ToList();
                decimal debtor = 0;
                decimal creditor = 0;
                _employeeService.GetAccountDetailsByMaster(CurrentDocument.ID)
                .GroupBy(g => new {g.AccountNo,g.Description})
                .OrderBy(x=>x.Key.AccountNo).ForEach(acd =>
                {
                    string[] splitCoding = acd.Key.AccountNo.Split('-');
                    string code = splitCoding[2];
                    string totalCode = splitCoding[1];
                    var cod = coings.FirstOrDefault(x => x.AccountCode == code);
                    var totalCod = coings.FirstOrDefault(x => x.AccountCode == totalCode);
                    var item = new AccountDocumentDetailsModel
                    {
                        AccountNo = acd.Key.AccountNo,
                        Creditor = acd.Sum(x=>x.Creditor),
                        Debtor = acd.Sum(x=>x.Debtor),
                        Description = acd.Key.Description
                    };
                    if (cod != null)
                    {
                        item.TotalAccount = totalCod.Name;
                    }
                    else
                    {
                        item.TotalAccount = "ناشناخته";
                    }
                    lsitem.Add(item);
                });

                debtor =lsitem.Sum(x=>x.Debtor);
                creditor = lsitem.Sum(x => x.Creditor);
                return new Tuple<List<AccountDocumentDetailsModel>, decimal, decimal>(lsitem, debtor, creditor);
            });
            ts.Start();
            return ts;
        }

        private void report()
        {
            Mouse.SetCursor(Cursors.Wait);
            var viewModel = new ReportViewModel();
            viewModel.AccountDocMainGroupedReport(CurrentDocument.ID, CurrentDocument.AccountDate.PersianDateString());
            _navigationService.ShowReportViewWindow(viewModel);

            Mouse.SetCursor(Cursors.Arrow);
        }

        #endregion

        #region commands

        public ICommand ReportCommand { get; private set; }

        private void initializCommands()
        {
            ReportCommand = new MvvmCommand(
                (parameter) => { this.report(); },
                (parameter) => { return true; }
                );
        }
        #endregion

        #region fields

        private readonly IUnityContainer _container;
        private readonly IDialogService _dialogService;
        private readonly INavigationService _navigationService;
        private readonly IEmployeeService _employeeService;

        #endregion
    }
}
