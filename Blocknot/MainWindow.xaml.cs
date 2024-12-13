using System.Text;
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
using System.Windows;
using Microsoft.Win32;
using Microsoft.VisualBasic;


namespace Blocknot
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string currFile = null;// Текущий путь к открытому файлу
        private bool changed = false;// Флаг изменений в тексте

        public MainWindow()
        {
            InitializeComponent();
            textArea.TextChanged += (s, e) => { changed = true; };

            // Привязки команд для стандартных действий
            CommandBindings.Add(
                new System.Windows.Input.CommandBinding(
                    System.Windows.Input.ApplicationCommands.Cut,
                    (s, e) => 
                    { 
                        try 
                        { 
                            textArea.Cut(); 
                        } 
                        catch 
                        { 
                            MessageBox.Show("Ошибка"); 
                        } 
                    },
                    (s, e) => e.CanExecute = textArea.SelectionLength > 0
                )
            );

            CommandBindings.Add(
                new System.Windows.Input.CommandBinding(
                    System.Windows.Input.ApplicationCommands.Copy,
                    (s, e) => 
                    { 
                        try 
                        { 
                            textArea.Copy(); 
                        } 
                        catch 
                        { 
                            MessageBox.Show("Ошибка"); 
                        } 
                    },
                    (s, e) => e.CanExecute = textArea.SelectionLength > 0
                )
            );

            CommandBindings.Add(
                new System.Windows.Input.CommandBinding(
                    System.Windows.Input.ApplicationCommands.Paste,
                    (s, e) => 
                    { 
                        try 
                        { 
                            textArea.Paste(); 
                        } 
                        catch 
                        {
                            MessageBox.Show("Ошибка"); 
                        } 
                    },
                    (s, e) => e.CanExecute = true
                )
            );

            CommandBindings.Add(
                new System.Windows.Input.CommandBinding(
                    System.Windows.Input.ApplicationCommands.Undo,
                    (s, e) => 
                    { 
                        try 
                        { 
                            if (textArea.CanUndo) 
                                textArea.Undo(); 
                        } 
                        catch 
                        { 
                            MessageBox.Show("Ошибка"); 
                        } 
                    },
                    (s, e) => e.CanExecute = textArea.CanUndo
                )
            );
        }

        // Проверка на необходимость сохранения файла
        private bool CheckNeedSave()
        {
            if (!changed) 
                return true;

            var res = MessageBox.Show("Сохранить изменения?", "Вопрос", MessageBoxButton.YesNoCancel);
            if (res == MessageBoxResult.Cancel) 
                return false;

            if (res == MessageBoxResult.Yes) 
                return SaveNow();

            return true;
        }

        // Сохранение текущего файла
        private bool SaveNow()
        {
            if (currFile == null)
            {
                var dlg = new SaveFileDialog();
                if (dlg.ShowDialog() == true)
                {
                    currFile = dlg.FileName;
                }
                else
                {
                    return false;
                }
            }
            try
            {
                File.WriteAllText(currFile, textArea.Text);
                changed = false;
                return true;
            }
            catch
            {
                MessageBox.Show("Ошибка");
                return false;
            }
        }

        // Создание нового файла
        private void NewFile_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!CheckNeedSave())
                    return;

                textArea.Clear();
                currFile = null;
                changed = false;
            }
            catch { MessageBox.Show("Ошибка"); }
        }

        // Открытие файла
        private void OpenFile_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!CheckNeedSave()) 
                    return;

                var dlg = new OpenFileDialog();
                if (dlg.ShowDialog() == true)
                {
                    textArea.Text = File.ReadAllText(dlg.FileName);
                    currFile = dlg.FileName;
                    changed = false;
                }
            }
            catch { MessageBox.Show("Ошибка"); }
        }

        // Сохранение файла
        private void SaveFile_Click(object sender, RoutedEventArgs e)
        {
            try 
            { 
                SaveNow(); 
            } 
            catch 
            { 
                MessageBox.Show("Ошибка"); 
            }
        }

        // Параметры страницы 
        private void PageSetup_Click(object sender, RoutedEventArgs e)
        {
            try 
            { 
                MessageBox.Show("Не реализовано"); 
            } 
            catch 
            {
                MessageBox.Show("Ошибка"); 
            }
        }

        // Закрытие приложения
        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (CheckNeedSave()) 
                    Close();
            }
            catch { MessageBox.Show("Ошибка"); }
        }

        // Проверка перед закрытием окна
        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            if (!CheckNeedSave()) 
                e.Cancel = true;

            base.OnClosing(e);
        }

        // Вырезание текста
        private void Cut_Click(object sender, RoutedEventArgs e)
        {
            try 
            { 
                textArea.Cut(); 
            } 
            catch 
            { 
                MessageBox.Show("Ошибка"); 
            }
        }

        // Копирование текста
        private void Copy_Click(object sender, RoutedEventArgs e)
        {
            try 
            { 
                textArea.Copy(); 
            } 
            catch 
            { 
                MessageBox.Show("Ошибка"); 
            }
        }

        // Вставка текста
        private void Paste_Click(object sender, RoutedEventArgs e)
        {
            try 
            { 
                textArea.Paste(); 
            } 
            catch 
            { 
                MessageBox.Show("Ошибка"); 
            }
        }

        // Удаление выделенного текста
        private void DeleteText_Click(object sender, RoutedEventArgs e)
        {
            try 
            { 
                textArea.SelectedText = ""; 
            } 
            catch 
            { 
                MessageBox.Show("Ошибка"); 
            }
        }

        // Поиск текста
        private void Find_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string s = Interaction.InputBox("Текст для поиска:", "Поиск");
                if (!string.IsNullOrEmpty(s))
                {
                    int pos = textArea.Text.IndexOf(s, StringComparison.CurrentCultureIgnoreCase);
                    if (pos >= 0)
                    {
                        textArea.Focus();
                        textArea.SelectionStart = pos;
                        textArea.SelectionLength = s.Length;
                    }
                    else
                    {
                        MessageBox.Show("Нет совпадений");
                    }
                }
            }
            catch { MessageBox.Show("Ошибка"); }
        }

        // Изменение размера шрифта
        private void FontSize_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string inp = Interaction.InputBox("Размер шрифта:", "Шрифт", "12");
                if (double.TryParse(inp, out double val) && val > 0)
                {
                    textArea.FontSize = val;
                }
                else
                {
                    MessageBox.Show("Неверно");
                }
            }
            catch { MessageBox.Show("Ошибка"); }
        }

        // Включение переноса строк
        private void WordWrapOn(object sender, RoutedEventArgs e)
        {
            try 
            { 
                textArea.TextWrapping = TextWrapping.Wrap; 
            } 
            catch 
            { 
                MessageBox.Show("Ошибка"); 
            }
        }

        // Отключение переноса строк
        private void WordWrapOff(object sender, RoutedEventArgs e)
        {
            try 
            { 
                textArea.TextWrapping = TextWrapping.NoWrap; 
            } 
            catch 
            {
                MessageBox.Show("Ошибка"); 
            }
        }

        // Включение отображения строки состояния
        private void StatusOn(object sender, RoutedEventArgs e)
        {
            try 
            { 
                statusBar.Visibility = Visibility.Visible; 
            } 
            catch 
            { 
                MessageBox.Show("Ошибка"); 
            }
        }

        // Отключение отображения строки состояния
        private void StatusOff(object sender, RoutedEventArgs e)
        {
            try 
            { 
                statusBar.Visibility = Visibility.Collapsed; 
            } 
            catch 
            {
                MessageBox.Show("Ошибка"); 
            }
        }

        // О программе
        private void About_Click(object sender, RoutedEventArgs e)
        {
            try 
            { 
                MessageBox.Show("Простой блокнот", "О программе"); 
            } 
            catch 
            { 
                MessageBox.Show("Ошибка"); 
            }
        }

        // Отмена последнего действия
        private void Undo_Click(object sender, RoutedEventArgs e)
        {
            try 
            { 
                if (textArea.CanUndo) 
                     textArea.Undo(); 
            } 
            catch 
            { 
                MessageBox.Show("Ошибка"); 
            }
        }
    }
}