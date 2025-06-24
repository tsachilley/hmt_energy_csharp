using hmt_energy_csharp.Energy.Configs;
using hmt_energy_csharp.Energy.TotalIndicators;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;

namespace hmt_energy_csharp
{
    public class hmt_energy_csharpDataSeederContributor : IDataSeedContributor, ITransientDependency
    {
        private readonly ITotalIndicatorRepository _totalIndicator;

        public hmt_energy_csharpDataSeederContributor(ITotalIndicatorRepository totalIndicator)
        {
            this._totalIndicator = totalIndicator;
        }

        public async Task SeedAsync(DataSeedContext context)
        {
            if (await _totalIndicator.GetCountAsync() > 0)
            {
                return;
            }

            await _totalIndicator.InsertAsync(new TotalIndicator
            {
                ReceiveDatetime = DateTime.Now,
                DeviceNo = "张三"
            }, autoSave: true);
        }
    }
}