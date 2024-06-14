using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagicMazeV1
{
    public class Space
    {
        public string Type { get; set; }
        public string Position { get; set; }

        public Space(string type, string position)
        {
            Type = type;
            Position = position;
        }

        public void Activate()
        {
            // Implement logic to activate the space
        }
    }
}
