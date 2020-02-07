using GhAutoUpdater.Utils;
using System.IO;
using System.Xml;

namespace GhAutoUpdater.Configs
{
    /// <summary>
    /// アプリケーション設定ファイル
    /// </summary>
    public class AppConfig
    {
        /// <summary>
        /// デフォルトファイル名
        /// </summary>
        private static readonly string FILE_DEFAULT_NAME = "GhAutoUpdater.config";

        /// <summary>
        /// ファイルパス
        /// </summary>
        private string filePath;

        /// <summary>
        /// 
        /// </summary>
        public string ApplicationName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string ApplicationFilePath { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string ApplicationVersionFilePath { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string GithubApiRootUrl { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string ReleasesLatestUri { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string ReleasesLatestOwner { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string ReleasesLatestRepo { get; set; }

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
                ApplicationFilePath = node.InnerText;
            }
            {
                var node = rootNode.SelectSingleNode("applicationVersionFilePath") as XmlElement;
                ApplicationVersionFilePath = node.InnerText;
            }
            {
                var node = rootNode.SelectSingleNode("githubApiRootUrl") as XmlElement;
                GithubApiRootUrl = node.InnerText;
            }
            {
                var node = rootNode.SelectSingleNode("releasesLatest/uri") as XmlElement;
                ReleasesLatestUri = node.InnerText;
            }
            {
                var node = rootNode.SelectSingleNode("releasesLatest/owner") as XmlElement;
                ReleasesLatestOwner = node.InnerText;
            }
            {
                var node = rootNode.SelectSingleNode("releasesLatest/repo") as XmlElement;
                ReleasesLatestRepo = node.InnerText;
            }
        }

    }
}
