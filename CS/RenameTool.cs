using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tools
{
    /// <summary>
    /// 文件重命名修改
    /// </summary>
    public class RenameTool
    {
        /// <summary>
        /// 重命名修改，可指定仅修改指定后缀的文件
        /// </summary>
        public static void ReName(String[] filePaths, String[] Endwith = null)
        {
            foreach (String filePath in filePaths)
            {
                ReName(filePath, Endwith);
            }
        }

        /// <summary>
        /// 重命名修改，可指定仅修改指定后缀的文件
        /// </summary>
        public static void ReName(String filePath, String[] Endwith = null)
        {
            // cmd重命名
            if (File.Exists(filePath))
            {
                if (!isEndWith(filePath, Endwith)) return;

                String newName = Path.GetFileName(filePath) + ".@data";
                Run("rename " + AddQutation(filePath) + " " + newName);

                if (File.Exists(filePath + ".@data") && !File.Exists(filePath))
                {
                    File.Move(filePath + ".@data", filePath);
                }
            }
        }

        /// <summary>
        /// 判断Str是否为以Endwith中的某一项为结尾, Endwith为空时返回true
        /// </summary>
        public static bool isEndWith(String Str, String[] Endwith = null)
        {
            if (Endwith != null && Endwith.Length > 0)
            {
                foreach (string ending in Endwith)
                {
                    if (Str.ToLower().EndsWith(ending))
                    {
                        return true;
                    }
                }
                return false;
            }
            else return true;
        }

        /// <summary>
        /// 添加引号
        /// </summary>
        private static string AddQutation(String filePath)
        {
            return "\"" + filePath + "\"";
        }

        /// <summary>
        /// 执行CMD命令
        /// </summary>
        private static string Run(string cmd)
        {
            Process P = newProcess("cmd.exe");
            P.StandardInput.WriteLine(cmd);
            P.StandardInput.WriteLine("exit");
            string outStr = P.StandardOutput.ReadToEnd();
            P.Close();
            return outStr;
        }

        /// <summary>
        /// 以后台进程的形式执行应用程序（d:\*.exe）
        /// </summary>
        private static Process newProcess(String exe)
        {
            Process P = new Process();
            P.StartInfo.CreateNoWindow = true;
            P.StartInfo.FileName = exe;
            P.StartInfo.UseShellExecute = false;
            P.StartInfo.RedirectStandardError = true;
            P.StartInfo.RedirectStandardInput = true;
            P.StartInfo.RedirectStandardOutput = true;
            //P.StartInfo.WorkingDirectory = @"C:\windows\system32";
            P.Start();
            return P;
        }

    }

}
