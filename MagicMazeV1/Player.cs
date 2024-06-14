using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagicMazeV1
{
    public class Player
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public Card Card { get; set; }

        public Player(int id, string name, Card card)
        {
            ID = id;
            Name = name;
            Card = card;
        }

        public void ExecuteAction()
        {
            Card.PerformAction();
        }
    }
}
