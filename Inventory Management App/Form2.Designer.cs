using System.Xml.Linq;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TaskbarClock;

namespace Inventory_Management_App
{
    partial class DetailForm2
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
        private void InitializeComponent() //string time, string quantity, string comment
        {
            this.InsertImagebutton = new System.Windows.Forms.Button();
            this.TimeLabel = new System.Windows.Forms.Label();
            this.QuantityLavel = new System.Windows.Forms.Label();
            this.CommentLavel = new System.Windows.Forms.Label();
            this.Deletebutton = new System.Windows.Forms.Button();
            this.SaveButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // InsertImagebutton
            // 
            this.InsertImagebutton.Location = new System.Drawing.Point(195, 25);
            this.InsertImagebutton.Name = "InsertImagebutton";
            this.InsertImagebutton.Size = new System.Drawing.Size(100, 25);
            this.InsertImagebutton.TabIndex = 0;
            this.InsertImagebutton.Text = "画像を挿入";
            this.InsertImagebutton.UseVisualStyleBackColor = true;
            this.InsertImagebutton.Click += new System.EventHandler(this.InsertImageButton_Click);
            // 
            // TimeLabel
            // 
            this.TimeLabel.AutoSize = true;
            this.TimeLabel.Location = new System.Drawing.Point(10, 10);
            this.TimeLabel.Name = "TimeLabel";
            this.TimeLabel.Size = new System.Drawing.Size(0, 15);
            this.TimeLabel.TabIndex = 1;
            // 
            // QuantityLavel
            // 
            this.QuantityLavel.AutoSize = true;
            this.QuantityLavel.Location = new System.Drawing.Point(12, 25);
            this.QuantityLavel.Name = "QuantityLavel";
            this.QuantityLavel.Size = new System.Drawing.Size(0, 15);
            this.QuantityLavel.TabIndex = 2;
            // 
            // CommentLavel
            // 
            this.CommentLavel.AutoSize = true;
            this.CommentLavel.Location = new System.Drawing.Point(12, 40);
            this.CommentLavel.Name = "CommentLavel";
            this.CommentLavel.Size = new System.Drawing.Size(0, 15);
            this.CommentLavel.TabIndex = 3;
            // 
            // Deletebutton
            // 
            this.Deletebutton.Location = new System.Drawing.Point(344, 25);
            this.Deletebutton.Name = "Deletebutton";
            this.Deletebutton.Size = new System.Drawing.Size(97, 25);
            this.Deletebutton.TabIndex = 4;
            this.Deletebutton.Text = "画像を削除";
            this.Deletebutton.UseVisualStyleBackColor = true;
            this.Deletebutton.Click += new System.EventHandler(this.DeleteButton_Click);
            // 
            // SaveButton
            // 
            this.SaveButton.Location = new System.Drawing.Point(497, 25);
            this.SaveButton.Name = "SaveButton";
            this.SaveButton.Size = new System.Drawing.Size(97, 25);
            this.SaveButton.TabIndex = 5;
            this.SaveButton.Text = "画像を保存";
            this.SaveButton.UseVisualStyleBackColor = true;
            this.SaveButton.Click += new System.EventHandler(this.SaveButton_Click);
            // 
            // DetailForm2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.SaveButton);
            this.Controls.Add(this.Deletebutton);
            this.Controls.Add(this.CommentLavel);
            this.Controls.Add(this.QuantityLavel);
            this.Controls.Add(this.TimeLabel);
            this.Controls.Add(this.InsertImagebutton);
            this.Name = "DetailForm2";
            this.Text = "DetailForm2";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        // endregion
        private System.Windows.Forms.Button InsertImagebutton;
        private System.Windows.Forms.Label TimeLabel;
        private System.Windows.Forms.Label QuantityLavel;
        private System.Windows.Forms.Label CommentLavel;
        private System.Windows.Forms.Button Deletebutton;
        private System.Windows.Forms.Button SaveButton;
    }
}