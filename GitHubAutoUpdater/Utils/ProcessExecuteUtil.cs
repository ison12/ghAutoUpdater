using System;
using System.Diagnostics;
using System.Text;

namespace GitHubAutoUpdater.Utils
{
    /// <summary>
    /// プロセス実行。
    /// </summary>
    public static class ProcessExecuteUtil
    {
        /// <summary>
        /// 実行処理。
        /// </summary>
        /// <param name="exec">実行ファイル</param>
        /// <param name="args">引数</param>
        /// <param name="output">標準出力</param>
        /// <param name="error">標準エラー</param>
        /// <param name="encoding">エンコーディング</param>
        /// <param name="isLoadUserProfile">ユーザープロファイルロード有無</param>
        /// <param name="isSync">同期有無</param>
        /// <returns>結果コード</returns>
        public static int Execute(string exec, string args, out string output, out string error, Encoding encoding, bool isLoadUserProfile = false, bool isSync = true)
        {
            ProcessStartInfo psi = new ProcessStartInfo(exec);
            psi.UseShellExecute = false;
            psi.CreateNoWindow = true;

            psi.Arguments = args;
            psi.Verb = "Runas";
            psi.RedirectStandardOutput = true;
            psi.RedirectStandardError = true;
            psi.LoadUserProfile = isLoadUserProfile;

            if (encoding != null)
            {
                psi.StandardOutputEncoding = encoding;
                psi.StandardErrorEncoding = encoding;
            }

            Process ps = System.Diagnostics.Process.Start(psi);

            output = string.Empty;
            output += exec + " " + args + Environment.NewLine;

            output += ps.StandardOutput.ReadToEnd();
            error = ps.StandardError.ReadToEnd();

            if (isSync)
            {
                ps.WaitForExit();
                return ps.ExitCode;
            }

            return 0;
        }

        /// <summary>
        /// 実行処理。
        /// </summary>
        /// <param name="exec">実行ファイル</param>
        /// <param name="args">引数</param>
        /// <param name="output">標準出力</param>
        /// <param name="error">標準エラー</param>
        /// <param name="isLoadUserProfile">ユーザープロファイルロード有無</param>
        /// <param name="isSync">同期有無</param>
        /// <returns>結果コード</returns>
        public static int Execute(string exec, string args, out string output, out string error, bool isLoadUserProfile = false, bool isSync = true)
        {
            return Execute(exec, args, out output, out error, null, isLoadUserProfile, isSync);
        }
    }
}
