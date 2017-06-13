using System;
using System.Linq;
using System.Text;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using System.IO;
using System.Security.Permissions;
using HTMLConverter;
using System.IO.Compression;



namespace JustReader
{

    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
    [System.Runtime.InteropServices.ComVisibleAttribute(true)]

    public partial class MainWindow : Window
    {
        string basePath;      
        string tempPath;
        string currentPath="";
        string currentFile = "";
        string currentName = "";
        string currentPage = "1";

        INIManager managerini;
        DispatcherTimer timer = null;
        int timerCount = 0;
        string foreGround = "#FF333333";
        string backGround = "#FFEEEEEE";
        string isClickNextPage = "1";
        string isSpaceNextPage = "1";
        public MainWindow()
        {
            InitializeComponent();
            sidSetStartSetting();
        }

        //---------------------------------------------------------------------------------------
        //
        // ОТКРЫТИЕ КНИГИ
        //
        //---------------------------------------------------------------------------------------

        private void sidOpenFileDialog() // Выбор файла
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            // Set filter for file extension and default file extension
            dlg.DefaultExt = ".fb2";
            dlg.InitialDirectory = currentPath;
            dlg.Filter = "book files (*.fb2,*.zip,*.html)|*.fb2;*zip;*.html";

            // Display OpenFileDialog by calling ShowDialog method
            Nullable<bool> result = dlg.ShowDialog();

            // Get the selected file name and display in a TextBox
            if (result == true)
            {
                // Open document
                string filename = dlg.FileName;
                sidReadFile(filename);
            }
        }
        //---------------------------------------------------------------------------------------
        private void sidReadFile(string fn) // Чтение файла
        {
            if (!File.Exists(@fn))
            {
                MessageBox.Show("Файл " + fn + " не найден");
                return;
            };
            currentPath = System.IO.Path.GetDirectoryName(@fn);
            currentFile = fn;
            String sext = "";
            FileInfo fi = new FileInfo(@fn);
            sext = fi.Extension;
            switch (sext)
            {
                case ".zip":
                    readZipFile(@fn);
                    break;
                case ".fb2":
                    sidLoadXmlFromFb2(@fn,0, "");
                    break;
                case ".html":
                    sidLoadHTML(@fn);
                    break;
            }
        }
        //---------------------------------------------------------------------------------------
        private void readZipFile(String filePath) // Разархивирование файла
        {
            String fileContents = "";
            try
            {
                if (System.IO.File.Exists(filePath))
                {

                    ZipArchive apcZipFile = System.IO.Compression.ZipFile.Open(filePath, System.IO.Compression.ZipArchiveMode.Read);
                    foreach (System.IO.Compression.ZipArchiveEntry entry in apcZipFile.Entries)
                    {
                        if (entry.Name.ToUpper().EndsWith(".FB2"))
                        {
                            System.IO.Compression.ZipArchiveEntry zipEntry = apcZipFile.GetEntry(entry.Name);
                            using (System.IO.StreamReader sr = new System.IO.StreamReader(zipEntry.Open()))
                            {
                                //read the contents into a string
                                fileContents = sr.ReadToEnd();
                            }
                        }
                    }
                    sidLoadXmlFromFb2(filePath, 1, fileContents);
                }
            }
            catch (Exception)
            {
                MessageBox.Show("0");
                throw;
            }
        }
        //---------------------------------------------------------------------------------------
        private void sidLoadXmlFromFb2(string fn, int zip, string sxml)
        {
            string s = "";
            StringBuilder sb = new StringBuilder();
            parseFb2 fb = new parseFb2(fn, zip, sxml);
            if (fb.validFile == true)
            {
                sb.Append("<div id='header'>");
                sb.Append(fb.getTitle());
                sb.Append(fb.getSequenceLine());
                sb.Append(fb.getAuthor());
                sb.Append(fb.getCover());
                sb.Append(fb.getAnnotation());
                sb.Append("</div>");
                currentFile = fn;
                currentName = fb.getTitleLine();
                this.Title = currentName;
                sb.Append(fb.getBody());
                sidInitNewDocument(sb.ToString(), 1);
            }
        }
        private void sidLoadHTML(string fn)
        {
            string s = "";
            if (System.IO.File.Exists(fn))
            {
                s = File.ReadAllText(fn, Encoding.GetEncoding(1251));
                s = s.Replace("<BR><DD>", "<DD>");
                if(s!="") sidInitNewDocument(s, 0);
            }

        }

