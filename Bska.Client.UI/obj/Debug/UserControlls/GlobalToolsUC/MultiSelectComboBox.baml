   M S B A M L   `   `   `  ����  Bska.Client.UI6    0Bska.Client.UI.UserControlls.MultiSelectComboBox   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      .��5      �� -   5          ��Name$  MultiSelectCombo��5   	    J� SnapsToDevicePixels .  � OverridesDefaultStyle . $
��Auto=�5   	   $
��Auto=�5	   	   ��. $ �� IsSynchronizedWithCurrentItem$
 TrueY�5   	   ��5   
   �� 5      .R�5      �� -   5           PrimaryHueMidBrush#���   5   5   ��6   �� *Title+ d� 	IsChecked 5      ��  �� Path$ 
IsSelected� �� Mode$ TwoWay=�
 � Tag 5      �� *FindAncestor+ �� AncestorType" ��5      5      ��5   
   �� 5      	 �� 
TargetType"	 ��6   .R�5      � .+�5      d� 
 d�Name$
 ToggleButton��5      $��2��5      |�.   �� 	ClickMode$ Press=�5      $
��Left=�6+    5   (   �� $ IsDropDownOpen�$ TwoWay=� �� RelativeSource �� *TemplatedParent+��5      �� 5      .R�5   "   �  � ColumnDefinitions 5    &   �� 5!   *   $��* �6;   �� 5"   *   $��18 �6;   .+�5$   &   ��  ��Name$ Border��5%      $��2��5&       �� CornerRadius$ 2��5'      
�������
���   �$��1,1,1,1q�5*      �� 5+   &   $ 
BorderComp��5,      $��0��5-      $ 2��5.      $��1q�5/      
�������
���   �$��0,0,0,0q�52      ._�53   *   �� 
������� �� Padding$ 3q�54   C   ��53   4   �� $
 Text� ��  �� Mode$ FindAncestor=�$ UserControlL�54   C   55   '   F� 56   &    F�Name$ Arrow��57      $��1��58      
���   �$��Center=�5:      $��Center=�5;      "����        � 	=  	= � z     	5<      5=   #   5>      5@      � 5A      $��Popup��5B      $��Bottom=�5C       � AllowsTransparency . |�.   �� IsDropDownOpen#��z 5E   ,   $��Slide=�5F      .��5G      � $��DropDown��5H   #    .  � ActualWidth#��z 5J       �� MaxDropDownHeight#��z 5K   #   .+�5L      �� $ DropDownBorder��5M   %   $��1q�5N   $   
�������
���   �5O   %   �� 5P      $��4,6,4,6q�6+    .  � DataContext 6W   �� .��5Q   "   ��  K� IsItemsHost . -  'System.Windows.Input.KeyboardNavigation   DirectionalNavigation$ 	Contained=�6@   5R      5S      5T      5U       �� Triggers 5V      P� 5W       P� Property �@��HasItems P� Value .  . �5X      ��  �� 
TargetName$ DropDownBorder��6!    �� Property � �� �� Value$ 95��6R   5Y      P� 5Z        P� 
SourceName$  Popup��6    �@�AllowsTransparency . . �5[      �� $ DropDownBorder��6!    �@��CornerRadius$ 4��6U   �� 5\      $ DropDownBorder��6!    � ��$ 0,2,0,0q�6O   5]      5_      5a      5b      