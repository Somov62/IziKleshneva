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
	public partial class Method6Page : ContentPage
	{
		public Method6Page ()
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
            for (int i = 1; i < stackParent.Children.Count;)
            {
                stackParent.Children.RemoveAt(i);
            }
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
            stack4.Children.Add(new Label() { Text = "x+h/2" });
            stackParentGrid.Children.Add(stack4, 3, 0);

            StackLayout stack5 = new StackLayout() { Margin = new Thickness(5), };
            stack5.Children.Add(new Label() { Text = "f(x;y)" });
            stackParentGrid.Children.Add(stack5, 4, 0);

            StackLayout stack6 = new StackLayout() { Margin = new Thickness(5), };
            stack6.Children.Add(new Label() { Text = "y+h/2*f(x;y)" });
            stackParentGrid.Children.Add(stack6, 5, 0);

            StackLayout stack7 = new StackLayout() { Margin = new Thickness(5), };
            stack7.Children.Add(new Label() { Text = "f(x+h/2;y+h/2*f(x;y))" });
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

                decimal xh2 = x + h2;
                decimal fXY = CalculateFormula(equlation, x, y, zCount);
                decimal yh2fXY = Math.Round(y + h2 * fXY, zCount);
                decimal fxh2yh2fxy = CalculateFormula(equlation, xh2, yh2fXY, zCount);
                decimal deltaY = Math.Round(hStep * fxh2yh2fxy, zCount);

                stack4.Children.Add(new Label() { Text = xh2.ToString() });
                stack5.Children.Add(new Label() { Text = fXY.ToString() });
                stack6.Children.Add(new Label() { Text = yh2fXY.ToString() });
                stack7.Children.Add(new Label() { Text = fxh2yh2fxy.ToString() });
                stack8.Children.Add(new Label() { Text = deltaY.ToString() });

                y += deltaY;
                x += hStep;
                counter++;
            }


            x = interval1;
            y = y0;

            stackParent.Children.Add(new Label() { Text = "Распишем первую строчку:", TextColor = Color.DeepPink, FontAttributes = FontAttributes.Bold });
            stackParent.Children.Add(new Frame() { HeightRequest = 1, BackgroundColor = Color.DeepPink, CornerRadius = 3, Padding = new Thickness(0), Margin = new Thickness(0, 10) });

            decimal Vxh2 = x + h2;
            stackParent.Children.Add(new Label() { Text = $"x + h/2 = {x} + {hStep}/2 = " + Vxh2, FontAttributes = FontAttributes.Bold });

            decimal VfXY = CalculateFormula(equlation, x, y, zCount);
            stackParent.Children.Add(new Label() { Text = "f(x; y) = " + txtEquation.Text + " = " + FormulaToCalculate(equlation, x, y) + " = " + VfXY, FontAttributes = FontAttributes.Bold });

            decimal Vyh2fXY = Math.Round(y + h2 * VfXY, zCount);
            stackParent.Children.Add(new Label() { Text = $"y + h/2 * f(x; y) = {y} + {hStep}/2 * {VfXY} = " + Vyh2fXY, FontAttributes = FontAttributes.Bold });

            decimal Vfxh2yh2fxy = CalculateFormula(equlation, Vxh2, Vyh2fXY, zCount);
            stackParent.Children.Add(new Label() { Text = $"f(x +h/2; y + h/2 * f(x; y)) = f({Vxh2}; {Vyh2fXY}) = " + FormulaToCalculate(equlation, Vxh2, Vyh2fXY) + " = " + Vfxh2yh2fxy, FontAttributes = FontAttributes.Bold });

            decimal VdeltaY = Math.Round(hStep * Vfxh2yh2fxy, zCount);
            stackParent.Children.Add(new Label() { Text = $"Δy = h * f(x +h/2; y + h/2 * f(x; y)) = {hStep} * {Vfxh2yh2fxy} = " + VdeltaY, FontAttributes = FontAttributes.Bold });

            stackParent.Children.Add(new Label() { Text = $"y₁ = y₀ + Δy₀ = {y} + {VdeltaY} = " + (y + VdeltaY), FontAttributes = FontAttributes.Bold });


            stackParent.Children.Add(new Frame() { HeightRequest = 1, BackgroundColor = Color.DeepPink, CornerRadius = 3, Padding = new Thickness(0), Margin = new Thickness(0, 10) });
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
            char[] symbols = { '^', '*', '/', '.', ',', '+', '=', '-', 'x', 'y', 'c', 's', 'o', 'i', 'n', 't', 'g', 'a', 's', 'q', 'r', 't', '(', ')' };
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