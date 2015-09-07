using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace _3dsnake
{
    public class Game
    {
        public int GameSize { get; private set; }
        public int Matches { get; private set; }
        public GameMode Mode { get; private set; }
        public List<Player> PlayerList { get; private set; }
        public event PlayerMovementEventHandler DoMovement;
        public event PlayerHasLostEventHandler PlayerHasLost;
        public ScoreBoard ScoreBoard { get; set; }

        public Game ( List<Player> players, GameMode mode, int size, int matches )
        {
            this.PlayerList = players;
            this.Mode = mode;
            this.GameSize = size;
            this.Matches = matches;
            this.ScoreBoard = new ScoreBoard();
            this.ScoreBoard.InitializeBoard( this.PlayerList );
        }

        public void StartGame( )
        {
            Matchmaking.Initialize( this.PlayerList, this.Mode, this.Matches );
            DoMatch();   
        }

        private void DoMatch()
        {
            Match m = new Match( GameSize, Matchmaking.GetNextMatch() );
            m.DoMovement += m_DoMovement;
            m.PlayerHasLost += m_PlayerHasLost;
            m.MatchEnded += m_MatchEnded;
            System.Threading.Thread t = new System.Threading.Thread( new System.Threading.ParameterizedThreadStart( bw_DoWork ) );
            t.Start( m );            
        }



        void m_MatchEnded( object sender, MatchEndedEventArgs e )
        {
            if ( e.IsValid )
            {
                this.ScoreBoard.UpdatePlayer( e.Match.Players.Single( x => x.IsAlive ).PlayerCode, 1, e.Match.Players.Single( x => x.IsAlive ).Score + 1 );

                playersThatHaveLost.ForEach( x => this.ScoreBoard.UpdatePlayer( x.Player.PlayerCode, 0, x.Player.Score ) );
            }
            else
            {
                Matchmaking.InvalidMatch();
            }

            System.Threading.Thread.Sleep( 1000 );

            playersThatHaveLost.Clear();

            if ( Matchmaking.DoMoreMatches )
                DoMatch();
        }

        void bw_DoWork( object o )
        {
            Match m = o as Match;
            m.Start();
        }

        List<PlayerHasLostEventArgs> playersThatHaveLost = new List<PlayerHasLostEventArgs>();

        bool m_PlayerHasLost( object sender, PlayerHasLostEventArgs e )
        {
            if ( PlayerHasLost != null )
            {
                playersThatHaveLost.Add( e );
                return PlayerHasLost( sender, e );
            }
            return true;
        }

        void m_DoMovement( object sender, PlayerMovementEventArgs e )
        {
            if ( DoMovement != null )
            {
                DoMovement( sender, e );
            }
        }
    }

    public delegate void PlayerMovementEventHandler ( object sender, PlayerMovementEventArgs e );
    public delegate bool PlayerHasLostEventHandler( object sender, PlayerHasLostEventArgs e );
    public delegate void MatchEndedEventHandler ( object sender, MatchEndedEventArgs e );

    public class MatchEndedEventArgs
    {
        public Match Match { get; private set; }
        public bool IsValid { get; private set; }

        public MatchEndedEventArgs  ( Match match, bool valid  )
        {
            this.Match = match;
            this.IsValid = valid;
        }
    }

    public class PlayerHasLostEventArgs
    {
        public Player Player { get; private set; }
        public List<Player> Players { get; private set; }
        public Match Match { get; private set; }

        public PlayerHasLostEventArgs ( Player player, List<Player> players, Match match )
        {
            this.Player = player;
            this.Players = players;
            this.Match = match;
        }
    }

    public class PlayerMovementEventArgs 
    {
        public Player Player { get; private set; }
        public Point Point { get; private set; }
        public bool IsHead { get; private set; }

        public PlayerMovementEventArgs ( Player player, Point point, bool head )
        {
            this.Player = player;
            this.Point = point;
            this.IsHead = head;
        }
    }


    public enum Direction
    {
        Up,
        Down,
        Front,
        Bottom,
        Left,
        Right
    }

    public enum GameMode
    {        
        Arena,
        OneVersusOne
    }
}
