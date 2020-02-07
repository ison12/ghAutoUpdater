using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace GhAutoUpdater.Services
{
    /// <summary>
    /// 
    /// </summary>
    public class WebRequest
    {
        /// <summary>
        /// 
        /// </summary>
        private string appName;

        /// <summary>
        /// 
        /// </summary>
        private string appVersion;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public WebRequest()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="url"></param>
        /// <param name="parameters"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public string GetAsString(string url, Dictionary<string, object> parameters, TimeSpan? timeout = null)
        {
            //自分自身のAssemblyを取得
            System.Reflection.Assembly asm =
                System.Reflection.Assembly.GetExecutingAssembly();
            //バージョンの取得
            System.Version ver = asm.GetName().Version;
            this.appName = asm.GetName().Name;
            this.appVersion = asm.GetName().Version.ToString();


            return GetAsStringAsync(url, parameters, timeout).Result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="url"></param>
        /// <param name="parameters"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public Task<string> GetAsStringAsync(string url, Dictionary<string, object> parameters, TimeSpan? timeout = null)
        {
            var ret = Task.Run(() => {

                if (timeout == null)
                {
                    timeout = TimeSpan.FromSeconds(30);
                }

                var content = string.Empty;

                var urlWithParams = BuildGetUrl(url, parameters);

                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue(appName, appVersion));
                    client.Timeout = timeout.Value;

                    Task<HttpResponseMessage> responseTask = client.GetAsync(urlWithParams, HttpCompletionOption.ResponseContentRead);
                    var response = responseTask.Result; // 処理終了まで待機する

                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        var contentTask = response.Content.ReadAsStringAsync();
                        content = contentTask.Result; // 処理終了まで待機する
                    }
                }

                return content;
            });

            return ret;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="url"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        private string BuildGetUrl(string url, Dictionary<string, object> parameters)
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
