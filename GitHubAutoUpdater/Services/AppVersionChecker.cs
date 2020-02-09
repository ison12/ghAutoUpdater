using GitHubAutoUpdater.Configs;
using GitHubAutoUpdater.Datas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitHubAutoUpdater.Services
{
    /// <summary>
    /// アプリケーションバージョンチェック。
    /// </summary>
    public class AppVersionChecker
    {
        /// <summary>
        /// アプリケーション設定
        /// </summary>
        private AppConfig appConfig;

        /// <summary>
        /// コンストラクタ。
        /// </summary>
        /// <param name="appConfig">アプリケーション設定</param>
        public AppVersionChecker(AppConfig appConfig)
        {
            this.appConfig = appConfig;
        }

        /// <summary>
        /// アップデートの必要性があるかどうかの判定。
        /// </summary>
        /// <param name="appVersionLocal">ローカルのアプリケーションバージョン</param>
        /// <param name="appVersionLatest">最新のアプリケーションバージョン</param>
        /// <returns>true アップデートの必要あり、false アップデート不要</returns>
        public bool NeedUpdate(
            VersionInfo appVersionLocal, 
            VersionInfo appVersionLatest)
        {

            if (appVersionLocal.CompareTo(appVersionLatest) < 0)
            {
                // 最新アプリが存在するのでアップデートの必要あり
                return true;
            }

            // アップデートの必要なし
            return false;

        }
    }
}
