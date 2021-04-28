using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;

namespace LiaoTextEditor
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string FileName = "";
        public MainWindow()
        {
            InitializeComponent();
            //Возможные и доступные настройки для форматирования текста
            Font_Family.ItemsSource = Fonts.SystemFontFamilies.OrderBy(f => f.Source);
            Text_Size.ItemsSource = new List<double>() { 8, 9, 10, 11, 12, 14, 16, 18, 20, 22, 24, 26, 28, 36, 48, 72 }; //Доступный размер текста
            Indent_Size.ItemsSource = new List<string>() { "0,5", "0,75", "1", "1,25", "1,5", "1,75", "2", "2,5" };//Доступный размер отступа
            // Текст по умолчанию.
            Font_Family.Text = "Times New Roman";
            Text_Size.Text = "14";
            Indent_Size.Text = "1";
        }
        
        private void Open_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Text(*.txt)|*.txt| Rich Text Format(*.rtf) | *.rtf | All Files|*"; //фильтр для лиалогового окна
            if (dialog.ShowDialog() == true) //Само окно
            {
                FileName = dialog.FileName;
                this.Title = FileName;
                FileStream fileStream = new FileStream(FileName, FileMode.Open);
                TextRange range = new TextRange(Texting.Document.ContentStart, Texting.Document.ContentEnd);
                range.Load(fileStream, DataFormats.Rtf);
                fileStream.Close();
            }
        }
        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (FileName != "")
            {
                this.Title = FileName;
                System.IO.File.WriteAllText(FileName, string.Empty);
                FileStream fileStream = new FileStream(FileName, FileMode.OpenOrCreate);
                TextRange range = new TextRange(Texting.Document.ContentStart, Texting.Document.ContentEnd);
                range.Save(fileStream, DataFormats.Rtf);
                fileStream.Close();
            }
            else SaveAs_Click(sender, e);
        }

        private void SaveAs_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = "Text(*.txt)|*.txt| Rich Text Format(*.rtf) | *.rtf | All Files|*";
            if (dialog.ShowDialog() == true)
            {
                FileName = dialog.FileName;
                Save_Click(sender, e);
            }
        }

        private void Save_Exit_Click(object sender, RoutedEventArgs e)
        {
            Save_Click(sender, e);//призыв метода для обычного сохранения
            Environment.Exit(1);//выход из программы с кодом 1
        }

        private void Exit(object sender, RoutedEventArgs e)
        {
            Environment.Exit(1);//завершение процесса с кодом 1
        }

        private void Print_Click(object sender, RoutedEventArgs e)
        {
            PrintDialog print = new PrintDialog();//диалоговое окно для печати
            if ((print.ShowDialog() == true))
            {
                print.PrintDocument((((IDocumentPaginatorSource)Texting.Document).DocumentPaginator), "printing as paginator");
            }
        }

        private void Font_Family_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Font_Family.SelectedItem != null)
                Texting.Selection.ApplyPropertyValue(Inline.FontFamilyProperty, Font_Family.SelectedItem);
        }

        private void Text_Size_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TextSelection selectedText = Texting.Selection;
            object pr_font = selectedText.GetPropertyValue(Inline.FontSizeProperty);
            if (selectedText.Start.GetOffsetToPosition(selectedText.End) == 0) return;
            if (float.TryParse(Text_Size.SelectedItem.ToString(), out _) &&
                pr_font.ToString() != Text_Size.SelectedItem.ToString())
                selectedText.ApplyPropertyValue(Inline.FontSizeProperty, Text_Size.SelectedItem.ToString());
        }

        private void Indent_Size_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Indent_Size.SelectedItem != null)
                Texting.Document.LineHeight = Convert.ToDouble(Indent_Size.SelectedItem);
        }

        private void ColorPicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            TextSelection text = Texting.Selection;
            Color c = (Color)e.NewValue;
            if (c != null)
                text.ApplyPropertyValue(TextElement.ForegroundProperty, (SolidColorBrush)(new BrushConverter().ConvertFrom(c.ToString())));
        }

        private void ColorPicker_SelectedColorChanged_1(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            TextSelection text = Texting.Selection;
            Color c = (Color)e.NewValue;
            if (c != null)
                text.ApplyPropertyValue(TextElement.BackgroundProperty, (SolidColorBrush)(new BrushConverter().ConvertFrom(c.ToString())));
        }
    }
}
