namespace Domain
{
    public class Player
    {
        public Board Board { get; set; }

        public Player(Board board)
        {
            Board = board;
        }
    }
}