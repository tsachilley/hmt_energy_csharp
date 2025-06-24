using hmt_energy_csharp.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;

namespace hmt_energy_csharp.Permissions;

public class hmt_energy_csharpPermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var myGroup = context.AddGroup(hmt_energy_csharpPermissions.GroupName);
        //Define your own permissions here. Example:
        //myGroup.AddPermission(hmt_energy_csharpPermissions.MyPermission1, L("Permission:MyPermission1"));
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<hmt_energy_csharpResource>(name);
    }
}