using HelixToolkit.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace _3dsnake
{
    public class SnakeCube : ModelVisual3D
    { 
        /// <summary>
        /// This array keeps track of which cubelet is located in each
        /// position (i,j,k) in the cube.
        /// </summary>
        private Model3DGroup[ , , ] cubelets;

        private readonly double _spacing = 0.06;
        private readonly double _size = 5.7;

        public int Size { get; private set; }

        public SnakeCube( int size )
        {
            this.Size = size;
            cubelets = new Model3DGroup[ size, size, size ];
            for ( int i = 0; i < size; i++ )
            {
                for ( int j = 0; j < size; j++ )
                {
                    for ( int k = 0; k < size; k++ )
                    {
                        var cubelet = new Model3DGroup();
                        cubelets[ i, j, k ] = cubelet;
                        Children.Add( new ModelVisual3D { Content = cubelet } );
                    }
                }
            }

            double origin = -( this.Size ) * 0.5 * _size;
            
            Children.Add( CreateFaceGrid( new Point3D( 0, origin, 0 ), new Vector3D( 0, 1, 0 ) ) );
            Children.Add( CreateFaceGrid( new Point3D( origin, 0, 0 ), new Vector3D( 1, 0, 0 ) ) );
            Children.Add( CreateFaceGrid( new Point3D( 0, 0, origin ), new Vector3D( 0, 0, 1 ) ) );            

        }

        private Visual3D CreateFaceGrid( Point3D V, Vector3D V2 )
        {
            GridLinesVisual3D grid = new GridLinesVisual3D();
            grid.Thickness = 0.4;
            grid.Length = this.Size * _size;
            grid.Width = this.Size * _size;
            grid.MinorDistance = _size;
            grid.Center = new Point3D( V.X, V.Y, V.Z);
            grid.Normal = V2;            
            return grid;
        }

        private static GeometryModel3D CreateFace( int face, Point3D center, double width, double length, double height, Brush brush )
        {
            var m = new GeometryModel3D();
            var b = new MeshBuilder( false, true );
            switch ( face )
            {
                case 0:
                    b.AddCubeFace( center, new Vector3D( -1, 0, 0 ), new Vector3D( 0, 0, 1 ), length, width, height );
                    break;
                case 1:
                    b.AddCubeFace( center, new Vector3D( 1, 0, 0 ), new Vector3D( 0, 0, -1 ), length, width, height );
                    break;
                case 2:
                    b.AddCubeFace( center, new Vector3D( 0, -1, 0 ), new Vector3D( 1, 0,0  ), width, length, height );
                    break;
                case 3:
                    b.AddCubeFace( center, new Vector3D( 0, 1, 0 ), new Vector3D( -1, 0, 0 ), width, length, height );
                    break;
                case 4:
                    b.AddCubeFace( center, new Vector3D( 0, 0, -1 ), new Vector3D( 0, 1, 0 ), height, length, width );
                    break;
                case 5:
                    b.AddCubeFace( center, new Vector3D( 0, 0, 1 ), new Vector3D( 0, -1, 0 ), height, length, width );
                    break;
            }

            m.Geometry = b.ToMesh();
            m.Material = MaterialHelper.CreateMaterial( brush );
            return m;
        }



        internal void Move( int x, int y, int z, Color color )
        {
            double origin = -( this.Size - 1 ) * 0.5 * _size;
            var center = new Point3D( origin + x * _size, origin + y * _size, origin +z * _size );            
            for ( int face = 0; face < 6; face++ )
            {
                cubelets[ x, y, z ].Children.Add( CreateFace( face, center, _size * ( 1 - _spacing ), _size * ( 1 - _spacing ), _size * ( 1 - _spacing ), new SolidColorBrush ( color ) ) );
            }            
        }

        internal void ClearCubes()
        {
            for ( int i = 0; i < Size; i++ )
            {
                for ( int j = 0; j < Size; j++ )
                {
                    for ( int k = 0; k < Size; k++ )
                    {
                        cubelets[ i, j, k ].Children.Clear();                        
                    }
                }
            }
        }
    }
}
