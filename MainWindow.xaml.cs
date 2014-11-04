using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BusApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private int count_click = 0;
        private double x_mouse, y_mouse, x_depart, y_depart, x_arrive, y_arrive;
        
        public MainWindow()
        {
            InitializeComponent();
            this.MouseMove += new MouseEventHandler(pictureBox1_MouseUp);
        }

        private void City_button_clicked(Object sender, RoutedEventArgs e)
        {
            System.Windows.Controls.Button clickedButton = (System.Windows.Controls.Button)sender;

            count_click++;
            if (count_click == 1)
            {
                ClearLine();
                x_depart = x_mouse;
                y_depart = y_mouse;
                DepartingCityTextBox.Text = clickedButton.Name;
                ArrivingCityTextBox.Text = "";
            }
            else if (count_click == 2) 
            {
                x_arrive = x_mouse;
                y_arrive = y_mouse;
                DrawWay();
                ArrivingCityTextBox.Text = clickedButton.Name;
            }
         }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            x_mouse = e.GetPosition(myGrid).X;
            y_mouse = e.GetPosition(myGrid).Y;      
        }

        private void DepartingCityTextBox_Update(object sender, RoutedEventArgs e)
        {
            String cityName = DepartingCityTextBox.Text;
            object buttonObj = myGrid.FindName(cityName);
            if (buttonObj != null)
            {
                Button button = (Button)buttonObj;
                Point point = button.TransformToAncestor(this).Transform(new Point(0, 0));
                x_depart = point.X + 10;
                y_depart = point.Y + 10;
                count_click = 1;
                if (ArrivingCityTextBox.Text != "")
                {
                    DrawWay();
                }
            }
        }

        private void ArrivingCityTextBox_Update(object sender, RoutedEventArgs e)
        {
            String cityName = ArrivingCityTextBox.Text;
            object buttonObj = myGrid.FindName(cityName);
            if (buttonObj != null)
            {
                Button button = (Button)buttonObj;
                Point point = button.TransformToAncestor(this).Transform(new Point(0, 0));
                x_arrive = point.X + 10;
                y_arrive = point.Y + 10;
                DrawWay();
            }
        }

        private void DepartingCityTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                DepartingCityTextBox_Update(DepartingCityTextBox, e);
            }
        }

        private void ArrivingCityTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                ArrivingCityTextBox_Update(ArrivingCityTextBox, e);
            }
        }

        private void DrawWay() 
        {
            ClearLine();
            myLine = new Line();
            myLine.Stroke = System.Windows.Media.Brushes.LightBlue;
            myLine.X1 = x_depart;
            myLine.X2 = x_arrive;
            myLine.Y1 = y_depart;
            myLine.Y2 = y_arrive;
            myLine.StrokeThickness = 5;
            myGrid.Children.Add(myLine);
            count_click = 0;
        }

        public void ClearLine()
        {
            if (myLine != null && myLine.IsVisible)
            {
                myLine.Visibility = System.Windows.Visibility.Collapsed;
            }
        }

        public Line myLine { get; set; }
    }
}
