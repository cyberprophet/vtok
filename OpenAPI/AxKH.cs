﻿using AxKHOpenAPILib;

using Newtonsoft.Json;

using ShareInvest.Mappers;
using ShareInvest.Models.OpenAPI.Request;
using ShareInvest.Observers;
using ShareInvest.Observers.OpenAPI;
using ShareInvest.Properties;

using System.Diagnostics;
using System.Reflection;

namespace ShareInvest;

public partial class AxKH : UserControl,
                            ISecuritiesMapper<MessageEventArgs>
{
    public event EventHandler<MessageEventArgs>? Send;

    public int ConnectState => axAPI.GetConnectState();

    public AxKH()
    {
        Delay.Instance.Milliseconds = 0x259;

        InitializeComponent();
    }
    public void GetCodeListByMarket()
    {
        var codeListByMarket = new List<string>(axAPI.GetCodeListByMarket("0")
                                                     .Split(';')
                                                     .OrderBy(o => Guid.NewGuid()));

        codeListByMarket.AddRange(axAPI.GetCodeListByMarket("10")
                                       .Split(';')
                                       .OrderBy(o => Guid.NewGuid()));

        foreach (var tr in Tr.OPTKWFID.GetListOfStocks(codeListByMarket))
        {
            var nCodeCount = tr.PrevNext;
            tr.PrevNext = 0;

            if (tr.Value is not null)
                Delay.Instance.RequestTheMission(new Task(() =>
                {
                    OnReceiveErrorMessage(tr.RQName,
                                          axAPI.CommKwRqData(tr.Value[0],
                                                             tr.PrevNext,
                                                             nCodeCount,
                                                             0,
                                                             tr.RQName,
                                                             tr.ScreenNo));
                }));
        }
    }
    public void CommRqData(Models.OpenAPI.TR tr)
    {
        var scrNo = tr.ScreenNo;

        Delay.Instance.RequestTheMission(new Task(() =>
        {
            for (int i = 0; i < tr.Id.Length; i++)
                axAPI.SetInputValue(tr.Id[i], tr.Value?[i]);

            OnReceiveErrorMessage(tr.RQName,
                                  axAPI.CommRqData(tr.RQName,
                                                   tr.TrCode,
                                                   tr.PrevNext,
                                                   scrNo));
        }));
        var tryAdd = Constructer.TryAdd(scrNo, tr);
#if DEBUG
        Debug.WriteLine(JsonConvert.SerializeObject(new
        {
            scrNo,
            rqName = tr.RQName,
            tryAdd
        },
        Formatting.Indented));
#endif
    }
    public bool CommConnect()
    {
        axAPI.OnReceiveChejanData += OnReceiveChejanData;
        axAPI.OnReceiveRealData += OnReceiveRealData;
        axAPI.OnReceiveTrData += OnReceiveTrData;
        axAPI.OnEventConnect += OnEventConnect;
        axAPI.OnReceiveMsg += OnReceiveMessage;
        axAPI.OnReceiveConditionVer += OnReceiveConditionVersion;
        axAPI.OnReceiveRealCondition += OnReceiveRealCondition;
        axAPI.OnReceiveTrCondition += OnReceiveTrCondition;

        return axAPI.CommConnect() == 0;
    }
    void OnReceiveErrorMessage(string? sRQName, int error)
    {
        if (error < 0)
            Send?.Invoke(this,
                         new AxMessageEventArgs(Status.Error[error],
                                                sRQName,
                                                Math.Abs(error).ToString("D4")));
    }
    void OnReceiveMessage(object sender,
                          _DKHOpenAPIEvents_OnReceiveMsgEvent e)
    {
        Send?.Invoke(this,
                     new AxMessageEventArgs(e.sMsg[9..],
                                            e.sRQName,
                                            e.sScrNo));
    }
    void OnReceiveTrData(object sender,
                         _DKHOpenAPIEvents_OnReceiveTrDataEvent e)
    {
        var name = string.Concat(typeof(TR).FullName, '.', e.sTrCode);

        if (Assembly.GetExecutingAssembly()
                    .CreateInstance(name, true) is TR tr)
        {
            var ctor = Constructer.GetInstance(e.sTrCode, e.sScrNo);

            foreach (var json in tr.OnReceiveTrData(axAPI, e, ctor))
            {
                if (ctor is OPT10081 && json.Length == 1)
                {
#if DEBUG
                    Debug.WriteLine(JsonConvert.SerializeObject(new
                    {
                        ctor,
                        json
                    },
                    Formatting.Indented));
#endif
                    if (int.TryParse(json, out int next))
                    {
                        ctor.PrevNext = next;

                        Send?.Invoke(this, new JsonMessageEventArgs(ctor));
                    }
                    continue;
                }
                Send?.Invoke(this, new JsonMessageEventArgs(ctor, json));
            }
        }
        Send?.Invoke(this,
                     new AxMessageEventArgs(e.sTrCode,
                                            e.sRQName,
                                            e.sScrNo));
    }
    void OnReceiveRealData(object sender,
                           _DKHOpenAPIEvents_OnReceiveRealDataEvent e)
    {
        Send?.Invoke(this,
                     new RealMessageEventArgs(e.sRealType,
                                              e.sRealKey,
                                              e.sRealData));
    }
    void OnReceiveTrCondition(object sender, _DKHOpenAPIEvents_OnReceiveTrConditionEvent e)
    {
        throw new NotImplementedException();
    }
    void OnReceiveRealCondition(object sender, _DKHOpenAPIEvents_OnReceiveRealConditionEvent e)
    {
        throw new NotImplementedException();
    }
    void OnReceiveConditionVersion(object sender, _DKHOpenAPIEvents_OnReceiveConditionVerEvent e)
    {
        throw new NotImplementedException();
    }
    void OnReceiveChejanData(object sender, _DKHOpenAPIEvents_OnReceiveChejanDataEvent e)
    {
        throw new NotImplementedException();
    }
    void OnEventConnect(object sender,
                        _DKHOpenAPIEvents_OnEventConnectEvent e)
    {
        if (e.nErrCode == 0)
        {
            GetUserInfo(axAPI.GetLoginInfo(Resources.SERVER));

            GetCodeListByMarket();
        }
        else
            OnReceiveErrorMessage(sender.GetType().Name, e.nErrCode);
    }
    void GetUserInfo(string server)
    {
        var num = int.TryParse(axAPI.GetLoginInfo(Resources.CNT), out int cnt) ? cnt : 0;

        Send?.Invoke(this, new UserInfoEventArgs(new Models.OpenAPI.KiwoomUser
        {
            Accounts = axAPI.GetLoginInfo(Resources.LIST)
                            .Split(';')
                            .Where(o => o.Length == 0xA)
                            .ToArray(),
            Name = axAPI.GetLoginInfo(Resources.NAME),
            Id = axAPI.GetLoginInfo(Resources.ID),
            NumberOfAccounts = num,
            IsNotMock = string.IsNullOrEmpty(server) ||
                        int.TryParse(server, out int mock) && mock is not 1
        }));
    }
}