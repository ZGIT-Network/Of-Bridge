using ModernWpf;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Of_Bridge
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool isSupportDarkMode = Environment.OSVersion.Version.Major == 10 && Environment.OSVersion.Version.Build >= 17763;
        private bool isSupportDarkMode2 = Environment.OSVersion.Version.Major == 10 && Environment.OSVersion.Version.Build >= 18362;

        public MainWindow()
        {
            InitializeComponent();
            Append("应用开启");
        }
        
        protected override void OnContentRendered(EventArgs e)
        {
            base.OnContentRendered(e);
            Window_ActualThemeChanged(this, new());
        }
        private void Window_ActualThemeChanged(object sender, RoutedEventArgs e)
        {
            IntPtr hwnd = new WindowInteropHelper(this).Handle;
            if (isSupportDarkMode)
            {
                if (ThemeManager.GetActualTheme(this) == ElementTheme.Dark)
                {
                    UxTheme.DwmSetWindowAttribute(hwnd, UxTheme.DwmWindowAttribute.DWMWA_USE_IMMERSIVE_DARK_MODE, ref UxTheme.TrueValue, sizeof(int));
                }
                else
                {
                    UxTheme.DwmSetWindowAttribute(hwnd, UxTheme.DwmWindowAttribute.DWMWA_USE_IMMERSIVE_DARK_MODE, ref UxTheme.FalseValue, sizeof(int));
                } 
            }

        }


        private bool AppState { get; set; }
        private Socket? serverSocket { get; set; }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            if (((Button)sender).Content is "开始联机")
            {
                ((Button)sender).IsEnabled = false;
                string[] _temple_1 = OfBridge_IPInput.Text.Split(':');
                OfBridge_Error.Text = string.Empty;
                try
                {
                    IPEndPoint serverIp = new((await Dns.GetHostAddressesAsync(_temple_1.First())).First(), Convert.ToInt32(_temple_1[1]));
                    IPEndPoint any = new(IPAddress.Any, 0);

                    // 获得连接用的
                    serverSocket = new(SocketType.Stream, ProtocolType.Tcp);
                    serverSocket.Bind(any);
                    serverSocket.Listen(100);
                    AppState = true;
                    
                    new Thread(() =>
                    {
                        if (!MotdBroadCaster((serverSocket.LocalEndPoint as IPEndPoint)!.Port))
                        {
                            AppState = false;
                        }
                    })
                        .Start();
                    new Thread(() =>
                    {
                        Socket? client = null;
                        Socket? server = null;
                        try
                        {
                            while (AppState)
                            {
                                client = serverSocket.Accept();
                                server = new(SocketType.Stream, ProtocolType.Tcp);
                                server.Connect(serverIp);
                                Append($"获得连接!!! {client.LocalEndPoint as IPEndPoint}");
                                new Thread(() => Forward(client, server)).Start();
                                new Thread(() => Forward(server, client)).Start();
                            }
                        }
                        catch(Exception ex)
                        {

                            try
                            {
                                client?.Disconnect(false);
                                server?.Close();
                            }
                            catch (Exception e2x)
                            {
                                Append(e2x.ToString());
                            }
                            AppState = false;
                            Append(ex.ToString());
                        }
                    }).Start();
                    ((Button)sender).Content = "关闭联机";
                    ((Button)sender).IsEnabled = true;

                }
                catch (Exception ex) 
                { 
                    OfBridge_Error.Text = "输入的格式不正确。";
                    Append(ex.ToString());
                    ((Button)sender).IsEnabled = true;
                }
            }
            else
            {
                //serverSocket?.Disconnect(true);
                serverSocket?.Close();
                ((Button)sender).Content = "开始联机";
            }
            
        }

        private void Append(string str) => App.Current?.Dispatcher.Invoke(() =>
        {
            Of_Log.AppendText($"\n[{DateTimeOffset.Now}] {str}");
        });

        private bool MotdBroadCaster(int port)
        {
            try
            {
                UdpClient udpClient = new("224.0.2.60", 4445);
                byte[] bytes = Encoding.UTF8.GetBytes($"[MOTD]§eOf Bridge 2.0 || 双击进入[/MOTD][AD]{port}[/AD]");
                Append("联机广播已开启!!!");
                while (AppState)
                {
                    udpClient.Send(bytes, bytes.Length);
                }
            }
            catch(Exception ex)
            {
                Append("UDP 发包失败," + ex);
            }
            return false;
        }
        private void Forward(Socket s,Socket c)
        {
            int bufferLength = App.Current?.Dispatcher.Invoke(() => (int)OfApp_Number.Value) ?? 1536;
            try
            {
                byte[] buffer = new byte[bufferLength];
                while (AppState)
                {
                    if (AppState)
                    {
                        int count = s.Receive(buffer, 0, buffer.Length, SocketFlags.None);
                        if (count > 0)
                        {
                            c.Send(buffer, 0, count,SocketFlags.None);
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Append(ex.ToString());
            }
            finally
            {
                c.Close();
                s.Close();
            }
        }

        private void Hyperlink_Click(object sender, RoutedEventArgs e)
        {
            Process.Start((sender as Hyperlink)!.NavigateUri.ToString());
        }
    }
}
