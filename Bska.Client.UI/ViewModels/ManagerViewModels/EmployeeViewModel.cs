
namespace Bska.Client.UI.ViewModels
{
    using Bska.Client.API.UnitOfWork;
    using Bska.Client.Common;
    using Bska.Client.Data.Service;
    using Bska.Client.Domain.Entity;
    using Bska.Client.UI.API;
    using Bska.Client.API.Infrastructure;
    using Bska.Client.UI.Services;
    using Microsoft.Practices.Unity;
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.IO;
    using System.Windows.Input;
    using System.Linq;
    using System.Data.Entity;
    using Helper;
    using System.Threading.Tasks;
    using System.Net.Http;
    using System.Collections.Generic;
    using Newtonsoft.Json;
    using System.Xml.Linq;
    using System.Runtime.ExceptionServices;
    using Repository.Model;
    using System.Transactions;

    public sealed class EmployeeViewModel : BaseDetailsViewModel<Employee>
    {
        #region ctor

        public EmployeeViewModel(IUnityContainer container,Employee currentEntity)
            :base(currentEntity)
        {
            this._container = container;
            this._dialogService = _container.Resolve<IDialogService>();
            this._navigationService = _container.Resolve<INavigationService>();
            this._unitOfWork = _container.Resolve<IUnitOfWorkAsync>();
            this._employeeService = _container.Resolve<IEmployeeService>(new ParameterOverride("repository", _unitOfWork.Repository<Employee>()));
            this.IdentificationCode = "";
            this._helper = new SeedDataHelper();
            this._client = UserLog.UniqueInstance.Client;
            this.initalizObj();
            this.initalizCommand();
        }

        #endregion

        #region properties

        public Int32 EmployeeId
        {
            get { return CurrentEntity.EmployeeId; }
        }

        [Required(ErrorMessage = "نام سازمان الزامی است")]
        public String Name
        {
            get { return CurrentEntity.Name; }
            set
            {
                CurrentEntity.Name = value;
                ValidateProperty(value);
                OnPropertyChanged("Name");
            }
        }

        [Required(ErrorMessage = "شماره ثبت سازمان الزامی است")]
        public String RegisterationNo
        {
            get { return CurrentEntity.RegisterationNo; }
            set
            {
                CurrentEntity.RegisterationNo = value;
                ValidateProperty(value);
                OnPropertyChanged("RegisterationNo");
            }
        }

        [Required(ErrorMessage = "کد بودجه الزامی است")]
        public int BudgetNo
        {
            get { return CurrentEntity.BudgetNo; }
            set
            {
                CurrentEntity.BudgetNo = value;
                ValidateProperty(value);
                OnPropertyChanged("BudgetNo");
            }
        }

        public String Tell
        {
            get { return CurrentEntity.Tell; }
            set
            {
                CurrentEntity.Tell = value;
                OnPropertyChanged("Tell");
            }
        }

        public String WebAddress
        {
            get { return CurrentEntity.WebAddress; }
            set
            {
                CurrentEntity.WebAddress = value;
                OnPropertyChanged("WebAddress");
            }
        }

        [RegularExpression("^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$", ErrorMessage = "آدرس پستی وارد شده نامعتبر است")]
        public String Email
        {
            get { return CurrentEntity.Email; }
            set
            {
                CurrentEntity.Email = value;
                ValidateProperty(value);
                OnPropertyChanged("Email");
            }
        }

        public String Fax
        {
            get { return CurrentEntity.Fax; }
            set
            {
                CurrentEntity.Fax = value;
                OnPropertyChanged("Fax");
            }
        }

        public String AddressLine
        {
            get { return CurrentEntity.AddressLine; }
            set
            {
                CurrentEntity.AddressLine = value;
                OnPropertyChanged("AddressLine");
            }
        }

        public Byte[] Image
        {
            get { return CurrentEntity.Logo; }
            set
            {
                CurrentEntity.Logo = value;
                OnPropertyChanged("Image");
            }
        }

