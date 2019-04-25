using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Tools
{
    /// <summary>
    /// 文件重编码
    /// </summary>
    public class RecodeTool
    {
        /// <summary>
        /// 文件重编码示例入口
        /// </summary>
        public static void Main(String[] args)
        {
            if (args.Length == 0)
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new ReCode_Form());
            }
            else
            {
                String[] files = DragDropTool.GetSubFiles(args);            // 载入文件信息

                String[] Endwith = new String[] { ".java", ".cs", ".txt", ".xml" };
                RenameTool.ReName(files, Endwith);                           // 重命名
                RecodeTool.ReCode(files, Encoding.UTF8, Endwith);            // 重编码所有文件
            }
        }

        /// <summary>
        /// 文件重编码。根据文件的编码格式读取文件内容，输出为指定的编码格式，可指定仅修改指定后缀的文件
        /// </summary>
        public static void ReCode(String[] filePaths, Encoding encoding = null, String[] Endwith = null)
        {
            foreach (String filePath in filePaths)
            {
                RecodeTool.ReCode(filePath, encoding, Endwith);
            }
        }

        /// <summary>
        /// 文件重编码。根据文件的编码格式读取文件内容，输出为指定的编码格式，可指定仅修改指定后缀的文件
        /// </summary>
        public static void ReCode(String filePath, Encoding encoding = null, String[] Endwith = null)
        {
            if (File.Exists(filePath))
            {
                try
                {
                    if (!RenameTool.isEndWith(filePath, Endwith)) return;

                    String data = FileTool.fileToString(filePath);
                    FileTool.SaveProcess(data, filePath, encoding, true);
                }
                catch (Exception ex) { }
            }
        }

        ///// <summary>
        ///// 获取文件中的数据,自动判定编码格式
        ///// </summary>
        //public static string fileToString(String filePath)
        //{
        //    string str = "";

        //    //获取文件内容
        //    if (File.Exists(filePath))
        //    {
        //        bool defaultEncoding = filePath.EndsWith(".txt");

        //        StreamReader file1;

        //        Encoding encoding = GetFileEncodeType(filePath);
        //        file1 = new StreamReader(filePath, encoding);      //读取文件中的数据

        //        str = file1.ReadToEnd();                           //读取文件中的全部数据

        //        file1.Close();
        //        file1.Dispose();
        //    }
        //    return str;
        //}

        ///// <summary>
        ///// 保存数据data到文件处理过程，返回值为保存的文件名
        ///// </summary>
        //public static String SaveProcess(String data, String filePath, Encoding encoding = null, bool replace = false)
        //{
        //    if (encoding == null) encoding = GetFileEncodeType(filePath);

        //    //不存在该文件时先创建
        //    StreamWriter file1 = new StreamWriter(filePath, !replace, encoding);            //文件已覆盖方式添加内容

        //    file1.Write(data);                                                              //保存数据到文件

        //    file1.Close();                                                                  //关闭文件
        //    file1.Dispose();                                                                //释放对象

        //    return filePath;
        //}

        ///// <summary>
        ///// 获取文件的编码格式
        ///// </summary>
        //public static Encoding GetFileEncodeType(string filename)
        //{
        //    Encoding encod = Encoding.Default;

        //    FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read);
        //    BinaryReader br = new BinaryReader(fs);

        //    try
        //    {

        //        Byte[] buffer = br.ReadBytes(2);
        //        if (buffer[0] >= 0xEF)
        //        {
        //            if (buffer[0] == 0xEF && buffer[1] == 0xBB)
        //            {
        //                encod = Encoding.UTF8;
        //            }
        //            else if (buffer[0] == 0xFE && buffer[1] == 0xFF)
        //            {
        //                encod = Encoding.BigEndianUnicode;
        //            }
        //            else if (buffer[0] == 0xFF && buffer[1] == 0xFE)
        //            {
        //                encod = Encoding.Unicode;
        //            }
        //        }
        //    }
        //    catch (Exception) { }

        //    br.Close();
        //    fs.Close();

        //    return encod;
        //}

    }

}
