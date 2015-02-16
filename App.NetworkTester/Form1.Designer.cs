namespace App.NetworkTester
{
    partial class Form1
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
            this.ReceiveBox = new System.Windows.Forms.TextBox();
            this.SendBox = new System.Windows.Forms.TextBox();
            this.HostButton = new System.Windows.Forms.Button();
            this.ClientSend = new System.Windows.Forms.Button();
            this.ClientSendBox = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // ReceiveBox
            // 
            this.ReceiveBox.Location = new System.Drawing.Point(351, 125);
            this.ReceiveBox.Multiline = true;
            this.ReceiveBox.Name = "ReceiveBox";
            this.ReceiveBox.Size = new System.Drawing.Size(295, 146);
            this.ReceiveBox.TabIndex = 0;
            // 
            // SendBox
            // 
            this.SendBox.Location = new System.Drawing.Point(12, 125);
            this.SendBox.Multiline = true;
            this.SendBox.Name = "SendBox";
            this.SendBox.Size = new System.Drawing.Size(333, 146);
            this.SendBox.TabIndex = 1;
            // 
            // HostButton
            // 
            this.HostButton.Location = new System.Drawing.Point(571, 12);
            this.HostButton.Name = "HostButton";
            this.HostButton.Size = new System.Drawing.Size(75, 23);
            this.HostButton.TabIndex = 2;
            this.HostButton.Text = "Host";
            this.HostButton.UseVisualStyleBackColor = true;
            // 
            // ClientSend
            // 
            this.ClientSend.Location = new System.Drawing.Point(270, 96);
            this.ClientSend.Name = "ClientSend";
            this.ClientSend.Size = new System.Drawing.Size(75, 23);
            this.ClientSend.TabIndex = 3;
            this.ClientSend.Text = "Send";
            this.ClientSend.UseVisualStyleBackColor = true;
            // 
            // ClientSendBox
            // 
            this.ClientSendBox.Location = new System.Drawing.Point(12, 96);
            this.ClientSendBox.Name = "ClientSendBox";
            this.ClientSendBox.Size = new System.Drawing.Size(252, 20);
            this.ClientSendBox.TabIndex = 4;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(658, 283);
            this.Controls.Add(this.ClientSendBox);
            this.Controls.Add(this.ClientSend);
            this.Controls.Add(this.HostButton);
            this.Controls.Add(this.SendBox);
            this.Controls.Add(this.ReceiveBox);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox ReceiveBox;
        private System.Windows.Forms.TextBox SendBox;
        private System.Windows.Forms.Button HostButton;
        private System.Windows.Forms.Button ClientSend;
        private System.Windows.Forms.TextBox ClientSendBox;
    }
}

