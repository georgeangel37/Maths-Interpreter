using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Newtonsoft.Json;
using System.IO;
using static System.Net.Mime.MediaTypeNames;
using Microsoft.Win32;

namespace Maths_Software_with_Interpreter
{
    using OxyPlot;
    using OxyPlot.Series;
    public partial class MainWindow : Window
    {
        DataPlot dp;
        public static int xIndex = Globals.variables.FindIndex(item => item.GetName() == "x");
        public static int yIndex = Globals.variables.FindIndex(item => item.GetName() == "y");
        public PlotModel MainPlot { get; private set; }

        public MainWindow()
        {
            InitializeComponent();
            dp = new DataPlot();
            DataContext = this;
            MainPlot = dp.MakePlotLineSeries("sin(x)", -10, 10, 0.1);
            dp.SetExpression("sin(x)");

            // Set up variables window
            var VariableData = new StringBuilder();

            foreach (Operand entry in Globals.variables)
            {
                if (entry.GetName() != "e" && entry.GetName() != "π") AddVariableToWindow(entry.GetName(), entry.GetVal());
            }
        }
        // Allows the window to be moved
        private void MoveWindow(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                DragMove();
            }
        }

        // Minimises the window when clicking the 'Min' button
        private void MinimiseWindow(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        // Maximises the window when clicking the 'Max' button.
        // Restores the original window dimensions when clicking the 'Restore' button
        private void MaximiseWindow(object sender, RoutedEventArgs e)
        {
            if (WindowState == WindowState.Normal)
            {
                WindowState = WindowState.Maximized;
                MaximiseButton.Content = "Restore";
            }
            else
            {
                WindowState = WindowState.Normal;
                MaximiseButton.Content = "Max";
            }
        }

        // Closes the window when clicking the 'Close' button
        private void CloseWindow(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void FindZeroCrossings(object sender, RoutedEventArgs e)
        {
            if (dp.GetExpression() != null && XMinTextBox.Text != "" && XMaxTextBox.Text != "" && XIncTextBox.Text != "")
            {
                // find all x values for which y values are approx. 0
                double[] Roots = new double[1000];
                int RootCount = 0;
                double Y;
                double PrevY = 0;
                double PrevX = 0;
                Operand XTemp = Globals.variables[xIndex];
                double MinX = double.Parse(XMinTextBox.Text);
                double MaxX = double.Parse(XMaxTextBox.Text);
                double Dx = double.Parse(XIncTextBox.Text);

                for (double i = MinX; i <= MaxX; i += Dx)
                {
                    Globals.variables[xIndex].SetVal(i);
                    Y = double.Parse(Interpreter.InterpretInput(dp.GetExpression(), false));
                    if (Y * PrevY < 0)
                    {
                        // Root exists between PrevX and i
                        double Root = FindRoot(PrevX, i);
                        Console.WriteLine("Root found: " + Root);
                        Roots[RootCount] = Math.Round(Root, 4, MidpointRounding.AwayFromZero);
                        RootCount++;
                    }
                    PrevY = Y;
                    PrevX = i;
                }
                Globals.variables[xIndex] = XTemp;

                WorkspaceTextBox.Text += ">>" + "Roots of equation: " + dp.GetExpression() + "\n" + "Within the X range " + MinX + " to " + MaxX + ":\n" + "Using an X increment of: " + Dx + "\n";
                if (RootCount > 0)
                {
                    for (int i = 0; i < RootCount; i++)
                    {
                        WorkspaceTextBox.Text += Roots[i].ToString() + "\n";
                    }
                }
                else
                {
                    WorkspaceTextBox.Text += "No roots found" + "\n";
                }
            }
            else
            {
                PrintToOutputLog("Error: Missing an equation/input parameters to find roots");
            }
        }
        private double FindRoot(double LowerX, double UpperX) // uses bisection method to find the root in the interval LowerX to UpperX
        {
            double Midpoint = LowerX;
            while (UpperX - LowerX > 0.00001)
            {
                Midpoint = (UpperX + LowerX) / 2;
                Globals.variables[xIndex].SetVal(Midpoint);
                double MidpointY = double.Parse(Interpreter.InterpretInput(dp.GetExpression(), false));
                if (MidpointY == 0.0)
                {
                    break;
                }
                Globals.variables[xIndex].SetVal(LowerX);
                double LowerXY = double.Parse(Interpreter.InterpretInput(dp.GetExpression(), false));
                if (MidpointY * LowerXY < 0)
                {
                    UpperX = Midpoint;
                }
                else
                {
                    LowerX = Midpoint;
                }
            }
            return Midpoint;
        }

        private void FindDefiniteIntegral(object sender, RoutedEventArgs e)
        {
            if (XIncTextBox.Text.Length <= 0 || XMinTextBox.Text.Length <= 0 || XMaxTextBox.Text.Length <= 0)
            {
                PrintToOutputLog("Error: Missing input parameters");
                return;
            }
            if ((float.Parse(XMinTextBox.Text) >= float.Parse(XMaxTextBox.Text)) || float.Parse(XIncTextBox.Text) <= 0)
            {
                PrintToOutputLog("Error: Invalid input parameters");
                return;
            }
            try
            {
                double UpperX = double.Parse(XMaxTextBox.Text);
                double LowerX = double.Parse(XMinTextBox.Text);
                int n = 64;
                double Interval = (UpperX - LowerX) / n;
                double Result = 0;
                Operand XTemp = Globals.variables[xIndex];
                for (int i = 0; i <= n; i++)
                {
                    Globals.variables[xIndex].SetVal(LowerX + (i * Interval));
                    if (i == 0 || i == n)  // first and last elements
                    {
                        Result += double.Parse(Interpreter.InterpretInput(dp.GetExpression(), false));
                    }
                    else if (i % 2 == 0)  // even element
                    {
                        Result += 2 * double.Parse(Interpreter.InterpretInput(dp.GetExpression(), false));
                    }
                    else  // odd element
                    {
                        Result += 4 * double.Parse(Interpreter.InterpretInput(dp.GetExpression(), false));
                    }
                }
                Result *= Interval / 3;

                Globals.variables[xIndex] = XTemp;

                WorkspaceTextBox.Text += ">>" + "Definite integral of equation: " + dp.GetExpression() + "\n" + "Within the X range " + LowerX + " to " + UpperX + ":\n" + Result + "\n";
            }
            catch
            {
                PrintToOutputLog("Error: Missing an equation/input parameters to find definite integral");
            }
        }

        public void DrawGraph(object sender, RoutedEventArgs e)
        {
            if (InputTextBox.Text.Length <= 0)
            {
                PrintToOutputLog("Error: Missing graph to draw");
                return;
            }
            if (XMinTextBox.Text != "" && XMaxTextBox.Text != "" && XIncTextBox.Text != "")
            {
                try
                {
                    dp.SetExpression(InputTextBox.Text);
                    UpdateParameters(sender, e);
                }
                catch
                {
                    PrintToOutputLog("Error: Invalid format for plotting");
                }
            }
            else
            {
                PrintToOutputLog("Error: Missing parameters for the graph to draw");
            }

        }

        private void UpdateParameters(object sender, RoutedEventArgs e)
        {
            if ((float.Parse(XMinTextBox.Text) >= float.Parse(XMaxTextBox.Text)) || float.Parse(XIncTextBox.Text) <= 0)
            {
                PrintToOutputLog("Error: Invalid input parameters");
                return;
            }
            if (XMinTextBox.Text != "" && XMaxTextBox.Text != "" && XIncTextBox.Text != "")
            {
                try
                {
                    Plot.Model.Series.Clear();
                    dp.UpdatePlot(dp.GetExpression(), double.Parse(XMinTextBox.Text), double.Parse(XMaxTextBox.Text), double.Parse(XIncTextBox.Text));
                    Plot.Model.InvalidatePlot(true);
                }
                catch
                {
                    PrintToOutputLog("Error: Invalid format for plotting");
                }
            }
            else
            {
                PrintToOutputLog("Error: Missing parameters for the graph to update");
            }
        }

        void PrintToOutputLog(string Message)
        {
            OutputTextBox.Text += ">> " + Message + "\n";
        }

        void DeleteVariable(object sender, RoutedEventArgs e)
        {
            Globals.variables = Globals.variables.Where(item => item.GetName() != (sender as Button).Tag.ToString()).ToList();

            // delete corresponding dockpanel from VariableDockPanel
            List<DockPanel> remove = new List<DockPanel>();
            foreach (var child in VariableStackPanel.Children)
            {
                if ((child.GetType() == typeof(DockPanel)))
                {
                    if ((child as DockPanel).Name == (sender as Button).Tag.ToString())
                        remove.Add(child as DockPanel);
                }
            }
            foreach (var dp in remove)
            {
                VariableStackPanel.Children.Remove(dp as DockPanel);
            }
        }

        public void AddVariableToWindow(string Key, double Value)
        {
            //Create dock panel containing a textblock and a button
            DockPanel dp = new DockPanel();
            dp.Margin = new Thickness(10, 0, 10, 0);
            dp.VerticalAlignment = System.Windows.VerticalAlignment.Top;
            dp.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
            dp.Name = Key;

            TextBlock textBlock = new TextBlock();
            textBlock.Text = Key + " = " + Value + "\n";

            Button button = new Button()
            {
                Content = "Delete",
                Tag = Key
            };
            button.Click += new RoutedEventHandler(DeleteVariable);
            button.VerticalAlignment = System.Windows.VerticalAlignment.Top;
            button.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;
            button.Tag = Key;

            dp.Children.Add(textBlock);
            dp.Children.Add(button);
            VariableStackPanel.Children.Add(dp);
        }

        public void UpdateVariableInWindow(String Key, double NewValue)
        {
            foreach (var child in VariableStackPanel.Children)
            {
                if (child.GetType() == typeof(DockPanel) && (child as DockPanel).Name == Key)
                {
                    foreach (var ch in (child as DockPanel).Children)
                    {
                        if ((ch.GetType() == typeof(TextBlock)))
                        {
                            (ch as TextBlock).Text = Key + " = " + NewValue + "\n";
                        }
                    }
                }
            }
        }

        public void UpdateAllVariablesInWindow()
        {
            VariableStackPanel.Children.Clear();
            foreach (Operand entry in Globals.variables)
            {
                Console.WriteLine("Entry named " + entry.GetName());
                if (entry.GetName() != "e" && entry.GetName() != "π") AddVariableToWindow(entry.GetName(), entry.GetVal());
            }
        }

        private void SaveFile(object sender, RoutedEventArgs e)
        {
            //get output log
            string[] outarray = Regex.Split(WorkspaceTextBox.Text, "\n");
            outarray = outarray.Where(x => !string.IsNullOrEmpty(x)).ToArray();
            //get variables
            string[] vararray = getVariables();
            //get function
            string[] plotarray = { XMinTextBox.Text, XMaxTextBox.Text, XIncTextBox.Text, dp.GetExpression() };
            //save to file
            string[][] savestate = new string[][] { outarray, vararray, plotarray };
            string jsonString = JsonConvert.SerializeObject(savestate);
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Json file (*.Json)|*.Json";
            saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            saveFileDialog.FileName = "save";
            if (saveFileDialog.ShowDialog() == true)
            {
                File.WriteAllText(saveFileDialog.FileName, jsonString);
            }

        }

        private void DiffFunc(object sender, RoutedEventArgs e)
        {
            if (InputTextBox.Text.Length <= 0)
            {
                PrintToOutputLog("Error: Missing function to differentiate");
                return;
            }

            string output = differentiation(InputTextBox.Text, 0, 0.1) + "\n";
            WorkspaceTextBox.Text = WorkspaceTextBox.Text + output;
        }

        private string[] getVariables()
        {
            var varlist = new ArrayList();
            foreach (var child in VariableStackPanel.Children)
            {
                if (child.GetType() == typeof(DockPanel))
                {
                    foreach (var ch in (child as DockPanel).Children)
                    {
                        if ((ch.GetType() == typeof(TextBlock)))
                        {
                            varlist.Add((ch as TextBlock).Text.Replace("\n", ""));
                        }
                    }
                }
            }
            return varlist.ToArray(typeof(string)) as string[];
        }

        private void OpenFile(object sender, RoutedEventArgs e)
        {
            //load file
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                string jsonString = File.ReadAllText(openFileDialog.FileName);
                string[][] savestate = JsonConvert.DeserializeObject<string[][]>(jsonString);
                string[] outarray = savestate[0];
                string[] vararray = savestate[1];
                string[] plotarray = savestate[2];
                NewWorkspace(sender, e);
                //load output
                foreach (string output in outarray)
                {
                    Console.WriteLine(output);

                    WorkspaceTextBox.Text = WorkspaceTextBox.Text + output + "\n";
                }
                //load variables
                foreach (string var in vararray)
                {

                    string[] array = Regex.Split(var, " = ");
                    Console.WriteLine(array[0]);
                    Console.WriteLine(array[1]);
                    AddVariableToWindow(array[0], Convert.ToDouble(array[1]));
                }
                //load function
                XMinTextBox.Text = plotarray[0];
                XMaxTextBox.Text = plotarray[1];
                XIncTextBox.Text = plotarray[2];
                dp.SetExpression(plotarray[3]);

                UpdateParameters(sender, e);
            }
        }
        internal static void GetWindow()
        {
            throw new NotImplementedException();
        }

        private void EnterInput(object sender, RoutedEventArgs e)
        {
            string output = ">> " + InputTextBox.Text + "\n" + Interpreter.InterpretInput(InputTextBox.Text) + "\n";
            // append new line to workspace 
            if (Interpreter.GetbShowOutput())
            {
                WorkspaceTextBox.Text = WorkspaceTextBox.Text + output;
            }
            InputTextBox.Clear();
        }

        private void ClearInput(object sender, RoutedEventArgs e)
        {
            InputTextBox.Clear();
        }

        private void NewWorkspace(object sender, RoutedEventArgs e)
        {
            VariableStackPanel.Children.Clear();

            // clear figures

            WorkspaceTextBox.Clear();

            OutputTextBox.Clear();

            InputTextBox.Clear();

            // clear plot

            XMinTextBox.Text = "-10";
            XMaxTextBox.Text = "10";
            XIncTextBox.Text = "0.1";
            dp.SetExpression("sin(x)");
            UpdateParameters(sender, e);
            XMinTextBox.Clear();
            XMaxTextBox.Clear();
            XIncTextBox.Clear();
        }

        private void SymbolButtonClick(object sender, RoutedEventArgs e)
        {
            String name = (sender as Button).Name;
            InputTextBox.Text += name;
        }

        private void FuncButtonClick(object sender, RoutedEventArgs e)
        {
            String name = (sender as Button).Name;
            InputTextBox.Text += (name + "()");
        }

        private void ClearWorkspace(object sender, RoutedEventArgs e)
        {
            WorkspaceTextBox.Clear();
        }

        private void ClearVariable(object sender, RoutedEventArgs e)
        {
            Globals.variables.Clear();
            VariableStackPanel.Children.Clear();

        }

        private void AnsButtonClick(object sender, RoutedEventArgs e)
        {
            InputTextBox.Text += Interpreter.GetOutput();
        }

        private void ToggleDegRad(object sender, RoutedEventArgs e)
        {
            Globals.rad = !Globals.rad;
            if (Globals.rad)
            {
                (sender as Button).Content = "rad";
            }
            else
            {
                (sender as Button).Content = "deg";
            }
        }

        private void OnKeyDownHandler(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                EnterInput(sender, e);
            }
        }

