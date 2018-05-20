namespace Antiplagiarism
{
    partial class MainForm
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
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.textBox2 = new System.Windows.Forms.RichTextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.LoadFileButton = new System.Windows.Forms.Button();
            this.HardcodedFileButton = new System.Windows.Forms.Button();
            this.GoogleSearchButton = new System.Windows.Forms.Button();
            this.QueryTextBox = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // richTextBox1
            // 
            this.richTextBox1.Location = new System.Drawing.Point(12, 37);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.Size = new System.Drawing.Size(502, 641);
            this.richTextBox1.TabIndex = 10;
            this.richTextBox1.Text = "";
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(533, 37);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(491, 641);
            this.textBox2.TabIndex = 11;
            this.textBox2.Text = "";
            this.textBox2.LinkClicked += new System.Windows.Forms.LinkClickedEventHandler(this.textBox2_LinkClicked);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(35, 12);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(134, 17);
            this.label3.TabIndex = 12;
            this.label3.Text = "Web search results:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(552, 12);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(156, 17);
            this.label4.TabIndex = 13;
            this.label4.Text = "Urls common text parts:";
            // 
            // LoadFileButton
            // 
            this.LoadFileButton.Location = new System.Drawing.Point(441, 689);
            this.LoadFileButton.Name = "LoadFileButton";
            this.LoadFileButton.Size = new System.Drawing.Size(151, 45);
            this.LoadFileButton.TabIndex = 14;
            this.LoadFileButton.Text = "Load File";
            this.LoadFileButton.UseVisualStyleBackColor = true;
            this.LoadFileButton.Click += new System.EventHandler(this.LoadFileButton_Click);
            // 
            // HardcodedFileButton
            // 
            this.HardcodedFileButton.Location = new System.Drawing.Point(457, 740);
            this.HardcodedFileButton.Name = "HardcodedFileButton";
            this.HardcodedFileButton.Size = new System.Drawing.Size(120, 26);
            this.HardcodedFileButton.TabIndex = 15;
            this.HardcodedFileButton.Text = "Hardcoded File";
            this.HardcodedFileButton.UseVisualStyleBackColor = true;
            this.HardcodedFileButton.Click += new System.EventHandler(this.HardcodedFileButton_Click);
            // 
            // GoogleSearchButton
            // 
            this.GoogleSearchButton.Location = new System.Drawing.Point(875, 699);
            this.GoogleSearchButton.Name = "GoogleSearchButton";
            this.GoogleSearchButton.Size = new System.Drawing.Size(93, 25);
            this.GoogleSearchButton.TabIndex = 16;
            this.GoogleSearchButton.Text = "GSearch";
            this.GoogleSearchButton.UseVisualStyleBackColor = true;
            this.GoogleSearchButton.Click += new System.EventHandler(this.GoogleSearchButton_Click);
            // 
            // QueryTextBox
            // 
            this.QueryTextBox.Location = new System.Drawing.Point(756, 700);
            this.QueryTextBox.Name = "QueryTextBox";
            this.QueryTextBox.Size = new System.Drawing.Size(100, 22);
            this.QueryTextBox.TabIndex = 17;
            this.QueryTextBox.Text = "TheFlow";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1038, 787);
            this.Controls.Add(this.QueryTextBox);
            this.Controls.Add(this.GoogleSearchButton);
            this.Controls.Add(this.HardcodedFileButton);
            this.Controls.Add(this.LoadFileButton);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.textBox2);
            this.Controls.Add(this.richTextBox1);
            this.Name = "MainForm";
            this.Text = "AntiPlagiarism";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.RichTextBox textBox2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button LoadFileButton;
        private System.Windows.Forms.Button HardcodedFileButton;
        private System.Windows.Forms.Button GoogleSearchButton;
        private System.Windows.Forms.TextBox QueryTextBox;
    }
}