        public String IdentificationCode
        {
            get { return GetValue(() => IdentificationCode); }
            set
            {
                SetValue(() => IdentificationCode, value);
            }
        }

        [Required(ErrorMessage = "نام سازمان بالا دستی الزامی است")]
        public String ParentName
        {
            get { return CurrentEntity.ParentName; }
            set
            {
                CurrentEntity.ParentName = value;
                ValidateProperty(value);
                OnPropertyChanged("ParentName");
            }
        }
        
        public Province Province
        {
            get { return GetValue(() => Province); }
            set
            {
                SetValue(() => Province, value);
            }
        }
        
        public TwonShip Twonship
        {
            get { return GetValue(() => Twonship); }
            set
            {
                SetValue(() => Twonship, value);
            }
        }
        
        public Zone Zone
        {
            get { return GetValue(() => Zone); }
            set
            {
                SetValue(() => Zone, value);
            }
        }
        
        public City City
        {
            get { return GetValue(() => City); }
            set
            {
                SetValue(() => City, value);
            }
        }

        public string ReportString
        {
            get { return GetValue(() => ReportString); }
            set
            {
                SetValue(() => ReportString, value);
            }
        }

        #endregion

        #region methods

        private async void initalizObj()
        {
            ReportString = "هویت شناخته شده نیست!";
            _currentEmployee = _employeeService.Queryable().AsNoTracking().FirstOrDefault();
            if (_currentEmployee != null)
            {
                this.Name = _currentEmployee.Name;
                this.ParentName = _currentEmployee.ParentName;
                this.Email = _currentEmployee.Email;
                this.AddressLine = _currentEmployee.AddressLine;
                this.Fax = _currentEmployee.Fax;
                this.Image = _currentEmployee.Logo;
                this.WebAddress = _currentEmployee.WebAddress;
                this.Tell = _currentEmployee.Tell;
                this.RegisterationNo = _currentEmployee.RegisterationNo;
                this.CurrentEntity.EmployeeId = _currentEmployee.EmployeeId;
                this.CurrentEntity.CreateDate = _currentEmployee.CreateDate;
                this.Image = _currentEmployee.Logo;
                var ls = new List<string>();
                ls.Insert(0, _currentEmployee.Province);
                ls.Insert(1, _currentEmployee.TwonShip);
                ls.Insert(2, _currentEmployee.Zone);
                ls.Insert(3, _currentEmployee.City);
                var ts=getState(ls);
                if (ts != null)
                {
                    await ts;
                }

                if (!string.IsNullOrWhiteSpace(APPSettings.Default.employeeCer))
                {
                    var dtnDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(APPSettings.Default.employeeCer);
                    if (dtnDictionary.ContainsKey(_currentEmployee.BudgetNo.ToString()))
                    {
                        IdentificationCode = GlobalClass.DecryptStringAES(dtnDictionary[_currentEmployee.BudgetNo.ToString()], "66Ak679Du4V3qo92");
                    }
                }
            }
        }

        private Task getState(List<String> addressIds)
        {
            if (addressIds.Count() != 4) return null;
            return Task.Run(() =>
            {
                Province province= _helper.GetProvinces().FirstOrDefault(x => x.ID == addressIds[0]);
                if (province != null)
                {
                    DispatchService.Invoke(() =>
                    {
                        this.Province = province;
                    });
                }
            }).ContinueWith(t1=>
            {
                TwonShip twonship= _helper.GetTwonShips(addressIds[0]).FirstOrDefault(x => x.ID == addressIds[1]);
                if (twonship != null)
                {
                    DispatchService.Invoke(() =>
                    {
                        this.Twonship = twonship;
                    });
                }
            }).ContinueWith(t2=>
            {
                Zone zone= _helper.GetZones(addressIds[1]).FirstOrDefault(x => x.ID == addressIds[2]);
                if (zone != null)
                {
                    DispatchService.Invoke(() =>
                    {
                        this.Zone = zone;
                    });
                }
            }).ContinueWith(t3=>
            {
                City city = _helper.GetCities(addressIds[2]).FirstOrDefault(x => x.ID == addressIds[3]);
                if (city != null)
                {
                    DispatchService.Invoke(() =>
                    {
                        this.City = city;
                    });
                }
            });
        }

