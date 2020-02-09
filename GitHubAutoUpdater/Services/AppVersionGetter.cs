using GitHubAutoUpdater.Configs;
using GitHubAutoUpdater.Datas;
using GitHubAutoUpdater.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitHubAutoUpdater.Services
{
    /// <summary>
    /// アプリケーションバージョン情報の取得。
    /// </summary>
    public class AppVersionGetter
    {
        /// <summary>
        /// アプリケーション設定
        /// </summary>
        private AppConfig appConfig;

        /// <summary>
        /// コンストラクタ。
        /// </summary>
        /// <param name="appConfig">アプリケーション設定</param>
        public AppVersionGetter(AppConfig appConfig)
        {
            this.appConfig = appConfig;
        }

        /// <summary>
        /// ローカルにインストール済みのアプリからバージョン情報を取得する。
        /// </summary>
        /// <returns>バージョン情報</returns>
        public VersionInfo GetFromLocalApp()
        {
            // バージョンファイルパス
            var versionFilePath = Path.Combine(FileUtil.GetExecuteDirPath(), this.appConfig.ApplicationVersionFilePath);

            // バージョンファイルからバージョン文字列を読み込む
            var content = string.Empty;
            if (File.Exists(versionFilePath))
            {
                content = FileUtil.ReadFileContents(versionFilePath).Trim();
            }

            // バージョン情報を生成する
            var versionInfo = new VersionInfo(content);

            return versionInfo;
        }
    }
}
