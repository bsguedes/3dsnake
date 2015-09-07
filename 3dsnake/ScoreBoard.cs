using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace _3dsnake
{
    public class ScoreBoard
    {
        public List<ScoreBoardEntry> Entries { get; set; }

        public void InitializeBoard( List<Player> players )
        {
            this.Entries = new List<ScoreBoardEntry>();
            foreach ( var player in players )
            {
                this.Entries.Add( new ScoreBoardEntry { PlayerName = player.PlayerInfo.PlayerName, PlayerCode = player.PlayerCode, Color = GetColorName ( player.Color ) } );
            }
        }

        static Dictionary<Color, string> colorToString = new Dictionary<Color, string>()
        {
            {Colors.Red, "Red"},
            {Colors.DodgerBlue, "Blue"},
            {Colors.Green, "Green"},
            {Colors.Yellow, "Yellow"},
            {Colors.Purple, "Purple"},
            {Colors.Magenta, "Magenta"},
            {Colors.Brown, "Brown"},
            {Colors.Blue , "Dark Blue" }
                
        };

        private string GetColorName( Color color )
        {
            return colorToString[ color ];
        }

        public void UpdatePlayer ( int playerCode, int wins, int points )
        {
            var p = this.Entries.First( x => x.PlayerCode == playerCode );
            p.Wins += wins;
            p.Cubes += points;
            p.Matches++;
        }
    }

    public class ScoreBoardEntry
    {
        public int PlayerCode { get; set; }
        public string PlayerName { get; set; }
        public int Wins { get; set; }
        public int Losses { get { return this.Matches - this.Wins; } }
        public int Cubes { get; set; }
        public int Matches { get; set; }
        public string Color { get; set; }
    }
}
