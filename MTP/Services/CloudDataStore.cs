using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using MTP.Services;
using Newtonsoft.Json;

namespace MTP
{
    public class CloudDataStore : IDataStore<Item>
    {
        private List<HttpPromise> Queuly { get; }
        private readonly IEnumerable<Item> _items;
        public X509Certificate Certificate { get; set; }

        public CloudDataStore()
        {
            _items = new List<Item>();
            Queuly = new List<HttpPromise>();
            Init();
        }

        public void Init()
        {
            CloudDataStoreExtension.Extend(this, async () =>
            {
                foreach (var task in Queuly.ToList())
                {
                    var success = await task.Exec();
                    Queuly.Remove(task);
                    if (!success)
                    {
                        break;
                    }
                }
            });
        }

        public async Task<bool> LoginAsync(LoginRecord record)
        {
            var task = new LoginTask(this, record);
            if (Certificate == null)
            {
                Queuly.Add(task);
                Init();
            }
            else
            {
                await task.Exec();
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
                Queuly.Add(task);
                Init();
            }
            else
            {
                await task.Exec();
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
                Queuly.Add(task);
                Init();
            }
            else
            {
                await task.Exec();
            }

            var result = await task.Task;
            return await Task.FromResult(result != null ? JsonConvert.DeserializeObject<Item>(result) : null);
        }

        public async Task<bool> AddItemAsync(Item item)
        {
            var task = new AddItemTask(this, item);
            if (Certificate == null)
            {
                Queuly.Add(task);
                Init();
            }
            else
            {
                await task.Exec();
            }

            var result = await task.Task;
            return await Task.FromResult(result != null && JsonConvert.DeserializeObject<bool>(result));
        }

        public async Task<bool> UpdateItemAsync(Item item)
        {
            var task = new UpdateItemTask(this, item);
            if (Certificate == null)
            {
                Queuly.Add(task);
                Init();
            }
            else
            {
                await task.Exec();
            }

            var result = await task.Task;
            return await Task.FromResult(result != null && JsonConvert.DeserializeObject<bool>(result));
        }

        public async Task<bool> DeleteItemAsync(string id)
        {
            var task = new DeleteItemTask(this, id);
            if (Certificate == null)
            {
                Queuly.Add(task);
                Init();
            }
            else
            {
                await task.Exec();
            }

            var result = await task.Task;
            return await Task.FromResult(result != null && JsonConvert.DeserializeObject<bool>(result));
        }

        public void RemoveCertificate()
        {
            CloudDataStoreExtension.Revert(this);
        }

        public bool IsValidCertificate()
        {
            var dateTime = DateTime.Parse(Certificate.GetExpirationDateString());
            var now = DateTime.Now;
            return dateTime.CompareTo(now) > 0;
        }
    }
}