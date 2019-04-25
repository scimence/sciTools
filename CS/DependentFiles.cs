using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tools
{
    /// <summary>
    /// 属性资源文件导出
    /// </summary>
    public class DependentFiles
    {
        /// <summary>
        /// 获取当前运行路径
        /// </summary>
        public static string curDir()
        {
            return AppDomain.CurrentDomain.BaseDirectory;
        }

        /// <summary>
        /// 获取当前运行Exe的路径
        /// </summary>
        public static string curExecutablePath()
        {
            return System.Windows.Forms.Application.ExecutablePath;
        }

        /// <summary>
        /// 检测目录是否存在，若不存在则创建
        /// </summary>
        public static void checkDir(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }


        /// <summary>
        /// 保存Byte数组为文件
        /// </summary>
        public static void SaveFile(Byte[] array, string path, bool repalce = false)
        {
            if (repalce && System.IO.File.Exists(path)) System.IO.File.Delete(path);    // 若目标文件存在，则替换
            if (!System.IO.File.Exists(path))
            {
                // 创建输出路径
                String dir = System.IO.Path.GetDirectoryName(path);
                checkDir(dir);

                // 创建输出流
                System.IO.FileStream fs = new System.IO.FileStream(path, System.IO.FileMode.Create);

                //将byte数组写入文件中
                fs.Write(array, 0, array.Length);
                fs.Close();
            }
        }

        ///// <summary>
        ///// 生成计费模版文件
        ///// </summary>
        //public static String CheckTemplate()
        //{
        //    SaveFile(Resources.template, curDir() + "ChargeModule.zip");
        //    return curDir() + "ChargeModule.zip";
        //}

        ///// <summary>
        ///// 生成计费模版文件
        ///// </summary>
        //public static void LoadFiles()
        //{
        //    SaveFile(Resources.ICSharpCode_SharpZipLib, curDir() + "ICSharpCode.SharpZipLib.dll");
        //}

    }
}
