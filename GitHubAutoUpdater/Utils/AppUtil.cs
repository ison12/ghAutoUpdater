using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace GitHubAutoUpdater.Utils
{
    /// <summary>
    /// アプリケーションユーティリティ。
    /// </summary>
    public static class AppUtil
    {
        /// <summary>
        /// アプリケーション名を取得。
        /// </summary>
        /// <returns>アプリケーション名</returns>
        public static string GetAppName()
        {
            // 自分自身のAssemblyを取得
            System.Reflection.Assembly asm =
                System.Reflection.Assembly.GetExecutingAssembly();

            return asm.GetName().Name;
        }

        /// <summary>
        /// アプリケーションバージョンを取得。
        /// </summary>
        /// <returns>アプリケーションバージョン</returns>
        public static string GetAppVersion()
        {
            // 自分自身のAssemblyを取得
            System.Reflection.Assembly asm =
                System.Reflection.Assembly.GetExecutingAssembly();

            return asm.GetName().Version.ToString();
        }
    }
}