        private async void PerformIdentityAccuracy()
        {
            if (GlobalClass.CheckForInternetConnection(Settings.Default.ServerUrl))
            {
                _dialogService.ShowAlert("توجه",ErrorMessages.Default.NoInternetAccess);
                return;
            }

            if (IdentificationCode.Length != 16)
            {
                _dialogService.ShowAlert("توجه", "کد شناسایی نامعتبر است");
                return;
            }

            if (this.HasErrors)
            {
                _dialogService.ShowError("خطا", ErrorMessages.Default.InputInvalid);
                return;
            }

            Mouse.SetCursor(Cursors.Wait);

            try
            {
                string path = $"{Settings.Default.APIServiceURL}/{HelpPageingAddress.Default.EmployeeIdentityLink}/{IdentificationCode}";
                _organ = await this.GetCustomerAsync(path);
                if (_organ == null)
                {
                    _dialogService.ShowAlert("توجه", "هیچ سازمانی یافت نشد");
                    return;
                }
                this.Name = _organ.Name;
                this.ParentName = _organ.ParentName;
                this.AddressLine = _organ.AddressLine;
                this.Email = _organ.Email;
                this.Fax = _organ.Fax;
                this.Tell = _organ.Tell;
                this.RegisterationNo = _organ.NationalCode;
                this.WebAddress = _organ.WebAddress;
                this.BudgetNo = _organ.BudgetNo;
                var ls = new List<string>();
                ls.Insert(0, _organ.Province);
                ls.Insert(1, _organ.TownShip);
                ls.Insert(2, _organ.Zone);
                ls.Insert(3, _organ.City);
                var ts = getState(ls);
                if (ts != null)
                {
                    await ts;
                }
                ReportString = "هویت شناسایی شد";
            }
            catch (TimeoutException)
            {
                _dialogService.ShowError("خطا", "هیچ پاسخی از سیستم مرکزی دریافت نشد");
            }
            catch (FileNotFoundException)
            {
                throw new FileNotFoundException("یک یا چند فایل در آدرس مورد نظر یافت نشد");
            }
            catch (Exception ex)
            {
                throw ex;
            }

            Mouse.SetCursor(Cursors.Arrow);
        }

        private async Task<CustomerModel> GetCustomerAsync(string path)
        {
            CustomerModel customer = null;
            HttpResponseMessage response = await _client.GetAsync(path);
            if (response.IsSuccessStatusCode)
            {
                customer = await response.Content.ReadAsAsync<CustomerModel>();
            }
            return customer;
        }

        private async Task<int> UpdateCustomerAsync(CustomerModel model)
        {
            HttpResponseMessage response = await _client.PutAsJsonAsync($"{Settings.Default.APIServiceURL}/{HelpPageingAddress.Default.EmployeeIdentityLink}/{IdentificationCode}", model);
            response.EnsureSuccessStatusCode();

            // Deserialize the updated product from the response body.
            return  await response.Content.ReadAsAsync<int>();
        }

