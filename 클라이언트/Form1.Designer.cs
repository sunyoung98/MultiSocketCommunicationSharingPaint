namespace WindowsFormsApp5
{
    partial class Form1
    {
        /// <summary>
        /// 필수 디자이너 변수입니다.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 사용 중인 모든 리소스를 정리합니다.
        /// </summary>
        /// <param name="disposing">관리되는 리소스를 삭제해야 하면 true이고, 그렇지 않으면 false입니다.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 디자이너에서 생성한 코드

        /// <summary>
        /// 디자이너 지원에 필요한 메서드입니다. 
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마세요.
        /// </summary>
        private void InitializeComponent()
        {
            this.server = new System.Windows.Forms.Button();
            this.port = new System.Windows.Forms.TextBox();
            this.ip = new System.Windows.Forms.TextBox();
            this.id = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // server
            // 
            this.server.Location = new System.Drawing.Point(113, 74);
            this.server.Name = "server";
            this.server.Size = new System.Drawing.Size(91, 23);
            this.server.TabIndex = 5;
            this.server.Text = "접속";
            this.server.UseVisualStyleBackColor = true;
            this.server.Click += new System.EventHandler(this.server_Click);
            // 
            // port
            // 
            this.port.Location = new System.Drawing.Point(196, 12);
            this.port.Name = "port";
            this.port.Size = new System.Drawing.Size(89, 25);
            this.port.TabIndex = 4;
            // 
            // ip
            // 
            this.ip.Location = new System.Drawing.Point(12, 12);
            this.ip.Name = "ip";
            this.ip.Size = new System.Drawing.Size(164, 25);
            this.ip.TabIndex = 3;
            // 
            // id
            // 
            this.id.Location = new System.Drawing.Point(116, 43);
            this.id.Name = "id";
            this.id.Size = new System.Drawing.Size(164, 25);
            this.id.TabIndex = 6;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(42, 53);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(25, 15);
            this.label1.TabIndex = 7;
            this.label1.Text = "ID:";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(319, 109);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.id);
            this.Controls.Add(this.server);
            this.Controls.Add(this.port);
            this.Controls.Add(this.ip);
            this.Name = "Form1";
            this.Text = "세계그림판클라이언트";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button server;
        private System.Windows.Forms.TextBox port;
        private System.Windows.Forms.TextBox ip;
        private System.Windows.Forms.TextBox id;
        private System.Windows.Forms.Label label1;
    }
}

