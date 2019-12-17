using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RisaAtelier.ProxyEnablementKit
{
    internal class Util
    {

        /// <summary>
        /// 実行モード
        /// </summary>
        internal enum ExecuteMode
        {
            Enterprise, Community
        }

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

        /// <summary>
        /// Consoleでの、パスワード入力（マスク表示）
        /// </summary>
        /// <returns></returns>
        internal static string InputPassword()
        {
            var ret = string.Empty;

            while(true)
            {
                var cki = Console.ReadKey(true);
                if(cki.Key == ConsoleKey.Enter)
                {
                    Console.WriteLine();
                    break;
                }
                else if(cki.Key == ConsoleKey.Backspace)
                {
                    if (ret.Length > 0)
                    {
                        Console.Write((Char)0x08);
                        Console.Write((Char)0x7F);
                        Console.Write((Char)0x08);
                    }
                    ret = ret.Equals(string.Empty) ? string.Empty : ret.Remove(ret.Length - 1, 1);
                }
                else if((int)cki.KeyChar < 0x20)
                {
                    // 制御文字は、無視します
                }
                else
                {
                    ret += cki.KeyChar;
                    Console.Write("*");
                }
            }

            return ret;
        }
    }
}
