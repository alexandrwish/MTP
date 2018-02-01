using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;

namespace MTP
{
    public class ItemsViewModel : BaseViewModel
    {
        public Command AddItemCommand { get; }
        public Command LoadItemsCommand { get; }
        public Command UpdateItemComand { get; }
        public ObservableCollection<Item> Items { get; }

        public ItemsViewModel()
        {
            Title = "Browse";
            Items = new ObservableCollection<Item>();
            AddItemCommand = new Command<Item>(async item => await AddItem(item));
            UpdateItemComand = new Command<Item>(async item => await UpdateItem(item));
            LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand());
        }

        private async Task ExecuteLoadItemsCommand()
        {
            if (IsBusy)
                return;

            IsBusy = true;

            try
            {
                Items.Clear();
                var items = await DataStore.GetItemsAsync(true);
                foreach (var item in items)
                {
                    Items.Add(item);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task AddItem(Item item)
        {
            Items.Add(item);
            await DataStore.AddItemAsync(item);
        }

        private async Task UpdateItem(Item item)
        {
            Item tmp = null;
            foreach (var i in Items)
            {
                if (i.Id != item.Id) continue;
                tmp = i;
                break;
            }

            if (tmp != null)
            {
                Items.Remove(tmp);
                Items.Add(item);
                await DataStore.UpdateItemAsync(item);
            }
        }
    }
}