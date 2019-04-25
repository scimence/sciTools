using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tools
{
    /// <summary>
    /// 文件读写
    /// </summary>
    public class FileTool
    {
        /// <summary>
        /// 从一个目录将其内容复制到另一目录
        /// </summary>
        public static void CopyFolderTo(string dirSource, string dirTarget, bool overwirite)
        {
            // 先获取Source目录下，当前的文件目录信息。在复制前先读取文件和目录信息，避免父目录向子目录复制时出现的无限复制循环，而只执行一次复制
            DirectoryInfo directoryInfo = new DirectoryInfo(dirSource);
            FileInfo[] files = directoryInfo.GetFiles();
            DirectoryInfo[] directoryInfoArray = directoryInfo.GetDirectories();

            //检查目标路径是否存在目的目录
            if (!Directory.Exists(dirTarget)) Directory.CreateDirectory(dirTarget);

            //先来复制所有文件  
            foreach (FileInfo file in files)
            {
                string fileSource = Path.Combine(file.DirectoryName, file.Name);
                string fileTarget = Path.Combine(dirTarget, file.Name);
                file.CopyTo(fileTarget, overwirite);
            }

            //最后复制目录
            foreach (DirectoryInfo dir in directoryInfoArray)
            {
                CopyFolderTo(Path.Combine(dirSource, dir.Name), Path.Combine(dirTarget, dir.Name), overwirite);
            }
        }

        /// <summary>
        /// 重命名目录
        /// </summary>
        public static void reName(String dir, String newName)
        {
            if (Directory.Exists(newName)) Directory.Delete(newName, true); // 若目标目录存在则删除
            Directory.Move(dir, newName);                                   // 重命名当前目录
        }

        /// <summary>
        /// 修改文件file中的 sourceValue -> targetValue
        /// </summary>
        public static void replace(String file, String sourceValue, String targetValue)
        {
            String data = fileToString(file);
            if (data.Contains(sourceValue))
            {
                data = data.Replace(sourceValue, targetValue);
                SaveProcess(data, file, null, true);
            }
        }

        /// <summary>
        /// 获取文件中的数据,自动判定编码格式
        /// </summary>
        public static string fileToString(String filePath)
        {
            string str = "";

            //获取文件内容
            if (File.Exists(filePath))
            {
                StreamReader file1;

                Encoding encoding = GetFileEncodeType(filePath);    // 获取原有编码
                file1 = new StreamReader(filePath, encoding);       // 读取文件中的数据

                str = file1.ReadToEnd();                            // 读取文件中的全部数据

                file1.Close();
                file1.Dispose();
                 
            }
            return str;
        }

        /// <summary>
        /// 保存数据data到文件处理过程，返回值为保存的文件名
        /// </summary>
        public static String SaveProcess(String data, String filePath, Encoding encoding = null, bool replace = false)
        {
            //if (encoding == null) encoding = GetFileEncodeType(filePath);

            //不存在该文件时先创建
            StreamWriter file1 = new StreamWriter(filePath, !replace/*, encoding*/);            //文件已覆盖方式添加内容

            file1.Write(data);                                                              //保存数据到文件

            file1.Close();                                                                  //关闭文件
            file1.Dispose();                                                                //释放对象

            return filePath;
        }

        /// <summary>
        /// 获取文件的编码格式
        /// </summary>
        public static Encoding GetFileEncodeType(string filename)
        {
            Encoding encod = Encoding.UTF8;

            FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read);
            BinaryReader br = new BinaryReader(fs);

            try
            {
                if (filename.ToLower().EndsWith(".xml")) encod = Encoding.UTF8; // xml文件默认使用utf-8编码

                Byte[] buffer = br.ReadBytes(2);
                if (buffer[0] >= 0xEF)
                {
                    if (buffer[0] == 0xEF && buffer[1] == 0xBB)
                    {
                        encod = Encoding.UTF8;
                    }
                    else if (buffer[0] == 0xFE && buffer[1] == 0xFF)
                    {
                        encod = Encoding.BigEndianUnicode;
                    }
                    else if (buffer[0] == 0xFF && buffer[1] == 0xFE)
                    {
                        encod = Encoding.Unicode;
                    }
                }
            }
            catch (Exception) { }

            br.Close();
            fs.Close();

            return encod;
        }

    }

}
