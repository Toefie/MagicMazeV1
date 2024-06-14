using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagicMazeV1
{
    public class Tile
    {
        public int ID { get; set; }
        public string TileName { get; set; }
        public string SpecialPlace { get; set; }

        public Tile(int id, string tileName, string specialPlace)
        {
            ID = id;
            TileName = tileName;
            SpecialPlace = specialPlace;
        }

        public void Explore()
        {
            // Implement logic to explore the tile
        }
    }
}
