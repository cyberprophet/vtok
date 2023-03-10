using Microsoft.AspNetCore.SignalR.Client;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using ShareInvest.Infrastructure;
using ShareInvest.Mappers;
using ShareInvest.Models;
using ShareInvest.Models.OpenAPI;
using ShareInvest.Observers;
using ShareInvest.Observers.OpenAPI;
using ShareInvest.Observers.Socket;
using ShareInvest.Properties;
using ShareInvest.Services;

using System.ComponentModel;
using System.Diagnostics;

namespace ShareInvest;

partial class Securities : Form
{
    internal Securities(Icon[] icons,
                        string[] args,
                        ICoreClient client,
                        ISocketClient<MessageEventArgs> socket,
                        string key)
    {
        this.key = key;
        this.icons = icons;
        this.client = client;
        this.socket = socket;

        userLoginInfo = new UserLoginInfo
        {
            LoginProvider = args[0],
            ProviderKey = args[1]
        };
        securities = SecuritiesExtensions.ConfigureServices(key);

        InitializeComponent();

        socket.Send += new EventHandler<MessageEventArgs>((sender, e) =>
        {
            switch (e)
            {
                case InstructEventArgs renew when securities is AxKH ax:

                    ax.CommRqData(new Models.OpenAPI.Request.OPW00004
                    {
                        Value = new[]
                        {
                            renew.AccNo,
                            string.Empty,
                            "0",
                            "00"
                        },
                        PrevNext = 0
                    });
                    ax.CommRqData(new Models.OpenAPI.Request.OPW00005
                    {
                        Value = new[]
                        {
                            renew.AccNo,
                            string.Empty,
                            "00"
                        },
                        PrevNext = 0
                    });
                    return;

                case SignalEventArgs signal:
                    notifyIcon.Text = $"{DateTime.Now:g}\n[{signal.State}] {sender?.GetType().Name}";
                    return;
            }
#if DEBUG
            Debug.WriteLine($"something went wrong with the {e}.", sender);
#endif
        });
        on = socket.On(nameof(IHubs.InstructToRenewAssetStatus));

        timer.Start();

        chart = new Queue<Models.OpenAPI.Response.OPT10081>(0x100);
    }
    async Task OnReceiveMessage(AxMessageEventArgs? e)
    {
        switch (e?.Screen)
        {
            case "0106" or "0100":

                Dispose(securities as Control);

                break;
        }
        var now = DateTime.Now;
        var param = $"{now:G}\n[{e?.Code}] {e?.Title}({e?.Screen})";
        var message = new KiwoomMessage
        {
            Code = e?.Code,
            Title = e?.Title,
            Screen = e?.Screen,
            Lookup = now.Ticks,
            Key = key
        };
        notifyIcon.Text = param.Length < 0x40 ? param :
                                                $"[{e?.Code}] {e?.Title}({e?.Screen})";

        _ = await client.PostAsync(message.GetType().Name, message);
    }
    async Task OnReceiveMessage(UserInfoEventArgs? e)
    {
        if (e is null)
            return;

        e.User.Key = key;

        var res = await client.PostAsync(e.User.GetType().Name, e.User);

        string content = await client.PostAsync(1,
                                                nameof(Account),
                                                new IntegrationAccount
                                                {
                                                    LoginProvider = userLoginInfo.LoginProvider,
                                                    ProviderKey = userLoginInfo.ProviderKey,
                                                    SerialNumber = key,
                                                    AccountNumber = Resources.OP
                                                });
        string[] accounts = JsonConvert.DeserializeObject<string[]>(content) ?? Array.Empty<string>();

        switch (securities)
        {
            case AxKH ax when e.User is KiwoomUser kw && kw.Accounts is not null:

                foreach (var acc in kw.Accounts)
                {
                    var compareTo = acc[^2..].CompareTo("31");

                    if (compareTo < 0)
                    {
                        ax.CommRqData(new Models.OpenAPI.Request.OPW00004
                        {
                            Value = new[]
                            {
                                acc,
                                string.Empty,
                                "0",
                                "00"
                            },
                            PrevNext = 0
                        });
                        ax.CommRqData(new Models.OpenAPI.Request.OPW00005
                        {
                            Value = new[]
                            {
                                acc,
                                string.Empty,
                                "00"
                            },
                            PrevNext = 0
                        });
                        if (accounts.Length > 0 &&
                            Array.Exists(accounts,
                                         match => match.Equals(acc)))
                            continue;

                        string register = await client.PostAsync(1,
                                                                 string.Concat(nameof(Account),
                                                                               '/',
                                                                               nameof(register)),

                                                                 new IntegrationAccount
                                                                 {
                                                                     LoginProvider = userLoginInfo.LoginProvider,
                                                                     ProviderKey = userLoginInfo.ProviderKey,
                                                                     AccountNumber = acc,
                                                                     SerialNumber = key
                                                                 });
                        continue;
                    }
                    if (compareTo == 0)
                    {


                        continue;
                    }
#if DEBUG
                    Debug.WriteLine(JsonConvert.SerializeObject(new
                    {
                        account = acc,
                        name = kw.Name
                    },
                    Formatting.Indented));
#endif
                }
                break;
        }
    }
    async Task OnReceiveMessage(JsonMessageEventArgs? e)
    {
        if (e?.Convey is null)
        {
            return;
        }
#if DEBUG
        Debug.WriteLine(JsonConvert.SerializeObject(new
        {
            name = e.Convey.GetType().Name,
            convey = e.Convey
        }));
#endif
        switch (e.Convey)
        {
            case Models.OpenAPI.Response.OPT10081 opt10081:

                chart.Enqueue(opt10081);

                return;

            case Models.OpenAPI.Response.OPTKWFID when IsAdministrator is false:

                return;

            case Models.OpenAPI.Request.OPT10081 tr when securities is AxKH ax:

                while (chart.TryDequeue(out Models.OpenAPI.Response.OPT10081? item))
                {
                    var res = await client.PostAsync(item.GetType().Name, item);

                    if (int.TryParse(Convert.ToString(res), out int changes) &&
                        changes > 0)
                        continue;

                    chart.Clear();

                    tr.PrevNext = chart.Count;
                }
                if (tr.PrevNext == 2)
                {
                    ax.CommRqData(tr);

                    return;
                }
                if (await GetCodeAsync() is string code && code.Length == 6)

                    ax.CommRqData(new Models.OpenAPI.Request.OPT10081
                    {
                        Value = new[]
                        {
                            code,
                            DateTime.Now.ToString("yyyyMMdd"),
                            "1"
                        },
                        PrevNext = 0
                    });
                return;
        }
        _ = await client.PostAsync(e.Convey.GetType().Name, e.Convey);
    }
    async Task<string> GetCodeAsync()
    {
        var contents = await client.GetAsync(nameof(Models.OpenAPI.Response.OPT10081)) as string;

        return string.IsNullOrEmpty(contents) ? string.Empty : contents.Replace("\"", string.Empty);
    }
    async Task OnReceiveMessage(RealMessageEventArgs? e)
    {
        if (e is not null)
        {
            if (IsAdministrator)
            {
                await socket.Hub.SendAsync(e.Type, e.Key, e.Data);
            }
            if (Resources.OPERATION.Equals(e.Type) &&
                Mappers.Kiwoom.MarketOperation.Get(e.Data.Split('\t')[0]) is
                Mappers.Kiwoom.EnumMarketOperation operation)
            {
                Delay.Instance.Milliseconds = operation switch
                {
                    Mappers.Kiwoom.EnumMarketOperation.장시작 => 0xC9,

                    Mappers.Kiwoom.EnumMarketOperation.장마감 => await new Func<Task<int>>(async () =>
                    {
                        var ax = securities as AxKH;

                        if (IsAdministrator)
                        {
                            ax?.GetCodeListByMarket();
                        }
                        if (await GetCodeAsync() is string code && code.Length == 6)

                            ax?.CommRqData(new Models.OpenAPI.Request.OPT10081
                            {
                                Value = new[]
                                {
                                    code,
                                    DateTime.Now.ToString("yyyyMMdd"),
                                    "1"
                                },
                                PrevNext = 0
                            });
                        return 0x259;
                    })(),

                    _ => 0x259
                };
                notifyIcon.Text = $"{DateTime.Now:G}\n{Enum.GetName(operation)}";
            }
        }
    }
    void OnReceiveMessage(object? sender, MessageEventArgs e)
    {
        _ = BeginInvoke(new Action(async () =>

            await (e.GetType().Name switch
            {
                nameof(RealMessageEventArgs) =>

                    OnReceiveMessage(e as RealMessageEventArgs),

                nameof(JsonMessageEventArgs) =>

                    OnReceiveMessage(e as JsonMessageEventArgs),

                nameof(AxMessageEventArgs) =>

                    OnReceiveMessage(e as AxMessageEventArgs),

                nameof(UserInfoEventArgs) =>

                    OnReceiveMessage(e as UserInfoEventArgs),

                _ => Task.CompletedTask
            })));
    }
    void TimerTick(object sender, EventArgs e)
    {
        if (securities is null)
        {
            timer.Stop();

            strip.ItemClicked -= StripItemClicked;

            Dispose();
        }
        else if (FormBorderStyle.Equals(FormBorderStyle.Sizable) &&
                 WindowState.Equals(FormWindowState.Minimized) is false)
        {
            _ = Task.Run(async () =>
            {
                while (HubConnectionState.Disconnected == socket.Hub.State)
                    try
                    {
                        await socket.Hub.StartAsync();
                    }
                    catch
                    {
                        await Task.Delay(0x400);
                    }
                if (await client.GetAsync(nameof(KiwoomUser),
                                          JToken.FromObject(new
                                          {
                                              key
                                          }))
                                              is string str)
                {
                    var admin = JsonConvert.DeserializeObject<KiwoomUser>(str);

                    IsAdministrator = admin != null && admin.IsAdministrator;
                }
                var ax = securities as AxKH;
                var storage = int.MinValue;

                while (HubConnectionState.Connected == socket.Hub.State)
                {
                    var count = Delay.Instance.Count;

                    if (storage != count && IsConnected && ax?.ConnectState == 1)
                    {
                        await socket.Hub.SendAsync(nameof(IHubs.GatherCluesToPrioritize),
                                                   count);
                        storage = count;
                    }
                    await Task.Delay(0x200);
                }
            });
            WindowState = FormWindowState.Minimized;
        }
        else
        {
            var now = DateTime.Now;

            if (IsConnected)
            {
                if (IsAdministrator &&
                    now.Hour == 8 && now.Minute == 1 && now.Second % 9 == 0 &&
                    (int)now.DayOfWeek > 0 && (int)now.DayOfWeek < 6)
                {
                    Dispose(securities as Control);
                }
                notifyIcon.Icon = icons[now.Second % 4];
            }
            else
                notifyIcon.Icon = icons[^1];

            if (now.Second == 0x3A && now.Minute % 2 == 0 &&
               (now.Hour == 5 || now.Hour == 6 && now.Minute < 0x35) is false)
            {
                _ = BeginInvoke(new Action(() =>
                {
                    try
                    {
                        if (securities is AxKH ax &&
                            ax.ConnectState == 0 &&
                            Process.GetProcessesByName(Resources.OP).Length == 0)
                        {
                            IsConnected = ax.CommConnect();

                            if (IsConnected)
                            {
                                securities.Send += OnReceiveMessage;

                                Delay.Instance.Run();
                            }
                        }
                    }
                    catch
                    {
                        Dispose(securities as AxKH);
                    }
                }));
            }
        }
    }
    void StripItemClicked(object? sender, ToolStripItemClickedEventArgs e)
    {
        if (e.ClickedItem.Name.Equals(reference.Name))

            _ = BeginInvoke(new Action(() =>
            {
                if (IsConnected)
                    _ = Process.Start(new ProcessStartInfo(Resources.URL)
                    {
                        UseShellExecute = true
                    });
                else
                {
                    IsConnected = securities switch
                    {
                        AxKH ax => ax.CommConnect(),

                        _ => false
                    };
                    if (IsConnected)
                    {
                        securities.Send += OnReceiveMessage;

                        Delay.Instance.Run();
                    }
                }
            }));
        else
            Close();
    }
    void SecuritiesResize(object sender, EventArgs e)
    {
        _ = BeginInvoke(new Action(() =>
        {
            SuspendLayout();

            Visible = false;
            ShowIcon = false;
            notifyIcon.Visible = true;

            ResumeLayout();

            Set(securities as Control);
        }));
    }
    void JustBeforeFormClosing(object sender, FormClosingEventArgs e)
    {
        if (CloseReason.UserClosing.Equals(e.CloseReason) &&
            DialogResult.Cancel.Equals(MessageBox.Show(Resources.WARNING.Replace('|', '\n'),
                                                       Text,
                                                       MessageBoxButtons.OKCancel,
                                                       MessageBoxIcon.Question,
                                                       MessageBoxDefaultButton.Button2)))
        {
            e.Cancel = true;

            return;
        }
        Dispose(securities as AxKH);
    }
    void Set(IComponent? component)
    {
        if (component is Control control)
        {
            Controls.Add(control);

            control.Dock = DockStyle.Fill;
            control.Show();

            FormBorderStyle = FormBorderStyle.None;

            CenterToScreen();
        }
        else
            Close();
    }
    void Dispose(IComponent? component)
    {
        if (component is Control control)

            control.Dispose();

        Dispose();
    }
    bool IsConnected
    {
        get; set;
    }
    bool IsAdministrator
    {
        get; set;
    }
    readonly ISecuritiesMapper<MessageEventArgs> securities;
    readonly ISocketClient<MessageEventArgs> socket;
    readonly ICoreClient client;
    readonly Queue<Models.OpenAPI.Response.OPT10081> chart;
    readonly UserLoginInfo userLoginInfo;
    readonly Icon[] icons;
    readonly string key;
}