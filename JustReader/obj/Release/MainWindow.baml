   M S B A M L   `   `   `  ���� E  AJustReader, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'clr-namespace:JustReader
JustReader  U QPresentationUI, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35[>clr-namespace:System.Windows.Documents;assembly=PresentationUISystem.Windows.Documents )clr-namespace:ColorPickerColorPicker      JustReader.MainWindow   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     *localclr-namespace:JustReader     T	Documents>clr-namespace:System.Windows.Documents;assembly=PresentationUI     1ColorPickerclr-namespace:ColorPicker     -     �Name$	  wnd�� � Title$ 
JustReader��$	��700��$
��1200��$	��500��$	��500��.��� $��Stretch=�$
��Auto��$	��Top=�$
��Auto��$��0,0,0,0q�.+�.� -    .�Name$	 fld��$	��Top=�$��0,0,0,0q� *  &FlowDocumentPageViewerControlTemplate1#���   $��0,0,0,0q�$��14(� .� MaxZoom$	 300Q� .� MinZoom$ 50Q� � 	Resources  �� 
TargetType �� Property �� Value 
 Header  
Annotation��%'  '��       'J� %     &	 �     &	 �     �� " ��.���  � ��
 �   ��� " J�.���  �@��Margin$ 	0 15 0 15q���  �@��Padding$ 0q���  �@�Cursor$ Arrow���� .���  � ��$ 48(���  � ��$ Center=���  � ��$ 20q��� .���  � ��$ 12(�.��/� -   	 /�Name$
	 fdoc��
 /� PagePadding$
 0,0,0,0q�  /� IsOptimalParagraphEnabled .   /� ColumnWidth$
 1000�� /� ColumnRuleWidth$ 1�� /� IsColumnWidthFlexible .  /� 	PageWidth$
 Auto�� � ContextMenu �� .@�m� -   $��Открыть книгу��m� -    m�Name$ recent_context_item��{�.  $)��#Последние открытые��m� -   $ booktoc_context_item��$��Оглавление��m� -   $��Настройки���� m� -   $��
Выход���� -	    ��Name$ 
settingDlg��$	��440��$	��240��
�������$y�	Collapsed=�.+��� -
    ��Name$ CanvasBorder��$	��440��$	��240��
�������$��1q�._�� $��
10 0 10 10q�$��Center=�$��Center=�$
��Auto��$	��400�� � RowDefinitions �� $��40 ��� $
��Auto ��� $��30 ��� $��30 ��� $��30 ��� $��30 ��� $
��Auto ��� $
��Auto � � ColumnDefinitions �� $	��240 ��� .+��� $��0��$��2��$��Center=�$��21(�$��НАСТРОЙКИ���� $��1��$��2��$	��0.5��
�������$��	0,0.5,0,0q�$��0,0,0,10q��� $��2��$��0��$��25��$��Цвет фона��   ColorPicker.ColorComboBox  -     Name$ colorPickerBack��$��2��$��1��$
��Left=�$��25��$��0,0,0,0q�$	��Top=�$	��150���� $��3��$��0��$��25��$��Цвет текста��  -   $ colorPickerFore��$��3��$��1��$
��Left=�$��25��$��0,0,0,0q�$	��Top=�$	��150���� $��4��$��0��$��2��$��25��$@��:Переход на сл. страницу по клику���� -    ��Name$ 
ChkIsClick��$��4��$��1���� $��5��$��0��$��2��$��25��$D��>Переход на сл. страницу по пробелу���� -   $ 
ChkIsSpace��$��5��$��1���� $��6��$��0��$��2��$	��0.5��
�������$��	0,0.5,0,0q�$��
10,10,0,10q�� $��7��$��0��$��2��$��Right=�$��0,0,0,0q�.+��� -    �� IsCancel . $��0 0 10 0q�$��80��$��Сохранить���� -   $��0 0 10 0q�$��80��$��Применить���� -    . $��60��$��Закрыть���� -   $ 	recentDlg��$	��440��$	��420��
�������$y�	Collapsed=�.+��� $	��440��$	��420��
�������$��1q�._�� $��10q�$��Center=�$��Center=�$
��Auto��$	��400�� �� $��40 ��� $
��Auto ��� $	��300 ��� $
��Auto ��� $
��Auto �.+��� $��0��$��Center=�$��21(�$4��.ПОСЛЕДНИЕ ОТКРЫТЫЕ КНИГИ���� $��1��$	��0.5��
�������$��	0,0.5,0,0q�$��0,0,0,10q��� -    ��Name$ 
recentList��$��2��$��0q��� $��3��
�������$��	0,0.5,0,0q�$��	0,10,0,10q�� $��4��$��Right=�$��0,0,0,0q�.+��� -    . $��60��$��Отмена���� -   $ 
booktocDlg��$	��640��$	��420��
�������$y�	Collapsed=�.+��� $	��640��$	��420��
�������$��1q�._�� $��10q�$��Center=�$��Center=�$	��620��$
��Auto��$	��620�� �� $��40 ��� $
��Auto ��� $	��300 ��� $
��Auto ��� $
��Auto �.+��� $��0��$��Center=�$��21(�$��ОГЛАВЛЕНИЕ���� $��1��$	��0.5��
�������$��	0,0.5,0,0q�$��0,0,0,10q��� -   $ booktocList��$��2��$��0q��� $��3��
�������$��	0,0.5,0,0q�$��	0,10,0,10q�� $��4��$��Right=�$��0,0,0,0q�.+��� -    . $��60��$��Отмена��o� -    o�Name$
 menu��$��28��$	��Top=�$��0,0,0,0q�$y�	Collapsed=�
�������
�������$��0,0,0,1q�.@�m� -   $��Открыть��$��	10,2,10,2q�m� -   $ recent_menu_item��$��История��$��	10,2,10,2q�m� -   $ booktoc_menu_item��$��Оглавление��$��	10,2,10,2q�m� -   $��Настройки��$��	10,2,10,2q�m� -   $��
Выход��$��	10,2,10,2q�]� -    ]�Name$ panel_notes��$��Stretch=�$
��Auto��$��0,0,0,20q�$��Bottom=�$y�	Collapsed=�.+�� .+�,� -    ,�Name$ 	fld_notes�� � Cursor$ Arrow��
�������$��0,1,0,0q�$��Stretch=�$��0,0,0,0q�$��Stretch=�.��/� -    $	 
fdoc_notes�� � Cursor$ Arrow��$
 20q� .  $
 1000��$ 1�� . $
 Auto�� /� 
FontFamily$ Arial)�  /� 
Background  �	#E5F0F0F0! /� FontSize$! 17(��� $	��Top=�$��Right=�$��0, 0, 15, 0q�$
 Hand��
���   �$��17(�$��verdana)�$��20��$��30��$��0,0,0,0q�.��x