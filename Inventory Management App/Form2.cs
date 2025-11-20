using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace Inventory_Management_App
{
    // 詳細情報フォーム
    public partial class DetailForm2 : Form
    {
        // 画像を管理（現在は1枚のみ）
        private PictureBox CurrentPictureBox = null;

        // パネルを使用してスクロール可能にする
        private Panel ImagePanel;

        // 画像サイズ制限（2MB）
        private const int MAX_IMAGES_SIZE_BYTES = 2 * 1024 * 1024;

        // ImagePanelの位置・サイズ
        private const int IMAGE_PANEL_X = 20;
        private const int IMAGE_PANEL_Y = 80;
        private const int IMAGE_PANEL_WIDTH = 520;
        private const int IMAGE_PANEL_HEIGHT = 280;

        // PictureBoxの位置
        private const int PictureBoxX = 100;
        private const int PictureBoxY = 20;

        // InventoryItemのID（画像の関連付けに使用）!!!
        private int? InventoryItemId;

        // Form1から渡されたデータを受け取るコンストラクタ
        public DetailForm2(string time, string quantity, string comment, int? itemId = null)
        {
            // コンポーネントの初期化
            InitializeComponent();

            // タイトルの設定
            this.Text = "詳細情報";

            // 渡されたデータをラベルに表示
            TimeLabel.Text = $"時刻: {time}";
            QuantityLavel.Text = $"数量: {quantity}";
            CommentLavel.Text = $"コメント: {comment}";

            // InventoryItemIdを保存
            InventoryItemId = itemId;

            // 画像表示用パネルの初期化
            InitializeImagePanel();

            // データベースから画像を読み込む
            LoadImageFromDatabase();
        }

        // 画像表示用パネルの初期化
        private void InitializeImagePanel()
        {
            ImagePanel = new Panel();
            ImagePanel.Location = new Point(IMAGE_PANEL_X, IMAGE_PANEL_Y);
            ImagePanel.Size = new Size(IMAGE_PANEL_WIDTH, IMAGE_PANEL_HEIGHT);
            ImagePanel.AutoScroll = true; // スクロールを有効にする
            ImagePanel.BorderStyle = BorderStyle.FixedSingle; // 枠線を追加
            this.Controls.Add(ImagePanel); // フォームにパネルを追加
        }

        // データベースから画像を読み込む（行IDに紐づく画像のみ）
        private void LoadImageFromDatabase()
        {
            // InventoryItemIdがない場合は画像を読み込まない
            if (!InventoryItemId.HasValue)
            {
                return; // 中止
            }

            try
            {
                // 共通メソッドを使用して画像を取得
                var images = DatabaseHelper.LoadImagesForInventoryItem(InventoryItemId.Value);
                if (images.Count > 0)
                {
                    AddImageToPanelFromImage(images[0]); // 最初の画像を表示
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"画像の読み込みに失敗しました: {ex.Message}", "エラー",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // 画像を挿入するボタンのイベント
        private void InsertImageButton_Click(object sender, EventArgs e)
        {
            // 既に画像がある場合は警告
            if (CurrentPictureBox != null)
            {
                DialogResult confirmResult = MessageBox.Show("既に画像が挿入されています。新しい画像に置き換えますか？","確認", 
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                // ユーザーが「いいえ」を選択した場合は処理を中止
                if (confirmResult != DialogResult.Yes)
                {
                    return; // 中止
                }

                // 既存の画像を削除
                RemoveCurrentImage();
            }

            // OpenFileDialogを使用して画像ファイルを選択
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                // ダイアログの設定
                openFileDialog.Title = "画像を選択"; 
                openFileDialog.Filter = "画像ファイル (*.jpg;*.jpeg;*.png;)|*.jpg;*.jpeg;*.png;";
                openFileDialog.Multiselect = false; // 複数選択不可

                // ダイアログを表示
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    // ファイルサイズチェック(2MBまで)
                    FileInfo fileInfo = new FileInfo(openFileDialog.FileName);

                    // サイズが制限を超えている場合は警告
                    if (fileInfo.Length > MAX_IMAGES_SIZE_BYTES)
                    {
                        MessageBox.Show($"画像ファイルのサイズが2MBを超えています。\n現在のサイズ: {fileInfo.Length / 1024.0 / 1024.0:F2}MB","エラー", 
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    // 画像ファイルをImageとして読み込む
                    Image image = Image.FromFile(openFileDialog.FileName);
                    // パネルに画像を追加
                    AddImageToPanelFromImage(image);
                }
            }
        }

        // パネルにImageオブジェクトから画像を追加するメソッド
        private void AddImageToPanelFromImage(Image image)
        {
            try
            {
                // PictureBoxを作成して画像を表示
                PictureBox pictureBox = new PictureBox(); // 新しいPictureBoxを作成
                pictureBox.SizeMode = PictureBoxSizeMode.Zoom; // サイズに合わせて拡大縮小
                pictureBox.Location = new Point(PictureBoxX, PictureBoxY); // 定数を使用
                pictureBox.Size = new Size(Math.Min(ImagePanel.Width, image.Width), Math.Min(ImagePanel.Height, image.Height)); // パネル内に収まるサイズに調整
                pictureBox.Anchor = AnchorStyles.Top | AnchorStyles.Left; // 左上に固定
                pictureBox.Image = new Bitmap(image); // 画像を設定

                // 既存の画像があれば削除
                RemoveCurrentImage();

                // 現在の画像として保存
                CurrentPictureBox = pictureBox;
                ImagePanel.Controls.Add(pictureBox);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"画像の表示に失敗しました: {ex.Message}", "エラー", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // 現在の画像を削除
        private void RemoveCurrentImage()
        {
            // 既存のPictureBoxがあれば削除
            if (CurrentPictureBox != null)
            {
                // パネルから削除してリソースを解放
                ImagePanel.Controls.Remove(CurrentPictureBox); // パネルから削除
                CurrentPictureBox.Image?.Dispose(); // 画像リソースを解放
                CurrentPictureBox.Dispose(); // PictureBox自体を解放
                CurrentPictureBox = null; // 参照をクリア
            }
        }

        // 画像を削除するボタンのイベント
        private void DeleteButton_Click(object sender, EventArgs e)
        {
            // 画像がない場合は警告
            if (CurrentPictureBox == null)
            {
                MessageBox.Show("削除する画像がありません。", "情報", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            DialogResult Result = MessageBox.Show("画像を削除しますか?", "確認", 
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            // ユーザーが「はい」を選択した場合に削除
            if (Result == DialogResult.Yes)
            {
                RemoveCurrentImage(); // 画像を削除
            }
        }

        // 画像を保存するボタンのイベント（共通メソッドを使用）
        private void SaveButton_Click(object sender, EventArgs e)
        {
            // 画像がない場合は警告
            if (CurrentPictureBox == null)
            {
                MessageBox.Show("保存する画像がありません。", "情報",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // InventoryItemIdがない場合は警告
            if (!InventoryItemId.HasValue)
            {
                MessageBox.Show("この行はまだデータベースに保存されていません。\nForm1の「更新」ボタンで保存してから、画像を登録してください。",
                    "情報", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                // 画像をバイト配列に変換
                byte[] imageData;
                // MemoryStreamを使用して画像をバイト配列に変換
                using (MemoryStream ms = new MemoryStream())
                {
                    // PNG形式で保存
                    CurrentPictureBox.Image.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                    imageData = ms.ToArray();
                }

                // 共通メソッドを使用して保存 !変数名変更予定!
                bool ImageSaveResult = DatabaseHelper.SaveImage(imageData, InventoryItemId);

                if (ImageSaveResult)
                {
                    // 保存成功メッセージ
                    MessageBox.Show("画像が正常に保存されました。", "情報",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"画像の保存に失敗しました: {ex.Message}", "エラー",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
