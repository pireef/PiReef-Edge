using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using UWP_App.Models;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using System.Threading.Tasks;
using System.Net;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Newtonsoft.Json;
using System.Globalization;


// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace UWP_App.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Chart : Page
    {        public Chart()
        {
            this.InitializeComponent();
            this.DataContext = GetData();
        }

        private List<ScienceModuleData> GetData()
        {
            List<ScienceModuleData> _data = new List<ScienceModuleData>();
            string endpointurl = "https://iotreefdb.documents.azure.com:443/";
            string PrimaryKey = "6PCYc2qsWXNpYc0rW7xskXW3sOQ7vDqZ7bcbGoTeLNfAFhBJs6NM7QXa5MErM8eZEKTgPazXxRVY9XQEiHbhFA==";
            string sqlquery = "SELECT * from c WHERE c.IoTHub.ConnectionDeviceId = 'DevelopmentDevice' AND c.TimeRead >" + "'" + DateTime.Now.AddHours(-24) + "'" + " ORDER BY c.TimeRead DESC";

            DocumentClient client = new DocumentClient(new Uri(endpointurl), PrimaryKey, new ConnectionPolicy { EnableEndpointDiscovery = false });

            _data = client.CreateDocumentQuery<ScienceModuleData>(UriFactory.CreateDocumentCollectionUri("iotreefdata", "iotreefcoll"), sqlquery, new FeedOptions { EnableCrossPartitionQuery = true }).ToList();

            return _data;
        }
    }
}
