using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bska.Client.UI.API.Client
{
    public interface IBaseInfoService
    {
        double CurrentProgress { get; set; }
        Task<int> saveStuffAsync(string apiUrl,IProgress<double> progress = null);
        Task<int> saveUnitAsync(string apiUrl, IProgress<double> progress = null);
        //Task<int> saveAccounCodingAsync(string apiUrl, IProgress<double> progress = null);
        Task<int> saveCountryAsync(string apiUrl, string companyUrl, IProgress<double> progress = null);
        Task<int> saveInsuranceAsync(string apiUrl, IProgress<double> progress = null);
        Task<int> saveCarCompanyAsync(string apiUrl, IProgress<double> progress = null);
        Task<int> saveOrganAsync(string apiUrl, IProgress<double> progress = null);
        Task<int> saveStateAsync(string provinceUrl,string townshipUrl,string zoneUrl,string cityUrl, IProgress<double> progress = null);
        Task<int> saveEstateOrganAsync(string apiUrl, IProgress<double> progress = null);
    }
}
