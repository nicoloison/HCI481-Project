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
        private int count_click = 0, ticketCount;
        private double x_mouse, y_mouse, x_depart, y_depart, x_arrive, y_arrive, totalCost;
        public static string depDate,depTime,retDate,repTime;
        public bool optchecked=false, datechecked=false; 
        public MainWindow()
        {
            InitializeComponent();
            this.MouseMove += new MouseEventHandler(pictureBox1_MouseUp);
            this.MouseLeftButtonDown += MainWindow_MouseLeftButtonDown;

            ShowHomeGrid();
            TicketAmount_Update();
        }

        #region CheckBox Options
        //Handles the logic behind the round trip or single trip options.
        //So far, only round trip option is deemed valid for prototyping purpuses. 
        //Choice of trip option is dealt with on first scheduling page (way in).
        private void RoundTrip_Checked(object sender, RoutedEventArgs e) 
        {
            StCheck.IsChecked = false;
            TripOptionsResult.Text = "You choose round trip option.";
            TripOptionsResult.Visibility = Visibility.Visible;
            optchecked = true;
            if (optchecked== true && datechecked==true)
            {
                ScheduleNext.Visibility = Visibility.Visible;
            }
        }

        private void RoundTrip_Unchecked(object sender, RoutedEventArgs e) 
        {
            TripOptionsResult.Visibility = Visibility.Collapsed;
            optchecked = false;
            ScheduleNext.Visibility = Visibility.Collapsed;
        }

        private void SignleTrip_Checked(object sender, RoutedEventArgs e)
        {
            RdtCheck.IsChecked = false;
            TripOptionsResult.Text = "You choose single trip option. This option is not available right now. Please choose round trip option.";
            TripOptionsResult.Visibility = Visibility.Visible;
            ScheduleNext.Visibility = Visibility.Collapsed;
            optchecked = false;
        }

        private void SignleTrip_Unchecked(object sender, RoutedEventArgs e)
        {
            TripOptionsResult.Visibility = Visibility.Collapsed;
        }
        #endregion
        
        #region Map Grid Functions
        //Handles UI elements appearing on map grid
        void MainWindow_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (HomeGrid.IsVisible)
            {
                ShowMapGrid();
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
            Next.Visibility = Visibility.Visible;
        }

        public void ClearLine()
        {
            if (myLine != null)
            {
                myLine.Visibility = Visibility.Collapsed;
            }
            Next.Visibility = Visibility.Collapsed;
        }

        public Line myLine { get; set; }

        #endregion

        #region Next and Previous Buttons
        //Handles conditional appearance of next buttons for each Grid and 
        //previous button options (transistion from one grid to another)
        private void Next_Click(object sender, RoutedEventArgs e)
        {
            ShowScheduleGrid();
        }
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            ShowHomeGrid();
        }

        private void PrevTime_Click(object sender, RoutedEventArgs e)
        {
            ShowScheduleGrid();
        }
        private void ScheduleNext_Click(object sender, RoutedEventArgs e)
        {
            ShowTimesGrid();
        }

        private void ScheduleNextReturn_Click(object sender, RoutedEventArgs e)
        {
            ShowTimesGridReturn();
        }

        private void PrevTimeReturn_Click(object sender, RoutedEventArgs e)
        {
            SchedulePrevReturn.Visibility = Visibility.Visible;
            ShowScheduleGridReturn();
        }

        private void SchedulePrev_Click(object sender, RoutedEventArgs e)
        {
            ShowMapGrid();
        }

        private void SchedulePrevReturn_Click(object sender, RoutedEventArgs e)
        {
            ShowTimesGrid();
        }

        private void NextTimeReturn_Click(object sender, RoutedEventArgs e)
        {
            ShowPaymentGrid();
        }
        private void NextTime_Click(object sender, RoutedEventArgs e)
        {
            ShowScheduleGridReturn();
        }
        #endregion

        #region Grid Handlers
        //Handles UI elemet appearance for each Grid
        private void ShowHomeGrid()
        {
            HomeGrid.Visibility = Visibility.Visible;
            mapGrid.Visibility = Visibility.Collapsed;
            ScheduleGrid.Visibility = Visibility.Collapsed;
            ScheduleGridReturn.Visibility = Visibility.Collapsed;
            TimeGrid.Visibility = Visibility.Collapsed;
            TimeGridReturn.Visibility = Visibility.Collapsed;

            PaymentGrid.Visibility = Visibility.Collapsed;
            PayGrid.Visibility = Visibility.Collapsed;

            TicketGrid.Visibility = Visibility.Collapsed;

            ClearLine();
            count_click = 0;
            totalCost = 0.00;
            depDate = depTime = retDate = repTime = "";
            DepartingCityTextBox.Text = ArrivingCityTextBox.Text = ChosenDepDates.Text = ChosenRetDates.Text = "";
            TimeChoice.Text = TimeChoiceReturn.Text = "";
            TicketTextbox.Text = "1";
            TicketAmount_Update();

            Next.Visibility = Visibility.Collapsed;
            ChosenDepDates.Visibility = Visibility.Collapsed;
            ScheduleNext.Visibility = Visibility.Collapsed;
            ChosenRetDates.Visibility = Visibility.Collapsed;
            NextTimeReturn.Visibility = Visibility.Collapsed;
            NextTime.Visibility = Visibility.Collapsed;
        }

        private void ShowMapGrid()
        {
            HomeGrid.Visibility = Visibility.Collapsed;
            mapGrid.Visibility = Visibility.Visible;
            ScheduleGrid.Visibility = Visibility.Collapsed;
            ScheduleGridReturn.Visibility = Visibility.Collapsed;
            TimeGrid.Visibility = Visibility.Collapsed;
            TimeGridReturn.Visibility = Visibility.Collapsed;

            PaymentGrid.Visibility = Visibility.Collapsed;
            PayGrid.Visibility = Visibility.Collapsed;

            TicketGrid.Visibility = Visibility.Collapsed;
        }

        private void ShowScheduleGrid()
        {
            HomeGrid.Visibility = Visibility.Collapsed;
            mapGrid.Visibility = Visibility.Collapsed;
            ScheduleGrid.Visibility = Visibility.Visible;
            ScheduleGridReturn.Visibility = Visibility.Collapsed;
            TimeGrid.Visibility = Visibility.Collapsed;
            TimeGridReturn.Visibility = Visibility.Collapsed;

            PaymentGrid.Visibility = Visibility.Collapsed;
            PayGrid.Visibility = Visibility.Collapsed;

            TicketGrid.Visibility = Visibility.Collapsed;

            Step2Desc.Text = "Showing trips from " + DepartingCityTextBox.Text + " to " + ArrivingCityTextBox.Text + ":";
            PrevTime.Visibility = Visibility.Visible;
        }

        private void ShowTimesGrid()
        {
            HomeGrid.Visibility = Visibility.Collapsed;
            mapGrid.Visibility = Visibility.Collapsed;
            ScheduleGrid.Visibility = Visibility.Collapsed;
            ScheduleGridReturn.Visibility = Visibility.Collapsed;
            TimeGrid.Visibility = Visibility.Visible;
            TimeGridReturn.Visibility = Visibility.Collapsed;

            Onepm.Visibility = Visibility.Visible;
            Nineam.Visibility = Visibility.Visible;
            Elevenam.Visibility = Visibility.Visible;

            PaymentGrid.Visibility = Visibility.Collapsed;
            PayGrid.Visibility = Visibility.Collapsed;

            TicketGrid.Visibility = Visibility.Collapsed;

            OnepmDetails.Text = "Departing from " + DepartingCityTextBox.Text + " at 1:00pm and arriving in " + ArrivingCityTextBox.Text + " at 8:00pm.";
            NineamDetails.Text = "Departing from " + DepartingCityTextBox.Text+" at 9:00am and arriving in " +ArrivingCityTextBox.Text+" at 4:00m.";
            ElevenamDetails.Text = "Departing from " + DepartingCityTextBox.Text + " at 11:00am and arriving in " + ArrivingCityTextBox.Text + " at 6:00pm.";

            AvailableTime.Text = "Step 3: Choose among available departure times for October " + depDate + " from " + DepartingCityTextBox.Text + " to " + ArrivingCityTextBox.Text + " :";

        }
        private void ShowScheduleGridReturn()
        {
            HomeGrid.Visibility = Visibility.Collapsed;
            mapGrid.Visibility = Visibility.Collapsed;
            ScheduleGrid.Visibility = Visibility.Collapsed;
            ScheduleGridReturn.Visibility = Visibility.Visible;
            TimeGrid.Visibility = Visibility.Collapsed;
            TimeGridReturn.Visibility = Visibility.Collapsed;

            PaymentGrid.Visibility = Visibility.Collapsed;
            PayGrid.Visibility = Visibility.Collapsed;

            TicketGrid.Visibility = Visibility.Collapsed;

            Step4Desc.Text = "Showing trips from " + ArrivingCityTextBox.Text + " to " + DepartingCityTextBox.Text + ":";
            PrevTimeReturn.Visibility = Visibility.Visible;
            SchedulePrevReturn.Visibility = Visibility.Visible;
            AvailableDatesReturn.Text = "";

        }
        private void ShowTimesGridReturn()
        {
            OnepmDetailsReturn.Text = "Departing from " + ArrivingCityTextBox.Text + " at 1:00pm and arriving in " + DepartingCityTextBox.Text + " at 8:00pm.";
            NineamDetailsReturn.Text = "Departing from " + ArrivingCityTextBox.Text + " at 9:00am and arriving in " + DepartingCityTextBox.Text + " at 4:00m.";
            ElevenamDetailsReturn.Text = "Departing from " + ArrivingCityTextBox.Text + " at 11:00am and arriving in " + DepartingCityTextBox.Text + " at 6:00pm.";

            HomeGrid.Visibility = Visibility.Collapsed;
            mapGrid.Visibility = Visibility.Collapsed;
            ScheduleGrid.Visibility = Visibility.Collapsed;
            ScheduleGridReturn.Visibility = Visibility.Collapsed;
            TimeGrid.Visibility = Visibility.Collapsed;
            TimeGridReturn.Visibility = Visibility.Visible;

            OnepmReturn.Visibility = Visibility.Visible;
            OnepmDetailsReturn.Visibility = Visibility.Visible;
            ElevenamReturn.Visibility = Visibility.Visible;
            NineamReturn.Visibility = Visibility.Visible;
            SchedulePrevReturn.Visibility = Visibility.Collapsed;

            PaymentGrid.Visibility = Visibility.Collapsed;
            PayGrid.Visibility = Visibility.Collapsed;

            TicketGrid.Visibility = Visibility.Collapsed;
           
            AvailableTimeReturn.Text = "Step 5: Choose among available arrival times for October " + retDate + " from " + ArrivingCityTextBox.Text + " to " + DepartingCityTextBox.Text + " :";
            TimeChoiceReturn.Text = "";
        }
        private void ShowPaymentGrid()
        {
            HomeGrid.Visibility = Visibility.Collapsed;
            mapGrid.Visibility = Visibility.Collapsed;
            ScheduleGrid.Visibility = Visibility.Collapsed;
            TimeGrid.Visibility = Visibility.Collapsed;
            PaymentGrid.Visibility = Visibility.Visible;
            PayGrid.Visibility = Visibility.Collapsed;
            TicketGrid.Visibility = Visibility.Collapsed;

            LeavingLabel.Text = "Leaving " + DepartingCityTextBox.Text + " on October " + depDate + " at " + depTime + ".";
            ArrivingLabel.Text = "Arriving at " + ArrivingCityTextBox.Text + " on October " + retDate + " at " + repTime + ".";
		}
        private void ShowPayGrid(String type)
        {
            HomeGrid.Visibility = Visibility.Collapsed;
            mapGrid.Visibility = Visibility.Collapsed;
            ScheduleGrid.Visibility = Visibility.Collapsed;
            TimeGrid.Visibility = Visibility.Collapsed;
            PaymentGrid.Visibility = Visibility.Collapsed;
            PayGrid.Visibility = Visibility.Visible;
            TicketGrid.Visibility = Visibility.Collapsed;

            if (type == "card")
            {
                PayText.Text = "Please insert, swipe, or tap your card on the credit card reader now.";
            }
            else
            {
                PayText.Text = "Please insert your cash and coins now.";
            }

            ticketCount = Convert.ToInt32(TicketTextbox.Text);
            totalCost = ticketCount * TICKET_PRICE;

            PayLeavingText.Text = "Leaving " + DepartingCityTextBox.Text + " on October " + depDate + " at " + depTime + ".";
            PayArrivingText.Text = "Arriving at " + ArrivingCityTextBox.Text + " on October " + retDate + " at " + repTime + ".";
            PayAmountText.Text = "$ " + string.Format("{0:0.00}", totalCost);
            NumberTicketsText.Text = "Number of Tickets: "+ ticketCount.ToString() +" ($"+TICKET_PRICE+" each)";
        }
        private void ShowTicketsGrid()
        {
            HomeGrid.Visibility = Visibility.Collapsed;
            mapGrid.Visibility = Visibility.Collapsed;
            ScheduleGrid.Visibility = Visibility.Collapsed;
            ScheduleGridReturn.Visibility = Visibility.Collapsed;
            TimeGrid.Visibility = Visibility.Collapsed;
            TimeGridReturn.Visibility = Visibility.Collapsed;

            OnepmReturn.Visibility = Visibility.Collapsed;
            OnepmDetailsReturn.Visibility = Visibility.Collapsed;
            ElevenamReturn.Visibility = Visibility.Collapsed;
            NineamReturn.Visibility = Visibility.Collapsed;
            SchedulePrevReturn.Visibility = Visibility.Collapsed;

            PaymentGrid.Visibility = Visibility.Collapsed;
            PayGrid.Visibility = Visibility.Collapsed;

            TicketGrid.Visibility = Visibility.Visible;
        }

		#endregion

        #region Dates Buttons Handler
        //Handles behavior of date options on schedule grids, 
        //such as two dates cannot be chosen together, graying out of chosen date
        //and updating of system memory
        private void Oct3rdButtonClicked(object sender, RoutedEventArgs e)
        {
            Oct3rd.Visibility = Visibility.Collapsed;
            Oct3rdClicked.Visibility = Visibility.Visible;

            Oct17th.Visibility = Visibility.Visible;
            Oct17thClicked.Visibility = Visibility.Collapsed;

            depDate = Oct3rd.Content.ToString();
            ChosenDepDates.Visibility = Visibility.Visible;

            ChosenDepDates.Text = "You chose departure on October " + depDate + ".";
            datechecked = true;
            if (optchecked == true && datechecked == true)
            {
                ScheduleNext.Visibility = Visibility.Visible;
            } 
        }
        private void Oct17thButtonClicked(object sender, RoutedEventArgs e)
        {
            Oct3rd.Visibility = Visibility.Visible;
            Oct3rdClicked.Visibility = Visibility.Collapsed;

            Oct17th.Visibility = Visibility.Collapsed;
            Oct17thClicked.Visibility = Visibility.Visible;

            depDate = Oct17th.Content.ToString();
            ChosenDepDates.Visibility = Visibility.Visible;
        
            datechecked = true;
            if (optchecked == true && datechecked == true)
            {
                ScheduleNext.Visibility = Visibility.Visible;
            }
            ChosenDepDates.Text = "You chose departure on October " + depDate + ".";
        }
        private void Oct12thButtonClicked(object sender, RoutedEventArgs e)
        {
            Oct12th.Visibility = Visibility.Collapsed;
            Oct12thClicked.Visibility = Visibility.Visible;

            Oct26th.Visibility = Visibility.Visible;
            Oct26thClicked.Visibility = Visibility.Collapsed;

            retDate = Oct12th.Content.ToString();
            ChosenRetDates.Visibility = Visibility.Visible;
            ScheduleNextReturn.Visibility = Visibility.Visible;
            ChosenRetDates.Text = "You chose return on October " + retDate + ".";
        }
        private void Oct26thButtonClicked(object sender, RoutedEventArgs e)
        {
            Oct12th.Visibility = Visibility.Visible;
            Oct12thClicked.Visibility = Visibility.Collapsed;

            Oct26th.Visibility = Visibility.Collapsed;
            Oct26thClicked.Visibility = Visibility.Visible;

            retDate = Oct26th.Content.ToString();
            ChosenRetDates.Visibility = Visibility.Visible;
            ScheduleNextReturn.Visibility = Visibility.Visible;
            ChosenRetDates.Text = "You chose return on October " + retDate + ".";
        }

        #endregion

        #region Time Buttons Handlers
        //Handles behavior of time options on time grid, 
        //such as two times cannot be chosen together, graying out of chosen time
        //and updating of system memory
        private void OnepmReturn_button_clicked(object sender, RoutedEventArgs e)
        {
            ElevenamReturnClicked.Visibility = Visibility.Collapsed;
            ElevenamReturn.Visibility = Visibility.Visible;

            NineamReturnClicked.Visibility = Visibility.Collapsed;
            NineamReturn.Visibility = Visibility.Visible;

            OnepmReturn.Visibility = Visibility.Collapsed;
            OnepmReturnClicked.Visibility = Visibility.Visible;

            repTime = "1:00 pm";
            TimeChoiceReturn.Text = "You chose the departure time of " + repTime + ".\n\nClick Next to validate time.";
            NextTimeReturn.Visibility = Visibility.Visible;
        }
        private void NineamReturn_button_clicked(object sender, RoutedEventArgs e)
        {
            ElevenamReturnClicked.Visibility = Visibility.Collapsed;
            ElevenamReturn.Visibility = Visibility.Visible;

            NineamReturnClicked.Visibility = Visibility.Visible;
            NineamReturn.Visibility = Visibility.Collapsed;

            OnepmReturn.Visibility = Visibility.Visible;
            OnepmReturnClicked.Visibility = Visibility.Collapsed;

            repTime = "9:00 am";
            TimeChoiceReturn.Text = "You chose the departure time of " + repTime + ".\n\nClick Next to validate time.";
            NextTimeReturn.Visibility = Visibility.Visible;
        }
        private void ElevenamReturn_button_clicked(object sender, RoutedEventArgs e)
        {
            ElevenamReturnClicked.Visibility = Visibility.Visible;
            ElevenamReturn.Visibility = Visibility.Collapsed;
            
            NineamReturnClicked.Visibility = Visibility.Collapsed;
            NineamReturn.Visibility = Visibility.Visible;

            OnepmReturn.Visibility = Visibility.Visible;
            OnepmReturnClicked.Visibility = Visibility.Collapsed;
            
            repTime = "11:00 am";
            TimeChoiceReturn.Text = "You chose the departure time of " + repTime + ".\n\nClick Next to validate time.";
            NextTimeReturn.Visibility = Visibility.Visible;
        }

        private void Onepm_button_clicked(object sender, RoutedEventArgs e)
        {
            ElevenamClicked.Visibility = Visibility.Collapsed;
            Elevenam.Visibility = Visibility.Visible;

            NineamClicked.Visibility = Visibility.Collapsed;
            Nineam.Visibility = Visibility.Visible;

            Onepm.Visibility = Visibility.Collapsed;
            OnepmClicked.Visibility = Visibility.Visible;

            depTime = "1:00 pm";
            TimeChoice.Text = "You chose departure at " + depTime + ".\n\nClick Next to validate time.";
            NextTime.Visibility = Visibility.Visible;
        }
        private void Nineam_button_clicked(object sender, RoutedEventArgs e)
        {
            ElevenamClicked.Visibility = Visibility.Collapsed;
            Elevenam.Visibility = Visibility.Visible;

            NineamClicked.Visibility = Visibility.Visible;
            Nineam.Visibility = Visibility.Collapsed;

            Onepm.Visibility = Visibility.Visible;
            OnepmClicked.Visibility = Visibility.Collapsed;
            depTime = "9:00 am";
            TimeChoice.Text = "You chose departure at " + depTime + ".\n\nClick Next to validate time.";
            NextTime.Visibility = Visibility.Visible;
        }
        private void Elevenam_button_clicked(object sender, RoutedEventArgs e)
        {
            ElevenamClicked.Visibility = Visibility.Visible;
            Elevenam.Visibility = Visibility.Collapsed;

            NineamClicked.Visibility = Visibility.Collapsed;
            Nineam.Visibility = Visibility.Visible;

            Onepm.Visibility = Visibility.Visible;
            OnepmClicked.Visibility = Visibility.Collapsed;
            depTime = "11:00 am";
            TimeChoice.Text = "You chose departure at " + depTime + ".\n\nClick Next to validate time.";
            NextTime.Visibility = Visibility.Visible;
        }

        #endregion

        #region Payment Grid Functions
        //Handles Payment grid and behavior of UI element
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
        private void PaymentPrevious_Click(object sender, RoutedEventArgs e)
        {
            ShowTimesGrid();
        }

        private void CardBtn_Click(object sender, RoutedEventArgs e)
        {
            ShowPayGrid("card");
        }
        private void CashBtn_Click(object sender, RoutedEventArgs e)
        {
            ShowPayGrid("cash");
        }

        private void PayPrevious_Click(object sender, RoutedEventArgs e)
        {
            ShowPaymentGrid();
        }

        private void PayNext_Click(object sender, RoutedEventArgs e)
        {
            ShowTicketsGrid();
        }

        #endregion

    }
}
