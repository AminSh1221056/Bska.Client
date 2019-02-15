
namespace Bska.Client.UI.ViewModels
{
    using Bska.Client.Common;
    using Bska.Client.Data.Service;
    using Bska.Client.UI.API;
    using Bska.Client.UI.Services;
    using Bska.Client.Domain.Entity.AssetEntity;
    using Bska.Client.API.Infrastructure;
    using Bska.Client.API.UnitOfWork;
    using Bska.Client.UI.Helper;
    using Bska.Client.Domain.Entity.StoredProcedures;
    using Bska.Client.Repository.Model;
    using Microsoft.Practices.Unity;
    using System;
    using System.Threading.Tasks;
    using System.Windows.Input;
    using System.Linq;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Threading;
    using Domain.Entity;
    using System.Windows;
    using Newtonsoft.Json;
    using Bska.Client.UI.Helper.SendingModels;

    public sealed class AssetSendViewModel : BaseViewModel
    {
        #region ctor

        public AssetSendViewModel(IUnityContainer container, Window window)
        {
            this._container = container;
            this._unitOfWork = _container.Resolve<IUnitOfWorkAsync>();
            this._dialogService = _container.Resolve<IDialogService>();
            this._movableAssetService = _container.Resolve<IMovableAssetService>(new ParameterOverride("repository", _unitOfWork.Repository<MovableAsset>()));
            this._commodityService = _container.Resolve<IMAssetCommodityService>(new ParameterOverride("repository", _unitOfWork.Repository<Commodity>()));
            this._storeBillService = _container.Resolve<IStoreBillService>();
            this._beskaProcedures = _container.Resolve<IBskaStoredProcedures>();
            this._sellerService = _container.Resolve<ISellerService>();
            this._client = UserLog.UniqueInstance.Client;
            this.IdentificationCode = "";
            this.SendingIsActive = true;
            this.Window = window;
            this.initalizObj();
            this.initalizCommand();
        }

        #endregion

        #region properties
        public Window Window
        {
            get;
            set;
        }
        public String IdentificationCode
        {
            get { return GetValue(() => IdentificationCode); }
            set
            {
                SetValue(() => IdentificationCode, value);
            }
        }

        public Double Report
        {
            get { return GetValue(() => Report); }
            set
            {
                SetValue(() => Report, value);
            }
        }

        public String ReportString
        {
            get { return GetValue(() => ReportString); }
            set
            {
                SetValue(() => ReportString, value);
            }
        }
        
        public Double Report3
        {
            get { return GetValue(() => Report3); }
            set
            {
                SetValue(() => Report3, value);
            }
        }
        
        public string ReportString3
        {
            get { return GetValue(() => ReportString3); }
            set
            {
                SetValue(() => ReportString3, value);
            }
        }
        
        public Boolean SendingIsActive
        {
            get { return GetValue(() => SendingIsActive); }
            set
            {
                SetValue(() => SendingIsActive, value);
            }
        }

        public string AutheniticationInfo
        {
            get { return GetValue(() => AutheniticationInfo); }
            set
            {
                SetValue(() => AutheniticationInfo, value);
            }
        }

        public CustomerModel Customer
        {
            get { return GetValue(() => Customer); }
            set
            {
                SetValue(() => Customer, value);
            }
        }

        public Boolean IsUnconsuptionSending
        {
            get { return GetValue(() => IsUnconsuptionSending); }
            set
            {
                SetValue(() => IsUnconsuptionSending, value);
            }
        }
        
        public Boolean IsCommoditySending
        {
            get { return GetValue(() => IsCommoditySending); }
            set
            {
                SetValue(() => IsCommoditySending, value);
            }
        }
        
        public Boolean IsDeleteAble
        {
            get { return GetValue(() => IsDeleteAble); }
            set
            {
                SetValue(() => IsDeleteAble, value);
            }
        }

        #endregion

        #region methods
        private void initalizObj()
        {
            if (UserLog.UniqueInstance.LogedEmployee != null)
            {
                var dtnDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(APPSettings.Default.employeeCer);
                if (dtnDictionary != null)
                {
                    if (dtnDictionary.ContainsKey(UserLog.UniqueInstance.LogedEmployee.BudgetNo.ToString()))
                    {
                        IdentificationCode = GlobalClass.DecryptStringAES(dtnDictionary[UserLog.UniqueInstance.LogedEmployee.BudgetNo.ToString()], "66Ak679Du4V3qo92");
                    }
                }
            }

            if (_movableAssetService.Queryable().Any(ma => ma.ISConfirmed))
            {
                this.IsDeleteAble = true;
            }
        }
        
