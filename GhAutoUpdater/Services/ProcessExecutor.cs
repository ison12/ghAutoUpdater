using System;
using System.Diagnostics;
using System.Text;

namespace GhAutoUpdater.Services
{
    /// <summary>
    /// プロセス実行。
    /// </summary>
    public class ProcessExecutor
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
        /// <returns>結果コード</returns>
        public static int Execute(string exec, string args, out string output, out string error, Encoding encoding, bool isLoadUserProfile = false)
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

            ps.WaitForExit();

            return ps.ExitCode;
        }

        /// <summary>
        /// 実行処理。
        /// </summary>
        /// <param name="exec">実行ファイル</param>
        /// <param name="args">引数</param>
        /// <param name="output">標準出力</param>
        /// <param name="error">標準エラー</param>
        /// <param name="isLoadUserProfile">ユーザープロファイルロード有無</param>
        /// <returns>結果コード</returns>
        public static int Execute(string exec, string args, out string output, out string error, bool isLoadUserProfile = false)
        {
            return Execute(exec, args, out output, out error, null, isLoadUserProfile);
        }


    }
}
