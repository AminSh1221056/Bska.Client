
namespace Bska.Client.UI.Helper
{
    using System;
    using Bska.Client.Domain.Entity;
    using Microsoft.Practices.Unity;
    using Domain.Entity.StoredProcedures;
    using Common;
    using System.Net.Http;
    using System.Collections.Generic;
    using System.Security.Cryptography.X509Certificates;

    public interface IUserLog
    {
        Users LogedUser { get; set; }
        Employee LogedEmployee { get; set; }
        HttpClient Client { get; set; }
        int AddLog(EventLog eventLog);
    }

    public class UserLog : IUserLog
    {
        public UserLog()
        {
            WebRequestHandler handler = new WebRequestHandler();
            X509Certificate2 cert = SeedDataHelper.GetX509Certificate();
            handler.ClientCertificates.Add(cert);
            Client = new HttpClient(handler);
            Client = new HttpClient();
            Client.BaseAddress = new Uri(Settings.Default.APIServiceURL);
            Client.DefaultRequestHeaders.Accept.Clear();
            this._lastViewDictionary = new Dictionary<string, string>();
        }

        [Dependency]
        public IBskaStoredProcedures IBskaStoredProc { get; set; }
        public Users LogedUser { get; set; }
        public Employee LogedEmployee { get; set; }
        public HttpClient Client { get; set; }
        public static UserLog UniqueInstance
        {
            get { return UserLogCreator.UniqueInstance; }
        }
        
        class UserLogCreator
        {
            //thread safe
            static UserLogCreator()
            {}
            internal static readonly UserLog UniqueInstance = new UserLog();
        }

        public int AddLog(EventLog eventLog)
        {
            try
            {
                return IBskaStoredProc.Insert_EventLog(eventLog);
            }
            catch
            {
                return 0;
            }
        }

        public string LastView(string key,string val)
        {
            if (!_lastViewDictionary.ContainsKey(key))
            {
                _lastViewDictionary.Add(key, val);
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(val))
                {
                    string temp = _lastViewDictionary[key];
                    if (!string.Equals(temp, val))
                    {
                        _lastViewDictionary[key] = val;
                    }
                }
            }
            return _lastViewDictionary[key];
        }

        private readonly Dictionary<string, string> _lastViewDictionary;
    }
}