        private double differentiation(string expression, double Xval, double h)
        {
            Operand XTemp = Globals.variables[xIndex];
            Globals.variables[xIndex].SetVal(Xval);
            double Y = double.Parse(Interpreter.InterpretInput(expression));
            Globals.variables[xIndex].SetVal(Xval + h);
            double Yh = double.Parse(Interpreter.InterpretInput(expression));
            Globals.variables[xIndex] = XTemp;
            return (Y + Yh) / h;
        }

    }

    public class DataPlot
    {
        PlotModel plot;
        string expression;
        public static int xIndex = Globals.variables.FindIndex(item => item.GetName() == "x");
        public static int yIndex = Globals.variables.FindIndex(item => item.GetName() == "y");
        public DataPlot() { }

        public PlotModel MakePlotLineSeries(string expression, double minX, double maxX, double dx)
        {
            this.expression = expression;
            plot = new PlotModel();
            return DrawPlot(expression, minX, maxX, dx);
        }

        public PlotModel UpdatePlot(string expression, double minX, double maxX, double dx)
        {
            plot.Series.Clear();
            return DrawPlot(expression, minX, maxX, dx);
        }

        public PlotModel DrawPlot(string expression, double minX, double maxX, double dx)
        {
            var firstSeries = new LineSeries();
            double Y;
            Operand XTemp = Globals.variables[xIndex];

            for (double i = minX; i <= maxX; i += dx)
            {
                Globals.variables[xIndex].SetVal(i);
                Y = double.Parse(Interpreter.InterpretInput(expression, false));
                firstSeries.Points.Add(new DataPoint(i, Y));
            }
            Globals.variables[xIndex] = XTemp;
            plot.Series.Add(firstSeries);
            return plot;
        }

        public string GetExpression() { return expression; }
        public void SetExpression(string exp)
        {
            expression = exp;
            var mainWindow = App.Current.MainWindow as MainWindow;
            mainWindow.PlotName.Text = expression;
        }
    }
}
