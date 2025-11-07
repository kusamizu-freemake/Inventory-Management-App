private void UpdateButton_Click(object sender, EventArgs e)
{
    // ========================================
    // ステップ1: ユーザーに保存確認
    // ========================================
    DialogResult result = MessageBox.Show(
        "DataGridViewの全データをデータベースに保存しますか？",
        "保存確認",
        MessageBoxButtons.YesNo,
        MessageBoxIcon.Question
    );

    // 「いいえ」が選択された場合、処理を中断
    if (result != DialogResult.Yes) return;

    try
    {

        int insertedCount = 0;  // 新規挿入されたレコード数
        int updatedCount = 0;   // 更新されたレコード数

        using (SqliteConnection connection = new SqliteConnection(connectionString))
        {
            // データベースへの接続を開始
            connection.Open();

            using (SqliteTransaction transaction = connection.BeginTransaction())
            {

                    string updateQuery = @"
                        UPDATE InventoryItems
                        SET IsChecked = @IsChecked,
                            Time = @Time,
                            Quantity = @Quantity,
                            Comment = @Comment
                        WHERE Id = @Id";

                    string insertQuery = @"
                        INSERT INTO InventoryItems (IsChecked, Time, Quantity, Comment)
                        VALUES (@IsChecked, @Time, @Quantity, @Comment)";

                    foreach (DataGridViewRow row in inventoryDataGridView.Rows)
                    {

                        bool isChecked = row.Cells[COLUMN_INDEX_CHECKBOX].Value is bool checkValue && checkValue;

                        string time = row.Cells[COLUMN_INDEX_TIME].Value?.ToString() 
                                     ?? DateTime.Now.ToString("HH:mm:ss");


                        string quantityText = row.Cells[COLUMN_INDEX_QUANTITY].Value?.ToString()?.Replace(",", "") 
                                            ?? "0";

                        int quantity = int.TryParse(quantityText, out int qty) ? qty : 0;
                        
                        string comment = row.Cells[COLUMN_INDEX_COMMENT].Value?.ToString() ?? "";


                        if (row.Tag != null && row.Tag is int id)
                        {

                            using (SqliteCommand updateCommand = new SqliteCommand(updateQuery, connection, transaction))
                            {
                                updateCommand.Parameters.AddWithValue("@Id", id);
                                updateCommand.Parameters.AddWithValue("@IsChecked", isChecked ? 1 : 0);
                                updateCommand.Parameters.AddWithValue("@Time", time);
                                updateCommand.Parameters.AddWithValue("@Quantity", quantity);
                                updateCommand.Parameters.AddWithValue("@Comment", comment);

                                updateCommand.ExecuteNonQuery();

                                updatedCount++;
                            }

                        }
                        else
                        {

                            using (SqliteCommand insertCommand = new SqliteCommand(insertQuery, connection, transaction))
                            {
                                insertCommand.Parameters.AddWithValue("@IsChecked", isChecked ? 1 : 0);
                                insertCommand.Parameters.AddWithValue("@Time", time);
                                insertCommand.Parameters.AddWithValue("@Quantity", quantity);
                                insertCommand.Parameters.AddWithValue("@Comment", comment);

                                insertCommand.ExecuteNonQuery();
                                insertedCount++;
                            }
                        }
                    }


                    transaction.Commit();
                }
            }

        LoadDataFromDatabase();

        MessageBox.Show(
            $"保存完了\n新規: {insertedCount}件\n更新: {updatedCount}件",
            "保存完了",
            MessageBoxButtons.OK,
            MessageBoxIcon.Information
        );
    }
}