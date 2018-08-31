namespace SciPay
{
    partial class PayForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.contentWebBrowser = new System.Windows.Forms.WebBrowser();
            this.timerRefresh = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // contentWebBrowser
            // 
            this.contentWebBrowser.Dock = System.Windows.Forms.DockStyle.Fill;
            this.contentWebBrowser.Location = new System.Drawing.Point(0, 0);
            this.contentWebBrowser.MinimumSize = new System.Drawing.Size(20, 20);
            this.contentWebBrowser.Name = "contentWebBrowser";
            this.contentWebBrowser.ScrollBarsEnabled = false;
            this.contentWebBrowser.Size = new System.Drawing.Size(474, 562);
            this.contentWebBrowser.TabIndex = 0;
            // 
            // timerRefresh
            // 
            this.timerRefresh.Enabled = true;
            this.timerRefresh.Interval = 3000;
            this.timerRefresh.Tick += new System.EventHandler(this.timerRefresh_Tick);
            // 
            // PayForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(474, 562);
            this.Controls.Add(this.contentWebBrowser);
            this.Name = "PayForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "UrlForm";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.WebBrowser contentWebBrowser;
        private System.Windows.Forms.Timer timerRefresh;
    }
}