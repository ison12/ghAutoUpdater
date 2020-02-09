using GitHubAutoUpdater.Forms;
using GitHubAutoUpdater.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GitHubAutoUpdater
{
    static class Program
    {
        /// <summary>
        /// アプリケーションのメイン エントリ ポイントです。
        /// </summary>
        [STAThread]
        static void Main()
        {
            using (var appDuplicateChecker = new AppDuplicateChecker("GitHubAutoUpdater-c6839605-a231-4230-9bc8-48204cb13698"))
            {
                if (!appDuplicateChecker.Check())
                {
                    // 二重起動しているため終了
                    return;
                }

                try
                {
                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);
                    Application.Run(new FMain());
                }
                catch (Exception)
                {
                    throw;
                }
                finally
                {
                }
            }
        }
    }
}
