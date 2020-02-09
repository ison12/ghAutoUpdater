using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GitHubAutoUpdater.Forms
{
    /// <summary>
    /// 進行状況フォーム
    /// </summary>
    public partial class FProgress : Form
    {
        /// <summary>
        /// 処理のキャンセル
        /// </summary>
        public delegate void OnCancel();

        /// <summary>
        /// キャンセルイベント
        /// </summary>
        public event OnCancel CancelEvent;

        /// <summary>
        /// ロード処理
        /// </summary>
        private Action onLoad;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public FProgress()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 表示処理。
        /// </summary>
        /// <param name="title">タイトル</param>
        /// <param name="message">メッセージ</param>
        /// <param name="owner">所有ウィンドウ</param>
        /// <param name="isCancel">キャンセル有無</param>
        /// <param name="onLoad">ロード処理</param>
        /// <returns></returns>
        public DialogResult Show(string title, string message, IWin32Window owner, bool isCancel, Action onLoad)
        {
            this.Text = title;
            this.lblMessage.Text = message;
            this.onLoad = onLoad;

            btnCancel.Visible = isCancel;

            return ShowDialog(owner);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCancel_Click(object sender, EventArgs e)
        {
            // キャンセルイベントを発生させる
            CancelEvent();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FProgress_Load(object sender, EventArgs e)
        {
            onLoad();
        }

        public void ChangeMarquee()
        {
            progressBar.Style = ProgressBarStyle.Marquee;
            progressBar.Minimum = 0;
            progressBar.Maximum = 100;
            progressBar.Step = 10;
            progressBar.Value = 0;
            Application.DoEvents();
        }

        public void ChangeBlocks(int size)
        {
            progressBar.Style = ProgressBarStyle.Blocks;
            progressBar.Minimum = 0;
            progressBar.Maximum = size;
            progressBar.Step = 1;
            progressBar.Value = 0;
            Application.DoEvents();
        }

        public void IncValue(int incValue)
        {
            progressBar.Value += incValue;
            Application.DoEvents();
        }

        public void SetValue(int value)
        {
            progressBar.Value = value;
            Application.DoEvents();
        }

        public void SetMessage(string message)
        {
            this.lblMessage.Text = message;
            Application.DoEvents();
        }
    }
}
