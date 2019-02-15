
namespace Bska.Client.UI.ViewModels.PersonDetailsInfoViewModels
{
    using Bska.Client.Data.Service;
    using Bska.Client.Domain.Entity;
    using Bska.Client.Repository.Model;
    using Bska.Client.UI.API;
    using Bska.Client.UI.Services;
    using Microsoft.Practices.Unity;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;
    using System.Windows.Input;
    using System.Threading.Tasks;
    using System.Collections.ObjectModel;
    using Helper;
    using Common;
    using System.ComponentModel;
    using System.Windows.Data;

    public sealed class PersonAssetDetailsViewModel : BaseViewModel
    {
        #region ctor

        public PersonAssetDetailsViewModel(IUnityContainer container,int personId)
        {
            this._container = container;
            this._movableAssetService = _container.Resolve<IMovableAssetService>();
            this._unitService = _container.Resolve<IUnitService>();
            this._personService = _container.Resolve<IPersonService>();
            this._employeeService = _container.Resolve<IEmployeeService>();
            this._dialogService = _container.Resolve<IDialogService>();
            this._navigationService = _container.Resolve<INavigationService>();
            this._organizCollection = new ObservableCollection<EmployeeDesignTreeViewModel>();
            this._strategyCollection = new ObservableCollection<EmployeeDesignTreeViewModel>();
            this._collection = new ObservableCollection<MovableAssetModel>();
            this.Assets = new CollectionViewSource { Source = Collection }.View;
            this._currentPersonId = personId;
            this._allOrganizId = new List<int>();
            this._allStrategyId = new List<int>();
            this.initalizObj();
            this.initializCommands();
        }

        #endregion

        #region properties
        
        public Boolean ChConsumSelectd
        {
            get { return GetValue(() => ChConsumSelectd); }
            set
            {
                SetValue(() => ChConsumSelectd, value);
                if (value) _mAssetTypes.Add("مصرفی");
                else _mAssetTypes.Remove("مصرفی");
            }
        }

        public Boolean ChInConsumSelectd
        {
            get { return GetValue(() => ChInConsumSelectd); }
            set
            {
                SetValue(() => ChInConsumSelectd, value);
                if (value) _mAssetTypes.Add("در حکم مصرف");
                else _mAssetTypes.Remove("در حکم مصرف");
            }
        }

        public Boolean ChUnConsumSelectd
        {
            get { return GetValue(() => ChUnConsumSelectd); }
            set
            {
                SetValue(() => ChUnConsumSelectd, value);
                if (value) _mAssetTypes.Add("غیرمصرفی");
                else _mAssetTypes.Remove("غیرمصرفی");
            }
        }

        public Boolean ChInstallableSelectd
        {
            get { return GetValue(() => ChInstallableSelectd); }
            set
            {
                SetValue(() => ChInstallableSelectd, value);
                if (value) _mAssetTypes.Add("قابل نصب در بنا");
                else _mAssetTypes.Remove("قابل نصب در بنا");
            }
        }

        public Boolean ChBelongingelectd
        {
            get { return GetValue(() => ChBelongingelectd); }
            set
            {
                SetValue(() => ChBelongingelectd, value);
                if (value) _mAssetTypes.Add("متعلقات");
                else _mAssetTypes.Remove("متعلقات");
            }
        }
        
        public ObservableCollection<MovableAssetModel> Collection
        {
            get { return _collection; }
        }

        public ICollectionView Assets { get; set; }
        public List<Unit> Units
        {
            get { return GetValue(() => Units); }
            set
            {
                SetValue(() => Units, value);
            }
        }
        public List<PersonModel> Persons
        {
            get { return GetValue(() => Persons); }
            set
            {
                SetValue(() => Persons, value);
            }
        }

        public PersonModel SelectedPerson
        {
            get { return GetValue(() => SelectedPerson); }
            set
            {
                SetValue(() => SelectedPerson, value);
                this.getAssetsAsync();
            }
        }

        public Person CurrentPerson
        {
            get { return GetValue(() => CurrentPerson); }
            set
            {
                SetValue(() => CurrentPerson, value);
            }
        }

        public EmployeeDesignTreeViewModel StrategySelected
        {
            get { return GetValue(() => StrategySelected); }
            set
            {
                SetValue(() => StrategySelected, value);
                if (value != null)
                {
                    if (_filteNo != 2 && _filteNo != 5)
                    {
                        _allStrategyId.Clear();
                        this.GetAllStrategyId(value.BuildingDesignCurrent);
                        this.FilterByBuildingDesign();
                    }
                }
            }
        }

