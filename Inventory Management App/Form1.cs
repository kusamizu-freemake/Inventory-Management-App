using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Threading;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TaskbarClock;

namespace Inventory_Management_App
{
    public partial class InventoryQuantityForm : Form
    {

        // 定数の設定
        // 最大値の設定
        private const int MAX_CURRENT_AMOUNT = 9999;
        // 最小値の設定
        private const int MIN_CURRENT_AMOUNT = 0;
        // デフォルト値の設定
        private const int DEFAULT_CURRENT_AMOUNT = 0;

        // 現在の数量を保持する変数
        private int CurrentAmount = DEFAULT_CURRENT_AMOUNT; 

        // 時刻
        private DispatcherTimer timer;

        // 在庫リスト表示用ListView
        private ListView InventoryListView;

        public InventoryQuantityForm()
        {
            InitializeComponent();
            // 初期値を設定,数値→文字形式に変換
            textBox1.Text = DEFAULT_CURRENT_AMOUNT.ToString();


            // タイマのインスタンスを生成
            timer = new DispatcherTimer();
            // 1秒ごとにイベントを発生させます。
            timer.Interval = TimeSpan.FromSeconds(1);
            // タイマーのTickイベントにTimer_Tickメソッドを関連付け
            // timer.Tick:イベント
            // Timer_Tick:メソッド
            timer.Tick += Timer_Tick;
            // timerをスタートさせます。
            timer.Start();

            // button1（+ボタン）のイベント設定
            button1.Click += PlusButton_Click;

            // button2（-ボタン）のイベント設定
            button2.Click += MinusButton_Click;

            // テキストボックスの手入力対応
            textBox1.Leave += TextBox1_Leave; // フォーカスが外れた時

            // 3桁区切りのカンマ付きで表示
            CommaValue();

            // button3（追加ボタン）のイベント設定
            button3.Click += AddButton_Click;

            // リストビューの設定
            SetupInventoryListView();

        }

        // +ボタンがクリックされたときの処理
        private void PlusButton_Click(object sender, EventArgs e)
        {
            // 数量を1ずつ増やす、最大値は9999
            if (CurrentAmount < MAX_CURRENT_AMOUNT)
            {
                CurrentAmount++;
                textBox1.Text = CurrentAmount.ToString(); // 変換
                CommaValue();
            }
        }

        // -ボタンがクリックされたときの処理
        private void MinusButton_Click(object sender, EventArgs e)
        {
            // 数量を1ずつ減らす、最小値は0（負の値にはならない）
            
            if (CurrentAmount > MIN_CURRENT_AMOUNT)
            {
                CurrentAmount--;
                textBox1.Text = CurrentAmount.ToString(); // 変換
                CommaValue();
            }

        }

        // テキストボックスからフォーカスが外れた時の処理
        private void TextBox1_Leave(object sender, EventArgs e)
        {
            // カンマを除去して数値のみを取得
            string InputText = textBox1.Text.Replace(",", "");

            // 数値に変換できるか確認
            if (int.TryParse(InputText, out int InputAmount))
            {
                // 0から9999の範囲に制限
                if (InputAmount < MIN_CURRENT_AMOUNT)
                {
                    CurrentAmount = MIN_CURRENT_AMOUNT;
                }
                else if (InputAmount > MAX_CURRENT_AMOUNT)
                {
                    CurrentAmount = MAX_CURRENT_AMOUNT;
                }
                else
                {
                    CurrentAmount = InputAmount;
                }
            }
            else
            {
                // 数値に変換できない場合は0にリセット
                CurrentAmount = MIN_CURRENT_AMOUNT;
            }   
            // テキストボックスに現在の数量を表示（カンマ付き）
            CommaValue();
        }

        private void CommaValue()
        {             
            // 3桁区切りのカンマ付きで表示
            textBox1.Text = CurrentAmount.ToString("N0");
        }

        // タイマー表示
        private void Timer_Tick(object sender, EventArgs e)
        {
            label2.Text = DateTime.Now.ToString("HH:mm:ss");
        }

