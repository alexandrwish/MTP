using System;
using System.Threading.Tasks;

namespace MTP
{
    public class ItemDetailViewModel : BaseViewModel
    {
        public Item Item { get; }

        public Command DeleteItemCommand { get; }

        public ItemDetailViewModel(Item item = null)
        {
            if (item != null)
            {
                Title = item.Text;
                Item = item;
            }

            DeleteItemCommand = new Command(async () => await DeleteItemAsync());
        }

        private async Task<bool> DeleteItemAsync()
        {
            if (IsBusy || Item == null)
            {
                return false;
            }

            IsBusy = true;

            try
            {
                await DataStore.DeleteItemAsync(Item.Id);
            }
            catch (Exception a)
            {
                Console.WriteLine(a);
            }
            finally
            {
                IsBusy = false;
            }

            return false;
        }
    }
}