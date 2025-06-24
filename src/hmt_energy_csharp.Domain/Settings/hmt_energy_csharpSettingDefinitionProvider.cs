using Volo.Abp.Settings;

namespace hmt_energy_csharp.Settings;

public class hmt_energy_csharpSettingDefinitionProvider : SettingDefinitionProvider
{
    public override void Define(ISettingDefinitionContext context)
    {
        //Define your own settings here. Example:
        //context.Add(new SettingDefinition(hmt_energy_csharpSettings.MySetting1));
    }
}