using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MTP
{
    public class MockDataStore : IDataStore<Item>
    {
        private readonly List<Item> _items;

        public MockDataStore()
        {
            _items = new List<Item>();
            var items = new List<Item>
            {
                new Item
                {
                    Id = Guid.NewGuid().ToString(),
                    Text = "First item",
                    Description = "This is a nice description"
                },
                new Item
                {
                    Id = Guid.NewGuid().ToString(),
                    Text = "Second item",
                    Description = "This is a nice description"
                },
                new Item
                {
                    Id = Guid.NewGuid().ToString(),
                    Text = "Third item",
                    Description = "This is a nice description"
                },
                new Item
                {
                    Id = Guid.NewGuid().ToString(),
                    Text = "Fourth item",
                    Description = "This is a nice description"
                },
                new Item
                {
                    Id = Guid.NewGuid().ToString(),
                    Text = "Fifth item",
                    Description = "This is a nice description"
                },
                new Item
                {
                    Id = Guid.NewGuid().ToString(),
                    Text = "Sixth item",
                    Description = "This is a nice description"
                },
            };

            foreach (Item item in items)
            {
                _items.Add(item);
            }
        }

        public async Task<bool> AddItemAsync(Item item)
        {
            _items.Add(item);

            return await Task.FromResult(true);
        }

        public async Task<bool> UpdateItemAsync(Item item)
        {
            var i = _items.FirstOrDefault(arg => arg.Id == item.Id);
            _items.Remove(i);
            _items.Add(item);

            return await Task.FromResult(true);
        }

        public async Task<bool> DeleteItemAsync(string id)
        {
            var item = _items.FirstOrDefault(arg => arg.Id == id);
            _items.Remove(item);

            return await Task.FromResult(true);
        }

        public async Task<Item> GetItemAsync(string id)
        {
            return await Task.FromResult(_items.FirstOrDefault(s => s.Id == id));
        }

        public async Task<IEnumerable<Item>> GetItemsAsync(bool forceRefresh = false)
        {
            return await Task.FromResult(_items);
        }

        public async Task<bool> LoginAsync(LoginRecord record)
        {
            return await Task.FromResult(false);
        }

        public void RemoveCertificate()
        {
        }
    }
}