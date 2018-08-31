using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SciPay
{
    /// <summary>
    /// 从指定网址获取配置信息：
    /// Configer.Instance.Get("QrPayServerAddress")
    /// 
    /// serverInfo.txt：
    /// QrPayServerAddress(127.0.0.1:7002)QrPayServerAddress
    /// ApplicationTest(127.0.0.1:7001)ApplicationTest
    /// </summary>
    public class WebConfiger
    {
        //public static WebConfiger Instance = new WebConfiger();

        private string url = "https://scimence.gitee.io/config/files/config.txt";    // 配置文件文件网址

        /// <summary>
        /// 获取指定网址的配置信息
        /// </summary>
        /// <param name="url"></param>
        public WebConfiger(string url)
        {
            if(url != null && !url.Equals("")) this.url = url;
        }

        /// <summary>
        /// 获取key对应的配置信息
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string Get(string key)
        {
            if (url == null || url.Equals("")) return "";
            if (key == null || key.Equals("")) return "";

            string value = WebTool.getWebData(url, key);
            if (key.Equals("QrPayServerAddress") && value.Equals("")) 
                value = "60.205.185.168:8002";

            return value;
        }
    }
}
