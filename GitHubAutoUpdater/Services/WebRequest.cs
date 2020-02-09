using GitHubAutoUpdater.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace GitHubAutoUpdater.Services
{
    /// <summary>
    /// ウェブリクエスト
    /// </summary>
    public class WebRequest : IDisposable
    {
        /// <summary>
        /// HttpClient
        /// </summary>
        private HttpClient httpClient;

        /// <summary>
        /// アプリケーション名
        /// </summary>
        private string appName;

        /// <summary>
        /// アプリケーションバージョン
        /// </summary>
        private string appVersion;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public WebRequest()
        {
            this.appName = AppUtil.GetAppName();
            this.appVersion = AppUtil.GetAppVersion();

            this.httpClient = new HttpClient();
        }

        /// <summary>
        /// 破棄処理
        /// </summary>
        public void Dispose()
        {
            if (httpClient != null)
            {
                httpClient.Dispose();
            }
        }

        /// <summary>
        /// GETメソッドでWebリクエストする。
        /// </summary>
        /// <param name="url">URL</param>
        /// <param name="parameters">パラメータ</param>
        /// <param name="timeout">タイムアウト</param>
        /// <returns></returns>
        public Task<HttpResponseMessage> GetAsync(
              string url
            , Dictionary<string, object> parameters
            , TimeSpan? timeout = null)
        {
            var ret = Task.Run(() =>
            {

                if (timeout == null)
                {
                    timeout = TimeSpan.FromSeconds(30);
                }

                var content = string.Empty;

                var urlWithParams = BuildUrl(url, parameters);

                Task<HttpResponseMessage> responseTask = null;

                httpClient.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue(appName, appVersion));
                httpClient.Timeout = timeout.Value;

                responseTask = httpClient.GetAsync(urlWithParams, HttpCompletionOption.ResponseHeadersRead);

                return responseTask;
            });

            return ret;
        }

        /// <summary>
        /// URLを構築する。
        /// </summary>
        /// <param name="url">URL</param>
        /// <param name="parameters">パラメータ</param>
        /// <returns></returns>
        private string BuildUrl(string url, Dictionary<string, object> parameters)
        {
            var ret = new StringBuilder();
            ret.Append(url);

            if (parameters.Count > 0)
            {
                ret.Append("?");
            }

            foreach (var item in parameters)
            {
                ret.Append(HttpUtility.UrlEncode(item.Key) + "=" + HttpUtility.UrlEncode(item.Value != null ? item.Value.ToString() : string.Empty));
            }

            return ret.ToString();
        }
    }
}
