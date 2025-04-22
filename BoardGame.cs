namespace Learning
{
    public class BoardGame
    {
        public BoardGame(int? id, string? name, int? year, int? min, int? max) 
        {
            Id = id;
            Name = name;
            Year = year;
            MinPlayers = min; 
            MaxPlayers = max;
        }

        public int? Id { get; set; }
        public string? Name { get; set; }
        public int? Year { get; set; }
        public int? MinPlayers { get; set; }
        public int? MaxPlayers { get; set; }
    }
}
