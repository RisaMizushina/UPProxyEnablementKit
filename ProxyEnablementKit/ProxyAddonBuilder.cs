using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RisaAtelier.ProxyEnablementKit
{
    internal class ProxyAddonBuilder
    {
            /// <summary>
            /// Proxy moduleの、ソースコードです
            /// </summary>
            private const string src = @"using System;
namespace ProxyEnablementKit
{
    public class ProxyEnablementModule : System.Net.IWebProxy
    {
        public System.Net.ICredentials Credentials
        {
            get 
            {
                return new System.Net.NetworkCredential(@""%%%_PROXY_USER_%%%""
                                                        , System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(@""%%%_PROXY_PASSWORD_%%%"")));
            }

            set { }
        }

        public Uri GetProxy(Uri dest)
        {
            return new Uri(@""%%%_PROXY_URL_%%%"");
        }

        public bool IsBypassed(Uri dest)
        {
            return false;
        }
    }
}
";

        private static string DotNetDir
        {
            get
            {
                var ret = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "Microsoft.NET", "Framework", "v4.0.30319");
                if (!Directory.Exists(ret)) throw new ApplicationException(Properties.Resources.ERROR_DOTNET_NOT_FOUND);
                return ret;
            }
        }
        
        /// <summary>
        /// DLLファイルを、作成します
        /// </summary>
        /// <param name="tempDir"></param>
        /// <param name="proxyUrl"></param>
        /// <param name="proxyUser"></param>
        /// <param name="proxyPass"></param>
        internal static string CreateDll(string tempDir, string proxyUrl, string proxyUser, string proxyPass)
        {
            var srcFile = CreateSource(tempDir, proxyUrl, proxyUser, proxyPass);

            var pi = new System.Diagnostics.ProcessStartInfo(Path.Combine(DotNetDir, "csc.exe"));
            pi.WorkingDirectory = tempDir;
            pi.Arguments = @"/out:ProxyModule.dll /target:library ProxyModule.cs";
            pi.CreateNoWindow = true;

            var proc = System.Diagnostics.Process.Start(pi);

            while(!proc.HasExited)
            {
                System.Threading.Thread.Sleep(0);
            }

            return Path.Combine(tempDir, "ProxyModule.dll");

        }

        /// <summary>
        /// ソースコードを、出力します
        /// </summary>
        /// <param name="tempDir"></param>
        /// <param name="proxyUrl"></param>
        /// <param name="proxyUser"></param>
        /// <param name="proxyPass"></param>
        /// <returns></returns>
        private static string CreateSource(string tempDir, string proxyUrl, string proxyUser, string proxyPass)
        {
            var srcFile = System.IO.Path.Combine(tempDir, "ProxyModule.cs");

            var srcCode = src.Replace("%%%_PROXY_URL_%%%", proxyUrl).Replace("%%%_PROXY_USER_%%%", proxyUser).Replace("%%%_PROXY_PASSWORD_%%%", Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(proxyPass)));

            using(var sw = new System.IO.StreamWriter(srcFile, false, System.Text.Encoding.UTF8))
            {
                sw.WriteLine(srcCode);
            }

            return srcFile;
        }
        

    }



}
