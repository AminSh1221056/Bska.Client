
namespace Bska.Client.UI.Helper
{
    using Bska.Client.Common;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Reflection;
    using System.Xml;
    using System.Xml.Linq;
    using System.Linq;
    using System.Data;
    using Domain.Entity;
    using System.IO;
    using MahApps.Metro.Controls;
    using MahApps.Metro.SimpleChildWindow;
    using System.Windows.Controls;
    using System.Runtime.CompilerServices;

    public static class HelperExtensions
    {
        public static void MutateVerbose<TField>(this INotifyPropertyChanged instance, ref TField field, TField newValue, Action<PropertyChangedEventArgs> raise, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<TField>.Default.Equals(field, newValue)) return;
            field = newValue;
            raise?.Invoke(new PropertyChangedEventArgs(propertyName));
        }

        public static async void showChidWindowAsync(this MetroWindow metroWin, ChildWindow window, Panel rootpanel)
        {
            await metroWin.ShowChildWindowAsync(window, rootpanel);
        }

        public static IEnumerable<T> Select<T>(this IDataReader reader, Func<IDataReader, T> projection)
        {
            while (reader.Read())
            {
                yield return projection(reader);
            }
        }

        public static bool EasyEquals(this string a, string b)
        {
            if (a == null)
            {
                if (b == null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                if (b == null)
                {
                    return false;
                }
                else
                {
                    return a.Equals(b, StringComparison.InvariantCultureIgnoreCase);
                }
            }
        }

        public static string SuperTrim(this string value)
        {
            if (value != null)
            {
                value.Trim();
                if (value.Length == 0)
                {
                    value = null;
                }
            }
            return value;
        }

        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (action == null) throw new ArgumentNullException("action");
            foreach (var element in source)
            {
                action(element);
            }
        }

        public static IEnumerable<IEnumerable<T>> Split<T>(this IEnumerable<T> source,int itemSize)
        {
            return source.Select((item1, index) => new { Index = index, Value = item1 })
                .GroupBy(x => x.Index / itemSize).Select(x => x.Select(v => v.Value));
        }

        public static void Update<TSource>(this IEnumerable<TSource> outer, Action<TSource> updator)
        {
            foreach (var item in outer)
            {
                updator(item);
            }
        }

        public static string RemoveAllNamespaces(string xmlDocument)
        {
            XElement xmlDocumentWithoutNs = RemoveAllNamespaces(XElement.Parse(xmlDocument));

            return xmlDocumentWithoutNs.ToString();
        }

        //Core recursion function
        public static XElement RemoveAllNamespaces(this XElement xmlDocument)
        {
            if (!xmlDocument.HasElements)
            {
                XElement xElement = new XElement(xmlDocument.Name.LocalName);
                xElement.Value = xmlDocument.Value;

                foreach (XAttribute attribute in xmlDocument.Attributes())
                    xElement.Add(attribute);

                return xElement;
            }
            return new XElement(xmlDocument.Name.LocalName, xmlDocument.Elements().Select(el => RemoveAllNamespaces(el)));
        }

        public static XmlDocument ToXmlDocument(this XDocument xDocument)
        {
            var xmlDocument = new XmlDocument();
            using (var xmlReader = xDocument.CreateReader())
            {
                xmlDocument.Load(xmlReader);
            }
            return xmlDocument;
        }

        public static XDocument ToXDocument(this XmlDocument xmlDocument)
        {
            using (var nodeReader = new XmlNodeReader(xmlDocument))
            {
                nodeReader.MoveToContent();
                return XDocument.Load(nodeReader, LoadOptions.None);
            }
        }

        public static int SetPersianYear(this string year)
        {
            int insertYear = -1;
            if (!int.TryParse(year, out insertYear))
            {
                return -1;
            }

            if (year.Length == 2)
            {
                string leftSec = PersianDate.Today.Year.ToString().Substring(0, 2);
                insertYear = int.Parse(string.Concat(leftSec, year));
            }
            else if (year.Length == 4)
            {
                insertYear = int.Parse(year);
            }
            else
            {
                insertYear = -1;
            }
            return insertYear;
        }