        private async void saveEmployee()
        {
            if (GlobalClass.CheckForInternetConnection(Settings.Default.ServerUrl))
            {
                _dialogService.ShowAlert("توجه", ErrorMessages.Default.NoInternetAccess);
                return;
            }
            if (_organ == null)
            {
                _dialogService.ShowAlert("توجه", "هویت شناخته شده نیست");
                return;
            }
            if (this.HasErrors)
            {
                _dialogService.ShowError("خطا", ErrorMessages.Default.InputInvalid);
                return;
            }
            bool confirm = _dialogService.AskConfirmation("ویرایش سازمان", "آیا مطمئن به ثبت این اطلاعات می باشید");
            if (confirm)
            {
                var option = new TransactionOptions();
                option.IsolationLevel = IsolationLevel.ReadCommitted;
                using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required, option))
                {
                    CurrentEntity.City = _organ.City;
                    CurrentEntity.Zone = _organ.Zone;
                    CurrentEntity.TwonShip = _organ.TownShip;
                    CurrentEntity.Province = _organ.Province;
                    if (_currentEmployee == null)
                    {
                        this.CurrentEntity.ObjectState = ObjectState.Added;
                        this.CurrentEntity.CreateDate = GlobalClass._Today;

                        CurrentEntity.EmployeeDesign.Add(new StrategyDesign
                        {
                            Name = "آزاد",
                            ObjectState = ObjectState.Added,
                        });
                        CurrentEntity.EmployeeDesign.Add(new OrganizationDesign
                        {
                            Name = "آزاد",
                            ObjectState = ObjectState.Added,
                        });
                        CurrentEntity.EmployeeDesign.Add(new StrategyDesign
                        {
                            Name = CurrentEntity.Name,
                            ObjectState = ObjectState.Added,
                        });
                        CurrentEntity.EmployeeDesign.Add(new OrganizationDesign
                        {
                            Name = CurrentEntity.Name,
                            ObjectState = ObjectState.Added,
                        });
                        CurrentEntity.AccountDocumentCodings.Add(this.initAccountCodings());
                        CurrentEntity.Sellers.Add(new Seller
                        {
                            AddressLine = "1",
                            City = CurrentEntity.City,
                            TownShip = CurrentEntity.TwonShip,
                            Province = CurrentEntity.Province,
                            Zone = CurrentEntity.Zone,
                            Coding = "111111",
                            Lastname = "1",
                            Mobile = "11111111",
                            Name = "موجودی اولیه",
                            ObjectState = ObjectState.Added,
                            Tell = "111111",
                            Type = SellerType.RealSeller,
                        });
                        _employeeService.InsertOrUpdateGraph(CurrentEntity);
                    }
                    else
                    {
                        this.CurrentEntity.ObjectState = ObjectState.Modified;
                        _employeeService.Update(CurrentEntity);
                    }

                    CustomerModel sendOrgan = new CustomerModel
                    {
                        Email = Email,
                        AddressLine = AddressLine,
                        Fax = Fax,
                        Tell = Tell,
                        WebAddress = WebAddress,
                    };

                    try
                    {
                        Mouse.SetCursor(Cursors.Wait);
                        _unitOfWork.SaveChanges();
                        await this.UpdateCustomerAsync(sendOrgan);
                        this.createRelatedEmployeeCer(GlobalClass.EncryptStringAES(IdentificationCode, "66Ak679Du4V3qo92"), 
                            CurrentEntity.BudgetNo);

                        UserLog.UniqueInstance.AddLog(new BskaUIHelper().eventLogCreator("ثبت یا ویرایش سیستم در سیستم مرکزی و دریافت مجوز فعالیت", EventType.Update));
                       
                        this.setDbServers();
                        scope.Complete();
                        if (UserLog.UniqueInstance.LogedEmployee == null)
                        {
                            UserLog.UniqueInstance.LogedEmployee = this.CurrentEntity;
                        }

                        Mouse.SetCursor(Cursors.Arrow);
                    }
                    catch (TransactionAbortedException ex)
                    {
                        _dialogService.ShowError("TransactionAbortedException", ex.Message);
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }

                _dialogService.ShowInfo("توجه", ErrorMessages.Default.SuccessfullyMessage);
            }
        }

        private void createRelatedEmployeeCer(string encriptCode,int budgetNo)
        {
            var dtnDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(APPSettings.Default.employeeCer);
            if (dtnDictionary == null)
            {
                dtnDictionary = new Dictionary<string, string>();
            }

            if (!dtnDictionary.ContainsKey(budgetNo.ToString()))
            {
                dtnDictionary.Add(budgetNo.ToString(), encriptCode);
            }

            APPSettings.Default.employeeCer = JsonConvert.SerializeObject(dtnDictionary);
            APPSettings.Default.Save();
            APPSettings.Default.Reload();
        }

