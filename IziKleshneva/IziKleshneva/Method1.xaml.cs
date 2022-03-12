using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.Forms.Platform.Android;
using System.Globalization;

namespace IziKleshneva
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Method1 : ContentPage
    {
        private int _decimalCount = 4;
        private Dictionary<decimal, decimal> _decimalMemory;
        private Dictionary<int, string> _subscripts = new Dictionary<int, string>()
        {
            {1, "₁" },
            {2, "₂" },
            {3, "₃" },
            {4, "₄" },
            {5, "₅" },
            {6, "₆" },
            {7, "₇" },
            {8, "₈" },
            {9, "₉" },
            {0, "₀" }
        };
        public Method1()
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
            equlation = equlation.Replace(" ", "");
            if (!int.TryParse(txtZnaki.Text, out int zCount))
            {
                return;
            }
            if (zCount > 4) _decimalCount = zCount;
            else
            {
                txtZnaki.Text = 4.ToString();
                _decimalCount = 4;
            }
            if (!decimal.TryParse(txtInterval1.Text, out decimal interval1)
                | !decimal.TryParse(txtInterval2.Text, out decimal interval2)
                | !decimal.TryParse(txtEpsilon.Text, out decimal epsilon))
            {
                return;
            }

            while (step1Stack.Children.Count > 1)
            {
                step1Stack.Children.RemoveAt(1);
            }
            while (step2Stack.Children.Count > 1)
            {
                step2Stack.Children.RemoveAt(1);
            }
            while (step3Stack.Children.Count > 1)
            {
                step3Stack.Children.RemoveAt(1);
            }

            #region Step 1
            step1Stack.Children.Add(new Label() { Text = "f(x) = " + equlation });
            step1Stack.Children.Add(new Label() { Text = "a = " + interval1 });
            step1Stack.Children.Add(new Label() { Text = "b = " + interval2 });
            step1Stack.Children.Add(new Label() { Text = "Е = " + epsilon });
            #endregion

            #region Step 2
            _decimalMemory = new Dictionary<decimal, decimal>();
            bool IsСonditionComplete = Step2(equlation, interval1, interval2);
            if (!IsСonditionComplete) return;
            #endregion

            #region Step 3
            //C = a + |b - a|
            //        -------
            //           2


            decimal a = interval1;
            decimal b = interval2;
            decimal oldDelta = 0;
            int counterPovt = 0;
            int counterRoots = 1;
            while (true)
            {
                decimal c = decimal.Parse((a + (Math.Abs(b - a) / 2)).ToString());
                c = Math.Round(c, _decimalCount);
                decimal delta = decimal.Parse(Math.Abs(b - c).ToString());
                delta = Math.Round(delta, _decimalCount);
                if (oldDelta == delta)
                {
                    counterPovt++;
                    if (counterPovt > 3)
                    {
                        step3Stack.Children.Add(new Label() { Text = "Корень: " + c });
                        step3Stack.Children.Add(new Label() { Text = "Внимание! Точность не достигнута! Попробуйте увеличить количество знаков после запятой." });
                        return;
                    }
                }
                else
                {
                    oldDelta = delta;
                    counterPovt = 0;
                }
                step3Stack.Children.Add(new Label() { Text = $"C{GetSubscripts(counterRoots)} = {a} + |{b} - {a}| ÷ 2 = " + c });
                step3Stack.Children.Add(new Label() { Text = DeltaToView(c, b, epsilon) });
                if (delta <= epsilon)
                {
                    step3Stack.Children.Add(new Label() { Text = "Точность достигнута" });
                    step3Stack.Children.Add(new Label() { Text = "Корень: " + c });
                    break;
                }
                Label label = step3Stack.Children[step3Stack.Children.Count - 1] as Label;
                label.Text += " (Можно не писать)";
                label.TextColor = Color.Gray;
                step3Stack.Children.Add(new Label() { Text = $" ", Margin = new Thickness(0, -7) });
                string f1 = FormulaToView(equlation, a);
                string f2 = FormulaToView(equlation, c);
                string f3 = FormulaToView(equlation, b);
                if (CheckConvergenceCondition(f1, f2))
                {
                    StackLayout fstack = new StackLayout();
                    fstack.Orientation = StackOrientation.Horizontal;
                    fstack.Children.Add(new Label() { Text = f1.Remove(f1.Length - 3) });
                    fstack.Children.Add(new Label() { Text = f1.Substring(f1.Length - 3), TextColor = Color.LawnGreen, VerticalTextAlignment = TextAlignment.End, MinimumWidthRequest = 30 });
                    step3Stack.Children.Add(fstack);
                    fstack = new StackLayout();
                    fstack.Orientation = StackOrientation.Horizontal;
                    fstack.Children.Add(new Label() { Text = f2.Remove(f2.Length - 3) });
                    fstack.Children.Add(new Label() { Text = f2.Substring(f2.Length - 3), TextColor = Color.LawnGreen, VerticalTextAlignment = TextAlignment.End, MinimumWidthRequest = 30 });
                    step3Stack.Children.Add(fstack);
                    fstack = new StackLayout();
                    fstack.Orientation = StackOrientation.Horizontal;
                    fstack.Children.Add(new Label() { Text = f3.Remove(f3.Length - 3) });
                    fstack.Children.Add(new Label() { Text = f3.Substring(f3.Length - 3), TextColor = Color.Red, VerticalTextAlignment = TextAlignment.End, MinimumWidthRequest = 30 });
                    step3Stack.Children.Add(fstack);
                    b = c;
                }
                else
                {
                    StackLayout fstack = new StackLayout();
                    fstack.Orientation = StackOrientation.Horizontal;
                    fstack.Children.Add(new Label() { Text = f1.Remove(f1.Length - 3) });
                    fstack.Children.Add(new Label() { Text = f1.Substring(f1.Length - 3), TextColor = Color.Red, VerticalTextAlignment = TextAlignment.End, MinimumWidthRequest = 30 });
                    step3Stack.Children.Add(fstack);
                    fstack = new StackLayout();
                    fstack.Orientation = StackOrientation.Horizontal;
                    fstack.Children.Add(new Label() { Text = f2.Remove(f2.Length - 3) });
                    fstack.Children.Add(new Label() { Text = f2.Substring(f2.Length - 3), TextColor = Color.LawnGreen, VerticalTextAlignment = TextAlignment.End, MinimumWidthRequest = 30 });
                    step3Stack.Children.Add(fstack);
                    fstack = new StackLayout();
                    fstack.Orientation = StackOrientation.Horizontal;
                    fstack.Children.Add(new Label() { Text = f3.Remove(f3.Length - 3) });
                    fstack.Children.Add(new Label() { Text = f3.Substring(f3.Length - 3), TextColor = Color.LawnGreen, VerticalTextAlignment = TextAlignment.End, MinimumWidthRequest = 30 });
                    step3Stack.Children.Add(fstack);
                    a = c;
                }
                step3Stack.Children.Add(new Label() { Text = $"Следующий отрезок: [{a} ; {b}]" });
                step3Stack.Children.Add(new Frame() { HeightRequest = 1, BackgroundColor = Color.DeepPink, CornerRadius = 3, Padding = new Thickness(0), Margin = new Thickness(0, 10) });
                counterRoots++;
            }

            #endregion

        }

        private bool Step2(string equlation, decimal interval1, decimal interval2)
        {
            string f1 = FormulaToView(equlation, interval1);
            string f2 = FormulaToView(equlation, interval2);
            step2Stack.Children.Add(new Label() { Text = f1 });
            step2Stack.Children.Add(new Label() { Text = f2 });
            if (CheckConvergenceCondition(f1, f2))
            {
                step2Stack.Children.Add(new Label() { Text = "Условие на сходимость выполнено", FontSize = 18 });
                return true;
            }
            step2Stack.Children.Add(new Label() { Text = "Условие на сходимость не выполнено", FontSize = 18 });
            return false;
        }
        private string GetSubscripts(int number)
        {
            string result = string.Empty;
            while (number != 0)
            {
                result = result.Insert(0, _subscripts[number % 10]);
                number /= 10;
            }
            return result;
        }
        private decimal CalculateFormula(string equlation, decimal value)
        {
            if (_decimalMemory.TryGetValue(value, out decimal result))
            {
                return result;
            }
            string strvalue = value.ToString();
            if (value < 0) strvalue = value.ToString().Replace("-", "~");
            string primer = equlation.Replace("x", strvalue);
            decimal result1 = decimal.Parse(Calculator.SolvePrimer(primer));
            result1 = Math.Round(result1, _decimalCount);
            _decimalMemory.Add(value, result1);
            return result1;
        }
        private string FormulaToView(string equlation, decimal value)
        {
            string strvalue = value.ToString();
            string primer = equlation.Replace("x", strvalue);
            if (_decimalMemory.TryGetValue(value, out decimal result))
            {
                return $"f({value}) = {result} {(result < 0 ? "<" : ">")} 0";
            }
            decimal result1 = CalculateFormula(equlation, value);
            primer = primer.Replace("*", " × ").Replace("~", "-");
            primer = primer.Replace("+", " + ").Replace("^", " ^");
            result1 = Math.Round(result1, _decimalCount);
            primer += " = " + result1;
            return $"f({value}) = {primer} {(result1 < 0 ? "<" : ">")} 0";
        }

        private string DeltaToView(decimal c1, decimal c2, decimal epsilon)
        {
            decimal result1 = decimal.Parse(Math.Abs(c2 - c1).ToString());
            result1 = Math.Round(result1, _decimalCount);
            string sxod1 = "";
            if (result1 < epsilon) sxod1 = " < " + epsilon;
            if (result1 > epsilon) sxod1 = " > " + epsilon;
            if (result1 == epsilon) sxod1 = " = " + epsilon;
            return $"ΔС = |{c2} - {c1}| = " + result1 + sxod1;
        }

        private bool CheckConvergenceCondition(string f1, string f2)
        {
            if ((f1.Contains("<") && f2.Contains(">")) || (f1.Contains(">") && f2.Contains("<")))
            {
                return true;
            }
            return false;
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
            if (count > 4)
            {
                _decimalCount = count;
            }
            else
            {
                _decimalCount = 4;
                count = 4;
            }
            txtZnaki.Text = count.ToString();
        }
    }

}
