using System.Reflection;
namespace Skocko
{
    public partial class MainPage : ContentPage
    {
        int row = 0;

        int[] combination = new int[4];
        int[] guess = new int[4];

        Random rnd = new Random();
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

        private void checkButton_Clicked(object sender, EventArgs e)
        {
            // Display the solution and guesses in alerts for testing
            correct.Text = string.Join(" ", combination);

            CheckValidity();

            MoveButton();

            row++;
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

            //delete afterwards
            DisplayAlert(red.ToString(), yellow.ToString(), "OK");
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
            }
        }

        private void MoveButton()
        {
            Button button = this.FindByName<Button>("checkButton");

            if (button != null)
            {
                int currentRow = Grid.GetRow(button);

                Grid.SetRow(button, currentRow + 1);
            }
            button.IsVisible = false;
        }
    }
}