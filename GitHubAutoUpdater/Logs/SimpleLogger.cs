using System;
using System.IO;
using System.Text;

namespace GitHubAutoUpdater.Logs
{
    /// <summary>
    /// ログ出力クラス。
    /// </summary>
    public class SimpleLogger : IDisposable
    {
        /// <summary>
        /// ファイルパス
        /// </summary>
        public string FilePath
        {
            get
            {
                return filePath;
            }
        }

        /// <summary>
        /// ファイルパス。
        /// </summary>
        private string filePath;

        /// <summary>
        /// 区切り文字。
        /// </summary>
        private string separate;

        /// <summary>
        /// ライター。
        /// </summary>
        private StreamWriter writer;

        /// <summary>
        /// コンストラクタ。
        /// </summary>
        /// <param name="filePath">ファイルパス</param>
        /// <param name="separate">区切り文字</param>
        public SimpleLogger(string filePath, string separate = " ")
        {
            this.filePath = filePath;
            this.separate = separate;
        }

        /// <summary>
        /// 破棄処理。
        /// </summary>
        public void Dispose()
        {
            Close();
        }


        /// <summary>
        /// 初期化処理。
        /// </summary>
        public void Open()
        {
            if (writer != null)
            {
                return;
            }

            if (!Directory.Exists(Path.GetDirectoryName(filePath)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(filePath));
            }

            writer = new System.IO.StreamWriter(filePath, true, new UTF8Encoding(false));
        }

        /// <summary>
        /// 後処理。
        /// </summary>
        public void Close()
        {
            if (writer == null)
            {
                return;
            }

            writer.Flush();
            writer.Close();
            writer = null;
        }

        /// <summary>
        /// 出力共通処理。
        /// </summary>
        /// <param name="level">レベル</param>
        /// <param name="message">メッセージ</param>
        private void Write(string level, string message)
        {
            writer.WriteLine(string.Format("{0}" + this.separate + "{1}" + this.separate + "{2}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), level, message));

        }

        /// <summary>
        /// デバッグ出力。
        /// </summary>
        /// <param name="message">メッセージ</param>
        public void Debug(string message)
        {
            Write("DEBUG", message);
        }

        /// <summary>
        /// インフォ出力。
        /// </summary>
        /// <param name="message">メッセージ</param>
        public void Info(string message)
        {
            Write("INFO ", message);
        }

        /// <summary>
        /// ワーニング出力。
        /// </summary>
        /// <param name="message">メッセージ</param>
        public void Warn(string message)
        {
            Write("WARN ", message);
        }

        /// <summary>
        /// エラー出力。
        /// </summary>
        /// <param name="message">メッセージ</param>
        public void Error(string message)
        {
            Write("ERROR", message);
        }

        /// <summary>
        /// エラー出力。
        /// </summary>
        /// <param name="ex">例外オブジェクト</param>
        /// <param name="message">メッセージ</param>
        public void Error(Exception ex, string message = null)
        {

            var exMessage = string.Empty;

            if (message != null)
            {
                message = message + " - ";
            }
            else
            {
                message = string.Empty;
            }

            var innerEx = ex.InnerException;
            if (innerEx != null)
            {
                exMessage = message + ex.Message +
                            Environment.NewLine +
                            ex.StackTrace +
                            Environment.NewLine +
                            "Cause Exception : " + innerEx.Message + Environment.NewLine +
                            innerEx.StackTrace;
            }
            else
            {
                exMessage = message + ex.Message +
                            Environment.NewLine +
                            ex.StackTrace;
            }

            Write("ERROR", exMessage);
        }
    }
}
