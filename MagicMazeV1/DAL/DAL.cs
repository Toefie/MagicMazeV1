using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace MagicMazeV1.DAL
{
    public class DAL
    {
        private string connectionString = "Data Source=.;Initial Catalog=magicmaze;Integrated Security=true";

        public void SaveMoveToDatabase(int pawnID, string direction)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "INSERT INTO Moves (PawnID, Direction, MoveTime) VALUES (@PawnID, @Direction, @MoveTime)";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@PawnID", pawnID);
                command.Parameters.AddWithValue("@Direction", direction);
                command.Parameters.AddWithValue("@MoveTime", DateTime.Now);

                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        public List<string> GetMovesFromDatabase(int pawnID)
        {
            List<string> moves = new List<string>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT Direction FROM Moves WHERE PawnID = @PawnID";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@PawnID", pawnID);

                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    moves.Add(reader.GetString(0));
                }
            }

            return moves;
        }

        public void UpdateCurrentTile(int pawnID, string newTile)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "UPDATE Pawns SET CurrentTile = @CurrentTile WHERE PawnID = @PawnID";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@CurrentTile", newTile);
                command.Parameters.AddWithValue("@PawnID", pawnID);

                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        public static Pawn GetPawnFromDatabase(int id, string connectionString)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT PawnID, Colour, CurrentTile FROM Pawns WHERE PawnID = @PawnID";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@PawnID", id);

                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    return new Pawn(reader.GetInt32(0), reader.GetString(1), reader.GetString(2));
                }
                return null;
            }
        }

        public void SaveToDatabase(Pawn pawn)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "INSERT INTO Pawns (Colour, CurrentTile) VALUES (@Colour, @CurrentTile)";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Colour", pawn.Colour);
                command.Parameters.AddWithValue("@CurrentTile", pawn.CurrentTile);

                connection.Open();
                command.ExecuteNonQuery();

                // Get the ID of the newly inserted pawn
                query = "SELECT @@IDENTITY";
                command = new SqlCommand(query, connection);
                pawn.ID = Convert.ToInt32(command.ExecuteScalar());
            }
        }

        public static void DeletePawnFromDatabase(int id, string connectionString)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "DELETE FROM Pawns WHERE PawnID = @PawnID";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@PawnID", id);

                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        public void CreatePlayer(Player player)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "INSERT INTO Players (Name) VALUES (@Name)";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Name", player.Name);
                    command.ExecuteNonQuery();
                }
            }
        }

        public bool DeletePlayer(Player player)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // Delete associated orderlines first
                    string deletePlayerQuery = "DELETE FROM Players WHERE Name = @Name";
                    using (SqlCommand deletePlayerCommand = new SqlCommand(deletePlayerQuery, connection))
                    {
                        deletePlayerCommand.Parameters.AddWithValue("@Name", player.Name);
                        int rowsAffected = deletePlayerCommand.ExecuteNonQuery();

                        // Check if any rows were affected (person was deleted)
                        return rowsAffected > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred while deleting the player: " + ex.Message);
                return false; // Return false to indicate deletion failure
            }
        }

        public void SaveGame(Game game)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlTransaction transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // Save Game
                        string gameQuery = "INSERT INTO Games (StartGame) OUTPUT INSERTED.GameID VALUES (@StartGame)";
                        SqlCommand gameCommand = new SqlCommand(gameQuery, connection, transaction);
                        gameCommand.Parameters.AddWithValue("@StartGame", game.StartGame);
                        int gameId = (int)gameCommand.ExecuteScalar();

                        // Save Players
                        foreach (Player player in game.Players)
                        {
                            string playerQuery = "INSERT INTO Players (GameID, Name) VALUES (@GameID, @Name)";
                            SqlCommand playerCommand = new SqlCommand(playerQuery, connection, transaction);
                            playerCommand.Parameters.AddWithValue("@GameID", gameId);
                            playerCommand.Parameters.AddWithValue("@Name", player.Name);
                            playerCommand.ExecuteNonQuery();
                        }

                        // Save Pawns
                        foreach (Pawn pawn in game.Pawns)
                        {
                            string pawnQuery = "INSERT INTO Pawns (GameID, Colour, CurrentTile) OUTPUT INSERTED.PawnID VALUES (@GameID, @Colour, @CurrentTile)";
                            SqlCommand pawnCommand = new SqlCommand(pawnQuery, connection, transaction);
                            pawnCommand.Parameters.AddWithValue("@GameID", gameId);
                            pawnCommand.Parameters.AddWithValue("@Colour", pawn.Colour);
                            pawnCommand.Parameters.AddWithValue("@CurrentTile", pawn.CurrentTile);
                            int pawnId = (int)pawnCommand.ExecuteScalar();

                            // Save Moves
                            foreach (string move in GetMovesFromDatabase(pawnId))
                            {
                                string moveQuery = "INSERT INTO Moves (PawnID, Direction, MoveTime) VALUES (@PawnID, @Direction, @MoveTime)";
                                SqlCommand moveCommand = new SqlCommand(moveQuery, connection, transaction);
                                moveCommand.Parameters.AddWithValue("@PawnID", pawnId);
                                moveCommand.Parameters.AddWithValue("@Direction", move);
                                moveCommand.Parameters.AddWithValue("@MoveTime", DateTime.Now);
                                moveCommand.ExecuteNonQuery();
                            }
                        }

                        // Save Timer
                        string timerQuery = "INSERT INTO Timers (GameID, Duration, RemainingTime) VALUES (@GameID, @Duration, @RemainingTime)";
                        SqlCommand timerCommand = new SqlCommand(timerQuery, connection, transaction);
                        timerCommand.Parameters.AddWithValue("@GameID", gameId);
                        timerCommand.Parameters.AddWithValue("@Duration", game.Timer.Duration);
                        timerCommand.Parameters.AddWithValue("@RemainingTime", game.Timer.RemainingTime);
                        timerCommand.ExecuteNonQuery();

                        transaction.Commit();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        // Load the entire game state from the database
        public Game LoadGame(int gameId)
        {
            Game game = new Game();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // Load Game
                string gameQuery = "SELECT StartGame FROM Games WHERE GameID = @GameID";
                SqlCommand gameCommand = new SqlCommand(gameQuery, connection);
                gameCommand.Parameters.AddWithValue("@GameID", gameId);
                SqlDataReader gameReader = gameCommand.ExecuteReader();
                if (gameReader.Read())
                {
                    game.StartGame = gameReader.GetString(0);
                }
                gameReader.Close();


                // Load Players
                string playerQuery = "SELECT PlayerID, Name, CardID FROM Players WHERE GameID = @GameID";
                SqlCommand playerCommand = new SqlCommand(playerQuery, connection);
                playerCommand.Parameters.AddWithValue("@GameID", gameId);
                SqlDataReader playerReader = playerCommand.ExecuteReader();
                while (playerReader.Read())
                {
                    int playerId = playerReader.GetInt32(0);
                    string playerName = playerReader.GetString(1);
                    int cardId = playerReader.GetInt32(2);

                    // Assuming you have a method to get a Card object by its ID
                    Card playerCard = GetCardById(cardId);

                    Player player = new Player(playerId, playerName, playerCard);
                    game.Players.Add(player);
                }
                playerReader.Close();

                // Load Pawns
                string pawnQuery = "SELECT PawnID, Colour, CurrentTile FROM Pawns WHERE GameID = @GameID";
                SqlCommand pawnCommand = new SqlCommand(pawnQuery, connection);
                pawnCommand.Parameters.AddWithValue("@GameID", gameId);
                SqlDataReader pawnReader = pawnCommand.ExecuteReader();
                while (pawnReader.Read())
                {
                    int pawnId = pawnReader.GetInt32(0);
                    Pawn pawn = new Pawn
                    {
                        ID = pawnId,
                        Colour = pawnReader.GetString(1),
                        CurrentTile = pawnReader.GetString(2)
                    };
                    game.Pawns.Add(pawn);

                    // Load Moves for each Pawn
                    List<string> moves = GetMovesFromDatabase(pawnId);
                    foreach (var move in moves)
                    {
                        pawn.AddMove(move); // Assuming Pawn has an AddMove method to add moves to it
                    }
                }
                pawnReader.Close();

                // Load Timer
                string timerQuery = "SELECT Duration, RemainingTime FROM Timers WHERE GameID = @GameID";
                SqlCommand timerCommand = new SqlCommand(timerQuery, connection);
                timerCommand.Parameters.AddWithValue("@GameID", gameId);
                SqlDataReader timerReader = timerCommand.ExecuteReader();
                if (timerReader.Read())
                {
                    game.Timer = new Timer(timerReader.GetInt32(0))
                    {
                        RemainingTime = timerReader.GetInt32(1)
                    };
                }
                timerReader.Close();
            }

            return game;
        }

        public Card GetCardById(int cardId)
        {
            // Implement the logic to retrieve the Card object by its ID from the database
            // This is just a placeholder implementation
            return new Card { ID = cardId, /* set other properties */ };
        }
    }
}
    