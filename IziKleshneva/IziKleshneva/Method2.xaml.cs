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
    public partial class Method2 : ContentPage
    {
        private int _decimalCount = 4;
        public Method2()
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
            step1Stack.Children.Add(new Label() { Text = "f(x) = " + txtEquation.Text.Replace(" ", "").Replace("=0", "") });
            step1Stack.Children.Add(new Label() { Text = "a = " + interval1 });
            step1Stack.Children.Add(new Label() { Text = "b = " + interval2 });
            step1Stack.Children.Add(new Label() { Text = "Е = " + epsilon });
            #endregion
            #region Step 2
            bool IsСonditionComplete = Step2(equlation, interval1, interval2);
            if (!IsСonditionComplete) return;
            #endregion


            #region Step 3
            //           f(a)
            //Ψ = a - --------- * (b - a)
            //        f(b)-f(a)


            decimal a = interval1;
            decimal b = interval2;

            step3Stack.Children.Add(new Label() { Text = $"Формула: Ψ = a - f(a) ÷ (f(b)-f(a)) × (b - a) "});
            step3Stack.Children.Add(new Label() { Text = $"Справка: Ψ - знак пси; знак ÷ в виде дробной черты"});
            step3Stack.Children.Add(new Label() { Text = $" "});

            decimal oldDelta = 0;
            int counterPovt = 0;
            while (true)
            {
                decimal fa = CalculateFormula(equlation, a); 
                decimal fb = CalculateFormula(equlation, b); 
                decimal c = decimal.Parse((a - fa / (fb - fa) * (b - a)).ToString());
                c = Math.Round(c, _decimalCount);
                step3Stack.Children.Add(new Label() { Text = $"Ψ = {a} - {fa} ÷ ({fb} - {fa}) × ({b} - {a}) ≈ " + c });

                decimal delta = decimal.Parse(Math.Abs(c - a).ToString());
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
                step3Stack.Children.Add(new Label() { Text = DeltaToView(a, c, epsilon) });
                if (delta <= epsilon)
                {
                    step3Stack.Children.Add(new Label() { Text = "Точность достигнута" });
                    step3Stack.Children.Add(new Label() { Text = "Корень: " + c });
                    break;
                }
                Label label = step3Stack.Children[step3Stack.Children.Count - 1] as Label;
                label.Text += " (Можно не писать)";
                label.TextColor = Color.Gray;
                string f1 = FormulaToView(equlation, a);
                string f2 = FormulaToView(equlation, c);
                string f3 = FormulaToView(equlation, b);
                
                decimal fc = CalculateFormula(equlation, c);
                if (CheckConvergenceCondition(f1, f2))
                {
                    string uslovie = $"}} f({a}) × f({c}) = {fa} × {fc} = {fa * fc} {(fa * fc < 0 ? "<" : ">")} 0";
                    if (uslovie.Contains("<")) uslovie += " сходится =>";
                    else uslovie += " не сходится";
                    step3Stack.Children.Add(new Label() { Text = f1 });
                    
                    step3Stack.Children.Add(new Label()
                    {
                        Text = uslovie,
                        HorizontalOptions = LayoutOptions.End,
                        Margin = new Thickness(150, -5, 0, -5)
                    });
                    step3Stack.Children.Add(new Label() { Text = f2 });
                    step3Stack.Children.Add(new Label() { Text = f3 });
                    if (uslovie.Contains("не сходится")) return;
                    b = c;
                }
                else
                {
                    string uslovie = $"}} f({c}) × f({b}) = {fc} × {fb} = {fc * fb} {(fb * fc < 0 ? "<" : ">")} 0";
                    if (uslovie.Contains("<")) uslovie += " сходится =>";
                    else uslovie += " не сходится";
                    step3Stack.Children.Add(new Label() { Text = f1 });
                    step3Stack.Children.Add(new Label() { Text = f2 });
                    step3Stack.Children.Add(new Label()
                    {
                        Text = uslovie,
                        HorizontalOptions = LayoutOptions.End,
                        Margin = new Thickness(150, -5, 0, -5)
                    });
                    step3Stack.Children.Add(new Label() { Text = f3 });
                    if (uslovie.Contains("не сходится")) return;
                    a = c;
                }
                step3Stack.Children.Add(new Label() { Text = $"Следующая хорда: [{a} ; {b}]" });
                step3Stack.Children.Add(new Label() { Text = $" " });
            }

            #endregion
        }
        private bool Step2(string equlation, decimal interval1, decimal interval2)
        {
            decimal r1 = CalculateFormula(equlation, interval1);
            decimal r2 = CalculateFormula(equlation, interval2);
            string f1 = FormulaToView(equlation, interval1);
            string f2 = FormulaToView(equlation, interval2);
            step2Stack.Children.Add(new Label() { Text = f1 });
            step2Stack.Children.Add(new Label() { Text = $"}} f({r1}) × f({r2}) = {r1} × {r2} = {r1 * r2} {(r1 * r2 < 0 ? "<":">")} 0",
            HorizontalOptions = LayoutOptions.End, Margin = new Thickness(0, -5)});
            step2Stack.Children.Add(new Label() { Text = f2 });
            if (r1 * r2 < 0 && CheckConvergenceCondition(f1, f2))
            {
                step2Stack.Children.Add(new Label() { Text = "Условие на сходимость выполнено", FontSize = 18 });
                return true;
            }
            step2Stack.Children.Add(new Label() { Text = "Условие на сходимость не выполнено", FontSize = 18 });
            return false;
        }
        private decimal CalculateFormula(string equlation, decimal value)
        {
            string strvalue = value.ToString();
            if (value < 0) strvalue = value.ToString().Replace("-", "~");
            string primer = equlation.Replace("x", strvalue);
            decimal result1 = decimal.Parse(Calculator.SolvePrimer(primer));
            return Math.Round(result1, _decimalCount);
        }
        private string FormulaToView(string equlation, decimal value)
        {
            string strvalue = value.ToString();
            if (value < 0) strvalue = value.ToString().Replace("-", "~");
            string primer = equlation.Replace("x", strvalue);
            decimal result1 = decimal.Parse(Calculator.SolvePrimer(primer));
            primer = primer.Replace("*", "×").Replace("~", "-");
            result1 = Math.Round(result1, _decimalCount);
            return $"f({value}) = {primer} = {result1} {(result1 < 0 ? "<" : ">")} 0";
        }
        private string DeltaToView(decimal c1, decimal c2, decimal epsilon)
        {
            decimal result1 = decimal.Parse(Math.Abs(c2 - c1).ToString());
            result1 = Math.Round(result1, _decimalCount);
            string sxod1 = "";
            if (result1 < epsilon) sxod1 = " < " + epsilon;
            if (result1 > epsilon) sxod1 = " > " + epsilon;
            if (result1 == epsilon) sxod1 = " = " + epsilon;
            return $"|{c2} - {c1}| = " + result1 + sxod1 + " = E";
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