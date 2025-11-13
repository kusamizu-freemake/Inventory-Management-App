using System.Runtime.Hosting;

namespace Inventory_Management_App
{
    partial class InventoryQuantityForm
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
            this.QuantityLabel = new System.Windows.Forms.Label();
            this.PlusButton = new System.Windows.Forms.Button();
            this.MinusButton = new System.Windows.Forms.Button();
            this.CountTextBox = new System.Windows.Forms.TextBox();
            this.TimeLabel = new System.Windows.Forms.Label();
            this.CommentTextBox = new System.Windows.Forms.TextBox();
            this.AddButton = new System.Windows.Forms.Button();
            this.TotalQuantitySelectedButton = new System.Windows.Forms.Button();
            this.ClearButton = new System.Windows.Forms.Button();
            this.UpdateButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // QuantityLabel
            // 
            this.QuantityLabel.AutoSize = true;
            this.QuantityLabel.Location = new System.Drawing.Point(30, 30);
            this.QuantityLabel.Name = "QuantityLabel";
            this.QuantityLabel.Size = new System.Drawing.Size(37, 15);
            this.QuantityLabel.TabIndex = 0;
            this.QuantityLabel.Text = "数量";
            // 
            // PlusButton
            // 
            this.PlusButton.Location = new System.Drawing.Point(200, 27);
            this.PlusButton.Name = "PlusButton";
            this.PlusButton.Size = new System.Drawing.Size(40, 27);
            this.PlusButton.TabIndex = 2;
            this.PlusButton.Text = "+";
            this.PlusButton.UseVisualStyleBackColor = true;
            // 
            // MinusButton
            // 
            this.MinusButton.Location = new System.Drawing.Point(250, 27);
            this.MinusButton.Name = "MinusButton";
            this.MinusButton.Size = new System.Drawing.Size(40, 27);
            this.MinusButton.TabIndex = 3;
            this.MinusButton.Text = "-";
            this.MinusButton.UseVisualStyleBackColor = true;
            // 
            // CountTextBox
            // 
            this.CountTextBox.Location = new System.Drawing.Point(85, 28);
            this.CountTextBox.Name = "CountTextBox";
            this.CountTextBox.Size = new System.Drawing.Size(100, 22);
            this.CountTextBox.TabIndex = 1;
            // 
            // TimeLabel
            // 
            this.TimeLabel.AutoSize = true;
            this.TimeLabel.Location = new System.Drawing.Point(30, 65);
            this.TimeLabel.Name = "TimeLabel";
            this.TimeLabel.Size = new System.Drawing.Size(0, 15);
            this.TimeLabel.TabIndex = 4;
            // 
            // CommentTextBox
            // 
            this.CommentTextBox.Location = new System.Drawing.Point(95, 56);
            this.CommentTextBox.Multiline = true;
            this.CommentTextBox.Name = "CommentTextBox";
            this.CommentTextBox.Size = new System.Drawing.Size(145, 32);
            this.CommentTextBox.TabIndex = 5;
            this.CommentTextBox.Text = "コメント";
            // 
            // AddButton
            // 
            this.AddButton.Location = new System.Drawing.Point(250, 56);
            this.AddButton.Name = "AddButton";
            this.AddButton.Size = new System.Drawing.Size(100, 30);
            this.AddButton.TabIndex = 7;
            this.AddButton.Text = "追加";
            this.AddButton.UseVisualStyleBackColor = true;
            // 
            // TotalQuantitySelectedButton
            // 
            this.TotalQuantitySelectedButton.Location = new System.Drawing.Point(200, 504);
            this.TotalQuantitySelectedButton.Name = "TotalQuantitySelectedButton";
            this.TotalQuantitySelectedButton.Size = new System.Drawing.Size(173, 33);
            this.TotalQuantitySelectedButton.TabIndex = 8;
            this.TotalQuantitySelectedButton.Text = "選択された合計数量";
            this.TotalQuantitySelectedButton.UseVisualStyleBackColor = true;
            // 
            // ClearButton
            // 
            this.ClearButton.Location = new System.Drawing.Point(33, 504);
            this.ClearButton.Name = "ClearButton";
            this.ClearButton.Size = new System.Drawing.Size(117, 33);
            this.ClearButton.TabIndex = 9;
            this.ClearButton.Text = "クリア";
            this.ClearButton.UseVisualStyleBackColor = true;
            // 
            // UpdateButton
            // 
            this.UpdateButton.Location = new System.Drawing.Point(536, 30);
            this.UpdateButton.Name = "UpdateButton";
            this.UpdateButton.Size = new System.Drawing.Size(149, 30);
            this.UpdateButton.TabIndex = 10;
            this.UpdateButton.Text = "更新";
            this.UpdateButton.UseVisualStyleBackColor = true;
            // 
            // InventoryQuantityForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 656);
            this.Controls.Add(this.UpdateButton);
            this.Controls.Add(this.ClearButton);
            this.Controls.Add(this.TotalQuantitySelectedButton);
            this.Controls.Add(this.AddButton);
            this.Controls.Add(this.CommentTextBox);
            this.Controls.Add(this.TimeLabel);
            this.Controls.Add(this.CountTextBox);
            this.Controls.Add(this.MinusButton);
            this.Controls.Add(this.PlusButton);
            this.Controls.Add(this.QuantityLabel);
            this.Name = "InventoryQuantityForm";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label QuantityLabel;
        private System.Windows.Forms.Button PlusButton;
        private System.Windows.Forms.Button MinusButton;
        private System.Windows.Forms.TextBox CountTextBox;
        private System.Windows.Forms.Label TimeLabel;
        private System.Windows.Forms.TextBox CommentTextBox;
        private System.Windows.Forms.Button AddButton;
        private System.Windows.Forms.Button TotalQuantitySelectedButton;
        private System.Windows.Forms.Button ClearButton;
        private System.Windows.Forms.Button UpdateButton;
    }
}

