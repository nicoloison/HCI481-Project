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
        int count_click = 0;
        double x_mouse, y_mouse;
        public MainWindow()
        {
            InitializeComponent();
            this.MouseMove += new MouseEventHandler(pictureBox1_MouseUp);
        }

        private void City_button_clicked(Object sender, RoutedEventArgs e)
        {
            System.Windows.Controls.Button clickedButton = (System.Windows.Controls.Button)sender;
            count_click++;
            double xdep=0, ydep=0, xarr, yarr;
            if (count_click==1) 
            {
                xdep = x_mouse;
                ydep = y_mouse;
            }

            if (count_click == 2) 
            {
                count_click=0;
                xarr = x_mouse;
                yarr = y_mouse;
                DrawWay(xdep, ydep, xarr, yarr);
            }
            Console.WriteLine("\nBUTTON CLICKED " + clickedButton.Name + "\n\n");
            Console.WriteLine("COUNT_CLICK " + count_click + "\n\n");
         }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            x_mouse = e.GetPosition(myGrid).X;
            y_mouse = e.GetPosition(myGrid).Y;      
        }

        private void DrawWay(double xdep, double ydep, double xarr, double yarr) 
        {
            myLine = new Line();
            myLine.Stroke = System.Windows.Media.Brushes.LightBlue;
            myLine.X1 = xdep;
            myLine.X2 = xarr;
            myLine.Y1 = ydep;
            myLine.Y2 = yarr;
            myLine.StrokeThickness = 5;
            myGrid.Children.Add(myLine);
        }


        public Line myLine { get; set; }
    }
}
