using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Data.SQLite;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Threading;
using System.Xml.Linq;
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

        // データベース初期化
        private string DbPath;
        private string ConnectionString;

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

            // 合計数量ボタンの設定
            button5.Click += ClearButton_Click;

            // 更新ボタンの設定
            button6.Click += UpdateButton_Click;

            // データベース初期化
            InitializeDatabase();

            // DBからデータを読み込んで表示
            LoadDataFromDatabase();

        }

        // データベース初期化
        private void InitializeDatabase()
        {
            try
            {
                // データベースファイルのパス設定
                DbPath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    "InventoryData.db"
                );
                ConnectionString = $"Data Source={DbPath};";

                // データベースファイルが存在しない場合のみ作成
                if (File.Exists(DbPath) == false)
                {
                    // 空ファイルを作成
                    using (File.Create(DbPath))
                    {
                        // DB作成成功メッセージ
                        MessageBox.Show(
                            $"データベースファイルを作成しました。{DbPath}",
                            "DB作成完了",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information
                        );
                    }
                }

                // テーブルの存在確認
                if (!TableExists("InventoryItems"))
                {
                    // テーブルが存在しない場合のみ作成
                    bool TableCreate = CreateInventoryTable();

                    // テーブル作成結果に応じてメッセージを表示
                    if (TableCreate)
                    {
                        // テーブル作成成功メッセージ
                        MessageBox.Show(
                            "テーブル「InventoryItems」を作成しました。",
                            "テーブル作成完了",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information
                        );
                    }
                    else
                    {
                        // テーブル作成失敗メッセージ
                        MessageBox.Show(
                            "テーブル「InventoryItems」の作成に失敗しました。",
                            "エラー",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error
                        );
                    }
                }
                else
                {
                    // テーブルが既に存在する場合のメッセージ
                    MessageBox.Show(
                        "テーブル「InventoryItems」は既に存在します。",
                        "既存しています",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );
                }

                    // DB生成確認
                    VerifyDatabaseCreation();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        $"データベース初期化エラー:\n{ex.Message}",
                        "エラー",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );
                }
            }


        // テーブル存在確認メソッド
        private bool TableExists(string tableName)
        {
            try
            {
                using (SqliteConnection Connection = new SqliteConnection(ConnectionString))
                {
                    Connection.Open();
                    string CheckQuery = @"
                        SELECT COUNT(*) 
                        FROM sqlite_master 
                        WHERE type='table' AND name=@TableName";

                    using (SqliteCommand Command = new SqliteCommand(CheckQuery, Connection))
                    {
                        Command.Parameters.AddWithValue("@TableName", tableName);
                        long count = (long)Command.ExecuteScalar();
                        return count > 0;
                    }
                }
            }
            catch
            {
                return false;
            }
        }

        // InventoryItemsテーブルを作成,bool型を返すように修正
        private bool CreateInventoryTable()
        {
            try
            {
                using (SqliteConnection Connection = new SqliteConnection(ConnectionString))
                {
                    Connection.Open();

                    string CreateTableQuery = @"
                    CREATE TABLE InventoryItems (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        IsChecked INTEGER NOT NULL DEFAULT 0,
                        Time TEXT NOT NULL,
                        Quantity INTEGER NOT NULL,
                        Comment TEXT
                    )";

                

                    using (SqliteCommand Command = new SqliteCommand(CreateTableQuery, Connection))
                    {
                        Command.ExecuteNonQuery();
                    }
                }
                return true; // 成功
            }
            catch
            {
                return false; // 失敗
            }

        }

        // データベースからデータを読み込む
        private void LoadDataFromDatabase()
        {
            InventoryDataGridView.Rows.Clear();

            using (SqliteConnection Connectionn = new SqliteConnection(ConnectionString))
            {
                // 接続開始
                Connectionn.Open();
                string SelectQuery = "SELECT * FROM InventoryItems ORDER BY Id";

                using (SqliteCommand Command = new SqliteCommand(SelectQuery, Connectionn))
                using (SqliteDataReader Reader = Command.ExecuteReader())
                {
                    while (Reader.Read())
                    {
                        int Id = Reader.GetInt32(0);
                        bool IsChecked = Reader.GetInt32(1) == 1;
                        string Time = Reader.GetString(2);
                        int Quantity = Reader.GetInt32(3);
                        string Comment = Reader.IsDBNull(4) ? "" : Reader.GetString(4);

                        // DataGridViewに行を追加
                        int RowIndex = InventoryDataGridView.Rows.Add(
                            IsChecked,
                            Time,
                            Quantity.ToString("N0"),
                            Comment,
                            ""
                        );

                        // 行のTagにIDを保存
                        InventoryDataGridView.Rows[RowIndex].Tag = Id;
                    }
                }
            }

            // 背景色を更新
            UpdateRowBackgroundColors();
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
            DialogResult Result = MessageBox.Show("この行を削除しますか？", "確認", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            
            // 
            DataGridViewRow row = InventoryDataGridView.Rows[RowIndex];
            if (Result == DialogResult.Yes)
            {
                // TagにIDがある場合は、DBからも削除
                if (row.Tag != null && row.Tag is int id)
                {
                    using (SqliteConnection connection = new SqliteConnection(ConnectionString))
                    {
                        connection.Open();
                        string deleteQuery = "DELETE FROM InventoryItems WHERE Id = @Id";

                        using (SqliteCommand command = new SqliteCommand(deleteQuery, connection))
                        {
                            command.Parameters.AddWithValue("@Id", id);
                            command.ExecuteNonQuery();
                        }
                    }
                }
                InventoryDataGridView.Rows.RemoveAt(RowIndex); //DataGridViewから削除
                UpdateRowBackgroundColors();

                MessageBox.Show("削除しました。", "完了", MessageBoxButtons.OK, MessageBoxIcon.Information);
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

                // 合計計算に含めるかどうかを示すチェックボックスの値を取得
                bool IncludedInTotal = Row.Cells[COLUMN_INDEX_CHECKBOX].Value is bool CheckBoxcellValue && CheckBoxcellValue;

                // 選択済み（合計計算に含まれる行）
                if (IncludedInTotal)
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
                // この行を合計計算に含めるかどうかを判定
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
            MessageBox.Show($"選択された合計数量: {TotalOfCheckedRows:N0}", "合計数量", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        // クリアボタンがクリックされたときの処理
        private void ClearButton_Click(object sender, EventArgs e)
        {
            // 削除確認ダイアログを表示
            DialogResult Result = MessageBox.Show("データをクリアしますか？", "確認", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            
            // 「はい」が選択された場合のみ削除
            if (Result == DialogResult.Yes)
            {
                // DBから全削除
                using (SqliteConnection Connection = new SqliteConnection(ConnectionString))
                {
                    Connection.Open();
                    string DeleteQuery = "DELETE FROM InventoryItems";

                    using (SqliteCommand command = new SqliteCommand(DeleteQuery, Connection))
                    {
                        command.ExecuteNonQuery();
                    }
                }

                // DataGridViewをクリア
                InventoryDataGridView.Rows.Clear();

                MessageBox.Show("全てのデータをクリアしました。", "完了", MessageBoxButtons.OK, MessageBoxIcon.Information);

            }
        }

        // 更新ボタンがクリックされたときの処理(DBに登録）
        private void UpdateButton_Click(object sender, EventArgs e)
        {

            DialogResult Result = MessageBox.Show("DataGridViewの全データをデータベースに保存しますか？", "保存確認",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (Result == DialogResult.Yes)
            {
                try
                {
                    using (SqliteConnection Connection = new SqliteConnection(ConnectionString))
                    {
                        // 接続開始
                        Connection.Open();

                        // トランザクション開始
                        using (SqliteTransaction Transaction = Connection.BeginTransaction())
                        {
                            // UPDATE用クエリ（既存レコードを更新）
                            string UpdateQuery = @"
                                    UPDATE InventoryItems
                                    SET IsChecked = @IsChecked,
                                        Time = @Time,
                                        Quantity = @Quantity,
                                        Comment = @Comment
                                    WHERE Id = @Id";

                            // INSERT用クエリ（新規レコードを挿入）
                            string InsertQuery = @"
                                        INSERT INTO InventoryItems (IsChecked, Time, Quantity, Comment)
                                        VALUES (@IsChecked, @Time, @Quantity, @Comment)";


                            foreach (DataGridViewRow row in InventoryDataGridView.Rows)
                            {
                                // チェックボックスの値を取得
                                bool IsChecked = row.Cells[COLUMN_INDEX_CHECKBOX].Value is bool checkValue && checkValue;

                                // 時刻を取得
                                string Time = row.Cells[COLUMN_INDEX_TIME].Value.ToString() ?? DateTime.Now.ToString("HH:mm:ss");

                                // 数量を取得（カンマを除去して数値に変換）
                                string QuantityText = row.Cells[COLUMN_INDEX_QUANTITY].Value.ToString()?.Replace(",", "");
                                int Quantity = int.TryParse(QuantityText, out int qty) ? qty : 0;

                                // コメントを取得
                                string Comment = row.Cells[COLUMN_INDEX_COMMENT].Value.ToString();

                                // 行のTagにIDがある場合はUPDATE、ない場合はINSERT
                                if (row.Tag != null && row.Tag is int id)
                                {
                                    // 既存レコードを更新
                                    using (SqliteCommand UpdateCommand = new SqliteCommand(UpdateQuery, Connection, Transaction))
                                    {
                                        UpdateCommand.Parameters.AddWithValue("@Id", id);
                                        UpdateCommand.Parameters.AddWithValue("@IsChecked", IsChecked ? 1 : 0);
                                        UpdateCommand.Parameters.AddWithValue("@Time", Time);
                                        UpdateCommand.Parameters.AddWithValue("@Quantity", Quantity);
                                        UpdateCommand.Parameters.AddWithValue("@Comment", Comment);

                                        UpdateCommand.ExecuteNonQuery();

                                    }
                                }
                                else
                                {
                                    // 新規レコードを挿入
                                    using (SqliteCommand InsertCommand = new SqliteCommand(InsertQuery, Connection, Transaction))
                                    {
                                        InsertCommand.Parameters.AddWithValue("@IsChecked", IsChecked ? 1 : 0);
                                        InsertCommand.Parameters.AddWithValue("@Time", Time);
                                        InsertCommand.Parameters.AddWithValue("@Quantity", Quantity);
                                        InsertCommand.Parameters.AddWithValue("@Comment", Comment);

                                        InsertCommand.ExecuteNonQuery();

                                    }
                                }
                            }

                            // コミット
                            Transaction.Commit();

                            // データを再読み込み
                            LoadDataFromDatabase();
                        }
                    }
                }
                catch (Exception ex)
                {
                    // 
                    MessageBox.Show(
                    $"データ読込エラー:\n{ex.Message}",
                    "エラー",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
                }
            }
        }

        // テーブルの存在確認メソッド(テーブルが存在しない場合のみ表示）
        private void VerifyDatabaseCreation()
        {
            // DBファイルの存在確認
            bool DbFileExists = File.Exists(DbPath);

            // テーブルの存在確認
            bool tableExists = TableExists("InventoryItems");

            // 確認結果メッセージ
            string DbStatus = DbFileExists ? "既存します。" : "存在しません。";
            string TableStatus = tableExists ? "既存します。" : "存在しません。";
            MessageBox.Show(
               $"=== データベース生成確認 ===\n\n" +
               $"DBファイル: {DbStatus}\n" +
               $"保存場所:\n{DbPath}\n\n" +
               $"テーブル: {TableStatus}",
               "DB確認",
               MessageBoxButtons.OK,
               MessageBoxIcon.Information
           );

            // エラーチェック
            if (!DbFileExists)
            {
                MessageBox.Show(
                    "データベースファイルの作成に失敗しました。",
                    "エラー",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                ); 
                throw new Exception("データベースファイルの作成に失敗しました。");
            }

            if (!tableExists)
            {
                MessageBox.Show(
                    "テーブルの作成に失敗しました。",
                    "エラー",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
                throw new Exception("テーブルの作成に失敗しました。");
            }
        }
    }
}

