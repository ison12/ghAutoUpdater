using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace GitHubAutoUpdater.Utils
{
    /// <summary>
    /// ファイルユーティリティ。
    /// </summary>
    public static class FileUtil
    {
        /// <summary>
        /// 実行ファイルパスの取得。
        /// </summary>
        /// <returns>実行ファイルパス</returns>
        public static string GetExecuteFilePath()
        {
            Assembly myAssembly = Assembly.GetExecutingAssembly();
            string path = myAssembly.Location;

            return path;
        }

        /// <summary>
        /// 実行ディレクトリパスの取得。
        /// </summary>
        /// <returns>実行ディレクトリパス</returns>
        public static string GetExecuteDirPath()
        {
            return Path.GetDirectoryName(GetExecuteFilePath());
        }

        /// <summary>
        /// 一時ファイルを生成する。
        /// </summary>
        /// <param name="rootDirPath">ルートディレクトリパス</param>
        /// <returns></returns>
        public static string GetTempFilePath(string rootDirPath)
        {
            string tempFilePath = null;
            while (
                (File.Exists(
                    tempFilePath = Path.Combine(rootDirPath, Path.GetRandomFileName())
                 ))
            )
            {
                // 一意なファイルができるまでループする
            }

            return tempFilePath;
        }

        /// <summary>
        /// 一時ディレクトリを生成する。
        /// </summary>
        /// <param name="rootDirPath">ルートディレクトリパス</param>
        /// <returns></returns>
        public static string CreateTempDirectory(string rootDirPath)
        {
            string tempDirPath = null;
            while (
                (Directory.Exists(
                    tempDirPath = Path.Combine(rootDirPath, Path.GetRandomFileName())
                 ))
            )
            {
                // 一意なファイルができるまでループする
            }
            Directory.CreateDirectory(tempDirPath);

            return tempDirPath;
        }

        /// <summary>
        /// ファイルを検索する。
        /// 再帰的にディレクトリを探索する場合は、isRecursive = true を設定する。
        /// </summary>
        /// <param name="dir">対象ディレクトリ</param>
        /// <param name="matchFunc">ファイルのマッチング処理</param>
        /// <param name="isRecursive">再帰処理</param>
        /// <returns>ヒットしたファイル</returns>
        public static List<string> FindFiles(string dir, Func<string, bool> matchFunc, bool isRecursive = true)
        {
            var ret = new List<string>();

            if (!Directory.Exists(dir))
            {
                return ret;
            }

            string[] files = Directory.GetFiles(dir);
            foreach (string s in files)
            {
                if (matchFunc(s))
                {
                    ret.Add(s);
                }
            }

            // 再帰的にディレクトリを探索する
            if (isRecursive)
            {

                string[] dirs = Directory.GetDirectories(dir);
                foreach (string s in dirs)
                {
                    ret.AddRange(FindFiles(s, matchFunc, isRecursive));
                }
            }

            return ret;
        }

        /// <summary>
        /// ディレクトリを検索する。
        /// 再帰的にディレクトリを探索する場合は、isRecursive = true を設定する。
        /// </summary>
        /// <param name="dir">対象ディレクトリ</param>
        /// <param name="matchFunc">ディレクトリのマッチング処理</param>
        /// <param name="isRecursive">再帰処理</param>
        /// <returns>ヒットしたディレクトリ</returns>
        public static List<string> FindDirs(string dir, Func<string, bool> matchFunc, bool isRecursive = true)
        {
            var ret = new List<string>();

            if (!Directory.Exists(dir))
            {
                return ret;
            }

            string[] dirs = Directory.GetDirectories(dir);
            foreach (string s in dirs)
            {
                if (matchFunc(s))
                {
                    ret.Add(s);
                }

                // 再帰的にディレクトリを探索する
                if (isRecursive)
                {
                    ret.AddRange(FindDirs(s, matchFunc));
                }
            }

            return ret;
        }

        /// <summary>
        /// ファイルの内容を読み込む。
        /// </summary>
        /// <param name="filePath">ファイルパス</param>
        /// <param name="encoding">エンコーディング</param>
        /// <returns>ファイルの内容</returns>
        public static string ReadFileContents(string filePath, Encoding encoding = null)
        {
            string contents = null;

            if (encoding == null)
            {
                encoding = new UTF8Encoding(false);
            }

            if (!File.Exists(filePath))
            {
                // ファイルが存在しない場合は、処理を中断
                return contents;
            }

            using (var sr = new StreamReader(filePath, encoding))
            {
                contents = sr.ReadToEnd();
            }

            return contents;
        }

        /// <summary>
        /// ファイルの内容を書き込む。
        /// </summary>
        /// <param name="filePath">ファイルパス</param>
        /// <param name="contents">コンテンツ</param>
        /// <param name="encoding">エンコーディング</param>
        public static void WriteFileContents(string filePath, string contents, Encoding encoding = null)
        {
            if (encoding == null)
            {
                encoding = new UTF8Encoding(false);
            }

            // ディレクトリを予め生成する
            Directory.CreateDirectory(Path.GetDirectoryName(filePath));

            using (var sw = new StreamWriter(filePath, false, encoding))
            {
                sw.Write(contents);
            }
        }
    }
}
