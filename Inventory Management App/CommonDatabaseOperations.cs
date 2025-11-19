using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace Inventory_Management_App
{
    // データベース操作の共通処理を行うクラス
    
    public static class DatabaseHelper
    {
        private static string dbPath; // データベースファイルのパス
        private static string connectionString; // 接続文字列


        // データベースパスを取得
        public static string DbPath
        {
            get
            {
                // 一度設定されたら保持
                if (string.IsNullOrEmpty(dbPath))
                {
                    // 初回はアプリケーションの実行フォルダに作る（権限問題を避けるため）
                    dbPath = Path.Combine(Application.StartupPath, "InventoryData.db");
                }
                return dbPath; // データベースファイルのパス
            }
        }

        // 接続文字列を取得
        public static string ConnectionString
        {
            get
            {
                // 一度設定されたら保持
                if (string.IsNullOrEmpty(connectionString))
                {
                    connectionString = $"Data Source={DbPath};"; // SQLiteの接続文字列
                }
                return connectionString; // 接続文字列
            }
        }

        /// データベースの初期化（ファイル作成とテーブル作成）
        public static void InitializeDatabase()
        {
            // データベースファイルが存在しない場合は作成
            if (!File.Exists(DbPath))
            {
                using (File.Create(DbPath)) { } // 空のファイルを作成
            }

            // テーブル作成はトランザクションで実施
            ExecuteWithTransaction(
                ConnectionString,
                (connection, transaction) =>
                {

                    // InventoryItemsテーブル作成(在庫項目)
                    string createInventoryTableQuery = @"
                        CREATE TABLE IF NOT EXISTS InventoryItems ( 
                            Id INTEGER PRIMARY KEY AUTOINCREMENT,
                            IsChecked INTEGER NOT NULL DEFAULT 0, 
                            Time TEXT NOT NULL,
                            Quantity INTEGER NOT NULL DEFAULT 0, 
                            Comment TEXT 
                        )";

                    // InventoryItemsテーブル作成
                    using (var command = connection.CreateCommand())
                    {
                        command.Transaction = transaction; // トランザクションを関連付け 
                        command.CommandText = createInventoryTableQuery;  // SQLコマンドを設定
                        command.ExecuteNonQuery(); // コマンドを実行
                    }



                    // Imagesテーブル作成(画像項目,InventoryItemId外部キー付き、ON DELETE CASCADEを定義)
                    string createImagesTableQuery = @"
                        CREATE TABLE IF NOT EXISTS Images (
                            Id INTEGER PRIMARY KEY AUTOINCREMENT, 
                            InventoryItemId INTEGER,
                            ImageData BLOB NOT NULL,
                            CreatedAt TEXT NOT NULL,
                            FOREIGN KEY (InventoryItemId) REFERENCES InventoryItems(Id) ON DELETE CASCADE
                        )";

                    // Imagesテーブル作成
                    using (var command = connection.CreateCommand())
                    {
                        command.Transaction = transaction;
                        command.CommandText = createImagesTableQuery;
                        command.ExecuteNonQuery();
                    }

                    

                },
                null  // 初期化時はメッセージ表示なし
            );
        }

        /// トランザクション内でデータベース操作を実行する共通メソッド
        
        public static bool ExecuteWithTransaction(
            string connectionString,
            Action<SqliteConnection, SqliteTransaction> action, // 実行する処理
            string successMessage = null)
        {
            try
            {
                // データベース接続を開く
                using (var connection = new SqliteConnection(connectionString))
                {
                    connection.Open(); // 接続を開く

                    // 外部キー制約を有効化（セッションレベル）→必須(ONにしないとON DELETE CASCADEが動かない(整合性が失われる）)
                    //using (var enableFK = connection.CreateCommand())
                    //{
                    //    enableFK.CommandText = "PRAGMA foreign_keys = ON"; // 外部キー制約を有効化
                    //    enableFK.ExecuteNonQuery(); // コマンドを実行
                    //}
                    
                    using (var transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            // 渡された処理を実行
                            action(connection, transaction);

                            // トランザクションコミット
                            transaction.Commit();

                            // 成功メッセージの表示（指定されている場合）
                            if (!string.IsNullOrEmpty(successMessage))
                            {
                                MessageBox.Show(
                                    successMessage,
                                    "成功",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Information);
                            }

                            return true;
                        }
                        catch (Exception)
                        {
                            // エラー時はロールバック
                            try
                            {
                                transaction.Rollback();
                            }
                            catch { /* rollback失敗は無視 */ }
                            throw;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"データベース操作に失敗しました: {ex.Message}",
                    "エラー",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return false;
            }
        }

        // SQLコマンドを実行してデータを取得する共通メソッド（SELECT用）
        
        public static bool ExecuteReader(
            string connectionString, // 接続文字列
            string query, // SQLクエリ
            Action<SqliteDataReader> readerAction, // データリーダー処理アクション
            Action<SqliteCommand> parameterSetter = null // パラメータ設定アクション（省略可）
            ) 
        {
            try
            {
                // データベース接続を開く
                using (var connection = new SqliteConnection(connectionString))
                {
                    connection.Open();

                    // SQLコマンドを作成して実行
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = query; // SQLクエリを設定
                        parameterSetter?.Invoke((SqliteCommand)command); // パラメータを設定（必要な場合）

                        // データリーダーを取得
                        using (var reader = command.ExecuteReader())
                        {
                            readerAction(reader); // データリーダー処理を実行
                        }
                    }
                }
                return true; // 成功
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"データの取得に失敗しました: {ex.Message}",
                    "エラー",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return false; // 失敗
            }
        }

        // 単一のSQLコマンドを実行する（INSERT/UPDATE/DELETE用）

        public static bool ExecuteNonQuery(
            string connectionString,
            string query,
            Action<SqliteCommand> parameters = null, // パラメータ設定アクション
            string successMessage = null) // 成功メッセージ
        {
            try
            {
                using (var connection = new SqliteConnection(connectionString))
                {
                    connection.Open();
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = query; // SQLクエリを設定
                        parameters?.Invoke((SqliteCommand)command); // パラメータを設定
                        command.ExecuteNonQuery(); // コマンドを実行
                    }
                }


                if (!string.IsNullOrEmpty(successMessage))
                    MessageBox.Show(successMessage, "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);

                return true;
            }

            catch (Exception ex)
            {
                MessageBox.Show(
                    $"データベース操作に失敗しました: {ex.Message}",
                    "エラー",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return false;
            }
           
        }

        // InventoryItemsテーブルの全データを保存する
        
        public static bool SaveInventoryItems(
            DataGridView dataGridView,
            int ColumnIndexCheckbox,
            int ColumnIndexTime,
            int ColumnIndexQuantity,
            int ColumnIndexComment,
            out int InsertedCount,
            out int UpdatedCount)
        {
            int Inserted = 0; // 挿入件数カウンタ
            int Updated = 0;  // 更新件数カウンタ

            // トランザクション内で保存処理を実行
            bool TransactionSucceeded = ExecuteWithTransaction(
                ConnectionString,
                (connection, transaction) =>
                {
                    string UpdateQuery = @"
                        UPDATE InventoryItems
                        SET IsChecked = @IsChecked,
                            Time = @Time,
                            Quantity = @Quantity,
                            Comment = @Comment
                        WHERE Id = @Id";

                    string InsertQuery = @"
                        INSERT INTO InventoryItems (IsChecked, Time, Quantity, Comment)
                        VALUES (@IsChecked, @Time, @Quantity, @Comment)";

                    foreach (DataGridViewRow row in dataGridView.Rows)
                    {
                        // 各セルの値を取得（安全に） ?の書き方
                        bool IsChecked = row.Cells[ColumnIndexCheckbox].Value is bool checkValue && checkValue;
                        
                        string Time = row.Cells[ColumnIndexTime].Value?.ToString()
                                     ?? DateTime.Now.ToString("HH:mm:ss");

                        string QuantityText = row.Cells[ColumnIndexQuantity].Value?.ToString()?.Replace(",", "")
                                            ?? "0";

                        int Quantity = int.TryParse(QuantityText, out int qty) ? qty : 0;

                        string Comment = row.Cells[ColumnIndexComment].Value?.ToString() ?? "";

                        // 既存データの更新 or 新規挿入
                        if (row.Tag is int id)
                        {
                            // 更新処理
                            using (var UpdateCommand = connection.CreateCommand())
                            {
                                UpdateCommand.Transaction = transaction;
                                UpdateCommand.CommandText = UpdateQuery;
                                var uc = (SqliteCommand)UpdateCommand;
                                uc.Parameters.AddWithValue("@Id", id);
                                uc.Parameters.AddWithValue("@IsChecked", IsChecked ? 1 : 0);
                                uc.Parameters.AddWithValue("@Time", Time);
                                uc.Parameters.AddWithValue("@Quantity", Quantity);
                                uc.Parameters.AddWithValue("@Comment", Comment);
                                uc.ExecuteNonQuery();
                            }
                            Updated++;
                        }
                        else
                        {
                            // 挿入処理
                            using (var InsertCommand = connection.CreateCommand())
                            {
                                InsertCommand.Transaction = transaction;
                                InsertCommand.CommandText = InsertQuery;
                                var ic = (SqliteCommand)InsertCommand;
                                ic.Parameters.AddWithValue("@IsChecked", IsChecked ? 1 : 0);
                                ic.Parameters.AddWithValue("@Time", Time);
                                ic.Parameters.AddWithValue("@Quantity", Quantity);
                                ic.Parameters.AddWithValue("@Comment", Comment);
                                ic.ExecuteNonQuery();
                            }
                            Inserted++;
                        }
                    }
                },
                null  // 成功メッセージは呼び出し側で表示
            );

            InsertedCount = Inserted; // 挿入件数を出力パラメータに設定
            UpdatedCount = Updated; // 更新件数を出力パラメータに設定
            return TransactionSucceeded; // 全体の成功状態を返す
        }

        // 画像データをデータベースに保存する
        
        public static bool SaveImage(byte[] imageData, int? inventoryItemId = null)
        {
            return ExecuteNonQuery(
                ConnectionString,
                @"INSERT INTO Images (InventoryItemId, ImageData, CreatedAt) 
                  VALUES (@InventoryItemId, @ImageData, @CreatedAt)",
                
                (command) =>
                {
                    var cmd = (SqliteCommand)command;
                    cmd.Parameters.AddWithValue("@InventoryItemId",
                        inventoryItemId.HasValue ? (object)inventoryItemId.Value : DBNull.Value);
                    cmd.Parameters.AddWithValue("@ImageData", imageData);
                    cmd.Parameters.AddWithValue("@CreatedAt", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                },
                "画像をデータベースに保存しました。"
            );
        }

        // 指定されたInventoryItemIdに関連する画像を取得
        
        public static List<Image> LoadImagesForInventoryItem(int inventoryItemId)
        {
            // 画像リストを初期化
            List<Image> images = new List<Image>();

            // データベースから画像データを取得
            ExecuteReader(
                ConnectionString,
                "SELECT ImageData FROM Images WHERE InventoryItemId = @InventoryItemId ORDER BY CreatedAt DESC", // SQLクエリ
                // データリーダー処理
                (reader) =>
                {
                    while (reader.Read())
                    {
                        // 画像データをバイト配列として取得
                        byte[] imageData = (byte[])reader["ImageData"];
                        // バイト配列からImageオブジェクトを作成
                        using (MemoryStream ms = new MemoryStream(imageData))
                        {
                            Image img = Image.FromStream(ms); // 元のImageオブジェクト
                            Image clonedImage = new Bitmap(img); // クローンを作成
                            images.Add(clonedImage); // クローンをリストに追加
                        }
                    }
                },
                // パラメータ設定
                (command) =>
                {
                    command.Parameters.AddWithValue("@InventoryItemId", inventoryItemId); 
                }
            );

            return images; // 画像リストを返す
        }

        // DataGridViewからデータを読み込む
        
        public static void LoadInventoryDataToGridView(DataGridView gridView)
        {
            // 既存の行をクリア
            gridView.Rows.Clear();

            // データベースからInventoryItemsデータを取得してDataGridViewに追加
            ExecuteReader(
                ConnectionString,
                "SELECT * FROM InventoryItems ORDER BY Id",
                (reader) =>
                {
                    while (reader.Read())
                    {
                        int Id = reader.GetInt32(0);
                        bool IsChecked = reader.GetInt32(1) == 1;
                        string Time = reader.GetString(2);
                        int Quantity = reader.GetInt32(3);
                        string Comment = reader.IsDBNull(4) ? "" : reader.GetString(4);

                        // DataGridViewに行を追加（Delete/Detail列分もプレースホルダを入れる）
                        int rowIndex = gridView.Rows.Add(
                            IsChecked,
                            Time,
                            Quantity.ToString("N0"),
                            Comment,
                            "", // Delete ボタンセルは UseColumnTextForButtonValue=true なので値は不要
                            ""  // Detail ボタンセル
                        );

                        // 行のTagにIDを保存
                        gridView.Rows[rowIndex].Tag = Id;
                    }
                }
            );
        }

        // すべてのInventoryItemsデータを削除する
        
        public static bool ClearAllInventoryItems()
        {
            // トランザクション内で削除処理を実行
            return ExecuteWithTransaction(
                ConnectionString,
                (connection, transaction) =>
                {
                    // InventoryItemIdがNULLの画像を先に削除
                    using (var command = connection.CreateCommand())
                    {
                        command.Transaction = transaction;
                        command.CommandText = "DELETE FROM Images WHERE InventoryItemId IS NULL"; // NULLの画像を削除
                        command.ExecuteNonQuery();
                    }
                    // 全データを削除（ON DELETE CASCADEにより関連画像も自動削除）
                    using (var command = connection.CreateCommand())
                    {
                        command.Transaction = transaction;
                        command.CommandText = "DELETE FROM InventoryItems"; // すべてのInventoryItemsを削除
                        command.ExecuteNonQuery();
                    }
                },
                "すべてのデータをクリアしました。"
            );
        }

        // 指定されたIDのInventoryItemを削除
        
        public static bool DeleteInventoryItem(int id)
        {
            return ExecuteNonQuery(
                ConnectionString,
                "DELETE FROM InventoryItems WHERE Id = @Id",
                cmd => cmd.Parameters.AddWithValue("@Id", id)
            );
        }
    }
}
