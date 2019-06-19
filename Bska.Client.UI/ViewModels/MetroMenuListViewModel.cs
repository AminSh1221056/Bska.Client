
namespace Bska.Client.UI.ViewModels
{
    using Bska.Client.UI.Helper;
    using System.Windows.Controls;
    using System.Windows.Media;
    using System.Collections.Generic;

    public class MetroMenuListViewModel
    {
        private Dictionary<string, List<MetroMenuItem>> _menu;

        private Dictionary<string,List<MenuItem>> _menuItems;

        public Dictionary<string, List<MetroMenuItem>> Menu
        {
            get { return _menu; }
        }
        public Dictionary<string, List<MenuItem>> MenuItems
        {
            get { return _menuItems; }
        }

        public MetroMenuListViewModel(string title)
        {
            _menu = new Dictionary<string, List<MetroMenuItem>>();
            _menuItems = new Dictionary<string, List<MenuItem>>();
            _generate(title);
        }

        private void _generate(string title)
        {
            if(string.Equals(title,"stuffHonest"))
            {
                var lstStuffHonestMenu = new List<MenuItem>();
                var lstStuffHonestMetroMenu = new List<MetroMenuItem>();
                if (!APPSettings.Default.IsCompletedAssets)
                {
                    if (UserLog.UniqueInstance.LogedUser.UserAttribute.Atttribute1)
                    {
                        lstStuffHonestMenu.Add(new MenuItem { Name = "A9", Header = "آپلود فایل اکسس اموال", Tag = 1009 });
                    }

                    if (UserLog.UniqueInstance.LogedUser.UserAttribute.Atttribute2)
                    {
                        lstStuffHonestMenu.Add(new MenuItem { Name = "A1", Header = "ثبت موجودی اولیه", Tag = 1001 });
                    }
                }

                if (UserLog.UniqueInstance.LogedUser.UserAttribute.Atttribute4)
                {
                    lstStuffHonestMenu.Add(new MenuItem { Name = "A2", Header = "اموال", Tag = 1003 });
                }

                if (UserLog.UniqueInstance.LogedUser.UserAttribute.Atttribute5)
                {
                    lstStuffHonestMenu.Add(new MenuItem { Name = "A3", Header = "درخواست ها", Tag = 1004 });
                }

                if (UserLog.UniqueInstance.LogedUser.UserAttribute.Atttribute6)
                {
                    lstStuffHonestMenu.Add(new MenuItem { Name = "A4", Header = "صورت جلسات", Tag = 1005 });
                }
                
                _menuItems.Add("stuffHonest", lstStuffHonestMenu);

                var lstorderMetroMenu = new List<MetroMenuItem>();
                var lstOrder = new List<MenuItem>();
                this.setOrderMenus(lstOrder, lstorderMetroMenu);

                _menuItems.Add("order", lstOrder);
            }
            else if(string.Equals(title, "store"))
            {
                var lstStoreMetroMenu = new List<MetroMenuItem>();
                var lstStore = new List<MenuItem>();

                if (UserLog.UniqueInstance.LogedUser.UserAttribute.Atttribute1)
                {
                    lstStoreMetroMenu.Add(new MetroMenuItem("طراحی انبار", MetroMenuResources.Logo.StoreDesign, "A1"));
                    lstStore.Add(new MenuItem { Header = "طراحی انبار", Tag = 1002, Name = "ENTREPOTmAnageMenuItem1" });
                }

                if (UserLog.UniqueInstance.LogedUser.UserAttribute.Atttribute3)
                {
                    lstStoreMetroMenu.Add(new MetroMenuItem("بررسی اموال", MetroMenuResources.Logo.StoreAsset, "A2"));
                    lstStore.Add(new MenuItem { Header = "بررسی اموال", Tag = 1001, Name = "ENTREPOTmAnageMenuItem3" });
                }

                if (UserLog.UniqueInstance.LogedUser.UserAttribute.Atttribute4)
                {
                    lstStoreMetroMenu.Add(new MetroMenuItem("صدور حواله انبار داخلی", MetroMenuResources.Logo.StoreInternalDraft, "A3"));
                    lstStore.Add(new MenuItem { Header = "صدور حواله انبار داخلی", Tag = 1005, Name = "ENTREPOTmAnageMenuItem4" });
                }

                if (UserLog.UniqueInstance.LogedUser.UserAttribute.Atttribute5)
                {
                    lstStoreMetroMenu.Add(new MetroMenuItem("قبض و حواله انبار خرید", MetroMenuResources.Logo.StoreBuyDocs, "A4"));
                    lstStore.Add(new MenuItem { Header = "قبض و حواله انبار خرید", Tag = 1006, Name = "ENTREPOTmAnageMenuItem5" });
                }

                if (UserLog.UniqueInstance.LogedUser.UserAttribute.Atttribute6)
                {
                    lstStoreMetroMenu.Add(new MetroMenuItem("صدور قبض انبار مستقیم", MetroMenuResources.Logo.StoreDirectDocs, "A5"));
                    lstStore.Add(new MenuItem { Header = "صدور قبض انبار مستقیم", Tag = 1003, Name = "ENTREPOTmAnageMenuItem6" });
                }

                if (UserLog.UniqueInstance.LogedUser.UserAttribute.Atttribute7)
                {
                    lstStoreMetroMenu.Add(new MetroMenuItem("بررسی قبض انبارها", MetroMenuResources.Logo.StoreRecipetCheck, "A6"));
                    lstStore.Add(new MenuItem { Header = "بررسی قبض انبارها", Tag = 1010, Name = "ENTREPOTmAnageMenuItem7" });
                }

                if (UserLog.UniqueInstance.LogedUser.UserAttribute.Atttribute8)
                {
                    lstStoreMetroMenu.Add(new MetroMenuItem("بررسی اسناد انبارها", MetroMenuResources.Logo.StoreDraftCheck, "A7"));
                    lstStore.Add(new MenuItem { Header = "بررسی اسناد انبارها", Tag = 1011, Name = "ENTREPOTmAnageMenuItem8" });
                }

                if (UserLog.UniqueInstance.LogedUser.UserAttribute.Atttribute9)
                {
                    lstStoreMetroMenu.Add(new MetroMenuItem("ثبت درخواست", MetroMenuResources.Logo.NewOrder, "A8"));
                    lstStore.Add(new MenuItem { Header = "ثبت درخواست", Tag = 1008, Name = "ENTREPOTmAnageMenuItem9" });

                }

                if (UserLog.UniqueInstance.LogedUser.UserAttribute.Atttribute10)
                {
                    lstStoreMetroMenu.Add(new MetroMenuItem("بررسی درخواست ها", MetroMenuResources.Logo.OuterAssetCheck, "A9"));
                    lstStore.Add(new MenuItem { Header = "بررسی درخواست ها", Tag = 1009, Name = "ENTREPOTmAnageMenuItem10" });
                }
                if (UserLog.UniqueInstance.LogedUser.UserAttribute.Atttribute11)
                {
                    lstStoreMetroMenu.Add(new MetroMenuItem("صورت جلسات خروج", MetroMenuResources.Logo.ProcedingManag, "A10"));
                    lstStore.Add(new MenuItem { Header = "صورت جلسات خروج", Tag = 1007, Name = "ENTREPOTmAnageMenuItem11" });
                }

                _menuItems.Add("store", lstStore);
                _menu.Add("store", lstStoreMetroMenu);

                var lstorderMetroMenu = new List<MetroMenuItem>();
                var lstOrder = new List<MenuItem>();
                this.setOrderMenus(lstOrder, lstorderMetroMenu);

                _menuItems.Add("order", lstOrder);
                _menu.Add("order", lstorderMetroMenu);
            }
            else if (string.Equals(title, "generalmanager"))
            {
                var lstgManager = new List<MenuItem>();
                if (UserLog.UniqueInstance.LogedUser.UserAttribute.Atttribute1)
                {
                    lstgManager.Add(new MenuItem { Header = "درخواست ها", Tag = 1003, Name = "A1" });
                }
                if (UserLog.UniqueInstance.LogedUser.UserAttribute.Atttribute2)
                {
                    lstgManager.Add(new MenuItem { Header = "صورت جلسات", Tag = 1001, Name = "A2" });
                }

                _menuItems.Add("gManager", lstgManager);

                var lstorderMetroMenu = new List<MetroMenuItem>();
                var lstOrder = new List<MenuItem>();
                this.setOrderMenus(lstOrder, lstorderMetroMenu);

                _menuItems.Add("order", lstOrder);
                _menu.Add("order", lstorderMetroMenu);
            }
            else if (string.Equals(title, "order"))
            {
                var lstorderMetroMenu = new List<MetroMenuItem>();
                var lstOrder = new List<MenuItem>();
                this.setOrderMenus(lstOrder, lstorderMetroMenu);

                _menuItems.Add("order", lstOrder);
                _menu.Add("order", lstorderMetroMenu);
            }
            else if (string.Equals(title, "munition"))
            {
                var lstMunitionMetroMenu = new List<MetroMenuItem>();
                var lstMunition = new List<MenuItem>();

                if (UserLog.UniqueInstance.LogedUser.UserAttribute.Atttribute1)
                {
                    lstMunitionMetroMenu.Add(new MetroMenuItem("فروشنده ها", MetroMenuResources.Logo.SalesMan, "A1"));
                    lstMunition.Add(new MenuItem { Header = "فروشنده ها", Tag = 1004, Name = "sellerManageMenuItem" });
                }

                if (UserLog.UniqueInstance.LogedUser.UserAttribute.Atttribute2)
                {
                    lstMunitionMetroMenu.Add(new MetroMenuItem("بررسی سفارش ها", MetroMenuResources.Logo.OuterAssetCheck, "A2"));
                    lstMunition.Add(new MenuItem { Header = "بررسی سفارش ها", Tag = 1001, Name = "munitionManageMenuItem1" });
                }

                if (UserLog.UniqueInstance.LogedUser.UserAttribute.Atttribute3)
                {
                    lstMunitionMetroMenu.Add(new MetroMenuItem("هزینه اموال نامشهود", MetroMenuResources.Logo.AssetCost, "A3"));
                    lstMunition.Add(new MenuItem { Header = "هزینه های اموال نامشهود", Tag = 1002, Name = "munitionManageMenuItem2" });
                }
                if (UserLog.UniqueInstance.LogedUser.UserAttribute.Atttribute4)
                {
                    lstMunitionMetroMenu.Add(new MetroMenuItem("کارپردازان", MetroMenuResources.Logo.Supplier, "A4"));
                    lstMunition.Add(new MenuItem { Header = "کارپردازان", Tag = 1003, Name = "supplierMenuItem1" });
                }

                if (UserLog.UniqueInstance.LogedUser.UserAttribute.Atttribute5)
                {
                    lstMunitionMetroMenu.Add(new MetroMenuItem("درخواست های انبار", MetroMenuResources.Logo.InternalOrder, "A5"));
                    lstMunition.Add(new MenuItem { Header = "درخواست های انبار", Tag = 1005, Name = "munitionManageMenuItem3" });
                }

                if (UserLog.UniqueInstance.LogedUser.UserAttribute.Atttribute6)
                {
                    lstMunitionMetroMenu.Add(new MetroMenuItem("عودت سفارش های خرید نشده", MetroMenuResources.Logo.NewOrder, "A6"));
                    lstMunition.Add(new MenuItem { Header = "عودت سفارش های خرید نشده", Tag = 1006, Name = "munitionManageMenuItem4" });
                }

                if (UserLog.UniqueInstance.LogedUser.UserAttribute.Atttribute7)
                {
                    lstMunitionMetroMenu.Add(new MetroMenuItem("سفارش ها برای مناقصه", MetroMenuResources.Logo.StoreBuyDocs, "A7"));
                    lstMunition.Add(new MenuItem { Header = "سفارش ها برای مناقصه", Tag = 1007, Name = "munitionManageMenuItem5" });
                }

                if (UserLog.UniqueInstance.LogedUser.UserAttribute.Atttribute8)
                {
                    lstMunitionMetroMenu.Add(new MetroMenuItem("سفارش های مناقصه برای کارپرداز", MetroMenuResources.Logo.StoreBuyDocs, "A8"));
                    lstMunition.Add(new MenuItem { Header = "سفارش های مناقصه برای کارپرداز", Tag = 1008, Name = "munitionManageMenuItem6" });
                }

                _menuItems.Add("munition", lstMunition);
                _menu.Add("munition", lstMunitionMetroMenu);

                var lstorderMetroMenu = new List<MetroMenuItem>();
                var lstOrder = new List<MenuItem>();
                this.setOrderMenus(lstOrder, lstorderMetroMenu);

                _menuItems.Add("order", lstOrder);
                _menu.Add("order", lstorderMetroMenu);
            }
            else if (string.Equals(title, "accounting"))
            {
                var lstAccountingMetroMenu = new List<MetroMenuItem>();
                var lstAccounting = new List<MenuItem>();

                if (UserLog.UniqueInstance.LogedUser.UserAttribute.Atttribute1)
                {
                    lstAccounting.Add(new MenuItem { Header = "کدینگ اسناد", Tag = 1001, Name = "A1" });
                }

                if (UserLog.UniqueInstance.LogedUser.UserAttribute.Atttribute2)
                {
                    lstAccounting.Add(new MenuItem { Header = "اسناد", Tag = 1002, Name = "A2" });
                }

                _menuItems.Add("accounting", lstAccounting);
                _menu.Add("accounting", lstAccountingMetroMenu);

                var lstorderMetroMenu = new List<MetroMenuItem>();
                var lstOrder = new List<MenuItem>();
                this.setOrderMenus(lstOrder, lstorderMetroMenu);

                _menuItems.Add("order", lstOrder);
                _menu.Add("order", lstorderMetroMenu);
            }
            else if (string.Equals(title, "supplier"))
            {
                var lstSupplerMetroMenu = new List<MetroMenuItem>();
                var lstsuppliers = new List<MenuItem>();

                if (UserLog.UniqueInstance.LogedUser.UserAttribute.Atttribute1)
                {
                    lstSupplerMetroMenu.Add(new MetroMenuItem("بررسی سفارش ها", MetroMenuResources.Logo.Supplier, "A4"));
                    lstsuppliers.Add(new MenuItem { Header = "بررسی سفارش ها", Tag = 1003, Name = "supplierMenuItem1" });
                }

                if (UserLog.UniqueInstance.LogedUser.UserAttribute.Atttribute2)
                {
                    lstSupplerMetroMenu.Add(new MetroMenuItem("سفارش های مناقصه", MetroMenuResources.Logo.StoreBuyDocs, "A8"));
                    lstsuppliers.Add(new MenuItem { Header = "سفارش های مناقصه", Tag = 1008, Name = "munitionManageMenuItem6" });
                }

                _menuItems.Add("supplier", lstsuppliers);
                _menu.Add("supplier", lstSupplerMetroMenu);

                var lstorderMetroMenu = new List<MetroMenuItem>();
                var lstOrder = new List<MenuItem>();
                this.setOrderMenus(lstOrder, lstorderMetroMenu);

                _menuItems.Add("order", lstOrder);
                _menu.Add("order", lstorderMetroMenu);
            }
            else if (string.Equals("manager", title))
            {
                var lstmanagerMetroMenu = new List<MenuItem>();
                lstmanagerMetroMenu.Add(new MenuItem { Name = "managerMenuItem1", Header = "تنظیمات اولیه", Tag = 1001 });
                lstmanagerMetroMenu.Add(new MenuItem { Name = "managerMenuItem2", Header = "پرسنل", Tag = 1002 });
                lstmanagerMetroMenu.Add(new MenuItem { Name = "managerMenuItem3", Header = "کاربری", Tag = 1003 });
                lstmanagerMetroMenu.Add(new MenuItem { Name = "managerMenuItem6", Header = "اموال", Tag = 1006 });
                lstmanagerMetroMenu.Add(new MenuItem { Name = "managerMenuItem4", Header = "بروز رسانی", Tag = 1004 });
                lstmanagerMetroMenu.Add(new MenuItem { Name = "managerMenuItem5", Header = "پایگاه داده", Tag = 1005 });
                _menuItems.Add("manager", lstmanagerMetroMenu);

                var lstStuffHonestMenu = new List<MenuItem>();
                if (!APPSettings.Default.IsCompletedAssets)
                {
                    lstStuffHonestMenu.Add(new MenuItem { Name = "A9", Header = "آپلود فایل اکسس اموال", Tag = 1009 });
                    lstStuffHonestMenu.Add(new MenuItem { Name = "A1", Header = "ثبت موجودی اولیه", Tag = 1001 });
                }

                lstStuffHonestMenu.Add(new MenuItem { Name = "A2", Header = "اموال", Tag = 1003 });
                lstStuffHonestMenu.Add(new MenuItem { Name = "A3", Header = "درخواست ها", Tag = 1004 });
                lstStuffHonestMenu.Add(new MenuItem { Name = "A4", Header = "صورت جلسات", Tag = 1005 });

                _menuItems.Add("stuffHonest", lstStuffHonestMenu);

                var lstStoreMetroMenu = new List<MetroMenuItem>();
                lstStoreMetroMenu.Add(new MetroMenuItem("طراحی انبار", MetroMenuResources.Logo.StoreDesign, "A1"));
                //lstStoreMetroMenu.Add(new MetroMenuItem("مدیریت کالا", MetroMenuResources.Logo.StoreKala, "A11"));
                lstStoreMetroMenu.Add(new MetroMenuItem("بررسی اموال", MetroMenuResources.Logo.StoreAsset, "A2"));
                lstStoreMetroMenu.Add(new MetroMenuItem("صدور حواله انبار داخلی", MetroMenuResources.Logo.StoreInternalDraft, "A3"));
                lstStoreMetroMenu.Add(new MetroMenuItem("قبض و حواله انبار خرید", MetroMenuResources.Logo.StoreBuyDocs, "A4"));
                lstStoreMetroMenu.Add(new MetroMenuItem("صدور قبض انبار مستقیم", MetroMenuResources.Logo.StoreDirectDocs, "A5"));
                lstStoreMetroMenu.Add(new MetroMenuItem("بررسی قبض انبارها", MetroMenuResources.Logo.StoreRecipetCheck, "A6"));
                lstStoreMetroMenu.Add(new MetroMenuItem("بررسی اسناد انبارها", MetroMenuResources.Logo.StoreDraftCheck, "A7"));
                lstStoreMetroMenu.Add(new MetroMenuItem("ثبت درخواست", MetroMenuResources.Logo.NewOrder, "A8"));
                lstStoreMetroMenu.Add(new MetroMenuItem("بررسی درخواست ها", MetroMenuResources.Logo.OuterAssetCheck, "A9"));
                lstStoreMetroMenu.Add(new MetroMenuItem("صورت جلسات خروج", MetroMenuResources.Logo.ProcedingManag, "A10"));

                var lstStore = new List<MenuItem>();
                lstStore.Add(new MenuItem { Header = "طراحی انبار", Tag = 1002, Name = "ENTREPOTmAnageMenuItem1" });
                //lstStore.Add(new MenuItem { Header = "مدیریت کالا", Tag = 1012, Name = "ENTREPOTmAnageMenuItem2" });
                lstStore.Add(new MenuItem { Header = "بررسی اموال", Tag = 1001, Name = "ENTREPOTmAnageMenuItem3" });
                lstStore.Add(new MenuItem { Header = "صدور حواله انبار داخلی", Tag = 1005, Name = "ENTREPOTmAnageMenuItem4" });
                lstStore.Add(new MenuItem { Header = "قبض و حواله انبار خرید", Tag = 1006, Name = "ENTREPOTmAnageMenuItem5" });
                lstStore.Add(new MenuItem { Header = "صدور قبض انبار مستقیم", Tag = 1003, Name = "ENTREPOTmAnageMenuItem6" });
                lstStore.Add(new MenuItem { Header = "بررسی قبض انبارها", Tag = 1010, Name = "ENTREPOTmAnageMenuItem7" });
                lstStore.Add(new MenuItem { Header = "بررسی اسناد انبارها", Tag = 1011, Name = "ENTREPOTmAnageMenuItem8" });
                lstStore.Add(new MenuItem { Header = "ثبت درخواست", Tag = 1008, Name = "ENTREPOTmAnageMenuItem9" });
                lstStore.Add(new MenuItem { Header = "بررسی درخواست ها", Tag = 1009, Name = "ENTREPOTmAnageMenuItem10" });
                lstStore.Add(new MenuItem { Header = "صورت جلسات خروج", Tag = 1007, Name = "ENTREPOTmAnageMenuItem11" });

                _menuItems.Add("store", lstStore);
                _menu.Add("store", lstStoreMetroMenu);
                
                var lstgManager = new List<MenuItem>();
                lstgManager.Add(new MenuItem { Header = "درخواست ها", Tag = 1003, Name = "A1" });
                lstgManager.Add(new MenuItem { Header = "صورت جلسات", Tag = 1001, Name = "A2" });
                _menuItems.Add("gManager", lstgManager);

                var lstMunitionMetroMenu = new List<MetroMenuItem>();
                lstMunitionMetroMenu.Add(new MetroMenuItem("فروشنده ها", MetroMenuResources.Logo.SalesMan, "A1"));
                lstMunitionMetroMenu.Add(new MetroMenuItem("بررسی سفارش ها", MetroMenuResources.Logo.OuterAssetCheck, "A2"));
                lstMunitionMetroMenu.Add(new MetroMenuItem("هزینه اموال نامشهود", MetroMenuResources.Logo.AssetCost, "A3"));
                lstMunitionMetroMenu.Add(new MetroMenuItem("کارپردازان", MetroMenuResources.Logo.Supplier, "A4"));
                lstMunitionMetroMenu.Add(new MetroMenuItem("درخواست های انبار", MetroMenuResources.Logo.InternalOrder, "A5"));
                lstMunitionMetroMenu.Add(new MetroMenuItem("عودت سفارش های خرید نشده", MetroMenuResources.Logo.NewOrder, "A6"));
                lstMunitionMetroMenu.Add(new MetroMenuItem("سفارش ها برای مناقصه", MetroMenuResources.Logo.StoreBuyDocs, "A7"));
                lstMunitionMetroMenu.Add(new MetroMenuItem("سفارش های مناقصه برای کارپرداز", MetroMenuResources.Logo.StoreBuyDocs, "A8"));

                var lstMunition = new List<MenuItem>();
                lstMunition.Add(new MenuItem { Header = "فروشنده ها", Tag = 1004, Name = "sellerManageMenuItem" });
                lstMunition.Add(new MenuItem { Header = "بررسی سفارش ها", Tag = 1001, Name = "munitionManageMenuItem1" });
                lstMunition.Add(new MenuItem { Header = "هزینه های اموال نامشهود", Tag = 1002, Name = "munitionManageMenuItem2" });
                lstMunition.Add(new MenuItem { Header = "کارپردازان", Tag = 1003, Name = "supplierMenuItem1" });
                lstMunition.Add(new MenuItem { Header = "درخواست های انبار", Tag = 1005, Name = "munitionManageMenuItem3" });
                lstMunition.Add(new MenuItem { Header = "عودت سفارش های خرید نشده", Tag = 1006, Name = "munitionManageMenuItem4" });
                lstMunition.Add(new MenuItem { Header = "سفارش ها برای مناقصه", Tag = 1007, Name = "munitionManageMenuItem5" });
                lstMunition.Add(new MenuItem { Header = "سفارش های مناقصه برای کارپرداز", Tag = 1008, Name = "munitionManageMenuItem6" });

                _menuItems.Add("munition", lstMunition);
                _menu.Add("munition", lstMunitionMetroMenu);

                var lstAccountingMetroMenu = new List<MetroMenuItem>();
                lstAccountingMetroMenu.Add(new MetroMenuItem("کدینگ اسناد", MetroMenuResources.Logo.DocumentCoding, "A1"));
                lstAccountingMetroMenu.Add(new MetroMenuItem("اسناد", MetroMenuResources.Logo.AssetCost, "A2"));

                var lstAccounting = new List<MenuItem>();
                lstAccounting.Add(new MenuItem { Header = "کدینگ اسناد", Tag = 1001, Name = "A1" });
                lstAccounting.Add(new MenuItem { Header = "اسناد", Tag = 1002, Name = "A2" });

                _menuItems.Add("accounting", lstAccounting);
                _menu.Add("accounting", lstAccountingMetroMenu);

                var lstorderMetroMenu = new List<MetroMenuItem>();
                var lstOrder = new List<MenuItem>();
                lstorderMetroMenu.Add(new MetroMenuItem("تنظیم درخواست جدید", MetroMenuResources.Logo.NewOrder, "A1"));
                lstorderMetroMenu.Add(new MetroMenuItem("درخواست های ارسالی", MetroMenuResources.Logo.InternalOrder, "A2"));
                lstorderMetroMenu.Add(new MetroMenuItem("بررسی درخواست ها", MetroMenuResources.Logo.InternalOrder, "A3"));
                lstorderMetroMenu.Add(new MetroMenuItem("بررسی اموال", MetroMenuResources.Logo.OuterAssetCheck, "A4"));

                lstOrder.Add(new MenuItem { Header = "تنظیم درخواست جدید", Tag = 1001, Name = "RequestMenuItem1" });
                lstOrder.Add(new MenuItem { Header = "درخواست های ارسالی", Tag = 1002, Name = "RequestMenuItem2" });
                lstOrder.Add(new MenuItem { Header = "بررسی درخواست ها", Tag = 1003, Name = "RequestMenuItem3" });
                lstOrder.Add(new MenuItem { Header = "بررسی اموال", Tag = 1004, Name = "RequestMenuItem4" });

                lstorderMetroMenu.Add(new MetroMenuItem("درخواست های مناقصه", MetroMenuResources.Logo.OuterAssetCheck, "A5"));
                lstOrder.Add(new MenuItem { Header = "درخواست های مناقصه", Tag = 1005, Name = "RequestMenuItem5" });

                _menuItems.Add("order", lstOrder);
                _menu.Add("order", lstorderMetroMenu);
            }
        }

