   M S B A M L   `   `   `  ���� a  ]wpf тесты для обучения, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null_4clr-namespace:wpf_тесты_для_обучения&wpf_тесты_для_обучения  0    *wpf_тесты_для_обучения.App   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     Flocal4clr-namespace:wpf_тесты_для_обучения     _ [MaterialDesignThemes.Wpf, Version=5.2.1.0, Culture=neutral, PublicKeyToken=df2a72020bd7962aFmaterialDesign1http://materialdesigninxaml.net/winfx/xaml/themes    �� 	Resources  F   @wpf_тесты_для_обучения.NullToVisibilityConverter   NullToVisibilityConverterB   <wpf_тесты_для_обучения.StringToBoolConverter  StringToBoolConverter  WindowDefaultBackground �� Color  DefaultDataGridStyle �� 
TargetType&   System.Windows.Controls.DataGrid �� Property �� Value*  $System.Windows.Controls.DataGridCell �� Triggers P� Property P� Value)  #System.Windows.Controls.DataGridRow=  7System.Windows.Controls.Primitives.DataGridColumnHeader �� 
TargetType	 �� Padding
 �� Orientation ��Name �� Triggers �� 
TargetName  CustomButtonStyle  OrangeButtonStyle �� CornerRadius  AddAnswerButtonStyle  OrangeTabItemStyle  DefaultTextBlockStyle 	 LinkTextStyle 
 AcentTextStyle  CheckBoxStyle  BoldTextBlockStyle��%�  &	        &	      &	 
     &	      &	 �    &	 �    &	 �    &	 �    &	 M
    &		 �
    &	
 n    &	 �    &	 �        �� $ White���� "  .���  � �� �#FFFFFF��  � �� �#E0E0E0��  � ��$ 1q���  �@ RowBackground �#F8F8F8��   �@ AlternatingRowBackground �#EEEEEE��  �@ GridLinesVisibility$	 All=���  �@ 
IsReadOnly . ��  � ��$ 16(���  �@ 	CellStyle �� "  .���  � ��$ 8q���  � ��$ 1q���  � �� �#DDDDDD��  � ��
 ���� ��  � �� �#333333 P�  �@ 
IsSelected . . ���  � �� �#fb9100��  � �� �#FFFFFF��  �@ RowStyle �� "  .���  � ��
 ���� ��  � �� �#DDDDDD��  � ��$ 0,0,0,1q� P�  �@J�IsMouseOver . . ���  � �� �#ffe0b2��  �@ ColumnHeaderStyle �� "  .���  � �� �#F0F0F0��  � �� �#222222��  � ��$
 Bold"���  � ��$ 6q���  � �� �#fb9100��  � ��$ 0,0,0,2q���  � �� �� "  .R�� .+��� #��z��#��z��#��z��#	 z��._��� $
 
Horizontal=�$��Center=�.+��� $��Center=��� $ SortIcon��$	��▼��$y�	Collapsed=�$��12(�$��5,0,0,0q� P�  �@J�IsMouseOver . . ���  � �� �#ffd699P�  �@ SortDirection$ 	AscendingX�. ��� $ SortIcon�� � ��$	 ▲���� $ SortIcon�� � y�$ Visible=�P�  �@ SortDirection$ 
DescendingX�. ��� $ SortIcon�� � ��$	 ▼���� $ SortIcon�� � y�$ Visible=��� " ��.���  � ��$ Arial)���  � ��$ 14(���  � �� �	#FF080808��  � ��
 �������  � ��$ 0,0,0,0q���  � ��$ 3,4,3,4q���  � ��$
 Left=��� " ��.���  � ��$ Arial)���  � �� �#fb9100��  � ��
 �������  � ��$ 0q���  � ��$ 40����  � ��$ Center=���  � ��$ 	0,20,0,10q���  � ��$ 
20,20,20,0q���  �@�Cursor$
 Hand����  � �� �� " ��.R��� #��z��$ 8��#��z��._��� $��Center=�$��Center=��� " ��.���  � ��
 �������  � ��
 �   ���  � ��$ Arial)���  � ��$ 5q� P�  � �� . . ���  � �� �#fb9100��  � ��
 ������� " ��.���  � ��$ Arial)���  � ��$ 20(���  � �� �#fb9100��  � ��
 �������  � ��$ 0q���  � ��$ 40����  � ��$	 200����  � ��$ -5,0,0,0q���  � ��$ 5q���  � �� �� " ��.R��� #��z��#��z��$��0,0,0,1q�$ 	0,10,10,0��._�� .+��� $��Center=�$��Center=�$��Header�� P�  �@��
IsSelected . . ���  � ��
 �������  � ��
 �   ��� " ��.���  � ��$ Arial)���  � ��$ 16(���  � ��
 �   ��� " ��.���  � ��$ Arial)���  � ��$ 14(���  � �� �	#FFA25100��  � ��$ Center=���  �@�Cursor$
 Hand����  � ��$ 	0,10,0,10q���  � ��$ 	Underline{��� " ��.���  � ��$ Arial Black)���  � ��$ 36(���  � ��
 �   ���  � ��$ Center=���  � ��$ 	0,20,0,20q��� " ��.���  � ��$ Arial)���  � ��$ 14(���  � ��$ 0,0,0,0q���  � ��
 �   � P�  �@J�IsMouseOver . . ���  � �� �#fb9100P�  �@d�	IsChecked$
 TrueY�. ���  � �� �#fb9100�� " ��.���  � ��$ 16(���  � ��$
 Bold"���  � ��$
 Wrap=���  � ��$ 0,2,0,10q���  � ��
 �   