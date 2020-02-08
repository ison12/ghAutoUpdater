using GhAutoUpdater.Configs;
using GhAutoUpdater.Datas;
using GhAutoUpdater.Logs;
using GhAutoUpdater.Services;
using GhAutoUpdater.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GhAutoUpdater.Forms
{
    /// <summary>
    /// メインフォーム
    /// </summary>
    public partial class FMain : Form
    {
        /// <summary>
        /// ロガー
        /// </summary>
        private SimpleLogger logger;

        /// <summary>
        /// アプリケーション設定
        /// </summary>
        private AppConfig appConfig;

        /// <summary>
        /// 進行状況フォーム
        /// </summary>
        private FProgress progressForm;

        /// <summary>
        /// 進行状況フォームのインストールキャンセル有無
        /// </summary>
        private bool isInstallCancel = false;

        /// <summary>
        /// バージョンチェック成功有無
        /// </summary>
        private bool isVersionCheckSuccess = false;

        /// <summary>
        /// インストール成功有無
        /// </summary>
        private bool isInstallSuccess = false;

        /// <summary>
        /// 最新リリース情報
        /// </summary>
        private dynamic releaseLatestInfo;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public FMain()
        {
            logger = new SimpleLogger(Path.Combine(FileUtil.GetExecuteDirPath(), "GhAutoUpdaterLogs", DateTime.Now.ToString("yyyy-MM-dd") + ".log"));
            logger.Open();

            appConfig = new AppConfig();
            appConfig.Read();

            InitializeComponent();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FMain_Load(object sender, EventArgs e)
        {
            this.Text = appConfig.ApplicationName;

            txtLocalVersion.Text = string.Empty;
            txtLatestVersion.Text = string.Empty;

            progressForm = new FProgress();
            progressForm.CancelEvent += () => { };
            progressForm.Show(
                  appConfig.ApplicationName
                , "バージョンチェック中"
                , this
                , false  // キャンセル有無
                , OnCheckVersion // バージョンチェック処理は進行状況フォームの表示が確定してから実行する
                );

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FMain_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (logger != null)
            {
                logger.Close();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnInstall_Click(object sender, EventArgs e)
        {

            if (MessageBox.Show(this
                , "最新のバージョンをインストールしてもよろしいですか？"
                , "インストール確認"
                , MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.No)
            {
                return;
            }

            if (!CheckProcessToBeStoppedWhenInstall(out var launchingProcesses))
            {
                // 起動中のプロセスがあるため、処理を中断
                MessageBox.Show(this
                    , string.Format(
                        "インストールの際に終了が必要なプロセスが起動しています。\n以下のプロセスを終了後、再度ボタンをクリックしてください。\n\n{0}"
                        , string.Join("\n", launchingProcesses.ToArray()))
                    , "インストールエラー"
                    , MessageBoxButtons.OK, MessageBoxIcon.Warning);

                return;
            }

            isInstallCancel = false;

            progressForm = new FProgress();
            progressForm.CancelEvent += () =>
            {
                isInstallCancel = true;
            };
            progressForm.Show(
                  appConfig.ApplicationName
                , "ファイルダウンロード中"
                , this
                , true // キャンセル有無
                , OnInstall // バージョンチェック処理は進行状況フォームの表示が確定してから実行する
                );

            if (isInstallCancel)
            {
                MessageBox.Show(this
                    , "インストールがキャンセルされました。"
                    , "インストールキャンセル"
                    , MessageBoxButtons.OK, MessageBoxIcon.Information);

            }
            else if (isInstallSuccess)
            {
                logger.Info(string.Format("Install complete. ver: {0} >> {1}", txtLocalVersion.Text, txtLatestVersion.Text));

                MessageBox.Show(this
                    , "インストールが完了しました。"
                    , "インストール完了"
                    , MessageBoxButtons.OK, MessageBoxIcon.Information);

                Close();
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// 停止されているべきプロセスが存在するかをチェックする。
        /// </summary>
        /// <param name="launchingProcesses">起動中のプロセス</param>
        /// <returns>true プロセスが存在しないのでOK、false プロセスが存在するのでNG</returns>     
        private bool CheckProcessToBeStoppedWhenInstall(out List<string> launchingProcesses)
        {
            var ret = true;
            launchingProcesses = new List<string>();

            foreach (var item in appConfig.InstallCheckProcesses)
            {
                Process[] ps = Process.GetProcessesByName(item);

                if (ps.Length > 0)
                {
                    ret = false;
                    launchingProcesses.Add(item);
                }
            }

            return ret;
        }

        /// <summary>
        /// バージョンをチェックする。
        /// </summary>
        private async void OnCheckVersion()
        {
            isVersionCheckSuccess = false;

            try
            {
                // ローカルアプリのバージョン情報を取得
                var appVersoinGetter = new AppVersionGetter(appConfig);
                var appVersionLocal = appVersoinGetter.GetFromLocalApp();

                // github上の最新リリース情報を取得
                using (var gitHubReleasesLatest = new GitHubReleasesLatest(this.appConfig))
                {
                    var releaseLatestResponse = await gitHubReleasesLatest.FetchReleaseLatestInfo(); // 非同期なので取得まで待機する

                    if (releaseLatestResponse.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        // 正常
                        var responseContent = await releaseLatestResponse.Content.ReadAsStringAsync(); // 非同期なので取得まで待機する
                        releaseLatestInfo = JsonConvert.DeserializeObject<dynamic>(responseContent);
                    }
                    else
                    {
                        // エラー発生
                        throw new Exception(string.Format("最新リリース情報取得時のエラー … HTTPステータスコード:{0}", releaseLatestResponse.StatusCode));
                    }
                }

                // 最新アプリのバージョン情報を取得
                var appVersionLatest = new VersionInfo(releaseLatestInfo.tag_name.ToString());

                // アプリケーションバージョンチェックを実施する
                var appVersionChecker = new AppVersionChecker(appConfig);
                var needUpdate = appVersionChecker.NeedUpdate(
                    appVersionLocal,
                    appVersionLatest);

                if (needUpdate)
                {
                    // 更新が必要な場合
                    btnInstall.Enabled = true;
                }
                else
                {
                    // 更新不要
                    btnInstall.Enabled = false;
                }

                // UIに反映
                txtLocalVersion.Text = appVersionLocal.GetVersion();
                txtLatestVersion.Text = appVersionLatest.GetVersion();

                isVersionCheckSuccess = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(this
                    , "バージョンチェック中にエラーが発生しました。\n" +
                      "ネットワーク通信に失敗したか、設定ファイルなどに不備があります。\n\n" +
                      "詳細はログファイルを確認してください。\n" +
                      "ファイルパス:" + logger.FilePath
                    , "バージョンチェックエラー"
                    , MessageBoxButtons.OK, MessageBoxIcon.Error);

                if (ex is AggregateException)
                {
                    foreach (var e in ((AggregateException)ex).InnerExceptions)
                    {
                        logger.Error(e, "Version check error");
                    }
                }
                else
                {
                    logger.Error(ex, "Version check error");
                }
            } finally
            {
                // 進行状況フォームを閉じる
                progressForm.Close();
            }
        }

        private async void OnInstall()
        {
            isInstallSuccess = false;

            var rootDirPath = FileUtil.GetExecuteDirPath();

            // 一時ディレクトリパスを生成
            var downloadDirPath = FileUtil.CreateTempDirectory(rootDirPath);
            // ファイルパスリスト
            var downloadFilePathList = new List<string>();

            // 一時バージョンファイルパス
            var tempVersionFilePath = FileUtil.GetTempFilePath(rootDirPath);

            // インストールスクリプトファイルパス
            var installScriptFilePath = Path.Combine(rootDirPath, appConfig.InstallScriptFilePath.TrimStart('\\'));
            // アプリケーションファイルパス
            var applicationFilePath = Path.Combine(rootDirPath, appConfig.ApplicationFilePath.TrimStart('\\'));
            // アプリケーションディレクトリパス
            var applicationDirPath = Path.GetDirectoryName(applicationFilePath);
            // アプリケーションバージョンファイルパス
            var applicationVersionFilePath = Path.Combine(rootDirPath, appConfig.ApplicationVersionFilePath.TrimStart('\\'));

            try
            {
                /*
                 * ファイルのダウンロード
                 */

                // github上の最新リリース情報を取得
                using (var gitHubReleasesLatest = new GitHubReleasesLatest(this.appConfig))
                {
                    int assetsIndex = 0;

                    foreach (JObject assetItem in releaseLatestInfo.assets)
                    {
                        var name = assetItem["name"].ToString();
                        var contentType = assetItem["content_type"].ToString();
                        var size = assetItem["size"].ToString();
                        var browserDownloadUrl = assetItem["browser_download_url"].ToString();

                        progressForm.SetMessage(string.Format("{0} / {1} ... Download {2}", (assetsIndex + 1), releaseLatestInfo.assets.Count, name));
                        progressForm.ChangeBlocks(int.Parse(size));

                        HttpResponseMessage releaseLatestResponse = await gitHubReleasesLatest.DownloadReleaseLatestFile(browserDownloadUrl); // 非同期なので取得まで待機する

                        if (isInstallCancel)
                        {
                            // キャンセル
                            return;
                        }

                        if (releaseLatestResponse.StatusCode == System.Net.HttpStatusCode.OK)
                        {
                            // 正常
                            var responseContent = await releaseLatestResponse.Content.ReadAsStreamAsync(); // 非同期なので取得まで待機する

                            if (isInstallCancel)
                            {
                                // キャンセル
                                return;
                            }

                            // 一時ファイルに書き込む
                            var downloadFilePath = Path.Combine(downloadDirPath, name);
                            using (var writer = new BinaryWriter(new FileStream(downloadFilePath, FileMode.Create)))
                            {
                                // レスポンスからデータを読み込む
                                int readByte = 0;
                                byte[] b = new byte[1024];
                                while ((readByte = await responseContent.ReadAsync(b, 0, b.Length)) != 0) // 非同期なので取得まで待機する
                                {
                                    if (isInstallCancel)
                                    {
                                        // キャンセル
                                        return;
                                    }

                                    writer.Write(b, 0, readByte);

                                    progressForm.IncValue(readByte);
                                }
                            }

                            downloadFilePathList.Add(downloadFilePath);

                        }
                        else
                        {
                            // エラー発生
                            throw new Exception(string.Format("ファイルダウンロード時のエラー … HTTPステータスコード:{0}", releaseLatestResponse.StatusCode));
                        }

                        assetsIndex++;
                    }

                }

                if (isInstallCancel)
                {
                    // キャンセル
                    return;
                }

                /*
                 * ZIPファイルの解凍
                 */
                foreach (var item in downloadFilePathList)
                {
                    if (Path.GetExtension(item) == ".zip")
                    {
                        progressForm.SetMessage("ZIPファイル解凍中");
                        progressForm.ChangeMarquee();

                        ZipFile.ExtractToDirectory(item, Path.Combine(Path.GetDirectoryName(item), Path.GetFileNameWithoutExtension(item)));
                    }
                }

                if (isInstallCancel)
                {
                    // キャンセル
                    return;
                }

                /*
                 * セキュリティブロックの解除
                 */
                progressForm.SetMessage("セキュリティロックの解除中");
                progressForm.ChangeMarquee();

                var securityUnblockExitCode = ProcessExecutor.Execute(
                      "powershell" // 実行ファイル
                    , string.Format("-Command \"Get-ChildItem '{0}\\*.*' -Recurse | Unblock-File\"", downloadDirPath.TrimEnd('\\')) // 実行ファイルの引数
                    , out string securityUnblockOutput // 標準出力
                    , out string securityUnblockError // 標準エラー出力
                    , false // ユーザープロファイルのロード
                    );

                if (securityUnblockExitCode != 0)
                {
                    // エラー発生
                    throw new Exception(string.Format("セキュリティブロック解除時のエラー … エラーコード:{0}", securityUnblockExitCode));
                }

                if (isInstallCancel)
                {
                    // キャンセル
                    return;
                }

                /*
                 * インストールスクリプトの実行
                 */
                progressForm.SetMessage("インストールの実行中");
                progressForm.ChangeMarquee();

                var installScriptExitCode = ProcessExecutor.Execute(
                      installScriptFilePath // 実行ファイル
                    , string.Format("\"{0}\" \"{1}\"", applicationDirPath.TrimEnd('\\'), downloadDirPath.TrimEnd('\\')) // 実行ファイルの引数
                    , out string installScriptOutput // 標準出力
                    , out string installScriptError // 標準エラー出力
                    , false // ユーザープロファイルのロード
                    );

                if (installScriptExitCode != 0)
                {
                    // エラー発生
                    throw new Exception(string.Format("インストールスクリプト実行時のエラー … エラーコード:{0}", installScriptExitCode));
                }

                /*
                 * バージョンファイルの更新
                 */
                progressForm.SetMessage("バージョンファイルの更新");
                progressForm.ChangeMarquee();

                FileUtil.WriteFileContents(tempVersionFilePath, releaseLatestInfo.tag_name.ToString());

                if (File.Exists(applicationVersionFilePath))
                {
                    File.Delete(applicationVersionFilePath);
                }
                File.Move(tempVersionFilePath, applicationVersionFilePath);

                isInstallSuccess = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(this
                    , "インストール中にエラーが発生しました。\n\n" +
                      "詳細はログファイルを確認してください。\n" +
                      "ファイルパス:" + logger.FilePath
                    , "インストールエラー"
                    , MessageBoxButtons.OK, MessageBoxIcon.Error);

                if (ex is AggregateException)
                {
                    foreach (var e in ((AggregateException)ex).InnerExceptions)
                    {
                        logger.Error(e, "Install error");
                    }
                }
                else
                {
                    logger.Error(ex, "Install error");
                }
            }
            finally
            {
                // 後処理
                if (Directory.Exists(downloadDirPath))
                {
                    Directory.Delete(downloadDirPath, true);
                }

                if (File.Exists(tempVersionFilePath))
                {
                    File.Delete(tempVersionFilePath);
                }

                // 進行状況フォームを閉じる
                progressForm.Close();
            }
        }
    }
}