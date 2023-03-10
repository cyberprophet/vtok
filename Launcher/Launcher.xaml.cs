using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using ShareInvest.Infrastructure.Http;
using ShareInvest.Infrastructure.Kiwoom;
using ShareInvest.Services;

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Media;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Threading;

namespace ShareInvest
{
    public partial class Launcher : Window
    {
        public Launcher()
        {
            var isRegistered = register.GetValue(Properties.Resources.TITLE);

            register.IsWritable = isRegistered;

            menu = new System.Windows.Forms.ContextMenuStrip
            {
                Cursor = System.Windows.Forms.Cursors.Hand
            };
            menu.Items.AddRange(new[]
            {
#if DEBUG
                new System.Windows.Forms.ToolStripMenuItem
                {
                    Name = nameof(Properties.Resources.APP),
                    Text = Properties.Resources.APP[..^4]
                },
                new System.Windows.Forms.ToolStripMenuItem
                {
                    Name = nameof(Properties.Resources.SERVER),
                    Text = Properties.Resources.SERVER[..^4]
                },
#endif
                new System.Windows.Forms.ToolStripMenuItem
                {
                    Text = isRegistered ? Properties.Resources.UNREGISTER :
                                          Properties.Resources.REGISTER,

                    Name = nameof(Properties.Resources.REGISTER)
                },
                new System.Windows.Forms.ToolStripMenuItem
                {
                    Name = nameof(Properties.Resources.INSTALL),
                    Text = Properties.Resources.INSTALL
                },
                new System.Windows.Forms.ToolStripMenuItem
                {
                    Name=nameof(Properties.Resources.TOREMOVECOOKIE),
                    Text = Properties.Resources.TOREMOVECOOKIE
                },
                new System.Windows.Forms.ToolStripMenuItem
                {
                    Name = nameof(Properties.Resources.EXIT),
                    Text = Properties.Resources.EXIT
                }
            });
            menu.ItemClicked += async (sender, e) =>
            {
                switch (e.ClickedItem.Name)
                {
                    case nameof(Properties.Resources.REGISTER):

                        e.ClickedItem.Text = Properties.Resources.UNREGISTER
                                                                 .Equals(e.ClickedItem.Text) ? Properties.Resources.REGISTER :
                                                                                               Properties.Resources.UNREGISTER;
                        register.IsWritable = register.IsWritable is false;

                        var res = register.AddStartupProgram(Properties.Resources.TITLE,
                                                             string.Concat(Assembly.GetEntryAssembly()?
                                                                                   .ManifestModule
                                                                                   .Name[..^4],
                                                                           Properties.Resources.EXE));

                        if (string.IsNullOrEmpty(res) is false &&
                            notifyIcon != null)
                        {
                            notifyIcon.Text = res;
                        }
                        return;

                    case nameof(Properties.Resources.SERVER):

                        await ExecuteAsync(Properties.Resources.SERVER);

                        return;

                    case nameof(Properties.Resources.APP):

                        await ExecuteAsync(Properties.Resources.APP);

                        return;

                    case nameof(Properties.Resources.INSTALL):

                        if (kiwoom.IsNotInstalled)
                            Process.Start(new ProcessStartInfo(Properties.Resources.KIWOOM)
                            {
                                UseShellExecute = kiwoom.IsNotInstalled
                            });
                        else
                            return;

                        break;

                    case nameof(Properties.Resources.TOREMOVECOOKIE):

                        if (launcher.CoreWebView2 != null)
                        {
                            launcher.CoreWebView2.CookieManager.DeleteAllCookies();
                        }
                        else
                            return;

                        break;
                }
                Visibility = Visibility.Hidden;

                Close();
            };
            icons = new[]
            {
                Properties.Resources.RECYCLING,
                Properties.Resources.RECYCLE
            };
            notifyIcon = new System.Windows.Forms.NotifyIcon
            {
                ContextMenuStrip = menu,
                Visible = true,
                Text = Properties.Resources.TITLE,
                Icon = icons[^1],
                BalloonTipIcon = System.Windows.Forms.ToolTipIcon.Info
            };
            notifyIcon.MouseDoubleClick += (sender, e) =>
            {
                if (IsVisible == false)
                {
                    Show();
                }
            };
            timer = new DispatcherTimer
            {
                Interval = new TimeSpan(0, 0, 1)
            };
            timer.Tick += async (sender, e) =>
            {
                await InitializeCoreWebView2Async();

                var now = DateTime.Now;

                switch (now.Minute % 3)
                {
                    case 0 when now.Second == 0:

                        if (UserLoginInfo != null)
                        {
                            var name = Properties.Resources.APP[..^4];

                            await Startup.StartProcess(name,
                                                       UserLoginInfo);
                        }
                        break;

                    case 1 when now.Second == 0x1E:

                        break;
                }
                notifyIcon.Icon = icons[now.Second % 2];
            };
            InitializeComponent();

            Title = Properties.Resources.TITLE;

            var hRgn = WindowAttribute.CreateRoundRectRgn(0,
                                                          0,
                                                          menu.Width,
                                                          menu.Height,
                                                          9,
                                                          9);
            var preference = DWM_WINDOW_CORNER_PREFERENCE.DWMWCP_ROUND;

            WindowAttribute.DwmSetWindowAttribute(new WindowInteropHelper(this).EnsureHandle(),
                                                  DWMWINDOWATTRIBUTE.DWMWA_WINDOW_CORNER_PREFERENCE,
                                                  ref preference,
                                                  sizeof(uint));

            _ = WindowAttribute.SetWindowRgn(menu.Handle, hRgn, true);

            timer.Start();
        }
        async Task InitializeCoreWebView2Async()
        {
            if (launcher.Source != null)
            {
                using (var sp = new SoundPlayer(Properties.Resources.BINGO))

                    if (string.IsNullOrEmpty(Stamp) is false &&
                        Stamp.Equals(notifyIcon.Text) is false)
                    {
                        string res = await client.GetUserAuthAsync(JToken.FromObject(new
                        {
                            stamp = Stamp
                        }));
                        UserLoginInfo = JsonConvert.DeserializeObject<Models.UserLoginInfo>(res);

                        notifyIcon.Text = UserLoginInfo?.LoginProvider;

                        Stamp = UserLoginInfo?.LoginProvider;

                        sp.PlaySync();
                    }
                return;
            }
            await launcher.EnsureCoreWebView2Async();

            launcher.NavigationStarting += (sender, args) =>
            {
#if DEBUG
                Debug.WriteLine(' ');
                Debug.WriteLine(sender, nameof(launcher.NavigationStarting));

                Status.GetProperites(args);
#endif
                if (args.Uri.Split('?')[^1]
                            .StartsWith(nameof(Stamp),
                                        StringComparison.OrdinalIgnoreCase))

                    Stamp = args.Uri.Split('=')[^1]
                                    .Trim();
            };
            launcher.SourceChanged += (sender, args) =>
            {
#if DEBUG
                Debug.WriteLine(' ');
                Debug.WriteLine(sender, nameof(launcher.SourceChanged));

                Status.GetProperites(args);
#endif
            };
            launcher.ContentLoading += (sender, args) =>
            {
#if DEBUG
                Debug.WriteLine(' ');
                Debug.WriteLine(sender, nameof(launcher.ContentLoading));

                Status.GetProperites(args);
#endif
            };
            launcher.CoreWebView2.HistoryChanged += (sender, e) =>
            {
#if DEBUG
                Debug.WriteLine(' ');
                Debug.WriteLine(sender, nameof(launcher.CoreWebView2.HistoryChanged));

                Status.GetProperites(e);
#endif
            };
            launcher.CoreWebView2.DOMContentLoaded += (sender, e) =>
            {
#if DEBUG
                Debug.WriteLine(e.NavigationId, nameof(launcher.CoreWebView2.DOMContentLoaded));
#endif
            };
            launcher.NavigationCompleted += (sender, args) =>
            {
#if DEBUG
                Debug.WriteLine(' ');
                Debug.WriteLine(sender, nameof(launcher.NavigationCompleted));

                Status.GetProperites(args);
#endif
            };
            launcher.CoreWebView2.WebResourceResponseReceived += (sender, args) =>
            {
                var resHeaders = args.Response.Headers.GetEnumerator();

                do
                {
#if DEBUG
                    Debug.WriteLine(resHeaders.Current.Value,
                                    resHeaders.Current.Key);
#endif
                    if (Properties.Resources.TRAILER.Equals(resHeaders.Current.Key))
                        Stamp = resHeaders.Current.Value;
                }
                while (resHeaders.MoveNext());
            };
            launcher.WebMessageReceived += (sender, args) =>
            {
#if DEBUG
                Debug.WriteLine(' ');
                Debug.WriteLine(sender, nameof(launcher.WebMessageReceived));

                Status.GetProperites(args);
#endif
            };
            launcher.Source = new Uri(string.Concat(Status.Address,
                                                    '/',
                                                    Properties.Resources.LOGIN,
                                                    Status.Address,
                                                    '/',
                                                    Properties.Resources.PROFILE));
        }
        async Task ExecuteAsync(string program)
        {
            foreach (var info in Install.GetVersionInfo(program))
            {
                var index = info.FileName.IndexOf(Properties.Resources.PUBLISH);

                if (index < 0)
                    continue;

                await client.PostAsync(info.GetType().Name,
                                       JsonConvert.SerializeObject(new Models.FileVersionInfo
                                       {
                                           App = program[..^4],
                                           Path = Path.GetDirectoryName(info.FileName)?[index..],
                                           FileName = Path.GetFileName(info.FileName),
                                           CompanyName = info.CompanyName,
                                           FileBuildPart = info.FileBuildPart,
                                           FileDescription = info.FileDescription,
                                           FileMajorPart = info.FileMajorPart,
                                           FileMinorPart = info.FileMinorPart,
                                           FilePrivatePart = info.FilePrivatePart,
                                           FileVersion = info.FileVersion,
                                           InternalName = info.InternalName,
                                           OriginalFileName = info.OriginalFilename,
                                           PrivateBuild = info.PrivateBuild,
                                           ProductBuildPart = info.ProductBuildPart,
                                           ProductMajorPart = info.ProductMajorPart,
                                           ProductMinorPart = info.ProductMinorPart,
                                           ProductName = info.ProductName,
                                           ProductPrivatePart = info.ProductPrivatePart,
                                           ProductVersion = info.ProductVersion,
                                           Publish = DateTime.Now.Ticks,
                                           File = await File.ReadAllBytesAsync(info.FileName)
                                       }));
            }
        }
        void OnClosing(object sender, CancelEventArgs e)
        {
            if (MessageBoxResult.Cancel.Equals(MessageBox.Show(Properties.Resources.WARNING.Replace('|', '\n'),
                                                               Title,
                                                               MessageBoxButton.OKCancel,
                                                               MessageBoxImage.Warning,
                                                               MessageBoxResult.Cancel)))
            {
                e.Cancel = true;

                return;
            }
            GC.Collect();
        }
        void OnStateChanged(object sender, EventArgs e)
        {
            if (WindowState.Normal != WindowState)
            {
                Hide();
            }
        }
        string? Stamp
        {
            get; set;
        }
        Models.UserLoginInfo? UserLoginInfo
        {
            get; set;
        }
        readonly CoreRestClient client = new(Status.Address);
        readonly Register register = new(Properties.Resources.RUN);
        readonly OpenAPI kiwoom = new();
        readonly DispatcherTimer timer;
        readonly System.Windows.Forms.ContextMenuStrip menu;
        readonly System.Windows.Forms.NotifyIcon notifyIcon;
        readonly System.Drawing.Icon[] icons;
    }
}