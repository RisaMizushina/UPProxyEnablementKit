using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace ProxyEnablementKit
{
    class Program
    {
        static void Main(string[] args)
        {
            // 前提条件として、 nuget.exeの、存在を確認します
            if(!CheckNugetExe())
            {
                Console.WriteLine("Nuget.exeが、見つかりませんでした。");
                return;
            }

            
        }

        /// <summary>
        /// Nuget.exeの、存在確認
        /// </summary>
        /// <returns></returns>
        static bool CheckNugetExe()
        {
            return File.Exists(Path.Combine(Util.AppDirectory, "nuget.exe"));
        }
    }
}
