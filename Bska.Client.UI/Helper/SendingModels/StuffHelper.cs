
namespace Bska.Client.UI.Helper
{
    using System;
    public class StuffHelper
    {
        public string StuffId { get; set; }
        public String Name { get; set; }
        public Boolean IsStuff { get; set; }
        public Int32 StuffType { get; set; }
        public Int32? FloorOld { get; set; }
        public Int32? FloorNew { get; set; }
        public Boolean isBelonging { get; set; }
        public int? parentId { get; set; }

        public string getreportDescByStuffId(int stuffId)
        {
            string str=null;
            if (stuffId == 23102)
            {
                str = "مدل|کارخانه سازنده|کشور سازنده|شماره سریال|قدرت|خالی|خالی|خالی";
            }
            else if (stuffId == 26101 || stuffId == 262)
            {
                str = "سال چاپ|نویسنده|نوع جلد|شابگ|نام کتاب|تعداد صفحه|تعداد جلد|زبان";
            }
            else if (stuffId >= 25101 && stuffId <= 25110)
            {
                str = "مدل|کارخانه سازنده|کشور سازنده|شماره سریال|قدرت|خالی|خالی|خالی";
            }
            else if (stuffId == 23101)
            {
                str = "رنگ/مدل|کارخانه سازنده|کشور سازنده|سریال|مساحت|خالی|خالی|خالی";
            }
            else if (stuffId == 23104)
            {
                str = "رنگ/مدل|کارخانه سازنده|کشور سازنده|سریال|اندازه|خالی|خالی|خالی";
            }
            else if (stuffId == 23105)
            {
                str = "رنگ اصلی|نقش|محل بافت|شکل هندسی|مساحت|طول/قطر|عرض|تعداد رج";
            }
            else if (stuffId == 23108)
            {
                str = "خالی|خالی|خالی|خالی|نام لوح|تعداد لوح|خالی|خالی";
            }
            else if (stuffId == 23202)
            {
                str = "رنگ / مدل|کارخانه سازنده|کشور سازنده|خالی|قدرت به وات|شماره موتور|شماره بدنه|خالی";
            }
            else if (stuffId == 23201)
            {
                str = "رنگ / مدل|کارخانه سازنده|کشور سازنده|سریال|وزن|قدرت|خالی|خالی";
            }
            else if (stuffId >= 23501 && stuffId <= 23505)
            {
                str = "رنگ / مدل|کارخانه سازنده|کشور سازنده|سریال|شماره عدسی|حجم حافظه|خالی|خالی";
            }
            else if ((stuffId >= 24101 && stuffId <= 24135) || stuffId == 24201)
            {
                str = "رنگ / مدل|کارخانه سازنده|کشور سازنده|سریال|قدرت|خالی|خالی|خالی";
            }
            else
            {
                str = "رنگ / مدل|جنس اصلی|کشور سازنده|سریال|طول عرض|ارتفاع|خالی|خالی";
            }
            return str;
        }
    }
}
