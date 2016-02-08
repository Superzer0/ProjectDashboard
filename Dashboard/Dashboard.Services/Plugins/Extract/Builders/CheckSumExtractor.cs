using System;
using System.Security.Cryptography;
using Dashboard.UI.Objects.DataObjects;
using Dashboard.UI.Objects.DataObjects.Extract;
using Dashboard.UI.Objects.Services.Plugins.Extract;

namespace Dashboard.Services.Plugins.Extract.Builders
{
    internal class CheckSumExtractor : IExtractPluginInformation<CheckSumPluginInformation>
    {
        public string Name => "CheckSumExtractor";

        public CheckSumPluginInformation Extract(ProcessedPlugin processedPlugin)
        {
            using (var cryptoProvider = new SHA1CryptoServiceProvider())
            {
                return new CheckSumPluginInformation
                {
                    IssuerName = Name,
                    CheckSum = BitConverter.ToString(cryptoProvider.ComputeHash(processedPlugin.PluginZipStream))
                };
            }
        }
    }
}
