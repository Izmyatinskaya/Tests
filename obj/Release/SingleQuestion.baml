   M S B A M L   `   `   `  ���� a  ]wpf тесты для обучения, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null_4clr-namespace:wpf_тесты_для_обучения&wpf_тесты_для_обучения  ;    5wpf_тесты_для_обучения.SingleQuestion   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     @d2http://schemas.microsoft.com/expression/blend/2008     Flocal4clr-namespace:wpf_тесты_для_обучения     
���������Z�   � 	Resources  I   Cwpf_тесты_для_обучения.BooleanToVisibilityConverter    BooleanToVisibilityConverter��%   &	          .���� .+�]� $��5q�.+��� $��18(�$
��Bold"�
���   ����� *Number+ �� StringFormat$
 {0}.�� �� RelativeSource ��  �� AncestorType$ UserControlL��� -   $��deleteQuestionButton��$	��❌��
���  ��$��20��$��20��
������ $��0q�$��Right=��� -    ��Name$ questionTextBox��$"��Введите вопрос?��$��14(� �� TextWrapping$
 Wrap=�" �� VerticalScrollBarVisibility$
 Auto=�$��5q��� -    ��Name$ questionTextBlock��$y�	Collapsed=�$��14(�$
��Wrap=�$��5q�
���   ��� -   $��image�� � Cursor$
 Hand��$	��500������ *ImageSource+y��� *	ShowImage+	 �� 	Converter#	 [  �� -   
 ��Name$
 AnswersPanel��$��5q��� -   $��addAnswerButton��$%��➕ Добавить ответ��$��5q�  AddAnswerButtonStyle#��[ �� -   $ ErrorTextBlock��
���  ��$y�	Collapsed=