        private void ConfirmAssets()
        {
            bool confirm = _dialogService.AskConfirmation("هشدار", "تمام اموال ثبت نهایی شده و دیگر قابل ویرایش نیست.همچنین دیگر قادر به ثبت اموال از طریق اطلاعات اولیه نمی باشید.آیا می خواهید ادامه دهید");
            if (confirm)
            {
                Mouse.SetCursor(Cursors.Wait);
                try
                {
                    int ok = _beskaProcedures.MAssetUpdate_Confirmed();
                    if (ok > 0)
                    {
                        UserLog.UniqueInstance.AddLog(new EventLog
                        {
                            EntryDate = GlobalClass._Today,
                            Key = UserLog.UniqueInstance.LogedUser.FullName,
                            Message = "تایید نهایی اموال ",
                            ObjectState = ObjectState.Added,
                            Type = EventType.Update,
                            UserId = UserLog.UniqueInstance.LogedUser.UserId
                        });
                    }
                    APPSettings.Default.IsCompletedAssets = false; //true in production;
                    APPSettings.Default.Save();
                    APPSettings.Default.Reload();
                    _dialogService.ShowInfo("توجه", ErrorMessages.Default.SuccessfullyMessage);
                    IsDeleteAble = true;
                }
                catch (Exception)
                {
                    throw;
                }
                Mouse.SetCursor(Cursors.Arrow);
            }
        }

        private async void PerformIdentityAccuracy()
        {
            if (GlobalClass.CheckForInternetConnection(Settings.Default.ServerUrl))
            {
                _dialogService.ShowAlert("توجه", ErrorMessages.Default.NoInternetAccess);
                return;
            }

            if (IdentificationCode.Length != 16)
            {
                _dialogService.ShowAlert("توجه", "کد شناسایی نامعتبر است");
                return;
            }

            Mouse.SetCursor(Cursors.Wait);
            try
            {
                string path = $"Identification/{IdentificationCode}";
                this.AutheniticationInfo = "در حال شناسایی...";
                Customer = await this.GetCustomerAsync(path);
                if (Customer != null)
                {
                    this.AutheniticationInfo = "هویت این سازمان شناسایی شد";
                }
                else
                {
                    _dialogService.ShowAlert("توجه", "هیچ سازمانی یافت نشد");
                    this.AutheniticationInfo = "هویت این سازمان شناخته شده نیست";
                }
            }
            catch (TimeoutException)
            {
                _dialogService.ShowError("خطا", "هیچ پاسخی از سیستم مرکزی دریافت نشد");
            }
            catch (Exception ex)
            {
                throw ex;
            }

            Mouse.SetCursor(Cursors.Arrow);
        }

