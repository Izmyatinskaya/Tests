   M S B A M L   `   `   `  ���� a  ]wpf тесты для обучения, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null_4clr-namespace:wpf_тесты_для_обучения&wpf_тесты_для_обучения  8    2wpf_тесты_для_обучения.TestAddForm   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     Flocal4clr-namespace:wpf_тесты_для_обучения     -     � Title$  TestAddForm��$	��895��$	��650��   WindowDefaultBackground#��[   � WindowStartupLocation$ CenterScreen=�$	��698��$
��Auto=�$
��Auto=�.���  � RowDefinitions �� $
��Auto ��� $
��279* ��� $
��302* ��� $
��Auto � � ColumnDefinitions �� $��7* ��� -    �Name$ columnGrid2��$
��2.5* �.+��� $��0��$��0��$��10,10,10,10q�.+��� -   $��titleTextBox��$��16(�
�������$4��.Введите название теста ...��$��0,0,0,10q� �� SelectionBrush �	#FFFFBE5E�� -   $��titleTextBlock��$��Arial Black)�$��24(�
���   �$y�	Collapsed=�$��0,0,0,10q��� $��0��$��10,53,10,10q�$
��Auto=�$��3��.���� -    ��Name$ questionsStackPanel���� -   $��endTestButton��$��3��$!��Закончить тест��$��20(����#FB9100
�������$
��Bold"�$y�	Collapsed=�$��Center=�$��	30,1,30,1q�$��	0,10,0,10q���Z� �� $��0��$��3��$��1��$��10,10,10,10q�y��� *
ViewMode+.+���   BoldTextBlockStyle#��[ CAНажмите, чтобы создать новый вопрос�� -   $��- Один вариант��  CustomButtonStyle#��[ �� -   $-��'- Несколько вариантов��#��[ �� -	   $��- С картинкой��#��[ �� -
   #��[ .���� $+��%- Картинка + варианты��$
��Wrap=��� #��[ dbВведите процент верных ответов для прохождения теста�� -   $��completedPercentTextBox��$��30�����	#FFABADB3$��1q��� #��[ geВыберите должности, которые смогут проходить этот тест�� -   $��rolesListBox��$	��115������ .R���  �� Orientation$ 
Horizontal=�.+���  d� 	IsChecked �� *
IsSelected+���� *Title+�� -   $��Готово��  OrangeButtonStyle#��[ $��20(�$	��150���� -   	 ��Name$	 ErrorTextBlock��$
��Wrap=�
���  ��$y�	Collapsed=�$��0,0,0,5q