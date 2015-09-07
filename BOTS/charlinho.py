#!/usr/bin/env python   
# -*- coding: windows-1252 -*-
'''
Created on 06/01/2014

@author: diego.hahn
'''
import time
import traceback
import sys
import pickle
import math


class Command:        # (  x,  y,  z )
    __directions__ = { 
                        (  0,  0, -1) : 'D', # Down
                        (  0,  0,  1) : 'U', # Up   
                        (  0, -1,  0) : 'L', # Left
                        (  0,  1,  0) : 'R', # Right
                        ( -1,  0,  0) : 'B', # Bottom
                        (  1,  0,  0) : 'F', # Front
                      }    
     
    @staticmethod
    def Direction( value ):
        return Command.__directions__[ value ]
        
    @staticmethod
    def Keys():
        return Command.__directions__.keys()
        
    @staticmethod
    def Keys2():
        return map( lambda x: Coord(*x), Command.__directions__.keys())
        
class Coord:

    def __init__( self, *points ):
        self.Value = points

    def __add__(self, coord):        
        return Coord(*([int(self.Value[w] + coord.Value[w]) for w in range(len(self.Value)) ]))

class SnakeMap():
    # Vértices do cubo
    CreateVertices = lambda a, s : [ ( (s*((i>>0)&1)),(s*((i>>1)&1)),(s*((i>>2)&1)) ) for i in range( 8 ) ] 

    def __init__( self , whereiam , whoiam, size ):
        self.__map = [[[0 for _ in range(size)] for _ in range(size)] for _ in range(size)]
        self.__whoiam = whoiam
        self.__whereiam = whereiam
        self.__size = size
        
        self.__aux_players = dict()
        self.__players = dict()
                        
        self.Vertices = self.CreateVertices( size-1 )
        self.Counter = 0
        self.MaxCounter = 0
        
    def PointValue( self , value , *point ):
        self.__map[point[0]][point[1]][point[2]] = value        
        
    def HeadValue( self, value , *point ):
        self.__map[point[0]][point[1]][point[2]] = value  
        if  value != self.__whoiam :        
            self.__players[ value ] = point      

    def GetPoint( self, *point ):
        if ( point[0] >= 0 and point[0] < self.__size) and ( point[1] >= 0 and point[1] < self.__size) and ( point[2] >= 0 and point[2] < self.__size):    
            return self.__map[point[0]][point[1]][point[2]]
        return -1
        
    def ScorePoint( self, log ):
        self.log = log
        checks = range( 1, 25)[::-1]        
        
        for self.MaxCounter in checks:
        
            _distances = []
            _distancesXYZ = []        
        
            # Calcula a distância do ponto atual até os vértices
            for vertice in self.Vertices:
                x = vertice[0] - self.__whereiam[0]
                y = vertice[1] - self.__whereiam[1]
                z = vertice[2] - self.__whereiam[2]
                
                _distancesXYZ.append( (x,y,z) )
                _distances.append( math.sqrt( x**2 + y**2 + z**2 ) )
                
            distances = []
            distancesXYZ = []       
            # Organiza as maiores distâncias
            for i, _d in enumerate(_distances):        
                for j, d in enumerate(distances):
                    if ( _d > d ):
                        distances.insert(j, _d)
                        distancesXYZ.insert(j, _distancesXYZ[i])
                        break
                else:
                    distances.append( _d )
                    distancesXYZ.append( _distancesXYZ[i] )

            for i, dXYZ in enumerate( distancesXYZ ):            
                ignore = []
                for _ in range( len(dXYZ) ):
                    _dMIN = 2 * self.__size
                    #_dMIN = 0
                    dMIN = 0 
                    axe = None
                    rel_coord = [0,0,0]
                    for j in range( len(dXYZ) ):
                        if ( dXYZ[j] == 0 ):
                            continue
                    
                        if ( abs(dXYZ[j]) < _dMIN and j not in ignore ):
                            _dMIN = abs(dXYZ[j])
                            dMIN = dXYZ[j]
                            axe = j
                    
                    if axe == None:
                        continue

                    rel_coord[axe] = math.copysign( 1, dMIN )
                    rel_coord = tuple(rel_coord)
                                   
                    abs_coord = tuple([int(self.__whereiam[w] + rel_coord[w]) for w in range(len(dXYZ)) ])
                        
                    if self.ScorePosition( Coord( *abs_coord ) , Coord( *self.__whereiam), self.MaxCounter ):                
                        return Command.Direction( rel_coord )

                    ignore.append( axe )
        
        # Se chegar até aqui, é porque caiu a casa :D:D
        return None
        
    def ScorePosition( self, coord, old_coord, counter ):
        self.MaxCounter = counter
        self.Counter = 0    
        #try:
        ret = self.ScoreNextPosition( coord, old_coord )
        if ret : 
            return True

            
        return False
        
    def ScoreNextPosition( self, coord, old_coord ):
        self.Counter += 1
        
        #self.log.write( "%s: %s , %s\n" % (self.Counter, str(coord.Value), str(old_coord.Value) )   )     
        if self.Counter == self.MaxCounter:
            if self.GetPoint( * coord.Value ) == 0:  
                return True
            else:
                self.Counter -= 1
                return False
    
        if self.GetPoint( * coord.Value ) == 0:     
            for cmd in Command.Keys2():                                
                new_coord = cmd + coord
                if isinstance(old_coord, Coord):
                    if new_coord.Value == old_coord.Value:
                        continue
                  
                if self.GetPoint( *(new_coord.Value) ) == 0:
                    #self.PointValue( self.__whoiam , *(new_coord.Value) )                
                    self.PointValue( self.__whoiam , *(coord.Value) )   
                    if self.ScoreNextPosition( new_coord, coord ):
                        self.PointValue( 0 , *(coord.Value) )
                        return True
                        
                    self.PointValue( 0 , *(coord.Value) )                                            

        self.Counter -= 1
        return False
    
        
    def __str__( self ):
        string = ""
        string += "[\n"
        for x in self.__map:
            string += ' [\n'
            for y in x:
                string += '  ' + str( y ) + '\n'
            string += ' ]\n'
        string += "]\n"
        return str( string )
        
