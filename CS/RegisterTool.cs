using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace SciPay
{
    /// <summary>
    /// RegisterTool.CheckRegLogic(SoftName, AuthorName); 检测注册逻辑是否执行完成。
    /// if(RegisterTool.Instance != null) RegisterTool.Instance.RegLogic_Online(是否已注册回调处理逻辑)
    /// </summary>
    public class RegisterTool
    {
        public static RegisterTool Instance = null;

        /// <summary>
        /// 检测注册逻辑是否执行完成
        /// </summary>
        /// <param name="SoftName"></param>
        /// <param name="AuthorName"></param>
        /// <returns></returns>
        public static bool CheckRegLogic(string SoftName, string AuthorName)
        {
            if (Instance == null)
            {
                string QrPayServerAddress = new WebConfiger("https://scimence.gitee.io/config/files/config.txt").Get("QrPayServerAddress");
                Instance = new RegisterTool(QrPayServerAddress, SoftName, AuthorName);
            }

            bool isRegUser = Instance.IsRegUser();
            if (!isRegUser)
            {
                DialogResult result = MessageBox.Show("您需要先完成注册,才能使用此功能", "软件注册", MessageBoxButtons.OKCancel);
                if (result == DialogResult.OK) Instance.RegLogic();
            }

            return isRegUser;
        }

        //--------------------------

        public string QrPayServerAddress = "127.0.0.1";   // Configer.Instance.Get("QrPayServerAddress");
        public string SoftName = "软件1";                 // DependentFiles.GetNamespace(Assembly.GetEntryAssembly());
        public string AuthorName = "无名";                // 软件作者

        /// <summary>
        /// 是否需要注册回调处理逻辑 code="yes"、"no"、"end"
        /// </summary>
        /// <param name="code">"yes"、"no"、"end"</param>
        public delegate void NeedRegisterCallBack(string code);
        public NeedRegisterCallBack needRegCallBack;

        private Timer Timer_CheckOnlineRegister = null;

        /// <summary>
        /// 创建Register对象
        /// </summary>
        /// <param name="QrPayServerAddress">Qr支付网址</param>
        /// <param name="SoftName">软件名称</param>
        public RegisterTool(string QrPayServerAddress, string SoftName, string AuthorName)
        {
            this.QrPayServerAddress = QrPayServerAddress;
            this.SoftName = SoftName;
            this.AuthorName = AuthorName;

            // 定时查询在线注册信息
            Timer_CheckOnlineRegister = new Timer();
            Timer_CheckOnlineRegister.Interval = 60000;
            Timer_CheckOnlineRegister.Enabled = true;
            Timer_CheckOnlineRegister.Tick += new System.EventHandler(Timer_CheckOnlineRegister_Tick);
        }

        // 定时器，用于首次打开界面时控制信息的延时载入
        private void Timer_CheckOnlineRegister_Tick(object sender, EventArgs e)
        {
            RegLogic_Online();    // 实时查询，在线注册逻辑
        }

        /// <summary>
        /// 根据网页控制开关，判定是否需要注册 
        /// 若软件的支付金额为0，则不需要注册
        /// </summary>
        /// <returns></returns>
        public bool needRegister()
        {
            string price = getPrice();
            if (price.Equals("") || !price.Equals("0"))
            {
                if (getStartTimes() <= getSoftFreeTimes()) return false;     // 小于10次启动次数，不需要注册
                else return true;
            }
            else return false;
        }

        int freeTimes = 0;
        private int getSoftFreeTimes()
        {
            if (freeTimes > 0) return freeTimes;

            try
            {
                string value = ExtInfo("freeTimes");
                if (value.Equals("")) freeTimes = 15;
                else freeTimes = int.Parse(value);
            }
            catch (Exception)
            {
                freeTimes = 15;
            }

            return freeTimes;
        }

        private int startTimes = -1;
        /// <summary>
        /// 获取客户端的当前启动次数
        /// </summary>
        /// <returns></returns>
        private int getStartTimes()
        {
            if (startTimes == -1)
            {
                try
                {
                    string times = GetValue("startTimes", MachineInfo_Serial.MachineSerial(), SoftName);
                    if (times.Equals("")) startTimes = 0;
                    else startTimes = int.Parse(times);
                }
                catch (Exception)
                {
                    return 10000;
                }
            }
            return startTimes;
        }

        private string price = "";

        /// <summary>
        /// 获取软件的支付金额信息
        /// </summary>
        /// <returns></returns>
        public string getPrice()
        {
            //if (!price.Equals("")) return price;
            string Url = "http://" + QrPayServerAddress + "/pages/softinfo.aspx?TYPE=Select&softName=" + SoftName + "&key=price";
            string data = WebTool.getWebData(Url).Trim();
            if (!data.Equals(""))
            {
                data = Locker.Decrypt(data, SoftName);  // 使用软件名称解密，加密的金额信息
                price = WebTool.getNodeData(data, "price", false);
            }
            if (price.Equals("")) price = "10.00";

            return price;
        }
            
        /// <summary>
        /// 在线注册逻辑,自动检测是否已在线注册，若已经注册，则完成本地注册逻辑。
        /// 启动后首次，通过接口发送软件启动信息至服务器；
        /// 再次检测时，在查询本地注册信息之后，若未注册则在线查询。
        /// 通过needRegCallBack执行回调处理逻辑
        /// </summary>
        public void RegLogic_Online(NeedRegisterCallBack needRegCallBack = null)
        {
            if (needRegCallBack != null) this.needRegCallBack = needRegCallBack;    // 记录回调处理逻辑

            string onlineSerial = "";

            // 0、发送软件启动信息至服务器
            bool isFirst = isFirstGetOnlineSerial;
            if (isFirst)
            {
                // 获取序列号信息
                onlineSerial = GetOnlineSerial(MachineInfo_Serial.MachineSerial(), SoftName, MachineInfo_Serial.ComputerName(), MachineInfo_Serial.UserName(), "");

                // 显示软件msg信息
                string SoftExt = ExtInfo("msg");
                showExtMessage("SoftExt", SoftExt);

                // 显示机器软件对应的msg信息
                string MachineSoftMsg = GetValue("msg", MachineInfo_Serial.MachineSerial(), SoftName);
                showExtMessage("MachineSoftMsg", MachineSoftMsg);
            }

            if (!this.IsRegUser())      // 1、若本地未注册
            {
                if (!isFirst) onlineSerial = GetOnlineSerial(MachineInfo_Serial.MachineSerial(), SoftName, MachineInfo_Serial.ComputerName(), MachineInfo_Serial.UserName(), "");

                // 2、则查询是否已在线注册
                if (!onlineSerial.Equals("") && onlineSerial.Equals(MachineInfo_Serial.RegSerial(SoftName)))
                {
                   RegistryTool.RegistrySave(AuthorName + @"\" + SoftName + @"\Set", "Serial", onlineSerial);
                    MessageBox.Show("恭喜，您已成功注册！（异步注册）\r\n\r\n即便重装系统，也无需再次注册.\r\nCopyright ©  2018 Scimence");
                }
            }
        }

        private bool isFirstGetOnlineSerial = true;

        /// <summary>
        /// 根据机器信息, 在线查询是否已注册码；
        /// TYPE=GetRegSerial&machinCode=XRUM-LYKS-4R2P-QP6H&soft=easyIcon&computerName=计算机名称&userName=用户名称&ext=拓展参数&counter=true
        /// </summary>
        /// <param name="machinCode"></param>
        /// <param name="soft"></param>
        /// <param name="computerName"></param>
        /// <param name="userName"></param>
        /// <param name="ext"></param>
        /// <returns></returns>
        public string GetOnlineSerial(string machinCode, string soft, string computerName = "", string userName = "", string ext = "")
        {
            string counter = "false";
            if (isFirstGetOnlineSerial)
            {
                counter = "true";
                isFirstGetOnlineSerial = false;
            }

            string Url = "http://" + QrPayServerAddress + "/pages/onlineserial.aspx?TYPE=GetRegSerial&machinCode=" + machinCode + "&soft=" + soft + "&computerName=" + computerName + "&userName=" + userName + "&ext=" + ext + "&counter=" + counter;
            string data = WebTool.getWebData(Url).Trim();

            return data;
        }


        /// <summary>
        /// 查询KEY列对应的数据信息
        /// </summary>
        /// <param name="KEY">列名称</param>
        /// <param name="machinCode">机器码</param>
        /// <param name="soft">软件名称</param>
        /// <returns></returns>
        public string GetValue(string KEY, string machinCode, string soft)
        {
            string Url = "http://" + QrPayServerAddress + "/pages/onlineserial.aspx?TYPE=GetValue&KEY=" + KEY + "&machinCode=" + machinCode + "&soft=" + soft;
            string data = WebTool.getWebData(Url).Trim();
            if (!data.Equals(""))
            {
                data = Locker.Decrypt(data, machinCode + soft);
                data = WebTool.getNodeData(data, KEY, false);
            }

            return data;
        }

        private string recomondUrl = "";
        /// <summary>
        /// 获取软件的支付金额信息
        /// </summary>
        /// <returns></returns>
        public string RecommendUrl()
        {
            if (!recomondUrl.Equals("")) return recomondUrl;

            string Url = "http://" + QrPayServerAddress + "/pages/softinfo.aspx?TYPE=Select&softName=" + SoftName + "&key=recomondUrl";
            string data = WebTool.getWebData(Url).Trim();
            recomondUrl = data;

            return data;
        }

        private string linkUrl = "";
        /// <summary>
        /// 获取软件的支付金额信息
        /// </summary>
        /// <returns></returns>
        public string LinkUrl()
        {
            if (!linkUrl.Equals("")) return linkUrl;

            string Url = "http://" + QrPayServerAddress + "/pages/softinfo.aspx?TYPE=Select&softName=" + SoftName + "&key=linkUrl";
            string data = WebTool.getWebData(Url).Trim();
            linkUrl = data;

            return data;
        }


        /// <summary>
        /// 获取获取ext附加参数中的信息
        /// </summary>
        /// <returns></returns>
        public string ExtInfo(string extKey)
        {
            string Url = "http://" + QrPayServerAddress + "/pages/softinfo.aspx?TYPE=Select&softName=" + SoftName + "&key=ext";
            string data = WebTool.getWebData(Url).Trim();
            if (!data.Equals(""))
            {
                data = Locker.Decrypt(data, SoftName);  // 使用软件名称解密，加密的金额信息
                price = WebTool.getNodeData(data, "ext", false);
            }
            string extValue = WebTool.getNodeData(data, extKey, false);

            return extValue;
        }

        /// <summary>
        /// 显示软件公告信息
        /// </summary>
        public void showExtMessage(string key, string msg)
        {
            //string msg = ExtInfo("msg").Replace("\\r", "\r").Replace("\\n", "\n").Replace("\\t", "\t");
            msg = msg.Replace("\\r", "\r").Replace("\\n", "\n").Replace("\\t", "\t");
            if (msg.Equals("")) return;     // 若无公告信息，则返回
            else
            {
                bool contains = RegistryTool.RegistryCotains(AuthorName + @"\" + SoftName + @"\Set", key);
                if (contains)
                {
                    string preMsg = RegistryTool.RegistryStrValue(AuthorName + @"\" + SoftName + @"\Set", key);
                    if (!preMsg.Equals("") && preMsg.Equals(msg)) return;
                }
            }

            MessageBox.Show(msg);
            RegistryTool.RegistrySave(AuthorName + @"\" + SoftName + @"\Set", key, msg);
        }

        /// <summary>
        ///  从注册表载入信息，判断用户是否已经注册
        /// </summary>
        /// <returns></returns>
        public bool IsRegUser()
        {
            // 检测本地注册信息，
            // 若已经注册成功
            bool containsSerial = RegistryTool.RegistryCotains(AuthorName + @"\" + SoftName + @"\Set", "Serial");
            //mainForm.F.RegistryRemove(AuthorName + @"\easyIcon\Set", "Serial");
            if (containsSerial)
            {
                // 优先判断本地注册表数据
                string serial =RegistryTool.RegistryStrValue(AuthorName + @"\" + SoftName + @"\Set", "Serial");
                if (!serial.Equals("") && serial.Equals(MachineInfo_Serial.RegSerial(SoftName)))
                {
                    if (Timer_CheckOnlineRegister.Enabled) Timer_CheckOnlineRegister.Enabled = false;   // 停止计时查询逻辑
                    if (this.needRegCallBack != null) this.needRegCallBack("end"); //4、若已注册，则执行结束逻辑
                    return true;
                }
            }

            // 若未注册，则弹出网络提示
            bool available = System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable();
            if (!available)
            {
                MessageBox.Show("网络不可用，无法获取软件状态信息\r\n请先连接网络后，再使用此工具");
                Application.Exit(); // 自动退出应用
                return false;       // 网络未连接视作未注册用户
            }

            // 若未注册成功，在线检测是否需要注册
            bool isNeed = needRegister();
            if (this.needRegCallBack != null)   // 2、先检测是否需要注册
            {
                if (isNeed) this.needRegCallBack("yes");
                else this.needRegCallBack("no");
            }

            return !isNeed; // 若不需要注册，则视作已注册用户
        }

        /// <summary>
        /// 调用注册逻辑
        /// </summary>
        public void RegLogic()
        {
            string price = getPrice();                              // 获取软件金额
            //string machinCode = MachineInfo.SoftSerial(SoftName);   // 获取当前的机器码
            string machinCode = MachineInfo_Serial.MachineSerial();        // 获取当前的机器码

            string SeverUrl = "http://" + this.QrPayServerAddress + "/pages/pay.aspx";
            PayForm.Pay(SoftName, "软件注册", price, SeverUrl, machinCode, "author(" + AuthorName + ")", PayResult);
        }

        // 支付结果处理逻辑
        private void PayResult(string param)
        {
            string isSuccess = WebTool.getNodeData(param, "isSuccess", false).ToLower();
            if (isSuccess.Equals("true"))
            {
                string serial = MachineInfo_Serial.RegSerial(SoftName);
               RegistryTool.RegistrySave(AuthorName + @"\" + SoftName + @"\Set", "Serial", serial);
                MessageBox.Show("恭喜，您已成功注册！（即时注册）");
            }
        }
    }
}
