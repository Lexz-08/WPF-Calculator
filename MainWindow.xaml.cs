using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;

namespace WPF_Calculator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        [DllImport("user32.dll")]
        private static extern bool ReleaseCapture();

        [DllImport("user32.dll")]
        private static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        private void DragWindow(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                ReleaseCapture();
                SendMessage(new WindowInteropHelper(this).Handle, 161, 2, 0);
            }
        }

        private void MinimizeWindow(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                WindowState = WindowState.Minimized;
        }

        private void CloseWindow(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                Application.Current.Shutdown();
        }

        // information for the calculator
        private double number1 = 0.0d, number2 = 0.0d;
        private char oper = '\x0';
        private bool firstNumber = true;
        private bool isDecimal = false;

        // updates the 'GivenMath' TextBox's text in the calculator
        private void UpdateCalculator()
		{
            // get the current number
            string number = (firstNumber ? number1 : number2).ToString();

            if (oper != '\x0')
                // prints out the full expression into the calculator
                GivenMath.Text = $"{number1} {oper} {number2}";
            
                // prints out the first number in the expression with no
                // second number or an operator since no operator is selected
            else GivenMath.Text = isDecimal && number.Split('.').Length == 1 ? number + ".0" : number;
        }

        // clears and resets all information in the calculator
        private void ClearCalculator(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                // resets and clears the calculator's information
                number1 = number2 = 0.0d;
                oper = '\x0';
                firstNumber = true;
                isDecimal = false;

                // resets the expression text label and the calculation result text label
                GivenMath.Text = "0";
                CalculatedMath.Text = "0";
            }
        }

        // clears the currently given information in the calculator
        private void ClearGivenNumber(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                // resets/clears the current number to '0'
                if (firstNumber)
                    number1 = 0.0d;
                else number2 = 0.0d;

                UpdateCalculator();
            }
        }

        // sets the current number to either NEGATIVE or POSITIVE depending on its current sign
        private void SetOppositeSymbol(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                // sets the current number to its opposite
                if (firstNumber)
					number1 = -number1;
				else number2 = -number2;

                UpdateCalculator();
            }
        }

        // sets the current operator of the calculator (BLANK until selected)
        private void SetOperator(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                // gets the "operation char" from the text of the selected operator button
                oper = ((Button)sender).Content.ToString()[0];
                firstNumber = false;

                // performs the calcuation if a second number is already provided when clicked
                if (number2 > 0.0d)
                {
                    Calculate(sender, e);

                    number2 = 0.0d;
                }

                UpdateCalculator();
            }
		}

        // adds the selected digit to the end of the current number
        private void AddNumber(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                // gets the current digit/number from the selected button
                double number = double.Parse(((Button)sender).Content.ToString());

                if (isDecimal)
				{
                    if (firstNumber)
					{
                        // makes the current number a whole number,
                        // adds the digit to the ones place,
                        // and moves the number back to being a decimal
                        string num1 = number1.ToString();
                        int decimals = num1.Split('.').Length > 1 ? num1.Split('.')[1].Length : 0;
                        number1 *= Math.Pow(10, decimals + 1);
                        number1 += number;
                        number1 /= Math.Pow(10, decimals + 1);
					}
                    else
					{
                        // makes the current number a whole number,
                        // adds the digit to the ones place,
                        // and moves the number back to being a decimal
                        string num2 = number2.ToString();
                        int decimals = num2.Split('.').Length > 1 ? num2.Split('.')[1].Length : 0;
                        number2 *= Math.Pow(10, decimals + 1);
                        number2 += number;
                        number2 /= Math.Pow(10, decimals + 1);
					}
				}
                else
				{
                    if (firstNumber)
					{
                        // multiplies the current number by ten
                        // and adds the digit to the ones place
                        number1 *= 10;
                        number1 += number;
					}
                    else
					{
                        // multiplies the current number by ten
                        // and adds the digit to the ones place
                        number2 *= 10;
                        number2 += number;
					}
				}

                UpdateCalculator();
            }
        }

        // tells the calculator to add decimal digits to the number instead of normal digits
        private void SetDecimal(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                // tells the calculator the current number is a decimal
                isDecimal = true;

                UpdateCalculator();
            }
        }

        // removes the digit at the very end of the current number, and resets it to a normal number if the decimal is just '0'
        private void RemoveLastDigit(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                // gets the current number in the calculator as a string
                string number = (firstNumber ? number1 : number2).ToString();

                if (isDecimal)
				{
                    if (firstNumber)
					{
                        // checks to see if the number has any decimals
                        if (number.Split('.').Length > 1)
                        {
                            // remove the last digit in the decimals place if there are any
                            if (number.Split('.')[1].Length == 1)
                                number1 = double.Parse(number.Split('.')[0]);
                            else if (number.Split('.')[1].Length > 1)
                                number1 = double.Parse(number.Substring(0, number.Length - 1));
                        }
                        else isDecimal = false; // set the number as whole since there are no decimals in place
					}
                    else
					{
                        // checks to see if the number has any decimals
                        if (number.Split('.').Length > 1)
                        {
                            // remove the last digit in the decimals place if there are any
                            if (number.Split('.')[1].Length == 1)
                                number2 = double.Parse(number.Split('.')[0]);
                            else if (number.Split('.')[1].Length > 1)
                                number2 = double.Parse(number.Substring(0, number.Length - 1));
                        }
                        else isDecimal = false; // set the number as whole since there are no decimals in place
					}
				}
                else
				{
                    // set the current number to '0' since there's just one digit in place
                    if (firstNumber)
                        number1 = double.Parse(double.Parse(number) < 10 ? "0" : number.Substring(0, number.Length > 1 ? number.Length - 1 : 1));
                    else number2 = double.Parse(double.Parse(number) < 10 ? "0" : number.Substring(0, number.Length > 1 ? number.Length - 1 : 1));
				}

                UpdateCalculator();
            }
        }

        // performs the selected calculation (divide, multiply, subtract, add)
        private void Calculate(object sender, MouseButtonEventArgs e)
		{
            // print error message into calculator since user attempted division by '0'
            if (number2 == 0.0d && oper == '/')
			{
                CalculatedMath.Text = "undefined - cannot divide by 0";

                return;
			}

            // perform the math operation based on operator selected
            switch (oper)
			{
                case '/':
                    number1 = (number1 / number2);
                    CalculatedMath.Text = number1.ToString();
                    break;
                case '*':
                    number1 = (number1 * number2);
                    CalculatedMath.Text = number1.ToString();
                    break;
                case '-':
                    number1 = (number1 - number2);
                    CalculatedMath.Text = number1.ToString();
                    break;
                case '+':
                    number1 = (number1 + number2);
                    CalculatedMath.Text = number1.ToString();
                    break;
            }

            // clear the calculator's input expression since a math operation was performed
            GivenMath.Text = "0";
		}
    }
}
