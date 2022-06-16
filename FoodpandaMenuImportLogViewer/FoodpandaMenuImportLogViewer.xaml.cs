using FoodpandaMenuImportLog.Dto;
using FoodpandaMenuImportLog.Model;
using FoodpandaMenuImportLog.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace FoodpandaMenuImportLog
{
    public partial class FoodpandaMenuImportLogViewer : Window
    {
        public FoodpandaMenuImportLogViewerViewModel ViewModel { get; set; }
        public string FoodpandaToken { get; set; }

        public FoodpandaMenuImportLogViewer()
        {
            if (DataContext == null)
            {
                DataContext = new FoodpandaMenuImportLogViewerViewModel();
            }

            ViewModel = DataContext as FoodpandaMenuImportLogViewerViewModel;
            LoadSettingFromJsonFile();

            InitializeComponent();
        }

        public static bool Request(
            out string result,
            out int statusCode,
            out WebHeaderCollection headers,
            string uri,
            string data,
            string contentType,
            string method,
            string authType = null,
            string authToken = null,
            NameValueCollection customizeHeaderList = null,
            string accept = null
        )
        {
            result = null;
            statusCode = 500;
            headers = null;

            byte[] dataBytes = data == null ? null : Encoding.UTF8.GetBytes(data);

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls
                 | SecurityProtocolType.Tls11
                 | SecurityProtocolType.Tls12;

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);

            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

            if (dataBytes != null) request.ContentLength = dataBytes.Length;

            request.ContentType = contentType;

            request.Method = method;

            if (accept != null) request.Accept = accept;

            if (authType != null && authToken != null)
            {
                if (string.IsNullOrWhiteSpace(authType)) authType = "Bearer";
                request.PreAuthenticate = true;
                request.Headers.Add("Authorization", $"{authType} " + authToken);
            }

            if (customizeHeaderList != null)
            {
                foreach (var tempKey in customizeHeaderList.AllKeys)
                {
                    request.Headers.Add(tempKey, customizeHeaderList[tempKey]);
                }
            }

            if (dataBytes != null)
            {
                using (Stream requestBody = request.GetRequestStream())
                {
                    requestBody.Write(dataBytes, 0, dataBytes.Length);
                }
            }

            HttpWebResponse response;

            try
            {
                using (response = (HttpWebResponse)request.GetResponse())
                using (Stream stream = response.GetResponseStream())
                using (StreamReader reader = new StreamReader(stream))
                {
                    statusCode = (int)response.StatusCode;
                    headers = response.Headers;
                    result = reader.ReadToEnd();
                }

                return true;
            }
            catch (WebException ex)
            {
                if (ex.Status == WebExceptionStatus.ProtocolError && ex.Response != null)
                {
                    HttpWebResponse resp = (HttpWebResponse)ex.Response;
                    statusCode = (int)resp.StatusCode;
                    headers = resp.Headers;
                    result = new StreamReader(ex.Response.GetResponseStream()).ReadToEnd();
                }

                return false;
            }
        }

        public void LoadSettingFromJsonFile()
        {
            try
            {
                string json = File.ReadAllText("foodpanda-menu-import-log-setting.json");
                SettingModel setting = JsonConvert.DeserializeObject<SettingModel>(json);

                ViewModel.UsernameInput = setting.username;
                ViewModel.PasswordInput = setting.password;
                ViewModel.ChainCodeInput = setting.chinCode;
                ViewModel.VendorIdInput = setting.vendorId;
                ViewModel.LimitInput = setting.limit;
            }
            catch (Exception _)
            {
            }
        }

        public void SaveSettingToJsonFile()
        {
            try
            {
                SettingModel setting = new SettingModel()
                {
                    username = ViewModel.UsernameInput,
                    password = ViewModel.PasswordInput,
                    chinCode = ViewModel.ChainCodeInput,
                    vendorId = ViewModel.VendorIdInput,
                    limit = ViewModel.LimitInput
                };
                string settingString = JsonConvert.SerializeObject(setting);
                File.WriteAllText("foodpanda-menu-import-log-setting.json", settingString);

            }
            catch (Exception _)
            {
            }
        }

        private void QueryButton_Click(object sender, RoutedEventArgs e)
        {
            SaveSettingToJsonFile();
            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += GetMenuImportLog_DoWork;
            worker.RunWorkerAsync();
        }

        private BackgroundWorker GenerateProcessBarWorker(string processBarLabel)
        {
            ViewModel.QueryProcessBarLabel = processBarLabel;
            ViewModel.QueryProcessBarValue = 0;
            BackgroundWorker worker = new BackgroundWorker();
            worker.WorkerSupportsCancellation = true;
            worker.DoWork += UpdateProcessBarValue_DoWork;
            return worker;
        }

        private async Task<string> GetFoodpandaToken()
        {
            BackgroundWorker getTokenProcessBarWorker = GenerateProcessBarWorker("Getting Foodpanda Token");
            getTokenProcessBarWorker.RunWorkerAsync();
            Dictionary<string, string> loginDataDictionary = new Dictionary<string, string>()
                                        {
                                            { "username",  ViewModel.UsernameInput },
                                            { "password", ViewModel.PasswordInput },
                                            { "grant_type", "client_credentials" }
                                        };
            string loginData = await new FormUrlEncodedContent(loginDataDictionary).ReadAsStringAsync();
            if (
                !Request(
                    out string tokenDataString,
                    out int statusCode,
                    out WebHeaderCollection _,
                    $"{ViewModel.ApiUrlInput}v2/login",
                    loginData,
                    "application/x-www-form-urlencoded",
                    "POST"
                )
            )
            {
                throw new Exception($"{statusCode} {tokenDataString}");
            }
            getTokenProcessBarWorker.CancelAsync();

            FoodpandaTokenDto tokenData = JsonConvert.DeserializeObject<FoodpandaTokenDto>(tokenDataString);
            return tokenData.access_token;
        }

        private void LoadMenuImportLogs(string foodpandaToken)
        {
            BackgroundWorker getMenuImportLogProcessBarWorker = GenerateProcessBarWorker("Getting Logs");
            getMenuImportLogProcessBarWorker.RunWorkerAsync();
            if (
                !Request(
                    out string queryDataString,
                    out int queryStatusCode,
                    out WebHeaderCollection queryResHeader,
                    $"{ViewModel.ApiUrlInput}v2/chains/{ViewModel.ChainCodeInput}/vendors/{ViewModel.VendorIdInput}/menu-import-logs?from={ViewModel.FormDateInput:yyyy-MM-dd}T00:00:00Z&to={ViewModel.ToDateInput:yyyy-MM-dd}T23:59:59Z&limit={ViewModel.LimitInput}",
                    null,
                    "application/x-www-form-urlencoded",
                    "GET",
                    "Bearer",
                    foodpandaToken
                )
            )
            {
                getMenuImportLogProcessBarWorker.CancelAsync();
                throw new Exception($"{queryStatusCode} {queryDataString}");
            }
            dynamic queryData = JsonConvert.DeserializeObject(queryDataString);
            ViewModel.MenuImportLogList = queryData[ViewModel.VendorIdInput].ToObject<List<FoodpandaMenuImportLogDto>>();

            getMenuImportLogProcessBarWorker.CancelAsync();
        }

        private async void GetMenuImportLog_DoWork(object sender, DoWorkEventArgs e)
        {
            ViewModel.QueryProcessBarVisibity = Visibility.Visible;

            try
            {
                if (string.IsNullOrWhiteSpace(FoodpandaToken))
                {
                    FoodpandaToken = await GetFoodpandaToken();
                }

                try
                {
                    LoadMenuImportLogs(FoodpandaToken);
                }
                catch (Exception ex)
                {
                    if (ex.Message.Contains("invalid token"))
                    {
                        // get new token and retry
                        FoodpandaToken = await GetFoodpandaToken();
                        LoadMenuImportLogs(FoodpandaToken);
                    }
                    else
                    {
                        throw ex;
                    }
                }
            }
            catch (Exception ex)
            {
                _ = MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            ViewModel.QueryProcessBarLabel = "Finished";
            ViewModel.QueryProcessBarValue = 100;
            Thread.Sleep(100);
            ViewModel.QueryProcessBarVisibity = Visibility.Collapsed;
        }

        private void UpdateProcessBarValue_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = (BackgroundWorker)sender;
            
            for (int i = 0; i < 99; i++)
            {
                if (worker.CancellationPending)
                {
                    e.Cancel = true;
                    return;
                }
                ViewModel.QueryProcessBarValue = i;
                Thread.Sleep(10);
            }
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            SaveSettingToJsonFile();
        }
    }
}
