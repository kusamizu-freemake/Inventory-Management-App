using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TaskbarClock;

namespace Inventory_Management_App
{
    public partial class Form2 : Form
    {
        // 画像を管理（現在は1枚のみ）
        private PictureBox currentPictureBox = null;

        // パネルを使用してスクロール可能にする
        private Panel imagePanel;

        //Form1から渡されたデータを受け取るコンストラクタ
        public Form2(string time, string quantity, string comment)
        {
            // 画像表示用パネルの初期化
            InitializeComponent();

            // タイトルの設定
            this.Text = "詳細情報";

            // 渡されたデータをラベルに表示
            label1.Text = $"時刻: {time}";
            label2.Text = $"数量: {quantity}";
            label3.Text = $"コメント: {comment}";

            // 画像表示用パネルの初期化
            InitializeImagePanel();
        }

        // 画像表示用パネルの初期化
        private void InitializeImagePanel()
        {
            //
            imagePanel = new Panel();
            imagePanel.Location = new Point(140, 20);
            imagePanel.Size = new Size(760, 280);
            imagePanel.AutoScroll = false; // スクロール不要（1枚のみ）
            imagePanel.BorderStyle = BorderStyle.FixedSingle;
            this.Controls.Add(imagePanel);
        }
        // 画像を挿入するボタンを作成
        private void InsertImageButton_Click(object sender, EventArgs e)
        {
            // 既に画像がある場合は警告
            if (currentPictureBox != null)
            {
                var result = MessageBox.Show("既に画像が挿入されています。新しい画像に置き換えますか？",
                    "確認", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.No)
                {
                    return;
                }

                // 既存の画像を削除
                RemoveCurrentImage();
            }

            // OpenFileDialogを使用して画像ファイルを選択
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Title = "画像を選択";
                openFileDialog.Filter = "画像ファイル (*.jpg;*.jpeg;*.png;*.bmp)|*.jpg;*.jpeg;*.png;*.bmp";
                openFileDialog.Multiselect = false; // 1枚のみ選択
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    foreach (string imagePath in openFileDialog.FileNames)
                    {
                        AddImageToPanel(imagePath);
                    }
                }
            }
        }

        // パネルに画像を追加
        private void AddImageToPanel(string imagePath)
        {
            try
            {
                // ピクチャーボックスを作成
                PictureBox pictureBox = new PictureBox();

                // 画像を読み込む
                Image originalImage = Image.FromFile(imagePath);


                // 現在の画像として保存
                currentPictureBox = pictureBox;

                // パネルに追加
                imagePanel.Controls.Add(pictureBox);
                pictureBox.Image = originalImage;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"画像の読み込みに失敗しました: {ex.Message}", "エラー",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // 現在の画像を削除
        private void RemoveCurrentImage()
        {
            if (currentPictureBox != null)
            {
                imagePanel.Controls.Remove(currentPictureBox);
                currentPictureBox.Dispose();
                currentPictureBox = null;
            }
        }


        // 画像を削除するボタンのイベント
        private void DeleteButton_Click(object sender, EventArgs e)
        {
            if (currentPictureBox == null)
            {
                MessageBox.Show("削除する画像がありません。", "情報",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var result = MessageBox.Show("画像を削除しますか?", "確認",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                RemoveCurrentImage();
            }
        }

        // 画像を保存するボタンのイベント
        private void SaveButton_Click(object sender, EventArgs e)
        {
        }
    }
}