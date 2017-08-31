namespace RSHelper
{
    partial class ObtainImageResources
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
            this.cbScreens = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.pbTopLeftInfo = new System.Windows.Forms.PictureBox();
            this.pbMapInfo = new System.Windows.Forms.PictureBox();
            this.pbInventoryInfo = new System.Windows.Forms.PictureBox();
            this.pbChatInfo = new System.Windows.Forms.PictureBox();
            this.btnSetMapCircle = new System.Windows.Forms.Button();
            this.btnSetAgilityCircle = new System.Windows.Forms.Button();
            this.btnSetHealthCircle = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pbTopLeftInfo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbMapInfo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbInventoryInfo)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbChatInfo)).BeginInit();
            this.SuspendLayout();
            // 
            // cbScreens
            // 
            this.cbScreens.FormattingEnabled = true;
            this.cbScreens.Location = new System.Drawing.Point(93, 688);
            this.cbScreens.Name = "cbScreens";
            this.cbScreens.Size = new System.Drawing.Size(835, 21);
            this.cbScreens.TabIndex = 0;
            this.cbScreens.SelectedIndexChanged += new System.EventHandler(this.cbScreens_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 691);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(75, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Game Screen:";
            // 
            // pbTopLeftInfo
            // 
            this.pbTopLeftInfo.Location = new System.Drawing.Point(15, 12);
            this.pbTopLeftInfo.Name = "pbTopLeftInfo";
            this.pbTopLeftInfo.Size = new System.Drawing.Size(553, 87);
            this.pbTopLeftInfo.TabIndex = 2;
            this.pbTopLeftInfo.TabStop = false;
            // 
            // pbMapInfo
            // 
            this.pbMapInfo.Location = new System.Drawing.Point(700, 12);
            this.pbMapInfo.Name = "pbMapInfo";
            this.pbMapInfo.Size = new System.Drawing.Size(228, 217);
            this.pbMapInfo.TabIndex = 3;
            this.pbMapInfo.TabStop = false;
            // 
            // pbInventoryInfo
            // 
            this.pbInventoryInfo.Location = new System.Drawing.Point(655, 296);
            this.pbInventoryInfo.Name = "pbInventoryInfo";
            this.pbInventoryInfo.Size = new System.Drawing.Size(273, 370);
            this.pbInventoryInfo.TabIndex = 4;
            this.pbInventoryInfo.TabStop = false;
            // 
            // pbChatInfo
            // 
            this.pbChatInfo.Location = new System.Drawing.Point(12, 494);
            this.pbChatInfo.Name = "pbChatInfo";
            this.pbChatInfo.Size = new System.Drawing.Size(556, 163);
            this.pbChatInfo.TabIndex = 5;
            this.pbChatInfo.TabStop = false;
            // 
            // btnSetMapCircle
            // 
            this.btnSetMapCircle.Location = new System.Drawing.Point(590, 12);
            this.btnSetMapCircle.Name = "btnSetMapCircle";
            this.btnSetMapCircle.Size = new System.Drawing.Size(104, 23);
            this.btnSetMapCircle.TabIndex = 6;
            this.btnSetMapCircle.Text = "Set Map Circle";
            this.btnSetMapCircle.UseVisualStyleBackColor = true;
            this.btnSetMapCircle.Click += new System.EventHandler(this.btnSetMapCircle_Click);
            // 
            // btnSetAgilityCircle
            // 
            this.btnSetAgilityCircle.Location = new System.Drawing.Point(590, 41);
            this.btnSetAgilityCircle.Name = "btnSetAgilityCircle";
            this.btnSetAgilityCircle.Size = new System.Drawing.Size(104, 23);
            this.btnSetAgilityCircle.TabIndex = 6;
            this.btnSetAgilityCircle.Text = "Set Agility Circle";
            this.btnSetAgilityCircle.UseVisualStyleBackColor = true;
            // 
            // btnSetHealthCircle
            // 
            this.btnSetHealthCircle.Location = new System.Drawing.Point(590, 70);
            this.btnSetHealthCircle.Name = "btnSetHealthCircle";
            this.btnSetHealthCircle.Size = new System.Drawing.Size(104, 23);
            this.btnSetHealthCircle.TabIndex = 6;
            this.btnSetHealthCircle.Text = "Set Health Circle";
            this.btnSetHealthCircle.UseVisualStyleBackColor = true;
            // 
            // ObtainImageResources
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(945, 713);
            this.Controls.Add(this.btnSetHealthCircle);
            this.Controls.Add(this.btnSetAgilityCircle);
            this.Controls.Add(this.btnSetMapCircle);
            this.Controls.Add(this.pbChatInfo);
            this.Controls.Add(this.pbInventoryInfo);
            this.Controls.Add(this.pbMapInfo);
            this.Controls.Add(this.pbTopLeftInfo);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cbScreens);
            this.Name = "ObtainImageResources";
            this.Text = "ObtainResources";
            ((System.ComponentModel.ISupportInitialize)(this.pbTopLeftInfo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbMapInfo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbInventoryInfo)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbChatInfo)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox cbScreens;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.PictureBox pbTopLeftInfo;
        private System.Windows.Forms.PictureBox pbMapInfo;
        private System.Windows.Forms.PictureBox pbInventoryInfo;
        private System.Windows.Forms.PictureBox pbChatInfo;
        private System.Windows.Forms.Button btnSetMapCircle;
        private System.Windows.Forms.Button btnSetAgilityCircle;
        private System.Windows.Forms.Button btnSetHealthCircle;
    }
}