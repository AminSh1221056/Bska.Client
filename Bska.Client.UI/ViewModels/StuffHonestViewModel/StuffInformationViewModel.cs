
namespace Bska.Client.UI.ViewModels.StuffHonestViewModel
{
    using Data.Service;
    using Microsoft.Practices.Unity;
    using System.Collections.ObjectModel;
    using System.Collections.Generic;
    using System.Linq;
    using Domain.Entity;
    using System;
    using System.Threading.Tasks;
    using Helper;
    using System.Windows.Input;
    using Bska.Client.API.UnitOfWork;
    using Bska.Client.UI.Services;
    using System.Windows;
    using System.Data.Entity.Infrastructure;
    using Bska.Client.UI.API;
    using Bska.Client.API.Infrastructure;
    using Bska.Client.UI.ViewModels.TreeViewModels;
    using System.Data.Entity;
    using Bska.Client.Repository.Model;

    public sealed class StuffInformationViewModel : BaseViewModel
    {
        #region ctor

        public StuffInformationViewModel(IUnityContainer container)
        {
            this._container = container;
            this._navigationService = _container.Resolve<INavigationService>();
            this._dialogService = _container.Resolve<IDialogService>();
            this._stuffService = _container.Resolve<IStuffService>();
            this._firstGeneration = new ObservableCollection<KalaManageTreeViewModel>();
            this._stuffList = new ObservableCollection<Stuff>();
            this._editItems = new Dictionary<string, string>();
            this.initializObj();
        }

        #endregion

        #region properties

        public ObservableCollection<KalaManageTreeViewModel> FirstGeneration
        {
            get { return _firstGeneration; }
        }

        public KalaManageTreeViewModel SelectedNode
        {
            get { return GetValue(() => SelectedNode); }
            set
            {
                SetValue(() => SelectedNode, value);
                if (value != null)
                {
                    this.initStuffAsync();
                }
            }
        }

        public ObservableCollection<Stuff> Stuffs
        {
            get { return _stuffList; }
        }
        public Dictionary<string,string> EditItems
        {
            get { return _editItems; }
        }

        public string CurrentEdit
        {
            get { return GetValue(() => CurrentEdit); }
            set
            {
                SetValue(() => CurrentEdit, value);
                this.initOnCurrentEdit();
            }
        }

        #endregion

        #region methods

        private async void initializObj()
        {
           await Task.Run(() =>
            {
                _stuffService.Queryable().Include(st => st.OrganizationPefectStuffs)
               .ToList().Where(x => x.Parent == null).ForEach(x =>
               {
                   DispatchService.Invoke(() =>
                   {
                       _firstGeneration.Add(new KalaManageTreeViewModel(x, false));
                   });
               });
            });
            _editItems.Add("B001", "اموال قابل ثبت");
            _editItems.Add("B002", "اموال ثبت شده");
        }

        private void initOnCurrentEdit()
        {
            if (!string.IsNullOrEmpty(CurrentEdit))
            {
                if(string.Equals(CurrentEdit, "B001"))
                {
                    this.getStuffs();
                }
                else if(string.Equals(CurrentEdit, "B002"))
                {

                }
            }
        }

        private void getStuffs()
        {
            if (SelectedNode != null)
            {
                SelectedNode.IsSelected = false;
            }
            _stuffList.Clear();
            Task.Run(() =>
            {
                _stuffService.Query(s => s.IsStuff).Include(s => s.Parent).Select().ToList().ForEach(s =>
                {
                    DispatchService.Invoke(() =>
                    {
                        _stuffList.Add(s);
                    });
                });
            });
        }

        private async void initStuffAsync()
        {
            if (SelectedNode == null)
            {
                return;
            }
            Mouse.SetCursor(Cursors.Wait);
            if (string.Equals(CurrentEdit, "B001"))
            {
                _stuffList.Clear();
                await Task.Run(() =>
                {
                    _stuffService.Query().Include(x => x.Parent)
                        .Select().Where(x => x.IsStuff == true && x.StuffType == SelectedNode.StuffCurrent.StuffType).ToList().ForEach(x =>
                        {
                            if (SelectedNode != null)
                            {
                                bool isTrue = parentRecovery(x.Parent);
                                if (isTrue)
                                {
                                    DispatchService.Invoke(() =>
                                    {
                                        _stuffList.Add(x);
                                    });
                                }
                            }
                            else
                            {
                                DispatchService.Invoke(() =>
                                {
                                    _stuffList.Add(x);
                                });
                            }
                        });
                });
            }
            else if (string.Equals(CurrentEdit, "B002"))
            {

            }
            else if (string.Equals(CurrentEdit, "A001"))
            {
              
            }

            Mouse.SetCursor(Cursors.Arrow);
        }

        private Boolean parentRecovery(Stuff parent)
        {
            bool isChild = false;
            if (parent.Equals(SelectedNode.StuffCurrent))
            {
                isChild = true;
            }
            else if (parent.Parent != null)
            {
                isChild = this.parentRecovery(parent.Parent);
            }
            return isChild;
        }

        #endregion

        #region commands

        #endregion

        #region fields

        private readonly IUnityContainer _container;
        private readonly IStuffService _stuffService;
        private readonly IDialogService _dialogService;
        private readonly INavigationService _navigationService;
        private readonly ObservableCollection<KalaManageTreeViewModel> _firstGeneration;
        private readonly ObservableCollection<Stuff> _stuffList;
        private readonly Dictionary<string, string> _editItems;

        #endregion

    }
}
