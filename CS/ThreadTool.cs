using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SciTools
{
    public class ThreadTool
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
    }
}
