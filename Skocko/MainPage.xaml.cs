using Microsoft.Maui.Controls;
using System.Reflection;
namespace Skocko
{
    public partial class MainPage : ContentPage
    {
        int row = 0;

        int[] combination = new int[4];
        int[] guess = new int[4];

        Random rnd = new Random();

        //timer
        private int timerSeconds = 90;
        private bool isTimerRunning = false;

        bool won = false;
        public MainPage()
        {
            InitializeComponent();

            Button[] symbols = { symbol1, symbol2, symbol3, symbol4, symbol5, symbol6 };

            foreach (var symbol in symbols)
            {
                symbol.Clicked += SymbolClicked;
            }

            for (int i = 1; i <= 24; i++)
            {
                Button button = this.FindByName<Button>("button" + i);

                if (button != null)
                {
                    button.Clicked += FieldClicked;
                }
            }

            GenerateCombination();
        }

        private void GenerateCombination()
        {
            for (int i = 0; i < 4; i++)
            {
                combination[i] = rnd.Next(0, 6);
            }
        }

        private void FieldClicked(object sender, EventArgs e)
        {
            Button button = sender as Button; 

            if (button != null)
            {
                button.Text = null;
                checkButton.IsVisible = false;
            }
        }

        private void SymbolClicked(object sender, EventArgs e)
        {
            int index = row * 4;

            for (int i = 1; i <= 4; i++)
            {
                int buttonNumber = index + i;
                string buttonName = "button" + buttonNumber;

                // Use FindByName to get the button by name
                Button button = this.FindByName<Button>(buttonName);

                if (button != null && button.Text == null)
                {
                    Button symb = sender as Button;
                    button.Text = symb.Text;

                    guess[i - 1] = ReturnIndexOfSymbol(button.Text);

                    bool isEverythingFilled = true;

                    for (int j = 1; j <= 4; j++)
                    {
                        Button btn = this.FindByName<Button>("button" + (index + j));

                        if (btn.Text == null)
                        {
                            isEverythingFilled = false;
                            break;
                        }
                    }
                    checkButton.IsVisible = isEverythingFilled;

                    return;
                }
            }
        }

        private int ReturnIndexOfSymbol(string s)
        {
            int num = 0;
            switch (s)
            {
                case "🦉":
                    num = 0; break;
                case "♣":
                    num = 1; break;
                case "♠":
                    num = 2; break;
                case "❤":
                    num = 3; break;
                case "♦":
                    num = 4; break;
                case "🌟":
                    num = 5; break;
            }
            return num;
        }

        private string ReturnSymbolAtIndex(int index)
        {
            string symbol = string.Empty;
            switch (index)
            {
                case 0:
                    symbol = "🦉"; break;
                case 1:
                    symbol = "♣"; break;
                case 2:
                    symbol = "♠"; break;
                case 3:
                    symbol = "❤"; break;
                case 4:
                    symbol = "♦"; break;
                case 5:
                    symbol = "🌟"; break;
            }
            return symbol;
        }


        private void checkButton_Clicked(object sender, EventArgs e)
        {
            // Display the solution and guesses in alerts for testing
            //correct.Text = string.Join(" ", combination);

            CheckValidity();

            MoveButton();

            DisableButtons();

            row++;

            if (row == 6 && won == false)
            {
                EndGame();
            }
        }

        private void CheckValidity()
        {
            int red = 0, yellow = 0;

            //Count reds
            for(int i = 0; i < 4; i++)
            {
                if (combination[i] == guess[i])
                    red++;
            }

            int[] combinationCopy = (int[])combination.Clone();

            //Count yellows
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    if (combinationCopy[j] == guess[i])
                    {
                        yellow++;
                        combinationCopy[j] = '0';
                        break;
                    }
                }
            }
            yellow = Math.Max(0, yellow - red);

