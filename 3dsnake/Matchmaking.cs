using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace _3dsnake
{
    public static class Matchmaking
    {
        public static Random Random = new Random();
        static List<Player> Players;
        static GameMode Mode;
        static List<List<Player>> Matches;
        static int Counter;
        static int MaximumMatches;
        public static int MatchCounter { get { return Counter; } }

        public static void Initialize( List<Player> players, GameMode mode, int maxMatches )
        {
            Matchmaking.Players = players;
            Matchmaking.Mode = mode;
            Matchmaking.Matches = new List<List<Player>>();
            Matchmaking.Counter = 0;
            Matchmaking.MaximumMatches = maxMatches;
            switch ( mode )
            {
                case GameMode.OneVersusOne:
                    for ( int i = 0; i < players.Count; i++ )
                        for ( int j = i + 1; j < players.Count; j++ )
                            Matchmaking.Matches.Add( new List<Player> { Players[ i ], Players[ j ] } );
                    break;
                case GameMode.Arena:
                    Matchmaking.Matches.Add( Players );
                    break;
                default:                    
                    break;
            }
        }

        public static bool DoMoreMatches { get { return Counter < MaximumMatches; } }

        public static List<Player> GetNextMatch()
        {
            return Matches.ElementAt( Counter++ % Matches.Count );           
        }

        public static IEnumerable<T> Shuffle<T>( this IEnumerable<T> source )
        {
            T[] copy = source.ToArray();

            for ( int i = copy.Length - 1; i >= 0; i-- )
            {
                int index = Random.Next( i + 1 );
                yield return copy[ index ];
                copy[ index ] = copy[ i ];
            }
        }


        internal static void StopCreatingMatches()
        {
            Counter = MaximumMatches;
        }

        internal static void InvalidMatch()
        {
            Counter--;
        }
    }
}
