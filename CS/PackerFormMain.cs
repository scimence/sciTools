using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SciTools
{
    /// <summary>
    /// Packer主界面类
    /// </summary>
    public partial class PackerFormMain : Form
    {
        public PackerFormMain()
        {
            InitializeComponent();
        }

        private void text_files_DragEnter(object sender, DragEventArgs e)
        {
            DragDropTool.Form_DragEnter(sender, e);
        }

        private void text_files_DragDrop(object sender, DragEventArgs e)
        {
            DragDropTool.Form_DragDrop(sender, e);
        }

        private void radio_CheckedChanged(object sender, EventArgs e)
        {
            textBox_webUrl.Enabled = radioOutter.Checked;
        }

        private void button_Gen_Click(object sender, EventArgs e)
        {
            // 参数信息解析
            bool isInner = radioInner.Checked;            // 是否作为内部EXE资源
            bool outSoucreCode = checkBox_Src.Checked;    // 是否输出壳源码
            string WebDataUrl = "";                       // 设置EXE资源载入网址
            if (outSoucreCode) WebDataUrl = textBox_webUrl.Text.Trim();

            string[] files = text_files.Text.Split(';');  // 待加壳文件
            foreach (string file in files)
            {
                PackerTool.PackeEXE(file, isInner, WebDataUrl, outSoucreCode);  // 执行加壳
            }
        }
    }
}
