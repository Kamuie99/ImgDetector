namespace ImgDetector
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
            this.label1 = new System.Windows.Forms.Label();
            this.btnDetect = new System.Windows.Forms.Button();
            this.btnLoadImage = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.trackBarThreshold = new System.Windows.Forms.TrackBar();
            this.trackBarBlur = new System.Windows.Forms.TrackBar();
            this.trackBarApprox = new System.Windows.Forms.TrackBar();
            this.labelThreshold = new System.Windows.Forms.Label();
            this.labelBlur = new System.Windows.Forms.Label();
            this.labelApprox = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarThreshold)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarBlur)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarApprox)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Roboto", 12F);
            this.label1.Location = new System.Drawing.Point(48, 56);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(78, 21);
            this.label1.TabIndex = 1;
            this.label1.Text = "자재 사진";
            // 
            // btnDetect
            // 
            this.btnDetect.Location = new System.Drawing.Point(407, 56);
            this.btnDetect.Name = "btnDetect";
            this.btnDetect.Size = new System.Drawing.Size(94, 27);
            this.btnDetect.TabIndex = 3;
            this.btnDetect.Text = "모서리 감지";
            this.btnDetect.UseVisualStyleBackColor = true;
            this.btnDetect.Click += new System.EventHandler(this.btnDetect_Click);
            // 
            // btnLoadImage
            // 
            this.btnLoadImage.Location = new System.Drawing.Point(198, 58);
            this.btnLoadImage.Name = "btnLoadImage";
            this.btnLoadImage.Size = new System.Drawing.Size(75, 23);
            this.btnLoadImage.TabIndex = 2;
            this.btnLoadImage.Text = "찾아보기";
            this.btnLoadImage.UseVisualStyleBackColor = true;
            this.btnLoadImage.Click += new System.EventHandler(this.btnLoadImage_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(52, 87);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(449, 431);
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // trackBarThreshold
            // 
            this.trackBarThreshold.Location = new System.Drawing.Point(579, 158);
            this.trackBarThreshold.Maximum = 255;
            this.trackBarThreshold.Name = "trackBarThreshold";
            this.trackBarThreshold.Size = new System.Drawing.Size(202, 45);
            this.trackBarThreshold.TabIndex = 4;
            this.trackBarThreshold.TickFrequency = 10;
            this.trackBarThreshold.Value = 180;
            // 
            // trackBarBlur
            // 
            this.trackBarBlur.Location = new System.Drawing.Point(579, 288);
            this.trackBarBlur.Maximum = 15;
            this.trackBarBlur.Minimum = 1;
            this.trackBarBlur.Name = "trackBarBlur";
            this.trackBarBlur.Size = new System.Drawing.Size(202, 45);
            this.trackBarBlur.TabIndex = 5;
            this.trackBarBlur.TickFrequency = 2;
            this.trackBarBlur.Value = 5;
            // 
            // trackBarApprox
            // 
            this.trackBarApprox.Location = new System.Drawing.Point(579, 377);
            this.trackBarApprox.Maximum = 50;
            this.trackBarApprox.Name = "trackBarApprox";
            this.trackBarApprox.Size = new System.Drawing.Size(202, 45);
            this.trackBarApprox.TabIndex = 6;
            this.trackBarApprox.TickFrequency = 5;
            this.trackBarApprox.Value = 10;
            // 
            // labelThreshold
            // 
            this.labelThreshold.AutoSize = true;
            this.labelThreshold.Font = new System.Drawing.Font("Roboto", 12F);
            this.labelThreshold.Location = new System.Drawing.Point(586, 120);
            this.labelThreshold.Name = "labelThreshold";
            this.labelThreshold.Size = new System.Drawing.Size(80, 21);
            this.labelThreshold.TabIndex = 7;
            this.labelThreshold.Text = "Threshold";
            // 
            // labelBlur
            // 
            this.labelBlur.AutoSize = true;
            this.labelBlur.Font = new System.Drawing.Font("Roboto", 12F);
            this.labelBlur.Location = new System.Drawing.Point(586, 248);
            this.labelBlur.Name = "labelBlur";
            this.labelBlur.Size = new System.Drawing.Size(38, 21);
            this.labelBlur.TabIndex = 8;
            this.labelBlur.Text = "Blur";
            // 
            // labelApprox
            // 
            this.labelApprox.AutoSize = true;
            this.labelApprox.Font = new System.Drawing.Font("Roboto", 12F);
            this.labelApprox.Location = new System.Drawing.Point(583, 336);
            this.labelApprox.Name = "labelApprox";
            this.labelApprox.Size = new System.Drawing.Size(59, 21);
            this.labelApprox.TabIndex = 9;
            this.labelApprox.Text = "approx";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(585, 273);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(81, 12);
            this.label2.TabIndex = 10;
            this.label2.Text = "가우시안 블러";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(588, 143);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(81, 12);
            this.label3.TabIndex = 11;
            this.label3.Text = "이진화 임계값";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(586, 360);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(121, 12);
            this.label4.TabIndex = 12;
            this.label4.Text = "다각형 근사화 정확도";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(905, 554);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.labelApprox);
            this.Controls.Add(this.labelBlur);
            this.Controls.Add(this.labelThreshold);
            this.Controls.Add(this.trackBarApprox);
            this.Controls.Add(this.trackBarBlur);
            this.Controls.Add(this.trackBarThreshold);
            this.Controls.Add(this.btnDetect);
            this.Controls.Add(this.btnLoadImage);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.pictureBox1);
            this.Name = "Form1";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarThreshold)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarBlur)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarApprox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnDetect;
        private System.Windows.Forms.Button btnLoadImage;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.TrackBar trackBarThreshold;
        private System.Windows.Forms.TrackBar trackBarBlur;
        private System.Windows.Forms.TrackBar trackBarApprox;
        private System.Windows.Forms.Label labelThreshold;
        private System.Windows.Forms.Label labelBlur;
        private System.Windows.Forms.Label labelApprox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
    }
}

