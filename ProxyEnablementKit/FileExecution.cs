using System;
using System.IO;
using System.Xml;

namespace RisaAtelier.ProxyEnablementKit
{
    class FileExecution
    {

        internal static void CopyBackups(string workDir, string UiPathDir, Util.ExecuteMode execMode)
        {
            switch(execMode)
            {
                case Util.ExecuteMode.Enterprise:
                    {
                        var bkupDir = Path.Combine(workDir, "backup");
                        var upCopyDir = Path.Combine(bkupDir, "UiPath");

                        WriteDirectoryList(workDir, "[UiPath] " + UiPathDir);
                        Directory.CreateDirectory(upCopyDir);

                        FileCopy(UiPathDir, upCopyDir, "NuGet.Config");
                        FileCopy(UiPathDir, upCopyDir, "UiPath.Executor.exe.config");
                        FileCopy(UiPathDir, upCopyDir, "UiPath.Service.Host.exe.config");
                        FileCopy(UiPathDir, upCopyDir, "UiPath.Agent.exe.config");
                    }
                    break;
                case Util.ExecuteMode.Community:
                    {
                        var bkupDir = Path.Combine(workDir, "backup");
                        var upCopyDir = Path.Combine(bkupDir, "UiPath");

                        WriteDirectoryList(workDir, "[UiPath] " + UiPathDir);
                        Directory.CreateDirectory(upCopyDir);

                        FileCopy(UiPathDir, upCopyDir, "NuGet.Config");
                        FileCopy(UiPathDir, upCopyDir, "UiPath.Executor.exe.config");
                        FileCopy(UiPathDir, upCopyDir, "UiPath.Service.Host.exe.config");
                        FileCopy(UiPathDir, upCopyDir, "UiPath.Service.UserHost.exe.config");
                        FileCopy(UiPathDir, upCopyDir, "UiPath.Agent.exe.config");

                    }
                    break;
                default:
                    throw new InvalidOperationException();
            }

        }

        /// <summary>
        /// 編集する、ファイルを作成します
        /// </summary>
        /// <param name="workDir"></param>
        /// <param name="execMode"></param>
        internal static void CreateNewFile(string workDir, Util.ExecuteMode execMode)
        {
            CopyDirectory(Path.Combine(workDir, "backup"), Path.Combine(workDir, "output"));
        }

        /// <summary>
        /// 各ファイルの、編集処理を実行します
        /// </summary>
        /// <param name="workDir"></param>
        /// <param name="execMode"></param>
        internal static void EditFile(string workDir, Util.ExecuteMode execMode, bool hasCredential, string proxyUrl)
        {
            switch(execMode)
            {
                case Util.ExecuteMode.Enterprise:
                    SetDefaultProxy(Path.Combine(workDir, "output", "UiPath", "UiPath.Executor.exe.config"), "ProxyModule", hasCredential, proxyUrl);
                    SetDefaultProxy(Path.Combine(workDir, "output", "UiPath", "UiPath.Service.Host.exe.config"), "ProxyModule", hasCredential, proxyUrl);
                    SetDefaultProxy(Path.Combine(workDir, "output", "UiPath", "UiPath.Agent.exe.config"), "ProxyModule", hasCredential, proxyUrl);

                    break;
                case Util.ExecuteMode.Community:
                    SetDefaultProxy(Path.Combine(workDir, "output", "UiPath", "UiPath.Executor.exe.config"), "ProxyModule", hasCredential, proxyUrl);
                    SetDefaultProxy(Path.Combine(workDir, "output", "UiPath", "UiPath.Service.Host.exe.config"), "ProxyModule", hasCredential, proxyUrl);
                    SetDefaultProxy(Path.Combine(workDir, "output", "UiPath", "UiPath.Service.UserHost.exe.config"), "ProxyModule", hasCredential, proxyUrl);
                    SetDefaultProxy(Path.Combine(workDir, "output", "UiPath", "UiPath.Agent.exe.config"), "ProxyModule", hasCredential, proxyUrl);

                    break;
                default:
                    throw new InvalidOperationException();
            }

        }

