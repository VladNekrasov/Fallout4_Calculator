using Fallout4_Calculator.Views;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Fallout4_Calculator
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AppShell : Xamarin.Forms.Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(ChangeObjectPage), typeof(ChangeObjectPage));
            Routing.RegisterRoute(nameof(ShowObjectPage), typeof(ShowObjectPage));
            Routing.RegisterRoute(nameof(CreateObjectPage), typeof(CreateObjectPage));
            Routing.RegisterRoute(nameof(ChangeComponentPage), typeof(ChangeComponentPage));
            Routing.RegisterRoute(nameof(ShowComponentPage), typeof(ShowComponentPage));
            Routing.RegisterRoute(nameof(CreateComponentPage),typeof(CreateComponentPage));
            Routing.RegisterRoute(nameof(ChangeJunkPage), typeof(ChangeJunkPage));
            Routing.RegisterRoute(nameof(ShowJunkPage), typeof(ShowJunkPage));
            Routing.RegisterRoute(nameof(CreateJunkPage),typeof(CreateJunkPage));
        }
    }
}