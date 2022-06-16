using FoodpandaMenuImportLog.Dto;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodpandaMenuImportLog.ViewModels
{
    public class FoodpandaMenuImportLogViewerViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private string _ApiUrlInput = "https://integration-middleware.as.restaurant-partners.com/";
        public string ApiUrlInput
        {
            get { return _ApiUrlInput; }
            set
            {
                if (value != _ApiUrlInput)
                {
                    _ApiUrlInput = value;
                    RaisePropertyChanged("ApiUrlInput");
                }
            }
        }

        private string _UsernameInput;
        public string UsernameInput
        {
            get { return _UsernameInput; }
            set
            {
                if (value != _UsernameInput)
                {
                    _UsernameInput = value;
                    RaisePropertyChanged("UsernameInput");
                }
            }
        }

        private string _PasswordInput;
        public string PasswordInput
        {
            get { return _PasswordInput; }
            set
            {
                if (value != _PasswordInput)
                {
                    _PasswordInput = value;
                    RaisePropertyChanged("PasswordInput");
                }
            }
        }

        private string _ChainCodeInput;
        public string ChainCodeInput
        {
            get { return _ChainCodeInput; }
            set
            {
                if (value != _ChainCodeInput)
                {
                    _ChainCodeInput = value;
                    RaisePropertyChanged("ChainCodeInput");
                }
            }
        }

        private string _VendorIdInput;
        public string VendorIdInput
        {
            get { return _VendorIdInput; }
            set
            {
                if (value != _VendorIdInput)
                {
                    _VendorIdInput = value;
                    RaisePropertyChanged("VendorIdInput");
                }
            }
        }

        private DateTime _FormDateInput = DateTime.Now;
        public DateTime FormDateInput
        {
            get { return _FormDateInput; }
            set
            {
                if (value != _FormDateInput)
                {
                    _FormDateInput = value;
                    RaisePropertyChanged("FormDateInput");
                }
            }
        }

        private DateTime _ToDateInput = DateTime.Now;
        public DateTime ToDateInput
        {
            get { return _ToDateInput; }
            set
            {
                if (value != _ToDateInput)
                {
                    _ToDateInput = value;
                    RaisePropertyChanged("ToDateInput");
                }
            }
        }

        private int _LimitInput = 5;
        public int LimitInput
        {
            get { return _LimitInput; }
            set
            {
                if (value != _LimitInput)
                {
                    _LimitInput = value;
                    RaisePropertyChanged("LimitInput");
                }
            }
        }

        private List<FoodpandaMenuImportLogDto> _MenuImportLogList;
        public List<FoodpandaMenuImportLogDto> MenuImportLogList
        {
            get { return _MenuImportLogList; }
            set
            {
                if (value != _MenuImportLogList)
                {
                    _MenuImportLogList = value;
                    RaisePropertyChanged("MenuImportLogList");
                }
            }
        }

        private System.Windows.Visibility _QueryProcessBarVisibity = System.Windows.Visibility.Collapsed;
        public System.Windows.Visibility QueryProcessBarVisibity
        {
            get { return _QueryProcessBarVisibity; }
            set
            {
                if (value != _QueryProcessBarVisibity)
                {
                    _QueryProcessBarVisibity = value;
                    RaisePropertyChanged("QueryProcessBarVisibity");
                }
            }
        }

        private int _QueryProcessBarValue = 0;
        public int QueryProcessBarValue
        {
            get { return _QueryProcessBarValue; }
            set
            {
                if (value != _QueryProcessBarValue)
                {
                    _QueryProcessBarValue = value;
                    RaisePropertyChanged("QueryProcessBarValue");
                }
            }
        }

        private string _QueryProcessBarLabel = "";
        public string QueryProcessBarLabel
        {
            get { return _QueryProcessBarLabel; }
            set
            {
                if (value != _QueryProcessBarLabel)
                {
                    _QueryProcessBarLabel = value;
                    RaisePropertyChanged("QueryProcessBarLabel");
                }
            }
        }
    }
}
