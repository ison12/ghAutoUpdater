using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitHubAutoUpdater.Utils
{
    /// <summary>
    /// アプリケーション二重起動チェック。
    /// </summary>
    public class AppDuplicateChecker : IDisposable
    {
        /// <summary>
        /// ハンドル所有フラグ
        /// </summary>
        private bool hasHandle;

        /// <summary>
        /// ミューテックス名
        /// </summary>
        private string mutexName;

        /// <summary>
        /// ミューテックス
        /// </summary>
        private System.Threading.Mutex mutex;

        /// <summary>
        /// コンストラクタ。
        /// </summary>
        /// <param name="mutexName">ミューテックス名</param>
        public AppDuplicateChecker(string mutexName)
        {
            this.mutexName = mutexName;
        }

        /// <summary>
        /// 二重起動チェック。
        /// </summary>
        /// <returns>true 正常、false 二重起動されている</returns>
        public bool Check()
        {
            mutex = new System.Threading.Mutex(false, mutexName);

            try
            {
                // ミューテックスの所有権を要求する
                hasHandle = mutex.WaitOne(0, false);
            }
            // .NET Framework 2.0以降の場合
            catch (System.Threading.AbandonedMutexException)
            {
                //別のアプリケーションがミューテックスを解放しないで終了した時
                hasHandle = true;
            }

            return hasHandle;
        }

        /// <summary>
        /// リソースの解放
        /// </summary>
        public void Dispose()
        {
            if (mutex == null)
            {
                return;
            }

            if (hasHandle)
            {
                //ミューテックスを解放する
                mutex.ReleaseMutex();
            }
            mutex.Close();
        }
    }
}
