using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SciTools
{
    /// <summary>
    /// 对文件生成加密数据:
    /// 
    /// 替换FileDeCoder中的,
    /// data   ->  public static byte[] data = new byte[] { getEncodeData（）};
    /// key    ->  key;
    /// key    ->  key;
    /// 启动，FileDeCoder.Run(args);
    /// </summary>
    public class FileEncoderTool
    {
        public static string key;
        public static string nameSpace;

        /// <summary>
        /// 获取filePath路径下的文件数据，
        /// 使用当前时间信息进行加密
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns>加密key+字节值</returns>
        public static string getEncodeData(string filePath)
        {
            key = DateTime.Now.ToString("yyyyMMddHHmmssffffff");

            byte[] bytes = File2Bytes(filePath);
            nameSpace = GetNamespace(bytes);

            Locker(ref bytes, key);
            string data = BytesToString(bytes);

            return data;
        }

        /// <summary>
        /// 获取filePath路径下的文件数据，
        /// 使用当前时间信息进行加密
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns>加密key+字节值</returns>
        public static void getEncodeDataOuter(string filePath, byte[] bytes = null, string data = null)
        {
            key = DateTime.Now.ToString("yyyyMMddHHmmssffffff");
            bytes = File2Bytes(filePath);                            // 读取文件数据
            nameSpace = GetNamespace(bytes);

            Locker(ref bytes, key);                                  // 数据加密

            SaveFile(bytes, filePath.Replace(".exe", ".data"), true); // 保存数据为文件
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
        /// 保存Byte数组为文件
        /// </summary>
        public static void SaveFile(Byte[] array, string path, bool repalce = false)
        {
            if (repalce && System.IO.File.Exists(path)) System.IO.File.Delete(path);    // 若目标文件存在，则替换
            if (!System.IO.File.Exists(path))
            {
                // 创建输出流
                System.IO.FileStream fs = new System.IO.FileStream(path, System.IO.FileMode.Create);

                //将byte数组写入文件中
                fs.Write(array, 0, array.Length);
                fs.Close();
            }
        }

        /// <summary>
        /// 获取Assembly所在的命名空间名称
        /// </summary>
        private static string GetNamespace(byte[] bytes)
        {
            Assembly asssembly = Assembly.Load(bytes);

            string Namespace = "";
            Type[] types = asssembly.GetTypes();
            if (types != null && types.Length > 0)
            {
                Namespace = types[0].Namespace;
            }
            return Namespace;
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
        /// 转化bytes数组为字符串
        /// </summary>
        /// <param name="bytes">待转化的数据</param>
        /// <returns></returns>
        private static string BytesToString(byte[] bytes)
        {
            StringBuilder builder = new StringBuilder();
            bool isFirst = true;
            foreach (byte b in bytes)
            {
                builder.Append((isFirst ? "" : ",") + b);
                if (isFirst) isFirst = false;
            }
            return builder.ToString();
        }
    }

}
