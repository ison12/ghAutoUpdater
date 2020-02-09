using GitHubAutoUpdater.Utils;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace GitHubAutoUpdater.Configs
{
    /// <summary>
    /// アプリケーション設定ファイル
    /// </summary>
    public class AppConfig
    {
        /// <summary>
        /// デフォルトファイル名
        /// </summary>
        private static readonly string FILE_DEFAULT_NAME = "GitHubAutoUpdater.config";

        /// <summary>
        /// ファイルパス
        /// </summary>
        private string filePath;

        /// <summary>
        /// アプリケーション名
        /// </summary>
        public string ApplicationName { get; set; }

        /// <summary>
        /// アプリケーションファイルパス
        /// </summary>
        public string ApplicationFilePath { get; set; }

        /// <summary>
        /// アプリケーションバージョンファイルパス
        /// </summary>
        public string ApplicationVersionFilePath { get; set; }

        /// <summary>
        /// アップデートスクリプトファイルパス
        /// </summary>
        public string UpdateScriptFilePath { get; set; }

        /// <summary>
        /// アップデートチェック時のプロセスリスト
        /// </summary>
        public List<string> UpdateCheckProcesses { get; set; } = new List<string>();

        /// <summary>
        /// GitHub APIルートURL
        /// </summary>
        public string GithubApiRootUrl { get; set; }

        /// <summary>
        /// GitHub API 最新リリース URL
        /// </summary>
        public string GitHubReleasesLatestUri { get; set; }

        /// <summary>
        /// GitHub API 最新リリース 所有者
        /// </summary>
        public string GitHubReleasesLatestOwner { get; set; }

        /// <summary>
        /// GitHub API 最新リリース リポジトリ
        /// </summary>
        public string GitHubReleasesLatestRepo { get; set; }

        /// <summary>
        /// コンストラクタ。
        /// </summary>
        /// <param name="filePath">ファイルパス</param>
        public AppConfig(string filePath = null)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                // デフォルトのファイルパスを設定
                filePath = Path.Combine(FileUtil.GetExecuteDirPath(), FILE_DEFAULT_NAME);
            }

            this.filePath = filePath;
        }

        /// <summary>
        /// 読み込み処理。
        /// </summary>
        public void Read()
        {
            var xmlDoc = new XmlDocument();
            xmlDoc.XmlResolver = null;
            xmlDoc.Load(filePath);

            var rootNode = xmlDoc.DocumentElement;

            {
                var node = rootNode.SelectSingleNode("applicationName") as XmlElement;
                ApplicationName = node.InnerText;
            }
            {
                var node = rootNode.SelectSingleNode("applicationFilePath") as XmlElement;
                ApplicationFilePath = node.InnerText.TrimStart('\\');
            }
            {
                var node = rootNode.SelectSingleNode("applicationVersionFilePath") as XmlElement;
                ApplicationVersionFilePath = node.InnerText.TrimStart('\\');
            }
            {
                var node = rootNode.SelectSingleNode("updateScriptFilePath") as XmlElement;
                UpdateScriptFilePath = node.InnerText.TrimStart('\\');
            }
            {
                var node = rootNode.SelectSingleNode("github/rootUrl") as XmlElement;
                GithubApiRootUrl = node.InnerText;
            }
            {
                var node = rootNode.SelectSingleNode("github/releasesLatest/uri") as XmlElement;
                GitHubReleasesLatestUri = node.InnerText;
            }
            {
                var node = rootNode.SelectSingleNode("github/releasesLatest/owner") as XmlElement;
                GitHubReleasesLatestOwner = node.InnerText;
            }
            {
                var node = rootNode.SelectSingleNode("github/releasesLatest/repo") as XmlElement;
                GitHubReleasesLatestRepo = node.InnerText;
            }

            {
                UpdateCheckProcesses.Clear();
                foreach (XmlElement item in rootNode.SelectNodes("updateCheckProcesses"))
                {
                    UpdateCheckProcesses.Add(item.InnerText);
                }
            }
        }

    }
}
