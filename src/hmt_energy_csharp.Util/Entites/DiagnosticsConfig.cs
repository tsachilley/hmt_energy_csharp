using System.Diagnostics;

namespace hmt_energy_csharp.Entites
{
    public class DiagnosticsConfig
    {
        public DiagnosticsConfig(string serviceName, ActivitySource activitySource)
        {
            ServiceName = serviceName;
            ActivitySource = activitySource;
        }

        public string ServiceName { get; set; }
        public ActivitySource ActivitySource { get; set; }
    }
}