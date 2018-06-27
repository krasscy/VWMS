namespace VWMS
{
    partial class frmRegister
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmRegister));
            this.btnReg = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.txtMNum = new System.Windows.Forms.TextBox();
            this.txtRNum = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // btnReg
            // 
            this.btnReg.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnReg.BackgroundImage")));
            this.btnReg.Location = new System.Drawing.Point(120, 74);
            this.btnReg.Name = "btnReg";
            this.btnReg.Size = new System.Drawing.Size(61, 23);
            this.btnReg.TabIndex = 1;
            this.btnReg.UseVisualStyleBackColor = true;
            this.btnReg.Click += new System.EventHandler(this.btnReg_Click);
            // 
            // btnClose
            // 
            this.btnClose.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("btnClose.BackgroundImage")));
            this.btnClose.Location = new System.Drawing.Point(189, 74);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(64, 23);
            this.btnClose.TabIndex = 1;
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // txtMNum
            // 
            this.txtMNum.Location = new System.Drawing.Point(74, 16);
            this.txtMNum.Name = "txtMNum";
            this.txtMNum.ReadOnly = true;
            this.txtMNum.Size = new System.Drawing.Size(181, 21);
            this.txtMNum.TabIndex = 2;
            // 
            // txtRNum
            // 
            this.txtRNum.Location = new System.Drawing.Point(74, 43);
            this.txtRNum.Name = "txtRNum";
            this.txtRNum.Size = new System.Drawing.Size(181, 21);
            this.txtRNum.TabIndex = 4;
            // 
            // frmRegister
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
            this.ClientSize = new System.Drawing.Size(267, 105);
            this.Controls.Add(this.txtRNum);
            this.Controls.Add(this.txtMNum);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.btnReg);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmRegister";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "软件注册";
            this.Load += new System.EventHandler(this.frmRegister_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnReg;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.TextBox txtMNum;
        private System.Windows.Forms.TextBox txtRNum;
    }
}