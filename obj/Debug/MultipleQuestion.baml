   M S B A M L   `   `   `  ����a  ]wpf тесты для обучения, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null_4clr-namespace:wpf_тесты_для_обучения&wpf_тесты_для_обучения  5      =    7wpf_тесты_для_обучения.MultipleQuestion   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      Flocal4clr-namespace:wpf_тесты_для_обучения     5      
���������5      Z�   � 	Resources  5	      I   Cwpf_тесты_для_обучения.BooleanToVisibilityConverter    BooleanToVisibilityConverter��%   &	          5
   
   6-   5      .��5      � .+�5   
   �� .+�5      ]� $��5q�6   $��Center=�6#   .+�5      ��   BaseTextStyle24#��[ 6   $	��Top=�6F   ��5      �� *Number+ �� StringFormat$ {0}. �� �� RelativeSource ��  �� AncestorType$ UserControlL��� 5      -   5        $��deleteQuestionButton��5      $	��❌��65   
���  ��$��20��6R   $��20��6]   
������ $��0q�5   .   $��Right=�5   )   $��Center=�6@   �� 5      -   5        $��copyQuestionButton��5      $
��📄��63   
�������$��20��6R   $��20��6]   
������ $��0q�5   "   $y�	Collapsed=�66   $��Right=�5      $��Center=�62   �� 5      -   5         ��Name$ questionTextBox��5      $"��Введите вопрос?��5      $��14(�5        �� TextWrapping$
 Wrap=�6    " �� VerticalScrollBarVisibility$
 Auto=�5!      $��5q�5"      $��Center=�6�   �� 5$      -   5         ��Name$ questionTextBlock��5$      #��[ 67   $y�	Collapsed=�6`   $��Center=�6w   $��Lato Medium)�6�   5&      �� 5(      -   5        $��image��5(       � Cursor$
 Hand��5)   ^   $	��500��6�   ��5(   !   �� *ImageSource+y�5)      �� *	ShowImage+	 �� 	Converter#	 [  6�   �� 5,      -   5        
 ��Name$
 AnswersPanel��5,      $��5q�6/   �� 5/      -   5        $��addAnswerButton��5/      $%��➕ Добавить ответ��6-   $��0q�6Z     OrangeLittleButtonStyle#��[ 6e   �� 51      -   5        $ ErrorTextBlock��51        ErrorBaseTextStyle#��[ 60   $y�	Collapsed=�6\   52      53      54      