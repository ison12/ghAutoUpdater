using GitHubAutoUpdater.Configs;
using GitHubAutoUpdater.Datas;
using GitHubAutoUpdater.Logs;
using GitHubAutoUpdater.Services;
using GitHubAutoUpdater.Utils;
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

namespace GitHubAutoUpdater.Forms
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
        /// 進行状況フォームのアップデートキャンセル有無
        /// </summary>
        private bool isUpdateCancel = false;

        /// <summary>
        /// アップデート成功有無
        /// </summary>
        private bool isUpdateSuccess = false;

        /// <summary>
        /// バージョンチェック成功有無
        /// </summary>
        private bool isVersionCheckSuccess = false;

        /// <summary>
        /// 最新リリース情報
        /// </summary>
        private GitHubReleaseLatestInfo releaseLatestInfo;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public FMain()
        {
            logger = new SimpleLogger(Path.Combine(FileUtil.GetExecuteDirPath(), "Logs", DateTime.Now.ToString("yyyy-MM-dd") + ".log"));
            logger.Open();

            appConfig = new AppConfig();
            appConfig.Read();

            InitializeComponent();
        }

        /// <summary>
        /// インスタンスが破棄された際の処理。
        /// </summary>
        public void Destroy()
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
        private void FMain_Load(object sender, EventArgs e)
        {
            this.Text = appConfig.ApplicationName + " " + "最新バージョンのアップデート";

            txtLocalVersion.Text = string.Empty;
            txtLatestVersion.Text = string.Empty;

            progressForm = new FProgress();
            progressForm.CancelEvent += () => { };
            progressForm.Show(
                  appConfig.ApplicationName + " " + "進行状況"
                , "バージョンチェック中"
                , this
                , false  // キャンセル有無
                , OnCheckVersion // バージョンチェック処理は進行状況フォームの表示が確定してから実行する
                );

            if (isVersionCheckSuccess)
            {
                // バージョンチェック成功時
            }
        }

        /// <summary>
        /// フォームクローズ時の処理。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FMain_FormClosed(object sender, FormClosedEventArgs e)
        {
            Destroy();
        }

        /// <summary>
        /// アップデートボタン押下時のイベントプロシージャ
        /// </summary>
        /// <param name="sender">発生元</param>
        /// <param name="e">イベントオブジェクト</param>
        private void btnUpdate_Click(object sender, EventArgs e)
        {

            if (MessageBox.Show(this
                , "最新バージョンにアップデートしてもよろしいですか？"
                , "アップデート確認"
                , MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.No)
            {
                return;
            }

            if (!CheckProcessToBeStoppedWhenUpdate(out var launchingProcesses))
            {
                // 起動中のプロセスがあるため、処理を中断
                MessageBox.Show(this
                    , string.Format(
                        "アップデートの際に終了が必要なプロセスが起動しています。\n以下のプロセスを終了後、再度ボタンをクリックしてください。\n\n{0}"
                        , string.Join("\n", launchingProcesses.ToArray()))
                    , "アップデートエラー"
                    , MessageBoxButtons.OK, MessageBoxIcon.Warning);

                return;
            }

            isUpdateCancel = false;

            progressForm = new FProgress();
            progressForm.CancelEvent += () =>
            {
                isUpdateCancel = true;
            };
            progressForm.Show(
                  appConfig.ApplicationName + " " + "進行状況"
                , "ファイルダウンロード中"
                , this
                , true // キャンセル有無
                , OnUpdate // バージョンチェック処理は進行状況フォームの表示が確定してから実行する
                );

            if (isUpdateCancel)
            {
                MessageBox.Show(this
                    , "アップデートがキャンセルされました。"
                    , "アップデートキャンセル"
                    , MessageBoxButtons.OK, MessageBoxIcon.Information);

            }
            else if (isUpdateSuccess)
            {
                logger.Info(string.Format("Update complete. ver: {0} >> {1}", txtLocalVersion.Text, txtLatestVersion.Text));

                MessageBox.Show(this
                    , "アップデートが完了しました。"
                    , "アップデート完了"
                    , MessageBoxButtons.OK, MessageBoxIcon.Information);

                Close();
            }

        }

        /// <summary>
        /// 閉じるボタン押下時のイベントプロシージャ
        /// </summary>
        /// <param name="sender">発生元</param>
        /// <param name="e">イベントオブジェクト</param>
        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// 停止されているべきプロセスが存在するかをチェックする。
        /// </summary>
        /// <param name="launchingProcesses">起動中のプロセス</param>
        /// <returns>true プロセスが存在しないのでOK、false プロセスが存在するのでNG</returns>
        private bool CheckProcessToBeStoppedWhenUpdate(out List<string> launchingProcesses)
        {
            var ret = true;
            launchingProcesses = new List<string>();

            foreach (var item in appConfig.UpdateCheckProcesses)
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
                /*
                 * ローカルアプリのバージョン情報を取得
                 */
                var appVersoinGetter = new AppVersionGetter(appConfig);
                var appVersionLocal = appVersoinGetter.GetFromLocalApp();

                /*
                 * github上の最新リリース情報を取得
                 */
                using (var gitHubReleasesLatest = new GitHubReleasesLatest(this.appConfig))
                {
                    var releaseLatestResponse = await gitHubReleasesLatest.FetchReleaseLatestInfo(); // 非同期なので取得まで待機する

                    if (releaseLatestResponse.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        // 正常
                        var responseContent = await releaseLatestResponse.Content.ReadAsStringAsync(); // 非同期なので取得まで待機する
                        releaseLatestInfo = JsonUtil.Deserialize<GitHubReleaseLatestInfo>(responseContent);
                    }
                    else
                    {
                        // エラー発生
                        throw new Exception(string.Format("最新リリース情報取得時のエラー … HTTPステータスコード:{0}", releaseLatestResponse.StatusCode));
                    }
                }

                // 最新アプリのバージョン情報を取得
                var appVersionLatest = new VersionInfo(releaseLatestInfo.TagName);

                // アプリケーションバージョンチェックを実施する
                var appVersionChecker = new AppVersionChecker(appConfig);
                var needUpdate = appVersionChecker.NeedUpdate(
                    appVersionLocal,
                    appVersionLatest);

                if (needUpdate)
                {
                    // 更新が必要な場合
                    btnUpdate.Enabled = true;
                }
                else
                {
                    // 更新不要
                    btnUpdate.Enabled = false;
                }

                // UIに反映
                txtLocalVersion.Text = appVersionLocal.GetVersion();
                txtLatestVersion.Text = appVersionLatest.GetVersion();

                isVersionCheckSuccess = true;
            }
            catch (Exception ex)
            {
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

                MessageBox.Show(this
                    , "バージョンチェック中にエラーが発生しました。\n" +
                      "ネットワーク通信に失敗したか、設定ファイルなどに不備があります。\n\n" +
                      "詳細はログファイルを確認してください。\n" +
                      "ファイルパス:" + logger.FilePath
                    , "バージョンチェックエラー"
                    , MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                // 進行状況フォームを閉じる
                progressForm.Close();
            }
        }

        /// <summary>
        /// アップデート処理。
        /// </summary>
        private async void OnUpdate()
        {
            isUpdateSuccess = false;

            // 本アプリの実行ファイルのディレクトリパス
            var rootDirPath = FileUtil.GetExecuteDirPath();

            // 一時ディレクトリパスを生成
            var downloadDirPath = FileUtil.CreateTempDirectory(rootDirPath);
            // ファイルパスリスト
            var downloadFilePathList = new List<string>();

            // 一時バージョンファイルパス
            var tempVersionFilePath = FileUtil.GetTempFilePath(rootDirPath);

            // アップデートスクリプトファイルパス
            var updateScriptFilePath = string.Empty;
            // アプリケーションファイルパス
            var applicationFilePath = Path.Combine(rootDirPath, appConfig.ApplicationFilePath);
            // アプリケーションディレクトリパス
            var applicationDirPath = Path.GetDirectoryName(applicationFilePath);
            // アプリケーションバージョンファイルパス
            var applicationVersionFilePath = Path.Combine(rootDirPath, appConfig.ApplicationVersionFilePath);

            try
            {
                /*
                 * ファイルのダウンロード
                 */

                // github上の最新リリース情報を取得
                using (var gitHubReleasesLatest = new GitHubReleasesLatest(this.appConfig))
                {
                    int assetsIndex = 0;

                    foreach (var assetItem in releaseLatestInfo.Assets)
                    {
                        var name = assetItem.Name;
                        var contentType = assetItem.ContentType;
                        var size = assetItem.Size;
                        var browserDownloadUrl = assetItem.BrowserDownloadUrl;

                        progressForm.SetMessage(string.Format("ダウンロード数：{0} / {1} ... {2}", (assetsIndex + 1), releaseLatestInfo.Assets.Count, name));
                        progressForm.ChangeBlocks(size);

                        HttpResponseMessage releaseLatestResponse = await gitHubReleasesLatest.DownloadReleaseLatestFile(browserDownloadUrl); // 非同期なので取得まで待機する

                        if (isUpdateCancel)
                        {
                            // キャンセル
                            return;
                        }

                        if (releaseLatestResponse.StatusCode == System.Net.HttpStatusCode.OK)
                        {
                            // 正常
                            var responseContent = await releaseLatestResponse.Content.ReadAsStreamAsync(); // 非同期なので取得まで待機する

                            if (isUpdateCancel)
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
                                    if (isUpdateCancel)
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

                if (isUpdateCancel)
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
                        progressForm.SetMessage(string.Format("ZIPファイル解凍中 ... {0}", Path.GetFileName(item)));
                        progressForm.ChangeMarquee();

                        ZipFile.ExtractToDirectory(item, Path.Combine(Path.GetDirectoryName(item), Path.GetFileNameWithoutExtension(item)));
                    }
                }

                if (isUpdateCancel)
                {
                    // キャンセル
                    return;
                }

                /*
                 * セキュリティブロックの解除
                 */
                progressForm.SetMessage("セキュリティロックの解除中");
                progressForm.ChangeMarquee();

                var securityUnblockExitCode = ProcessExecuteUtil.Execute(
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

                if (isUpdateCancel)
                {
                    // キャンセル
                    return;
                }

                /*
                 * ダウンロードしたファイルからアプリケーションファイルを取得する
                 */
                var downloadApplicationFilePathList = FileUtil.FindFiles(downloadDirPath
                    , (filePath) =>
                    {
                        if (Path.GetFileName(filePath) == Path.GetFileName(appConfig.ApplicationFilePath))
                        {
                            return true;
                        }

                        return false;
                    }
                    , true);

                /*
                 * アップデートスクリプトの検索
                 */
                if (downloadApplicationFilePathList.Count <= 0)
                {
                    // エラー
                    throw new Exception(string.Format("アプリケーションファイルの検索エラー … アプリケーションファイルパス:{0}", appConfig.ApplicationFilePath));
                }

                var downloadApplicationDirPath = string.Empty;
                foreach (var downloadApplicationFilePath in downloadApplicationFilePathList)
                {
                    downloadApplicationDirPath = Path.GetDirectoryName(downloadApplicationFilePath);
                    var tempFilePath = Path.Combine(downloadApplicationDirPath, appConfig.UpdateScriptFilePath);
                    if (File.Exists(tempFilePath))
                    {
                        // OK
                        updateScriptFilePath = tempFilePath;
                        break;
                    }
                }

                if (string.IsNullOrEmpty(updateScriptFilePath))
                {
                    // エラー
                    throw new Exception(string.Format("アップデートスクリプトの検索エラー … アップデートスクリプトファイルパス:{0}", appConfig.UpdateScriptFilePath));
                }

                /*
                 * アップデートスクリプトの実行
                 */
                progressForm.SetMessage("アップデートの実行中");
                progressForm.ChangeMarquee();

                var updateScriptArgs = string.Format("\"{0}\" \"{1}\" \"{2}\""
                          , applicationDirPath.TrimEnd('\\')
                          , downloadDirPath.TrimEnd('\\')
                          , downloadApplicationDirPath.TrimEnd('\\'));

                logger.Info(string.Format("アップデートスクリプトの実行 ... {0} {1}"
                      , updateScriptFilePath
                      , updateScriptArgs
                      ));

                var updateScriptExitCode = ProcessExecuteUtil.Execute(
                      updateScriptFilePath // 実行ファイル
                    , updateScriptArgs // 実行ファイルの引数
                    , out string updateScriptOutput // 標準出力
                    , out string updateScriptError // 標準エラー出力
                    , false // ユーザープロファイルのロード
                    );

                if (updateScriptExitCode != 0)
                {
                    // エラー発生
                    throw new Exception(string.Format("アップデートスクリプト実行時のエラー … エラーコード:{0}", updateScriptExitCode));
                }

                /*
                 * バージョンファイルの更新
                 */
                progressForm.SetMessage("バージョンファイルの更新");
                progressForm.ChangeMarquee();

                FileUtil.WriteFileContents(tempVersionFilePath, releaseLatestInfo.TagName);

                if (File.Exists(applicationVersionFilePath))
                {
                    File.Delete(applicationVersionFilePath);
                }
                File.Move(tempVersionFilePath, applicationVersionFilePath);

                isUpdateSuccess = true;
            }
            catch (Exception ex)
            {
                if (ex is AggregateException)
                {
                    foreach (var e in ((AggregateException)ex).InnerExceptions)
                    {
                        logger.Error(e, "Update error");
                    }
                }
                else
                {
                    logger.Error(ex, "Update error");
                }

                MessageBox.Show(this
                    , "アップデート中にエラーが発生しました。\n\n" +
                      "詳細はログファイルを確認してください。\n" +
                      "ファイルパス:" + logger.FilePath
                    , "アップデートエラー"
                    , MessageBoxButtons.OK, MessageBoxIcon.Error);
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