using GhAutoUpdater.Configs;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace GhAutoUpdater.Services
{
    /// <summary>
    /// GitHubの最新のリリース情報の取得
    /// </summary>
    public class GitHubReleasesLatest : IDisposable
    {
        /// <summary>
        /// アプリケーション設定
        /// </summary>
        private AppConfig appConfig;

        /// <summary>
        /// ウェブリクエスト
        /// </summary>
        private WebRequest webRequest;

        /// <summary>
        /// コンストラクタ。
        /// </summary>
        /// <param name="appConfig">アプリケーション設定</param>
        public GitHubReleasesLatest(AppConfig appConfig)
        {
            this.appConfig = appConfig;
            this.webRequest = new WebRequest();
        }

        /// <summary>
        /// 破棄処理
        /// </summary>
        public void Dispose()
        {
            if (webRequest != null)
            {
                webRequest.Dispose();
            }
        }

        /// <summary>
        /// 最新リリースのレスポンス情報を取得する。
        /// </summary>
        /// <returns>最新リリースのレスポンス情報</returns>
        public Task<HttpResponseMessage> FetchReleaseLatestInfo()
        {
            var responseContent = webRequest.GetAsync(
                appConfig.GithubApiRootUrl.TrimEnd('/') + '/' +
                appConfig.ReleasesLatestUri.Replace(":owner", appConfig.ReleasesLatestOwner)
                                           .Replace(":repo", appConfig.ReleasesLatestRepo)
                , new Dictionary<string, object>());

            return responseContent;
        }

        /// <summary>
        /// 最新リリースのバイナリレスポンス情報を取得する。
        /// </summary>
        /// <returns>最新リリースのバイナリレスポンス情報</returns>
        public Task<HttpResponseMessage> DownloadReleaseLatestFile(string url)
        {
            var responseContent = webRequest.GetAsync(
                url, new Dictionary<string, object>());

            return responseContent;
        }
    }
}
