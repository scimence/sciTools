using SciTools.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SciTools
{
    /// <summary>
    /// EXE加壳处理逻辑：
    /// 将EXE文件加密，作为资源数据bytes,在壳EXE中解密执行；
    /// 生成壳EXE对应源码文件，编译该源码为壳EXE
    /// </summary>
    public class PackerTool
    {
        /// <summary>
        /// 对指定的应用加壳
        /// </summary>
        /// <param name="args">控制参数和待加壳的文件</param>
        public static void PackeEXE(string[] args)
        {
            // 参数信息解析
            bool isInner = true;            // 是否作为内部EXE资源
            bool outSoucreCode = false;     // 是否输出壳源码
            string WebDataUrl = "";         // 设置EXE资源载入网址

            List<string> argList = new List<string>();
            foreach (string arg in args)
            {
                if (arg.Equals("APPENDDATA")) isInner = false;
                else if (arg.Equals("WITHSOURCE")) outSoucreCode = true;
                else if (arg.Equals("WEBURL:"))
                {
                    WebDataUrl = arg.Substring("WEBURL:".Length);
                    isInner = false;
                }
                else argList.Add(arg);
            }

            // 执行packer操作
            if (argList.Count > 0) args = argList.ToArray();
            if (args != null && args.Length > 0)
            {
                foreach (string arg in args)
                {
                    PackeEXE(arg, isInner, WebDataUrl, outSoucreCode);
                }
            }
        }

        /// <summary>
        /// 对指定的应用加壳
        /// </summary>
        /// <param name="path">EXE文件路径</param>
        /// <param name="isInner">是否在壳应用内部附带资源</param>
        /// <param name="WebDataUrl">EXE文件对应资源路径</param>
        public static void PackeEXE(string path, bool isInner = true, string WebDataUrl = "", bool outSoucreCode = true)
        {
            if (!WebDataUrl.Equals("")) isInner = false;
            string Append = isInner ? "Inner" : "Outter";
            string sourceCode = GenSourceCode(path, isInner, WebDataUrl);
            if (outSoucreCode) SaveFile(sourceCode, path.Replace(".exe", "_pac" + Append + ".cs"));

            string exeFile = path.Replace(".exe", "_pac" + Append + ".exe");
            ExeTool.CompileExe(sourceCode, exeFile);
        }

        /// <summary>
        /// 从path载入数据，生成Packer源码
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private static string GenSourceCode(string path, bool isInner = true, string WebDataUrl = "")
        {
            string sourceCode = Resources.FileDeCoderTool;

            if (isInner)
            {
                string data = FileEncoderTool.getEncodeData(path);
                sourceCode = sourceCode.Replace("DATA_VALUE", data);
            }
            else
            {
                FileEncoderTool.getEncodeDataOuter(path);
                sourceCode = sourceCode.Replace("DATA_VALUE", "");
            }

            sourceCode = sourceCode.Replace("KEY_VALUE", FileEncoderTool.key);
            //sourceCode = sourceCode.Replace("NAMESPACE_VALUE", FileEncoderTool.nameSpace);

            String ENTRY_VALUE = "SciTools.FileDeCoderTool.Run(args);";
            if (!isInner)
            {
                string fileName = System.IO.Path.GetFileName(path).Replace(".exe", ".data"); ;
                if (WebDataUrl != null && !WebDataUrl.Equals("")) fileName = WebDataUrl;
                ENTRY_VALUE = "SciTools.FileDeCoderTool.RunOuter(\"" + fileName + "\", args);";
            }

            sourceCode = sourceCode.Replace("ENTRY_VALUE", ENTRY_VALUE);

            return sourceCode;
        }


        /// <summary>
        /// 保存数据data到文件处理过程，返回值为保存的文件名
        /// </summary>
        private static void SaveFile(String data, String filePath)
        {
            //string CurDir = System.AppDomain.CurrentDomain.BaseDirectory + @"SaveDir\";    //设置当前目录
            //if (!System.IO.Directory.Exists(CurDir)) System.IO.Directory.CreateDirectory(CurDir);   //该路径不存在时，在当前文件目录下创建文件夹"导出.."

            ////不存在该文件时先创建
            //String filePath = CurDir + name + ".txt";
            System.IO.StreamWriter file1 = new System.IO.StreamWriter(filePath, false);     //文件已覆盖方式添加内容

            file1.Write(data);                                                              //保存数据到文件

            file1.Close();                                                                  //关闭文件
            file1.Dispose();                                                                //释放对象
        }
    }
}
