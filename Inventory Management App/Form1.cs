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


        private int CurrentAmount = DEFAULT_CURRENT_AMOUNT; // 現在の数量を保持する変数

        // 時刻
        private DispatcherTimer timer;

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

    }
}
