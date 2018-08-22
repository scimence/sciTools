using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SciTools
{

    //static class Program
    //{
    //    /// <summary>
    //    /// 应用程序的主入口点。
    //    /// </summary>
    //    [STAThread]
    //    static void Main()
    //    {
    //        DllTool.LoadResourceDll();                // 载入依赖dll
    //        _Main();
    //    }

    //    static void _Main()
    //    {
    //        SciUpdate.RunningExceptionTool.Run(call);   // 调用异常信息捕获类，进行异常信息的捕获
    //    }

    //    // 应用程序，入口逻辑
    //    public static void call(string[] args)
    //    {
    //        //UpdateTool.AutoUpdate();
    //        Application.EnableVisualStyles();
    //        Application.SetCompatibleTextRenderingDefault(false);

    //        Application.Run(new Form1());
    //    }
    //}

    /// <summary>
    /// 载入Resources中附带的dll文件，
    /// SciTools.DllTool.LoadResourceDll();
    /// C# 在EXE应用中集成dll https://blog.csdn.net/scimence/article/details/79018812
    /// </summary>
    public class DllTool
    {
        /// <summary>
        /// 载入资源文件中附带的所有dll文件
        /// 此函数需和dll函数调用前执行，且不在同一个函数体内
        /// </summary>
        public static void LoadResourceDll()
        {
            AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(CurrentDomain_AssemblyResolve);
        }

        private static System.Reflection.Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            string dllName = args.Name.Contains(",") ? args.Name.Substring(0, args.Name.IndexOf(',')) : args.Name.Replace(".dll", "");
            dllName = dllName.Replace(".", "_");
            if (dllName.EndsWith("_resources")) return null;
            string Namespace = Assembly.GetEntryAssembly().GetTypes()[0].Namespace;
            System.Resources.ResourceManager rm = new System.Resources.ResourceManager(Namespace + ".Properties.Resources", System.Reflection.Assembly.GetExecutingAssembly());
            byte[] bytes = (byte[])rm.GetObject(dllName);
            return System.Reflection.Assembly.Load(bytes);
        }
    }
}
