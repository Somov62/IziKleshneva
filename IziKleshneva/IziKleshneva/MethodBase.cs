using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace IziKleshneva
{
    public abstract class MethodBase
    {
        protected int _decimalCount = 4;
        protected Dictionary<decimal, decimal> _decimalMemory;
        protected StackLayout _stack;
        protected Dictionary<int, string> _subscripts = new Dictionary<int, string>()
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
        public int CountZnaks { get; set; }
        public StackLayout Solve(string equlation, decimal interval1, decimal interval2, decimal epsilon)
        {
            if (string.IsNullOrEmpty(equlation)) return null;
            _stack = new StackLayout();
            equlation = equlation.Replace("=0", "");
            equlation = equlation.Replace(" ", "");
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

            while (_stack.Children.Count > 0)
            {
                _stack.Children.RemoveAt(1);
            }

            #region Step 1
            _stack.Children.Add(new Label() { Text = "Первый шаг - переменные", TextColor = Color.DeepPink, FontSize = 20, FontAttributes = FontAttributes.Bold });
            _stack.Children.Add(new Label() { Text = "f(x) = " + equlation });
            _stack.Children.Add(new Label() { Text = "a = " + interval1 });
            _stack.Children.Add(new Label() { Text = "b = " + interval2 });
            _stack.Children.Add(new Label() { Text = "Е = " + epsilon });
            #endregion
            #region Step 2
            _stack.Children.Add(new Label() { Text = "Второй шаг - условие сходимости", TextColor = Color.DeepPink, FontSize = 20, FontAttributes = FontAttributes.Bold });
            _decimalMemory = new Dictionary<decimal, decimal>();
            bool IsСonditionComplete = Step2(equlation, interval1, interval2);
            if (!IsСonditionComplete) return _stack;
            #endregion


            #region Step 3
            _stack.Children.Add(new Label() { Text = "Третий шаг - действие по методу", TextColor = Color.DeepPink, FontSize = 20, FontAttributes = FontAttributes.Bold });
            Step3(equlation, interval1, interval2, epsilon);
            return _stack;
            #endregion
        }
        protected virtual void Step3(string equlation, decimal interval1, decimal interval2, decimal epsilon)
        {

        }
        protected void FormulaToStackPanel(string equlation, ref decimal a, ref decimal b, decimal c)
        {
            _stack.Children.Add(new Label() { Text = $" ", Margin = new Thickness(0, -7) });
            string f1 = FormulaToView(equlation, a);
            string f2 = FormulaToView(equlation, c);
            string f3 = FormulaToView(equlation, b);


            if (CheckConvergenceCondition(f1, f2))
            {
                StackLayout fstack = new StackLayout();
                fstack.Orientation = StackOrientation.Horizontal;
                fstack.Children.Add(new Label() { Text = f1.Remove(f1.Length - 3) });
                fstack.Children.Add(new Label() { Text = f1.Substring(f1.Length - 3), TextColor = Color.LawnGreen, VerticalTextAlignment = TextAlignment.End, MinimumWidthRequest = 30 });
                _stack.Children.Add(fstack);
                fstack = new StackLayout();
                fstack.Orientation = StackOrientation.Horizontal;
                fstack.Children.Add(new Label() { Text = f2.Remove(f2.Length - 3) });
                fstack.Children.Add(new Label() { Text = f2.Substring(f2.Length - 3), TextColor = Color.LawnGreen, VerticalTextAlignment = TextAlignment.End, MinimumWidthRequest = 30 });
                _stack.Children.Add(fstack);
                fstack = new StackLayout();
                fstack.Orientation = StackOrientation.Horizontal;
                fstack.Children.Add(new Label() { Text = f3.Remove(f3.Length - 3) });
                fstack.Children.Add(new Label() { Text = f3.Substring(f3.Length - 3), TextColor = Color.Red, VerticalTextAlignment = TextAlignment.End, MinimumWidthRequest = 30 });
                _stack.Children.Add(fstack);
                b = c;
            }
            else
            {
                StackLayout fstack = new StackLayout();
                fstack.Orientation = StackOrientation.Horizontal;
                fstack.Children.Add(new Label() { Text = f1.Remove(f1.Length - 3) });
                fstack.Children.Add(new Label() { Text = f1.Substring(f1.Length - 3), TextColor = Color.Red, VerticalTextAlignment = TextAlignment.End, MinimumWidthRequest = 30 });
                _stack.Children.Add(fstack);
                fstack = new StackLayout();
                fstack.Orientation = StackOrientation.Horizontal;
                fstack.Children.Add(new Label() { Text = f2.Remove(f2.Length - 3) });
                fstack.Children.Add(new Label() { Text = f2.Substring(f2.Length - 3), TextColor = Color.LawnGreen, VerticalTextAlignment = TextAlignment.End, MinimumWidthRequest = 30 });
                _stack.Children.Add(fstack);
                fstack = new StackLayout();
                fstack.Orientation = StackOrientation.Horizontal;
                fstack.Children.Add(new Label() { Text = f3.Remove(f3.Length - 3) });
                fstack.Children.Add(new Label() { Text = f3.Substring(f3.Length - 3), TextColor = Color.LawnGreen, VerticalTextAlignment = TextAlignment.End, MinimumWidthRequest = 30 });
                _stack.Children.Add(fstack);
                a = c;
            }


        }
        private bool Step2(string equlation, decimal interval1, decimal interval2)
        {
            string f1 = FormulaToView(equlation, interval1);
            string f2 = FormulaToView(equlation, interval2);
            _stack.Children.Add(new Label() { Text = f1 });
            _stack.Children.Add(new Label() { Text = f2 });
            if (CheckConvergenceCondition(f1, f2))
            {
                _stack.Children.Add(new Label() { Text = "Условие сходимости выполнено", FontSize = 18 });
                return true;
            }
            _stack.Children.Add(new Label() { Text = "Условие сходимости не выполнено", FontSize = 18 });
            return false;
        }
        protected string GetSubscripts(int number)
        {
            string result = string.Empty;
            while (number != 0)
            {
                result = result.Insert(0, _subscripts[number % 10]);
                number /= 10;
            }
            return result;
        }
        protected decimal CalculateFormula(string equlation, decimal value)
        {
            if (_decimalMemory.TryGetValue(value, out decimal result))
            {
                return result;
            }
            string strvalue = value.ToString();
            if (value < 0) strvalue = value.ToString().Replace("-", "~");
            string primer = equlation.Replace("x", strvalue);
            decimal result1 = decimal.Parse(Calculator.SolvePrimer(primer));
            result1 = Math.Round(result1, CountZnaks);
            _decimalMemory.Add(value, result1);
            return result1;
        }
        protected string FormulaToView(string equlation, decimal value)
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
            result1 = Math.Round(result1, CountZnaks);
            primer += " = " + result1;
            return $"f({value}) = {primer} {(result1 < 0 ? "<" : ">")} 0";
        }
        protected virtual string DeltaToView(decimal c1, decimal c2, decimal epsilon)
        {
            return "";
        }
        protected bool CheckConvergenceCondition(string f1, string f2)
        {
            if ((f1.Contains("<") && f2.Contains(">")) || (f1.Contains(">") && f2.Contains("<")))
            {
                return true;
            }
            return false;
        }



    }
}
