﻿using System;
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
        public static string depDate,depTime,depTimeArrival,retDate,repTime,repTimeArrival;
        public bool optchecked=false, datechecked=false, roundTrip=false;

        public string[] cities = {  
                                     "Winnipeg", 
                                     "Victoria", 
                                     "Edmonton", 
                                     "Whitehorse", 
                                     "Yellowknife", 
                                     "Iqaluit", 
                                     "StJohns",
                                     "Halifax", 
                                     "Toronto", 
                                     "Charlottetown",
                                     "Regina",
                                     "Quebec",
                                     "Ottawa", 
                                     "Fredericton",
                                 };

        public MainWindow()
        {
            InitializeComponent();
            this.MouseMove += new MouseEventHandler(pictureBox1_MouseUp);
            this.MouseLeftButtonDown += MainWindow_MouseLeftButtonDown;
            this.WindowsBar.MouseDown += WindowsBar_MouseDown;
            // Add cities to combobox selection in alphabetical order
            Array.Sort(cities);
            foreach(string city in cities){
                DepartingCityTextBox.Items.Add(city);
                ArrivingCityTextBox.Items.Add(city);
            }
            ShowHomeGrid();
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
            roundTrip = true;
            if (optchecked== true && datechecked==true)
            {
                ScheduleNext.Visibility = Visibility.Visible;
            }
        }

        private void RoundTrip_Unchecked(object sender, RoutedEventArgs e) 
        {
            TripOptionsResult.Visibility = Visibility.Collapsed;
            optchecked = false;
            roundTrip = false;
            ScheduleNext.Visibility = Visibility.Collapsed;
        }

        private void SignleTrip_Checked(object sender, RoutedEventArgs e)
        {
            RdtCheck.IsChecked = false;
            roundTrip = false;
            TripOptionsResult.Text = "You choose single trip option.";
            TripOptionsResult.Visibility = Visibility.Visible;
            ScheduleNext.Visibility = Visibility.Collapsed;
            optchecked = true;
            if (optchecked == true && datechecked == true)
            {
                ScheduleNext.Visibility = Visibility.Visible;
            }
        }

        private void SignleTrip_Unchecked(object sender, RoutedEventArgs e)
        {
            optchecked = false;
            TripOptionsResult.Visibility = Visibility.Collapsed;
            ScheduleNext.Visibility = Visibility.Collapsed;
        }

        private void ResetCheckboxOptions()
        {
            RdtCheck.IsChecked = false;
            StCheck.IsChecked = false;
            optchecked = false;
            roundTrip = false;
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
        void WindowsBar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void City_button_clicked(Object sender, RoutedEventArgs e)
        {
            Button clickedButton = (Button)sender;

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
            if (ArrivingCityTextBox.Text == DepartingCityTextBox.Text)
            {
                if (count_click == 1)
                {
                    LeavingFromError.Visibility = Visibility.Visible;
                    LeavingFromError.ToolTip = "Leaving from and going to cities are the same. Please specify two different cities.";
                }
                else
                {
                    GoingToError.Visibility = Visibility.Visible;
                    GoingToError.ToolTip = "Leaving from and going to cities are the same. Please specify two different cities.";
                }
                Next.Visibility = Visibility.Collapsed;
            }
            else if ((ArrivingCityTextBox.SelectedIndex != -1 && DepartingCityTextBox.SelectedIndex != -1) || (ArrivingCityTextBox.Text != "" && DepartingCityTextBox.Text != ""))
            {
                LeavingFromError.Visibility = Visibility.Collapsed;
                GoingToError.Visibility = Visibility.Collapsed;
                Next.Visibility = Visibility.Visible;
            }
         }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            x_mouse = e.GetPosition(mapGrid).X;
            y_mouse = e.GetPosition(mapGrid).Y;      
        }

        private void DepartingCityTextBox_Update(object sender, System.EventArgs e)
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
                if (ArrivingCityTextBox.Text == DepartingCityTextBox.Text)
                {
                    LeavingFromError.Visibility = Visibility.Visible;
                    LeavingFromError.ToolTip = "Leaving from and going to cities are the same. Please specify two different cities.";
                    Next.Visibility = Visibility.Collapsed;
                }
                else if ((ArrivingCityTextBox.SelectedIndex != -1 && DepartingCityTextBox.SelectedIndex != -1) || (ArrivingCityTextBox.Text != "" && DepartingCityTextBox.Text != ""))
                {
                    LeavingFromError.Visibility = Visibility.Collapsed;
                    GoingToError.Visibility = Visibility.Collapsed;
                    Next.Visibility = Visibility.Visible;
                }
            }
        }

        private void ArrivingCityTextBox_Update(object sender, System.EventArgs e)
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
                if (ArrivingCityTextBox.Text == DepartingCityTextBox.Text)
                {
                    GoingToError.Visibility = Visibility.Visible;
                    GoingToError.ToolTip = "Leaving from and going to cities are the same. Please specify two different cities.";
                    Next.Visibility = Visibility.Collapsed;
                }
                else if ((ArrivingCityTextBox.SelectedIndex != -1 && DepartingCityTextBox.SelectedIndex != -1) || (ArrivingCityTextBox.Text != "" && DepartingCityTextBox.Text != ""))
                {
                    LeavingFromError.Visibility = Visibility.Collapsed;
                    GoingToError.Visibility = Visibility.Collapsed;
                    Next.Visibility = Visibility.Visible;
                }
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

        private void ArrivingCityTextBox_Closed(object sender, System.EventArgs e)
        {
            ArrivingCityTextBox_Update(sender, e);
        }

        private void DepartingCityTextBox_Closed(object sender, System.EventArgs e)
        {
            DepartingCityTextBox_Update(sender, e);
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

        private void ResetMapGrid()
        {
            ClearLine();
            count_click = 0;
            DepartingCityTextBox.Text = ArrivingCityTextBox.Text = ChosenDepDates.Text = ChosenRetDates.Text = "";
            Next.Visibility = Visibility.Collapsed;
            mapGrid.Visibility = Visibility.Collapsed;
        }

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
            if (roundTrip)
            {
                ShowScheduleGridReturn();
            }
            else
            {
                ShowPaymentGrid();
            }
        }
        #endregion

        #region Grid Handlers
        //Handles UI elemet appearance for each Grid
        private void ShowHomeGrid()
        {
            HomeGrid.Visibility = Visibility.Visible;

            ResetMapGrid();
            ResetScheduleButtonClicked();
            ResetCheckboxOptions();
            ResetTimeButtonClicked();
            ResetPaymentGrid();
            ResetTicketGrid();
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

            OnepmDetails.Text = "Departing on October " + depDate + " at 1:00 pm\nArriving on October " + depDate + " at 8:00 pm";
            NineamDetails.Text = "Departing on October " + depDate + " at 9:00 am\nArriving on October " + depDate + " at 4:00 pm";
            ElevenamDetails.Text = "Departing on October " + depDate + " at 11:00 am\nArriving on October " + depDate + " at 6:00 pm";

            AvailableTimeSub.Text = "Showing available times for " + ArrivingCityTextBox.Text + " to " + DepartingCityTextBox.Text + " on October " + depDate + ":";

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

            OnepmDetailsReturn.Text = "Departing on October " + retDate + " at 1:00 pm\nArriving on October " + retDate + " at 8:00 pm";
            NineamDetailsReturn.Text = "Departing on October " + retDate + " at 9:00 am\nArriving on October " + retDate + " at 4:00 pm";
            ElevenamDetailsReturn.Text = "Departing on October " + retDate + " at 11:00 am\nArriving on October " + retDate + " at 6:00 pm";

            AvailableTimeReturnSub.Text = "Showing available times for " + DepartingCityTextBox.Text + " to " + ArrivingCityTextBox.Text + " on October " + retDate + ":";
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

            if (roundTrip)
            {
                PaymentStepTitle.Content = "Step 6: Payment";
                TicketLabel.Text = "How many tickets do you require? ($" + TICKET_PRICE_RNDTRP + " each)";

                LeavingLabel.Text = "";
                ArrivingLabel.Text = "";

                DepartureTrip.Text = "Departure Trip:\nLeaving " + DepartingCityTextBox.Text + " on Oct. " + depDate + " at " + depTime + "\nArriving at " + ArrivingCityTextBox.Text + " on Oct. " + depDate + " at " + depTimeArrival + ".";
                ReturnTrip.Text = "Return Trip:\nArriving " + ArrivingCityTextBox.Text + " on Oct. " + retDate + " at " + repTime + "\nArriving at " + DepartingCityTextBox.Text + " on Oct. " + retDate + " at " + repTimeArrival + ".";
            }
            else
            {
                PaymentStepTitle.Content = "Step 4: Payment";
                TicketLabel.Text = "How many tickets do you require? ($" + TICKET_PRICE + " each)";

                LeavingLabel.Text = "Leaving " + DepartingCityTextBox.Text + " on October " + depDate + " at " + depTime + ".";
                ArrivingLabel.Text = "Arriving at " + ArrivingCityTextBox.Text + " on October " + depDate + " at " + depTimeArrival + ".";

                DepartureTrip.Text = "";
                ReturnTrip.Text = "";
            }

            TicketAmount_Update();
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

            if (roundTrip)
            {
                totalCost = ticketCount * TICKET_PRICE_RNDTRP;
                NumberTicketsText.Text = "Number of Tickets: " + ticketCount.ToString() + " ($" + TICKET_PRICE_RNDTRP + " each)";

                PayOneWayTrip.Visibility = Visibility.Collapsed;
                PayLeavingText.Text = "";
                PayLeavingText.Text = "";

                PayRoundTrip.Visibility = Visibility.Visible;
                PayDepartureTrip.Text = "Departure Trip:\nLeaving " + DepartingCityTextBox.Text + " on Oct. " + depDate + " at " + depTime + "\nArriving at " + ArrivingCityTextBox.Text + " on Oct. " + depDate + " at " + depTimeArrival + ".";
                PayReturnTrip.Text = "Return Trip:\nArriving " + ArrivingCityTextBox.Text + " on Oct. " + retDate + " at " + repTime + "\nArriving at " + DepartingCityTextBox.Text + " on Oct. " + retDate + " at " + repTimeArrival + ".";
            }
            else
            {
                totalCost = ticketCount * TICKET_PRICE;
                NumberTicketsText.Text = "Number of Tickets: " + ticketCount.ToString() + " ($" + TICKET_PRICE + " each)";
                
                PayOneWayTrip.Visibility = Visibility.Visible;
                PayLeavingText.Text = "Leaving " + DepartingCityTextBox.Text + " on October " + depDate + " at " + depTime + ".";
                PayArrivingText.Text = "Arriving at " + ArrivingCityTextBox.Text + " on October " + depDate + " at " + depTimeArrival + ".";
                
                PayRoundTrip.Visibility = Visibility.Collapsed;
                DepartureTrip.Text = "";
                ReturnTrip.Text = "";
            }

            PayAmountText.Text = "$ " + string.Format("{0:0.00}", totalCost);
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
        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
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
        private void ResetScheduleButtonClicked()
        {
            // Departure Trip
            Oct3rd.Visibility = Visibility.Visible;
            Oct3rdClicked.Visibility = Visibility.Collapsed;

            Oct17th.Visibility = Visibility.Visible;
            Oct17thClicked.Visibility = Visibility.Collapsed;

            ScheduleNext.Visibility = Visibility.Collapsed;
            ChosenDepDates.Visibility = Visibility.Collapsed;

            depDate = "";
            datechecked = false;
            ScheduleGrid.Visibility = Visibility.Collapsed;

            // Return Trip
            Oct12th.Visibility = Visibility.Visible;
            Oct12thClicked.Visibility = Visibility.Collapsed;

            Oct26th.Visibility = Visibility.Visible;
            Oct26thClicked.Visibility = Visibility.Collapsed;

            ScheduleNextReturn.Visibility = Visibility.Collapsed;
            ChosenRetDates.Visibility = Visibility.Collapsed;

            retDate = "";
            ScheduleGridReturn.Visibility = Visibility.Collapsed;
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

            repTimeArrival = "8:00 pm";
            TimeChoiceReturn.Text = "You chose the departure of " + repTime + " and arrival of " + repTimeArrival + ".\n\nClick Next to validate time.";

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
//<<<<<<< HEAD
            TimeChoiceReturn.Text = "You chose the departure time of " + repTime + ".\n\nClick Next to validate time.";
//=======
            repTimeArrival = "4:00 pm";
            TimeChoiceReturn.Text = "You chose the departure of " + repTime + " and arrival of " + repTimeArrival + ".\n\nClick Next to validate time.";
//>>>>>>> a1c391a980d4b1d111ea86ee7ada280aa82951c0
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
            repTime = "11:00 am";
            repTimeArrival = "6:00 pm";
            TimeChoiceReturn.Text = "You chose the departure of " + repTime + " and arrival of " + repTimeArrival + ".\n\nClick Next to validate time.";

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
            depTimeArrival = "8:00 pm";
            TimeChoice.Text = "You chose the departure of " + depTime + " and arrival of " + depTimeArrival + ".\n\nClick Next to validate time.";
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
            depTimeArrival = "4:00 pm";
            TimeChoice.Text = "You chose the departure of " + depTime + " and arrival of " + depTimeArrival + ".\n\nClick Next to validate time.";
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
            depTimeArrival = "6:00 pm";
            TimeChoice.Text = "You chose the departure of " + depTime + " and arrival of " + depTimeArrival + ".\n\nClick Next to validate time.";
            NextTime.Visibility = Visibility.Visible;
        }
        private void ResetTimeButtonClicked()
        {
            // Departure Trip
            Elevenam.Visibility = Visibility.Visible;
            ElevenamClicked.Visibility = Visibility.Collapsed;

            Nineam.Visibility = Visibility.Visible;
            NineamClicked.Visibility = Visibility.Collapsed;

            Onepm.Visibility = Visibility.Visible;
            OnepmClicked.Visibility = Visibility.Collapsed;

            depTime = depTimeArrival = TimeChoice.Text = "";
            NextTime.Visibility = Visibility.Collapsed;
            TimeGrid.Visibility = Visibility.Collapsed;

            // Return Trip
            ElevenamReturn.Visibility = Visibility.Visible;
            ElevenamReturnClicked.Visibility = Visibility.Collapsed;

            NineamReturn.Visibility = Visibility.Visible;
            NineamReturnClicked.Visibility = Visibility.Collapsed;

            OnepmReturn.Visibility = Visibility.Visible;
            OnepmReturnClicked.Visibility = Visibility.Collapsed;

            repTime = repTimeArrival = TimeChoiceReturn.Text = "";
            NextTimeReturn.Visibility = Visibility.Visible;
            TimeGridReturn.Visibility = Visibility.Collapsed;
        }

        #endregion

        #region Payment Grid Functions
        //Handles Payment grid and behavior of UI element
        int TICKET_MIN_AMOUNT = 1;
        int TICKET_MAX_AMOUNT = 99;
        double TICKET_PRICE = 15.00;
        double TICKET_PRICE_RNDTRP = 25.00;
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

            double newTotal = TICKET_PRICE;
            if (roundTrip)
            {
                newTotal = TICKET_PRICE_RNDTRP * ticketAmount;
            }
            else
            {
                newTotal = TICKET_PRICE * ticketAmount;
            }
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
            if (roundTrip)
            {
                ShowTimesGridReturn();
            }
            else
            {
                ShowTimesGrid();
            }
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

        private void ResetPaymentGrid()
        {
            ticketCount = TICKET_MIN_AMOUNT;
            totalCost = ticketCount * TICKET_PRICE;
            TicketTextbox.Text = TICKET_MIN_AMOUNT.ToString();
            TicketAmount_Update();

            PaymentGrid.Visibility = Visibility.Collapsed;
            PayGrid.Visibility = Visibility.Collapsed;
        }

        #endregion

        #region TicketGridMethods

        private void ResetTicketGrid()
        {
            TicketGrid.Visibility = Visibility.Collapsed;
        }

        private void TicketDone_Click(object sender, System.Windows.RoutedEventArgs e)
        {
        	ShowHomeGrid();
        }

        #endregion
    }
}
