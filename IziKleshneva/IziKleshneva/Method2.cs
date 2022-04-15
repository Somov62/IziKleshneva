using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace IziKleshneva
{
    class Method2 : Method123Base
    {
       protected override void Step3(string equlation, decimal interval1, decimal interval2, decimal epsilon)
        {
            #region Step 3
            //           f(a)
            //Ψ = a - --------- * (b - a)
            //        f(b)-f(a)


            decimal a = interval1;
            decimal b = interval2;

            base._stack.Children.Add(new Label() { Text = $"Формула: ξ = a - f(a) ÷ (f(b)-f(a)) × (b - a) " });
            base._stack.Children.Add(new Label() { Text = $"Справка: ξ - знак кси; знак ÷ в виде дробной черты" });
            base._stack.Children.Add(new Frame() { HeightRequest = 1, BackgroundColor = Color.DeepPink, CornerRadius = 3, Padding = new Thickness(0), Margin = new Thickness(0, 10) });
            base._stack.Children.Add(new Label() { Text = $" " });

            decimal oldDelta = 0;
            decimal oldс = 0;
            int counterPovt = 0;
            int counterRoots = 1;

            while (true)
            {
                decimal fa = CalculateFormula(equlation, a);
                decimal fb = CalculateFormula(equlation, b);
                decimal c = decimal.Parse((a - fa / (fb - fa) * (b - a)).ToString());
                c = Math.Round(c, _decimalCount);
                base._stack.Children.Add(new Label() { Text = $"ξ{GetSubscripts(counterRoots)} = {a} - {fa} ÷ ({fb} - {fa}) × ({b} - {a}) ≈ " + c });
                if (oldс != 0)
                {
                    decimal delta = decimal.Parse(Math.Abs(c - oldс).ToString());
                    delta = Math.Round(delta, _decimalCount);
                    if (oldDelta == delta)
                    {
                        counterPovt++;
                        if (counterPovt > 3)
                        {
                            base._stack.Children.Add(new Label() { Text = "Корень: " + c });
                            base._stack.Children.Add(new Label() { Text = "Внимание! Точность не достигнута! Попробуйте увеличить количество знаков после запятой." });
                            return;
                        }
                    }
                    else
                    {
                        oldDelta = delta;
                        counterPovt = 0;
                    }
                    base._stack.Children.Add(new Label() { Text = this.DeltaToView(oldс, c, epsilon) });
                    if (delta <= epsilon)
                    {
                        base._stack.Children.Add(new Label() { Text = "Точность достигнута" });
                        base._stack.Children.Add(new Label() { Text = "Корень: " + c });
                        break;
                    }
                    Label label = base._stack.Children[base._stack.Children.Count - 1] as Label;
                    label.Text += " (Можно не писать)";
                    label.TextColor = Color.Gray;
                }
                FormulaToStackPanel(equlation, ref a, ref b, c);
                base._stack.Children.Add(new Label() { Text = $"Следующая хорда: [{a} ; {b}]" });
                base._stack.Children.Add(new Frame() { HeightRequest = 1, BackgroundColor = Color.DeepPink, CornerRadius = 3, Padding = new Thickness(0), Margin = new Thickness(0, 10) });
                oldс = c;
                counterRoots++;
            }

            #endregion
        }


       protected override string DeltaToView(decimal c1, decimal c2, decimal epsilon)
       {
           decimal result1 = decimal.Parse(Math.Abs(c2 - c1).ToString());
           result1 = Math.Round(result1, _decimalCount);
           string sxod1 = "";
           if (result1 < epsilon) sxod1 = " < " + epsilon;
           if (result1 > epsilon) sxod1 = " > " + epsilon;
           if (result1 == epsilon) sxod1 = " = " + epsilon;
           return $"|{c2} - {c1}| = " + result1 + sxod1 + " = E";
       }
    }
}
