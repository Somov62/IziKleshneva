using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace IziKleshneva
{
    public class Method1 : MethodBase
    {
        protected override void Step3(string equlation, decimal interval1, decimal interval2, decimal epsilon)
        {
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
                base._stack.Children.Add(new Label() { Text = $"C{GetSubscripts(counterRoots)} = {a} + |{b} - {a}| ÷ 2 = " + c });
                base._stack.Children.Add(new Label() { Text = DeltaToView(c, b, epsilon) });
                if (delta <= epsilon)
                {
                    base._stack.Children.Add(new Label() { Text = "Точность достигнута" });
                    base._stack.Children.Add(new Label() { Text = "Корень: " + c });
                    break;
                }
                Label label = base._stack.Children[base._stack.Children.Count - 1] as Label;
                label.Text += " (Можно не писать)";
                label.TextColor = Color.Gray;
                FormulaToStackPanel(equlation, ref a, ref b, c);
                base._stack.Children.Add(new Label() { Text = $"Следующий отрезок: [{a} ; {b}]" });
                base._stack.Children.Add(new Frame() { HeightRequest = 1, BackgroundColor = Color.DeepPink, CornerRadius = 3, Padding = new Thickness(0), Margin = new Thickness(0, 10) });
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
            return $"ΔС = |{c2} - {c1}| = " + result1 + sxod1;
        }
    }
}
