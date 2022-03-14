using MyJetWallet.Sdk.Service;
using MyYamlParser;

namespace Service.PaymentProviderBridge.Test3.Settings
{
    public class SettingsModel
    {
        [YamlProperty("PaymentProviderBridge.Test3.SeqServiceUrl")]
        public string SeqServiceUrl { get; set; }

        [YamlProperty("PaymentProviderBridge.Test3.ZipkinUrl")]
        public string ZipkinUrl { get; set; }

        [YamlProperty("PaymentProviderBridge.Test3.ElkLogs")]
        public LogElkSettings ElkLogs { get; set; }

        [YamlProperty("PaymentProviderBridge.Test3.ServiceUrl")]
        public string ServiceUrl { get; set; }

        [YamlProperty("PaymentProviderBridge.Test3.OkUrl")]
        public string OkUrl { get; set; }

        [YamlProperty("PaymentProviderBridge.Test3.FailUrl")]
        public string FailUrl { get; set; }

        [YamlProperty("PaymentProviderBridge.Test3.TryCount")]
        public int TryCount { get; set; }
    }
}
