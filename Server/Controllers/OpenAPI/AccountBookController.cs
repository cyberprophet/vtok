using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

using ShareInvest.Infrastructure;
using ShareInvest.Mappers;
using ShareInvest.Models.OpenAPI.Response;
using ShareInvest.Server.Data;

namespace ShareInvest.Server.Controllers.OpenAPI;

[Route("[action]")]
public class AccountBookController : KiwoomController
{
    [ApiExplorerSettings(GroupName = "balance"),
     HttpPost]
    public async Task<IActionResult> BalanceOPW00005([FromBody] BalanceOPW00005 bal)
    {
        if (context.ClosedBalances is not null)
        {
            if (bal.Lookup == 0)
            {
                bal.Lookup = DateTime.Now.Ticks;
            }
            var tuple = await context.ClosedBalances.FindAsync(bal.AccNo,
                                                               bal.Date,
                                                               bal.Code);
            if (tuple is null)
                context.ClosedBalances.Add(bal);

            else
                service.SetValuesOfColumn(tuple, bal);

            return Ok(context.SaveChanges());
        }
        logger.LogWarning(nameof(AccountBookController), bal.Name);

        return NoContent();
    }
    [ApiExplorerSettings(GroupName = "balance"),
     HttpPost]
    public async Task<IActionResult> BalanceOPW00004([FromBody] BalanceOPW00004 bal)
    {
        if (context.Balances is not null)
        {
            if (bal.Lookup == 0)
            {
                bal.Lookup = DateTime.Now.Ticks;
            }
            var tuple = await context.Balances.FindAsync(bal.AccNo,
                                                         bal.Date,
                                                         bal.Code);
            if (tuple is null)
                context.Balances.Add(bal);

            else
                service.SetValuesOfColumn(tuple, bal);

            if (string.IsNullOrEmpty(bal.AccNo) is false)
            {
                var balance = new Models.Balance
                {
                    Amount = bal.Amount,
                    Average = bal.Average,
                    Code = bal.Code,
                    Current = bal.Current,
                    Name = bal.Name,
                    PaymentBalance = bal.PaymentBalance,
                    PreviousPurchaseQuantity = bal.PreviousPurchaseQuantity,
                    PreviousSalesQuantity = bal.PreviousSalesQuantity,
                    Purchase = bal.Purchase,
                    PurchaseQuantity = bal.PurchaseQuantity,
                    Quantity = bal.Quantity,
                    Rate = bal.Rate,
                    SalesQuantity = bal.SalesQuantity,
                    Valuation = bal.Evaluation,
                    AccNo = bal.AccNo,
                    Date = bal.Date
                };
                foreach (var groupName in UpdateTheStatusOfAssets(bal.AccNo))
                {
                    await hub.Clients.Group(groupName)
                                     .UpdateTheStatusOfBalances(balance);
                }
                return Ok(context.SaveChanges());
            }
        }
        logger.LogWarning(nameof(AccountBookController), bal.Name);

        return NoContent();
    }
    [ApiExplorerSettings(GroupName = "account"),
     HttpPost]
    public async Task<IActionResult> AccountOPW00005([FromBody] AccountOPW00005 acc)
    {
        if (context.ClosedAccounts is not null)
        {
            if (acc.Lookup == 0)
            {
                acc.Lookup = DateTime.Now.Ticks;
            }
            var tuple = await context.ClosedAccounts.FindAsync(acc.AccNo, acc.Date);

            if (tuple is null)
                context.ClosedAccounts.Add(acc);

            else
                service.SetValuesOfColumn(tuple, acc);

            if (string.IsNullOrEmpty(acc.AccNo) is false)
            {
                Models.Account asset = new()
                {
                    AccNo = acc.AccNo,
                    Date = acc.Date,
                    OrderableCash = acc.OrderableCash,
                    Balances = Array.Empty<Models.Balance>()
                };
                foreach (var groupName in UpdateTheStatusOfAssets(acc.AccNo))
                {
                    await hub.Clients.Group(groupName)
                                     .UpdateTheStatusOfAssets(asset);
                }
                return Ok(context.SaveChanges());
            }
        }
        logger.LogWarning(nameof(AccountBookController), acc.AccNo);

        return NoContent();
    }
    [ApiExplorerSettings(GroupName = "account"),
     HttpPost]
    public async Task<IActionResult> AccountOPW00004([FromBody] AccountOPW00004 acc)
    {
        if (context.Accounts is not null)
        {
            if (acc.Lookup == 0)
            {
                acc.Lookup = DateTime.Now.Ticks;
            }
            var tuple = await context.Accounts.FindAsync(acc.AccNo, acc.Date);

            if (tuple is null)
                context.Accounts.Add(acc);

            else
                service.SetValuesOfColumn(tuple, acc);

            if (string.IsNullOrEmpty(acc.AccNo) is false)
            {
                Models.Account asset = new()
                {
                    AccNo = acc.AccNo,
                    Asset = acc.Asset,
                    Balance = acc.Balance,
                    Date = acc.Date,
                    Deposit = acc.Deposit,
                    NumberOfPrints = acc.NumberOfPrints,
                    OrderableCash = string.Empty,
                    PresumeAsset = acc.PresumeAsset,
                    PresumeDeposit = acc.PresumeDeposit,
                    TotalPurchaseAmount = acc.TotalPurchaseAmount,
                    Balances = int.TryParse(acc.NumberOfPrints, out int index) && index > 0 ?

                               new Models.Balance[index] : Array.Empty<Models.Balance>()
                };
                foreach (var groupName in UpdateTheStatusOfAssets(acc.AccNo))
                {
                    await hub.Clients.Group(groupName)
                                     .UpdateTheStatusOfAssets(asset);
                }
                return Ok(context.SaveChanges());
            }
        }
        logger.LogWarning(nameof(AccountBookController), acc.AccNo);

        return NoContent();
    }
    public AccountBookController(CoreContext context,
                                 IPropertyService service,
                                 ILogger<AccountBookController> logger,
                                 IHubContext<Hubs.KiwoomHub, IHubs> hub)
    {
        this.context = context;
        this.service = service;
        this.logger = logger;
        this.hub = hub;
    }
    IEnumerable<string> UpdateTheStatusOfAssets(string accNo)
    {
        foreach (var groupName in from o in context.KiwoomUsers?.AsNoTracking()
                                  where accNo.Equals(o.AccNo)
                                  select o.Key)

            yield return groupName;
    }
    readonly CoreContext context;
    readonly IPropertyService service;
    readonly ILogger<AccountBookController> logger;
    readonly IHubContext<Hubs.KiwoomHub, IHubs> hub;
}