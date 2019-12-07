using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace RisaAtelier.ProxyEnablementKit
{
    class Program
    {


        /// <summary>
        /// Entry Pointです
        /// バッチ的な処理となるので、あまり、綺麗ではないです・・・
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            // 前提条件として、 nuget.exeが、配置されていることを、確認します
            if(!CheckNugetExe())
            {
                Console.WriteLine(Properties.Resources.ERROR_NUGET_NOT_FOUND);
                return;
            }

            #region モード (Enterprise / Community) を、選択します
            {
                var execMode = Util.ExecuteMode.Enterprise;
                var executePath = string.Empty;

                Console.WriteLine(Properties.Resources.MSG_UIPATH_TYPE);
                var choice = Console.ReadLine();
                if (choice.Trim().Equals("0"))
                {
                    execMode = Util.ExecuteMode.Enterprise;

                    // Default設定だと、 Profram Files (x86) に、インストールされています
                    var enterpriseDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "UiPath", "Studio");
                    if (Directory.Exists(enterpriseDir) && File.Exists(Path.Combine(enterpriseDir, "UiPath.Executor.exe")))
                    {
                        executePath = enterpriseDir;
                    }
                }
                else if (choice.Trim().Equals("1"))
                {
                    execMode = Util.ExecuteMode.Community;

                    // こちらは、設定の変更はできないので、存在しなければ、インストールされていないと、思いますが・・・
                    var communityDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "UiPath");
                    if (Directory.Exists(communityDir) && File.Exists(Path.Combine(communityDir, "UiPath.Studio.exe")))
                    {
                        executePath = communityDir;
                    }
                }
                else
                {
                    return;
                }

                // 実行ディレクトリが、見つけられなければ、入力を求めます
                if(executePath.Equals(string.Empty))
                {
                    Console.WriteLine(Properties.Resources.MSG_INPUT_TARGET_DIR);
                    executePath = Console.ReadLine();

                    if(executePath.Trim().Equals(string.Empty) || !Directory.Exists(executePath))
                    {
                        throw new DirectoryNotFoundException();
                    }
                }
            }
            #endregion

            // プロキシサーバーの、URLを入力します


            var workDir = Path.Combine(Util.AppDirectory, String.Format("ProxyKitOutput_{0}", DateTime.Now.ToString("yyyyMMdd_HHmmss")));
            Directory.CreateDirectory(workDir);

            ProxyAddonBuilder.CreateDll(workDir, @"http://proxy.hogehoge.com:80", "scott", "tiger");

#if DEBUG
            Console.ReadLine();
#endif
            return;
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
