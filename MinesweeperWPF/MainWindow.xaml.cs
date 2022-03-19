using Minesweeper;
using System;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace MinesweeperWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Game myGame;
        private Timer timer;
        private int timeElapsed;
        private Button button;
        private int nbCasesRevealed;
        private int nbSafeCases;
        public MainWindow()
        {
            InitializeComponent();
            setChrono();
            startGame();
        }

        private void setChrono()
        {
            timer = new Timer();
            timer.Interval = 1000;
            timer.Elapsed += timer_Tick;
        }
        private void startGame()
        {
            this.myGame = new Game();
            this.timeElapsed = 0;
            this.button = null;
            this.nbCasesRevealed = 0;
            this.nbSafeCases = (myGame.length * myGame.length) - myGame.nbMines;
            displayNbMines();
            createGrid();
            this.timer.Start();

        }

        private void displayNbMines()
        {
            this.nbMinesLabel.Content = this.myGame.nbMines;
        }

        void timer_Tick(object sender, EventArgs e)
        {
            this.Dispatcher.BeginInvoke(new Action(() =>
                this.time.Content = timeElapsed++
            ));
        }

        private void endGame(Boolean win)
        {
            this.timer.Stop();
            this.revealBombs();
            if (win) MessageBox.Show("Tu as gagné en " + (timeElapsed - 1) + " secondes.");
            else MessageBox.Show("Tu as perdu.");
            this.startGame();

        }

        private void revealBombs()
        {
            for (int i = 0; i < myGame.length; i++)
            {
                for (int j = 0; j < myGame.length; j++)
                {
                    int caseValue = myGame.board[i, j];
                    if (caseValue >= 10)
                    {
                        string name = String.Format("btn_{0}_{1}", i, j);
                        button = (Button)this.FindName(name);
                        this.setImageToButton(caseValue);
                    }
                }
            }
        }

        private void createGrid()
        {
            this.myBoard.RowDefinitions.Clear();
            this.myBoard.ColumnDefinitions.Clear();
            for (int i = 0; i < myGame.board.GetLength(0); i++)
            {
                this.myBoard.RowDefinitions.Add(new RowDefinition());
                for (int j = 0; j < myGame.board.GetLength(1); j++)
                {
                    if (j == 0) this.myBoard.ColumnDefinitions.Add(new ColumnDefinition());
                    addButton(i, j);
                }
            }
        }

        private void addButton(int i, int j)
        {
            Button button = new Button()
            {
                Tag = new Tuple<int, int>(i, j),
                Background = new SolidColorBrush(Color.FromRgb(128, 128, 128)),
                Name = String.Format("btn_{0}_{1}", i, j)
            };
            button.PreviewMouseLeftButtonDown += OnClickLeft;
            button.PreviewMouseRightButtonDown += manageFlag;
            Grid.SetColumn(button, i);
            Grid.SetRow(button, j);
            this.myBoard.Children.Add(button);
            if (this.FindName(button.Name) == null) RegisterName(button.Name, button);
        }

        private void OnClickLeft(object sender, MouseButtonEventArgs e)
        {
            button = sender as Button;
            Tuple<int, int> tag = (Tuple<int, int>)button.Tag;
            int line = tag.Item1;
            int column = tag.Item2;
            int caseValue = myGame.board[line, column];
            setImageToButton(caseValue);
            if (caseValue == 0) revealAjdCases(line, column);
            else if (caseValue >= 10) this.endGame(false);

        }

        public void revealAjdCases(int line, int column)
        {

            int maxLength = myGame.length - 1;
            int prevColumn = column == 0 ? 0 : column - 1;
            int prevLine = line == 0 ? 0 : line - 1;
            int nextLine = line == maxLength ? maxLength : line + 1;
            int nextColumn = column == maxLength ? maxLength : column + 1;
            for (int i = prevLine; i <= nextLine; i++)
            {
                for (int j = prevColumn; j <= nextColumn; j++)
                {
                    int caseValue = myGame.board[i, j];
                    bool isAdjacent = (i == prevLine && j == prevColumn)
                                            || (i == prevLine && j == nextColumn)
                                            || (i == nextLine && j == prevColumn)
                                            || (i == nextLine && j == nextColumn);
                    if ((isAdjacent && caseValue < 8) || caseValue < 10)
                    {
                        string name = String.Format("btn_{0}_{1}", i, j);
                        button = (Button)this.FindName(name);
                        setImageToButton(caseValue);
                        if (caseValue == 0)
                        {
                            myGame.board[i, j] = 8; //set a value to declare the case has been revealed
                            revealAjdCases(i, j);
                        }

                    }

                }
            }

        }

        public void setImageToButton(int caseValue)
        {
            if (button.Content == null)
            {
                string imageName = caseValue >= 10 ? "bomb" : this.getBombOrFlag(caseValue);
                string uri = String.Format("C:\\Users\\Khasinjy\\source\\repos\\Minesweeper\\Minesweeper\\resources\\{0}.png", imageName);
                Image image = new Image();
                image.Source = new BitmapImage(new Uri(uri, UriKind.Absolute));
                button.Content = image;
                disabledButton(caseValue);
                if (caseValue != 9) this.nbCasesRevealed++;
                if (this.nbCasesRevealed == this.nbSafeCases && caseValue < 10) this.endGame(true);
            }

        }

        public void setHiddenImageToButton()
        {
            button.Background = new SolidColorBrush(Color.FromRgb(128, 128, 128));
            button.Content = null;
            button.PreviewMouseLeftButtonDown += OnClickLeft;
        }

        private string getBombOrFlag(int caseValue)
        {
            return caseValue == 9 ? "flag" : caseValue.ToString();
        }

        private void disabledButton(int caseValue)
        {
            button.PreviewMouseLeftButtonDown -= OnClickLeft;
            if (caseValue != 9)
            {
                button.PreviewMouseRightButtonDown -= manageFlag;
                button.IsEnabled = false;
            }
        }

        private void manageFlag(object sender, MouseButtonEventArgs e)
        {
            button = sender as Button;
            Tuple<int, int> tag = (Tuple<int, int>)button.Tag;
            int line = tag.Item1;
            int column = tag.Item2;
            int caseValue = myGame.board[line, column];
            if (button.Content != null && caseValue == 9)
            {
                int oldCaseValue = myGame.flagged[new Tuple<int, int>(line, column)];
                myGame.flagged.Remove(new Tuple<int, int>(line, column));
                myGame.board[line, column] = oldCaseValue;
                this.setHiddenImageToButton();
            }
            else
            {
                myGame.flagged.Add(new Tuple<int, int>(line, column), caseValue);
                myGame.board[line, column] = 9;
                this.setImageToButton(9);
            }
        }
    }
}