        /// <summary>
        /// 再帰でフォルダごとコピーします
        /// </summary>
        /// <param name="src"></param>
        /// <param name="dest"></param>
        private static void CopyDirectory(string src, string dest)
        {
            foreach(var d in Directory.GetDirectories(src))
            {
                Directory.CreateDirectory(Path.Combine(dest, Path.GetFileName(d)));
                CopyDirectory(d, Path.Combine(dest, Path.GetFileName(d)));
            }

            foreach(var f in Directory.GetFiles(src))
            {
                File.Copy(f, Path.Combine(dest, Path.GetFileName(f)));
            }
        }

        /// <summary>
        /// コピー先に関する、情報を、記録します
        /// </summary>
        /// <param name="workDir"></param>
        /// <param name="expression"></param>
        private static void WriteDirectoryList(string workDir, string expression)
        {
            using(var sw = new StreamWriter(Path.Combine(workDir, "DirectoryList.txt"), true, System.Text.Encoding.UTF8))
            {
                sw.WriteLine(expression);
            }
        }

        /// <summary>
        /// ファイルの、単純コピーです
        /// </summary>
        /// <param name="fromDir"></param>
        /// <param name="destDir"></param>
        /// <param name="fileName"></param>
        private static void FileCopy(string fromDir, string destDir, string fileName)
        {
            File.Copy(Path.Combine(fromDir, fileName), Path.Combine(destDir, fileName));
        }

        /// <summary>
        /// ～.exe.config に、system.net要素を追加し、プロキシ設定を書き込みます
        /// </summary>
        /// <param name="targetFile"></param>
        /// <param name="moduleName"></param>
        /// <param name="hasCredential"></param>
        /// <param name="proxyUrl"></param>
        private static void SetDefaultProxy(string targetFile, string moduleName, bool hasCredential, string proxyUrl)
        {
            var xDoc = new XmlDocument();
            xDoc.Load(targetFile);

            // configuration/system.net 要素
            XmlElement sysNet = null;
            if(xDoc.SelectNodes(@"configuration/system.net").Count == 0)
            {
                sysNet = xDoc.CreateElement("system.net");
                xDoc.SelectSingleNode("configuration").AppendChild(sysNet);
            }
            else
            {
                sysNet = (XmlElement)xDoc.SelectSingleNode(@"configuration/system.net");
            }

            // defaultProxy 要素
            XmlElement defProxy = null;
            if (sysNet.SelectNodes("defaultProxy").Count == 0)
            {
                defProxy = xDoc.CreateElement("defaultProxy");
                sysNet.AppendChild(defProxy);
            }
            else
            {
                defProxy = (XmlElement)sysNet.SelectSingleNode(@"./defaultProxy");
            }

            if(hasCredential)
            {
                defProxy.SetAttribute("useDefaultCredentials", hasCredential ? "true" : "false");
                defProxy.SetAttribute("enabled", "true");

                // module 要素
                XmlElement module = null;
                if (defProxy.SelectNodes("module").Count == 0)
                {
                    module = xDoc.CreateElement("module");
                    defProxy.AppendChild(module);
                }
                else
                {
                    module = (XmlElement)defProxy.SelectSingleNode(@"./module");
                }

                module.SetAttribute("module", "ProxyEnablementKit.ProxyEnablementModule, " + moduleName);

                // proxyノードがあったら、消しておきます
                XmlNodeList nodeList = defProxy.SelectNodes("proxy");
                foreach(XmlNode node in nodeList)
                {
                    defProxy.RemoveChild(node);
                }
            }
            else
            {
                // proxy要素
                XmlElement proxy = null;
                if(defProxy.SelectNodes("proxy").Count == 0)
                {
                    proxy = xDoc.CreateElement("proxy");
                    defProxy.AppendChild(proxy);
                }
                else
                {
                    proxy = (XmlElement)defProxy.SelectSingleNode(@"./proxy");
                }

                proxy.SetAttribute("proxyaddress", proxyUrl);
                proxy.SetAttribute("bypassonlocal", "true");

                // moduleノードがあったら、消しておきます
                XmlNodeList nodeList = defProxy.SelectNodes("module");
                foreach (XmlNode node in nodeList)
                {
                    defProxy.RemoveChild(node);
                }

            }

            // 上書き保存します
            xDoc.Save(targetFile);
        }