class PySnake():
    
    class Info():
        def __init__( self, *args ):
            self.Size = int(args[0])
            self.Code = int(args[1])
            self.Pos3d = map(int, args[2:])
            
        def __str__( self ):
            return '''\
size  : {self.Size}
code  : {self.Code}
pos3d : ( {self.Pos3d[0]} , {self.Pos3d[1]} , {self.Pos3d[2]} )
'''.format( self = self )            
        
    def iter( self ):
        with open("log.txt", "a+") as logout:
         # first, gets head position and general info
            self.__stream = sys.stdin.readline().strip('\r\n')
            self.info = self.Info( *self.__stream.split( ',' ) )
            # logout.write( self.__stream )
            # logout.write(str(self.info))
        # next, gets filled fields
            self.map = SnakeMap(self.info.Pos3d, self.info.Code, self.info.Size)
            for x in range( self.info.Size ):
                self.__stream = sys.stdin.readline().strip('\r\n')
                for y in range( self.info.Size ):
                    partial = self.__stream[ y*self.info.Size:(y+1)*self.info.Size ]
                    for z in range( self.info.Size ):
                        
                        if partial[z] == '.':
                            c = 0
                        elif partial[z].isupper():
                            c = ord(partial[z]) - 0x40
                            self.map.HeadValue( c, *(x,y,z) )
                        else:
                            c = ord(partial[z].upper()) - 0x40
                            self.map.PointValue( c, *(x,y,z) )                                        
                    
            #logout.write(str(self.map))
                
        
            a = self.map.ScorePoint( logout )
            
            #logout.close()
            #time.sleep( 10000.0 )
            print a
    
_p = PySnake()

if __name__ == "__main__":
    #a = open("log.txt", "w")
    #a.close()
    try:
        _p.iter()
    except Exception, e:
        print traceback.format_exc()