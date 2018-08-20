namespace SciTools
{
    partial class PackerFormMain
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.text_files = new System.Windows.Forms.TextBox();
            this.groupBoxSetting = new System.Windows.Forms.GroupBox();
            this.checkBox_Src = new System.Windows.Forms.CheckBox();
            this.labelOutter = new System.Windows.Forms.Label();
            this.textBox_webUrl = new System.Windows.Forms.TextBox();
            this.radioOutter = new System.Windows.Forms.RadioButton();
            this.radioInner = new System.Windows.Forms.RadioButton();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.button_Gen = new System.Windows.Forms.Button();
            this.groupBoxSetting.SuspendLayout();
            this.SuspendLayout();
            // 
            // text_files
            // 
            this.text_files.AllowDrop = true;
            this.text_files.Location = new System.Drawing.Point(12, 12);
            this.text_files.Name = "text_files";
            this.text_files.Size = new System.Drawing.Size(308, 21);
            this.text_files.TabIndex = 0;
            this.text_files.Text = "请拖动待处理的文件至此";
            this.text_files.DragDrop += new System.Windows.Forms.DragEventHandler(this.text_files_DragDrop);
            this.text_files.DragEnter += new System.Windows.Forms.DragEventHandler(this.text_files_DragEnter);
            // 
            // groupBoxSetting
            // 
            this.groupBoxSetting.Controls.Add(this.checkBox_Src);
            this.groupBoxSetting.Controls.Add(this.labelOutter);
            this.groupBoxSetting.Controls.Add(this.textBox_webUrl);
            this.groupBoxSetting.Controls.Add(this.radioOutter);
            this.groupBoxSetting.Controls.Add(this.radioInner);
            this.groupBoxSetting.Location = new System.Drawing.Point(13, 39);
            this.groupBoxSetting.Name = "groupBoxSetting";
            this.groupBoxSetting.Size = new System.Drawing.Size(307, 151);
            this.groupBoxSetting.TabIndex = 1;
            this.groupBoxSetting.TabStop = false;
            this.groupBoxSetting.Text = "设置";
            // 
            // checkBox_Src
            // 
            this.checkBox_Src.AutoSize = true;
            this.checkBox_Src.Location = new System.Drawing.Point(18, 118);
            this.checkBox_Src.Name = "checkBox_Src";
            this.checkBox_Src.Size = new System.Drawing.Size(96, 16);
            this.checkBox_Src.TabIndex = 4;
            this.checkBox_Src.Text = "输出加壳源码";
            this.checkBox_Src.UseVisualStyleBackColor = true;
            // 
            // labelOutter
            // 
            this.labelOutter.AutoSize = true;
            this.labelOutter.Location = new System.Drawing.Point(16, 80);
            this.labelOutter.Name = "labelOutter";
            this.labelOutter.Size = new System.Drawing.Size(83, 12);
            this.labelOutter.TabIndex = 3;
            this.labelOutter.Text = "外部资源网址:";
            // 
            // textBox_webUrl
            // 
            this.textBox_webUrl.Enabled = false;
            this.textBox_webUrl.Location = new System.Drawing.Point(105, 77);
            this.textBox_webUrl.Name = "textBox_webUrl";
            this.textBox_webUrl.Size = new System.Drawing.Size(172, 21);
            this.textBox_webUrl.TabIndex = 2;
            // 
            // radioOutter
            // 
            this.radioOutter.AutoSize = true;
            this.radioOutter.Location = new System.Drawing.Point(18, 55);
            this.radioOutter.Name = "radioOutter";
            this.radioOutter.Size = new System.Drawing.Size(269, 16);
            this.radioOutter.TabIndex = 1;
            this.radioOutter.Text = "Outter (EXE作为附加资源文件，通过壳调用）";
            this.radioOutter.UseVisualStyleBackColor = true;
            this.radioOutter.CheckedChanged += new System.EventHandler(this.radio_CheckedChanged);
            // 
            // radioInner
            // 
            this.radioInner.AutoSize = true;
            this.radioInner.Checked = true;
            this.radioInner.Location = new System.Drawing.Point(18, 33);
            this.radioInner.Name = "radioInner";
            this.radioInner.Size = new System.Drawing.Size(263, 16);
            this.radioInner.TabIndex = 0;
            this.radioInner.TabStop = true;
            this.radioInner.Text = "Inner  (EXE作为内部资源，加壳为单个文件)";
            this.radioInner.UseVisualStyleBackColor = true;
            this.radioInner.CheckedChanged += new System.EventHandler(this.radio_CheckedChanged);
            // 
            // button_Gen
            // 
            this.button_Gen.Location = new System.Drawing.Point(13, 196);
            this.button_Gen.Name = "button_Gen";
            this.button_Gen.Size = new System.Drawing.Size(75, 23);
            this.button_Gen.TabIndex = 2;
            this.button_Gen.Text = "生成";
            this.button_Gen.UseVisualStyleBackColor = true;
            this.button_Gen.Click += new System.EventHandler(this.button_Gen_Click);
            // 
            // FormMain
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(332, 226);
            this.Controls.Add(this.button_Gen);
            this.Controls.Add(this.groupBoxSetting);
            this.Controls.Add(this.text_files);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "FormMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Packer";
            this.groupBoxSetting.ResumeLayout(false);
            this.groupBoxSetting.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox text_files;
        private System.Windows.Forms.GroupBox groupBoxSetting;
        private System.Windows.Forms.RadioButton radioOutter;
        private System.Windows.Forms.RadioButton radioInner;
        private System.Windows.Forms.Label labelOutter;
        private System.Windows.Forms.TextBox textBox_webUrl;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.CheckBox checkBox_Src;
        private System.Windows.Forms.Button button_Gen;
    }
}

