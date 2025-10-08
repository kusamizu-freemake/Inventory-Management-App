using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
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

        // DataGridView列のインデックス定数
        private const int COLUMN_INDEX_CHECKBOX = 0;
        private const int COLUMN_INDEX_TIME = 1;
        private const int COLUMN_INDEX_QUANTITY = 2;
        private const int COLUMN_INDEX_COMMENT = 3;
        private const int COLUMN_INDEX_DELETE = 4;

        // 現在の数量を保持する変数
        private int CurrentAmount = DEFAULT_CURRENT_AMOUNT;

        // 時刻
        private DispatcherTimer timer;

        // 在庫リスト表示用DataGridView
        private DataGridView InventoryDataGridView;

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

            // 合計数量ボタンの設定
            button4.Click += TotalQuantity_Click;

            // リストビューの設定
            SetupInventoryDataGridView();

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

        // DataGridの設定
        private void SetupInventoryDataGridView()
        {
            InventoryDataGridView = new DataGridView();

            // 位置とサイズを明示的に指定
            InventoryDataGridView.Location = new Point(20, 150);
            InventoryDataGridView.Size = new Size(650, 250);
            InventoryDataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill; // Grid幅に列を合わせる
            InventoryDataGridView.RowHeadersVisible = false; // 左端の項目列を削除
            InventoryDataGridView.AllowUserToAddRows = false; // 行の自動追加をオフ

            // DataGridViewの列設定
            InventoryDataGridView.Columns.Clear();

            // チェックボックス列
            DataGridViewCheckBoxColumn checkBoxColumn = new DataGridViewCheckBoxColumn();
            checkBoxColumn.HeaderText = "";
            checkBoxColumn.Name = "Check";
            checkBoxColumn.Width = 40;
            checkBoxColumn.ReadOnly = false;
            InventoryDataGridView.Columns.Add(checkBoxColumn);

            // 時刻列
            DataGridViewTextBoxColumn timeColumn = new DataGridViewTextBoxColumn();
            timeColumn.HeaderText = "時刻";
            timeColumn.Name = "Time";
            timeColumn.Width = 100;
            timeColumn.ReadOnly = true;
            InventoryDataGridView.Columns.Add(timeColumn);

            // 数量列
            DataGridViewTextBoxColumn quantityColumn = new DataGridViewTextBoxColumn();
            quantityColumn.HeaderText = "数量";
            quantityColumn.Name = "Quantity";
            quantityColumn.Width = 100;
            quantityColumn.ReadOnly = true;
            quantityColumn.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            InventoryDataGridView.Columns.Add(quantityColumn);

            // コメント列
            DataGridViewTextBoxColumn commentColumn = new DataGridViewTextBoxColumn();
            commentColumn.HeaderText = "コメント";
            commentColumn.Name = "Comment";
            commentColumn.Width = 250;
            commentColumn.ReadOnly = true;
            InventoryDataGridView.Columns.Add(commentColumn);

            // 削除ボタン列
            DataGridViewButtonColumn deleteColumn = new DataGridViewButtonColumn();
            deleteColumn.HeaderText = "削除";
            deleteColumn.Name = "Delete";
            deleteColumn.Text = "削除";
            deleteColumn.UseColumnTextForButtonValue = true;
            deleteColumn.Width = 100;
            InventoryDataGridView.Columns.Add(deleteColumn);

            // イベント登録
            InventoryDataGridView.CellContentClick += InventoryDataGridView_CellContentClick;

            // フォームに追加
            this.Controls.Add(InventoryDataGridView);
        }

        // ListViewのクリックイベント
        private void InventoryDataGridView_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

            // 削除ボタンがクリックされた場合
            if (e.ColumnIndex == COLUMN_INDEX_DELETE)
            {
                DeleteRow(e.RowIndex);
            }
            // チェックボックスがクリックされた場合
            else if (e.ColumnIndex == COLUMN_INDEX_CHECKBOX)
            {
                InventoryDataGridView.EndEdit(); // チェックボックスの状態を確定(編集モードを終了)
                UpdateRowBackgroundColors();
            }
        }

        // 削除メソッドを追加
        private void DeleteRow(int RowIndex)
        {
            // 削除確認ダイアログを表示
            var result = MessageBox.Show("この行を削除しますか？", "確認", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                InventoryDataGridView.Rows.RemoveAt(RowIndex);
                UpdateRowBackgroundColors();
            }
        }

        // 在庫追加ボタン
        private void AddButton_Click(object sender, EventArgs e)
        {
            // チェックボックス列の追加(自動でチェックOFF)
            bool CheckBoxValue = false;
            // 現在時刻を取得
            string CurrentTime = DateTime.Now.ToString("HH:mm:ss");
            // 数量を取得
            string Quantity = CurrentAmount.ToString("N0"); // カンマ付き
            // コメントを取得
            string Comment = textBox2.Text;
            // 削除を取得
            string Delete = "";

            // DataGridViewに新しい行を追加
            InventoryDataGridView.Rows.Add(CheckBoxValue, CurrentTime, Quantity, Comment, Delete);

            // 行の背景色を更新
            UpdateRowBackgroundColors();
        }

        // 行の背景色を更新（選択済み、奇数行、偶数行で色分け）
        private void UpdateRowBackgroundColors()
        {
            for (int i = 0; i < InventoryDataGridView.Rows.Count; i++)
            {
                DataGridViewRow Row = InventoryDataGridView.Rows[i];

                // チェックボックスの値を取得
                // 合計計算に含めるかどうかを示すチェックボックスの値を取得
                var IncludeInTotalCheckBox = Row.Cells[COLUMN_INDEX_CHECKBOX].Value;
                // この行の数量を合計計算に含めるかどうかを判定
                bool IsIncludedInTotal = IncludeInTotalCheckBox is bool CheckBoxcellValue && CheckBoxcellValue;

                // 選択済み（合計計算に含まれる行）
                if (IsIncludedInTotal)
                {
                    Row.DefaultCellStyle.BackColor = Color.LightGreen;  // 緑色
                }
                // 偶数行（0, 2, 4...）
                else if (i % 2 == 0)
                {
                    Row.DefaultCellStyle.BackColor = Color.White;  // 白色（背景色なし）
                }
                // 奇数行（1, 3, 5...）
                else
                {
                    Row.DefaultCellStyle.BackColor = Color.LightBlue;  // 青色
                }
            }
        }


        // 合計数量ボタンがクリックされたときの処理
        private void TotalQuantity_Click(object sender, EventArgs e)
        {
            // チェックされた行の合計
            int TotalOfCheckedRows = 0; 

            foreach (DataGridViewRow row in InventoryDataGridView.Rows)
            {
                // 合計計算に含めるかを制御するチェックボックスの値を取得
                var IncludeInTotalCheckBox = row.Cells[COLUMN_INDEX_CHECKBOX].Value;
                // この行を合計計算に含めるかどうかを判定
                bool IsIncludedInTotal = IncludeInTotalCheckBox is bool CheckBoxcellValue && CheckBoxcellValue;
                
                if (IsIncludedInTotal)
                {
                    // 数量列の値を取得し、カンマを除去して数値に変換
                    string QuantityText = row.Cells[COLUMN_INDEX_QUANTITY].Value.ToString().Replace(",", "");
                    if (int.TryParse(QuantityText, out int SelectQuantity))
                    {
                        // この行の数量を合計に加算
                        TotalOfCheckedRows += SelectQuantity;
                    }
                }
            }
            // 合計数量をメッセージボックスで表示
            MessageBox.Show($"選択された合計数量: {TotalOfCheckedRows:N0}", "合計数量", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}