        public EmployeeDesignTreeViewModel OrganizSelected
        {
            get { return GetValue(() => OrganizSelected); }
            set
            {
                SetValue(() => OrganizSelected, value);
                if (value != null)
                {
                    if (_filteNo != 3 && _filteNo != 6)
                    {
                        _allOrganizId.Clear();
                        this.GetAllOrganizId(value.BuildingDesignCurrent);
                        this.FilterByBuildingDesign();
                    }
                }
            }
        }
        public ObservableCollection<EmployeeDesignTreeViewModel> OrganizCollection
        {
            get { return _organizCollection; }
        }

        public ObservableCollection<EmployeeDesignTreeViewModel> StrategyCollection
        {
            get { return _strategyCollection; }
        }

        public Boolean ChGroupView
        {
            get { return GetValue(() => ChGroupView); }
            set
            {
                SetValue(() => ChGroupView, value);
                if (value)
                {
                    this.GroupingFiltering();
                }
                else
                {
                    this.normalFilteringAsync();
                }
            }
        }
        public Boolean NestPropertyView
        {
            get { return GetValue(() => NestPropertyView); }
            set
            {
                SetValue(() => NestPropertyView, value);
            }
        }
        public Boolean OrganizFiltering
        {
            get { return GetValue(() => OrganizFiltering); }
            set
            {
                SetValue(() => OrganizFiltering, value);
                if (value)
                {
                    _filteNo = 2;
                    StrategySelected = null;
                }
                else
                {
                    if (!StrategyFiltering)
                    {
                        if (NestPropertyView)
                        {
                            _filteNo = 4;
                        }
                        else
                        {
                            _filteNo = 1;
                        }
                    }
                }
            }
        }

        public Boolean StrategyFiltering
        {
            get { return GetValue(() => StrategyFiltering); }
            set
            {
                SetValue(() => StrategyFiltering, value);
                if (value)
                {
                    _filteNo = 3;
                    OrganizSelected = null;
                }
                else
                {
                    if (!OrganizFiltering)
                    {
                        if (NestPropertyView)
                        {
                            _filteNo = 4;
                        }
                        else
                        {
                            _filteNo = 1;
                        }
                    }
                }
            }
        }

        public String SearchCriteria
        {
            get { return GetValue(() => SearchCriteria); }
            set
            {
                SetValue(() => SearchCriteria, value);
                this.movableAssetFilters();
            }
        }

        #endregion

        #region methods

        private async void initalizObj()
        {
            if (UserLog.UniqueInstance.LogedUser.PermissionType == PermissionsType.Manager)
            {
                Persons = _personService.Queryable().Where(p=>p.PersonId!=1).Select(p => new PersonModel
                {
                    FullName = p.FirstName + " " + p.LastName,
                    NationalId = p.NationalId,
                    PersonId = p.PersonId
                }).ToList();
            }
            else
            {
                Persons = _personService.Queryable()
                   .Where(x => x.PersonId == _currentPersonId).Select(x => new PersonModel
                   {
                       FullName = x.FirstName + " " + x.LastName,
                       NationalId = x.NationalId,
                       PersonId = x.PersonId
                   }).ToList();
            }

            if (Persons.Count <= 0) return;

            SelectedPerson = Persons.SingleOrDefault(p => p.PersonId == _currentPersonId);
            
            ChConsumSelectd = ChInConsumSelectd = ChUnConsumSelectd =ChInstallableSelectd=ChBelongingelectd= true;
            this.Units = _unitService.Queryable().ToList();
            await this.GetParentNodeAsync();
        }
        
        private void showMAssetDetails(IList<object> parameter)
        {
            if (!(parameter[1] is Window)) return;
            var mAsset = parameter[0] as MovableAssetModel;
            if (mAsset == null) return;
            Mouse.SetCursor(Cursors.Wait);
            var viewModel = new MovableAssetDetailsViewModel(_container,mAsset.AssetId);
            StoryboardManager.PlayStoryboard("StoryboardHideWindow", parameter[1] as Window);
            var window= _navigationService.ShowMAssetDetailsWindow(viewModel);
            StoryboardManager.PlayStoryboard("StoryboardShowWindow", parameter[1] as Window);
            Mouse.SetCursor(Cursors.Arrow);
        }

