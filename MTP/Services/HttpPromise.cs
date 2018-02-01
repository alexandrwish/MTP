using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Plugin.Connectivity;

namespace MTP
{
    public abstract class HttpPromise : TaskCompletionSource<string>
    {
        protected readonly CloudDataStore Store;

        protected HttpPromise(CloudDataStore store)
        {
            Store = store;
        }

        public abstract void Exec();
    }

    internal class LoginTask : HttpPromise
    {
        private readonly LoginRecord _record;

        public LoginTask(CloudDataStore store, LoginRecord record) : base(store)
        {
            _record = record;
        }

        public override async void Exec()
        {
            if (!CrossConnectivity.Current.IsConnected || Store.Certificate == null)
            {
                SetResult(null);
                return;
            }

            var request = (HttpWebRequest) WebRequest.Create(App.BackendUrl + "api/login");
            request.ClientCertificates.Add(Store.Certificate);
            request.ContentType = "application/json";
            request.Method = "POST";
            var jsonString = JsonConvert.SerializeObject(_record);
            using (var streamWriter = new StreamWriter(request.GetRequestStream()))
            {
                streamWriter.Write(jsonString);
                streamWriter.Flush();
                streamWriter.Close();

                var httpResponse = await request.GetResponseAsync();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var readToEnd = streamReader.ReadToEnd();
                    SetResult(readToEnd);
                }
            }
        }
    }

    internal class GetItemsTask : HttpPromise
    {
        public GetItemsTask(CloudDataStore store) : base(store)
        {
        }

        public override async void Exec()
        {
            if (!CrossConnectivity.Current.IsConnected || Store.Certificate == null)
            {
                SetResult(null);
                return;
            }

            var request = (HttpWebRequest) WebRequest.Create(App.BackendUrl + "api/item/");
            request.ClientCertificates.Add(Store.Certificate);
            var httpResponse = await request.GetResponseAsync();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                SetResult(streamReader.ReadToEnd());
            }
        }
    }

    internal class GetItemTask : HttpPromise
    {
        private readonly string _id;

        public GetItemTask(CloudDataStore store, string id) : base(store)
        {
            _id = id;
        }

        public override async void Exec()
        {
            if (!CrossConnectivity.Current.IsConnected || Store.Certificate == null || _id == null)
            {
                SetResult(null);
                return;
            }

            var request = (HttpWebRequest) WebRequest.Create(App.BackendUrl + "api/item/" + _id);
            request.ClientCertificates.Add(Store.Certificate);
            var httpResponse = await request.GetResponseAsync();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                SetResult(streamReader.ReadToEnd());
            }
        }
    }

    internal class AddItemTask : HttpPromise
    {
        private readonly Item _item;

        public AddItemTask(CloudDataStore store, Item item) : base(store)
        {
            _item = item;
        }

        public override async void Exec()
        {
            if (!CrossConnectivity.Current.IsConnected || Store.Certificate == null || _item == null)
            {
                SetResult(null);
                return;
            }

            var request = (HttpWebRequest) WebRequest.Create(App.BackendUrl + "api/item");
            request.ClientCertificates.Add(Store.Certificate);
            request.Method = "POST";
            request.ContentType = "application/json";
            var jsonString = JsonConvert.SerializeObject(_item);
            using (var streamWriter = new StreamWriter(request.GetRequestStream()))
            {
                streamWriter.Write(jsonString);
                streamWriter.Flush();
                streamWriter.Close();

                var httpResponse = await request.GetResponseAsync();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    SetResult(streamReader.ReadToEnd());
                }
            }
        }
    }

    internal class UpdateItemTask : HttpPromise
    {
        private readonly Item _item;

        public UpdateItemTask(CloudDataStore store, Item item) : base(store)
        {
            _item = item;
        }

        public override async void Exec()
        {
            if (!CrossConnectivity.Current.IsConnected || Store.Certificate == null || _item?.Id == null)
            {
                SetResult(null);
                return;
            }

            try
            {
                var request = (HttpWebRequest) WebRequest.Create(App.BackendUrl + "api/item/" + _item.Id);
                request.ClientCertificates.Add(Store.Certificate);
                request.Method = "PUT";
                request.ContentType = "application/json";
                var jsonString = JsonConvert.SerializeObject(_item);
                using (var streamWriter = new StreamWriter(request.GetRequestStream()))
                {
                    streamWriter.Write(jsonString);
                    streamWriter.Flush();
                    streamWriter.Close();

                    var httpResponse = await request.GetResponseAsync();
                    using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                    {
                        SetResult(streamReader.ReadToEnd());
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                SetResult(null);
            }
        }
    }

    internal class DeleteItemTask : HttpPromise
    {
        private readonly string _id;

        public DeleteItemTask(CloudDataStore store, string id) : base(store)
        {
            _id = id;
        }

        public override async void Exec()
        {
            if (!CrossConnectivity.Current.IsConnected || Store.Certificate == null || _id == null)
            {
                SetResult(null);
                return;
            }

            var request = (HttpWebRequest) WebRequest.Create(App.BackendUrl + "api/item/" + _id);
            request.ClientCertificates.Add(Store.Certificate);
            request.Method = "DELETE";
            var httpResponse = await request.GetResponseAsync();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                SetResult(streamReader.ReadToEnd());
            }
        }
    }
}