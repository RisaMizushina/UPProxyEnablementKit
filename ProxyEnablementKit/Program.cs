using System;
using System.IO;
using System.Xml;

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
                    var targetDir = string.Empty;

                    // 処理対象の、ディレクトリを、探します
                    foreach(var subDir in Directory.GetDirectories(communityDir))
                    {
                        // app- ではじまるのが、UiPathのディレクトリ
                        if(Path.GetFileName(subDir).StartsWith("app-"))
                        {
                            targetDir = targetDir.Equals(string.Empty) ? subDir : (targetDir.CompareTo(subDir) > 0 ? targetDir : subDir);
                        }
                    }
                    executePath = targetDir;
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

            Console.WriteLine(Properties.Resources.LABEL_TARGET_DIR + executePath);
            #endregion

            #region プロキシサーバー設定
            
                // プロキシサーバーの、URLを入力します
            Console.WriteLine(Properties.Resources.MSG_INPUT_PROXY_URL);
            var proxyUrl = Console.ReadLine().Trim();
            if (proxyUrl.Equals(string.Empty)) return;
            
            // プロキシサーバーの、ユーザーを入力します
            Console.WriteLine(Properties.Resources.MSG_INPUT_PROXY_USER);
            var proxyUser = Console.ReadLine().Trim();

            var proxyPass = string.Empty;
            if (!proxyUser.Equals(string.Empty))
            {
                Console.WriteLine(Properties.Resources.MSG_INPUT_PROXY_PASS);
                proxyPass = Util.InputPassword();

                Console.WriteLine(Properties.Resources.MSG_INPUT_PROXY_PASS_CONFIRM);
                if (Util.InputPassword() != proxyPass)
                {
                    Console.WriteLine(Properties.Resources.ERROR_PASSWORD_NOMATCH);
                    return;
                }
             }
            
            #endregion

            var workDir = Path.Combine(Util.AppDirectory, String.Format("ProxyKitOutput_{0}", DateTime.Now.ToString("yyyyMMdd_HHmmss")));
            Directory.CreateDirectory(workDir);

            // DLLを、作成します
            var dllFile = string.Empty;
            if(!proxyUser.Equals(string.Empty))
            {
                dllFile = ProxyAddonBuilder.CreateDll(workDir, proxyUrl, proxyUser, proxyPass);
                if (!File.Exists(dllFile))
                {
                    Console.WriteLine(Properties.Resources.ERROR_BUILD_FAILUE);
                    return;
                }
            }

            // Backupを、作成します
            FileExecution.CopyBackups(workDir, executePath, execMode);

            // 編集用の、ファイルのコピーを、作成します
            FileExecution.CreateNewFile(workDir, execMode);

            // 設定を、変更します
            // DLLを作成していたときは、移動も行います
            if(!dllFile.Equals(string.Empty))
            {
                File.Move(dllFile, Path.Combine(workDir, "output", "UiPath", Path.GetFileName(dllFile)));
            }
            FileExecution.EditFile(workDir, execMode, !proxyUser.Equals(string.Empty), proxyUrl);

            // NuGetの設定です
            FileExecution.NuGetSetting(workDir, proxyUrl, proxyUser, proxyPass);

            Console.WriteLine(Properties.Resources.MSG_EXEC_COMPLETE);
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
