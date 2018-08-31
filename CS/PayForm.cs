using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SciPay
{
    /// <summary>
    /// 支付示例： PayForm.Pay("自定义软件名称1", "自定义商品名称1", "0.01", "", "", "", PayResult);
    /// private void PayResult(string param)
    /// {
    ///     MessageBox.Show("支付结果:" + param);
    /// }
    /// </summary>
    public partial class PayForm : Form
    {
        /// <summary>
        /// 调用支付
        /// </summary>
        /// <param name="SeverUrl">计费服务器Url</param>
        /// <param name="soft">软件名称</param>
        /// <param name="product">商品名称</param>
        /// <param name="money">金额</param>
        /// <param name="machinCode">机器码</param>
        /// <param name="ext">拓展参数</param>
        /// <param name="call">支付结果回调</param>
        public static void Pay(string soft, string product, string money, string SeverUrl = "", string machinCode = "", string ext = "", PayCallBack call = null)
        {
            PayParam param = new PayParam(soft, product, money, SeverUrl, machinCode, ext);
            PayForm payForm = new PayForm(param, call);
            payForm.Text = product + " - " + "待支付金额：" + money + "元";

            //payForm.Show();
            payForm.ShowDialog();
        }

        /// <summary>
        /// 预下单,获取订单号
        /// </summary>
        public static string PreOrder(PayParam param)
        {
            string orderUrl = param.SeverUrl + "?" + "TYPE=PreOrder" + "&" + param.ToString();

            string data = WebTool.getWebData(orderUrl);
            string orderId = WebTool.getNodeData(data, "Result", false).Trim();
            return orderId;
        }

        /// <summary>
        /// 获取预下单展示Url
        /// </summary>
        public static string ShowPreOrderUrl(string SeverUrl, string preOrderId)
        {
            string url = SeverUrl + "?" + "TYPE=ShowPreOrder" + "&" + "preOrderId=" + preOrderId;
            return url;
        }

        /// <summary>
        /// 查询订单支付结果
        /// </summary>
        public static string OrderResult(PayParam param, string preOrderId)
        {
            string url = param.SeverUrl + "?" + "TYPE=OrderResult" + "&" + "preOrderId=" + preOrderId;
            string paramStr = param.ToString();

            string data = WebTool.getWebData(url);
            string result = WebTool.getNodeData(data, "Result", false).Trim();

            // 使用创建订单时的参数信息，解密查询结果
            if (paramStr != null && !paramStr.Equals(""))
            {
                result = Locker.Decrypt(result, preOrderId + paramStr);
            }

            return result;
        }

        //--------------


        PayParam param;         // 支付参数信息

        public delegate void PayCallBack(string param);
        PayCallBack call = null;   // 支付回调处理逻辑


        public PayForm(PayParam param, PayCallBack call)
        {
            InitializeComponent();
            init(param, call);
        }

        public PayForm(string soft, string product, string money, string SeverUrl = "", string machinCode = "", string ext = "", PayCallBack call = null)
        {
            InitializeComponent();
            PayParam param = new PayParam(soft, product, money, SeverUrl, machinCode, ext);
            init(param, call);
        }

        private void init(PayParam param, PayCallBack call)
        {
            this.param = param;
            this.call = call;

            timerRefresh_Tick(null, null);
        }

        string orderId = "";
        private void timerRefresh_Tick(object sender, EventArgs e)
        {
            if (orderId.Equals(""))
            {
                orderId = PreOrder(param);                      // 1、下单，获取订单号

                string url = ShowPreOrderUrl(param.SeverUrl, orderId);
                contentWebBrowser.Navigate(url);                // 2、显示二维码支付页面
            }
            else
            {
                string result = OrderResult(param, orderId);    // 3、查询支付结果
                string isSuccess = WebTool.getNodeData(result, "isSuccess", false).ToLower();
                if (isSuccess.Equals("true"))
                {
                    timerRefresh.Enabled = false;
                    if (call != null) call(result);
                    this.Close();
                }
            }
        }
    }

    /// <summary>
    /// 支付参数信息
    /// </summary>
    public class PayParam
    {
        //public string SeverUrl = "http://" + Register.QrPayServerAddress + "/pages/pay.aspx";
        public string SeverUrl = "";

        public string machinCode = "机器码1";
        public string soft = "easyIcon软件";
        public string product = "注册0.01元";
        public string money = "0.01";
        public string ext = "备注信息";

        public PayParam(string soft, string product, string money, string SeverUrl = "", string machinCode = "", string ext = "")
        {
            if (SeverUrl != null && !SeverUrl.Equals("")) this.SeverUrl = SeverUrl;
            this.machinCode = machinCode;
            this.soft = soft;
            this.product = product;
            this.money = money;
            this.ext = ext;
        }

        /// <summary>
        /// 拼接为参数串
        /// </summary>
        public string ToString(/*string soft, string product, string money, string machinCode = "", string ext = ""*/)
        {
            return "machinCode=" + machinCode + "&" + "soft=" + soft + "&" + "product=" + product + "&" + "money=" + money + "&" + "ext=" + ext;
        }

    }
}
