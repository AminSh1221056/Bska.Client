   M S B A M L   `   `   `  ����  Bska.Client.UID    >Bska.Client.UI.UserControlls.MAssetDetailsUC.CameraAccessories   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      .��5      �   � RowDefinitions  5   
   �� 5	      $��30 �6   �� 5
      $��30 �6   �� 5      $��30 �6   �� 5      $��* �6    � ColumnDefinitions 5   
   �� 5      $	��100 �6   �� 5      $	��180 �6   �� 5      $	��100 �6   �� 5      $��* �6   .+�5   
   �� $��0��6   $��0��6$   $��شماره عدسی:��61   $��Right=�6D   $��Center=�6`   $��5,0,5,0q�6{   �� 5   
   $��2��6   $��0��6$   $��حجم حافظه:��61   $��Right=�6C   $��Center=�6_   $��5,0,5,0q�6z   �� 5   
   $��0��6   $��1��6$   $��رنگ / مدل:��61   $��Right=�6C   $��Center=�6_   $��5,0,5,0q�6z   �� 5   
   $��2��6   $��1��6$   $��سریال:��61   $��Right=�6?   $��Center=�6[   $��5,0,5,0q�6v   �� 5   
   $��2��6   $��2��6$   $"��کارخانه سازنده:��61   $��Right=�6H   $��Center=�6d   $��5,0,5,0q�6   �� 5   
   $��0��6   $��2��6$   $��کشور سازنده:��61   $��Right=�6E   $��Center=�6a   $��5,0,5,0q�6|   �� 5   
   $��0��6   $��1��6   $
��Left=�6/   $��Center=�6J   $��5,0,5,0q�6e   $	��120��6v   ��6�   �� *
LensesNo+�� 5   
   $��0��6   $��3��6   $
��Left=�6/   $��Center=�6J   $��5,0,5,0q�6e   $	��180��6v   ��6�   �� *	RamVolume+�� 5   
   $��1��6   $��1��6   $
��Left=�6/   $��Center=�6J   $��5,0,5,0q�6e   $	��120��6v   ��6�   �� *Model+�� 5   
   $��1��6   $��3��6   $
��Left=�6/   $��Center=�6J   $��5,0,5,0q�6e   $	��120��6v   ��6�   �� *
SerialNo+�� 5   
      BeskaComboBoxStyle#��[  6   $
��Left=�6?   $��Center=�6Z   $��5,0,5,0q�6u   $��1��6�   $��2��5       $	��160��6(   $��20��64    �� DisplayMemberPath$ CountryName��6�    �� SelectedValuePath$ CountryName��6   �� SelectedValue 6@   �� *	Country+��6b   ��  �� RelativeSource ��  �� AncestorType" � �� AncestorLevel$ 2�� �� Path$" DataContext.CountryMakerList�6  �� 5!   
   #��[  6   $
��Left=�6?   $��Center=�6Z   $��5,0,5,0q�6u   $��3��6�   $��2��5"      $	��160��6(   $��20��64   $
 Name��6�   $
 Name��6�    6@   �� *	Company+��6b   ��  �� " �$ 2��$ DataContext.Company�6�   5#      5$      