        private void setOrderMenus(List<MenuItem> lstOrder, List<MetroMenuItem> lstorderMetroMenu)
        {
            lstorderMetroMenu.Add(new MetroMenuItem("تنظیم درخواست جدید", MetroMenuResources.Logo.NewOrder, "A1"));
            lstorderMetroMenu.Add(new MetroMenuItem("درخواست های ارسالی", MetroMenuResources.Logo.InternalOrder, "A2"));
            lstorderMetroMenu.Add(new MetroMenuItem("بررسی درخواست ها", MetroMenuResources.Logo.InternalOrder, "A3"));
            lstorderMetroMenu.Add(new MetroMenuItem("بررسی اموال", MetroMenuResources.Logo.OuterAssetCheck, "A4"));

            lstOrder.Add(new MenuItem { Header = "تنظیم درخواست جدید", Tag = 1001, Name = "RequestMenuItem1" });
            lstOrder.Add(new MenuItem { Header = "درخواست های ارسالی", Tag = 1002, Name = "RequestMenuItem2" });
            lstOrder.Add(new MenuItem { Header = "بررسی درخواست ها", Tag = 1003, Name = "RequestMenuItem3" });
            lstOrder.Add(new MenuItem { Header = "بررسی اموال", Tag = 1004, Name = "RequestMenuItem4" });

            if (UserLog.UniqueInstance.LogedUser.UserAttribute.CanEditTrenderRequest)
            {
                lstorderMetroMenu.Add(new MetroMenuItem("درخواست های مناقصه", MetroMenuResources.Logo.OuterAssetCheck, "A5"));
                lstOrder.Add(new MenuItem { Header = "درخواست های مناقصه", Tag = 1005, Name = "RequestMenuItem5" });
            }
        }
    }

    public class MetroMenuItem
    {
        private Geometry _image;
        private string _title;
        private string _id;
        public string Title
        {
            get { return _title; }
            set { _title = value; }
        }
        public string Id
        {
            get { return _id; }
            set { _id = value; }
        }
        public Geometry Image
        {
            get { return _image; }
            set { _image = value; }
        }

        public MetroMenuItem(string title, Geometry image,string id)
        {
            this._image = image;
            this._title = title;
            this._id = id;
        }
    }
}
