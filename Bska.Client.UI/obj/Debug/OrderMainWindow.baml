   M S B A M L   `   `   `  ����a  ]PresentationFramework.Aero, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35gJclr-namespace:Microsoft.Windows.Themes;assembly=PresentationFramework.AeroMicrosoft.Windows.Themes  5   O    Bska.Client.UI$   Bska.Client.UI.OrderMainWindow   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   oMicrosoft_Windows_ThemesJclr-namespace:Microsoft.Windows.Themes;assembly=PresentationFramework.Aero     5   O   -   5          �Name$  window��5   	    � Title$ درخواست ها��5         PrimaryHueMidBrush#���   6    � WindowStartupLocation$ CenterScreen=�5      $��RightToLeft=�6>     PrimaryFontFamily#��[ 6Z   $	��768��6�   $
��1024��6�    � WindowState$ 	Maximized=�5   	    � 	Resources 5        ExpandingStoryboard g� 	BeginTime �� 
TargetName �� TargetProperty O� KeyTime	 O� Value
 �� 	KeySpline R� KeyTime  System.Windows.Visibility   Visible R� Value   Hidden  ContractingStoryboard �� 
TargetType �� Property   PrimaryHueMidForegroundBrush �� Value��%�  &	       &	 �    'Q� �    �� 5   
   .��5	      U� $ 
00:00:0.01X�6,   $ LeftPane��6C   $ (FrameworkElement.Width)�6d   .\�5
      �� $ 	00:00:0.5��6'   $		 175Q�6;   $
 0.681,0,0,1��6G   5      T� 5      $ 
00:00:0.01X�6,   $ LeftPane��6C   $ (UIElement.Visibility)�6d   ..�5      i� $ 
00:00:0.01��6)   # Z 6>   i� 5      $ 	00:00:0.5��6)   # Z 6=   5      U� 5      $ 
00:00:0.01X�6,   $ LeftPane��6C   $ (UIElement.Opacity)�6d   .\�5      �� $ 
00:00:0.01��6'   $	 0Q�6<   �� 5      $ 	00:00:0.3��6'   $		 0.1Q�6;   �� 5      $ 	00:00:0.4��6'   $		 0.5Q�6;   �� 5      $ 	00:00:0.5��6'   $	 1Q�6;   $
 0.705,0,0,1��6E   5      T� 5      $ 
00:00:0.01X�6,   $ 	btnExpand��6C   $ (UIElement.Visibility)�6e   ..�5      i� $ 	00:00:0.5��6)   # Z 6=   5      T� 5      $ 
00:00:0.01X�6,   $ btnContract��6C   $ (UIElement.Visibility)�6g   ..�5      i� $ 	00:00:0.5��6)   # Z 6=   5      5      �� 5   
   .��5      U� $ 00:00:00X�6,   $ LeftPane��6A   $ (FrameworkElement.Width)�6b   .\�5      �� $ 00:00:00��6'   $		 175Q�6:   $
 0,0,1,1��6F   �� 5       $ 00:00:01��6'   $	 0Q�6:   $
 0.602,0,0,1��6D   5!      T� 5#      $ 00:00:00X�6,   $ LeftPane��6A   $ (UIElement.Visibility)�6b   ..�5$      i� $ 00:00:00��6)   # Z 6<   i� 5%      $ 00:00:01��6)   # Z 6<   5&      U� 5'      $ 00:00:00X�6,   $ LeftPane��6A   $ (UIElement.Opacity)�6b   .\�5(      �� $ 00:00:00��6'   $	 1Q�6:   �� 5)      $ 	00:00:0.3��6'   $		 0.5Q�6;   �� 5*      $ 	00:00:0.5��6'   $	 0Q�6;   �� 5+      $ 00:00:01��6'   $	 0Q�6:   $
 0.705,0,0,1��6D   5,      T� 5-      $ 00:00:00X�6,   $ btnContract��6A   $ (UIElement.Visibility)�6e   ..�5.      i� $ 00:00:00��6)   # Z 6<   i� 5/      $ 00:00:01��6)   # Z 6<   50      T� 51      $ 00:00:00X�6,   $ 	btnExpand��6A   $ (UIElement.Visibility)�6c   ..�52      i� $ 00:00:00��6)   # Z 6<   i� 53      $ 00:00:01��6)   # Z 6<   54      55      �� 56   
   ForeGroundTreeItem��" Q�6,   .�57      ��  � ��# �  6+   58      59       � Triggers 5:      :� 5;   
    :� RoutedEvent$ ButtonBase.Click��6    :� 
SourceName$ 	btnExpand��66   .[�5<      �� #��[ 6   5=      :� 5>   
   $ ButtonBase.Click��6   $ btnContract��66   .[�5?      �� #��[ 6   5@      .��5B      ]� .+�5C   
   �� -   5         ��Name$ HeaderBorder��5C   x   $��40��6   $	��Top=�6     BorderBrush#���  62   $��0,0,0,1q�6^    �� Padding$ 	10,0,10,0q�6�   ._�5D      � $��2q�6    � ColumnDefinitions 5E      �� 5F      $	��250��6'   �� 5G      $��50��6'   �� 5H      .+�5J      �� $��0��6    �� Padding$ 0,2,0,0q�6,    J� Opacity$ 1Q�6>   $
��Left=�6J   $��Segoe)�6f   $��22(�6y   $��Normal&�6�   $��Light"�6�   $
��Wrap=�6�   #���  6�   $��Bottom=�6�   $��0,0,0,5q�6  $��29��6+  بسکا-درخواست ها6H  5K      5L      � 5M   
    5N      �� 5O      $
��Auto �6#   �� 5P      $��10 �6#   �� 5Q      $��* �6#   .+�5S      � -   5         �Name$ MainPane��5S      $��2��6&   $��0q�66   .+�5T      �� -   5        $ 
MainBorder��5T      $��2q�6.    �� CornerRadius$ 2��69   #���   6J     BorderStyle#���  6|   ._�5U      � -   5        $ gridMainArea��5U      .+�5V       � -   5          �Name$ frame��5V         � NavigationUIVisibility$ Hidden=�6/   5X      5Y      5Z      5[      � 5\      $��1��6   .+�5]      �� -   5         ��Name$ 	btnExpand��5]   �   $��14(�6   $��0,0,0,0q�6'   $��Center=�68   $��Center=�6S   $��10��6p   $��50��6{   $��LeftToRight=�6�   $��<��6�   $y�Hidden=�6�   �� 5^      -   5        $ btnContract��5^   S   $��14(�6   $��>��6(   $��LeftToRight=�67   $��Center=�6h   $��Center=�6�   $��10��6�   $��50��6�   $y�Hidden=�6�   5_      � 5a      -	   5        $ LeftPane��5a      $��0��6%   $��0��65   $y�	Collapsed=�6@   .+�5b      �� -
   5        $ 
LeftBorder��5b      $��0q�6-   $ 0��68   #���   5c      ���	#FF191818$��0,0,1,0q�6c   ._�5d      � -   5        $ gridSmallArea��5d      .+�5e      �� -   5         ��Name$ LbxMenu��5f   
   $
��Left=�5e   "   $	��Top=�6=     MetroListBoxItemTemplate#���  5g   
     MetroListBoxItemStyle#���  5h   
   ��5f      Z� 5i   
   5j      5k      5l      5m      5n      5o      