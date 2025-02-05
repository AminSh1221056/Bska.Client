﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using Bska.Client.Common;

namespace Bska.Client.UI.Controls.CustomGridView.Support
{
    [Serializable()]
    public class FilterData : INotifyPropertyChanged
    {
        #region Metadata

        public FilterType Type { get; set; }
        public String ValuePropertyBindingPath { get; set; }
        public Type ValuePropertyType { get; set; }
        public bool IsTypeInitialized { get; set; }
        public bool IsCaseSensitiveSearch { get; set; }

        //query optimization fileds
        public bool IsSearchPerformed { get; set; }
        public bool IsRefresh { get; set; }
        //query optimization fileds
        #endregion

        #region Filter Change Notification
        public event EventHandler<EventArgs> FilterChangedEvent;
        private bool isClearData;

        private void OnFilterChangedEvent()
        {
            EventHandler<EventArgs> temp = FilterChangedEvent;

            if (temp != null)
            {
                bool filterChanged = false;

                switch (Type)
                {
                    case FilterType.Numeric:
                    case FilterType.DateTime:
                    case FilterType.PersianTime:
             
                        filterChanged = (Operator != FilterOperator.Undefined || QueryString != String.Empty);
                        break;

                    case FilterType.NumericBetween:
                    case FilterType.DateTimeBetween:
                    case FilterType.PersianTimeBetween:

                        _operator = FilterOperator.Between;
                        filterChanged = true;
                        break;

                    case FilterType.Text:

                        _operator = FilterOperator.Like;
                        filterChanged = true;
                        break;
                        
                    case FilterType.List:
                    case FilterType.Boolean:

                        _operator = FilterOperator.Equals;
                        filterChanged = true;
                        break;

                    default:
                        filterChanged = false;
                        break;
                }

                if (filterChanged && !isClearData) temp(this, EventArgs.Empty);
            }
        }
        #endregion
        public void ClearData()
        {
            isClearData = true;

            Operator = FilterOperator.Undefined;
            if (QueryString   != String.Empty) QueryString = null;
            if (QueryStringTo != String.Empty) QueryStringTo = null;

            isClearData = false;
        }

        private FilterOperator _operator;
        public FilterOperator Operator
        {
            get { return _operator; }
            set
            {
                if(_operator != value)
                {
                    _operator = value;
                    NotifyPropertyChanged("Operator");
                    OnFilterChangedEvent();
                }
            }
        }

        private FilterStringOperator _stringOperator;
        public FilterStringOperator StringOperator
        {
            get { return _stringOperator; }
            set
            {
                if (_stringOperator != value)
                {
                    _stringOperator = value;
                    NotifyPropertyChanged("StringOperator");
                    OnFilterChangedEvent();
                }
            }
        }

        private PersianDate _pDate=PersianDate.Today;
        public PersianDate PDate
        {
            get { return _pDate; }
            set
            {
                if (_pDate != value)
                {
                    _pDate = value;
                    if(_pDate==null) queryString = String.Empty;
                    else
                    {
                        queryString = _pDate.ToString();
                    }
                    NotifyPropertyChanged("PDate");
                    OnFilterChangedEvent();
                }
            }
        }

        private PersianDate _pdateTo=PersianDate.Today;
        public PersianDate PDateTo
        {
            get { return _pdateTo; }
            set
            {
                if (_pdateTo != value)
                {
                    _pdateTo = value;
                    if (_pdateTo == null) queryStringTo = String.Empty;
                    else
                    {
                        queryStringTo = _pdateTo.ToString();
                    }
                    NotifyPropertyChanged("PDateTo");
                    OnFilterChangedEvent();
                }
            }
        }


        private string queryString;
        public string QueryString
        {
            get { return queryString; }
            set
            {
                if (queryString != value)
                {
                    queryString = value;

                    if (queryString == null) queryString = String.Empty;

                    NotifyPropertyChanged("QueryString");
                    OnFilterChangedEvent();
                }
            }
        }

        private string queryStringTo;
        public string QueryStringTo
        {
            get { return queryStringTo; }
            set
            {
                if (queryStringTo != value)
                {
                    queryStringTo = value;

                    if (queryStringTo == null) queryStringTo = String.Empty;

                    NotifyPropertyChanged("QueryStringTo");
                    OnFilterChangedEvent();
                }
            }
        }

        public FilterData(
            FilterOperator Operator,
            FilterStringOperator stringOperator,
            FilterType Type,
            String ValuePropertyBindingPath,
            Type ValuePropertyType,
            String QueryString,
            String QueryStringTo,
            bool IsTypeInitialized,
            bool IsCaseSensitiveSearch
            )
        {
            this.Operator = Operator;
            this.StringOperator = stringOperator;
            this.Type = Type;
            this.ValuePropertyBindingPath = ValuePropertyBindingPath;
            this.ValuePropertyType = ValuePropertyType;
            this.QueryString   = QueryString;
            this.QueryStringTo = QueryStringTo;

            this.IsTypeInitialized    = IsTypeInitialized;
            this.IsCaseSensitiveSearch = IsCaseSensitiveSearch;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
