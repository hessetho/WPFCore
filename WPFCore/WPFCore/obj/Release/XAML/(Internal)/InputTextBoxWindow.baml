   M S B A M L   `   `   `  ���� B  >WPFCore, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null+clr-namespace:WPFCore.XAMLWPFCore.XAML  %   WPFCore.XAML.InputTextBoxWindow   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     @d2http://schemas.microsoft.com/expression/blend/2008     +xamlclr-namespace:WPFCore.XAML     -     �Name$  WindowInputTextBox�� � SizeToContent$ Height=� � WindowStartupLocation$ CenterScreen=� � WindowStyle$ 
ToolWindow=� � ShowInTaskbar .    WPFCore.XAML.PowerUp   
FocusFirst . $	��300�� � Title$ 
Input text�� � 	Resources  �� 
TargetType	 �� Property
 �� Value �� CornerRadius � ToolTip �� ElementName �� Path��%�  '��       �� " ��.��� 	 � ��$
 Center=��� 	 �@=�ErrorTemplate
 �� .R�]� ��. .+��� 
��� E��$��right=�$��5,0,0,0q�$��20��$��20��$ 5�� �� $ customAdorner��$8 2AdornedElement.(Validation.Errors)[0].ErrorContent�._��� $��!��$��center=�$��center=�$
��Bold"�
��������� $��customAdorner��$��Center=�.v��� 
���  ��$��1q�.��� $��5q� � RowDefinitions �� $
��Auto ��� $
��Auto ��� $
��Auto �.+��� $��0��$
��Wrap=����� $ WindowInputTextBox��$ MessageText� �� FallbackValue$ Please enter a text:���� $��1��$	��0,5q�   SelectTextOnFocus . ���� $ WindowInputTextBox��$ 	InputText� �� UpdateSourceTrigger$ PropertyChanged=� �� ValidatesOnDataErrors .  �� NotifyOnValidationError . ]� $��2����.  $��0,5,0,0q�.+��� -    ��Name$ 
OkayButton��$
��Left=� �� 	IsDefault . $��45��$��Ok���� -   $��Right=� �� IsCancel . $��60��$��Cancel��