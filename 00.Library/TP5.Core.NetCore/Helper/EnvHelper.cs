using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace TP5.Core.NetCore.Helper
{
    public class EnvHelper
    {
        /// <summary>
        /// 取得本機 IP
        /// </summary>
        /// <returns>ip addresses</returns>
        public static string GetServerIP()
        {
            var strHostName = Dns.GetHostName();
            var ipEntry = Dns.GetHostEntry(strHostName);
            foreach (var ipaddress in ipEntry.AddressList
                .Where(ipaddress => ipaddress.AddressFamily.ToString().ToLower() == "internetwork"))
            {
                return ipaddress.ToString();
            }

            return string.Empty;
        }

        /// <summary>
        /// 取得本機電腦名稱
        /// </summary>
        /// <returns>電腦名稱</returns>
        public static string GetMachineName()
        {
            return Environment.MachineName;
        }

        /// <summary>
        /// 取得本機作業系統版本
        /// </summary>
        /// <returns>作業系統版本</returns>
        public static string GetOSVersion()
        {
            return Environment.OSVersion.ToString();
        }
    }
}
