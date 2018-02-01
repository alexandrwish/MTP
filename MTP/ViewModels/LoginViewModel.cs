using System;
using System.Threading.Tasks;
using MTP.ViewModel;

namespace MTP
{
    public class LoginViewModel : BaseViewModel
    {
        public Command LoginCommand { get; }

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
                LoginViewModelExtension.ExtendLogin(await DataStore.LoginAsync(login), () =>
                    {
                        DataStore.RemoveCertificate();
                    });
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