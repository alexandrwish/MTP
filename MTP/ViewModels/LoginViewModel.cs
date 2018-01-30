using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MTP
{
    public class LoginViewModel : BaseViewModel
    {
        public Command LoginCommand { get; set; }

        public LoginViewModel()
        {
            LoginCommand = new Command<LoginRecord>(async (login) => await Login(login));
        }

        private async Task Login(LoginRecord login)
        {
            if (IsBusy)
                return;

            IsBusy = true;
            try
            {
                var success = await DataStore.LoginAsync(login);
                if (success)
                {
                    
                }
                else
                {
                    
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}