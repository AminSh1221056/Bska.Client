   M S B A M L   `   `   `  ����  Bska.Client.UI7    1Bska.Client.UI.UserControlls.ProceedingsUC.SaleUC   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      .��5      �   � ColumnDefinitions  5   
   �� 5	      $��90 �6   �� 5
      $	��200 �6   �� 5      $	��130 �6   �� 5      $��* �6    � RowDefinitions 5   
   �� 5      $��35 �6   �� 5      $��* �6   .+�5   
   �� $��Right=�6   $��Center=�60   $��5,0,5,0q�6K   $��نام خریدار:��6\   $��0��6o   �� 5   
   $��Right=�6   $��Center=�60   $��5,0,5,0q�6K   $&�� قیمت کل صورت جلسه:��6\   $��0��6v   $��2��6�   �� 5   
   $
��Left=�6   $��Center=�6-   $��1��6H   $��0��6X   $	��180��6e   $��5,0,5,0q�6q   ��6�   �� *Desc1+ �� UpdateSourceTrigger$ PropertyChanged=� �� Mode$ TwoWay=� �� ValidatesOnDataErrors .  �� NotifyOnValidationError . �� 5   
    �� 	MaxLength$ 14��6   $��3��6!   $	��150��6�   $��20��6�   $
��Left=�6�   $��Center=�6  $��5,0,5,0q�6"  ��61   ��  �� RelativeSource ��  �� AncestorType" �	 �� AncestorLevel$	 3��
 �� Path$
 DataContext.GlobalPrice� �� StringFormat$ {0:N0}�� �� ConverterCulture$ fa��{�5      �� *IsEditablePrice+5      5      