        // 
        private void SetupInventoryListView()
        {
            
            //
            InventoryListView = new ListView();

            // 位置とサイズを明示的に指定
            InventoryListView.Location = new Point(20, 150);
            InventoryListView.Size = new Size(500, 250);


            // 表示設定
            InventoryListView.View = View.Details;              // 詳細表示モード（表形式）
            InventoryListView.FullRowSelect = true;             // 行全体を選択対象にする
            InventoryListView.GridLines = true;                 // グリッド線を表示
            InventoryListView.CheckBoxes = true;                // チェックボックスを表示
            InventoryListView.HeaderStyle = ColumnHeaderStyle.Nonclickable; // ヘッダークリック無効
            InventoryListView.MultiSelect = true;               // 複数選択を許可

            // チェックボックスの状態変更イベントを追加
            InventoryListView.ItemChecked += InventoryListView_ItemChecked;
            // ListViewのクリックイベントを登録
            InventoryListView.MouseClick += InventoryListView_MouseClick;

            // ListViewの列設定
            InventoryListViewColumns();

           
            // ListViewをフォームに追加
            this.Controls.Add(InventoryListView); 
        }

        // チェックボックスの状態変更イベント
        private void InventoryListView_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            // 背景色を更新
            UpdateRowBackgroundColors();
        }

        // ListViewのクリックイベント
        private void InventoryListView_MouseClick(object sender, MouseEventArgs e)
        {
            // クリックされた位置の情報を取得
            ListViewHitTestInfo HitTest = InventoryListView.HitTest(e.Location);
            // クリックされた列のインデックスを取得
            int ColumnIndex = HitTest.Item.SubItems.IndexOf(HitTest.SubItem);

            // 削除列がクリックされた場合
            if (ColumnIndex == 3)
            {
                // 項目を削除
                InventoryListView.Items.Remove(HitTest.Item);

                // 背景色を再設定（行番号が変わるため）
                UpdateRowBackgroundColors();
            }
        }

        // ListViewの初期設定
        private void InventoryListViewColumns()
        {
            // ListViewの位置とサイズを設定
            InventoryListView.Location = new Point(30, 100);
            InventoryListView.Size = new Size(500, 300);

            // 既存の列をクリア
            InventoryListView.Columns.Clear();

            // 列ヘッダーを追加
            InventoryListView.Columns.Add("時刻", 150);
            InventoryListView.Columns.Add("数量", 100);
            InventoryListView.Columns.Add("コメント", 150);
            InventoryListView.Columns.Add("削除");
        }

        // 在庫追加ボタン
        private void AddButton_Click(object sender, EventArgs e)
        {

            // 現在時刻を取得
            string CurrentTime = DateTime.Now.ToString("HH:mm:ss");
            // 数量を取得
            string Quantity = CurrentAmount.ToString("N0"); // カンマ付き
            // コメントを取得
            string Comment = textBox2.Text;

            // 新しい行を作成
            ListViewItem NewItem = new ListViewItem(CurrentTime);



            // チェックボックスはデフォルトでチェックなし状態
            NewItem.Checked = false;

            // 時刻を設定
            NewItem.Text = CurrentTime;
            // 数量を追加
            NewItem.SubItems.Add(Quantity);
            // コメントを追加
            NewItem.SubItems.Add(Comment);
            // 削除列（空欄）
            NewItem.SubItems.Add("削除");

            // ListViewに追加
            InventoryListView.Items.Add(NewItem);

            // 行の背景色を更新
            UpdateRowBackgroundColors();

        }

        // 行の背景色を更新（選択済み、奇数行、偶数行で色分け）
        private void UpdateRowBackgroundColors()
        {
            for (int i = 0; i < InventoryListView.Items.Count; i++)
            {
                ListViewItem Item = InventoryListView.Items[i];

                // 選択済み（チェックON）の場合
                if (Item.Checked)
                {
                    Item.BackColor = Color.LightGreen;  // 緑色
                }
                // 偶数行（0, 2, 4...）
                else if (i % 2 == 0)
                {
                    Item.BackColor = Color.White;  // 白色（背景色なし）
                }
                // 奇数行（1, 3, 5...）
                else
                {
                    Item.BackColor = Color.LightBlue;  // 青色
                }
            }
        }
    }
}

