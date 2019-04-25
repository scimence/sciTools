using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Tools
{
    /// <summary>
    /// 源码自动备份资源类，可在Program.Main() 函数启动时，调用BackUpSource()
    /// </summary>
    public class SourceCodeTool
    {
        /// <summary>
        /// 定义委托接口处理函数，用于实时处理cmd输出信息，或传递功能逻辑变量
        /// </summary>
        public delegate void Method();

        /// <summary>
        /// 在新的线程中执行method逻辑
        /// </summary>
        public static void ThreadRun(Method method)
        {
            Thread thread = new Thread(delegate()
                {
                    if (method != null) method();   // 执行method逻辑
                });

            thread.Priority = ThreadPriority.AboveNormal;           // 设置子线程优先级
            Thread.CurrentThread.Priority = ThreadPriority.Highest; // 设置当前线程为最高优先级
            thread.Start();
        }

        //--------------------

        /// <summary>
        /// 备份Source到Resources目录
        /// </summary>
        public static void BackUpSource()
        {
            ThreadRun(BackUpSourceProcess);
        }

        /// <summary>
        /// 备份Source到Resources目录
        /// </summary>
        public static void BackUpSourceProcess()
        {
            string slnDir = getSlnDir();            // 获取所在目录

            if (!slnDir.Equals(""))
            {
                string rootDirName = new DirectoryInfo(slnDir).Name;
                string srcName = rootDirName + "_src.zip";

                addResourcesInfoToResx(srcName);    // 添加生成的资源文件到Resources.resx
                zipSrcToResources(slnDir, srcName); // 压缩为zip
            }

            OutPutEntryNamespaceSrc();              // 输出资源
        }

        /// <summary>
        /// 获取Assembly所在的命名空间名称
        /// </summary>
        public static string GetNamespace(Assembly asssembly)
        {
            string Namespace = "";
            Type[] types = asssembly.GetTypes();
            if (types != null && types.Length > 0)
            {
                Namespace = types[0].Namespace;
            }
            return Namespace;
        }

        /// <summary>
        /// 输出入口Namespace对应的src资源
        /// </summary>
        public static void OutPutEntryNamespaceSrc()
        {
            Assembly asssembly = Assembly.GetEntryAssembly();
            string projectName = GetNamespace(asssembly);
            string srcName = projectName + "_src.zip";

            OutPutSource(srcName);              // 执行资源输出
        }

        /// <summary>
        /// 压缩工程src为Resources资源
        /// </summary>
        private static void zipSrcToResources(string slnDir, string srcName)
        {
            string Resources = getRsourcesDir(slnDir);              // 获取Resources目录
            string newZipFile = Resources + "\\" + srcName;

            String[] ignores = new String[] { "\\bin\\", "\\obj\\", "\\" + srcName };

            // 复制到缓存目录进行重编码、压缩
            string slnDirTmp = slnDir + "_src";
            if (Directory.Exists(slnDirTmp)) Directory.Delete(slnDirTmp, true);
            FileTool.CopyFolderTo(slnDir, slnDirTmp, true);        // 复制目录文件
            RecodeTool.Main(new String[] { slnDirTmp });           // 重编码
            bool ziped = ZipTool.zip(slnDirTmp, null, ignores);    // 压缩为zip，压缩时忽略bin和obj目录
            if (Directory.Exists(slnDirTmp)) Directory.Delete(slnDirTmp, true);

            string zipFile = slnDirTmp + ".zip";
            if (File.Exists(newZipFile)) File.Delete(newZipFile);  // 删除原有文件
            if (ziped) File.Move(zipFile, newZipFile);
        }

        /// <summary>
        /// 获取项目名称
        /// </summary>
        private static string getProjectName()
        {
            string slnDir = getSlnDir();                            // 获取所在目录
            if (!slnDir.Equals(""))
            {
                string rootDirName = new DirectoryInfo(slnDir).Name;
                return rootDirName;
            }
            return "";
        }

        /// <summary>
        /// 获取指定名称的Resource资源，无后缀
        /// </summary>
        public static byte[] getResourceObj(string name)
        {
            //string projectName = "ChargeModule";
            //string projectName = getProjectName();
            //if (projectName.Equals("")) return new byte[] { };

            Assembly asssembly = Assembly.GetEntryAssembly();
            string projectName = GetNamespace(asssembly);

            //Assembly asssembly = Assembly.GetExecutingAssembly();
            global::System.Resources.ResourceManager ResourceManager = new global::System.Resources.ResourceManager(projectName + ".Properties.Resources", asssembly);

            object obj = ResourceManager.GetObject(name);
            return ((byte[])(obj));
        }



        //--------------------

        /// <summary>
        /// 转化形如ICSharpCode.SharpZipLib.dll 的资源为对应名称 -> ICSharpCode_SharpZipLib
        /// </summary>
        public static string ToResourceName(string sourceName)
        {
            int index = sourceName.LastIndexOf(".");
            string resourceName = sourceName.Substring(0, index).Replace('.', '_');
            return resourceName;
        }

        /// <summary>
        /// 输出备份的源码资源
        /// </summary>
        public static void OutPutSource(string sourceName)
        {
            string curExe = DependentFiles.curExecutablePath();
            if (curExe.ToLower().EndsWith(".exe"))
            {
                string resourceName = ToResourceName(sourceName);

                byte[] obj = getResourceObj(resourceName);
                if (obj != null) DependentFiles.SaveFile(obj, DependentFiles.curDir() + sourceName, true);
            }
        }

        /// <summary>
        /// 获取sln目录下的Resources目录， 若sln目录存在，Resource不存在则创建,否则返回""
        /// </summary>
        public static string getRsourcesDir(string slnParenDir, string subDir = "Resources")
        {
            string Resources = "";
            if (slnParenDir != null && !slnParenDir.Equals("") && Directory.Exists(slnParenDir))
            {
                string name = new DirectoryInfo(slnParenDir).Name;
                string dir = slnParenDir + "\\" + name + "\\" + subDir;

                if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
                Resources = dir;
            }
            return Resources;
        }

        /// <summary>
        /// 在当前运行路径下的父路径下，获取.sln所在的文件目录， 若不存在则返回""
        /// </summary>
        public static string getSlnDir()
        {
            string curDir = AppDomain.CurrentDomain.BaseDirectory;
            while (!curDir.Equals("") && !isParentDirOfFile(curDir, ".sln"))
            {
                DirectoryInfo dirInfo = Directory.GetParent(curDir);
                if (dirInfo != null) curDir = dirInfo.FullName;
                else curDir = "";
            }
            return curDir;
        }

        /// <summary>
        /// 判断Dir目录下，是否存在文件名以endwith结尾的文件
        /// </summary>
        public static bool isParentDirOfFile(String Dir, String endwith)
        {
            if (Directory.Exists(Dir))
            {
                foreach (string file in Directory.GetFiles(Dir))
                {
                    if (file.ToLower().EndsWith(endwith)) return true;
                }
            }
            return false;
        }


        //--------------------

        /// <summary>
        /// 添加资源文件，到项目资源记录文件 \Properties\Resources.resx
        /// </summary>
        public static void addResourcesInfoToResx(string sourceName)
        {
            string slnDir = getSlnDir();                                    // 获取所在目录
            string Properties = getRsourcesDir(slnDir, "Properties");       // 
            string resx = Properties + "\\" + "Resources.resx";             // 获取资源记录文件
            if (File.Exists(resx))
            {
                string data = FileTool.fileToString(resx);                  // 读取文件内容
                if (!data.Contains(@"<value>..\Resources\" + sourceName + ";"))   // 不含有当前添加的资源,则添加
                {
                    string PropertyInfo = getReaderPropertyInfo(data);      // 获取属性信息
                    string resInfo = NewResInfo(sourceName, PropertyInfo);  // 生成新的资源配置信息

                    data = data.Replace("</root>", resInfo + "</root>");    // 添加至结尾
                    FileTool.SaveProcess(data, resx, null, true);           // 保存修改
                }
            }
        }

        //<data name="ChargeModule_src" type="System.Resources.ResXFileRef, System.Windows.Forms">
        //    <value>..\Resources\ChargeModule_src.zip;System.Byte[], mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089</value>
        //</data>
        /// <summary>
        /// 生成新的资源配置信息
        /// </summary>
        private static string NewResInfo(string sourceName, string propertyInfo)
        {
            string resourceName = ToResourceName(sourceName);

            string info = "";
            info += "  <data name=\"" + resourceName + "\" type=\"System.Resources.ResXFileRef, System.Windows.Forms\">" + "\r\n";
            info += @"    <value>..\Resources\" + sourceName + ";System.Byte[], mscorlib, " + propertyInfo + "</value>" + "\r\n";
            info += "  </data>" + "\r\n";

            return info;
        }

        // <value>System.Resources.ResXResourceReader, System.Windows.Forms, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089</value>
        /// <summary>
        /// 获取资源文件中的属性信息，Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
        /// </summary>
        private static string getReaderPropertyInfo(string data)
        {
            string info = "";
            string ReaderKey = "<value>System.Resources.ResXResourceReader, System.Windows.Forms, ";
            if (data.Contains(ReaderKey))
            {
                int start = data.IndexOf(ReaderKey) + ReaderKey.Length;
                int end = data.IndexOf("</value>", start);
                info = data.Substring(start, end - start);
            }

            return info;
        }

    }

}
