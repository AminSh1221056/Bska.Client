
namespace Bska.Client.Common
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Data.SqlClient;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Security.Cryptography;
    using System.Text;

    public static class GlobalClass
    {
        public static string _UserName = "";
        public static string _Password = "";
        public static string _FullName = "";
        public static PermissionsType _PermissionType;
        public static Int32 _UserID = -1;
        public static DateTime _Today = DateTime.Now;
        static string iv = "1869D7528A9CCA79";
        static string key = "66Ak679Du4V3qo92";
        static byte[] _salt = Encoding.ASCII.GetBytes("v2805642JcN2c5");
        public static string GetMd5Hash(string input)
        {
            // Create a new instance of the MD5CryptoServiceProvider object.
            MD5 md5Hasher = MD5.Create();

            // Convert the input string to a byte array and compute the hash.
            byte[] data = md5Hasher.ComputeHash(Encoding.Default.GetBytes(input));

            // Create a new Stringbuilder to collect the bytes
            // and create a string.
            StringBuilder sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data 
            // and format each one as a hexadecimal string.
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("X2"));
            }

            // Return the hexadecimal string.
            return sBuilder.ToString();
        }

        public static string EncryptStringAES(string plainText, string sharedSecret)
        {
            if (string.IsNullOrEmpty(plainText))
                throw new ArgumentNullException("plainText");
            if (string.IsNullOrEmpty(sharedSecret))
                throw new ArgumentNullException("sharedSecret");

            string outStr = null;                       // Encrypted string to return
            RijndaelManaged aesAlg = null;              // RijndaelManaged object used to encrypt the data.

            try
            {
                // generate the key from the shared secret and the salt
                Rfc2898DeriveBytes key = new Rfc2898DeriveBytes(sharedSecret, _salt);

                // Create a RijndaelManaged object
                aesAlg = new RijndaelManaged();
                aesAlg.Key = key.GetBytes(aesAlg.KeySize / 8);

                // Create a decryptor to perform the stream transform.
                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for encryption.
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    // prepend the IV
                    msEncrypt.Write(BitConverter.GetBytes(aesAlg.IV.Length), 0, sizeof(int));
                    msEncrypt.Write(aesAlg.IV, 0, aesAlg.IV.Length);
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            //Write all data to the stream.
                            swEncrypt.Write(plainText);
                        }
                    }
                    outStr = Convert.ToBase64String(msEncrypt.ToArray());
                }
            }
            finally
            {
                // Clear the RijndaelManaged object.
                if (aesAlg != null)
                    aesAlg.Clear();
            }

            string rpCode = outStr;
            // Return the encrypted bytes from the memory stream.
            return rpCode;
        }

        // Decrypts using AES and an IV (use the same IV when encrypting/decrypting)

        public static string DecryptStringAES(string rpCode, string sharedSecret)
        {
            if (string.IsNullOrEmpty(rpCode))
                throw new ArgumentNullException("cipherText");
            if (string.IsNullOrEmpty(sharedSecret))
                throw new ArgumentNullException("sharedSecret");
            string cipherText = rpCode;
            // Declare the RijndaelManaged object
            // used to decrypt the data.
            RijndaelManaged aesAlg = null;

            // Declare the string used to hold
            // the decrypted text.
            string plaintext = null;

            try
            {
                // generate the key from the shared secret and the salt
                Rfc2898DeriveBytes key = new Rfc2898DeriveBytes(sharedSecret, _salt);

                // Create the streams used for decryption.                
                byte[] bytes = Convert.FromBase64String(cipherText);
                using (MemoryStream msDecrypt = new MemoryStream(bytes))
                {
                    // Create a RijndaelManaged object
                    // with the specified key and IV.
                    aesAlg = new RijndaelManaged();
                    aesAlg.Key = key.GetBytes(aesAlg.KeySize / 8);
                    // Get the initialization vector from the encrypted stream
                    aesAlg.IV = ReadByteArray(msDecrypt);
                    // Create a decrytor to perform the stream transform.
                    ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))

                            // Read the decrypted bytes from the decrypting stream
                            // and place them in a string.
                            plaintext = srDecrypt.ReadToEnd();
                    }
                }
            }
            finally
            {
                // Clear the RijndaelManaged object.
                if (aesAlg != null)
                    aesAlg.Clear();
            }
            return plaintext;
        }

        private static byte[] ReadByteArray(Stream s)
        {
            byte[] rawLength = new byte[sizeof(int)];
            if (s.Read(rawLength, 0, rawLength.Length) != rawLength.Length)
            {
                throw new SystemException("Stream did not contain properly formatted byte array");
            }

            byte[] buffer = new byte[BitConverter.ToInt32(rawLength, 0)];
            if (s.Read(buffer, 0, buffer.Length) != buffer.Length)
            {
                throw new SystemException("Did not read byte array properly");
            }

            return buffer;
        }

        public static bool CheckForInternetConnection(string uri)
        {
            try
            {
                using (var client = new WebClient())
                {
                    using (var stream = client.OpenRead(uri))
                    {
                        return false;
                    }
                }
            }
            catch
            {
                return true;
            }
        }

        public static string CheckAccountCode(string code)
        {
            if (code.Length == 10) return code;
            else if (code.Length > 10) return code.Substring(0, 10);
            else
            {
                int l =10- code.Length;
                var str = "";
                for(int i = 0; i < l; i++)
                {
                    str += "0";
                }
                return str+code;
            }
        }
    }

    public interface IPortotype<T>
    {
       T Clone();
    }

    public enum PermissionsType : byte
    {
        [Description("مدیر سامانه")]
        Manager = 1,
        [Description("پرسنل عادی")]
        StandardUser,
        [Description("امین اموال")]
        StuffHonest,
        [Description("مدیریت سازمان")]
        GeneralManager,
        [Description("مسئول انبار")]
        StoreLeader,
        [Description("مسئول تدارکات")]
        MunitionLeader,
        [Description("کار پرداز")]
        Supplier,
        [Description("ذی حساب")]
        Accountant,
        [Description("نگهبان")]
        Guard,
    }

    public enum ContractType
    {
        [Description("نا مشخص")]
        None = 0,
        [Description("استخدام")]
        ToEmploye = 1,
        [Description("قراردادی")]
        Contractual,
    }

    public enum StoreType
    {
        [Description("اصلی")]
        Mixed=1,
        [Description("اسقاط")]
        Retiring
    }

    public enum StuffType
    {
        None = 11000,
        [Description("مصرفی")]
        Consumable = 11001,
        [Description("غیرمصرفی")]
        UnConsumption,
        [Description("در حکم مصرف")]
        OrderConsumption,
        [Description("قابل نصب در بنا")]
        Installable,
        [Description("متعلقات")]
        Belonging
    }

    public enum OrderType
    {
        [Description("درخواست داخلی")]
        InternalRequest = 1,
        [Description("اعلام مازاد")]
        Displacement,
        [Description("صورت جلسه")]
        Procceding,
        [Description("انبار")]
        Store,
        [Description("انتقال داخلی")]
        InternalTransfer
    }

    public enum OrderStatus
    {
        [Description("کل وضعیت ها")]
        None =0,
        [Description("در دست اقدام")]
        OrganizManagerConfirm,
        [Description("مدیریت")]
        ManagerConfirm,
        [Description("امین اموال")]
        StuffHonest,
        [Description("سفارش")]
        SubOrder,
        [Description("تحویل")]
        Deliviry,
        [Description("رد شده")]
        Reject,
    }

    public enum OrderDetailsState
    {
        None = 0,
        OrganizManagerConfirm,
        ManagerConfirm,
        StuffHonest,
        SubOrder,
        Deliviry,
        ToOther,
    }

    public enum SubOrderType
    {
        Store = 1,
        Supplier,
        Displacement,
        StoreBillDirect,
        TenderOffer
    }

    public enum SubOrderState
    {
        None = 0, 
        Deliviry=4,
        Confirm=5,
        Reject=6
    }

    public enum SupplierIndentState
    {
        Ongoing = 1,
        Delivery = 4,
    }

    public enum StuffTypeColor
    {
        FF4682B4 = 11000, //displacement
        FFB1C70C = 11001, //UseStuff
        FFF24E22, //NoUseStuff
        FF529011, //InUseStuff
        FFDA1A4C, //InstallableStructre
        FFFB791A//Appurtenance
    }

    public enum OrderStatusColor
    {
        FFFB791B = 0,
        FF4682B3,
        FFF24E22,
        FFFB791A,
        FFB1C70C,
        FF529011,
        FFDA1A4C,
    }

    public enum MeterBillTypeColor
    {
        FF4294DE =1, //WaterBill
        FFFE7C22,//powerBill
        FFDE4AAD,//tellBill
        FFE1B700,//gasBill
        FF439D9A,//mobileBill
    }

    public enum DocumentType
    {
        [Description("کل اسناد")]
        None =0,
        [Description("حواله انبار داخلی")]
        StoreInternalDraft,
        [Description("بازگشت به انبار")]
        ReturnToStoreDraft,
        [Description("سند موجودی اولیه")]
        InitialBalance,
        [Description("حواله انبار امانی داخلی")]
        InternalStoreTrustDraft,
        [Description("بازگشت به انبار امانی داخلی")]
        ReturnToStoreTrustDraft,
        [Description("حواله انبار خروج")]
        ExitStoreDraft,
        [Description("حواله انبار امانی خروج")]
        ExitStoreTrustDraft,
        [Description("قبض انبار اسقاط")]
        StoreBillRetiring,
        [Description("قبض انبار بازگشت از امانی")]
        StoreBillReturnFromTrust,
    }

    public enum UnitMathType
    {
        [Description("تعریف نشده")]
        None = 0,
        [Description("ضرب")]
        Multiple,
        [Description("تقسیم")]
        Divide,
    }

    public enum StateOwnership
    {
        [Description("خریداری")]
        Purchase = 13001,
        [Description("اهدایی")]
        Donation,
        [Description("تملیکی")]
        Owned,
        [Description("انتقالی")]
        GovCompanyRecived,
        [Description("امانی")]
        Trust
    }

    public enum CarType
    {
        [Description("سواری")]
        Driving = 2001,
        [Description("آمبولانس")]
        Ambulances,
        [Description("وانت")]
        Pickups
    }

    public enum EstateType
    {
        [Description("زمین مزروعی")]
        FarmLand = 3001,
        [Description("باغ")]
        Garden,
        [Description("ساختمان")]
        Building
    }
    
    public enum AgentType
    {
        [Description("نماینده حقیقی")]
        Actual = 5001,
        [Description("نماینده حقوقی")]
        Legal
    }
    
    public enum LocationStatus
    {
        Executive=1,
        Active = 2,
        MovedRequest,
        StoreActive,
        DeActive,
        StoreDeActive,
        Trust,
        Delete,
        Sale,
        Retiring,
        RetiringDeActive,
        Transfer,
        TransferState,
        Accident,
        AccidentDeActive,
        Send,
        TrustDeActive,
        RefundTrust,
    }

    public enum AccountDocumentType
    {
        [Description("کل اسناد")]
        None = 81000,
        [Description("دستگاه اجرایی-رسیده")]
        ExecutiveToReached = 81001,
        [Description("رسیده-انبار")]
        ReachedToStock = 81002,
        [Description("انبار-واحدها")]
        StockToUnits = 81003,
        [Description("واحدها-انبار")]
        UnitsToStock = 81004,
        [Description("انبار-امانی")]
        StockToTrust = 81051,
        [Description("واحدها-امانی")]
        UnitsToTrust = 81052,
        [Description("امانی-انبار")]
        TrustToStock = 81061,
        [Description("امانی-واحدها")]
        TrustToUnits = 81062,
        [Description("طرف حساب اموال امانی-امانی")]
        EscrowToTrust = 81007,
        [Description("امانی-طرف حساب اموال امانی")]
        TrustToEscrow = 81008,
        [Description("انبار-اسقاط")]
        StockToRetiring = 81091,
        [Description("واحدها-اسقاط")]
        UnitsToRetiring = 81092,
        [Description("انبار-حادثه")]
        StockToDisaster = 81101,
        [Description("واحد ها - حادثه")]
        UnitsToDisaster = 81102,
        [Description("اسقاط-فرستاده")]
        RetiringToSent = 81201,
        [Description("حادثه-فرستاده")]
        DisasterToSent = 81202,
        [Description("فرستاده-دستگاه اجرایی")]
        SentToExecutive = 81301,
        [Description("انبار-فرستاده")]
        StockToSent = 81401,
        [Description("واحدها - واحدها")]
        UnitsToUnits = 81501,
        [Description("اسقاط - انبار")]
        RetiringToStock = 81601,
        [Description("اسقاط - واحدها")]
        RetiringToUnits = 81602,
    }

    public enum CertainAccountsType
    {
        None=0,
        [Description("حساب معین خریداری")]
        ReachedAssetBuy = 0101,
        [Description("حساب معین انتقالی")]
        ReachedAssetTransfer = 0102,
        [Description("حساب معین اهدایی")]
        ReachedAssetDenotion = 0103,
        [Description("حساب معین سایر")]
        ReachedAssetOther = 0104,
        [Description("حساب معین برحسب نوع مال و بهای خرید آن")]
        StockAssetBuyAndType =0201,
        [Description("حساب معین برحسب واحد تحویل گیرنده")]
        UnitsDeliviry =0301,
        [Description("حساب معین برحسب دستگاه اجرایی امانت گیرنده")]
        TrustOrganizationReciver =0401,
        [Description("حساب معین برحسب دستگاه اجرایی امانت دهنده")]
        ReachedTrustOrganizationSender =0501,
        [Description("طرف حساب اموال امانی")]
        Escrow =0601,
        [Description("حساب معین برحسب نوع مال")]
        RetiringAssetType =0701,
        [Description("حساب معین برحسب نوع مال")]
        DisasterAssetType =0801,
        [Description("حساب معین اموال انتقال داده شده")]
        SendTransfer =0901,
        [Description("حساب معین اموال فروخته شده")]
        SendSold =0902,
        [Description("حساب معین اموال حذف شده")]
        SendDelete =0903,
        [Description("حساب معین برحسب تسلسل برچسب")]
        ExecutiveSequenceLabel = 1001,
        Tafsili = 20010,
        UnitsChilderen = 20020
    }

    public enum AccountingDescrtiption
    {
        [Description("کل اسناد")]
        None = 16000,
        [Description("حساب اموال رسیده")]
        Reached = 16001,
        [Description("حساب اموال موجود در انبار")]
        Stock,
        [Description("حساب اموال تحویلی به واحد ها")]
        Units,
        [Description("حساب اموال امانی")]
        Trust,
        [Description("حساب اموال امانی رسیده")]
        ReachedTrust,
        [Description("طرف حساب اموال امانی رسیده")]
        Escrow,
        [Description("حساب اموال اسقاط")]
        Retiring,
        [Description("حساب اموال حادثه دیده")]
        Disaster,
        [Description("حساب اموال فرستاده")]
        Send,
        [Description("حساب اموال دستگاه اجرایی")]
        Executive,
    }

    public enum MAssetCurState
    {
        [Description("کل وضعیت ها")]
        Noen = 15000,
        [Description("درحال بهره برداری / موجود در انبار")]
        AtOperation = 15001,
        [Description("درحال بهره برداری - انتقالی")]
        AtOpTransfer,
        [Description("درحال بهره برداری - بازگشت از امانی")]
        AtOpTrustReturn,
        [Description("درحال بهره برداری -انتقالی استانی")]
        AtOpTransferState,
        [Description("دارای مجوز انتقال")]
        TransferLicensing,
        [Description("دارای مجوز انتقال سازمانی")]
        TransferStateLicensing,
        [Description("دارای مجوز امانی")]
        TrustLicensing,
        [Description("دارای مجوز فروش مازاد")]
        SurplusSalesLicensing,
        [Description("دارای مجوز فروش اسقاط")]
        RetiringSalesLicensing,
        [Description("دارای مجوز حوادث")]
        AccidentLicensing,
        [Description("انتقال داده شده")]
        GovCompanyTransfer,
        [Description("انتقال درون سازمانی")]
        OutStateTransfer,
        [Description("فروش رفته")]
        Sold,
        [Description("حادثه دیده")]
        Disaster,
        [Description("ارسال امانی")]
        SendTrust,
        [Description("دارای مجوز مقررات ویژه")]
        SpecialProvisionsLicencing,
        [Description("دارای مجوز مقررات تبصره های بودجه")]
        BudgetLicencing,
        [Description("دارای مجوز حذف اموال غیرقابل فروش")]
        DeleteUnsaleableLicencing,
        [Description("حذف شده مقررات ویژه")]
        DeleteSpecialProvisions,
        [Description("حذف شده تبصره های بودجه")]
        DeleteBudget,
        [Description("حذف اموال غیر قابل فروش")]
        DeleteUnsaleable,
        [Description("دارای مجوز استرداد امانی")]
        RefundTrustLicencing,
        [Description("استرداد امانی")]
        RefundTrust,
    }

    public enum ProceedingsType
    {
        [Description("کل صورت جلسات")]
        None = 17000,
        [Description("انتقال قطعی")]
        DefinitiveTransfer = 17001,
        [Description("انتقال امانی")]
        TrustTransfer=17002,
        [Description("فروش")]
        Sale=17003,
        [Description("حذف مال")]
        Delete=17005,
        [Description("مقررات ویژه")]
        SpecialLicencing=17006,
        [Description("تبصره های بودجه")]
        BudgetLicencing=17007,
        [Description("ویرایش مال")]
        EditRequest = 17008,
        [Description("حادثه - سیل")]
        Flood=170041,
        [Description("حادثه - زلزله")]
        Earthquake=170042,
        [Description("حادثه - آتش سوزی")]
        Fire=170043,
        [Description("حادثه - تصادف")]
        Accident=170044,
        [Description("حادثه - سرقت")]
        Theft=170045,
        [Description("اسقاط")]
        AssetRetiring=170010,
        [Description("بازگشت از امانی")]
        ReturnFromTrust=170011,
        [Description("استرداد امانی")]
        RefundTrust=170012,
        [Description("انتقال درون سازمانی")]
        StateTransfer = 170013,
        [Description("بازگشت از اسقاط")]
        ReturnFromRetiring =170014,
    }

    public enum ProceedingState
    {
        [Description("در دست اقدام")]
        None = 0,
        [Description("تایید مدیریت")]
        ManagerConfirming = 1,
        [Description("تایید شده")]
        Confirmed = 2,
        [Description("تکمیل - تایید شده")]
        CompletedConfirm = 3,
        [Description("تکمیل - رد شده")]
        CompletedReject = 4,
        [Description("رد شده")]
        Rejected = 5,
        [Description("تایید شده - انبار")]
        StoreConfirm = 6
    }

    public enum AssetProceedingState
    {
        InProgress=1,
        IsRejected,
        IsConfirmed
    }

    public enum EventType
    {
        LogIn=1,
        LogOut,
        Exception,
        Insert,
        Update,
        Delete,
    }

    public enum DepreciationType
    {
        [Description("نزولی")]
        Descending = 1,
        [Description("مستقیم")]
        straight
    }

    public enum TaxCostType
    {
        [Description("ناشناخته")]
        None =1,
        [Description("تعمیرات")]
        Repair,
        [Description("بیمه")]
        Insurance
    }

    public enum ExportState
    {
        None=0,
        Pending,
        Confirmed,
        Rejected
    }

    public enum ExportType
    {
        FileTransfer=1,
        ServerTransfer
    }

    public enum CompietionState
    {
        [Description("گزارش نشده")]
        NotReported =10,
        [Description("در حال ارسال")]
        Reporting,
        [Description("گزازش شده")]
        Reported,
    }

    public enum MeterType
    {
        [Description("برق")]
        Power =1,
        [Description("آب")]
        Water,
        [Description("گاز")]
        Gas,
        [Description("تلفن")]
        Tell,
        [Description("موبایل")]
        Mobile,
    }

    public enum SellerType
    {
        [Description("حقیقی")]
        RealSeller=501,
        [Description("حقوقی")]
        LegalSeller,
    }

    public enum InsuranceType
    {
        [Description("آتش سوزی")]
        Fire=101,
        [Description("شخص ثالث")]
        ThirdPerson,
        [Description("بدنه")]
        Body,
        [Description("تضمین کیفیت")]
        QualityGuarantee
    }

    public enum AnalizModelIdentity
    {
        Stock=1,
        StoreBill,
        InternalDraft,
        Order,
    }
    public enum ExchangeType
    {
        [Description("ارسال اموال غیرمصرفی")]
        Unconsuption = 1,
        [Description("ارسال صورت جلسه")]
        Proceeding = 2,
        [Description("ارسال مشخصات ملک")]
        Estate = 3,
        [Description("ارسال اطلاعات پرسنل")]
        Peson = 4,
        [Description("ارسال اموال در حکم مصرف")]
        InCommodity = 5,
        [Description("ارسال اموال مصرفی")]
        Commodity = 6,
        [Description("ارسال اموال قابل نصب در بنا")]
        Installable = 7,
        [Description("ارسال اموال متعلقات")]
        Belonging = 8,
        [Description("ارسال اطلاعات کنتورها")]
        Meter = 9,
    }

    public enum CalendarMode
    {
        Month,
        Year,
        Decade,
    }

    public enum GlobalRequestStatus
    {
        [Description("در دست اقدام")]
        Pending =1,
        [Description("تایید شده")]
        Confirmed,
        [Description("رد شده")]
        Rejected,
        [Description("تکمیل شده")]
        Completed,
    }

    public enum MAssetReserveStatus
    {
        Reserved=1,
        UnReservedRequested,
        UnReserved,
        UnReservedToStore
    }

    public enum DocumentationLinkType
    {
        Wiki,
        DemoPageSource,
        ControlSource,
        StyleSource,
        Video
    }
}
