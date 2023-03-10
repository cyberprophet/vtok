using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using ShareInvest.Mappers;
using ShareInvest.Models;
using ShareInvest.Server.Data;

namespace ShareInvest.Server.Controllers;

[Route("[controller]"),
 Produces("application/json"),
 ProducesResponseType(StatusCodes.Status200OK),
 ProducesResponseType(StatusCodes.Status204NoContent),
 ApiController]
public class CoreMemberController : ControllerBase
{
    [ApiExplorerSettings(GroupName = "user"),
     HttpGet("{id}")]
    public async Task<IActionResult> GetMemberAsync([FromRoute] string id)
    {
        if (context.Members != null &&
            context.Integrations != null &&
            context.KiwoomUsers != null)
        {
            var dao = context.KiwoomUsers.AsNoTracking();

            return Ok(await (from user in context.UserLogins.AsNoTracking()
                             where id.Equals(user.UserId)
                             join i in context.Integrations.AsNoTracking()
                             on user.ProviderKey equals i.ProviderKey
                             join m in context.Members.AsNoTracking()
                             on i.SerialNumber equals m.Key
                             select new Member
                             {
                                 Key = i.SerialNumber,
                                 Longitude = m.Longitude,
                                 Latitude = m.Latitude,
                                 Accuracy = m.Accuracy,
                                 Name = dao.Single(p => string.IsNullOrEmpty(p.AccNo) == false &&
                                                        string.IsNullOrEmpty(p.Key) == false &&
                                                        p.AccNo.Equals(i.AccountNumber) &&
                                                        p.Key.Equals(i.SerialNumber))
                                           .Id
                             })
                             .ToArrayAsync() ?? Array.Empty<Member>());
        }
        return NoContent();
    }
    [ApiExplorerSettings(GroupName = "user"),
     HttpGet]
    public async Task<IActionResult> GetAsync([FromQuery] string key)
    {
        if (context.Integrations != null && context.Members != null &&

            await context.Integrations.AsNoTracking()
                                      .AnyAsync(p => key.Equals(p.SerialNumber)))
        {
            logger.LogInformation("hand over the code to { }.",
                                  key);

            var member = context.Members.AsNoTracking()
                                        .SingleOrDefault(p => key.Equals(p.Key));

            return Ok(new
            {
                api_authorization = member != null &&
                                    member.Longitude != 0 &&
                                    member.Latitude != 0 ?

                                    string.Empty :
                                    this.key
            });
        }
        logger.LogWarning("an unwelcome { } came in.",
                          key);

        return NoContent();
    }
    [ApiExplorerSettings(GroupName = "user"),
     HttpPost]
    public async Task<IActionResult> PostAsync([FromBody] Member member)
    {
        if (context.Members != null && context.Companies != null &&
            member.Latitude != 0 && member.Longitude != 0)
        {
            var dao = context.Members.AsNoTracking();
            var companies = context.Companies.AsNoTracking();

            while (dao.Any(p => p.Longitude == member.Longitude &&
                                p.Latitude == member.Latitude) ||

                   companies.Any(p => p.Latitude == member.Latitude &&
                                      p.Longitude == member.Longitude))
            {
                member.Latitude += 1e-4 * new Random().Next(-5, 7);
                member.Longitude -= 1e-4 * new Random().Next(-5, 7);
            }
            var tuple = await context.Members.FindAsync(member.Key);

            if (tuple != null)
            {
                property.SetValuesOfColumn(tuple, member);
            }
            else
                context.Members.Add(member);

            return Ok(context.SaveChanges());
        }
        return NoContent();
    }
    public CoreMemberController(ILogger<CoreMemberController> logger,
                                IConfiguration configuration,
                                IPropertyService property,
                                CoreContext context)
    {
        this.property = property;
        this.context = context;
        this.logger = logger;

        key = configuration[Properties.Resources.PINOKIO];
    }
    readonly ILogger<CoreMemberController> logger;
    readonly IPropertyService property;
    readonly CoreContext context;
    readonly string key;
}