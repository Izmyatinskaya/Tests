   M S B A M L   `   `   `  ����a  ]wpf тесты для обучения, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null_4clr-namespace:wpf_тесты_для_обучения&wpf_тесты_для_обучения  5   	   =    7wpf_тесты_для_обучения.TestViewPassForm   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   Flocal4clr-namespace:wpf_тесты_для_обучения     5   	   -   5        $	��895��5      $	��650��5	   	      WindowDefaultBackground#��[  5
   	     � WindowStartupLocation$  CenterScreen=�5   	   $	��698��6G   $
��Auto=�5   	   $
��Auto=�6;    � Title$ Тест��6k   .��5      � -   5         �Name$ myGrid��5      $��73,35,73,35q�6    � RowDefinitions 5   
   �� 5      $
��Auto �6   �� 5      $��* �6   �� 5      $
��Auto �6    � ColumnDefinitions 5   
   �� 5      $��7* �6   .+�5   
   �� $��0��6   $��0��6"   $��	0,-3,0,10q�62   .+�5      �� -   5        $��titleTextBlock��5      $��Test material��5      $y�Visible=�5         BaseTextBrush#��[ 5!        HeaderTextStyle#��[ 5"      �� 5#      -   5        $��	zoomPanel��5#       �� Orientation$ 
Horizontal=�6*   $��10,0,0,0q�6C   $y�Visible=�6U   $��Right=�6j   .+�5$      �� -   5        $��18(�5$   0   $��	10,1,10,1q�6>   $��0,0,10,0q�6R   ��6d   Z� ��6z   Z� .��5%      �� $��/+zoom90.png��6   $
��Fill=�62   $��30��6A   5&      �� 5(      -   5        $��18(�5(      $��	10,1,10,1q�6'   ��6R   Z� ��6j   Z� .��5)      �� $��/-zoom90.png��6   $
��Fill=�62   $��30��6A   5*      5,      5.      �� 51   
     LightBorder#��[ 6   $��0��66   $��1��6F   $��0,10,0,0q�6S   ._�52      �� $
��Auto=�6   $��Disabled=�6>   $��20q�6g   .��53      �� -   5         ��Name$ questionsStackPanel��53      $
��Auto��6:   $��Stretch=�6G    � LayoutTransform 54      �� 55      -   5        scaleTransform�� �� ScaleX$ 1Q�55   A   	 �� ScaleY$	 1Q�6L   5f      5g      5h      �� 5u   
   -	   5        $��endTestButton��5x      $��3��5u        OrangeButtonStyle#��[ 5v      $!��Закончить тест��5w      $y�Visible=�5y      $��Center=�5z   *   $��	30,1,30,1q�6G   $��	0,10,0,10q�6[   5|      5}      