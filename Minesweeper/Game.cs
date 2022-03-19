namespace Minesweeper
{
    public class Game
    {

        public int[,] board { get; set; }
        public int nbMines { get; set; }
        public int length { get; set; }

        public Dictionary<Tuple<int,int>, int> flagged { get; set; }

        public Game()
        {
            this.nbMines = 6;
            this.length = 10;
            this.board = new int[this.length, this.length];
            this.flagged = new Dictionary<Tuple<int,int>, int>();
            initBoard();
            placeMines();
        }
        private void initBoard()
        {
            for (int i = 0; i < this.length; i++)
            {
                for (int j = 0; j < this.length; j++)
                {
                    this.board[i, j] = 0;
                }
            }

        }

        private void placeMines()
        {
            Random rnd = new Random();
            for (int i = 0; i < this.nbMines; i++)
            {
                int line = rnd.Next(0, this.length);
                int column = rnd.Next(0, this.length);
                int value = this.board[line, column];
                if (value >= 10) i--;
                else
                {
                    this.board[line, column] = 10;
                    putProximityNumber(line, column);
                }
            }
        }

        private void putProximityNumber(int line, int column)
        {
            int maxLength = this.length - 1;
            int prevColumn = column == 0 ? 0 : column - 1;
            int prevLine = line == 0 ? 0 : line - 1;
            int nextLine = line == maxLength ? maxLength : line + 1;
            int nextColumn = column == maxLength ? maxLength : column + 1;
            for (int i = prevLine; i <= nextLine; i++)
            {
                for (int j = prevColumn; j <= nextColumn; j++)
                {
                    this.board[i, j]++;
                }
            }
        }
    }


}