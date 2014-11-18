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
    /// 
    public partial class MainWindow : Window
    {
        private int count_click = 0;
        private double x_mouse, y_mouse, x_depart, y_depart, x_arrive, y_arrive;
        public static string depDate,depTime,retDate,repTime;
        public MainWindow()
        {
            InitializeComponent();
            this.MouseMove += new MouseEventHandler(pictureBox1_MouseUp);
            this.MouseLeftButtonDown += MainWindow_MouseLeftButtonDown;

            mapGrid.Visibility = Visibility.Collapsed;
            ScheduleGrid.Visibility = Visibility.Collapsed;
            TimeGrid.Visibility = Visibility.Collapsed;

            TicketAmount_Update();
            PaymentGrid.Visibility = Visibility.Collapsed;
        }

        #region Map Grid Functions
        void MainWindow_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (HomeGrid.IsVisible)
            {
                HomeGrid.Visibility = Visibility.Collapsed;
                mapGrid.Visibility = Visibility.Visible;
                ScheduleGrid.Visibility = Visibility.Collapsed;
                TimeGrid.Visibility = Visibility.Collapsed;
                PaymentGrid.Visibility = Visibility.Collapsed;
            }
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
            x_mouse = e.GetPosition(mapGrid).X;
            y_mouse = e.GetPosition(mapGrid).Y;      
        }

        private void DepartingCityTextBox_Update(object sender, RoutedEventArgs e)
        {
            String cityName = DepartingCityTextBox.Text;
            object buttonObj = mapGrid.FindName(cityName);
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
            object buttonObj = mapGrid.FindName(cityName);
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
            mapGrid.Children.Add(myLine);
            count_click = 0;
        }

        public void ClearLine()
        {
            if (myLine != null && myLine.IsVisible)
            {
                myLine.Visibility = Visibility.Collapsed;
            }
        }

        public Line myLine { get; set; }

        #endregion

        private void Next_Click(object sender, RoutedEventArgs e)
        {
            ShowScheduleGrid();
        }
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            ClearLine();
            DepartingCityTextBox.Text = "";
            ArrivingCityTextBox.Text = "";

            ShowHomeGrid();
        }
        private void ScheduleNext_Click(object sender, RoutedEventArgs e)
        {
            ShowTimesGrid();
        }
        private void SchedulePrev_Click(object sender, RoutedEventArgs e)
        {
            ShowMapGrid();
        }

        #region Grid Handlers
        private void ShowHomeGrid()
        {
            HomeGrid.Visibility = Visibility.Visible;
            mapGrid.Visibility = Visibility.Collapsed;
            ScheduleGrid.Visibility = Visibility.Collapsed;
            TimeGrid.Visibility = Visibility.Collapsed;
            PaymentGrid.Visibility = Visibility.Collapsed;
        }

        private void ShowMapGrid()
        {
            HomeGrid.Visibility = Visibility.Collapsed;
            mapGrid.Visibility = Visibility.Visible;
            ScheduleGrid.Visibility = Visibility.Collapsed;
            TimeGrid.Visibility = Visibility.Collapsed;
            PaymentGrid.Visibility = Visibility.Collapsed;
        }

        private void ShowScheduleGrid()
        {
            HomeGrid.Visibility = Visibility.Collapsed;
            mapGrid.Visibility = Visibility.Collapsed;
            ScheduleGrid.Visibility = Visibility.Visible;
            TimeGrid.Visibility = Visibility.Collapsed;
            PaymentGrid.Visibility = Visibility.Collapsed;

            Step2Desc.Text = "Showing trips from " + DepartingCityTextBox.Text + " to " + ArrivingCityTextBox.Text + ":";
            
        }

        private void ShowTimesGrid()
        {
            HomeGrid.Visibility = Visibility.Collapsed;
            mapGrid.Visibility = Visibility.Collapsed;
            ScheduleGrid.Visibility = Visibility.Collapsed;
            TimeGrid.Visibility = Visibility.Visible;
            PaymentGrid.Visibility = Visibility.Collapsed;

            AvailableTime.Text = "Choose among available departure times for October "+depDate+" from "+DepartingCityTextBox.Text+ " to "+ArrivingCityTextBox.Text+" :";
        }
        private void ShowPaymentGrid()
        {
            HomeGrid.Visibility = Visibility.Collapsed;
            mapGrid.Visibility = Visibility.Collapsed;
            ScheduleGrid.Visibility = Visibility.Collapsed;
            TimeGrid.Visibility = Visibility.Collapsed;
            PaymentGrid.Visibility = Visibility.Visible;

            AvailableTime.Text = "Choose among available departure times for October " + depDate + " from " + DepartingCityTextBox.Text + " to " + ArrivingCityTextBox.Text + " :";
        }
        #endregion

        #region Dates Buttons Handler
        private void Oct3rdClicked(object sender, RoutedEventArgs e)
        {
            depDate = Oct3rd.Content.ToString();
            ChosenDepDates.Visibility = Visibility.Visible;
            ChosenDepDates.Text = "You choose departure on October "+depDate;
        }
        private void Oct17thClicked(object sender, RoutedEventArgs e)
        {
            depDate = Oct17th.Content.ToString();
            ChosenDepDates.Visibility = Visibility.Visible;
            ChosenDepDates.Text = "You choose departure on October " + depDate;
        }
        private void Oct12thClicked(object sender, RoutedEventArgs e)
        {
            retDate = Oct12th.Content.ToString();
            ChosenRetDates.Visibility = Visibility.Visible;
            ChosenRetDates.Text = "You choose return on October " + retDate;
        }
        private void Oct26thClicked(object sender, RoutedEventArgs e)
        {
            retDate = Oct26th.Content.ToString();
            ChosenRetDates.Visibility = Visibility.Visible;
            ChosenRetDates.Text = "You choose return on October " + retDate;
        }
        #endregion

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ShowPaymentGrid();
        }

        #region Payment Grid Functions

        int TICKET_MIN_AMOUNT = 1;
        int TICKET_MAX_AMOUNT = 99;
        double TICKET_PRICE = 15.00;
        private void TicketDecBtn_Click(object sender, RoutedEventArgs e)
        {
            int ticketAmount = Convert.ToInt32(TicketTextbox.Text);
            if (ticketAmount > TICKET_MIN_AMOUNT)
            {
                int newAmount = --ticketAmount;
                TicketTextbox.Text = newAmount.ToString();
                TicketAmount_Update();
            }
        }

        private void TicketIncBtn_Click(object sender, RoutedEventArgs e)
        {
            int ticketAmount = Convert.ToInt32(TicketTextbox.Text);
            if (ticketAmount < TICKET_MAX_AMOUNT)
            {
                int newAmount = ++ticketAmount;
                TicketTextbox.Text = newAmount.ToString();
                TicketAmount_Update();
            }
        }
        private void TicketAmount_Update()
        {
            int ticketAmount = Convert.ToInt32(TicketTextbox.Text);
            double newTotal = TICKET_PRICE * ticketAmount;
            TotalAmount.Content = "$"+ string.Format("{0:0.00}", newTotal);

            if (ticketAmount == TICKET_MIN_AMOUNT)
            {
                TicketDecBtn.IsEnabled = false;
                TicketDecreaseArrow.Fill = new SolidColorBrush(Color.FromRgb(176, 176, 176)); ;
            }
            else if (ticketAmount == TICKET_MAX_AMOUNT)
            {
                TicketIncBtn.IsEnabled = false;
                TicketIncreaseArrow.Fill = new SolidColorBrush(Color.FromRgb(176, 176, 176));
            }
            else
            {
                TicketDecBtn.IsEnabled = true;
                TicketDecreaseArrow.Fill = new SolidColorBrush(Color.FromRgb(0, 0, 153));
                TicketIncBtn.IsEnabled = true;
                TicketIncreaseArrow.Fill = new SolidColorBrush(Color.FromRgb(0, 0, 153));
            }
        }

        private void TicketAmountTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                int ticketAmount = Convert.ToInt32(TicketTextbox.Text);
                if (ticketAmount < TICKET_MIN_AMOUNT)
                {
                    TicketTextbox.Text = "1";
                }
                TicketAmount_Update();
            }
        }

        #endregion
    }
}
