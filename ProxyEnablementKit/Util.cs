using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProxyEnablementKit
{
    internal class Util
    {
        /// <summary>
        /// アプリケーションのディレクトリ
        /// </summary>
        internal static string AppDirectory
        {
            get
            {
                return System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
            }
        }
    }
}
