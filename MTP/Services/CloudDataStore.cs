using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using MTP.Services;
using Newtonsoft.Json;
using Plugin.Connectivity;

namespace MTP
{
    public class CloudDataStore : IDataStore<Item>
    {
        public X509Certificate Certificate { private get; set; }
        private IEnumerable<Item> _items;

        public CloudDataStore()
        {
            CloudDataStoreExtension.Extend(this);
            _items = new List<Item>();
        }

        public async Task<bool> LoginAsync(LoginRecord record)
        {
            return true;
        }

        public async Task<IEnumerable<Item>> GetItemsAsync(bool forceRefresh = false)
        {
            var t = await Task<Task<IEnumerable<Item>>>.Factory.StartNew(async () =>
            {
                SpinWait.SpinUntil(() => Certificate != null);
                if (!forceRefresh || !CrossConnectivity.Current.IsConnected) return _items;
                string result;
                var request = (HttpWebRequest) WebRequest.Create(App.BackendUrl + "api/item");
                if (Certificate != null)
                {
                    request.ClientCertificates.Add(Certificate);
                }

                var httpResponse = await request.GetResponseAsync();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    result = streamReader.ReadToEnd();
                }

                _items = await Task.Run(() => JsonConvert.DeserializeObject<IEnumerable<Item>>(result));

                return _items;
            });
            t.Wait();
            return t.Result;
        }

        public async Task<Item> GetItemAsync(string id)
        {
            if (id == null || !CrossConnectivity.Current.IsConnected) return null;
            string result;
            var request = (HttpWebRequest) WebRequest.Create(App.BackendUrl + "api/item/" + id);
            if (Certificate != null)
            {
                request.ClientCertificates.Add(Certificate);
            }

            var httpResponse = await request.GetResponseAsync();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                result = streamReader.ReadToEnd();
            }

            return await Task.Run(() => JsonConvert.DeserializeObject<Item>(result));
        }

        public async Task<bool> AddItemAsync(Item item)
        {
            if (item == null || !CrossConnectivity.Current.IsConnected)
                return false;

            var request = (HttpWebRequest) WebRequest.Create(App.BackendUrl + "api/item");
            if (Certificate != null)
            {
                request.ClientCertificates.Add(Certificate);
            }

            request.Method = "POST";
            request.ContentType = "application/json";
            var jsonString = JsonConvert.SerializeObject(item);
            using (var streamWriter = new StreamWriter(request.GetRequestStream()))
            {
                streamWriter.Write(jsonString);
                streamWriter.Flush();
                streamWriter.Close();

                var httpResponse = await request.GetResponseAsync();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    return await Task.Run(() => JsonConvert.DeserializeObject<bool>(result));
                }
            }
        }

        public async Task<bool> UpdateItemAsync(Item item)
        {
            if (item?.Id == null || !CrossConnectivity.Current.IsConnected)
                return false;

            var request = (HttpWebRequest) WebRequest.Create(App.BackendUrl + "api/item/" + item.Id);
            if (Certificate != null)
            {
                request.ClientCertificates.Add(Certificate);
            }

            request.Method = "PUT";
            request.ContentType = "application/json";
            var jsonString = JsonConvert.SerializeObject(item);
            using (var streamWriter = new StreamWriter(request.GetRequestStream()))
            {
                streamWriter.Write(jsonString);
                streamWriter.Flush();
                streamWriter.Close();

                var httpResponse = await request.GetResponseAsync();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    return await Task.Run(() => JsonConvert.DeserializeObject<bool>(result));
                }
            }
        }

        public async Task<bool> DeleteItemAsync(string id)
        {
            if (id == null || !CrossConnectivity.Current.IsConnected) return false;
            string result;
            var request = (HttpWebRequest) WebRequest.Create(App.BackendUrl + "api/item/" + id);
            if (Certificate != null)
            {
                request.ClientCertificates.Add(Certificate);
            }

            request.Method = "DELETE";
            var httpResponse = await request.GetResponseAsync();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                result = streamReader.ReadToEnd();
            }

            return await Task.Run(() => JsonConvert.DeserializeObject<bool>(result));
        }
    }
}