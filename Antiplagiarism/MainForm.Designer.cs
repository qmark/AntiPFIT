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
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.button3 = new System.Windows.Forms.Button();
            this.progressTextBox = new System.Windows.Forms.TextBox();
            this.tasksCreatedTextBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.compareTextButton = new System.Windows.Forms.Button();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(23, 697);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(82, 31);
            this.button1.TabIndex = 1;
            this.button1.Text = "OCR";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Enabled = false;
            this.button2.Location = new System.Drawing.Point(812, 697);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(94, 31);
            this.button2.TabIndex = 2;
            this.button2.Text = "GSearch";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(655, 13);
            this.textBox2.Multiline = true;
            this.textBox2.Name = "textBox2";
            this.textBox2.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBox2.Size = new System.Drawing.Size(349, 665);
            this.textBox2.TabIndex = 3;
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(172, 697);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(131, 31);
            this.button3.TabIndex = 4;
            this.button3.Text = "From Word Doc";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_ClickAsync);
            // 
            // progressTextBox
            // 
            this.progressTextBox.Location = new System.Drawing.Point(419, 725);
            this.progressTextBox.Name = "progressTextBox";
            this.progressTextBox.Size = new System.Drawing.Size(51, 22);
            this.progressTextBox.TabIndex = 5;
            // 
            // tasksCreatedTextBox
            // 
            this.tasksCreatedTextBox.Location = new System.Drawing.Point(419, 694);
            this.tasksCreatedTextBox.Name = "tasksCreatedTextBox";
            this.tasksCreatedTextBox.Size = new System.Drawing.Size(51, 22);
            this.tasksCreatedTextBox.TabIndex = 6;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(309, 697);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(93, 17);
            this.label1.TabIndex = 7;
            this.label1.Text = "tasks created";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(309, 725);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(101, 17);
            this.label2.TabIndex = 8;
            this.label2.Text = "tasks progress";
            // 
            // compareTextButton
            // 
            this.compareTextButton.Location = new System.Drawing.Point(531, 697);
            this.compareTextButton.Name = "compareTextButton";
            this.compareTextButton.Size = new System.Drawing.Size(137, 31);
            this.compareTextButton.TabIndex = 9;
            this.compareTextButton.Text = "Compare Texts";
            this.compareTextButton.UseVisualStyleBackColor = true;
            this.compareTextButton.Click += new System.EventHandler(this.CompareTextButton_Click);
            // 
            // richTextBox1
            // 
            this.richTextBox1.Location = new System.Drawing.Point(12, 13);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.Size = new System.Drawing.Size(620, 665);
            this.richTextBox1.TabIndex = 10;
            this.richTextBox1.Text = "";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1036, 757);
            this.Controls.Add(this.richTextBox1);
            this.Controls.Add(this.compareTextButton);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.tasksCreatedTextBox);
            this.Controls.Add(this.progressTextBox);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.textBox2);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Name = "Form1";
            this.Text = "AntiPFIT";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.TextBox progressTextBox;
        private System.Windows.Forms.TextBox tasksCreatedTextBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button compareTextButton;
        private System.Windows.Forms.RichTextBox richTextBox1;
    }
}

