﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SciTools
{

    // 示例：scimence( Name1(6JSO-F2CM-4LQJ-JN8P) )scimence
    // string url = "https://git.oschina.net/scimence/easyIcon/wikis/OnlineSerial";
    // 
    // string data = getWebData(url);
    // string str1 = getNodeData(data, "scimence", false);
    // string str2 = getNodeData(str1, "Name1", true);

    /// <summary>
    /// 此类用于获取，在网络文件中的配置信息
    /// </summary>
    public class WebTool
    {
        #region 网络数据的读取

        /// <summary>
        /// 获取url页面，key(**value**)中的value数据
        /// </summary>
        public static string getWebData(string url, string key)
        {
            string data = getWebData(url);
            string str1 = getNodeData(data, key, true);

            return str1;
        }

        //从给定的网址中获取数据
        public static string getWebData(string url)
        {
            try
            {
                System.Net.WebClient client = new System.Net.WebClient();
                client.Encoding = System.Text.Encoding.UTF8;
                string data = client.DownloadString(url);
                return data;
            }
            catch (Exception) { return ""; }
        }

        #endregion


        // 从自定义格式的数据data中，获取key对应的节点数据
        //p>scimence(&#x000A;NeedToRegister(false)NeedToRegister&#x000A;RegisterPrice(1)RegisterPrice&#x000A;)scimence</p>&#x000A;</div>
        // NeedToRegister(false)&#x000A;RegisterPrice(1)   finalNode的数据格式
        public static string getNodeData(string data, string key, bool finalNode)
        {
            try
            {
                string S = key + "(", E = ")" + (finalNode ? "" : key);
                int indexS = data.IndexOf(S) + S.Length;
                int indexE = data.IndexOf(E, indexS);

                return data.Substring(indexS, indexE - indexS);
            }
            catch (Exception) { return data; }
        }
    }
}
