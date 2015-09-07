using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using HelixToolkit.Wpf;
using System.Windows.Media.Media3D;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.IO;

namespace _3dsnake
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        SnakeCube sc;
        HelixToolkit.Wpf.HelixViewport3D hv;

        public MainWindow()
        {
            LoadPlayersFromSettings();

            InitializeComponent();

            var cwd = System.IO.Path.Combine( Directory.GetParent( Directory.GetCurrentDirectory() ).Parent.Parent.FullName, "BOTS" );
            Directory.SetCurrentDirectory( cwd );

        }

        private void Button_Click( object sender, RoutedEventArgs e )
        {
            SetDefaults( hv );
        }

        private void SetDefaults( HelixViewport3D hv )
        {
            if ( hv != null )
            {
                hv.ShowCoordinateSystem = true;
                hv.ShowFrameRate = true;
                int s = 10 * sc.Size;
                hv.Camera.Position = new Point3D( s, s, s );
                hv.Camera.LookDirection = new Vector3D( -s, -s, -s );
                hv.Camera.UpDirection = new Vector3D( 0.000, 0.000, 1.000 );
                hv.Camera.NearPlaneDistance = 0.001;
            }
        }

        public GameMode SelectedMode { get; set; }
        public string SelectedSize { get; set; }
        public string NumberOfMatches { get; set; }
        public int Moves { get; set; }

        private void btnStartGame_Click( object sender, RoutedEventArgs e )
        {
            Dispatcher.BeginInvoke( new Action( () => 
            {                
                int size, matches;
                if ( !int.TryParse( SelectedSize, out size ) )
                {
                    size = 10;
                }
                if ( !int.TryParse( NumberOfMatches, out matches ) )
                {
                    matches = 100;
                }
                List<Player> players = CreateListOfPlayers();
                Game game = new Game( players, SelectedMode, size, matches );                                
                game.DoMovement += game_DoMovement;
                game.PlayerHasLost += game_PlayerHasLost;

                score.ItemsSource = game.ScoreBoard.Entries;
                ICollectionView cv = CollectionViewSource.GetDefaultView( game.ScoreBoard.Entries );
                cv.SortDescriptions.Add( new SortDescription( "Wins", ListSortDirection.Descending ) );
                foreach ( DataGridColumn c in this.score.Columns )
                {
                    if ( c.SortMemberPath == "Wins" )
                    {
                        c.SortDirection = ListSortDirection.Descending;
                    }
                    else
                    {
                        c.SortDirection = null;
                    }
                }
                if ( cbxUpdateUI.IsChecked ?? false )
                {
                    CreateCube( size );
                }
                else
                {
                    if ( hv != null )
                        grid.Children.Remove( hv );
                }
                game.StartGame();

            } ) );

        }

        bool game_PlayerHasLost( object sender, PlayerHasLostEventArgs e )
        {
            Dispatcher.BeginInvoke( new Action( () =>
            {
                if ( hv != null && sc != null )
                    sc.Move( e.Player.Head.X, e.Player.Head.Y, e.Player.Head.Z, e.Player.Color.ChangeIntensity( 0.667 ) );
                Random r = new Random();
                txtStatus.Text = GetPhrase( e.Player.PlayerInfo.PlayerName );
                
                
            } ) );

            bool matchContinues = e.Players.Where( x => x.IsAlive ).Count() > 1;

            if ( !matchContinues )
            {

                Dispatcher.BeginInvoke( new Action( () =>
                {
                    if ( this.Moves > Math.Pow( e.Match.Size / 2, 2 ) * this.PlayerList.Count )
                    {
                        txtStatus.Text = string.Format( "{0} ganhou o Lápis de Ouro!", e.Match.Players.Single( x => x.IsAlive ).PlayerInfo.PlayerName );
                    }
                    else
                    {
                        txtStatus.Text = string.Format( "NULLIFIED MATCH" );                        
                    }
                } ) );

                e.Match.EndMatch( this.Moves > Math.Pow( e.Match.Size / 2, 2 ) * this.PlayerList.Count );

                Dispatcher.BeginInvoke( new Action( () =>
                {
                    UpdateScore();
                    ClearCube();
                } ) );

            }
            return matchContinues;
        }

        private string GetPhrase( string name )
        {
            string[] phrases = new string[]
            {
                string.Format( "{0} sofreu um doloso (quando há a intenção de matar)", name),
                string.Format( "{0} traiu o movimento punk.", name),                      
                string.Format( "{0} deixou o bolo solar.", name),                        
                string.Format( "{0} entrou no vortex!", name),                        
                string.Format( "{0} se amarra em coisa americana.", name ),                        
                string.Format( "{0} nunca se considerou um ator global.", name),                        
                string.Format( "{0} gosta de biriba.", name),                        
                string.Format( "{0} só quer saber de passarela.", name ),                        
                string.Format( "{0} ganhou bica na costela, voleio na nuca, tapa na cara, telefone e etc.", name ),                        
                string.Format( "{0} foi açoitado pela Dona Máxima.", name),
                string.Format( "{0} foi criar porco.", name),
                string.Format( "{0} sofreu overbook.", name),
                string.Format( "{0} nunca viu italiano comendo amendoim.", name),
                string.Format( "{0} não conhece a piada do não nem eu.", name),
                string.Format( "{0} fugiu de agua-viva no caixote brasil open.", name),
                string.Format( "{0} é criado a leite com pêra e a ovomaltino.", name),
                string.Format( "{0} não pagou conta de lixo conta de luz nem imposto.", name),
            };

            return phrases[ new Random().Next( phrases.Count() ) ];
        }

        private void ClearCube()
        {
            txtMatches.Content = string.Format ( "Match number {0}", Matchmaking.MatchCounter );
            if ( hv != null && sc != null )
                this.sc.ClearCubes();
            lblMoves.Content = null;
            this.Moves = 0;
        }

        private void UpdateScore()
        {            
            this.score.Items.Refresh();

            try
            {
                SortDescription sortDescription = this.score.Items.SortDescriptions[ 0 ];
                this.score.Items.SortDescriptions.Clear();
                this.score.Items.SortDescriptions.Add( sortDescription );
            }
            catch { }                    
        }

        void game_DoMovement( object sender, PlayerMovementEventArgs e )
        {
            Dispatcher.BeginInvoke( new Action( () =>
            {
                if ( hv != null && sc != null )
                    sc.Move( e.Point.X, e.Point.Y, e.Point.Z, e.Player.Color );
                Moves++;
                lblMoves.Content = Moves.ToString();
            } ) );
        }

        private void CreateCube( int size )
        {
            txtMatches.Content = "Match number 1";
            if ( hv != null )
                grid.Children.Remove( hv );
            hv = new HelixViewport3D();
            hv.ShowCoordinateSystem = true;
            hv.Camera = CameraHelper.CreateDefaultCamera();
            hv.Children.Add( new DefaultLights() );
            this.sc = new SnakeCube( size );
            hv.Children.Add( sc );
            grid.Children.Add( hv );
            SetDefaults( hv );
        }

        private Color[] PlayerColors = new Color[] { Colors.Red, Colors.DodgerBlue, Colors.Green, Colors.Yellow, Colors.Purple, Colors.Magenta, Colors.Brown, Colors.Blue };

        private List<Player> CreateListOfPlayers()
        {
            int i = 0;
            List<Player> list = new List<Player>();
            foreach ( var player in this.PlayerList )
            {
                list.Add( new Player( player, PlayerColors[ i ], i + 1, ( char )( 0x40 + i + 1 ) ) );
                i++;
            }
            return list;
        }

        #region Player management

        public string NewPlayerInfo { get; set; }
        public ObservableCollection<PlayerInfo> PlayerList { get; set; }

        private void LoadPlayersFromSettings()
        {
            this.PlayerList = new ObservableCollection<PlayerInfo>();
            string[] list = Properties.Settings.Default.ListPlayers.Split( ';' );
            foreach ( var s in list )
            {
                string[] ss = s.Split( ',' );
                if ( ss.Count() == 3 )
                    this.PlayerList.Add( new PlayerInfo( ss[ 0 ], ss[ 1 ], ss[ 2 ] ) );
            }            
        }

        private void btnAddPlayer_Click( object sender, RoutedEventArgs e )
        {
            string[] ss = NewPlayerInfo.Split( ',' );
            this.PlayerList.Add( new PlayerInfo( ss[ 0 ], ss[ 1 ], ss[ 2 ] ) );
            Properties.Settings.Default.ListPlayers += NewPlayerInfo + ';';
            Properties.Settings.Default.Save();
        }

        private void btnRemovePlayer_Click( object sender, RoutedEventArgs e )
        {
            PlayerInfo pi = ( ( sender as Button ).Tag as PlayerInfo );
            this.PlayerList.Remove( pi );
            Properties.Settings.Default.ListPlayers = string.Join( string.Empty, PlayerList.Select( x => x.ToString() ) );
            Properties.Settings.Default.Save();

        }

        #endregion

        private void btnStopGame_Click( object sender, RoutedEventArgs e )
        {            
            Matchmaking.StopCreatingMatches();
        }

        private void Window_Closed( object sender, EventArgs e )
        {
            Matchmaking.StopCreatingMatches();
        }
    }

    public class PlayerInfo : INotifyPropertyChanged
    {
        public string PlayerName { get; set; }
        public string PlayerCommand { get; set; }
        public string PlayerArguments { get; set; }
        public double MeanPlayTime { get; set; }
        private double Turns;

        public PlayerInfo ( string name, string cmd, string args )
        {
            this.PlayerName = name;
            this.PlayerArguments = args;
            this.PlayerCommand = cmd;
            this.MeanPlayTime = 0;
            this.Turns = 0;
        }

        public override string ToString()
        {
            return string.Format( "{0},{1},{2};", this.PlayerName, this.PlayerCommand, this.PlayerArguments );
        }

        internal void UpdateStatistics( double ms )
        {
            this.Turns++;
            this.MeanPlayTime = ( this.MeanPlayTime * ( this.Turns - 1 ) + ms ) / this.Turns;
            if ( PropertyChanged != null )
                PropertyChanged( this, new PropertyChangedEventArgs( "MeanPlayTime" ) );
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}

