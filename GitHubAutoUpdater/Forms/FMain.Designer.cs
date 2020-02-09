namespace GitHubAutoUpdater.Forms
{
    partial class FMain
    {
        /// <summary>
        /// 必要なデザイナー変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージド リソースを破棄する場合は true を指定し、その他の場合は false を指定します。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows フォーム デザイナーで生成されたコード

        /// <summary>
        /// デザイナー サポートに必要なメソッドです。このメソッドの内容を
        /// コード エディターで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FMain));
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.btnUpdate = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.txtLocalVersion = new System.Windows.Forms.TextBox();
            this.txtLatestVersion = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 30);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(85, 15);
            this.label1.TabIndex = 1;
            this.label1.Text = "現在のバージョン";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(9, 60);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(85, 15);
            this.label2.TabIndex = 3;
            this.label2.Text = "最新のバージョン";
            // 
            // btnUpdate
            // 
            this.btnUpdate.Enabled = false;
            this.btnUpdate.Location = new System.Drawing.Point(12, 98);
            this.btnUpdate.Name = "btnUpdate";
            this.btnUpdate.Size = new System.Drawing.Size(223, 23);
            this.btnUpdate.TabIndex = 5;
            this.btnUpdate.Text = "最新をダウンロードしてアップデート";
            this.btnUpdate.UseVisualStyleBackColor = true;
            this.btnUpdate.Click += new System.EventHandler(this.btnUpdate_Click);
            // 
            // btnClose
            // 
            this.btnClose.Location = new System.Drawing.Point(241, 98);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(158, 23);
            this.btnClose.TabIndex = 6;
            this.btnClose.Text = "何もしないで終了する";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // txtLocalVersion
            // 
            this.txtLocalVersion.Location = new System.Drawing.Point(100, 27);
            this.txtLocalVersion.Name = "txtLocalVersion";
            this.txtLocalVersion.ReadOnly = true;
            this.txtLocalVersion.Size = new System.Drawing.Size(299, 23);
            this.txtLocalVersion.TabIndex = 2;
            this.txtLocalVersion.Text = "1.0.0.0";
            // 
            // txtLatestVersion
            // 
            this.txtLatestVersion.Location = new System.Drawing.Point(100, 57);
            this.txtLatestVersion.Name = "txtLatestVersion";
            this.txtLatestVersion.ReadOnly = true;
            this.txtLatestVersion.Size = new System.Drawing.Size(299, 23);
            this.txtLatestVersion.TabIndex = 4;
            this.txtLatestVersion.Text = "1.0.0.1";
            // 
            // FMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(413, 142);
            this.Controls.Add(this.txtLatestVersion);
            this.Controls.Add(this.txtLocalVersion);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.btnUpdate);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Font = new System.Drawing.Font("Yu Gothic UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.MaximizeBox = false;
            this.Name = "FMain";
            this.Text = "アプリケーション名";
            this.TopMost = true;
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FMain_FormClosed);
            this.Load += new System.EventHandler(this.FMain_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnUpdate;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.TextBox txtLocalVersion;
        private System.Windows.Forms.TextBox txtLatestVersion;
    }
}