        //----------------------------------------------------------------------------------------------//
        private void sidInitNewDocument(string s, int bimg) // Инициализация новой книги
        { 
            BitmapImage bitmap;
            Image image;
            BlockUIContainer bl = new BlockUIContainer();
            Paragraph p1 = new Paragraph();
            HtmlToXamlConverter.sidRefreshConverter();
            s = HtmlToXamlConverter.ConvertHtmlToXaml(s, true);
            fdoc = null;
            fdoc = XamlReader.Parse(s) as FlowDocument;
            // прорисовка изображений в книге
            if (bimg == 1)
            {
                Paragraph img;
                for (int i = 1; i < 101; i++)
                {
                    img = (Paragraph)fdoc.FindName("img" + i.ToString());
                    if (img != null)
                    {
                        try
                        {
                            s = ((Run)img.Inlines.FirstInline).Text;
                            img.Inlines.Clear();
                            bitmap = new BitmapImage();
                            bitmap.BeginInit();
                            bitmap.StreamSource = new MemoryStream(Convert.FromBase64String(s));
                            bitmap.EndInit();
                            image = new Image();
                            image.Source = bitmap;
                            image.Stretch = Stretch.None;
                            img.Inlines.Add(image);

                        }
                        catch (Exception ex)
                        {
                            break;
                        }
                    }
                    else break;
                }
            }
                // --
            fld.Document = fdoc;
            fdoc.IsOptimalParagraphEnabled = true;
            fdoc.ColumnWidth = 1000;
            fdoc.IsColumnWidthFlexible = true;
            fdoc.FontFamily = new FontFamily("Arial");
            fdoc.FontSize = 24.0;
            fdoc.Background = new SolidColorBrush(colorPickerBack.SelectedColor);
            sidSaveLastFileIni(currentFile, currentName);

            // Переход на текущую страницу после ее обработки ридером
            s = managerini.GetPrivateString("main", "pageIni");
                if (s != "")
                {
                    string[] apage = s.Split('|');
                    currentPage = apage[0];
                    if (currentPage != "1")
                    {
                        try
                        {
                            fdoc.Foreground = Brushes.LightGray;
                            timerStart();
                        }
                        catch (Exception ex)
                        {
                            fdoc.Foreground = new SolidColorBrush(colorPickerFore.SelectedColor);
                        }
                    }
                    else fdoc.Foreground = new SolidColorBrush(colorPickerFore.SelectedColor);
                }
                else fdoc.Foreground = new SolidColorBrush(colorPickerFore.SelectedColor);

        }

        //---------------------------------------------------------------------------------------
        //
        // Переход на текущую страницу после ее обработки ридером
        //
        //---------------------------------------------------------------------------------------
        private void timerStart()
        {
            timer = new DispatcherTimer();
            timer.Tick += new EventHandler(timerTick);
            timer.Interval = new TimeSpan(0, 0, 0, 0, 500);
            timer.Start();
        }

        private void timerTick(object sender, EventArgs e)
        {
            Int32 n = Int32.Parse(currentPage);
            if (fld.CanGoToPage(n))
            {
                timer.Stop();
                try
                {
                    fdoc.Foreground = new SolidColorBrush(colorPickerFore.SelectedColor);
                    fld.GoToPage(n);
                }
                catch (Exception ex)
                {
                    fdoc.Foreground = Brushes.Black;
                }
            }
            timerCount++;
            if (timerCount > 10)
            {
                timer.Stop();
                fdoc.Foreground = new SolidColorBrush(colorPickerFore.SelectedColor);
            }
        }

