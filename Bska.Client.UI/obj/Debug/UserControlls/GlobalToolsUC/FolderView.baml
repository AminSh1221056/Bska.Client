   M S B A M L   `   `   `  ����G  CPixelLab.Wpf, Version=0.0.0.0, Culture=neutral, PublicKeyToken=nullA0clr-namespace:PixelLab.Wpf;assembly=PixelLab.WpfPixelLab.Wpf  5   �    Bska.Client.UI-   'Bska.Client.UI.UserControlls.FolderView   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     6Q   @wpf0clr-namespace:PixelLab.Wpf;assembly=PixelLab.Wpf     6�   -   5          >�Name$  lstFolderView��5      .��5      � .+�5   
   �� -   5         ��Name$ 	m_listbox��5   <    �� SelectedValuePath$ Item1��5   *    �� SelectionMode$ Single=�6D    �� SelectedIndex$ 0��6[   $
��Auto��6m   $��34��5      $��-1,0,0,0q�6O   ��5      �� ��5      �� 5       �� 
TargetType" ��6"   .R�5	      ��    LineScrollViewer#��[  6#   $��Hidden=�6M   $
��Auto=�6t   ��. .��5
      ��  K� IsItemsHost .  �� Orientation$ 
Horizontal=�68   $��0,0,85,0q�6Q   ��.  �� CanHorizontallyScroll . 6�   5      5      ��5      �� 5      .R�5      �� $��Center=�6<   ��6    �� 	 �� Path$	 Item2�6<   5      ��5      �� 5      
 �� 
TargetType"
 ��6   .�5      ��  �� Property �@�OverridesDefaultStyle �� Value . 6>   �� 5       � �� 63   ��  �� ElementName$ lstFolderView��$	 
Foreground��� 5       �@�FocusVisualStyle 69   Z� �� 5       �@J�SnapsToDevicePixels . 6<   �� 5       �@�Cursor$
 Hand��6/   �� 5       � ��$ 64��61   �� 5       � ��$ Light"�63   �� 5       � ��$ 16(�61   �� 5       � �� 63   �� $ lstFolderView��$		 Tag��� 5       � �� 5      �� 5       " ��6.   .R�5!   "   �  �Name$
 Grid��6'   #��z��65   $��1,3,1,0q�6_   .+�5"   &   ��  ��Name$ ShadowBorder��6-   ���#565e63$y�	Collapsed=�6X    J� Effect 5#   *   3  -System.Windows.Media.Effects.DropShadowEffect  5$   .      
BlurRadius$ 15Q�6?      	Direction$	 -90Q�6O   5&   '   �� 5'   &   ���#565e636-   �� 5(   &   #��z��6-   �� 5)   &    ��Name$ ContentHost��68   $��
20,-1,20,1q�6M   $��Center=�6a   �� 5*   &   $ BottomShadowBorder��6-   $��5��6I   $��Bottom=�6T   $	��1,0q�6o   $��2��6|   ��5+   *   �� 5,   .    �� 
StartPoint$	 0,0&�6B    �� EndPoint$	 0,1&�6S   .��5-   2   �  � Color$ #0000��6?    � Offset$ 0Q�6M   � 5.   2   $ #1000��6?   $	 0.5Q�6M   � 5/   2   $ #4000��6?   $ 1Q�6M   50   /   52   '   53   #    �� Triggers 54   "   P� 55   &    P� Property �@��
IsSelected P� Value . . �56   *   ��  �@K�ZIndex$	 100��6I   �� 57   *    �� 
TargetName$
 Grid��61    � ��$	 1,0q�6U   �� 58   *   $ ContentHost��61    � ��$ 
20,-2,20,2q�6\   �� 59   *   $ ShadowBorder��61    � y�$ Visible=�6a   �� 5:   *   $ BottomShadowBorder��61    � y�$ 	Collapsed=�6g   5;   '   5=      5?      5@      5B      5C      5D      