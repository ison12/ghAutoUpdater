using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace GitHubAutoUpdater.Datas
{
    /// <summary>
    /// GitHub 最新リリース情報（Assets）
    /// </summary>
    [DataContract]
    public class GitHubReleaseLatestAssetInfo
    {
        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "content_type")]
        public string ContentType { get; set; }

        [DataMember(Name = "size")]
        public int Size { get; set; }

        [DataMember(Name = "browser_download_url")]
        public string BrowserDownloadUrl { get; set; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public GitHubReleaseLatestAssetInfo()
        {
        }
    }
}
