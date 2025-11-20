using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Threading;

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
        private const int COLUMN_INDEX_DETAIL = 5; // 詳細ボタン列

        // 現在の数量を保持する変数
        private int CurrentAmount = DEFAULT_CURRENT_AMOUNT;

        // 時刻
        private System.Windows.Forms.Timer timer; // WinForms用タイマー

        // 在庫リスト表示用DataGridView
        private DataGridView InventoryDataGridView;

        public InventoryQuantityForm()
        {
            InitializeComponent();

            // 初期値を設定,数値→文字形式に変換
            CountTextBox.Text = DEFAULT_CURRENT_AMOUNT.ToString();

            //タイマーのインスタンスを生成（WinForms.Timerを使用）
            timer = new System.Windows.Forms.Timer();
             timer.Interval = 1000; // 1秒（ミリ秒）
             timer.Tick += Timer_Tick;
             timer.Start();

            // イベント設定
            PlusButton.Click += PlusButton_Click; // +ボタン
            MinusButton.Click += MinusButton_Click; // -ボタン
            CountTextBox.Leave += TextBox1_Leave; // フォーカスが外れた時
            AddButton.Click += AddButton_Click; // 追加ボタン
            TotalQuantitySelectedButton.Click += TotalQuantity_Click; // 合計数量ボタン
            ClearButton.Click += ClearButton_Click; // クリアボタン
            UpdateButton.Click += UpdateButton_Click; // 更新ボタン

            // 3桁区切りのカンマ付きで表示
            CommaValue();

            // リストビュー（DataGridView）の設定
            SetupInventoryDataGridView();

            // データベース初期化
            DatabaseHelper.InitializeDatabase();

            // DBからデータを読み込んで表示
            LoadDataFromDatabase();
        }

        // データベースからデータを読み込む
        private void LoadDataFromDatabase()
        {
            DatabaseHelper.LoadInventoryDataToGridView(InventoryDataGridView);
            UpdateRowBackgroundColors();
        }

        // +ボタンがクリックされたときの処理
        private void PlusButton_Click(object sender, EventArgs e)
        {
            // 数量を1ずつ増やす、最大値は9999
            if (CurrentAmount < MAX_CURRENT_AMOUNT)
            {
                CurrentAmount++;
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
                CommaValue();
            }
        }

        // テキストボックスからフォーカスが外れた時の処理
        private void TextBox1_Leave(object sender, EventArgs e)
        {
            // カンマを除去して数値のみを取得
            string InputText = CountTextBox.Text.Replace(",", "");

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
            CountTextBox.Text = CurrentAmount.ToString("N0");
        }

        // タイマー表示
        private void Timer_Tick(object sender, EventArgs e)
        {
            TimeLabel.Text = DateTime.Now.ToString("HH:mm:ss");
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
            quantityColumn.ReadOnly = false; // 編集可能へ
            quantityColumn.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            InventoryDataGridView.Columns.Add(quantityColumn);

            // コメント列
            DataGridViewTextBoxColumn commentColumn = new DataGridViewTextBoxColumn();
            commentColumn.HeaderText = "コメント";
            commentColumn.Name = "Comment";
            commentColumn.Width = 250;
            commentColumn.ReadOnly = false; // 編集可能へ
            InventoryDataGridView.Columns.Add(commentColumn);

            // 削除ボタン列
            DataGridViewButtonColumn deleteColumn = new DataGridViewButtonColumn();
            deleteColumn.HeaderText = "削除";
            deleteColumn.Name = "Delete";
            deleteColumn.Text = "削除";
            deleteColumn.UseColumnTextForButtonValue = true;
            deleteColumn.Width = 100;
            InventoryDataGridView.Columns.Add(deleteColumn);

            // 詳細ボタン列
            DataGridViewButtonColumn detailColumn = new DataGridViewButtonColumn();
            detailColumn.HeaderText = "詳細";
            detailColumn.Name = "Detail";
            detailColumn.Text = "詳細";
            detailColumn.UseColumnTextForButtonValue = true;
            detailColumn.Width = 100;
            InventoryDataGridView.Columns.Add(detailColumn);

            // イベント登録
            InventoryDataGridView.CellContentClick += InventoryDataGridView_CellContentClick;

            // フォームに追加
            this.Controls.Add(InventoryDataGridView);
        }

        // DataGridView のクリックイベント
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
            // 詳細ボタンがクリックされた場合
            else if (e.ColumnIndex == COLUMN_INDEX_DETAIL)
            {
                DetailRow(e.RowIndex);
            }
        }

        // 削除メソッド（DBからも削除）
        private void DeleteRow(int RowIndex)
        {
            DialogResult Result = MessageBox.Show("この行を削除しますか？\n※データベースからも削除されます。","確認", 
                                    MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (Result == DialogResult.Yes)
            {
                DataGridViewRow row = InventoryDataGridView.Rows[RowIndex];

                // Tagの取得を安全に行う　!!!
                int? ItemId = row.Tag is int id ? id : (int?)null;

                // データベースに保存済みの行の場合は、DBからも削除
                if (ItemId.HasValue)
                {
                    // DB削除処理 !変数名変更!
                    bool Success = DatabaseHelper.DeleteInventoryItem(ItemId.Value);
                    if (!Success)
                    {
                        // DB削除に失敗したら処理を中止
                        return;
                    }
                }

                InventoryDataGridView.Rows.RemoveAt(RowIndex); // DataGridViewから行を削除
                UpdateRowBackgroundColors(); // 行の背景色を更新
            }
        }

        // 詳細ボタン
        private void DetailRow(int RowIndex)
        {
            // 行データを取得 ??について
            DataGridViewRow row = InventoryDataGridView.Rows[RowIndex];
            string time = row.Cells[COLUMN_INDEX_TIME].Value?.ToString() ?? "";
            string quantity = row.Cells[COLUMN_INDEX_QUANTITY].Value?.ToString() ?? "";
            string comment = row.Cells[COLUMN_INDEX_COMMENT].Value?.ToString() ?? "";

            // 行のIDを取得（画像表示のため）!!!
            int? inventoryItemId = row.Tag is int id ? id : (int?)null;

            // 詳細フォームを表示
            Form detailForm = new DetailForm2(time, quantity, comment, inventoryItemId);
            detailForm.Show();
        }

        // 在庫追加ボタン
        private void AddButton_Click(object sender, EventArgs e)
        {
            // チェックボックス列の追加(自動でチェックOFF)
            bool CheckBoxValue = false;
            // 現在時刻を取得
            string CurrentTime = DateTime.Now.ToString("HH:mm:ss");
            // 数量を取得
            string Quantity = CurrentAmount.ToString("N0");
            // コメントを取得
            string Comment = CommentTextBox.Text;
            // 削除を取得
            string Delete = "";

            // DataGridViewに新しい行を追加
            int RowIndex = InventoryDataGridView.Rows.Add(CheckBoxValue, CurrentTime, Quantity, Comment, Delete);

            // 行の背景色を更新
            UpdateRowBackgroundColors();
        }

        // 行の背景色を更新（選択済み、奇数行、偶数行で色分け）
        private void UpdateRowBackgroundColors()
        {
            for (int i = 0; i < InventoryDataGridView.Rows.Count; i++)
            {
                // 各行を取得
                DataGridViewRow Row = InventoryDataGridView.Rows[i];

                // // 合計計算に含めるかどうかを示すチェックボックスの値を取得
                bool IncludedInTotal = Row.Cells[COLUMN_INDEX_CHECKBOX].Value is bool CheckBoxcellValue && CheckBoxcellValue;

                // 選択済み（合計計算に含まれる行）
                if (IncludedInTotal)
                {
                    Row.DefaultCellStyle.BackColor = Color.LightGreen; // 緑色
                }
                // 偶数行（0, 2, 4...）
                else if (i % 2 == 0)
                {
                    Row.DefaultCellStyle.BackColor = Color.White; // 白色（背景色なし）
                }
                // 奇数行（1, 3, 5...）
                else
                {
                    Row.DefaultCellStyle.BackColor = Color.LightBlue; // 青色
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
                // チェックボックスの値を取得
                bool IncludedInTotal = row.Cells[COLUMN_INDEX_CHECKBOX].Value is bool CheckBoxcellValue && CheckBoxcellValue;

                if (IncludedInTotal)
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
            MessageBox.Show($"選択された合計数量: {TotalOfCheckedRows:N0}", "合計数量",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        // クリアボタンがクリックされたときの処理
        private void ClearButton_Click(object sender, EventArgs e)
        {
            // 削除確認ダイアログを表示
            DialogResult Result = MessageBox.Show(
                "すべてのデータをクリアしますか？\n※データベースからも完全に削除されます。",
                "確認",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            // 「はい」が選択された場合のみ削除
            if (Result == DialogResult.Yes)
            {
                // データベースからも全データを削除 !変数名変更!
                bool Success = DatabaseHelper.ClearAllInventoryItems();

                // DataGridViewから全データを削除
                if (Success)
                {
                    InventoryDataGridView.Rows.Clear();
                }
            }
        }

        // 更新ボタンがクリックされたときの処理(DBに登録）
        private void UpdateButton_Click(object sender, EventArgs e)
        {
            DialogResult Result = MessageBox.Show("DataGridViewの全データをデータベースに保存しますか？", "保存確認", 
                                    MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            // 「はい」が選択された場合のみ保存
            if (Result == DialogResult.Yes)
            {
                // DataGridViewのデータをDBに保存(挿入・更新)
                int InsertedCount;
                int UpdatedCount;
                List<int> NewInsertedIds; // 追加: 新規挿入されたIDリスト

                // 保存処理 !変数名変更!
                bool Success = DatabaseHelper.SaveInventoryItems(
                    InventoryDataGridView,
                    COLUMN_INDEX_CHECKBOX,
                    COLUMN_INDEX_TIME,
                    COLUMN_INDEX_QUANTITY,
                    COLUMN_INDEX_COMMENT,
                    out InsertedCount,
                    out UpdatedCount,
                    out NewInsertedIds // 追加: 新規挿入されたIDリスト
                );

                // 保存成功時の処理
                if (Success)
                {
                    // 新規挿入された行に Tag を設定
                    int newIdIndex = 0;
                    for (int i = 0; i < InventoryDataGridView.Rows.Count; i++)
                    {
                        DataGridViewRow row = InventoryDataGridView.Rows[i];

                        if (!(row.Tag is int))  // Tagが未設定 = 新規挿入行
                        {
                            if (newIdIndex < NewInsertedIds.Count)
                            {
                                row.Tag = NewInsertedIds[newIdIndex];  // IDを設定
                                newIdIndex++;
                            }
                        }
                    }
                    // 背景色を更新
                    UpdateRowBackgroundColors();

                }
            }
        }
    }
}