        private async void compeitionAssets()
        {
            if (this.Customer == null)
            {
                _dialogService.ShowAlert("توجه", "هویت ناشناخته است");
                return;
            }
            var sellers = _sellerService.Queryable().ToList();
            List<StoreBillExchangeModel> storeBillExchange = new List<StoreBillExchangeModel>() ;
            var bills = _storeBillService.Query()
               .Include(sb => sb.MAssets).Include(sb => sb.Store).Select();
            if (bills.Count() <= 0)
            {
                _dialogService.ShowAlert("توجه", "هیچ مالی یافت نشد");
                return;
            }
            bills.ForEach(sb =>
               {
                   var item = new StoreBillExchangeModel
                   {
                       AcqType = sb.AcqType,
                       ArrivalDate = sb.ArrivalDate,
                       Desc1 = sb.Desc1,
                       Desc2 = sb.Desc2,
                       Desc3 = sb.Desc3,
                       StoreBillId = string.Format("{0}{1}", Customer.OrganId, sb.StoreBillId),
                       SellerName = sb.SellerId != null ? sellers.First(s => s.SellerId == sb.SellerId).Name : null,
                       StuffType = sb.StuffType,
                       StoreName = sb.Store?.Name,
                       StoreBillNo = sb.StoreBillNo,
                   };
                   foreach (var m in sb.MAssets)
                   {
                       string parentId = null;
                       if (m is Belonging)
                       {
                           var belonging = m as Belonging;
                           var parentAsset = _movableAssetService.GetBelongingParnet(belonging.AssetId);
                           parentId = parentAsset != null ? string.Format("{0}{1}", Customer.OrganId, parentAsset.AssetId) : null;
                       }

                       item.MAssets.Add(new MovableAssetExchangeModel
                       {
                           Name = m.Name,
                           Num = m.Num,
                           Cost = m.Cost,
                           CurState = m.CurState,
                           Desc1 = m.Desc1,
                           Desc2 = m.Desc2,
                           Desc3 = m.Desc3,
                           Desc4 = m.Desc4,
                           Description = m.Description,
                           InsertDate = m.InsertDate,
                           KalaUid = m.KalaUid.ToString(),
                           Label = m.Label,
                           ModeifiedDate = m.ModeifiedDate,
                           Uid1 = m.Uid1,
                           Uid2 = m.Uid2,
                           Uid3 = m.Uid3,
                           Uid4 = m.Uid4,
                           UnitId = m.UnitId,
                           AssetId = string.Format("{0}{1}", Customer.OrganId, m.AssetId),
                           BelongingParentId = parentId,
                           Type = (int)m.StoreBill.StuffType,
                           Floor = m.Floor,
                           FloorType = m.FloorType,
                           ISCompietion = m.ISCompietion,
                           ISConfirmed = m.ISConfirmed,
                           OldLabel = m.OldLabel,
                           OrganLabel = m.OrganLabel,
                           StoreBillId = string.Format("{0}{1}", Customer.OrganId, sb.StoreBillId)
                       });
                   };

                   storeBillExchange.Add(item);
               });

            List<DocumentExchangeModel> DocumentExchange = new List<DocumentExchangeModel>();
            var documents = _movableAssetService.GetAllDocumentsToMovableAssets().ToList();

            documents.ForEach(doc =>
            {
                var item = new DocumentExchangeModel
                {
                    Desc1 = doc.Desc1,
                    Desc2 = doc.Desc2,
                    Desc3 = doc.Desc3,
                    Desc4 = doc.Desc4,
                    DocumentDate = doc.DocumentDate,
                    DocumentId = string.Format("{0}{1}", Customer.OrganId, doc.DocumentId),
                    DocumentType = doc.DocumentType,
                    StoreName = doc.Store?.Name,
                    Transferee = doc.Transferee
                };
                foreach (var m in doc.MovableAsset)
                {
                    item.MAssets.Add(new PortableAssetDocumentExchangeModel
                    {
                        DocumentId = string.Format("{0}{1}", Customer.OrganId, doc.DocumentId),
                        AssetId = string.Format("{0}{1}", Customer.OrganId, m.AssetId),
                    });
                };

                DocumentExchange.Add(item);
            });

            var sendItem = new GlobalExchangeModel<StoreBillExchangeModel,DocumentExchangeModel>(storeBillExchange, DocumentExchange);
            try
            {
                ReportString = "ارسال اموال غیرمصرفی";
                this.IsUnconsuptionSending = true;
                var progressIndicator = new Progress<int>(ReportProgress);
                _cts = new CancellationTokenSource();
                await CreateCustomerAssetAsync(sendItem, $"PortableAssetExchange/bySendStoreBill/{IdentificationCode}", progressIndicator, _cts, _cts.Token);
            }
            catch (OperationCanceledException ex)
            {
                _dialogService.ShowAlert("error", ex.Message);
                Report = 0;
                ReportString = "";
                IsUnconsuptionSending = false;
            }
            catch (HttpRequestException ex)
            {
                _dialogService.ShowError("خطا در بروز رسانی اطلاعات", ex.Message + ".برای اطلاعات بیشتر به آدرس زیر مراجعه کنید" + ex.HelpLink);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        
        private async void SendCommodityAssets(StuffType type)
        {
            if (this.Customer == null)
            {
                _dialogService.ShowAlert("توجه", "هویت ناشناخته است");
                return;
            }

            if (this.Customer == null)
            {
                _dialogService.ShowAlert("توجه", "هویت ناشناخته است");
                return;
            }
            var sellers = _sellerService.Queryable().ToList();
            List<StoreBillExchangeModel> storeBillExchange = new List<StoreBillExchangeModel>();
            var bills = _storeBillService.Query(sb => sb.StuffType == type)
               .Include(sb => sb.Commodities).Include(sb => sb.Store).Select();
            if (bills.Count() <= 0)
            {
                _dialogService.ShowAlert("توجه", "هیچ مالی یافت نشد");
                return;
            }
            bills.ForEach(sb =>
            {
                var item = new StoreBillExchangeModel
                {
                    AcqType = sb.AcqType,
                    ArrivalDate = sb.ArrivalDate,
                    Desc1 = sb.Desc1,
                    Desc2 = sb.Desc2,
                    Desc3 = sb.Desc3,
                    StoreBillId = string.Format("{0}{1}", Customer.OrganId, sb.StoreBillId),
                    SellerName = sb.SellerId != null ? sellers.First(s => s.SellerId == sb.SellerId).Name : null,
                    StuffType = sb.StuffType,
                    StoreName = sb.Store?.Name,
                    StoreBillNo = sb.StoreBillNo,
                };

                foreach (var m in sb.Commodities)
                {
                    string parentId = null;
                    if (m.UnConsuptionId.HasValue)
                    {
                        parentId =string.Format("{0}{1}", Customer.OrganId, m.UnConsuptionId);
                    }

                    item.MAssets.Add(new MovableAssetExchangeModel
                    {
                        Name = m.Name,
                        Num = m.Num,
                        Cost = m.Cost,
                        Desc1 = m.Country,
                        Desc2 = m.Company,
                        Desc3 = m.BatchNumber,
                        Description = m.Description,
                        InsertDate = m.InsertDate,
                        KalaUid = m.KalaUid.ToString(),
                        ModeifiedDate = m.ModeifiedDate,
                        UnitId = m.UnitId,
                        AssetId = string.Format("{0}{1}", Customer.OrganId, m.AssetId),
                        BelongingParentId = parentId,
                        StoreBillId = string.Format("{0}{1}", Customer.OrganId, sb.StoreBillId),
                        DateOfBirth=m.DateOfBirth,
                        ExpirationDate=m.ExpirationDate,
                    });
                };

                storeBillExchange.Add(item);
            });

            List<DocumentExchangeModel> DocumentExchange = new List<DocumentExchangeModel>();
            var documents = _commodityService.GetAllDocumentsToCommodity().ToList();

            documents.ForEach(doc =>
            {
                var item = new DocumentExchangeModel
                {
                    Desc1 = doc.Desc1,
                    Desc2 = doc.Desc2,
                    Desc3 = doc.Desc3,
                    Desc4 = doc.Desc4,
                    DocumentDate = doc.DocumentDate,
                    DocumentId = string.Format("{0}{1}", Customer.OrganId, doc.DocumentId),
                    DocumentType = doc.DocumentType,
                    StoreName = doc.Store?.Name,
                    Transferee = doc.Transferee
                };
                foreach (var m in doc.Commodities)
                {
                    string s1 = string.Format("{0}{1}", Customer.OrganId, doc.DocumentId);
                    string s2 = string.Format("{0}{1}", Customer.OrganId, m.CommodityId);
                    if (!item.MAssets.Any(pl=>pl.AssetId==s2 && pl.DocumentId == s1))
                    {
                        item.MAssets.Add(new PortableAssetDocumentExchangeModel
                        {
                            DocumentId = s1,
                            AssetId = s2,
                        });
                    }
                };

                DocumentExchange.Add(item);
            });

            var sendItem = new GlobalExchangeModel<StoreBillExchangeModel, DocumentExchangeModel>(storeBillExchange, DocumentExchange);

            Mouse.SetCursor(Cursors.Wait);
            try
            {
                ReportString3 = "ارسال اموال مصرفی";
                this.IsCommoditySending = true;
                var progressIndicator = new Progress<int>(ReportProgress2);
                _cts3 = new CancellationTokenSource();
                await CreateCustomerAssetAsync(sendItem, $"PortableAssetExchange/bySendCommodityStoreBill/{IdentificationCode}", progressIndicator, _cts3, _cts3.Token);
            }
            catch (OperationCanceledException ex)
            {
                _dialogService.ShowAlert("error", ex.Message);
                Report3 = 0;
                ReportString3 = "";
                IsCommoditySending = false;
            }
            catch (HttpRequestException ex)
            {
                _dialogService.ShowError("خطا در بروز رسانی اطلاعات", ex.Message + ".برای اطلاعات بیشتر به آدرس زیر مراجعه کنید" + ex.HelpLink);
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
        
        private async Task<Uri> CreateCustomerAssetAsync<T,TM>(GlobalExchangeModel<T,TM> model, string uri, IProgress<int> progress, CancellationTokenSource cts, CancellationToken ct)
        {
            int counter = 1;
            HttpResponseMessage response = null;
            response = await _client.PutAsJsonAsync(uri, model);
            response.EnsureSuccessStatusCode();

            if (progress != null)
            {
                progress.Report(100);
            }
            ct.ThrowIfCancellationRequested();
            counter++;
            return response?.Headers.Location;
        }
        
        void ReportProgress(int value)
        {
            Report = value;
            if (value == 100)
            {
                _dialogService.ShowInfo("توجه", "عملیات ارسال با موفقیت انجام شد");
                ReportString = "";
                UserLog.UniqueInstance.AddLog(new EventLog
                {
                    EntryDate = GlobalClass._Today,
                    Key = UserLog.UniqueInstance.LogedUser.FullName,
                    Message = ReportString,
                    ObjectState = ObjectState.Added,
                    Type = EventType.Update,
                    UserId = UserLog.UniqueInstance.LogedUser.UserId
                });
                this.IsUnconsuptionSending = false;
            }
        }
        
        void ReportProgress2(int value)
        {
            Report3 = value;
            if (value == 100)
            {
                _dialogService.ShowInfo("توجه", "عملیات ارسال  با موفقیت انجام شد");
                ReportString3 = "";
                UserLog.UniqueInstance.AddLog(new EventLog
                {
                    EntryDate = GlobalClass._Today,
                    Key = UserLog.UniqueInstance.LogedUser.FullName,
                    Message = ReportString3,
                    ObjectState = ObjectState.Added,
                    Type = EventType.Update,
                    UserId = UserLog.UniqueInstance.LogedUser.UserId
                });
                this.IsCommoditySending = false;
            }
        }
        
        private void deleteAllAsset()
        {
            bool confirm = _dialogService.AskConfirmation("هشدار", "تمام اموال ثبت شده،قبض انبار ها،حواله های انبار،سند های حسابداری پاک شده و از ابتدا باید ثبت شوند.آیا می خواهید ادامه دهید");
            if (confirm)
            {
                Mouse.SetCursor(Cursors.Wait);
                try
                {
                    int ok = _beskaProcedures.DeleteAll_Assets();
                    if (ok > 0)
                    {
                        UserLog.UniqueInstance.AddLog(new EventLog
                        {
                            EntryDate = GlobalClass._Today,
                            Key = UserLog.UniqueInstance.LogedUser.FullName,
                            Message = "حذف تمام اطلاعات اموال تا قبل از تایید نهایی",
                            ObjectState = ObjectState.Added,
                            Type = EventType.Update,
                            UserId = UserLog.UniqueInstance.LogedUser.UserId
                        });
                        _dialogService.ShowInfo("توجه", ErrorMessages.Default.SuccessfullyMessage);
                    }
                }
                catch (Exception)
                {
                    throw;
                }
                Mouse.SetCursor(Cursors.Arrow);
            }
        }
        #endregion

        #region commands

        public ICommand IdentifyCommand { get; private set; }
        public ICommand SendNonConsumCommand { get; private set; }
        public ICommand ConfirmAssetCommand { get; private set; }
        public ICommand SendCommodityCommand { get; private set; }
        public ICommand DeleteAssetCommand { get; private set; }
        private void initalizCommand()
        {
            ConfirmAssetCommand = new MvvmCommand(
                 (parameter) => { this.ConfirmAssets(); },
               (parameter) => { return true; }
                );

            IdentifyCommand = new MvvmCommand(
               (parameter) =>
               {
                   this.PerformIdentityAccuracy();
               },
               (parameter) =>
               {
                   return IdentificationCode.Length == 16;
               }
              ).AddListener<AssetSendViewModel>(this, x => x.IdentificationCode);

            SendNonConsumCommand = new MvvmCommand(
              (parameter) =>
              {
                  this.compeitionAssets();
              },
              (parameter) =>
              {
                  return !this.IsUnconsuptionSending;
              }
              ).AddListener<AssetSendViewModel>(this, x => x.IsUnconsuptionSending);
            
            SendCommodityCommand = new MvvmCommand(
                (parameter) => { this.SendCommodityAssets(StuffType.Consumable); },
                (parameter) => { return !this.IsCommoditySending; }
                ).AddListener<AssetSendViewModel>(this, x => x.IsCommoditySending);
            
            DeleteAssetCommand = new MvvmCommand(
                (parameter) => { this.deleteAllAsset(); },
                (parameter) => { return true; }//!IsDeleteAble; }
                );//.AddListener<AssetSendViewModel>(this, x => x.IsDeleteAble);
        }

        #endregion

        #region fields

        private readonly IUnityContainer _container;
        private readonly IDialogService _dialogService;
        private readonly IMovableAssetService _movableAssetService;
        private readonly IMAssetCommodityService _commodityService;
        private readonly IStoreBillService _storeBillService;
        private readonly IUnitOfWorkAsync _unitOfWork;
        private readonly IBskaStoredProcedures _beskaProcedures;
        private readonly ISellerService _sellerService;
        private readonly HttpClient _client;
        private CancellationTokenSource _cts;
        private CancellationTokenSource _cts3;

        #endregion
    }
}
