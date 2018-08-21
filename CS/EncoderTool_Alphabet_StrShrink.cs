using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApplication11
{
    /// <summary>
    /// 全字母字符串压缩
    /// </summary>
    class EncoderTool_Alphabet_StrShrink
    {
        /// <summary>
        /// 重复字符压缩
        /// "enfkjaaaadaaaaaaaeaaaaaappppaaaaliaaaaaaaaaaaaaaeaaaaaaaaaaa" -> 
        /// "enfkja4da6p4a4lia14ea11"
        /// </summary>
        /// <param name="alphaStr"></param>
        /// <returns></returns>
        public static string shrink(string alphaStr)
        {
            StringBuilder builder = new StringBuilder();
            int count = 0;

            char C = ' ';
            foreach(char c in alphaStr)
            {
                if (c == C) count++;
                else
                {
                    if (count <= 2)
                    {
                        if (C != ' ') builder.Append(C.ToString() + (count == 2 ? C.ToString() : ""));
                    }
                    else builder.Append(C + count.ToString());

                    C = c;
                    count = 1;
                }
            }

            if (count <= 2)
            {
                if (C != ' ') builder.Append(C.ToString() + (count == 2 ? C.ToString() : ""));
            }
            else builder.Append(C + count.ToString());

            return builder.ToString();
        }

        /// <summary>
        /// 还原为原有串信息
        /// "enfkja4da6p4a4lia14ea11" -> 
        /// "enfkjaaaadaaaaaaaeaaaaaappppaaaaliaaaaaaaaaaaaaaeaaaaaaaaaaa"
        /// </summary>
        /// <param name="shrinkStr"></param>
        /// <returns></returns>
        public static string restore(string shrinkStr)
        {
            char C = ' ';
            StringBuilder builder = new StringBuilder();
            string numStr = "";

            foreach (char c in shrinkStr)
            {
                if ('a' <= c && c <= 'z')
                {
                    if (!numStr.Equals(""))
                    {
                        int n = Int32.Parse(numStr);
                        while (n-- > 1) builder.Append(C.ToString());
                        numStr = "";
                    }

                    builder.Append(c.ToString());
                    C = c;
                }
                else if ('0' <= c && c <= '9')
                {
                    numStr += c.ToString();
                }
            }

            if ('a' <= C && C <= 'z')
            {
                if (!numStr.Equals(""))
                {
                    int n = Int32.Parse(numStr);
                    while (n-- > 1) builder.Append(C.ToString());
                    numStr = "";
                }
            }

            return builder.ToString();
        }
    }
}
