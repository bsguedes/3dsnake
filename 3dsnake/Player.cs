using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace _3dsnake
{
    public class Player
    {
        private Direction NextDirection;
        public Direction PreviousDirection { get; set; }
        public PlayerInfo PlayerInfo { get; private set; }
        public Color Color { get; private set; }
        public char PlayerChar { get; private set; }
        public int PlayerCode { get; private set; }
        public Process CurrentProcess { get; private set; }
        
        public int Score { get; set; }
        public Point Head { get; set; }
        public bool IsAlive { get; set; }
        public bool IsPlaying { get; set; }

        public Player( PlayerInfo player, Color color, int playerCode, char playerChar  )
        {
            this.PlayerInfo = player;
            this.Color = color;
            this.PlayerCode = playerCode;
            this.PlayerChar = playerChar;
            this.PreviousDirection = Direction.Bottom;
            this.IsAlive = true;
        }

        public Direction Play( int[][][] cube, List<Player> players )
        {            
            ProcessStartInfo startInfo = new ProcessStartInfo( this.PlayerInfo.PlayerCommand, this.PlayerInfo.PlayerArguments );
            startInfo.RedirectStandardInput = true;
            startInfo.UseShellExecute = false;
            startInfo.RedirectStandardOutput = true;
            startInfo.CreateNoWindow = true;
            
            using ( Process process = Process.Start( startInfo ) )
            {
                this.CurrentProcess = process;
                NextDirection = this.PreviousDirection;
                process.OutputDataReceived += process_OutputDataReceived;
                process.StandardInput.WriteLine( string.Format( "{0:D2},{1},{2:D2},{3:D2},{4:D2}{5}{6}", cube.GetLength( 0 ), this.PlayerCode, Head.X, Head.Y, Head.Z, Environment.NewLine, Match.GetCubeString( cube, players ) ) );                
                process.BeginOutputReadLine();

                process.WaitForExit();
                return NextDirection;
            }
        }

        void process_OutputDataReceived( object sender, DataReceivedEventArgs e )
        {
            Process p = sender as Process;
            Console.WriteLine( e.Data );
            char result = e.Data == null ? '\0' : e.Data.FirstOrDefault();
            NextDirection = GetDirection( result );            
        }

        private Direction GetDirection( char result )
        {
            switch ( result )
            {
                case 'U':
                    this.PreviousDirection = Direction.Up;
                    return Direction.Up;
                case 'D':
                    this.PreviousDirection = Direction.Down;
                    return Direction.Down;
                case 'L':
                    this.PreviousDirection = Direction.Left;
                    return Direction.Left;
                case 'R':
                    this.PreviousDirection = Direction.Right;
                    return Direction.Right;
                case 'F':
                    this.PreviousDirection = Direction.Front;
                    return Direction.Front;
                case 'B':
                    this.PreviousDirection = Direction.Bottom;
                    return Direction.Bottom;
                default:
                    return this.PreviousDirection;
            }
        }


    }

    public class Point
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Z { get; set; }

        public Point ( int x, int y, int z )
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
