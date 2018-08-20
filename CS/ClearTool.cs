using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClearDirectory
{
    public class ClearTool
    {
        /// <summary>
        /// 清空目录或文件
        /// </summary>
        public static void ClearDelet(string path)
        {
            if (File.Exists(path)) ClearDeletFile(path);
            if (Directory.Exists(path)) ClearDeletDirectory(path);
        }

        /// <summary>
        /// 先清空目录中的所有文件和子目录内容，再删除当前目录
        /// </summary>
        public static void ClearDeletDirectory(string dir)
        {
            if (Directory.Exists(dir))
            {
                // 清除目录下的所有文件
                foreach (String iteam in Directory.GetFiles(dir))
                {
                    ClearDeletFile(iteam);
                }

                // 清除目录下的所有子目录
                foreach (String iteam in Directory.GetDirectories(dir))
                {
                    ClearDeletDirectory(iteam);
                }

                String newName = System.IO.Directory.GetParent(dir).FullName + "\\$";
                while (File.Exists(newName)) newName += "$";

                // 清除当前目录
                Directory.Move(dir, newName);   // 重命名当前目录，清除目录名信息
                Directory.Delete(newName);      // 清除当前目录
            }
        }

        /// <summary>
        /// 先清空文件内容，再删除
        /// </summary>
        public static void ClearDeletFile(string file)
        {
            ClearFile(file);                // 清空文件内容
            if (File.Exists(file))
            {
                String newName = System.IO.Directory.GetParent(file).FullName + "\\$";
                while (File.Exists(newName)) newName += "$";

                File.Move(file, newName);   // 重命名文件，清除文件名称信息
                File.Delete(newName);       // 删除文件
            }
        }

        /// <summary>
        /// 清空文件内容
        /// </summary>
        public static void ClearFile(string file)
        {
            if (File.Exists(file))
            {
                int SIZE = 1024 * 10240;
                byte[] array = new byte[SIZE];
                array.Initialize();

                FileStream s = new FileStream(file, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite, SIZE, FileOptions.RandomAccess);

                // 清空原有文件内容
                while (s.Position + SIZE <= s.Length - 1)
                {
                    s.Write(array, 0, SIZE);
                }
                int reminds = (int)(s.Length - s.Position);
                if (reminds > 0) s.Write(array, 0, reminds);

                // 清除文件长度信息
                s.SetLength(0);
                s.Close();
            }
        }

    }

}
