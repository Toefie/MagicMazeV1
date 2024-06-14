using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagicMazeV1
{
    public class Card
    {
        public string ActionType { get; set; }

        public Card(string actionType)

        {
            ActionType = actionType;
        }

        public void PerformAction()
        {
            // Implement logic to perform the action
        }
    }
}