        /// <summary>
        /// NuGetの設定をします
        /// </summary>
        /// <param name="workDir"></param>
        /// <param name="proxyUrl"></param>
        /// <param name="proxyUser"></param>
        /// <param name="proxyPass"></param>
        internal static void NuGetSetting(string workDir, string proxyUrl, string proxyUser, string proxyPass)
        {
            // NuGet.exe CLIを、コピーします
            var nugetExe = Path.Combine(Util.AppDirectory, "nuget.exe");

            // ここが少し、難物です
            // NuGet.exeは、AppData/Roamingに、書き出すので
            // そこの内容を、移動する必要が、あります

            // まず、AppDataの、nuget.configを、バックアップします
            var nugetRoaming = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "NuGet", "NuGet.Config");

            // 存在しないケース対応
            var hasNugetRoaming = File.Exists(nugetRoaming);

            var nugetRoamingCopy = string.Empty;

            if(hasNugetRoaming)
            {
                nugetRoamingCopy = nugetRoaming + string.Format(".backup.{0}", DateTime.Now.ToString("yyyyMMdd_HHmmss"));
                File.Copy(nugetRoaming, nugetRoamingCopy);
            }

            // NuGet CLIで、設定を、行います
            {
                {
                    var proc = System.Diagnostics.Process.Start(nugetExe, "config -set http_proxy=" + proxyUrl);
                    while (!proc.HasExited) System.Threading.Thread.Sleep(0);
                }

                if (!proxyUser.Equals(string.Empty))
                {
                    {
                        var proc = System.Diagnostics.Process.Start(nugetExe, "config -set http_proxy.user=" + proxyUser);
                        while (!proc.HasExited) System.Threading.Thread.Sleep(0);
                    }

                    {
                        var proc = System.Diagnostics.Process.Start(nugetExe, "config -set http_proxy.password=" + proxyPass);
                        while (!proc.HasExited) System.Threading.Thread.Sleep(0);
                    }
                }
            }

            // 書き換えで、作成された、要素を取得します
            var editedNuConf = new XmlDocument();
            editedNuConf.Load(nugetRoaming);

            var fragment = editedNuConf.CreateDocumentFragment();
            foreach(XmlElement node in editedNuConf.SelectNodes(@"configuration/config/add[contains(@key, ""http_proxy"")]"))
            {
                fragment.AppendChild(node.CloneNode(true));
            }

            var targetNuConf = new XmlDocument();
            var targetNuConfFile = Path.Combine(workDir, "output", "UiPath", "NuGet.Config");
            targetNuConf.Load(targetNuConfFile);

            // キーを移動します
            XmlElement configNode = null;
            if(targetNuConf.SelectNodes(@"configuration/config").Count == 0)
            {
                configNode = targetNuConf.CreateElement("config");
                targetNuConf.DocumentElement.AppendChild(configNode);
            }
            else
            {
                configNode = (XmlElement)targetNuConf.SelectSingleNode(@"configuration/config");
            }

            foreach(XmlElement el in fragment.ChildNodes)
            {
                // Keyが重複するときは、古い方を、削除します
                if(configNode.SelectNodes(string.Format("add[@key='']", el.GetAttribute("key"))).Count != 0)
                {
                    configNode.RemoveChild(configNode.SelectNodes(string.Format("add[@key='']", el.GetAttribute("key")))[0]);
                }

                var newEl = targetNuConf.CreateElement("add");
                foreach(XmlAttribute attr in el.Attributes)
                {
                    newEl.SetAttribute(attr.Name, attr.Value);
                }
                configNode.AppendChild(newEl);
            }

            // 上書きで、更新します
            targetNuConf.Save(targetNuConfFile);

            File.Delete(nugetRoaming);

            // バックアップから、Roamingを、復元します
            if (hasNugetRoaming)
            {
                File.Move(nugetRoamingCopy, nugetRoaming);
            }
        }

    }
}