        private async void getAssetsAsync()
        {
            if (SelectedPerson == null) return;

            CurrentPerson = _personService.Find(SelectedPerson.PersonId);
            var ts = new Task(() =>
            {
                _allAsset= _movableAssetService.GetPersonAssets(SelectedPerson.NationalId).Where(x => _mAssetTypes.Contains(x.MAssetType));
            });
            ts.Start();
            await ts;
            FilterByBuildingDesign();
        }

        private Task GetParentNodeAsync()
        {
            Task ts = new Task(() =>
              {
                  _allOrganiz = _employeeService.GetParentNode(1).ToList();
                  var orgItems = _allOrganiz.Where(x => x.ParentNode == null);
                  _allStrategy = _employeeService.GetParentNode(2).ToList();
                  var stgyItems = _allStrategy.Where(x => x.ParentNode == null);

                  DispatchService.Invoke(() =>
                  {
                      StrategySelected = null;
                      OrganizSelected = null;
                      _organizCollection.Clear();
                      _strategyCollection.Clear();

                      foreach (var org in orgItems)
                      {
                          _organizCollection.Add(new EmployeeDesignTreeViewModel(org, null));
                      }

                      foreach (var stgy in stgyItems)
                      {
                          _strategyCollection.Add(new EmployeeDesignTreeViewModel(stgy, null, null));
                      }
                  });
              });
            ts.Start();
            return ts;
        }

        private void GetAllOrganizId(EmployeeDesign selectedItem)
        {
            if (selectedItem.ChildNode.Count > 0)
            {
                foreach (var k in selectedItem.ChildNode.AsParallel<EmployeeDesign>())
                {
                    this.GetAllOrganizId(k);
                }
            }

            _allOrganizId.Add(selectedItem.BuidldingDesignId);
        }

        private void GetAllStrategyId(EmployeeDesign selectedItem)
        {
            if (selectedItem.ChildNode.Count > 0)
            {
                foreach (var k in selectedItem.ChildNode.AsParallel<EmployeeDesign>())
                {
                    this.GetAllStrategyId(k);
                }
            }

            _allStrategyId.Add(selectedItem.BuidldingDesignId);
        }

        private void FilterByBuildingDesign()
        {
            if (_allAsset == null) return;
            if (NestPropertyView)
            {
                if (OrganizFiltering)
                {
                    if (OrganizSelected != null)
                    {
                        _filteNo = 5;
                    }
                }
                else if (StrategyFiltering)
                {
                    if (StrategySelected != null)
                    {
                        _filteNo = 6;
                    }
                }
                else
                {
                    if (OrganizSelected != null && StrategySelected != null)
                    {
                        _filteNo = 4;
                    }
                    else if ((OrganizSelected != null && StrategySelected == null) || (OrganizSelected == null && StrategySelected != null))
                    {
                        return;
                    }
                }
            }
            else
            {
                if (OrganizFiltering)
                {
                    if (OrganizSelected != null)
                    {
                        _filteNo = 2;
                    }
                }
                else if (StrategyFiltering)
                {
                    if (StrategySelected != null)
                    {
                        _filteNo = 3;
                    }
                }
                else
                {
                    if (OrganizSelected != null && StrategySelected != null)
                    {
                        _filteNo = 1;
                    }
                    else if ((OrganizSelected != null && StrategySelected == null) || (OrganizSelected == null && StrategySelected != null))
                    {
                        return;
                    }
                }
            }
            if (ChGroupView) this.GroupingFiltering();
            else this.normalFilteringAsync();
        }

