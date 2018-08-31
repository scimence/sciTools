using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SciPay
{
    /// <summary>
    /// 自定义可逆加解密算法，数据、密码，可以为任意字符串；
    /// 1、加密 Locker.Encrypt()
    /// 2、解密 Locker.Decrypt()
    /// </summary>
    public class Locker
    {
        public static void example()
        {
            string data = Locker.Encrypt("待加密的数据信息", "自定义密码");
            string result = Locker.Decrypt("9olrx3xiueqa9iegopfqskl2updirdfp922o0zi1tqxsltks", "自定义密码");

            //MessageBox.Show(data.Equals("9olrx3xiueqa9iegopfqskl2updirdfp922o0zi1tqxsltks") + "");  // true
            //MessageBox.Show(result.Equals("待加密的数据信息") + "");                                // true
        }

        /// <summary>
        /// 使用SecretKey对数据data进行加密
        /// </summary>
        /// <param name="data">待加密的数据</param>
        /// <param name="SecretKey">自定义密码串</param>
        /// <returns></returns>
        public static string Encrypt(string data, string SecretKey)
        {
            string key = MD5.Encrypt(SecretKey);    // 7f01583d4ae071a9e39ffc18e3a12d9f
            data = EncoderTool_Alphabet.EncodeAlphabet(data);

            StringBuilder builder = new StringBuilder();

            int i = 0;
            foreach (char A in data)                // ofloifofikkaofkpigohjkieogjflaoginkooelpkbogibkp"
            {
                if (i >= key.Length)
                {
                    i = i % key.Length;
                    key = MD5.Encrypt(key);     // 变换新的key
                }
                char B = key[i++];
                char n = ToChar(ToInt(A) + ToInt(B) * 3);


                builder.Append(n);
            }

            return builder.ToString();
        }

        /// <summary>
        /// 使用SecretKey对数据data进行解密
        /// </summary>
        /// <param name="data">待解密的数据</param>
        /// <param name="SecretKey">解密密码串</param>
        /// <returns></returns>
        public static string Decrypt(string data, string SecretKey)
        {
            try
            {
                string key = MD5.Encrypt(SecretKey);// 7f01583d4ae071a9e39ffc18e3a12d9f

                StringBuilder builder = new StringBuilder();

                int i = 0;
                foreach (char A in data)            // 663r6uf0cew7cr5d9g08pnitiy7c9mi7ut8u98615nc1fkzp
                {
                    if (i >= key.Length)
                    {
                        i = i % key.Length;
                        key = MD5.Encrypt(key);
                    }
                    char B = key[i++];
                    char n = ToChar(ToInt(A) - ToInt(B) * 3);

                    builder.Append(n);
                }
                data = builder.ToString();
                data = EncoderTool_Alphabet.DecodeAlphabet(data);
            }
            catch (Exception ex)
            {
            }
            return data;
        }

        /// <summary>
        /// "0-9 a-z"映射为 0到35
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        private static int ToInt(char c)
        {
            if ('0' <= c && c <= '9') return c - '0';
            else if ('a' <= c && c <= 'z') return 10 + c - 'a';
            else return 0;
        }

        /// <summary>
        /// 0到35依次映射为"0-9 a-z"，
        /// 超出35取余数映射
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        private static char ToChar(int n)
        {
            if (n >= 36) n = n % 36;
            else if (n < 0) n = n % 36 + 36;

            if (n > 9) return (char)(n - 10 + 'a');
            else return (char)(n + '0');
        }


        ///// <summary>
        ///// "0-9 a-z A-Z"映射为 0到9 10到35 36到61
        ///// </summary>
        ///// <param name="c"></param>
        ///// <returns></returns>
        //public static int ToInt(char c)
        //{
        //    if ('0' <= c && c <= '9') return c - '0';
        //    else if ('a' <= c && c <= 'z') return 10 + c - 'a';
        //    else if ('A' <= c && c <= 'Z') return 36 + c - 'A';
        //    else return 0;
        //}

        ///// <summary>
        ///// 0到61依次映射为"0-9 a-z A-Z"，
        ///// 超出61取余数映射
        ///// </summary>
        ///// <param name="n"></param>
        ///// <returns></returns>
        //public static char ToChar(int n)
        //{
        //    if (n >= 62) n = n % 62;
        //    else if (n < 0) n = n % 62 + 62;

        //    if (n > 35) return (char)(n - 36 + 'A');
        //    else if (n > 9) return (char)(n - 10 + 'a');
        //    else return (char)(n + '0');
        //}
    }


}
