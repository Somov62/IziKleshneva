using System;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace IziKleshneva
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Method1Page : ContentPage
    {
        public Method1Page()
        {
            InitializeComponent();
        }
        private void Solve_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtEquation.Text)) return;
            string equlation = txtEquation.Text;
            if (!int.TryParse(txtZnaki.Text, out int zCount))
            {
                return;
            }
            if (!decimal.TryParse(txtInterval1.Text, out decimal interval1)
                | !decimal.TryParse(txtInterval2.Text, out decimal interval2)
                | !decimal.TryParse(txtEpsilon.Text, out decimal epsilon))
            {
                return;
            }
            Method1 method = new Method1();
            method.CountZnaks = zCount;
            step1Stack.Children.Clear();
            var stackLayout = method.Solve(equlation, interval1, interval2, epsilon);
            foreach (var item in stackLayout.Children.ToList())
            {
                step1Stack.Children.Add(item);
            }
        }

        private void txtEquation_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (e.NewTextValue.Length == 0) return;
            txtEquation.Text = e.NewTextValue;
            char[] symbols = { '^', '*', '/', '.', ',', '+', '=', '-', 'x' };
            for (int i = 0; i < txtEquation.Text.Length; i++)
            {
                if (char.IsDigit(txtEquation.Text[i])) continue;
                if (symbols.Contains(txtEquation.Text[i])) continue;
                txtEquation.Text = txtEquation.Text.Remove(i);
            }
        }

        private void txtEpsilon_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txtEpsilon.Text.Length < 3) return;
            txtEpsilon.Text = txtEpsilon.Text.Replace(".", ",");
            if (txtEpsilon.Text.IndexOf(",") == -1) return;
            int count = txtEpsilon.Text.Substring(txtEpsilon.Text.IndexOf(",") + 1).Length;
            txtZnaki.Text = count.ToString();
            if (count < 4 || count > 15)
                txtZnaki.Text = 4.ToString();
        }

        private void OpenFlyout_Click(object sender, EventArgs e)
        {
            Shell.Current.FlyoutIsPresented = true;
        }
    }

}