        private async void normalFilteringAsync()
        {
            Mouse.SetCursor(Cursors.Wait);
            Task _globalTask = Task.Factory.StartNew(() =>
            {
                int i = 0;
                switch (_filteNo)
                {
                    case 1:
                        _allAsset.Where(x => _allStrategyId.Contains(x.StrategyId) && _allOrganizId.Contains(x.OrganizId)).ForEach(x =>
                        {
                            DispatchService.Invoke(() =>
                            {
                                _collection.Insert(i, x);
                            });
                            i++;
                        });
                        break;
                    case 2:
                        _allAsset.Where(x => _allOrganizId.Contains(x.OrganizId)).ForEach(x =>
                        {
                            DispatchService.Invoke(() =>
                            {
                                _collection.Insert(i, x);
                            });
                            i++;
                        });
                        break;
                    case 3:
                        _allAsset.Where(x => _allStrategyId.Contains(x.StrategyId)).ForEach(x =>
                        {
                            DispatchService.Invoke(() =>
                            {
                                _collection.Insert(i, x);
                            });
                            i++;
                        });
                        break;
                    case 4:
                        _allAsset.Where(x => x.StrategyId == StrategySelected.BuildingDesignCurrent.BuidldingDesignId
                            && x.OrganizId == OrganizSelected.BuildingDesignCurrent.BuidldingDesignId).ForEach(x =>
                            {
                                DispatchService.Invoke(() =>
                                {
                                    _collection.Insert(i, x);
                                });
                                i++;
                            });
                        break;
                    case 5:
                        _allAsset.Where(x => x.OrganizId == OrganizSelected.BuildingDesignCurrent.BuidldingDesignId).ForEach(x =>
                        {
                            DispatchService.Invoke(() =>
                            {
                                _collection.Insert(i, x);
                            });
                            i++;
                        });
                        break;
                    case 6:
                        _allAsset.Where(x => x.StrategyId == StrategySelected.BuildingDesignCurrent.BuidldingDesignId).ForEach(x =>
                        {
                            DispatchService.Invoke(() =>
                            {
                                _collection.Insert(i, x);
                            });
                            i++;
                        });
                        break;
                    default:
                        _allAsset.ForEach(x =>
                        {
                            DispatchService.Invoke(() =>
                            {
                                _collection.Insert(i, x);
                            });
                            i++;
                        });
                        break;
                }
            });
            try
            {
                _collection.Clear();
                await _globalTask;
            }
            catch (ArgumentOutOfRangeException) { }
            catch (Exception) { throw; }
            Mouse.SetCursor(Cursors.Arrow);
        }

        private void GroupingFiltering()
        {
            Mouse.SetCursor(Cursors.Wait);
            List<MovableAssetModel> Commoditygroup = null;
            switch (_filteNo)
            {
                case 1:
                    Commoditygroup = (from c in _allAsset
                                      where (_allStrategyId.Contains(c.StrategyId) && _allOrganizId.Contains(c.OrganizId))
                                      group c by new { c.UnitId, c.Name, c.MAssetType } into g
                                      where g.Count() >= 1
                                      select new MovableAssetModel
                                      {
                                          AssetId = g.First().AssetId,
                                          Name = g.Key.Name,
                                          Num = g.Sum(x => x.Num),
                                          UnitId = g.Key.UnitId,
                                          PersonId = SelectedPerson.NationalId,
                                          OrganizId = g.First().OrganizId,
                                          StrategyId = g.First().StrategyId,
                                          MAssetType = g.Key.MAssetType,
                                          InsertDate = g.First().InsertDate,
                                          Label = null
                                      }).ToList();
                    break;
                case 2:
                    Commoditygroup = (from c in _allAsset
                                      where _allOrganizId.Contains(c.OrganizId)
                                      group c by new { c.UnitId, c.Name, c.MAssetType } into g
                                      where g.Count() >= 1
                                      select new MovableAssetModel
                                      {
                                          AssetId = g.First().AssetId,
                                          Name = g.Key.Name,
                                          Num = g.Sum(x => x.Num),
                                          UnitId = g.Key.UnitId,
                                          PersonId = SelectedPerson.NationalId,
                                          OrganizId = g.First().OrganizId,
                                          StrategyId = 0,
                                          MAssetType = g.Key.MAssetType,
                                          InsertDate = g.First().InsertDate,
                                          Label = null
                                      }).ToList();
                    break;
                case 3:
                    Commoditygroup = (from c in _allAsset
                                      where _allStrategyId.Contains(c.StrategyId)
                                      group c by new { c.UnitId, c.Name, c.MAssetType } into g
                                      where g.Count() >= 1
                                      select new MovableAssetModel
                                      {
                                          AssetId = g.First().AssetId,
                                          Name = g.Key.Name,
                                          Num = g.Sum(x => x.Num),
                                          UnitId = g.Key.UnitId,
                                          PersonId = SelectedPerson.NationalId,
                                          OrganizId = 0,
                                          StrategyId = g.First().StrategyId,
                                          MAssetType = g.Key.MAssetType,
                                          InsertDate = g.First().InsertDate,
                                          Label = null
                                      }).ToList();
                    break;
                case 4:
                    Commoditygroup = (from c in _allAsset
                                      where (c.OrganizId == OrganizSelected.BuildingDesignCurrent.BuidldingDesignId &&
                                      c.StrategyId == StrategySelected.BuildingDesignCurrent.BuidldingDesignId)
                                      group c by new { c.UnitId, c.Name, c.MAssetType } into g
                                      where g.Count() >= 1
                                      select new MovableAssetModel
                                      {
                                          AssetId = g.First().AssetId,
                                          Name = g.Key.Name,
                                          Num = g.Sum(x => x.Num),
                                          UnitId = g.Key.UnitId,
                                          PersonId = SelectedPerson.NationalId,
                                          OrganizId = g.First().OrganizId,
                                          StrategyId = g.First().StrategyId,
                                          MAssetType = g.Key.MAssetType,
                                          InsertDate = g.First().InsertDate,
                                          Label = null
                                      }).ToList();
                    break;
                case 5:
                    Commoditygroup = (from c in _allAsset
                                      where c.OrganizId == OrganizSelected.BuildingDesignCurrent.BuidldingDesignId
                                      group c by new { c.UnitId, c.Name, c.MAssetType } into g
                                      where g.Count() >= 1
                                      select new MovableAssetModel
                                      {
                                          AssetId = g.First().AssetId,
                                          Name = g.Key.Name,
                                          Num = g.Sum(x => x.Num),
                                          UnitId = g.Key.UnitId,
                                          PersonId = SelectedPerson.NationalId,
                                          OrganizId = g.First().OrganizId,
                                          StrategyId = 0,
                                          MAssetType = g.Key.MAssetType,
                                          InsertDate = g.First().InsertDate,
                                          Label = null
                                      }).ToList();
                    break;
                case 6:
                    Commoditygroup = (from c in _allAsset
                                      where c.StrategyId == StrategySelected.BuildingDesignCurrent.BuidldingDesignId
                                      group c by new { c.UnitId, c.Name, c.MAssetType } into g
                                      where g.Count() >= 1
                                      select new MovableAssetModel
                                      {
                                          AssetId = g.First().AssetId,
                                          Name = g.Key.Name,
                                          Num = g.Sum(x => x.Num),
                                          UnitId = g.Key.UnitId,
                                          PersonId = SelectedPerson.NationalId,
                                          OrganizId = 0,
                                          StrategyId = g.First().StrategyId,
                                          MAssetType = g.Key.MAssetType,
                                          InsertDate = g.First().InsertDate,
                                          Label = null
                                      }).ToList();
                    break;
                default:
                    Commoditygroup = (from c in _allAsset
                                      group c by new { c.UnitId, c.Name, c.MAssetType } into g
                                      where g.Count() >= 1
                                      select new MovableAssetModel
                                      {
                                          AssetId = g.First().AssetId,
                                          Name = g.Key.Name,
                                          Num = g.Sum(x => x.Num),
                                          UnitId = g.Key.UnitId,
                                          PersonId = SelectedPerson.NationalId,
                                          OrganizId = 0,
                                          StrategyId = 0,
                                          MAssetType = g.Key.MAssetType,
                                          InsertDate = g.First().InsertDate,
                                          Label = null
                                      }).ToList();
                    break;
            }

            if (Commoditygroup != null)
            {
                _collection.Clear();
                Commoditygroup.ForEach(x => _collection.Add(x));
            }

            Mouse.SetCursor(Cursors.Arrow);
        }

