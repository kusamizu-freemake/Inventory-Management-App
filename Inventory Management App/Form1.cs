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

namespace Inventory_Management_App
{
    public partial class Form1 : Form
    {
        private int Currentamount = 0; // 現在の数量を保持する変数

        public Form1()
        {
            InitializeComponent();
            // 初期値を設定
            textBox1.Text = "0"; 

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
            if (Currentamount < 9999)
            {
                Currentamount++;
                textBox1.Text = Currentamount.ToString(); // 変換
                CommaValue();
            }
        }

        // -ボタンがクリックされたときの処理
        private void MinusButton_Click(object sender, EventArgs e)
        {
            // 数量を1ずつ減らす、最小値は0（負の値にはならない）
            if (Currentamount > 0)
            {
                Currentamount--;
                textBox1.Text = Currentamount.ToString(); // 変換
                CommaValue();
            }

        }

        // テキストボックスからフォーカスが外れた時の処理
        private void TextBox1_Leave(object sender, EventArgs e)
        {
            // カンマを除去して数値のみを取得
            string inputText = textBox1.Text.Replace(",", "");

            // 数値に変換できるか確認
            if (int.TryParse(inputText, out int inputAmount))
            {
                // 0から9999の範囲に制限
                if (inputAmount < 0)
                {
                    Currentamount = 0;
                }
                else if (inputAmount > 9999)
                {
                    Currentamount = 9999;
                }
                else
                {
                    Currentamount = inputAmount;
                }
            }
            else
            {
                // 数値に変換できない場合は0にリセット
                Currentamount = 0;
            }   
            // テキストボックスに現在の数量を表示（カンマ付き）
            CommaValue();
        }

        private void CommaValue()
        {             
            // 3桁区切りのカンマ付きで表示
            textBox1.Text = Currentamount.ToString("N0");
        }   



    }
}
