using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using MTP.Services;
using Newtonsoft.Json;

namespace MTP
{
    public class CloudDataStore : IDataStore<Item>
    {
        private bool _isBusy;
        private readonly IEnumerable<Item> _items;
        private readonly List<HttpPromise> _queuly;
        public X509Certificate Certificate { get; set; }

        public CloudDataStore()
        {
            _items = new List<Item>();
            _queuly = new List<HttpPromise>();
            Init();
        }

        private void Init()
        {
            if (_isBusy) return;
            _isBusy = true;
            CloudDataStoreExtension.Extend(this, () =>
            {
                _isBusy = false;
                foreach (var task in _queuly)
                {
                    task.Exec();
                }
            });
        }

        public async Task<bool> LoginAsync(LoginRecord record)
        {
            var task = new LoginTask(this, record);
            if (Certificate == null)
            {
                _queuly.Add(task);
                Init();
            }
            else
            {
                task.Exec();
            }

            var result = await task.Task;
            return await Task.FromResult(result != null &&
                                         !JsonConvert.DeserializeObject<LoginResponceRecord>(result).Error);
        }

        public async Task<IEnumerable<Item>> GetItemsAsync(bool forceRefresh = false)
        {
            var task = new GetItemsTask(this);
            if (Certificate == null)
            {
                _queuly.Add(task);
                Init();
            }
            else
            {
                task.Exec();
            }

            var result = await task.Task;
            return await Task.FromResult(result != null
                ? JsonConvert.DeserializeObject<IEnumerable<Item>>(result)
                : _items);
        }

        public async Task<Item> GetItemAsync(string id)
        {
            var task = new GetItemTask(this, id);
            if (Certificate == null)
            {
                _queuly.Add(task);
                Init();
            }
            else
            {
                task.Exec();
            }

            var result = await task.Task;
            return await Task.FromResult(result != null ? JsonConvert.DeserializeObject<Item>(result) : null);
        }

        public async Task<bool> AddItemAsync(Item item)
        {
            var task = new AddItemTask(this, item);
            if (Certificate == null)
            {
                _queuly.Add(task);
                Init();
            }
            else
            {
                task.Exec();
            }

            var result = await task.Task;
            return await Task.FromResult(result != null && JsonConvert.DeserializeObject<bool>(result));
        }

        public async Task<bool> UpdateItemAsync(Item item)
        {
            var task = new UpdateItemTask(this, item);
            if (Certificate == null)
            {
                _queuly.Add(task);
                Init();
            }
            else
            {
                task.Exec();
            }

            var result = await task.Task;
            return await Task.FromResult(result != null && JsonConvert.DeserializeObject<bool>(result));
        }

        public async Task<bool> DeleteItemAsync(string id)
        {
            var task = new DeleteItemTask(this, id);
            if (Certificate == null)
            {
                _queuly.Add(task);
                Init();
            }
            else
            {
                task.Exec();
            }

            var result = await task.Task;
            return await Task.FromResult(result != null && JsonConvert.DeserializeObject<bool>(result));
        }

        public void RemoveCertificate()
        {
            CloudDataStoreExtension.Revert(this);
        }
    }
}