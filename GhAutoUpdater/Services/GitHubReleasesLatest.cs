using GhAutoUpdater.Configs;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GhAutoUpdater.Services
{
    /// <summary>
    /// GitHubの最新のリリース情報の取得
    /// </summary>
    public class GitHubReleasesLatest
    {
        /// <summary>
        /// アプリケーション設定
        /// </summary>
        private AppConfig appConfig;

        /// <summary>
        /// コンストラクタ。
        /// </summary>
        /// <param name="appConfig">アプリケーション設定</param>
        public GitHubReleasesLatest(AppConfig appConfig)
        {
            this.appConfig = appConfig;
        }

        /// <summary>
        /// 情報を取得する。
        /// </summary>
        /// <returns>情報</returns>
        public dynamic Fetch()
        {
            var webRequest = new WebRequest();
            var responseContent = webRequest.GetAsString(
                appConfig.GithubApiRootUrl.TrimEnd('/') + '/' +
                appConfig.ReleasesLatestUri.Replace(":owner", appConfig.ReleasesLatestOwner)
                                           .Replace(":repo", appConfig.ReleasesLatestRepo)
                , new Dictionary<string, object>());

            var ret = JsonConvert.DeserializeObject<dynamic>(responseContent);
            return ret;
        }
    }
}
