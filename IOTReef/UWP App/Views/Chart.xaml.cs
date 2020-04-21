using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UWP_App.Models;
using Windows.UI.Xaml.Controls;


// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace UWP_App.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Chart : Page
    {
        public Chart()
        {
            this.InitializeComponent();
            GetDataAsync();
        }

        private async Task<List<ScienceModuleData>> GetDataAsync()
        {
            //WE need to get this async 
            List<ScienceModuleData> _data = new List<ScienceModuleData>();
            string endpointurl = "https://iotreefdb.documents.azure.com:443/";
            string PrimaryKey = "6PCYc2qsWXNpYc0rW7xskXW3sOQ7vDqZ7bcbGoTeLNfAFhBJs6NM7QXa5MErM8eZEKTgPazXxRVY9XQEiHbhFA==";
            string sqlquery = "SELECT * from c WHERE c.IoTHub.ConnectionDeviceId = 'DevelopmentDevice' AND c.TimeRead >" + "'" + DateTime.Now.AddHours(-24) + "'" + " ORDER BY c.TimeRead DESC";

            //DocumentClient client = new DocumentClient(new Uri(endpointurl), PrimaryKey, new ConnectionPolicy { EnableEndpointDiscovery = false });

            //_data = client.CreateDocumentQuery<ScienceModuleData>(UriFactory.CreateDocumentCollectionUri("iotreefdata", "iotreefcoll"), sqlquery, new FeedOptions { EnableCrossPartitionQuery = true }).ToList();

            using (var client = new DocumentClient(new Uri(endpointurl), PrimaryKey, new ConnectionPolicy { EnableEndpointDiscovery = false }))
            {
                var prop = client.CreateDocumentQuery<ScienceModuleData>
                    (UriFactory.CreateDocumentCollectionUri("iotreefdata", "iotreefcoll"),
                    sqlquery,
                    new FeedOptions { EnableCrossPartitionQuery = true }).AsDocumentQuery();

                while (prop.HasMoreResults)
                {
                    foreach (ScienceModuleData smd in await prop.ExecuteNextAsync<ScienceModuleData>())
                    {
                        _data.Add(smd);
                    }
                }
            }

            this.DataContext = _data;
            return _data;
        }
    }
}
