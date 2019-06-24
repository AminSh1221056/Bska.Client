
namespace Bska.Client.UI.ViewModels.StuffHonestViewModel
{
    using Bska.Client.UI.API;
    using Bska.Client.Common;
    using Bska.Client.UI.Helper;
    using Bska.Client.Repository.Model;
    using Bska.Client.Domain.Entity;
    using Bska.Client.UI.Services;
    using Bska.Client.Data.Service;
    using System;
    using Microsoft.Practices.Unity;
    using System.Windows;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Windows.Input;
    using System.Threading.Tasks;
    using System.Threading;
    using Bska.Client.UI.ViewModels.AssetViewModel;

    public sealed class MAssetManageViewModel : BaseViewModel
    {
        #region ctor

        public MAssetManageViewModel(IUnityContainer container)
        {
            this._container = container;
            this._movableAssetService = _container.Resolve<IMovableAssetService>();
            this._commodityService = _container.Resolve<IMAssetCommodityService>();
            this._unitService = _container.Resolve<IUnitService>();
            this._personService = _container.Resolve<IPersonService>();
            this._employeeService = _container.Resolve<IEmployeeService>();
            this._navigationService = _container.Resolve<INavigationService>();
            this._organizCollection = new ObservableCollection<EmployeeDesignTreeViewModel>();
            this._strategyCollection = new ObservableCollection<EmployeeDesignTreeViewModel>();
            _collection = new ObservableCollection<MovableAssetModel>();
            this._allOrganizId = new List<int>();
            this._allStrategyId = new List<int>();
            this._allAsset = new List<MovableAssetModel>();
            this.initializObj();
            this.initializCommand();
        }

        #endregion

        #region properties

        public Window Window
        {
            get { return GetValue(() => Window); }
            set
            {
                SetValue(() => Window, value);
            }
        }

        public Dictionary<string, object> StuffTypes
        {
            get { return GetValue(() => StuffTypes); }
            set
            {
                SetValue(() => StuffTypes, value);
            }
        }

