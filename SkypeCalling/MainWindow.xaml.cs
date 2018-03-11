using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
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
using SKYPE4COMLib;

namespace SkypeCalling
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        #region Глобальные данные
        public static AutoResetEvent NewJabberMessageWindowSignal = new AutoResetEvent(false);
        AutoResetEvent GlobalStopEvent = new AutoResetEvent(false);
        Skype _skype = new SKYPE4COMLib.Skype();
        int LastSignalWindowHWND = 0;
        bool Stopclick = false;

        #endregion

        #region Работа с формой 
        private void START(object sender, RoutedEventArgs e)
        {
            try
            {
                TrayIcon.Icon = SkypeCalling.Properties.Resources.telephone_green;
                TrayIcon.Visible = true;
                Stopclick = false;
                if (_skype.Client.IsRunning != true)
                {
                    _skype.Client.Start(false, true);
                }
                _skype.Attach(5, true);


                Utils_.ActionInThread(() => { CallSignalThread(); });

                Utils_.ActionInThread(() => { NewJabberMessageMonitorThread(); });
            }
            catch (Exception ecc)
            {
                MessageBox.Show("Ошибка запуска мониторинга : " + ecc.Message);
            }


        }
        private void STOP(object sender, RoutedEventArgs e)
        {
            Stopclick = true;
            GlobalStopEvent.Set();
            NewJabberMessageWindowSignal.Set();
            TrayIcon.Icon = SkypeCalling.Properties.Resources.telephone_red;
            TrayIcon.Visible = true;
        }
        private void Settings_Load_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Settings_Content.Text = File.ReadAllText(Environment.CurrentDirectory + "\\settings.ini");
                MessageBox.Show("Success Load !");
            }
            catch (Exception ex)
            {

            }
        }
        private void Settings_Save_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                File.WriteAllText(Environment.CurrentDirectory + "\\settings.ini", Settings_Content.Text);
                MessageBox.Show("Success Save !");
            }
            catch (Exception ex)
            {

            }
        }
        #endregion

        #region Основные методы
        private void CallSignalThread()
        {
            string tel = GetNumberPhone();//получим номер телефона 
            while (true)
            {
                if (Stopclick == true)
                    break;
                NewJabberMessageWindowSignal.WaitOne(); //ждем сигнала от потока мониторинга нового сообщения .
                while (true)
                {
                    if (Stopclick == true)
                        break;
                    Call call = _skype.PlaceCall(tel); //Вызываем по телефону (на счету должны быть деньги)
                    if (call.Status == TCallStatus.clsRefused)
                        break;
                    if (call.Status == TCallStatus.clsBusy)
                        break;
                    if (call.Status == TCallStatus.clsRouting)
                        break;
                    GlobalStopEvent.WaitOne(20000);
                }
            }
        }
        private void NewJabberMessageMonitorThread()
        {
            while (true)
            {
                if (Stopclick == true)
                    break;
                string clientname = "";
                // ищем все окна и инфо про них
                List<int> allhandles = Gui.FindAllWindows_TopWindows();
                List<Gui.WindowInfoEx> allwnds = allhandles.Select(x => Gui.GetWindowInformation(x)).ToList(); //получаем их хендлы
                foreach (var winfo in allwnds)
                {
                    if (Stopclick == true)
                        break;
                    //pidgin
                    if (winfo.className == "gdkWindowToplevel")
                    {
                        Process procs = Process.GetProcessById(winfo.pid);
                        if (procs.ProcessName == "pidgin") // проверяем действительно ли новое окно это окно pidgin jabber.
                        {
                            if (user32.IsWindowVisible((IntPtr)winfo.hwnd) == true) //если оно видимое
                                if (user32.IsIconic((IntPtr)winfo.hwnd) == false)
                                {
                                    if (winfo.hwnd != LastSignalWindowHWND)//если мы еще не звонили 
                                    {

                                        LastSignalWindowHWND = winfo.hwnd;
                                        clientname = winfo.windowName;
                                        NewJabberMessageWindowSignal.Set();
                                        break;

                                    }
                                }
                        }
                    }
                    //trillian
                    if (winfo.className == "icoJabber")
                    {
                        Process procs = Process.GetProcessById(winfo.pid);
                        if (procs.ProcessName == "trillian")
                        {
                            if (user32.IsWindowVisible((IntPtr)winfo.hwnd) == true)
                                if (user32.IsIconic((IntPtr)winfo.hwnd) == false)
                                {
                                    if (winfo.hwnd != LastSignalWindowHWND)
                                    {
                                        LastSignalWindowHWND = winfo.hwnd;
                                        clientname = winfo.windowName;
                                        NewJabberMessageWindowSignal.Set();
                                        break;
                                    }
                                }
                        }
                    }
                }
                GlobalStopEvent.WaitOne(30000);
            }
        }
        private string GetNumberPhone()
        {
            string res = Extract.BetweenEnd(File.ReadAllText(Environment.CurrentDirectory + "\\settings.ini"), "=");
            return res;
        }
        #endregion

        #region Сворачивание в Трей и работа контекстного меню в Трее

        private System.Windows.Forms.NotifyIcon TrayIcon = null;
        private ContextMenu TrayMenu = null;
        // переопределяем обработку первичной инициализации приложения
        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e); // базовый функционал приложения в момент запуска.
            createTrayIcon();// создание нашей иконки.
        }
        private WindowState fCurrentWindowState = WindowState.Normal;
        public WindowState CurrentWindowState
        {
            get { return fCurrentWindowState; }
            set { fCurrentWindowState = value; }
        }


        private bool fCanClose = false;
        public bool CanClose// флаг, позволяющий или запрещающий выход из приложения
        {
            get { return fCanClose; }
            set { fCanClose = value; }
        }

        private bool createTrayIcon()
        {
            bool result = false;
            if (TrayIcon == null) // только если мы не создали иконку ранее
            {
                TrayIcon = new System.Windows.Forms.NotifyIcon(); // создаем новую
                TrayIcon.Icon = SkypeCalling.Properties.Resources.telephone_red; // указать полный namespace
                TrayMenu = Resources["TrayMenu"] as ContextMenu; // а здесь уже ресурсы окна и тот самый x:Key
                TrayIcon.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(_MouseDoubleClick);
                // сразу же опишем поведение при щелчке мыши, о котором мы говорили ранее
                // это будет просто анонимная функция, незачем выносить ее в класс окна
                TrayIcon.Click += delegate (object sender, EventArgs e)
                {
                    if ((e as System.Windows.Forms.MouseEventArgs).Button == System.Windows.Forms.MouseButtons.Right) // по правой кнопке  показываем меню
                    {
                        TrayMenu.IsOpen = true;
                        Activate(); // нужно отдать окну фокус, см. ниже
                    }
                };
                result = true;
            }
            else  // все переменные были созданы ранее
            {
                result = true;
            }
            TrayIcon.Visible = true; // делаем иконку видимой в трее
            return result;
        }
        private void ShowHideMainWindow(object sender, RoutedEventArgs e)
        {
            TrayMenu.IsOpen = false; // спрячем менюшку, если она вдруг видима
            if (IsVisible)// если окно видно на экране  прячем его
            {
                Hide();
                (TrayMenu.Items[0] as MenuItem).Header = "Показать";// меняем надпись на пункте меню
            }
            else  // а если не видно показываем
            {
                Show();
                (TrayMenu.Items[0] as MenuItem).Header = "Спрятать";// меняем надпись на пункте меню
                WindowState = CurrentWindowState;
                Activate(); // обязательно нужно отдать фокус окну,
                // иначе пользователь сильно удивится, когда увидит окно
                // но не сможет в него ничего ввести с клавиатуры
            }
        }

        protected override void OnStateChanged(EventArgs e)  // переопределяем встроенную реакцию на изменение состояния сознания окна
        {
            base.OnStateChanged(e);  // системная обработка
            if (this.WindowState == System.Windows.WindowState.Minimized)  // если окно минимизировали, просто спрячем
            {
                Hide();
                (TrayMenu.Items[0] as MenuItem).Header = "Показать"; // и поменяем надпись на менюшке
            }
            else
            {
                CurrentWindowState = WindowState; // в противном случае запомним текущее состояние
            }
        }

        void _MouseDoubleClick(object sender, System.Windows.Forms.MouseEventArgs e) // обработчик двойного клика мышки на развертывание из трея
        {
            this.Show();
            this.WindowState = WindowState.Normal;
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e) // переопределяем обработчик запроса выхода из приложения
        {
            base.OnClosing(e);// встроенная обработка
            if (!CanClose)  // если нельзя закрывать
            {
                e.Cancel = true;//выставляем флаг отмены закрытия
                                // запоминаем текущее состояние окна
                CurrentWindowState = this.WindowState;
                (TrayMenu.Items[0] as MenuItem).Header = "Показать";// меняем надпись в менюшке
                Hide(); // прячем окно
            }
            else // все-таки закрываемся  убираем иконку из трея
            {
                TrayIcon.Visible = false;
            }
        }

        private void MenuExitClick(object sender, RoutedEventArgs e)
        {
            CanClose = true;
            Close();
        }

        #endregion
    }
}