            DisplayRedAndYellow(red, yellow);

        }

        private void DisplayRedAndYellow(int red, int yellow)
        {
            string show = "   ";

            for (int i = 0; i < red; i++)
            {
                show += "🔴";
            }
            for (int i = 0; i < yellow; i++)
            {
                show += "🟡";
            }
            for (int i = red + yellow; i < 4; i++)
            {
                show += "⚫";
            }

            string labelName = "results" + (row + 1);

            Label label = this.FindByName<Label>(labelName);

            if (label != null)
            {
                label.Text = show;
                label.IsVisible = true;

                if (red == 4)
                {
                    EndGame(red);
                    won = true;
                }
            }
        }

        private void MoveButton()
        {
            Button button = this.FindByName<Button>("checkButton");

            if (button != null)
            {
                Grid.SetRow(button, row + 1);
            }
            button.IsVisible = false;
        }

        private void DisableButtons()
        {
            for (int i = 1; i <= 4; i++)
            {
                Button button = this.FindByName<Button>("button" + (row * 4 + i));
                
                if (button != null)
                {
                    button.IsEnabled = false;
                }
            }
        }

        private void DisableSymbols()
        {
            for (int i = 1; i <= 6; i++)
            {
                Button button = this.FindByName("symbol" + i) as Button;

                if (button != null)
                {
                    button.IsEnabled = false;
                }
            }
        }

        private void EndGame()
        {
            string reason = string.Empty;

            if (timerSeconds <= 0)
            {
                reason = "Time is up.";
                DisableButtons();
                checkButton.IsVisible = false;
            }
            else
            {
                reason = "You have no attempts left.";
            }

            StopTimer();
            DisableSymbols();

            won = true;

            DisplayAlert("Game Over", reason + "\n" + " You earned 0 points", "Ok");

            DisplayCombination();

            replay.IsVisible = true;
        }

        private void EndGame(int red)
        {
            if (red != 4)
                return;

            StopTimer();

            int points = CalculatePoints();

            DisplayAlert("Congratulations", "You earned " + points + " points", "Ok");

            DisableSymbols();

            DisplayCombination();

            replay.IsVisible = true;
        }

        private int CalculatePoints()
        {
            if (row <= 3)
                return 20;
            else if (row == 4)
                return 15;
            else if (row == 5)
                return 10;
            else
                return 0;
        }

        private void DisplayCombination()
        {
            HorizontalStackLayout stackLayout = this.FindByName<HorizontalStackLayout>("rightCombinationLayout");
            stackLayout.IsVisible = true;

            solutionLine.IsVisible = true;

            // Set IsVisible to true to make it visible
            stackLayout.IsVisible = true;

            for (int i = 1; i <= 4; i++)
            {
                Button button = this.FindByName<Button>("solution" + i) as Button;

                if (button != null)
                {
                    button.Text = ReturnSymbolAtIndex(combination[i - 1]);
                }
            }
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            StartTimer();
        }

        // Stop the timer when the page disappears
        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            StopTimer();
        }

        private void StartTimer()
        {
            isTimerRunning = true;

            Device.StartTimer(TimeSpan.FromSeconds(1), () =>
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    timeLabel.Text = $"Timer: {timerSeconds}";
                });

                if (timerSeconds <= 0 || !isTimerRunning)
                {
                    if (timerSeconds <= 0)
                        EndGame();

                    return false; // Stop the timer
                }

                timerSeconds--;
                return true; // Continue the timer
            });
        }

        private void StopTimer()
        {
            isTimerRunning = false;
        }

        private void ResetButtons()
        {
            for (int i = 1; i <= 24; i++)
            {
                Button button = this.FindByName<Button>("button" + i);

                if (button != null)
                {
                    button.IsEnabled = true;
                    button.Text = null;
                }
            }
        }

        private void EnableSymbols()
        {
            for (int i = 1; i <= 6; i++)
            {
                Button button = this.FindByName<Button>("symbol" + i);

                if (button != null)
                {
                    button.IsEnabled = true;
                }
            }
        }
        private void ClearLabels()
        {
            for (int i = 1; i <= 6; i++)
            {
                Label label = this.FindByName<Label>("results" + i);

                if (label != null)
                {
                    label.Text = null;
                }
            }
        }

        private void ResetCheckButton()
        {
            Button button = this.FindByName<Button>("checkButton");

            if (button != null)
            {
                Grid.SetRow(button, 0);
            }
            button.IsVisible = false;
        }

        private void Replay(object sender, EventArgs e)
        {
            ResetButtons();
            EnableSymbols();
            ClearLabels();
            ResetCheckButton();

            solutionLine.IsVisible= false;
            rightCombinationLayout.IsVisible = false;


            row = 0;
            won = false;

            timerSeconds = 90;
            isTimerRunning = false;

            GenerateCombination();
            StartTimer();

            replay.IsVisible = false;
        }
    }
}