        public Dictionary<string, object> SelectedStuffType
        {
            get { return GetValue(() => SelectedStuffType); }
            set
            {
                SetValue(() => SelectedStuffType, value);
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

        public ObservableCollection<MovableAssetModel> Collection
        {
            get { return _collection; }
            set
            {
                _collection = value;
                OnPropertyChanged("Collection");
            }
        }

        public List<Unit> Units
        {
            get { return GetValue(() => Units); }
            set { SetValue(() => Units, value); }
        }

        public PersianDate FromDate
        {
            get { return GetValue(() => FromDate); }
            set
            {
                SetValue(() => FromDate, value);
            }
        }

        public PersianDate ToDate
        {
            get { return GetValue(() => ToDate); }
            set
            {
                SetValue(() => ToDate, value);
            }
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
        public MovableAssetModel Selected
        {
            get { return GetValue(() => Selected); }
            set
            {
                SetValue(() => Selected, value);
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

        public Boolean DateEnabled
        {
            get { return GetValue(() => DateEnabled); }
            set
            {
                SetValue(() => DateEnabled, value);
                if (value)
                {
                    FromDate = GlobalClass._Today.AddMonths(-(APPSettings.Default.SearchDateMonth)).PersianDateTime();
                    ToDate = GlobalClass._Today.PersianDateTime();
                }
            }
        }

        #endregion

        #region methods

        private async void initializObj()
        {
            Mouse.SetCursor(Cursors.Wait);

            if (Thread.CurrentPrincipal.IsInRole("Manager")
                || Thread.CurrentPrincipal.IsInRole("StuffHonest")
                || Thread.CurrentPrincipal.IsInRole("GeneralManager"))
            {
                Persons = _personService.Queryable().Where(x => x.LastName != "Administrator").Select(p => new PersonModel
                {
                    FullName = p.FirstName + " " + p.LastName,
                    NationalId = p.NationalId,
                    PersonId = p.PersonId
                }).ToList();

                Persons.Insert(0, new PersonModel { PersonId = 0, FullName = "کل پرسنل" });
            }
            else
            {
                Persons = _personService.Queryable().Where(x => x.PersonId ==UserLog.UniqueInstance.LogedUser.PersonId)
                    .Select(p => new PersonModel
                {
                    FullName = p.FirstName + " " + p.LastName,
                    NationalId = p.NationalId,
                    PersonId = p.PersonId
                }).ToList();

                if (Thread.CurrentPrincipal.IsInRole("StandardUser"))
                {

                }
            }

            StuffTypes = new Dictionary<string, object> { { StuffType.Consumable.GetDescription(),StuffType.Consumable}, { StuffType.UnConsumption.GetDescription(), StuffType.UnConsumption }
            ,{ StuffType.OrderConsumption.GetDescription(),StuffType.OrderConsumption},{ StuffType.Installable.GetDescription(),StuffType.Installable},{ StuffType.Belonging.GetDescription(),StuffType.Belonging}};

            SelectedStuffType = new Dictionary<string, object> {{ StuffType.UnConsumption.GetDescription(), StuffType.UnConsumption }
            ,{ StuffType.OrderConsumption.GetDescription(),StuffType.OrderConsumption},{ StuffType.Installable.GetDescription(),StuffType.Installable},{ StuffType.Belonging.GetDescription(),StuffType.Belonging}};


            SelectedPerson = Persons.First();
            
            Units = _unitService.Queryable().ToList();
            await this.GetParentNodeAsync();
            getMAssets();
            Mouse.SetCursor(Cursors.Arrow);
        }

        private Task GetParentNodeAsync()
        {
            _organizCollection.Clear();
            _strategyCollection.Clear();
            Task ts = new Task(() =>
            {
                var _allOrganiz = _employeeService.GetParentNode(1).ToList();
                var orgItems = _allOrganiz.Where(x => x.ParentNode == null);
                var _allStrategy = _employeeService.GetParentNode(2).ToList();
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

        private String GetHirecharyNode(EmployeeDesign item)
        {
            String _nodeName = "";

            if (item.ParentNode != null)
            {
                _nodeName += this.GetHirecharyNode(item.ParentNode);
                _nodeName += "***";
            }

            _nodeName += item.Name;

            return _nodeName;
        }

        private void getMAssets()
        {
            Mouse.SetCursor(Cursors.Wait);
            DateTime? date1 = default(DateTime?);
            DateTime? date2 = default(DateTime?);
            if (this.DateEnabled)
            {
                date1 = FromDate.ToDateTime();
                DateTime dt = ToDate.ToDateTime();
                date2 = new DateTime(dt.Year, dt.Month, dt.Day, GlobalClass._Today.Hour,
                    GlobalClass._Today.Minute, GlobalClass._Today.Second);
            }

            _allAsset = _movableAssetService.GetOuterMovableAssetByLocation(date1, date2)
                         .Where(x => SelectedStuffType.ContainsKey(x.MAssetType));

            if (SelectedStuffType.ContainsKey("مصرفی"))
            {
                var commodity=_commodityService.GetCommodityForOutOfStore(date1, date2);
                commodity.AsParallel().ForAll(co =>
                {
                    co.PersianInsertDate = co.InsertDate.PersianDateTime();
                });
                _allAsset.Concat(commodity);
            }

            if (SelectedPerson.PersonId != 0)
            {
                _allAsset = _allAsset.Where(x => x.PersonId == SelectedPerson.NationalId).ToList();
            }

            FilterByBuildingDesign();
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
            DateTime now = DateTime.Now;
            Mouse.SetCursor(Cursors.Wait);
            Task _globalTask = Task.Factory.StartNew(() =>
            {
                int i = 0;
                switch (_filteNo)
                {
                    case 1:
                        _allAsset.Where(x => _allStrategyId.Contains(x.StrategyId) && _allOrganizId.Contains(x.OrganizId)).AsParallel().ForAll(x =>
                        {
                            DispatchService.Invoke(() =>
                            {
                                _collection.Insert(i, x);
                            });
                            i++;
                        });
                        break;
                    case 2:
                        _allAsset.Where(x => _allOrganizId.Contains(x.OrganizId)).AsParallel().ForAll(x =>
                        {
                            DispatchService.Invoke(() =>
                            {
                                _collection.Insert(i, x);
                            });
                            i++;
                        });
                        break;
                    case 3:
                        _allAsset.Where(x => _allStrategyId.Contains(x.StrategyId)).AsParallel().ForAll(x =>
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
                            && x.OrganizId == OrganizSelected.BuildingDesignCurrent.BuidldingDesignId).AsParallel().ForAll(x =>
                            {
                                DispatchService.Invoke(() =>
                                {
                                    _collection.Insert(i, x);
                                });
                                i++;
                            });
                        break;
                    case 5:
                        _allAsset.Where(x => x.OrganizId == OrganizSelected.BuildingDesignCurrent.BuidldingDesignId).AsParallel().ForAll(x =>
                        {
                            DispatchService.Invoke(() =>
                            {
                                _collection.Insert(i, x);
                            });
                            i++;
                        });
                        break;
                    case 6:
                        _allAsset.Where(x => x.StrategyId == StrategySelected.BuildingDesignCurrent.BuidldingDesignId)
                        .AsParallel().ForAll(x =>
                        {
                            DispatchService.Invoke(() =>
                            {
                                _collection.Insert(i, x);
                            });
                            i++;
                        });
                        break;
                    default:
                        _allAsset.AsParallel().ForAll(x =>
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
                this.Collection = new ObservableCollection<MovableAssetModel>
                  (new List<MovableAssetModel> { new MovableAssetModel() });
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
            IEnumerable<MovableAssetModel> Commoditygroup = null;
            switch (_filteNo)
            {
                case 1:
                    Commoditygroup = (from c in _allAsset
                                      where (_allStrategyId.Contains(c.StrategyId) && _allOrganizId.Contains(c.OrganizId))
                                      group c by new { c.UnitId, c.KalaNo, c.MAssetType } into g
                                      where g.Count() >= 1
                                      select new MovableAssetModel
                                      {
                                          AssetId = g.First().AssetId,
                                          Name = g.First().Name,
                                          KalaNo = g.Key.KalaNo,
                                          kalaUid=g.First().kalaUid,
                                          Num = g.Sum(x => x.Num),
                                          UnitId = g.Key.UnitId,
                                          PersonId = SelectedPerson.NationalId,
                                          OrganizId = g.First().OrganizId,
                                          StrategyId = g.First().StrategyId,
                                          MAssetType = g.Key.MAssetType,
                                          PersianInsertDate = g.First().InsertDate.PersianDateTime(),
                                          Label = null
                                      });
                    break;
                case 2:
                    Commoditygroup = (from c in _allAsset
                                      where _allOrganizId.Contains(c.OrganizId)
                                      group c by new { c.UnitId, c.KalaNo, c.MAssetType } into g
                                      where g.Count() >= 1
                                      select new MovableAssetModel
                                      {
                                          AssetId = g.First().AssetId,
                                          Name = g.First().Name,
                                          KalaNo = g.Key.KalaNo,
                                          kalaUid = g.First().kalaUid,
                                          Num = g.Sum(x => x.Num),
                                          UnitId = g.Key.UnitId,
                                          PersonId = SelectedPerson.NationalId,
                                          OrganizId = g.First().OrganizId,
                                          StrategyId = 0,
                                          MAssetType = g.Key.MAssetType,
                                          PersianInsertDate = g.First().InsertDate.PersianDateTime(),
                                          Label = null
                                      });
                    break;
                case 3:
                    Commoditygroup = (from c in _allAsset
                                      where _allStrategyId.Contains(c.StrategyId)
                                      group c by new { c.UnitId, c.KalaNo, c.MAssetType } into g
                                      where g.Count() >= 1
                                      select new MovableAssetModel
                                      {
                                          AssetId = g.First().AssetId,
                                          Name = g.First().Name,
                                          KalaNo = g.Key.KalaNo,
                                          kalaUid = g.First().kalaUid,
                                          Num = g.Sum(x => x.Num),
                                          UnitId = g.Key.UnitId,
                                          PersonId = SelectedPerson.NationalId,
                                          OrganizId = 0,
                                          StrategyId = g.First().StrategyId,
                                          MAssetType = g.Key.MAssetType,
                                          PersianInsertDate = g.First().InsertDate.PersianDateTime(),
                                          Label = null
                                      });
                    break;
                case 4:
                    Commoditygroup = (from c in _allAsset
                                      where (c.OrganizId == OrganizSelected.BuildingDesignCurrent.BuidldingDesignId &&
                                      c.StrategyId == StrategySelected.BuildingDesignCurrent.BuidldingDesignId)
                                      group c by new { c.UnitId, c.KalaNo, c.MAssetType } into g
                                      where g.Count() >= 1
                                      select new MovableAssetModel
                                      {
                                          AssetId = g.First().AssetId,
                                          Name = g.First().Name,
                                          KalaNo = g.Key.KalaNo,
                                          kalaUid = g.First().kalaUid,
                                          Num = g.Sum(x => x.Num),
                                          UnitId = g.Key.UnitId,
                                          PersonId = SelectedPerson.NationalId,
                                          OrganizId = g.First().OrganizId,
                                          StrategyId = g.First().StrategyId,
                                          MAssetType = g.Key.MAssetType,
                                          PersianInsertDate = g.First().InsertDate.PersianDateTime(),
                                          Label = null
                                      });
                    break;
                case 5:
                    Commoditygroup = (from c in _allAsset
                                      where c.OrganizId == OrganizSelected.BuildingDesignCurrent.BuidldingDesignId
                                      group c by new { c.UnitId, c.KalaNo, c.MAssetType } into g
                                      where g.Count() >= 1
                                      select new MovableAssetModel
                                      {
                                          AssetId = g.First().AssetId,
                                          Name = g.First().Name,
                                          KalaNo = g.Key.KalaNo,
                                          kalaUid = g.First().kalaUid,
                                          Num = g.Sum(x => x.Num),
                                          UnitId = g.Key.UnitId,
                                          PersonId = SelectedPerson.NationalId,
                                          OrganizId = g.First().OrganizId,
                                          StrategyId = 0,
                                          MAssetType = g.Key.MAssetType,
                                          PersianInsertDate = g.First().InsertDate.PersianDateTime(),
                                          Label = null
                                      });
                    break;
                case 6:
                    Commoditygroup = (from c in _allAsset
                                      where c.StrategyId == StrategySelected.BuildingDesignCurrent.BuidldingDesignId
                                      group c by new { c.UnitId, c.KalaNo, c.MAssetType } into g
                                      where g.Count() >= 1
                                      select new MovableAssetModel
                                      {
                                          AssetId = g.First().AssetId,
                                          Name = g.First().Name,
                                          KalaNo = g.Key.KalaNo,
                                          kalaUid = g.First().kalaUid,
                                          Num = g.Sum(x => x.Num),
                                          UnitId = g.Key.UnitId,
                                          PersonId = SelectedPerson.NationalId,
                                          OrganizId = 0,
                                          StrategyId = g.First().StrategyId,
                                          MAssetType = g.Key.MAssetType,
                                          PersianInsertDate = g.First().InsertDate.PersianDateTime(),
                                          Label = null
                                      });
                    break;
                default:
                    Commoditygroup = (from c in _allAsset
                                      group c by new { c.UnitId, c.KalaNo, c.MAssetType } into g
                                      where g.Count() >= 1
                                      select new MovableAssetModel
                                      {
                                          AssetId = g.First().AssetId,
                                          Name = g.First().Name,
                                          KalaNo = g.Key.KalaNo,
                                          kalaUid = g.First().kalaUid,
                                          Num = g.Sum(x => x.Num),
                                          UnitId = g.Key.UnitId,
                                          PersonId = SelectedPerson.NationalId,
                                          OrganizId = 0,
                                          StrategyId = 0,
                                          MAssetType = g.Key.MAssetType,
                                          PersianInsertDate = g.First().InsertDate.PersianDateTime(),
                                          Label = null
                                      });
                    break;
            }

            if (Commoditygroup != null)
            {
                _collection.Clear();
                Commoditygroup.ForEach(x =>
                    {
                        _collection.Add(x);
                    });
            }

            Mouse.SetCursor(Cursors.Arrow);
        }
        
        private void showMAssetDetails(object parameter)
        {
            var mAsset = parameter as MovableAssetModel;
            if (mAsset == null) return;
            Mouse.SetCursor(Cursors.Wait);
            StoryboardManager.PlayStoryboard("StoryboardFadeOut", Window);
            this.Selected = mAsset;
            if (string.Equals(mAsset.MAssetType, "مصرفی"))
            {
                var asset = _commodityService.Query(ma => ma.AssetId == mAsset.AssetId)
                    .Include(ma => ma.StoreBill).Include(ma => ma.PlaceOfUses).Select().Single();
                var viewModel = new CommodityDetailsViewModel(_container, asset.AssetId, null);
               _navigationService.ShowCommodityDetailsWindow(viewModel);
            }
            else
            {
                var viewModel = new MovableAssetDetailsViewModel(_container, mAsset.AssetId);
                _navigationService.ShowMAssetDetailsWindow(viewModel);
            }
          
            StoryboardManager.PlayStoryboard("StoryboardFadeIn", Window);
            Mouse.SetCursor(Cursors.Arrow);
        }

        private void ReportMAssetsList()
        {
            Mouse.SetCursor(Cursors.Wait);

            var viewModel = new ReportViewModel();
            string personName = SelectedPerson.FullName;
            string personId = null;
            string organizPath = "";
            string strategyPath = "";
            int orgId = 0;
            int strId = 0;
            string[] Type = new string[5];
            DateTime? pDate1 = default(DateTime?);
            DateTime? pDate2 = default(DateTime?);

            if (SelectedStuffType.ContainsKey("مصرفی"))
            {
                Type[0] = "11001";
            }

            if (SelectedStuffType.ContainsKey("غیرمصرفی"))
            {
                Type[1] = "11002";
            }

            if (SelectedStuffType.ContainsKey("در حکم مصرف"))
            {
                Type[2] = "11003";
            }

            if (SelectedStuffType.ContainsKey("قابل نصب در بنا"))
            {
                Type[3] = "11004";
            }

            if (SelectedStuffType.ContainsKey("متعلقات"))
            {
                Type[4] = "11005";
            }
            

            if (SelectedPerson.PersonId > 0)
            {
                personId = SelectedPerson.NationalId;
            }

            if (OrganizSelected != null && (_filteNo != 3 || _filteNo != 6))
            {
                organizPath = GetHirecharyNode(OrganizSelected.BuildingDesignCurrent);
                orgId = OrganizSelected.BuildingDesignCurrent.BuidldingDesignId;
            }

            if (StrategySelected != null && (_filteNo != 2 || _filteNo != 5))
            {
                strategyPath = GetHirecharyNode(StrategySelected.BuildingDesignCurrent);
                strId = StrategySelected.BuildingDesignCurrent.BuidldingDesignId;
            }

            if (DateEnabled)
            {
                pDate1 = FromDate.ToDateTime();
                pDate2 = ToDate.ToDateTime();
            }

            viewModel.MAssetListReport(organizPath, strategyPath, _filteNo, orgId, strId
                , personId, personName,Type,pDate1,pDate2,"",ChGroupView);
            _navigationService.ShowReportViewWindow(viewModel);

            Mouse.SetCursor(Cursors.Arrow);
        }

        #endregion

        #region commands

        public ICommand SearchCommand { get; private set; }
        public ICommand EditCommand { get; private set; }
        public ICommand ReportCommand { get; private set; }
        public ICommand DoubleClickListViewItemCommand { get; private set; }
        private void initializCommand()
        {
            SearchCommand = new MvvmCommand(
               (parameter) => { this.getMAssets(); },
               (parameter) => { return true; }
               );

            EditCommand = new MvvmCommand(
                (parameter) => { this.showMAssetDetails(parameter); },
                (parameter) => { return true; }
                );

            ReportCommand = new MvvmCommand(
              (parameter) =>
              {
                 this.ReportMAssetsList();
              },
              (parameter) => { return true; }
              );

            DoubleClickListViewItemCommand = new MvvmCommand(
             (parameter) => { this.showMAssetDetails(parameter); },
             (parameter) => { return true; }
             );
        }

        #endregion

        #region fields

        private readonly IUnityContainer _container;
        private readonly IMovableAssetService _movableAssetService;
        private readonly IMAssetCommodityService _commodityService;
        private readonly IPersonService _personService;
        private readonly IEmployeeService _employeeService;
        private readonly IUnitService _unitService;
        private readonly INavigationService _navigationService;
        private readonly ObservableCollection<EmployeeDesignTreeViewModel> _organizCollection;
        private readonly ObservableCollection<EmployeeDesignTreeViewModel> _strategyCollection;
        private ObservableCollection<MovableAssetModel> _collection;
        private List<int> _allOrganizId;
        private List<int> _allStrategyId;
        private int _filteNo = 7;
        IEnumerable<MovableAssetModel> _allAsset;

        #endregion
    }
}
