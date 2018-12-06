namespace Domain
{
    public class Player
    {
        public Board Board { get; set; }
        public string Name { get; set; }

        public Player(Board board, string name)
        {
            Name = name;
            Board = board;
        }
    }
}