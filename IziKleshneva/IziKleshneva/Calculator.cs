using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IziKleshneva
{
    public static class Calculator
    {
        public static string SolvePrimer(string primer)
        {
            primer = primer.Replace(" ", "");
            primer = primer.Replace("_", " ");
            primer = primer.Replace(":", "÷");
            primer = primer.Replace("/", "÷");
            //primer = primer.Replace(".", ",");
            primer = primer.Replace("π", Math.PI.ToString());
            try
            {
                while (primer.IndexOf("(") != -1)
                {
                    primer = FindMainPrimer(primer);//раскрываем скобки
                }
                primer = SolvePrimer1(primer);//решаем пример без скобок
                                             //if (primer.IndexOf(',') != -1 && primer.Length - primer.IndexOf(',') - 1 > 2)
                            
            }
            catch
            {
                primer = "0";
            }
            return primer;
        }
        private static string FindMainPrimer(string primer)
        {
            //поиск правой границы примера
            int rightBorder = primer.IndexOf(")");
            //поиск левой границы примера на основе правой
            int leftborder = primer.LastIndexOf("(", rightBorder);
            //записываем то что внутри скобок в отдельный простой пример и решаем его и записываем получившийся результат
            string middlePrimer = SolvePrimer(primer.Substring(leftborder + 1, rightBorder - leftborder - 1));
            try
            {
                char[] simbols = { 'i', 'o', 'a', 'r' };
                if (simbols.Contains(primer[leftborder - 2]))
                {
                    double value = Convert.ToDouble(middlePrimer);
                    if (primer[leftborder - 2] == 'i')//синус
                    {
                        value = Math.Sin(value);
                    }
                    if (primer[leftborder - 2] == 'o')//косинус
                    {
                        value = Math.Cos(value);
                    }
                    if (primer[leftborder - 2] == 'a')//тангенс
                    {
                        value = Math.Tan(value);
                    }
                    if (primer[leftborder - 2] == 't')//котангенс
                    {
                        value = 1 / Math.Tan(value);
                    }
                    if (primer[leftborder - 2] == 'r')//корень
                    {
                        if (value < 0) DisplayAlert("Error", $"Ошибка: подкоренное выражение ({value}) отрицательное", "OK");
                        value = Math.Sqrt(value);
                        leftborder -= 1;
                    }
                    leftborder -= 3;//раздвигаем границу промежуточного действия, чтобы вырезать буквы
                    middlePrimer = value.ToString();
                }
            }
            catch
            { }
            primer = primer.Remove(leftborder, rightBorder - leftborder + 1);//удаляем промежуточный пример
            primer = primer.Insert(leftborder, middlePrimer);//вставляем посчитанный в этом методе результат
            return primer;
        }

        private static void DisplayAlert(string v1, string v2, string v3)
        {
            throw new NotImplementedException();
        }

        private static string SolvePrimer1(string primer)
        {
            primer = primer.Replace(" ", "");
            primer = primer.Replace("_", " ");
            primer = primer.Replace(":", "÷");
            primer = primer.Replace("*", "×");
            //решаем факториал
            while (primer.IndexOf("!") != -1)
            {
                int znakIndex = primer.IndexOf("!");
                int leftBorder = LeftBorderIndex(ref primer, znakIndex);
                decimal num = decimal.Parse(primer.Substring(leftBorder, znakIndex - leftBorder));
                primer = primer.Remove(leftBorder, znakIndex - leftBorder + 1);
                int rez = 1;
                for (int i = 1; i <= num; i++)
                {
                    rez *= i;
                }
                primer = primer.Insert(leftBorder, rez.ToString());
            }
            //решаем степень
            while (primer.IndexOf("^") != -1)
            {
                int znakIndex = primer.IndexOf("^");
                int leftBorder = LeftBorderIndex(ref primer, znakIndex);
                primer = NewPrimer(primer, leftBorder, znakIndex, out decimal fNum, out decimal sNum);
                primer = primer.Insert(leftBorder, Math.Pow((double)fNum, (double)sNum).ToString());
            }

            //решаем умножение/деление
            while (primer.IndexOf("×") != -1 || primer.IndexOf("÷") != -1)
            {
                char[] simbols = { '×', '÷' };
                int znakIndex = primer.IndexOfAny(simbols, 1);
                int leftBorder = LeftBorderIndex(ref primer, znakIndex);
                char znak = primer[znakIndex];
                primer = NewPrimer(primer, leftBorder, znakIndex, out decimal fNum, out decimal sNum);
                if (znak == '×') primer = primer.Insert(leftBorder, ((fNum * sNum)).ToString());
                else primer = primer.Insert(leftBorder, ((fNum / sNum)).ToString());
            }

            //решаем сложение/вычитание
            while (primer.IndexOf("+") != -1 || primer.IndexOf("-", 1) != -1)
            {
                char[] simbols = { '-', '+' };
                int znakIndex = primer.IndexOfAny(simbols, 1);
                int leftBorder = LeftBorderIndex(ref primer, znakIndex);
                char znak = primer[znakIndex];
                primer = NewPrimer(primer, leftBorder, znakIndex, out decimal fNum, out decimal sNum);
                if (znak == '+') primer = primer.Insert(leftBorder, ((fNum + sNum)).ToString());
                else primer = primer.Insert(leftBorder, ((fNum - sNum)).ToString());
            }
            return primer;
        }
        /// <summary>
        /// Метод выделяющий из примера единичное действия типа "хRу", где R - знак арифметического действия. Метод наход x, y и возвращает их. 
        /// Также вырезает из примера действие "xRy" для последующей вставки результата вычислений.
        /// </summary>        
        private static string NewPrimer(string primer, int lBorder, int i, out decimal fNum, out decimal sNum)
        {
            int rBorder = RightBorderIndex(primer, i);//находим правую границу правого от знака числа
            fNum = decimal.Parse(primer.Substring(lBorder, i - lBorder));//получаем первое число
            sNum = decimal.Parse(primer.Substring(i + 1, rBorder - i).Replace("~", "-"));//получаем второе
            primer = primer.Remove(lBorder, rBorder - lBorder + 1);//вырезаем все действие
            return primer;//возвращаем строку с вырезанным действием и два числа для последущего действия с ними
        }
        /// <summary>
        /// Возвращает правую границу промежуточного действия при помощи индекса знака. Используется для последущего получения правого от знака числа методом Substring в методе NewPrimer
        /// </summary>
        private static int RightBorderIndex(string primer, int znakIndex)
        {
            int rightBorder = znakIndex + 1;//ищем правую границу, начиная от знака действия
            if (primer[znakIndex + 1] == '-' || primer[znakIndex + 1] == '~') znakIndex++;//если сразу после знака стоит минус, значит мы работаем с отрицательным числом
            for (int i = znakIndex; i < primer.Length - 1; i++)
            {
                if (primer[i + 1] == ',' || primer[i + 1] == '.') continue;//запятая не влияет на границу, пропускаем ее
                if (!Int32.TryParse(primer[i + 1].ToString(), out _))//если натыкаемся на знак или скобку - находим нашу границу
                {
                    rightBorder = i;
                    break;
                }
                //если ни на что не наткнулись, за правую границу принимаем последний элемент строки
                if (i + 1 == primer.Length - 1) rightBorder = i + 1;
            }
            return rightBorder;
        }
        private static int LeftBorderIndex(ref string primer, int znakIndex)
        {
            int leftBorder = znakIndex - 1;
            for (int i = znakIndex; i > 0; i--)
            {
                if (primer[i - 1] == ',' || primer[i - 1] == '.') continue;
                if (primer[i - 1] == '~')
                {
                    primer = primer.Remove(i - 1, 1);
                    primer = primer.Insert(i - 1, "-");
                    leftBorder = i - 1;
                    break;
                }
                if (primer[i - 1] == '-' && i - 1 == 0)
                {
                    leftBorder = 0;
                    break;
                }
                if (!Int32.TryParse(primer[i - 1].ToString(), out _))//если натыкаемся на знак или скобку - находим нашу границу
                {
                    leftBorder = i;
                    break;
                }
                if (i - 1 == 0) leftBorder = 0;
            }
            return leftBorder;
        }
    }
}
