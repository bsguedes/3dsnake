using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Noobot
{
    class Noobot
    {
        static void Main( string[] args )
        {
            using ( Stream str = Console.OpenStandardInput() )
            {
                // first fifteen bytes contain level data: 00,A,00,00,00
                // size, player code, x, y, z (player code is a uppercase char)
                byte[] fl = new byte[ 15 ];
                str.Read( fl, 0, 15 );

                string firstLine = Encoding.Default.GetString( fl );
                string[] ss = firstLine.Split( ',' );

                int size = int.Parse( ss[ 0 ] );
                char code = ss[ 1 ][ 0 ];
                int x = int.Parse( ss[ 2 ] );
                int y = int.Parse( ss[ 3 ] );
                int z = int.Parse( ss[ 4 ] );

                // next we have cube information, one char per cell
                // 0 is a free cell, 1-9 is a cell occupied by player 1-9
                // your cells will contain your player code
                // the cube contains 'size' lines, each line is a cube slice on a constant x value.
                // first line contains cells which x = 0. then we iterate on z and then on y
                // (0,0,0), (0,0,1), ... , (0,0,9), (0,1,0), (0,1,1) ... (0,9,8), (0,9,9) if size is 10 (thus each line contains size^2 chars).
                // next line contains cells where x = 1 and so on.
                fl = new byte[ size * size * size + size * 2 ];
                str.Read( fl, 0, size * size * size + size * 2 );
                string cubeString = Encoding.Default.GetString( fl );
                ss = cubeString.Split( new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries );


                int[ , , ] cube = new int[ size, size, size ];
                for ( int i = 0; i < size; i++ )
                {
                    ss[ i ] = ss[ i ].Trim( '\r', '\n', ' ' );
                    for ( int j = 0; j < size; j++ )
                    {
                        for ( int k = 0; k < size; k++ )
                        {
                            char c = ss[ i ].ElementAt( j * size + k );
                            cube[ i, j, k ] = c == '.' ? 0 : ( int )( char.ToUpper( c ) ) - 0x40;
                        }
                    }
                }

                foreach ( var i in GetRandomSequence(6) )
                {
                    if ( ExecuteMovement( ( Direction )i, new Point( x, y, z ), cube, size ) != null )
                    {
                        switch ( i )
                        {
                            case 0:
                                Console.WriteLine( "U" );
                                return;
                            case 1:
                                Console.WriteLine( "D" );
                                return;
                            case 2:
                                Console.WriteLine( "F" );
                                return;
                            case 3:
                                Console.WriteLine( "B" );
                                return;
                            case 4:
                                Console.WriteLine( "L" );
                                return;
                            case 5:
                                Console.WriteLine( "R" );
                                return;
                        }
                    }
                }
                // dead =/
                Console.WriteLine( "R" );
                return;
            }
        }

        private static IEnumerable<int> GetRandomSequence( int p )
        {
            Random r = new Random();
            bool[] values = new bool[ p ];
            while ( !values.All( x => x ) )
            {
                int v = r.Next( p );
                if ( values[ v ] ) continue;
                values[ v ] = true;
                yield return v;
            }
            yield break;
        }

        public enum Direction
        {
            Up=0,
            Down=1,
            Front=2,
            Bottom=3,
            Left=4,
            Right=5
        }        

        static Point ExecuteMovement( Direction dir, Point head, int[ , , ] cube, int size )
        {
            Point p = head;
            switch ( dir )
            {
                case Direction.Up:
                    if ( p.Z + 1 < size && cube[ p.X, p.Y, p.Z + 1 ] == 0 )
                        return new Point( p.X, p.Y, p.Z + 1 );
                    break;
                case Direction.Down:
                    if ( p.Z - 1 >= 0 && cube[ p.X, p.Y, p.Z - 1 ] == 0 )
                        return new Point( p.X, p.Y, p.Z - 1 );
                    break;
                case Direction.Front:
                    if ( p.X + 1 < size && cube[ p.X + 1, p.Y, p.Z ] == 0 )
                        return new Point( p.X + 1, p.Y, p.Z );
                    break;
                case Direction.Bottom:
                    if ( p.X - 1 >= 0 && cube[ p.X - 1, p.Y, p.Z ] == 0 )
                        return new Point( p.X - 1, p.Y, p.Z );
                    break;
                case Direction.Left:
                    if ( p.Y - 1 >= 0 && cube[ p.X, p.Y - 1, p.Z ] == 0 )
                        return new Point( p.X, p.Y - 1, p.Z );
                    break;
                case Direction.Right:
                    if ( p.Y + 1 < size && cube[ p.X, p.Y + 1, p.Z ] == 0 )
                        return new Point( p.X, p.Y + 1, p.Z );
                    break;
                default:
                    return null;
            }
            return null;
        }
    }

    public class Point
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Z { get; set; }

        public Point( int x, int y, int z )
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }

        public override bool Equals( object obj )
        {
            if ( !( obj is Point ) ) return false;
            Point p = ( obj as Point );
            return ( p.X == this.X && p.Y == this.Y && p.Z == this.Z );
        }

        public override int GetHashCode()
        {
            return string.Format( "{0}|{1}|{2}", X, Y, Z ).GetHashCode();
        }
    }
}