        //---------------------------------------------------------------------------------------
        //
        // СОБЫТИЯ
        //
        //---------------------------------------------------------------------------------------
        //
        // Window
        //
        //---------------------------------------------------------------------------------------
        private void Window_PreviewKeyDown(object sender, KeyEventArgs e) // Обработка клавиатуры
        {
            string code = e.Key.ToString();
            switch (code)
            {
                // "Down":"Next":"Up":"PageUp":"Home":"End":
                case "OemMinus":
                case "Subtract":
                    // уменьшение масштаба
                    fld.Zoom = fld.Zoom - 20;
                    break;
                case "OemPlus":
                case "Add":
                    // увеличение масштаба
                    fld.Zoom = fld.Zoom + 20;
                    break;
                case "Space":
                    // переход на следующую страницу
                    if (isSpaceNextPage == "1") fld.NextPage();
                    break;
                case "Escape":
                    // выход из программы
                    Application.Current.Shutdown();
                    break;
            }
            fld.Focus();
        }
        //---------------------------------------------------------------------------------------
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) // сохранить текущие свойства ридера и страницу в книге в INI при выходе из программы
        {
            sidSaveLastIni("sizeIni", wnd.Width.ToString() + "x" + wnd.Height.ToString());
            sidSaveLastIni("pageIni", fld.MasterPageNumber.ToString());
            sidSaveLastIni("windowStateIni", this.WindowState.ToString());
            sidSaveLastIni("zoomIni", fld.Zoom.ToString());
        }

        //---------------------------------------------------------------------------------------
        //
        // FlowDocumentReader
        //
        //---------------------------------------------------------------------------------------

        private void fld_MouseDown(object sender, MouseButtonEventArgs e) // переход на следующую страницу по клику
        {
            if (e.LeftButton.ToString() == "Pressed" && isClickNextPage == "1") fld.NextPage();
        }
        //---------------------------------------------------------------------------------------
        private void fld_MouseMove(object sender, MouseEventArgs e) // скрыть/показать верхнее меню
        {
            if (e.GetPosition(fld).Y < 40)
            {
                if (menu.Visibility == Visibility.Collapsed) menu.Visibility = Visibility.Visible;
            }
            else
            {
                if (menu.Visibility == Visibility.Visible) menu.Visibility = Visibility.Collapsed;
            }
        }

        //---------------------------------------------------------------------------------------
        //
        // Меню / контекстное меню
        //
        //---------------------------------------------------------------------------------------

        private void file_open_Click(object sender, RoutedEventArgs e) // открыть FileDialog
        {
            sidOpenFileDialog();
        }     
        //---------------------------------------------------------------------------------------
        private void file_recent_Click(object sender, RoutedEventArgs e) // открыть окно с историей
        {
            ListBoxItem l;
            string[] aname = managerini.GetPrivateString("main", "fileNameIni").Split('|');
            recentList.Items.Clear();
            for (int i=1; i<aname.Length; i++)
            {
                l = new ListBoxItem();
                l.Content = aname[i];
                l.Name = "recent_item_"+i.ToString();
                recentList.Items.Add(l);
            }            
            recentDlg.Visibility=Visibility.Visible;
        }
        //---------------------------------------------------------------------------------------
        public void setting_Click(object sender, RoutedEventArgs e) // открыть окно с историей
        {
            settingDlg.Visibility = Visibility.Visible;
        }        
        //---------------------------------------------------------------------------------------
        private void file_exit_Click(object sender, RoutedEventArgs e) // Выход
        {
            Application.Current.Shutdown();
        }

        //---------------------------------------------------------------------------------------
        //
        // Окно История открытых книг
        //
        //---------------------------------------------------------------------------------------

        public void recentList_SelectionChanged(object sender, SelectionChangedEventArgs e) // выбрать книгу в истории
        {
            string s = "";
            ListBox l = e.Source as ListBox;
            if (l.SelectedIndex > -1)
            {
                string[] apath = managerini.GetPrivateString("main", "filePathIni").Split('|');
                s=apath[l.SelectedIndex+1];
                if (s != "") sidReadFile(s);
                recentDlg.Visibility = Visibility.Collapsed;
            }
        }
        //---------------------------------------------------------------------------------------
        private void recent_Button_Cancel(object sender, RoutedEventArgs e) // закрыть окно истории
        {
            recentDlg.Visibility = Visibility.Collapsed;
        }

        //---------------------------------------------------------------------------------------
        //
        // Окно Настройки
        //
        //---------------------------------------------------------------------------------------

        private void setting_Button_Save(object sender, RoutedEventArgs e) // применить и сохранить
        {
            settingDlg.Visibility = Visibility.Collapsed;
            sidApplySetting();
            managerini.WritePrivateString("main", "foreGroundIni", foreGround);
            managerini.WritePrivateString("main", "backGroundIni", backGround);
            managerini.WritePrivateString("main", "isClickNextPageIni", isClickNextPage);
            managerini.WritePrivateString("main", "isSpaceNextPageIni", isSpaceNextPage);
        }
        //---------------------------------------------------------------------------------------
        private void setting_Button_Apply(object sender, RoutedEventArgs e) // применить настройки
        {
            sidApplySetting();
        }
        //---------------------------------------------------------------------------------------
        private void setting_Button_Cancel(object sender, RoutedEventArgs e) // закрыть окно настройки
        {
            settingDlg.Visibility = Visibility.Collapsed;
        }


        //---------------------------------------------------------------------------------------
        //
        // РАБОТА С INI
        //
        //---------------------------------------------------------------------------------------

        private string sidGetLastIni(string name) // получение последнего в истории парамметра
        {
            string s = "";
            string[] ar = managerini.GetPrivateString("main", name).Split('|');
            if (ar.Length > 0) s = ar[0];
            return s;
        }
        //---------------------------------------------------------------------------------------
        private void sidSaveLastIni(string name, string val)  // сохранение последнего в истории парамметра
        {
            string s = managerini.GetPrivateString("main", name);
            if (s != "")
            {
                string[] a = s.Split('|');
                a[0] = val;
                managerini.WritePrivateString("main", name, string.Join("|", a));
            }
            else managerini.WritePrivateString("main", name, val);
        }
        //---------------------------------------------------------------------------------------
        private void sidSaveLastFileIni(string path, string name) // сохранение всех парамметров для текущей книги
        {
            string wstate = "Maximized";
            string page = "1";
            string size = "1000x700";
            string zoom = fld.Zoom.ToString();
            string s = managerini.GetPrivateString("main", "filePathIni");
            if (s != "") // если файл INI существует
            {
                string[] apath = s.Split('|');
                string[] aname = managerini.GetPrivateString("main", "fileNameIni").Split('|');
                string[] awindowstate = managerini.GetPrivateString("main", "windowStateIni").Split('|');
                string[] asize = managerini.GetPrivateString("main", "sizeIni").Split('|');
                string[] apage = managerini.GetPrivateString("main", "pageIni").Split('|');
                string[] azoom = managerini.GetPrivateString("main", "zoomIni").Split('|');
                int n = apath.Length;
                if (awindowstate.Length != n || asize.Length != n || apage.Length != n || aname.Length != n) // если файл INI не корректный
                {
                    managerini.WritePrivateString("main", "filePathIni", path);
                    managerini.WritePrivateString("main", "fileNameIni", name);
                    managerini.WritePrivateString("main", "windowStateIni", wstate);
                    managerini.WritePrivateString("main", "sizeIni", size);
                    managerini.WritePrivateString("main", "pageIni", page);
                    managerini.WritePrivateString("main", "zoomIni", zoom);
                    return;
                }

                if (path != apath[0]) // если книга не последняя или отсутствуетв в истории вставляем ее последней
                {
                    bool b = false;
                    for (int i = 0; i < apath.Length; i++)
                    {
                        if (apath[i] == path) // если книга не последняя в истории - вырезаем из истории и вставляем ее последней
                        {
                            wstate = awindowstate[i];
                            page = apage[i];
                            size = asize[i];
                            zoom = azoom[i];
                            ArrayCut(ref apath, i);
                            ArrayCut(ref aname, i);
                            ArrayCut(ref awindowstate, i);
                            ArrayCut(ref asize, i);
                            ArrayCut(ref apage, i);
                            ArrayCut(ref azoom, i);
                            // запоминаем и устанавливаем настройки книги из истории
                            apath[0] = path;
                            aname[0] = name;
                            awindowstate[0] = wstate;
                            asize[0] = size;
                            azoom[0] = zoom;
                            try
                            {
                                if (fld.Zoom.ToString() != zoom) fld.Zoom = Double.Parse(zoom);
                            }
                            catch (Exception ex)
                            {
                            }

                            apage[0] = page;
                            if (page != "1")
                            {
                                if (wstate == "Maximized")
                                {
                                    this.WindowState = WindowState.Maximized;
                                }
                                else
                                {
                                    try
                                    {
                                        string[] atempsize = size.Split('x');
                                        wnd.Width = Int32.Parse(atempsize[0]);
                                        wnd.Height = Int32.Parse(atempsize[1]);
                                    }
                                    catch (Exception ex)
                                    {
                                        this.WindowState = WindowState.Normal;
                                    }
                                }
                            }
                            b = true;
                            break;
                        }
                    }
                    if (!b) // если книги нет в истории - вставляем ее последней c новыми настройкам
                    {
                        ArrayPop(ref apath, path);
                        ArrayPop(ref aname, name);
                        ArrayPop(ref awindowstate, wstate);
                        ArrayPop(ref apage, page);
                        ArrayPop(ref asize, size);
                        ArrayPop(ref azoom, zoom);
                    }
                };
                managerini.WritePrivateString("main", "filePathIni", string.Join("|", apath));
                managerini.WritePrivateString("main", "fileNameIni", string.Join("|", aname));
                managerini.WritePrivateString("main", "windowStateIni", string.Join("|", awindowstate));
                managerini.WritePrivateString("main", "sizeIni", string.Join("|", asize));
                managerini.WritePrivateString("main", "pageIni", string.Join("|", apage));
                managerini.WritePrivateString("main", "zoomIni", string.Join("|", azoom));
            }
            else // если истории нет
            {
                managerini.WritePrivateString("main", "filePathIni", path);
                managerini.WritePrivateString("main", "fileNameIni", name);
                managerini.WritePrivateString("main", "windowStateIni", wstate);
                managerini.WritePrivateString("main", "sizeIni", size);
                managerini.WritePrivateString("main", "pageIni", page);
                managerini.WritePrivateString("main", "zoomIni", zoom);
            }
        }

        //---------------------------------------------------------------------------------------
        //
        // УСТАНОВКА НАСТРОЕК
        //
        //---------------------------------------------------------------------------------------
        private void sidSetStartSetting() // Установка первичных настроек
        {
            string s = "";
            string[] a;
            basePath = AppDomain.CurrentDomain.BaseDirectory;
            tempPath = basePath + "temp";
            managerini = new INIManager(basePath + "JustReader.ini");
            sidSetWindowSize();  // Установка размеров окна
            sidSetZoom(); // Установка масштаба книги
            sidSetColor(); // Установка цвета текста и фона

            isClickNextPage = managerini.GetPrivateString("main", "isClickNextPageIni");
            isSpaceNextPage = managerini.GetPrivateString("main", "isSpaceNextPageIni");
            if (isClickNextPage == "") isClickNextPage = "1";
            if (isSpaceNextPage == "") isSpaceNextPage = "1";
            ChkIsClick.IsChecked = isClickNextPage == "1" ? true : false;
            ChkIsSpace.IsChecked = isSpaceNextPage == "1" ? true : false;

            sidSetStartBook(); // Определение книги для автозагрузке
        }
        //---------------------------------------------------------------------------------------
        private void sidSetWindowSize() // Установка размеров окна
        {
            string s = sidGetLastIni("windowStateIni");
            string[] a;
            if (s == "Maximized")
            {
                this.WindowState = WindowState.Maximized;
            }
            else
            {
                s = sidGetLastIni("sizeIni");
                if (s != "")
                {
                    try
                    {
                        a = s.Split('|');
                        a = a[0].Split('x');
                        wnd.Width = Int32.Parse(a[0]);
                        wnd.Height = Int32.Parse(a[1]);

                    }
                    catch (Exception ex)
                    {
                        this.WindowState = WindowState.Normal;
                    }
                }
                else this.WindowState = WindowState.Normal;
            }

        }
        //---------------------------------------------------------------------------------------
        private void sidSetZoom() // Установка масштаба книги
        {
            string s = sidGetLastIni("zoomIni");
            try
            {
                if (fld.Zoom.ToString() != s) fld.Zoom = Double.Parse(s);
            }
            catch (Exception ex)
            {
            }
        }
        //---------------------------------------------------------------------------------------
        private void sidSetColor() // Установка цвета текста и фона
        {
            foreGround = managerini.GetPrivateString("main", "foreGroundIni");
            backGround = managerini.GetPrivateString("main", "backGroundIni");
            if (foreGround == "") foreGround = "#FF333333";
            if (backGround == "") backGround = "#FFF5F5F5";
            byte[] ab = StringToByteArray(backGround.Replace("#", ""));
            colorPickerBack.SelectedColor = Color.FromRgb(ab[1], ab[2], ab[3]);
            ab = StringToByteArray(foreGround.Replace("#", ""));
            colorPickerFore.SelectedColor = Color.FromRgb(ab[1], ab[2], ab[3]);
        }
        //---------------------------------------------------------------------------------------
        private void sidSetStartBook() // Определение книги для автозагрузке
        {
            string[] apath = managerini.GetPrivateString("main", "filePathIni").Split('|');
            if (apath.Length < 2)
            {
                recent_menu_item.IsEnabled = false;
                recent_context_item.IsEnabled = false;
            }
            else
            {
                recent_menu_item.IsEnabled = true;
                recent_context_item.IsEnabled = true;
            }
            string s = "";
            string[] args = Environment.GetCommandLineArgs();
            if (args.Length > 1)
            {
                s = args[1];
                if (s.IndexOf("\\") == -1)
                {
                    s = args[0].Replace("JustReader.exe", "") + s;
                }
                sidReadFile(s);
            }
            else
            {
                currentFile = sidGetLastIni("filePathIni");
                if (currentFile != "") sidReadFile(currentFile);
            }
        }
        //---------------------------------------------------------------------------------------
        private void sidApplySetting() // установка настроек из окна настроек
        {
            if(backGround != colorPickerBack.SelectedColor.ToString()) fdoc.Background = new SolidColorBrush(colorPickerBack.SelectedColor);
            if(foreGround != colorPickerFore.SelectedColor.ToString()) fdoc.Foreground = new SolidColorBrush(colorPickerFore.SelectedColor);
            foreGround = colorPickerFore.SelectedColor.ToString();
            backGround = colorPickerBack.SelectedColor.ToString();            
            isClickNextPage = ChkIsClick.IsChecked==true? "1" : "0";
            isSpaceNextPage = ChkIsSpace.IsChecked == true ? "1" : "0";
        }

        //---------------------------------------------------------------------------------------
        //
        // СЛУЖЕБНЫЕ МЕТОДЫ
        //
        //---------------------------------------------------------------------------------------
        public static byte[] StringToByteArray(string hex) // конверт шестнадцатиричный цвет в rgb
        {
            return Enumerable.Range(0, hex.Length)
                             .Where(x => x % 2 == 0)
                             .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                             .ToArray();
        }
        //---------------------------------------------------------------------------------------
        static void ArrayCut(ref string[] ar, int n) // вырезать из массива
        {
            string[] tempAr = new string[ar.Length];
            int counter = 1;
            for (int j = 0; j < ar.Length && j < 10; j++)
            {
                if (j != n)
                {
                    tempAr[counter] = ar[j];
                    counter++;
                }

            }
            ar = tempAr;
        }
        //---------------------------------------------------------------------------------------
        static void ArrayPop(ref string[] ar, string s) // добавить первым в массив
        {
            string[] tempAr = new string[ar.Length + 1];
            tempAr[0] = s;
            for (int j = 0; j < ar.Length && j < 10; j++)
            {
                tempAr[j + 1] = ar[j];
            }
            ar = tempAr;
        }

    }
}