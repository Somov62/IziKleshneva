using System;
using System.Collections.Generic;
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
            primer = primer.Replace("*", "×");
            //решаем факториал
            while (primer.IndexOf("!") != -1)
            {
                int znakIndex = primer.IndexOf("!");
                int leftBorder = LeftBorderIndex(primer, znakIndex);
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
                int leftBorder = LeftBorderIndex(primer, znakIndex);
                primer = NewPrimer(primer, leftBorder, znakIndex, out decimal fNum, out decimal sNum);
                decimal result = 1;
                for (int i = 0; i < sNum; i++)
                {
                    result *= fNum;
                }
                primer = primer.Insert(leftBorder, result.ToString());
            }

            //решаем умножение/деление
            while (primer.IndexOf("×") != -1 || primer.IndexOf("÷") != -1)
            {
                char[] simbols = { '×', '÷' };
                int znakIndex = primer.IndexOfAny(simbols, 1);
                int leftBorder = LeftBorderIndex(primer, znakIndex);
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
                int leftBorder = LeftBorderIndex(primer, znakIndex);
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
            sNum = decimal.Parse(primer.Substring(i + 1, rBorder - i));//получаем второе
            primer = primer.Remove(lBorder, rBorder - lBorder + 1);//вырезаем все действие
            return primer;//возвращаем строку с вырезанным действием и два числа для последущего действия с ними
        }
        /// <summary>
        /// Возвращает правую границу промежуточного действия при помощи индекса знака. Используется для последущего получения правого от знака числа методом Substring в методе NewPrimer
        /// </summary>
        private static int RightBorderIndex(string primer, int znakIndex)
        {
            int rightBorder = znakIndex + 1;//ищем правую границу, начиная от знака действия
            if (primer[znakIndex + 1] == '-') znakIndex++;//если сразу после знака стоит минус, значит мы работаем с отрицательным числом
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
        private static int LeftBorderIndex(string primer, int znakIndex)
        {
            int leftBorder = znakIndex - 1;
            for (int i = znakIndex; i > 0; i--)
            {
                if (primer[i - 1] == ',' || primer[i - 1] == '.') continue;
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
