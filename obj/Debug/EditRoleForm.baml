   M S B A M L   `   `   `  ����a  ]wpf тесты для обучения, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null_4clr-namespace:wpf_тесты_для_обучения&wpf_тесты_для_обучения  5   	   9    3wpf_тесты_для_обучения.EditRoleForm   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   Flocal4clr-namespace:wpf_тесты_для_обучения     5   	      WindowDefaultBackground#��[  5   	     � Title$  Изменить роль��5	   	   $	��500��6    � 
ResizeMode$ NoResize=�6,   $	��600��6C    � WindowStartupLocation$ CenterScreen=�6O   .��5
      � $��73,35,73,35q�6    � RowDefinitions 5   
   �� 5      $
��Auto �6   �� 5      $��* �6   .+�5   
   �� $��Изменить роль��6    �� TextAlignment$ Center=�6)     HeaderTextStyle#��[ 5      �� 5   
   $��1��6     LightBorder#��[ 6   $��Center=�5      $	��Top=�5      $	��400��5      ._�5      �� .+�5      ��  �� Orientation$ Vertical=�6   $��0,15,0,0q�64   .+�5      �� $C��=Введите название роли (должности)��6!     BaseTextStyle#��[ 5      �� 5      -   5        $��titleRoleTextBox��5        TextBoxStyle#��[ 5      5       �� 5"      $ Vertical=�6   $��0,10,0,0q�64   .+�5#      �� $Q��KВыберите доступные тесты (необязательно)��6    #��[ 5$      �� 5%      -   5        $��testsListBox��5%        ListBoxStyle1#��[ 62   $	��100��6Y   ��5&      �� 5'      .R�5(   "   �� $ 
Horizontal=�6-   .+�5)   &   ��   OrangeCheckBoxStyle#��[ 6/    d� 	IsChecked 6\   �� *
IsSelected+ �� Mode$ TwoWay=���6�   �� *Title+5*   #   5+      5-      �� 5/      -   5        $��errorTextBlock��5/   L     ErrorBaseTextStyle#��[ 6    $��0,0,5,0q�50   
   $��Center=�51   
   52      53      �� 56      $��2��6   $ 
Horizontal=�57   
   $��Center=�58   
   $��0,20,0,0q�59   
   .+�5;      �� -   5        $��
Назад��5;         LinkTextStyle#��[ 5<      $��
0,10,12,10q�5=   *   �� 5?      -   5        $��Сохранить��5?       	 OrangeButtonStyle#��[	 5@   
   5A   
   5C      5D      5E      5F      5G      