   M S B A M L   `   `   `  ����B  >3dsnake, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null#clr-namespace:_3dsnake_3dsnake  5   	   O Kmscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e0891&clr-namespace:System;assembly=mscorlibSystem 5   	       _3dsnake.MainWindow   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   (localclr-namespace:_3dsnake     5   	   6sys&clr-namespace:System;assembly=mscorlib     5   	   -   5          � Title$  Snake 3D��5   	   $	��750��5	   	   $
��1280��6    � DataContext 5   	   ��  �� RelativeSource �� *Self+ � 	Resources 5
       S� 
MethodName S� 
ObjectType  System.Enum   GameModeValues S� MethodParameters M� TypeName��%y   &	        S� 5   
   $ 	GetValues��6   "  5   	    5      M� 5      $ local:GameMode��6   5      5      .��5      � -   5        $
��grid��5       � ColumnDefinitions 5   
   �� 5      $
��419* �6   �� 5      $
��213* �6   .+�5   
   �� $��1��6   .+�5      �� $��Players��6   .��5      �� $��Stretch=�6>   ��6   �� *
PlayerList+��5      �� 5      .R�5      � $��Stretch=�6#    5   "   �� 5   &   $	��150 �67   �� 5    &   $��30 �67   �� 5!   &   $	��150 �67   .+�5#   "   �� $��Stretch=�6H   ��6,   �� *
PlayerName+6H   �� 5$   "   -   5        $��X��5$   )   $��1��6c   
	 � Tag	 6S   �� 6c   �� 5%   "   $��2��6d   $��2q�6t   ��6,   �� *MeanPlayTime+
 �� StringFormat$
 	{0:F2} ms��6t   5&      5'      5)      5*      �� 5+      $!��Player info (Name,Exe,Args)��6   �� 5,      ��6   �� *NewPlayerInfo+ �� Mode$ OneWayToSource=�6L   �� 5-      -   5        $��btnAddPlayer��5-   *   $��
Add player��6   6[   �� 5.      -   5        $��Default Camera Position��5.      67   �� 5/      $��	Game mode��6   �� 50       �� SelectedIndex$ 0��6V   ��6   ��  �� Source# [   �� SelectedItem 6h   �� *SelectedMode+$ OneWayToSource=��� 51      $
��Size��6   �� 52      ��6   �� *SelectedSize+$ OneWayToSource=��� 53      $��Number of Matches��6   �� 54      ��6   �� *NumberOfMatches+$ OneWayToSource=��� 55      -   5        $��cbxUpdateUI��55       d� 	IsChecked$
 TrueY�6*   $��Update Interface��6;   $��3q�6V   �� 56      -   5        $��
Start Game��56      6*   �� 57      -   5        $��	Stop Game��57      6)   �� 58      $��Moves��6   �� 59      -	   5        $��lblMoves��59      $��36(�6$   �� 5;      $��
ScoreBoard��6   &   System.Windows.Controls.DataGrid  5<      -
   5        $��score��5<      5>      �� 5?      -   5        $��
txtMatches��5?      $��14(�6&   �� 5@      -   5        $��	txtStatus��5@      $��14(�6)   $
��Wrap=�67   $��5q�6K   5A      5B      5C      