        public static String GetDescription<T>(this T enumerationValue) where T : struct
        {
            Type type = enumerationValue.GetType();
            if (!type.IsEnum)
            {
                throw new ArgumentException("EnumerationValue must be of Enum type", "enumerationValue");
            }
            MemberInfo[] memberInfo = type.GetMember(enumerationValue.ToString());
            if (memberInfo != null && memberInfo.Length > 0)
            {
                object[] attrs = memberInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);

                if (attrs != null && attrs.Length > 0)
                {
                    //Pull out the description value
                    return ((DescriptionAttribute)attrs[0]).Description;
                }
            }
            //If we have no description attribute, just return the ToString of the enum
            return enumerationValue.ToString();
        }

        public static T ToObject<T>(this DataRow row) where T : class, new()
        {
            T obj = new T();

            foreach (var prop in obj.GetType().GetProperties())
            {
                try
                {
                    if (prop.PropertyType.IsGenericType && prop.PropertyType.Name.Contains("Nullable"))
                    {
                        if (!string.IsNullOrEmpty(row[prop.Name].ToString()))
                            prop.SetValue(obj, Convert.ChangeType(row[prop.Name],
                            Nullable.GetUnderlyingType(prop.PropertyType), null));
                        //else do nothing
                    }
                    else
                        prop.SetValue(obj, Convert.ChangeType(row[prop.Name], prop.PropertyType), null);
                }
                catch
                {
                    continue;
                }
            }
            return obj;
        }
        /// <summary>
        /// Converts a DataTable to a list with generic objects
        /// </summary>
        /// <typeparam name="T">Generic object</typeparam>
        /// <param name="table">DataTable</param>
        /// <returns>List with generic objects</returns>
        public static List<T> DataTableToList<T>(this DataTable table) where T : class, new()
        {
            try
            {
                List<T> list = new List<T>();

                foreach (var row in table.AsEnumerable())
                {
                    var obj = row.ToObject<T>();

                    list.Add(obj);
                }

                return list;
            }
            catch
            {
                return null;
            }
        }

        public static T GetNext<T>(this IEnumerable<T> list,T current)
        {
            try
            {
                return list.SkipWhile(x => !x.Equals(current)).Skip(1).First();
            }
            catch
            {
                return default(T);
            }
        }

        public static T GetPrevious<T>(this IEnumerable<T> list,T current)
        {
            try
            {
                return list.TakeWhile(x => !x.Equals(current)).Last();
            }
            catch
            {
                return default(T);
            }
        }

        public static Country FromDataReaderCountry(this IDataReader r)
        {
            return new Country
            {
                CountryId = r["CountryId"] is DBNull ? 0 : Convert.ToInt32(r["CountryId"]),
                CountryName = r["CountryName"] is DBNull ? "نامشخص" : r["CountryName"].ToString(),
                CarCorporationId = r["CarCorporationId"] is DBNull ? 0 : Convert.ToInt32(r["CarCorporationId"]),
            };
        }

        public static Company FromDataReaderCompany(this IDataReader r)
        {
            return new Company
            {
                CountryId = r["CountryId"] is DBNull ? 0 : Convert.ToInt32(r["CountryId"]),
                Name = r["Name"] is DBNull ? "نامشخص" : r["Name"].ToString(),
                CompanyId = r["CompanyId"] is DBNull ? 0 : Convert.ToInt32(r["CompanyId"]),
            };
        }

        public static InsuranceCompany FromDataReaderInsuranceCo(this IDataReader r)
        {
            return new InsuranceCompany
            {
                InsuranceId = r["InsuranceId"] is DBNull ? 0 : Convert.ToInt32(r["InsuranceId"]),
                Name = r["Name"] is DBNull ? "نامشخص" : r["Name"].ToString(),
            };
        }

        public static CarDetails FromDataReaderCarDetails(this IDataReader r)
        {
            return new CarDetails
            {
                CarDetailsId = r["CarDetailsId"] is DBNull ? 0 : Convert.ToInt32(r["CarDetailsId"]),
                Model = r["Model"] is DBNull ? "نامشخص" : r["Model"].ToString(),
                CompanyId = r["CompanyId"] is DBNull ? 0 : Convert.ToInt32(r["CompanyId"]),
                SystemType = r["SystemType"] is DBNull ? "نامشخص" : r["SystemType"].ToString(),
                Tipe = r["Tipe"] is DBNull ? "نامشخص" : r["Tipe"].ToString(),
                CarType=(CarType)Enum.ToObject(typeof(CarType),r["CarType"] is DBNull ? 0 : Convert.ToInt32(r["CarType"])),
            };
        }

        public static OrganizationModel FromDataReaderOrganization(this IDataReader r)
        {
            return new OrganizationModel
            {
                BudgetNo = r["BudgetNo"] is DBNull ? 0 : Convert.ToInt32(r["BudgetNo"]),
                Name= r["Name"] is DBNull ? "نامشخص" : r["Name"].ToString(),
            };
        }

        public static Province FromDataReaderProvince(this IDataReader r)
        {
            return new Province
            {
                ID = r["ID"] is DBNull ? "نامشخص" : r["ID"].ToString(),
                Name = r["Name"] is DBNull ? "نامشخص" : r["Name"].ToString(),
            };
        }

        public static TwonShip FromDataReaderTwonShip(this IDataReader r)
        {
            return new TwonShip
            {
                ID = r["ID"] is DBNull ? "نامشخص" : r["ID"].ToString(),
                Name = r["Name"] is DBNull ? "نامشخص" : r["Name"].ToString(),
            };
        }

        public static Zone FromDataReaderZone(this IDataReader r)
        {
            return new Zone
            {
                ID = r["ID"] is DBNull ? "نامشخص" : r["ID"].ToString(),
                Name = r["Name"] is DBNull ? "نامشخص" : r["Name"].ToString(),
            };
        }

        public static City FromDataReaderCity(this IDataReader r)
        {
            return new City
            {
                ID = r["ID"] is DBNull ? "نامشخص" : r["ID"].ToString(),
                Name = r["Name"] is DBNull ? "نامشخص" : r["Name"].ToString(),
            };
        }

        public static EstateOrgan FromDataReaderEstateOrgan(this IDataReader r)
        {
            return new EstateOrgan
            {
                Id = r["Id"] is DBNull ? 0 : Convert.ToInt32(r["Id"]),
                ProvinceId = r["ProvinceId"] is DBNull ? "نامشخص" : r["ProvinceId"].ToString(),
                Name = r["Name"] is DBNull ? "نامشخص" : r["Name"].ToString(),
            };
        }

        public static string GetEmbeddedResource(string resourceName, Assembly assembly)
        {
            resourceName = FormatResourceName(assembly, resourceName);
            using (Stream resourceStream = assembly.GetManifestResourceStream(resourceName))
            {
                if (resourceStream == null)
                    return null;

                using (StreamReader reader = new StreamReader(resourceStream))
                {
                    return reader.ReadToEnd();
                }
            }
        }

        private static string FormatResourceName(Assembly assembly, string resourceName)
        {
            return assembly.GetName().Name + "." + resourceName.Replace(" ", "_")
                                                               .Replace("\\", ".")
                                                               .Replace("/", ".");
        }
    }
}
