using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tools
{
    /// <summary>
    /// 文件压缩zip()、解压缩unzip()；
    /// 引用->添加引用：ICSharpCode.SharpZipLib.dll
    /// </summary>
    public class ZipTool
    {
        /// <summary>
        /// 根据给的文件参数，自动进行压缩或解压缩操作
        /// </summary>
        public static void Process(String[] files, String Password = null)
        {
            if (files.Length > 0)
            {
                if (files.Length == 1 && (files[0].ToLower().EndsWith(".zip") || files[0].ToLower().EndsWith(".rar")))
                {
                    unzip(files[0], null, Password, null);                  // 解压缩
                }
                else
                {
                    String zipPath = FileTools.getPathNoExt(files[0]) + ".zip";	// 以待压缩的第一个文件命名生成的压缩文件
                    String BaseDir = FileTools.getParent(files[0]);				// 获取第一个文件的父路径信息
                    if (files.Length == 1)									// 若载入的为单个目录，则已当前目录作为基础路径
                    {
                        String file = files[0];
                        if (Directory.Exists(file)) BaseDir = file + "\\";
                    }

                    String[] subFiles = FileTools.getSubFiles(files);			// 获取args对应的所有目录下的文件列表
                    zip(zipPath, BaseDir, subFiles, Password, null);		// 对载入的文件进行压缩操作
                }
            }
        }

        /// <summary>
        /// 压缩所有文件files为zip
        /// </summary>
        public static bool zipFiles(String[] files, String Password = null, String[] ignoreNames = null)
        {
            return zip(null, null, files, Password, ignoreNames);
        }

        /// <summary>
        /// 压缩指定的文件或文件夹为zip
        /// </summary>
        public static bool zip(String file, String Password = null, String[] ignoreNames = null)
        {
            return zip(null, null, new String[] { file }, Password, ignoreNames);
        }

        /// <summary>
        /// 判断fileName中是否含有ignoreNames中的某一项
        /// </summary>
        private static bool ContainsIgnoreName(String fileName, String[] ignoreNames)
        {
            if (ignoreNames != null && ignoreNames.Length > 0)
            {
                foreach (string name in ignoreNames)
                {
                    if (fileName.Contains(name)) return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 压缩所有文件files，为压缩文件zipFile, 以相对于BaseDir的路径构建压缩文件子目录，ignoreNames指定要忽略的文件或目录
        /// </summary>
        public static bool zip(String zipPath, String BaseDir, String[] files, String Password = null, String[] ignoreNames = null)
        {
            if (files == null || files.Length == 0) return false;
            if (zipPath == null || zipPath.Equals("")) zipPath = FileTools.getPathNoExt(files[0]) + ".zip";	// 默认以第一个文件命名压缩文件
            if (BaseDir == null || BaseDir.Equals("")) BaseDir = FileTools.getParent(files[0]);				// 默认以第一个文件的父目录作为基础路径
            Console.WriteLine("所有待压缩文件根目录：" + BaseDir);

            try
            {
                FileTools.mkdirs(FileTools.getParent(zipPath));         // 创建目标路径
                Console.WriteLine("创建压缩文件：" + zipPath);

                FileStream input = null;
                ZipOutputStream zipStream = new ZipOutputStream(File.Create(zipPath));
                if (Password != null && !Password.Equals("")) zipStream.Password = Password;

                files = FileTools.getSubFiles(files);               // 获取子目录下所有文件信息
                for (int i = 0; i < files.Length; i++)
                {
                    if (ContainsIgnoreName(files[i], ignoreNames)) continue;    // 跳过忽略的文件或目录

                    String entryName = FileTools.relativePath(BaseDir, files[i]);
                    zipStream.PutNextEntry(new ZipEntry(entryName));
                    Console.WriteLine("添加压缩文件：" + entryName);

                    if (File.Exists(files[i]))                  // 读取文件内容
                    {
                        input = File.OpenRead(files[i]);
                        Random rand = new Random();
                        byte[] buffer = new byte[10240];
                        int read = 0;
                        while ((read = input.Read(buffer, 0, 10240)) > 0)
                        {
                            zipStream.Write(buffer, 0, read);
                        }
                        input.Close();
                    }
                }
                zipStream.Close();
                Console.WriteLine("文件压缩完成！");

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            return false;
        }

        /// <summary>
        /// 解压文件 到指定的路径，可通过targeFileNames指定解压特定的文件
        /// </summary>
        public static bool unzip(String zipPath, String targetPath = null, String Password = null, String[] targeFileNames = null)
        {
            if (File.Exists(zipPath))
            {
                if (targetPath == null || targetPath.Equals("")) targetPath = FileTools.getPathNoExt(zipPath);
                Console.WriteLine("解压文件：" + zipPath);
                Console.WriteLine("解压至目录：" + targetPath);

                try
                {
                    ZipInputStream zipStream = null;
                    FileStream bos = null;

                    zipStream = new ZipInputStream(File.OpenRead(zipPath));
                    if (Password != null && !Password.Equals("")) zipStream.Password = Password;

                    ZipEntry entry = null;
                    while ((entry = zipStream.GetNextEntry()) != null)
                    {
                        if (targeFileNames != null && targeFileNames.Length > 0)                // 若指定了目标解压文件
                        {
                            if (!ContainsIgnoreName(entry.Name, targeFileNames)) continue;      // 跳过非指定的文件
                        }

                        String target = targetPath + "\\" + entry.Name;
                        if (entry.IsDirectory) FileTools.mkdirs(target); // 创建目标路径
                        if (entry.IsFile)
                        {
                            FileTools.mkdirs(FileTools.getParent(target));

                            bos = File.Create(target);
                            Console.WriteLine("解压生成文件：" + target);

                            int read = 0;
                            byte[] buffer = new byte[10240];
                            while ((read = zipStream.Read(buffer, 0, 10240)) > 0)
                            {
                                bos.Write(buffer, 0, read);
                            }
                            bos.Flush();
                            bos.Close();
                        }
                    }
                    zipStream.CloseEntry();

                    Console.WriteLine("解压完成！");
                    return true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString()); ;
                }
            }
            return false;
        }

        #region 读取压缩文件中指定路径的文件

        /// <summary>
        /// 读取zip中指定路径的文件；
        /// 
        /// 如：
        /// zipPath="E:/tmp/1.apk"
        /// entryName ="assets/ltpay_config.txt"
        /// </summary>
        public static string ReadFile(String zipPath, String entryName, Encoding encode = null, String Password = null)
        {
            string data = "";

            if(encode == null) encode = Encoding.UTF8;
            ZipInputStream zipStream = null;

            zipStream = new ZipInputStream(File.OpenRead(zipPath));
            if (Password != null && !Password.Equals("")) zipStream.Password = Password;

            ZipEntry entry = null;
            while ((entry = zipStream.GetNextEntry()) != null && data.Equals(""))
            {
                if(entry.Name.Equals(entryName))
                {
                    long len = zipStream.Length;
                    byte[] buff = new byte[len];

                    zipStream.Read(buff, 0, Convert.ToInt32(len));
                    data = encode.GetString(buff);
                }
                zipStream.CloseEntry();
            }
            zipStream.Close();

            return data;
        }

        /// <summary>
        /// 向压缩文件zipPath中entryName路径，写入文件数据data
        /// </summary>
        /// <param name="zipPth"></param>
        /// <param name="entryName"></param>
        /// <param name="data"></param>
        public static void WriteFile(String zipPath, String entryName, byte[] data, String Password = null)
        {
            ZipOutputStream zipStream = new ZipOutputStream(File.Open(zipPath, FileMode.Open));
            if (Password != null && !Password.Equals("")) zipStream.Password = Password;

            zipStream.PutNextEntry(new ZipEntry(entryName));
            Console.WriteLine("添加压缩文件：" + entryName);

            zipStream.Write(data, 0, data.Length);
            zipStream.CloseEntry();

            zipStream.Close();
        }

        #endregion

    }


    /// <summary>
    /// 通用功能函数
    /// </summary>
    public class FileTools
    {
        /// <summary>
        /// 检测目录是否存在，若不存在则创建
        /// </summary>
        public static void mkdirs(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        /// <summary>
        /// 获取去除拓展名的文件路径
        /// </summary>
        public static String getPathNoExt(String path)
        {
            if (File.Exists(path)) return Directory.GetParent(path).FullName + "\\" + Path.GetFileNameWithoutExtension(path);
            else return Directory.GetParent(path).FullName + "\\" + Path.GetFileName(path);
        }

        /// <summary>
        /// 获取父目录的路径信息
        /// </summary>
        public static String getParent(String path)
        {
            return System.IO.Directory.GetParent(path).FullName + "\\";
        }


        /// <summary>
        /// 获取父目录的路径信息
        /// </summary>
        public static String getFileName(String path)
        {
            return System.IO.Path.GetFileName(path);
        }

        /// <summary>
        /// 获取filePath的相对于BaseDir的路径
        /// </summary>
        public static String relativePath(String BaseDir, String filePath)
        {
            String relativePath = "";
            if (filePath.StartsWith(BaseDir)) relativePath = filePath.Substring(BaseDir.Length);
            return relativePath;
        }


        //-----------------------------------------------------------------------------------------

        /// <summary>
        /// 获取paths路径下所有文件信息
        /// </summary>
        public static String[] getSubFiles(String[] Paths)
        {
            List<String> list = new List<String>();	        // paths路径下所有文件信息

            foreach (String path in Paths)
            {
                List<String> subFiles = getSubFiles(path);	// 获取路径path下所有文件列表信息
                list = ListAdd(list, subFiles);
            }

            String[] A = List2Array(list);					// 转化为数组形式

            return A;
        }

        /// <summary>
        /// 合并list1和list2到新的list
        /// </summary>
        public static List<String> ListAdd(List<String> list1, List<String> list2)
        {
            List<String> list = new List<String>();

            foreach (String path in list1) if (!list.Contains(path)) list.Add(path);
            foreach (String path in list2) if (!list.Contains(path)) list.Add(path);

            return list;
        }

        /// <summary>
        /// 获取file目录下所有文件列表
        /// </summary>
        public static List<String> getSubFiles(String file)
        {
            List<String> list = new List<String>();

            if (File.Exists(file))
            {
                if (!list.Contains(file)) list.Add(file);
            }

            if (Directory.Exists(file))
            {
                // 获取目录下的文件信息
                foreach (String iteam in Directory.GetFiles(file))
                {
                    if (!list.Contains(iteam)) list.Add(iteam);
                }

                // 获取目录下的子目录信息
                foreach (String iteam in Directory.GetDirectories(file))
                {
                    List<String> L = getSubFiles(iteam);	// 获取子目录下所有文件列表
                    foreach (String path in L)
                    {
                        if (!list.Contains(path)) list.Add(path);
                    }
                }

                // 记录当前目录
                if (Directory.GetFiles(file).Length == 0 && Directory.GetDirectories(file).Length == 0)
                {
                    if (!list.Contains(file)) list.Add(file + "\\");
                }
            }

            return list;
        }

        /// <summary>
        /// 转化list为数组
        /// </summary>
        public static String[] List2Array(List<String> list)
        {
            int size = (list == null ? 0 : list.Count);
            String[] A = new String[size];

            int i = 0;
            foreach (String S in list)
            {
                A[i++] = S;
            }

            return A;
        }

    }
}
