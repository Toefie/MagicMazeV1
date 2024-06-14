using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagicMazeV1
{
    public class Pawn
    {
        public int ID { get; set; }
        public string Colour { get; set; }
        public string CurrentTile { get; set; }
        public DAL.DAL dal;

        public Pawn(int id, string colour, string currentTile)
        {
            ID = id;
            Colour = colour;
            CurrentTile = currentTile;
            dal = new DAL.DAL(); // Initialize the DAL instance
        }

        public void Move(string direction)
        {
            // Implement logic to move the pawn in the specified direction
            // After moving the pawn, save the move to the database
            dal.SaveMoveToDatabase(ID, direction);
        }

        public void TakeEquipment()
        {
            // Implement logic to take equipment
        }

        public void UpdateCurrentTile(string newTile)
        {
            CurrentTile = newTile;
            dal.UpdateCurrentTile(ID, newTile);
        }
    }
}