        private void setDbServers()
        {
            var dtnDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(APPSettings.Default.DbServers);
            if (dtnDictionary == null)
            {
                dtnDictionary = new Dictionary<string, string>();
            }

            if (!dtnDictionary.ContainsKey(Settings.Default.initialCatalog))
            {
                try
                {
                    dtnDictionary.Add(Settings.Default.initialCatalog, this.Name);
                    APPSettings.Default.DbServers = JsonConvert.SerializeObject(dtnDictionary);
                    APPSettings.Default.Save();
                    APPSettings.Default.Reload();
                }
                catch (Exception ex)
                {
                    ExceptionDispatchInfo.Capture(ex).Throw();
                }
            }
        }

        private AccountDocumentCoding initAccountCodings()
        {
            var parent = new AccountDocumentCoding
            {
                AccountCode = "1",
                Name = CurrentEntity.Name,
                ObjectState = ObjectState.Added,
                TotalAccountType = AccountingDescrtiption.None,
                CertainAccountType = CertainAccountsType.None,
            };

            var child10 = new AccountDocumentCoding
            {
                AccountCode = "01",
                Name = "حساب اموال دستگاه اجرایی",
                TotalAccountType = AccountingDescrtiption.Executive,
                CertainAccountType = CertainAccountsType.None,
                ObjectState = ObjectState.Added
            };

            child10.Childeren.Add(new AccountDocumentCoding
            {
                AccountCode = "0101",
                Name = "حساب معین دفتری برحسب تسلسل برچسب",
                TotalAccountType = AccountingDescrtiption.Executive,
                CertainAccountType = CertainAccountsType.ExecutiveSequenceLabel,
                ObjectState = ObjectState.Added
            });
            parent.Childeren.Add(child10);

            var child1 = new AccountDocumentCoding
            {
                AccountCode = "02",
                Name = "حساب اموال رسیده",
                TotalAccountType = AccountingDescrtiption.Reached,
                CertainAccountType = CertainAccountsType.None,
                ObjectState = ObjectState.Added
            };

            child1.Childeren.Add(new AccountDocumentCoding
            {
                AccountCode = "0201",
                Name = "خریداری",
                TotalAccountType = AccountingDescrtiption.Reached,
                CertainAccountType = CertainAccountsType.ReachedAssetBuy,
                ObjectState = ObjectState.Added
            });

            child1.Childeren.Add(new AccountDocumentCoding
            {
                AccountCode = "0202",
                Name = "انتقالی",
                TotalAccountType = AccountingDescrtiption.Reached,
                CertainAccountType = CertainAccountsType.ReachedAssetTransfer,
                ObjectState = ObjectState.Added
            });
            child1.Childeren.Add(new AccountDocumentCoding
            {
                AccountCode = "0203",
                Name = "اهدایی",
                TotalAccountType = AccountingDescrtiption.Reached,
                CertainAccountType = CertainAccountsType.ReachedAssetDenotion,
                ObjectState = ObjectState.Added
            });
            child1.Childeren.Add(new AccountDocumentCoding
            {
                AccountCode = "0204",
                Name = "تملیکی",
                TotalAccountType = AccountingDescrtiption.Reached,
                CertainAccountType = CertainAccountsType.ReachedAssetOther,
                ObjectState = ObjectState.Added
            });
            parent.Childeren.Add(child1);

            var child2 = new AccountDocumentCoding
            {
                AccountCode = "03",
                Name = "حساب اموال موجود در انبار",
                TotalAccountType = AccountingDescrtiption.Stock,
                CertainAccountType = CertainAccountsType.None,
                ObjectState = ObjectState.Added
            };

            child2.Childeren.Add(new AccountDocumentCoding
            {
                AccountCode = "0301",
                Name = "حساب معین برحسب نوع مال و بهای خرید آن",
                TotalAccountType = AccountingDescrtiption.Stock,
                CertainAccountType = CertainAccountsType.StockAssetBuyAndType,
                ObjectState = ObjectState.Added
            });

            parent.Childeren.Add(child2);

            var child3 = new AccountDocumentCoding
            {
                AccountCode = "04",
                Name = "حساب اموال تحویلی به واحدها",
                TotalAccountType = AccountingDescrtiption.Units,
                CertainAccountType = CertainAccountsType.None,
                ObjectState = ObjectState.Added
            };

            child3.Childeren.Add(new AccountDocumentCoding
            {
                AccountCode = "0401",
                Name = "حساب معین برحسب واحد تحویل گیرنده",
                TotalAccountType = AccountingDescrtiption.Units,
                CertainAccountType = CertainAccountsType.UnitsDeliviry,
                ObjectState = ObjectState.Added
            });
            parent.Childeren.Add(child3);

            var child4 = new AccountDocumentCoding
            {
                AccountCode = "05",
                Name = "حساب اموال امانی",
                TotalAccountType = AccountingDescrtiption.Trust,
                CertainAccountType = CertainAccountsType.None,
                ObjectState = ObjectState.Added
            };

            child4.Childeren.Add(new AccountDocumentCoding
            {
                AccountCode = "0501",
                Name = "حساب معین بر حسب دستگاه اجرایی امانت گیرنده",
                TotalAccountType = AccountingDescrtiption.Trust,
                CertainAccountType = CertainAccountsType.TrustOrganizationReciver,
                ObjectState = ObjectState.Added
            });
            parent.Childeren.Add(child4);

            var child5 = new AccountDocumentCoding
            {
                AccountCode = "06",
                Name = "حساب اموال امانی رسیده",
                TotalAccountType = AccountingDescrtiption.ReachedTrust,
                CertainAccountType = CertainAccountsType.None,
                ObjectState = ObjectState.Added
            };

            child5.Childeren.Add(new AccountDocumentCoding
            {
                AccountCode = "0601",
                Name = "حساب معین بر حسب دستگاه اجرایی امانت دهنده",
                TotalAccountType = AccountingDescrtiption.ReachedTrust,
                CertainAccountType = CertainAccountsType.ReachedTrustOrganizationSender,
                ObjectState = ObjectState.Added
            });
            parent.Childeren.Add(child5);

            var child6 = new AccountDocumentCoding
            {
                AccountCode = "07",
                Name = "طرف حساب اموال امانی رسیده",
                TotalAccountType = AccountingDescrtiption.Escrow,
                CertainAccountType = CertainAccountsType.None,
                ObjectState = ObjectState.Added
            };

            child6.Childeren.Add(new AccountDocumentCoding
            {
                AccountCode = "0701",
                Name = "طرف حساب اموال امانی رسیده",
                TotalAccountType = AccountingDescrtiption.Escrow,
                CertainAccountType = CertainAccountsType.Escrow,
                ObjectState = ObjectState.Added
            });
            parent.Childeren.Add(child6);

            var child7 = new AccountDocumentCoding
            {
                AccountCode = "08",
                Name = "حساب اموال اسقاط",
                TotalAccountType = AccountingDescrtiption.Retiring,
                CertainAccountType = CertainAccountsType.None,
                ObjectState = ObjectState.Added
            };

            child7.Childeren.Add(new AccountDocumentCoding
            {
                AccountCode = "0801",
                Name = "حساب معین برحسب نوع مال",
                TotalAccountType = AccountingDescrtiption.Retiring,
                CertainAccountType = CertainAccountsType.RetiringAssetType,
                ObjectState = ObjectState.Added
            });
            parent.Childeren.Add(child7);

            var child8 = new AccountDocumentCoding
            {
                AccountCode = "09",
                Name = "حساب اموال حادثه دیده",
                TotalAccountType = AccountingDescrtiption.Disaster,
                CertainAccountType = CertainAccountsType.None,
                ObjectState = ObjectState.Added
            };

            child8.Childeren.Add(new AccountDocumentCoding
            {
                AccountCode = "0901",
                Name = "حساب معین برحسب نوع مال",
                TotalAccountType = AccountingDescrtiption.Disaster,
                CertainAccountType = CertainAccountsType.DisasterAssetType,
                ObjectState = ObjectState.Added
            });
            parent.Childeren.Add(child8);

            var child9 = new AccountDocumentCoding
            {
                AccountCode = "10",
                Name = "حساب اموال فرستاده",
                TotalAccountType = AccountingDescrtiption.Send,
                CertainAccountType = CertainAccountsType.None,
                ObjectState = ObjectState.Added
            };

            child9.Childeren.Add(new AccountDocumentCoding
            {
                AccountCode = "1001",
                Name = "حساب معین اموال انتقالی",
                TotalAccountType = AccountingDescrtiption.Send,
                CertainAccountType = CertainAccountsType.SendTransfer,
                ObjectState = ObjectState.Added
            });

            child9.Childeren.Add(new AccountDocumentCoding
            {
                AccountCode = "1002",
                Name = "حساب معین اموال فروخته شده",
                TotalAccountType = AccountingDescrtiption.Send,
                CertainAccountType = CertainAccountsType.SendSold,
                ObjectState = ObjectState.Added
            });

            child9.Childeren.Add(new AccountDocumentCoding
            {
                AccountCode = "1003",
                Name = "حساب معین اموال حذفی",
                TotalAccountType = AccountingDescrtiption.Send,
                CertainAccountType = CertainAccountsType.SendDelete,
                ObjectState = ObjectState.Added
            });
            parent.Childeren.Add(child9);

            return parent;
        }

