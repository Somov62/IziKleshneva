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
    public partial class Method4Page : ContentPage
    {
        public Method4Page()
        {
            InitializeComponent();
        }
        protected Dictionary<int, string> _subscriptsLower = new Dictionary<int, string>()
        {
            {1, "₁" }, {2, "₂" }, {3, "₃" }, {4, "₄" }, {5, "₅" },
            {6, "₆" }, {7, "₇" }, {8, "₈" }, {9, "₉" }, {0, "₀" }
        };
        protected Dictionary<int, string> _subscriptsUpper = new Dictionary<int, string>()
        {
            {1, "¹" }, {2, "²" }, {3, "³" }, {4, "⁴" }, {5, "⁵" },
            {6, "⁶" }, {7, "⁷" }, {8, "⁸" }, {9, "⁹" }, {0, "⁰" }
        };
        protected string GetSub(int number, bool isUp = false)
        {
            string result = string.Empty;
            do
            {
                if (isUp)
                    result = result.Insert(0, _subscriptsUpper[number % 10]);
                else
                    result = result.Insert(0, _subscriptsLower[number % 10]);
                number /= 10;
            } while (number != 0);
            return result;
        }
        private void OpenFlyout_Click(object sender, EventArgs e)
        {
            Shell.Current.FlyoutIsPresented = true;
        }

        private void CreateMatrix_Click(object sender, EventArgs e)
        {

            matrixContainer.Children.Clear();
            matrixResultContainer.Children.Clear();
            if (!int.TryParse(txtCountEqulation.Text, out int countEqulation)) return;
            if (!int.TryParse(txtCountX.Text, out int countX)) return;
            if (!int.TryParse(txtCountIteration.Text, out _)) return;

            for (int i = 0; i < countEqulation; i++)
            {

                StackLayout row = new StackLayout();
                row.Orientation = StackOrientation.Horizontal;
                row.Children.Add(new Label() { Text = "(" });
                for (int j = 0; j < countX; j++)
                {
                    row.Children.Add(new Entry() { TextColor = Color.Black });
                }
                row.Children.Add(new Label() { Text = ")" });
                matrixContainer.Children.Add(row);
            }
            for (int i = 0; i < countEqulation; i++)
            {
                StackLayout row = new StackLayout();
                row.Orientation = StackOrientation.Horizontal;
                row.Children.Add(new Label() { Text = "(" });
                row.Children.Add(new Entry() { TextColor = Color.Black });
                row.Children.Add(new Label() { Text = ")" });
                matrixResultContainer.Children.Add(row);
            }
            tip1.IsVisible = true;
            tip2.IsVisible = true;
            btnSolve.IsVisible = true;
        }

        private decimal[,] FillMatrix()
        {
            int countEqulation = matrixResultContainer.Children.Count();
            int countX = ((StackLayout)matrixContainer.Children[0]).Children.Count() - 1;
            decimal[,] matrix = new decimal[countEqulation, countX];
            try
            {
                for (int i = 0; i < countEqulation; i++)
                {
                    for (int j = 0; j < countX - 1; j++)
                    {
                        matrix[i, j] = decimal.Parse(((Entry)((StackLayout)matrixContainer.Children[i]).Children[j + 1]).Text);
                    }
                    matrix[i, countX - 1] = decimal.Parse(((Entry)((StackLayout)matrixResultContainer.Children[i]).Children[1]).Text);
                }
            }
            catch (Exception)
            {
                return null;
            }
            return matrix;
        }

        private void ShowMatrix(decimal[,] matrix)
        {
            StackLayout matrixLayout = new StackLayout() { Orientation = StackOrientation.Horizontal, HorizontalOptions = LayoutOptions.FillAndExpand, Margin = new Thickness(0, 15) };
            StackLayout skobLayout = new StackLayout() { HorizontalOptions = LayoutOptions.FillAndExpand };
            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                skobLayout.Children.Add(new Label() { Text = "(", FontSize = 20, TextColor = Color.White, HorizontalOptions = LayoutOptions.EndAndExpand, VerticalOptions = LayoutOptions.Center });
            }
            matrixLayout.Children.Add(skobLayout);
            for (int j = 0; j < matrix.GetLength(1); j++)
            {
                StackLayout stack = new StackLayout() { HorizontalOptions = LayoutOptions.FillAndExpand };
                for (int i = 0; i < matrix.GetLength(0); i++)
                {
                    stack.Children.Add(new Label()
                    {
                        Text = matrix[i, j].ToString(),
                        TextColor = i == j ? Color.LawnGreen : Color.White,
                        HorizontalOptions = LayoutOptions.EndAndExpand,
                        FontSize = 20,
                        VerticalOptions = LayoutOptions.Center,
                        Margin = new Thickness(5, 0, 0, 0)
                    });
                }
                matrixLayout.Children.Add(stack);
            }
            skobLayout = new StackLayout() { HorizontalOptions = LayoutOptions.FillAndExpand };
            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                skobLayout.Children.Add(new Label() { Text = ")", FontSize = 20, TextColor = Color.White, HorizontalOptions = LayoutOptions.FillAndExpand, VerticalOptions = LayoutOptions.Center });
            }
            matrixLayout.Children.Add(skobLayout);
            stackParent.Children.Add(matrixLayout);
        }


        private bool CheckCondition(decimal[,] matrix)
        {
            bool result = true;
            int lowerDimension = 0;
            if (matrix.GetLength(1) < matrix.GetLength(0)) lowerDimension = 1;
            for (int i = 0; i < matrix.GetLength(lowerDimension); i++)
            {
                string label = $"{i + 1} ст. |{matrix[i, i]}| > ";
                decimal sumElements = 0;
                for (int j = 0; j < matrix.GetLength(1) - 1; j++)
                {
                    if (i == j) continue;
                    sumElements += Math.Abs(matrix[i, j]);
                    label += $"|{matrix[i, j]}| + ";
                }
                label = label.Remove(label.LastIndexOf("+"), 2);
                label += $"= {sumElements} ";
                if (Math.Abs(matrix[i, i]) > sumElements) label += "верно";
                else
                {
                    label += "не верно";
                    result = false;
                }
                stackParent.Children.Add(new Label() { Text = label, TextColor = Color.White });
            }
            return result;
        }
        private StackLayout GetFraction(string numerator, string denominator)
        {
            StackLayout stack = new StackLayout();
            stack.Children.Add(new Label() { Text = numerator, HorizontalOptions = LayoutOptions.Center });
            stack.Children.Add(new Frame() { HeightRequest = 1, Padding = new Thickness(0), Margin = new Thickness(0, -3, 0, -4), HorizontalOptions = LayoutOptions.FillAndExpand, BackgroundColor = Color.White, VerticalOptions = LayoutOptions.Start });
            stack.Children.Add(new Label() { Text = denominator, HorizontalOptions = LayoutOptions.Center });
            return stack;
        }
        private StackLayout FindAccuracy(decimal[] oldcorny, decimal[] newcorny, int actualIteration)
        {
            StackLayout stack = new StackLayout();
            stack.Children.Add(new Label() { Text = $"Нахождение точности", TextColor = Color.DeepPink, FontSize = 17 });

            decimal[] accur = new decimal[oldcorny.Length];
            decimal max = decimal.MinValue;
            for (int i = 0; i < oldcorny.Length; i++)
            {
                string line = $"Δ{GetSub(i + 1)} = |x{GetSub(i + 1)}{GetSub(actualIteration + 1, true)}- x{GetSub(i + 1)}{GetSub(actualIteration, true)}| = |";
                line += Math.Round(newcorny[i], 4).ToString();
                if (oldcorny[i] < 0) line += " + ";
                else line += " - ";
                line += Math.Round(Math.Abs(oldcorny[i]), 4).ToString() + "| = ";
                accur[i] = Math.Abs(newcorny[i] - oldcorny[i]);
                if (accur[i] > max) max = accur[i];
                line += Math.Round(accur[i], 4);
                stack.Children.Add(new Label() { Text = line });
            }
            stack.Children.Add(new Label() { Text = $"E = {Math.Round(max, 4)}", TextColor = Color.LawnGreen });
            return stack;
        }
        private decimal[] Step3(decimal[,] matrix, int iterationCount, bool findAccuracy)
        {
            stackParent.Children.Add(new Label() { Text = "Начальное приближение:", FontSize = 17 });
            int iteration = 0;
            decimal[] corny = new decimal[matrix.GetLength(1) - 1];
            for (int i = 0; i < corny.Length; i++)
            {
                corny[i] = 0;
                stackParent.Children.Add(new Label() { Text = $"x{GetSub(i + 1)}{GetSub(iteration, true)}= 0" });
            }

            decimal[] diagonal = new decimal[matrix.GetLength(0)];
            for (int i = 0; i < diagonal.Length; i++)
            {
                for (int j = 0; j < matrix.GetLength(1); j++)
                {
                    if (i == j)
                    {
                        diagonal[i] = matrix[i, j];
                        break;
                    }
                }
            }
            decimal[] newCorny = new decimal[corny.Length];
            while (iterationCount > iteration)
            {
                stackParent.Children.Add(new Label() { Text = $"Итерация {iteration + 1}  i = " + iteration, TextColor = Color.DeepPink, FontSize = 17 });
                for (int i = 0; i < corny.Length; i++)
                {
                    StackLayout stack = new StackLayout() { Orientation = StackOrientation.Horizontal };
                    stack.Children.Add(new Label() { Text = $"x{GetSub(i + 1)}{GetSub(iteration + 1, true)}=", VerticalOptions = LayoutOptions.Center });

                    string numerator = "";
                    decimal coren = 0;
                    for (int j = 0; j < matrix.GetLength(1) - 1; j++)
                    {
                        if (i == j) continue;
                        coren += matrix[i, j] * -1 * newCorny[j];
                        if (matrix[i, j] == 0) continue;
                        int upindex = iteration + (j > i ? 0 : 1);
                        string element = Math.Round(matrix[i, j] * -1, 4).ToString() + $"x{GetSub(j + 1)}{GetSub(upindex, true)}";
                        if (element[0] != '-') element = element.Insert(0, "+");
                        numerator += element;
                    }
                    numerator = numerator.Insert(0, matrix[i, matrix.GetLength(1) - 1].ToString());
                    stack.Children.Add(GetFraction(numerator, diagonal[i].ToString()));
                    stack.Children.Add(new Label() { Text = $" = ", VerticalOptions = LayoutOptions.Center });
                    stack.Children.Add(GetFraction(Math.Round(matrix[i, matrix.GetLength(1) - 1] + coren, 4).ToString(), diagonal[i].ToString()));
                    newCorny[i] = (matrix[i, matrix.GetLength(1) - 1] + coren) / diagonal[i];
                    stack.Children.Add(new Label() { Text = $" = " + Math.Round(newCorny[i], 4), VerticalOptions = LayoutOptions.Center });
                    stackParent.Children.Add(stack);
                }
                StackLayout accuracy = null;
                if (findAccuracy && iteration + 1 == iterationCount)
                {
                    accuracy = FindAccuracy(corny, newCorny, iteration);
                }
                Array.Copy(newCorny, corny, newCorny.Length);
                for (int i = 0; i < corny.Length; i++)
                {
                    stackParent.Children.Add(new Label() { Text = $"x{GetSub(i + 1)}{GetSub(iteration + 1, true)}= " + Math.Round(corny[i], 4) });
                }

                if (accuracy != null) stackParent.Children.Add(accuracy);
                iteration++;
            }
            return corny;
        }
        private void SolveClick(object sender, EventArgs e)
        {
            decimal[,] matrix = FillMatrix();

            if (matrix == null)
            {
                DisplayAlert("Error", "Неверные значения матрицы", "ok");
                return;
            }
            stackParent.Children.Clear();
            stackParent.Children.Add(new Label() { Text = "Первый шаг - переменные", TextColor = Color.DeepPink, FontSize = 20, FontAttributes = FontAttributes.Bold });
            ShowMatrix(matrix);
            stackParent.Children.Add(new Label() { Text = "Второй шаг - условие сходимости", TextColor = Color.DeepPink, FontSize = 20, FontAttributes = FontAttributes.Bold });
            if (CheckCondition(matrix))
            {
                stackParent.Children.Add(new Label() { Text = "Условие сходимости выполнено", FontSize = 18, TextColor = Color.LawnGreen });
            }
            else
            {
                stackParent.Children.Add(new Label() { Text = "Условие сходимости не выполнено", FontSize = 18, TextColor = Color.Red });
                return;
            }
            stackParent.Children.Add(new Label() { Text = "Третий шаг - действие по методу", TextColor = Color.DeepPink, FontSize = 20, FontAttributes = FontAttributes.Bold });
            var corny = Step3(matrix, 3, toggle.IsToggled);

            StackLayout stack = new StackLayout() { Orientation = StackOrientation.Horizontal };
            stack.Children.Add(new Label() { Text = "Ответ:   x = ", VerticalOptions = LayoutOptions.Center });
            StackLayout matrStack = new StackLayout();
            for (int i = 0; i < corny.Length; i++)
            {
                matrStack.Children.Add(new Label() { Text = "( " });
            }
            stack.Children.Add(matrStack);
            matrStack = new StackLayout();
            for (int i = 0; i < corny.Length; i++)
            {
                matrStack.Children.Add(new Label() { Text = Math.Round(corny[i], 4).ToString() });
            }
            stack.Children.Add(matrStack);
            matrStack = new StackLayout();
            for (int i = 0; i < corny.Length; i++)
            {
                matrStack.Children.Add(new Label() { Text = " ) " });
            }
            stack.Children.Add(matrStack);
            stackParent.Children.Add(stack);

        }
    }
}

