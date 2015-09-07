using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace _3dsnake
{
    public class Match
    {        
        public int Size { get; set; }
        public List<Player> Players { get; set; }
        public int[][][] Cube { get; set; }
        public event PlayerMovementEventHandler DoMovement;
        public event PlayerHasLostEventHandler PlayerHasLost;
        public event MatchEndedEventHandler MatchEnded;

        public Match ( int gameSize, List<Player> players )
        {            
            this.Size = gameSize;
            this.Players = players.Shuffle().ToList();
            this.Cube = new int[ this.Size ][][];
            for ( int i = 0; i < this.Size; i++ )
            {
                this.Cube[ i ] = new int[ this.Size ][];
            }
            for ( int i = 0; i < this.Size; i++ )
            {
                for ( int j = 0; j < this.Size; j++ )
                {
                    this.Cube[ i ][ j ] = new int[ this.Size ]; 
                }
            }
        }

        public void Start () 
        {
            foreach ( var player in this.Players )
            {
                player.Score = 1;
                player.IsAlive = true;
                player.Head = GenerateStartingPosition();
                this.Cube[ player.Head.X ][ player.Head.Y ][ player.Head.Z ] = player.PlayerCode;
                if ( DoMovement != null )
                {
                    DoMovement( this, new PlayerMovementEventArgs( player, player.Head, true ) );
                }                
            }

            while ( this.Players.Where( x => x.IsAlive ).Count() > 1 )
            {
                foreach ( var player in this.Players.Where ( x => x.IsAlive ) )
                {
                    while ( player.IsPlaying ) continue;
                    player.IsPlaying = true;
                    Direction dir = player.PreviousDirection;
                    DateTime start = DateTime.Now;
                    Task turn = new Task ( new Action ( () => { dir = player.Play( this.Cube, this.Players ); } ) );
                    turn.Start();
                    turn.Wait( 1000 );
                    player.PlayerInfo.UpdateStatistics( DateTime.Now.Subtract( start ).TotalMilliseconds );                    
                    Point p = ExecuteMovement( dir, player );
                    if ( p != null && DoMovement != null )
                    {
                        this.Cube[ p.X ][ p.Y ][ p.Z ] = player.PlayerCode;
                        DoMovement( this, new PlayerMovementEventArgs( player, p, false ) );
                        player.Head = p;
                        player.Score++;
                        player.IsPlaying = false;
                    }
                    else
                    {
                        player.IsAlive = false;
                        player.IsPlaying = false;
                        player.CurrentProcess.Dispose();
                        if ( !PlayerHasLost( this, new PlayerHasLostEventArgs( player, this.Players, this ) ) )
                        {
                            break;
                        }
                    }
                }
            }

        }

        private Point ExecuteMovement( Direction dir, Player player )
        {
            Point p = player.Head;
            switch ( dir )
            {
                case Direction.Up:
                    if ( p.Z + 1 < this.Size && Cube[ p.X ][ p.Y ][ p.Z + 1 ] == 0 )
                        return new Point( p.X, p.Y, p.Z + 1 );
                    break;
                case Direction.Down:
                    if ( p.Z - 1 >= 0 && Cube[ p.X ][ p.Y ][ p.Z - 1 ] == 0 )
                        return new Point( p.X, p.Y, p.Z - 1 );
                    break;
                case Direction.Front:
                    if ( p.X + 1 < this.Size && Cube[ p.X + 1 ][ p.Y ][ p.Z ] == 0 )
                        return new Point( p.X + 1, p.Y, p.Z );
                    break;
                case Direction.Bottom:
                    if ( p.X - 1 >= 0 && Cube[ p.X - 1 ][ p.Y ][ p.Z ] == 0 )
                        return new Point( p.X - 1, p.Y, p.Z );
                    break;
                case Direction.Left:
                    if ( p.Y - 1 >= 0 && Cube[ p.X ][ p.Y - 1 ][ p.Z ] == 0 )
                        return new Point( p.X, p.Y - 1, p.Z );
                    break;
                case Direction.Right:
                    if ( p.Y + 1 < this.Size && Cube[ p.X ][ p.Y + 1 ][ p.Z ] == 0 )
                        return new Point( p.X, p.Y + 1, p.Z );
                    break;
                default:
                    return null;
            }
            return null;
        }


        private Point GenerateStartingPosition()
        {
            Point point = new Point( Matchmaking.Random.Next() % Size, Matchmaking.Random.Next() % Size, Matchmaking.Random.Next() % Size );
            while ( Players.Select( x => x.Head ).Contains( point ) )
            {
                point = new Point( Matchmaking.Random.Next() % Size, Matchmaking.Random.Next() % Size, Matchmaking.Random.Next() % Size );
            }
            return point;
        }

        internal static string GetCubeString( int[][][] cube, List<Player> players )
        {
            string[] layers = new string[ cube.GetLength( 0 ) ];
            for ( int i = 0; i < cube.GetLength( 0 ); i++ )
            {
                string s = string.Empty;
                for ( int j = 0; j < cube[i].GetLength( 0 ); j++ )
                {
                    for ( int k = 0; k < cube[ i ][ j ].GetLength( 0 ); k++ )
                    {
                        int val = cube[ i ][ j ][ k ];
                        if ( val == 0 )
                        {
                            s += ".";
                        }
                        else
                        {
                            var player = players.First( x => x.PlayerCode == val );
                            s += ( player.Head == new Point( i, j, k ) )
                                ? player.PlayerChar
                                : char.ToLower( player.PlayerChar );
                        }
                    }
                }
                layers[ i ] = s;
            }
            return string.Join( Environment.NewLine, layers );
        }

        internal void EndMatch( bool valid )
        {
            if ( MatchEnded != null )
            {
                MatchEnded( this, new MatchEndedEventArgs( this, valid ) );
            }
        }

    }
}