        private void openHelpDoc()
        {
            //var viewModel = new HelpViewModel(HelpPageingAddress.Default.HelpFileName, HelpPageingAddress.Default.mainWinHelpPage);
            //_navigationService.ShowHelpWindow(viewModel);
            Mouse.SetCursor(Cursors.Wait);

            var viewModel = new ReportViewModel();
            viewModel.showGlobalSinglePageHelp("110002");
            _navigationService.ShowReportViewWindow(viewModel);
            Mouse.SetCursor(Cursors.Arrow);
        }

        private void reportDetails()
        {
            Mouse.SetCursor(Cursors.Wait);

            var viewModel = new ReportViewModel();
            viewModel.EmployeeDetailsReport();
            _navigationService.ShowReportViewWindow(viewModel);

            Mouse.SetCursor(Cursors.Arrow);

        }

        #endregion

        #region commands

        public ICommand IdentifyCommand { get; private set; }
        public ICommand SaveCommand { get; private set; }
        public ICommand HelpCommand { get; private set; }
        public ICommand ReportCommand { get; private set; }
        private void initalizCommand()
        {
            IdentifyCommand = new MvvmCommand(
                (parameter) => { this.PerformIdentityAccuracy(); },
                (parameter) => { return IdentificationCode.Length == 16; }
                ).AddListener<EmployeeViewModel>(this, x => x.IdentificationCode);

            SaveCommand = new MvvmCommand(
                (parameter) => { this.saveEmployee(); },
                (parameter) => { return true; }
                );

            HelpCommand = new MvvmCommand(
                 (parameter) => { this.openHelpDoc(); },
                 (parameter) => { return true; }
                );

            ReportCommand = new MvvmCommand(
                (parameter) => { this.reportDetails(); },
                 (parameter) => { return true; }
                );
        }

        #endregion

        #region fields

        private readonly IUnityContainer _container;
        private readonly IDialogService _dialogService;
        private readonly INavigationService _navigationService;
        private readonly IUnitOfWorkAsync _unitOfWork;
        private readonly IEmployeeService _employeeService;
        private Employee _currentEmployee;
        private SeedDataHelper _helper;
        private readonly HttpClient _client;
        private XDocument _xdocuments;
        private CustomerModel _organ;

        #endregion
    }
}
