using MagicMazeV1;

class Program
{
    static void Main(string[] args)
    {
        // Initialize game components
        Game game = new Game();
        game.GameTimer = new MagicMazeV1.Timer(300); // 5 minutes timer
        game.Pawns.Add(new Pawn(1, "Red", "Tile1"));
        game.Tiles.Add(new Tile(1, "Start Tile", "None"));

        // Start the game
        game.Start();

        // Example actions
        game.Pawns[0].Move("North");
        game.Pawns[0].TakeEquipment();
        game.Reset();
    }
}