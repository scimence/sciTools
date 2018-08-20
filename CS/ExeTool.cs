using Microsoft.CSharp;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SciTools
{
    /// <summary>
    /// EXE文件工具类
    /// </summary>
    public class ExeTool
    {
        # region 动态编译源码

        /// <summary>
        /// 解析并编译源码sourceCode，生成EXE
        /// </summary>
        public static void CompileExe(string sourceCode, string exeFile)
        {
            try
            {
                string[] assemblies = getUsing(sourceCode).ToArray();   // 获取引用程序集
                string result = (string) Compile(sourceCode, exeFile, assemblies);        // 编译源码
                if (!result.Equals("编译通过")) MessageBox.Show(result);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }


        /// <summary>
        /// 动态编译执行
        /// </summary>
        /// <param name="sourceCode">源码</param>
        /// <param name="assemblies">引用程序集</param>
        static object Compile(string sourceCode, string exeFile, string[] assemblies = null)
        {
            try
            {
                // 设置编译参数 System.Xml.dll
                CompilerParameters param = new CompilerParameters();
                param.GenerateExecutable = true;
                param.OutputAssembly = exeFile;
                //param.GenerateInMemory = false;
                //param.GenerateInMemory = true;

                // 添加常用的默认程序集
                param.ReferencedAssemblies.Add("Microsoft.CSharp.dll");
                param.ReferencedAssemblies.Add("mscorlib.dll");
                param.ReferencedAssemblies.Add("System.dll");
                param.ReferencedAssemblies.Add("System.Core.dll");
                param.ReferencedAssemblies.Add("System.Data.dll");
                param.ReferencedAssemblies.Add("System.Data.DataSetExtensions.dll");
                param.ReferencedAssemblies.Add("System.Drawing.dll");
                param.ReferencedAssemblies.Add("System.Windows.Forms.dll");
                param.ReferencedAssemblies.Add("System.Xml.dll");
                param.ReferencedAssemblies.Add("System.Xml.Linq.dll");

                if (assemblies != null)
                {
                    foreach (string name in assemblies)
                    {
                        string assembly = name + ".dll";
                        if (!param.ReferencedAssemblies.Contains(assembly))
                        {
                            param.ReferencedAssemblies.Add(assembly);
                        }
                    }
                }


                // 动态编译字符串代码
                CompilerResults result = new CSharpCodeProvider().CompileAssemblyFromSource(param, sourceCode);

                if (result.Errors.HasErrors)
                {
                    // 编译出错：
                    StringBuilder str = new StringBuilder();
                    foreach (CompilerError err in result.Errors)
                    {
                        str.AppendLine(err.ToString());
                    }
                    return str.ToString();
                }
                else
                {
                    // 编译通过：
                    return "编译通过";
                }
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        # endregion


        # region 相关功能函数

        ///// <summary>
        ///// 获取文件中的数据,自动判定编码格式
        ///// </summary>
        //private static string fileToString(String filePath)
        //{
        //    string str = "";

        //    //获取文件内容
        //    if (File.Exists(filePath))
        //    {
        //        StreamReader file1;

        //        file1 = new StreamReader(filePath, Encoding.UTF8);  // 读取文件中的数据
        //        str = file1.ReadToEnd();                            // 读取文件中的全部数据

        //        file1.Close();
        //        file1.Dispose();
        //    }
        //    return str;
        //}

        ///// <summary>
        ///// 获取第一个公用方法
        ///// </summary>
        ///// <param name="sourceCode"></param>
        ///// <returns></returns>
        //private static string getFirstPublicMethod(string sourceCode)
        //{
        //    string methodName = "";
        //    String[] lines = sourceCode.Replace("\r\n", "\n").Split('\n');
        //    foreach (string iteam in lines)
        //    {
        //        string line = iteam.Trim();
        //        if (line.StartsWith("public ") && line.Contains("(") && line.Contains(")"))
        //        {
        //            methodName = line.Substring(0, line.IndexOf("("));
        //            methodName = methodName.Substring(methodName.LastIndexOf(" ") + 1);
        //            break;
        //        }
        //    }
        //    return methodName;
        //}

        ///// <summary>
        ///// 判断指定的方法是否为静态方法
        ///// </summary>
        ///// <returns></returns>
        //private static bool isPublicStaticMethod(string sourceCode, string methodName)
        //{
        //    bool isStatic = false;
        //    String[] lines = sourceCode.Replace("\r\n", "\n").Split('\n');
        //    foreach (string iteam in lines)
        //    {
        //        string line = iteam.Trim();
        //        if (line.StartsWith("public ") && line.Contains(" " + methodName) && line.Contains("(") && line.Contains(")") && line.Contains("static"))
        //        {
        //            isStatic = true;
        //        }
        //    }
        //    return isStatic;
        //}

        /// <summary>
        /// 获取应用的程序集信息
        /// </summary>
        private static List<string> getUsing(string sourceCode)
        {
            String[] lines = sourceCode.Replace("\r\n", "\n").Split('\n');
            List<string> usings = new List<string>();
            foreach (string iteam in lines)
            {
                string line = iteam.Trim();
                if (line.StartsWith("using ") && line.EndsWith(";"))
                {
                    string usingAssembley = line.TrimEnd(';').Substring("using ".Length);
                    CheckAddAssembly(usings, usingAssembley);
                }
            }
            return usings;
        }

        /// <summary>
        /// 检测添加较短长度的Assembly名称
        /// </summary>
        private static void CheckAddAssembly(List<string> usings, string usingAssembley)
        {
            if (usings.Contains(usingAssembley)) return;
            for (int i = 0; i < usings.Count; i++)
            {
                string name = usings[i];
                if (usingAssembley.StartsWith(name + ".")) return;
                else if (name.StartsWith(usingAssembley + "."))
                {
                    usings[i] = usingAssembley;
                }
            }
            usings.Add(usingAssembley);
        }

        # endregion
    }
}