        private void movableAssetFilters()
        {
            this.Assets.Filter = ((obj) =>
            {
                MovableAssetModel items = obj as MovableAssetModel;
                if (items != null)
                    return items.Name.StartsWith(SearchCriteria) || items.Label.ToString() == SearchCriteria;
                return false;
            });
        }

        #endregion

        #region commands

        public ICommand DetailsCommand { get; private set; }
        private void initializCommands()
        {
            DetailsCommand = new MvvmCommand(
               (parameter) => { this.showMAssetDetails(parameter as IList<object>); },
               (parameter) => { return true; }
               );
        }

        #endregion

        #region fields

        private readonly int _currentPersonId;
        private readonly IUnityContainer _container;
        private readonly IMovableAssetService _movableAssetService;
        private readonly IEmployeeService _employeeService;
        private readonly IUnitService _unitService;
        private readonly IPersonService _personService;
        private readonly IDialogService _dialogService;
        private readonly INavigationService _navigationService;
        private HashSet<String> _mAssetTypes = new HashSet<String>();
        private readonly ObservableCollection<EmployeeDesignTreeViewModel> _organizCollection;
        private readonly ObservableCollection<EmployeeDesignTreeViewModel> _strategyCollection;
        private List<EmployeeDesign> _allOrganiz;
        private List<EmployeeDesign> _allStrategy;
        private List<int> _allOrganizId;
        private List<int> _allStrategyId;
        private int _filteNo = 7;
        IEnumerable<MovableAssetModel> _allAsset;
        private readonly ObservableCollection<MovableAssetModel> _collection;

        #endregion
    }
}
