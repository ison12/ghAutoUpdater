using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GhAutoUpdater.Datas
{
    /// <summary>
    /// バージョン情報
    /// </summary>
    public class VersionInfo : IComparable
    {
        /// <summary>
        /// バージョン文字列
        /// </summary>
        private readonly string versionStr;

        /// <summary>
        /// バージョン配列（数値を前提）
        /// </summary>
        private string[] versionArray;

        /// <summary>
        /// コンストラクタ。
        /// </summary>
        /// <param name="versionStr">バージョン文字列</param>
        public VersionInfo(string versionStr)
        {
            this.versionStr = versionStr;
            this.versionArray= this.ConvertVersionStrToIntArray(this.versionStr);
        }

        /// <summary>
        /// バージョン情報文字列を取得する。
        /// </summary>
        /// <returns>バージョン情報文字列</returns>
        public string GetVersion()
        {
            return versionStr;
        }

        /// <summary>
        /// 2つのバージョン情報を比較する。
        /// </summary>
        /// <param name="other">比較対象の情報</param>
        /// <returns>
        /// 　0の場合は、同一。
        /// -1の場合は、own &lt; other。
        /// +1の場合は、own &gt; other。
        /// </returns>
        public int CompareTo(object obj)
        {
            var other = obj as VersionInfo;
            if (other == null)
            {
                return 1;
            }

            string[] infoA;
            string[] infoB;

            bool isNormal = true;

            /*
             * バージョン配列の長さが異なることも想定
             * 例）1.0.0.0 と 1.0.1 の比較など
             */
            if (versionArray.Length <= other.versionArray.Length)
            {
                // 長さが短い方の配列を基準にループする

                // 自身    がA
                // 比較対象がB
                infoA = versionArray;
                infoB = other.versionArray;
                isNormal = true;
            }
            else
            {
                // 長さが短い方の配列を基準にループする

                // 比較対象がA
                // 自身    がB
                infoA = other.versionArray;
                infoB = versionArray;
                isNormal = false;
            }

            for (int indexA = 0; indexA < infoA.Length; indexA++)
            {
                for (int indexB = 0; indexB < infoB.Length; indexB++)
                {
                    if (int.TryParse(infoA[indexA], out int intA) &&
                        int.TryParse(infoB[indexB], out int intB))
                    {
                        // 数値変換可能な場合
                        if (intA < intB)
                        {
                            // a < b
                            return isNormal ? -1 : 1;
                        }
                        else if (intA > intB)
                        {
                            // a > b
                            return isNormal ? 1 : -1;
                        }
                        else
                        {
                            // a = b
                        }

                    }
                    else
                    {
                        // 文字列の場合
                        if (infoA[indexA].CompareTo(infoB[indexB]) < 0)
                        {
                            // a < b
                            return isNormal ? -1 : 1;
                        }
                        else if (infoA[indexA].CompareTo(infoB[indexB]) > 0)
                        {
                            // a > b
                            return isNormal ? 1 : -1;
                        }
                        else
                        {
                            // a = b
                        }
                    }

                }
            }

            if (infoA.Length < infoB.Length)
            {
                // a < b
                return isNormal ? -1 : 1;
            }

            // a = b
            return 0;
        }

        /// <summary>
        /// バージョン文字列をバージョン数値配列に変換する。
        /// </summary>
        /// <param name="versionStr">バージョン文字列</param>
        /// <returns>バージョン数値配列</returns>
        private string[] ConvertVersionStrToIntArray(string versionStr)
        {
            var ret = new string[0];

            if (!string.IsNullOrEmpty(this.versionStr))
            {
                var versoinStrArray = this.versionStr.Split('.');
                ret = new string[versoinStrArray.Length];

                int i = 0;
                foreach (var item in versoinStrArray)
                {
                    if (string.IsNullOrWhiteSpace(item))
                    {
                        ret[i] = "0";
                    } else
                    {
                        ret[i] = item;
                    }

                    i++;
                }
            }

            return ret;
        }
    }
}
