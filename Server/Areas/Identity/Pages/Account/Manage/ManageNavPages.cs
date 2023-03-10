using Microsoft.AspNetCore.Mvc.Rendering;

namespace ShareInvest.Server.Areas.Identity.Pages.Account.Manage
{
    public static class ManageNavPages
    {
        static string? PageNavClass(ViewContext viewContext, string page)
        {
            var activePage = viewContext.ViewData["ActivePage"] as string ?? Path.GetFileNameWithoutExtension(viewContext.ActionDescriptor.DisplayName);

            return string.Equals(activePage, page, StringComparison.OrdinalIgnoreCase) ? "active" : null;
        }
        public static string Index => "Index";
        public static string ChangePassword => "ChangePassword";
        public static string ExternalLogins => "ExternalLogins";
        public static string PersonalData => "PersonalData";
        public static string TwoFactorAuthentication => "TwoFactorAuthentication";
        public static string GrantAdminPrivileges => nameof(GrantAdminPrivileges);
        public static string LinkTheSecuritiesId => nameof(LinkTheSecuritiesId);
        public static string Logout => nameof(Logout);
        public static string? IndexNavClass(ViewContext viewContext) => PageNavClass(viewContext, Index);
        public static string? ChangePasswordNavClass(ViewContext viewContext) => PageNavClass(viewContext, ChangePassword);
        public static string? ExternalLoginsNavClass(ViewContext viewContext) => PageNavClass(viewContext, ExternalLogins);
        public static string? PersonalDataNavClass(ViewContext viewContext) => PageNavClass(viewContext, PersonalData);
        public static string? LogoutNavClass(ViewContext viewContext) => PageNavClass(viewContext, Logout);
        public static string? TwoFactorAuthenticationNavClass(ViewContext viewContext) => PageNavClass(viewContext, TwoFactorAuthentication);
        public static string? LinkTheSecuritiesIdNavClass(ViewContext context) => PageNavClass(context, LinkTheSecuritiesId);
        public static string? GrantAdminPrivilegesNavClass(ViewContext context) => PageNavClass(context, GrantAdminPrivileges);
    }
}