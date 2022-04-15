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
    public partial class Method7Page : ContentPage
    {
        public Method7Page()
        {
            InitializeComponent();
        }
        private void Solve_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtEquation.Text)) return;
            string equlation = txtEquation.Text;
            for (int i = 0; i < equlation.Length; i++)
            {
                if (equlation[i] == 'x' || equlation[i] == 'y' || equlation[i] == 'c' || equlation[i] == 't' || equlation[i] == 's')
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
                | !decimal.TryParse(txtHStep.Text, out decimal hStep)
                | !decimal.TryParse(txtY0.Text, out decimal y0))
            {
                return;
            }
            stackParentGrid.Children.Clear();
            stackFormula.Children.Clear();
            for (int i = 2; i < stackParent.Children.Count;)
            {
                stackParent.Children.RemoveAt(i);
            }

            stackFormula.Children.Add(new Label() { Text = "Формулы:", TextColor = Color.DeepPink, FontAttributes = FontAttributes.Bold });
            stackFormula.Children.Add(new Frame() { HeightRequest = 1, BackgroundColor = Color.DeepPink, CornerRadius = 3, Padding = new Thickness(0), Margin = new Thickness(0, 10) });
            stackFormula.Children.Add(new Label() { Text = "Δy = h/6 * (k₁ + 2k₂ + 2k₃ + k₄)" });
            stackFormula.Children.Add(new Label() { Text = "k₁ = f(x; y)" });
            stackFormula.Children.Add(new Label() { Text = "k₂ = f(x + h/2; y + h*k₁/2)" });
            stackFormula.Children.Add(new Label() { Text = "k₃ = f(x + h/2; y + h*k₂/2)" });
            stackFormula.Children.Add(new Label() { Text = "k₄ = f(x + h; y + h*k₃)" });
            stackFormula.Children.Add(new Frame() { HeightRequest = 1, BackgroundColor = Color.DeepPink, CornerRadius = 3, Padding = new Thickness(0), Margin = new Thickness(0, 10) });
            stackFormula.Children.Add(new Label());

            #region ShowTableHeader
            StackLayout stack1 = new StackLayout() { Margin = new Thickness(5), };
            stack1.Children.Add(new Label() { Text = "i" });
            stackParentGrid.Children.Add(stack1, 0, 0);

            StackLayout stack2 = new StackLayout() { Margin = new Thickness(5), };
            stack2.Children.Add(new Label() { Text = "x" });
            stackParentGrid.Children.Add(stack2, 1, 0);

            StackLayout stack3 = new StackLayout() { Margin = new Thickness(5), };
            stack3.Children.Add(new Label() { Text = "y" });
            stackParentGrid.Children.Add(stack3, 2, 0);

            StackLayout stack4 = new StackLayout() { Margin = new Thickness(5), };
            stack4.Children.Add(new Label() { Text = "k₁" });
            stackParentGrid.Children.Add(stack4, 3, 0);

            StackLayout stack5 = new StackLayout() { Margin = new Thickness(5), };
            stack5.Children.Add(new Label() { Text = "k₂" });
            stackParentGrid.Children.Add(stack5, 4, 0);

            StackLayout stack6 = new StackLayout() { Margin = new Thickness(5), };
            stack6.Children.Add(new Label() { Text = "k₃" });
            stackParentGrid.Children.Add(stack6, 5, 0);

            StackLayout stack7 = new StackLayout() { Margin = new Thickness(5), };
            stack7.Children.Add(new Label() { Text = "k₄" });
            stackParentGrid.Children.Add(stack7, 6, 0);

            StackLayout stack8 = new StackLayout() { Margin = new Thickness(5), };
            stack8.Children.Add(new Label() { Text = "Δy" });
            stackParentGrid.Children.Add(stack8, 7, 0);
            #endregion

            int counter = 0;
            decimal x = interval1;
            decimal y = y0;
            decimal h2 = Math.Round(hStep / 2, zCount);

            while (x <= interval2)
            {
                stack1.Children.Add(new Label() { Text = counter.ToString() });
                stack2.Children.Add(new Label() { Text = x.ToString() });
                stack3.Children.Add(new Label() { Text = y.ToString() });
                if (x == interval2) break;

                decimal k1 = CalculateFormula(equlation, x, y, zCount);
                decimal k2 = CalculateFormula(equlation, x + h2, y + Math.Round(h2 * k1, zCount), zCount);
                decimal k3 = CalculateFormula(equlation, x + h2, y + Math.Round(h2 * k2, zCount), zCount);
                decimal k4 = CalculateFormula(equlation, x + hStep, y + Math.Round(hStep * k3, zCount), zCount);
                decimal deltaY = Math.Round(hStep / 6 * (k1 + 2 * k2 + 2 * k3 + k4), zCount);

                stack4.Children.Add(new Label() { Text = k1.ToString() });
                stack5.Children.Add(new Label() { Text = k2.ToString() });
                stack6.Children.Add(new Label() { Text = k3.ToString() });
                stack7.Children.Add(new Label() { Text = k4.ToString() });
                stack8.Children.Add(new Label() { Text = deltaY.ToString() });

                y += deltaY;
                x += hStep;
                counter++;
            }

            stackParent.Children.Add(new Label());
            stackParent.Children.Add(new Label());
        }

        private decimal CalculateFormula(string equlation, decimal x, decimal y, int countZnaks)
        {
            string primer = FormulaToCalculate(equlation, x, y);

            decimal result1 = decimal.Parse(Calculator.SolvePrimer(primer));
            result1 = Math.Round(result1, countZnaks);

            return result1;
        }

        private string FormulaToCalculate(string equlation, decimal x, decimal y)
        {
            string strX = x.ToString();
            if (x < 0) strX = x.ToString().Replace("-", "~");
            string primer = equlation.Replace("x", strX);

            string strY = y.ToString();
            if (y < 0) strY = y.ToString().Replace("-", "~");
            primer = primer.Replace("y", strY);
            return primer;
        }
        private void txtEquation_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (e.NewTextValue.Length == 0) return;
            txtEquation.Text = e.NewTextValue;
            char[] symbols = { '^', '*', '/', '.', ',', '+', '=', '-', 'x', 'y', 'c', 's', 'o', 'i', 'n', 't', 'g', 'a', '(', ')' };
            for (int i = 0; i < txtEquation.Text.Length; i++)
            {
                if (char.IsDigit(txtEquation.Text[i])) continue;
                if (symbols.Contains(txtEquation.Text[i])) continue;
                txtEquation.Text = txtEquation.Text.Remove(i);
            }
        }

        private void txtEpsilon_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txtY0.Text.Length < 3) return;
            if (txtY0.Text.IndexOf(",") == -1) return;
            int count = txtY0.Text.Substring(txtY0.Text.IndexOf(",") + 1).Length;
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