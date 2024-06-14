using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace MagicMazeV1
{
    public class Game
    {
        public string StartGame { get; set; }
        public List<Player> Players { get; set; } = new List<Player>();
        public List<Pawn> Pawns { get; set; } = new List<Pawn>();
        public Timer Timer { get; set; }

        public Game()
        {
            // Initialisatie code
        }

        public void Save()
        {
            DAL.DAL dal = new DAL.DAL();
            dal.SaveGame(this);
        }

        public static Game Load(int gameId)
        {
            DAL.DAL dal = new DAL.DAL();
            return dal.LoadGame(gameId);
        }
    }
}

