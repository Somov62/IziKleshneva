using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace IziKleshneva
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    //Метод хорд
    public partial class Method2Page : ContentPage
    {
        public Method2Page()
        {
            InitializeComponent();
        }

        private void Solve_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtEquation.Text)) return;
            string equlation = txtEquation.Text.Replace("=0", "");
            for (int i = 0; i < equlation.Length; i++)
            {
                if (equlation[i] == 'x')
                {
                    try
                    {
                        if (char.IsDigit(equlation[i - 1])) equlation = equlation.Insert(i, "*");
                    }
                    catch { }
                }
            }
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

            Method2 method = new Method2();
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
            if (txtEpsilon.Text.IndexOf(",") == -1) return;
            int count = txtEpsilon.Text.Substring(txtEpsilon.Text.IndexOf(",") + 1).Length;
            if (count < 4 || count > 15)
                txtZnaki.Text = 4.ToString();
        }
    }
}