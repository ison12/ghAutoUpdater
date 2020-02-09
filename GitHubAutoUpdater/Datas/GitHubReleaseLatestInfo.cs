using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace GitHubAutoUpdater.Datas
{
    /// <summary>
    /// GitHub 最新リリース情報
    /// </summary>
    [DataContract]
    public class GitHubReleaseLatestInfo
    {
        [DataMember(Name = "tag_name")]
        public string TagName { get; set; }

        [DataMember(Name = "assets")]
        public List<GitHubReleaseLatestAssetInfo> Assets { get; set; } = new List<GitHubReleaseLatestAssetInfo>();

        /// <summary>
        /// コンストラクタ。
        /// </summary>
        public GitHubReleaseLatestInfo()
        {
        }

    }
}
