using System;
using System.Data.SqlClient;

namespace MagicMazeV1
{
    public class Timer
    {
        public int Duration { get; set; }
        public int RemainingTime { get; set; }
        private readonly string _connectionString;

        public Timer(int duration, string connectionString)
        {
            Duration = duration;
            RemainingTime = duration;
            _connectionString = connectionString;
        }

        public void Start()
        {
            // Implement logic to start the timer
        }

        public void Reset()
        {
            RemainingTime = Duration;
            UpdateDatabase();
        }

        public void Decrement()
        {
            if (RemainingTime > 0)
            {
                RemainingTime--;
                
            }
        }
    }
}
