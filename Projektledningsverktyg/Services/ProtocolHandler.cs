using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projektledningsverktyg.Services
{
    public static class ProtocolHandler
    {
        public static void RegisterProtocol()
        {
            string protocolName = "projektverktyg";
            string applicationPath = System.Reflection.Assembly.GetExecutingAssembly().Location;

            using (var key = Registry.CurrentUser.CreateSubKey($@"Software\Classes\{protocolName}"))
            {
                key.SetValue("", $"URL:{protocolName} Protocol");
                key.SetValue("URL Protocol", "");

                using (var commandKey = key.CreateSubKey(@"shell\open\command"))
                {
                    commandKey.SetValue("", $"\"{applicationPath}\" \"%1\"");
                }
            }
        }
    }
}
