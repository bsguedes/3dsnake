#!/usr/bin/env python   
# -*- coding: windows-1252 -*-
'''
Created on 06/01/2014

@author: diego.hahn
'''


import sys
import pickle

class Commands():
    @staticmethod
    def Up():         print 'U'
    @staticmethod
    def Down():       print 'D'
    @staticmethod
    def Right():      print 'R'
    @staticmethod
    def Left():       print 'L'
    @staticmethod
    def Bottom():     print 'B'
    @staticmethod
    def Front():      print 'F'
    
class Players(dict):

    def __init__( self ):
        dict.__init__(self)
    
    def SetPlayer( self, number , pos ):
        dict[ number ] = pos
    
    def GetPlayer( self, number )
    
        
    
    


class SnakeMap():
    def __init__( self , whereiam , whoiam, size ):
        self.__map = [[[0 for _ in range(size)] for _ in range(size)] for _ in range(size)]
        self.__whoiam = whoiam
        self.__whereiam = whereiam
        
        self.__players = dict()

        
    def PointValue( self , value , *point ):
        self.__aux_map = list( self.__map )    
        self.__map[point[0]][point[1]][point[2]] = value
        
        
        
    
    def GiveScore2Axis( self ):
        score = []
        score += self.GiveScore2Axe('x')
        score += self.GiveScore2Axe('y')
        score += self.GiveScore2Axe('z')
        return score
        
    def GiveScore2Axe( self, axe ):
        if axe == 'x':
            pass   
        
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
        with open("log.txt", "w") as logout:
         # first, gets head position and general info
            self.__stream = sys.stdin.readline().strip('\r\n')
            self.info = self.Info( *self.__stream.split( ',' ) )
            logout.write( self.__stream )
            logout.write(str(self.info))
        # next, gets filled fields
            self.map = SnakeMap(self.info.Pos3d, self.info.Code, self.info.Size)
            for x in range( self.info.Size ):
                self.__stream = sys.stdin.readline().strip('\r\n')
                for y in range( self.info.Size ):
                    partial = self.__stream[ y*self.info.Size:(y+1)*self.info.Size ]
                    for z in range( self.info.Size ):
                        c = 0 if partial[z] == '.' else (ord(partial[z].upper()) - 0x40)
                        self.map.PointValue( c , *(x,y,z) )
                    
            logout.write(str(self.map))
                
        
        Commands.Down()
    
_p = PySnake()

if __name__ == "__main__":
    try:
        _p.iter()
    except Exception, e:
        print e