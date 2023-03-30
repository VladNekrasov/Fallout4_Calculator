using System.Threading.Tasks;
using MvvmHelpers;
using MvvmHelpers.Commands;
using Xamarin.Essentials;

namespace Fallout4_Calculator.ViewModels
{
    public class AboutViewModel : BaseViewModel
    {
        public AsyncCommand OpenWebCommand { get; }
        public AboutViewModel()
        {
            OpenWebCommand = new AsyncCommand(OpenWeb);
        }
        async Task OpenWeb()
        {
            await Browser.OpenAsync("https://vk.com/id483146293");
        }
    }
}
