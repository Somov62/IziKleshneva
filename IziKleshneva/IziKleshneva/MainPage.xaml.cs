using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace IziKleshneva
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainPage : Shell
    {
        public MainPage()
        {
            InitializeComponent();
            this.FlyoutIsPresented = true;
        }

    }
}