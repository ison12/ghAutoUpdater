using GhAutoUpdater.Configs;
using GhAutoUpdater.Datas;
using GhAutoUpdater.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
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
        /// アプリケーション設定
        /// </summary>
        private AppConfig appConfig;

        /// <summary>
        /// 進行状況フォーム
        /// </summary>
        private FProgress progressForm;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public FMain()
        {
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

            progressForm = new FProgress();
            progressForm.CancelEvent += () => { };
            progressForm.Show(
                  appConfig.ApplicationName
                , "バージョンチェック中"
                , this
                , OnCheckVersion // バージョンチェック処理は進行状況フォームの表示が確定してから実行する
                );
        }

        /// <summary>
        /// バージョンをチェックする。
        /// </summary>
        private void OnCheckVersion() {

            var task = Task.Run(() =>
            {
                // ローカルアプリのバージョン情報を取得
                var appVersoinGetter = new AppVersionGetter(appConfig);
                var appVersionLocal = appVersoinGetter.GetFromLocalApp();

                // github上の最新リリース情報からアプリのバージョン情報を取得
                var gitHubReleasesLatest = new GitHubReleasesLatest(this.appConfig);
                var releaseLatestInfo = gitHubReleasesLatest.Fetch();
                var appVersionLatest = new VersionInfo(releaseLatestInfo.tag_name.ToString());

                var appVersionChecker = new AppVersionChecker(appConfig);
                var needUpdate = appVersionChecker.NeedUpdate(
                    appVersionLocal,
                    appVersionLatest);

                Invoke(new Action(() =>
                {

                    if (needUpdate)
                    {
                        btnInstall.Enabled = true;
                    }
                    else
                    {
                        btnInstall.Enabled = false;
                    }

                    txtLocalVersion.Text = appVersionLocal.GetVersion();
                    txtLatestVersion.Text = appVersionLatest.GetVersion();

                    progressForm.Close();
                }));
            });
        }

        private void btnInstall_Click(object sender, EventArgs e)
        {

        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
