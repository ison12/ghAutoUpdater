# GitHubAutoUpdater

## GitHubAutoUpdaterで何ができるのか？
特定の**Windows**アプリケーションをバージョンアップするための、アプリケーションです。

GitHub上の特定のリポジトリの最新のリリース情報を読み取り、バージョンの差異をチェックして、アプリケーションをダウンロードしてアップデートを実施します。

## アプリケーションの簡単な動作説明
簡単な動作説明は以下のとおりです。

 1. GitHub上の特定のリポジトリの”最新のリリース情報”を読み取りバージョン情報を取得します。
 2. ローカルマシンのインストール済みのアプリケーションのバージョンを取得します。
 3. 両者のバージョンを比較して新しいものであるかを確認します。
 4. バージョンが新しければ、GitHub上の最新リリース情報のassetsファイルをダウンロードします。
 5. ダウンロード後に、アップデートを実施します。

## GitHubAutoUpdaterのファイル構成
リポジトリ内の[releases](https://github.com/ison12/gitHubAutoUpdater/releases)からアプリケーションファイル一式を取得してください。

| ファイル名 | 説明 |
|--|--|
| GitHubAutoUpdater.config | アップデートのための設定ファイル（要編集） |
| GitHubAutoUpdater.exe | 実行ファイル |
| GitHubAutoUpdater.exe.config | 実行ファイルの設定ファイル（編集不要） |

## GitHubAutoUpdater.configの設定例
重要なのは、GitHubAutoUpdater.configファイルです。

設定例として、[CliboDone](https://github.com/ison12/cliboDone)をアップデートするための内容を示します。
まずはフォルダ構成からです。

**フォルダ構成**

    CliboDone
    │  CliboDone.config
    │  CliboDone.exe
    │  CliboDone.exe.config
    │  CliboDone.version
    │  Update.bat
    │
    ├─ConvertScripts
    │
    ├─GitHubAutoUpdater
    │  │  GitHubAutoUpdater.config
    │  │  GitHubAutoUpdater.exe
    │  │  GitHubAutoUpdater.exe.config
    │
    └─Manual

ここで重要なのは、GitHubAutoUpdater.exeとCliboDone.exeのお互いの位置関係です。
設定ファイルを以下のようにします。

**設定ファイル**

    <?xml version="1.0" encoding="utf-8" ?>
    <root>
      <applicationName>CliboDone</applicationName>
      <applicationFilePath>..\CliboDone.exe</applicationFilePath> … (1)
      <applicationVersionFilePath>..\CliboDone.version</applicationVersionFilePath> … (2)
      <updateScriptFilePath>Update.bat</updateScriptFilePath> … (3)
      <updateCheckProcesses>
        <process>CliboDone</process> … (4)
      </updateCheckProcesses>
      <github>
        <rootUrl>https://api.github.com</rootUrl>
        <releasesLatest>
          <uri>repos/:owner/:repo/releases/latest</uri>
          <owner>ison12</owner> … (5)
          <repo>cliboDone</repo> … (6)
        </releasesLatest>
      </github>
    </root>

重要な点は、上記の設定ファイル内の番号部分です… **(1)～(6)**

 1. GitHubAutoUpdater.exeから見た、対象アプリ（CliboDone.exe）の相対パス
 2. GitHubAutoUpdater.exeから見た、対象アプリのバージョンファイル（CliboDone.version）の相対パス
 3. 対象アプリ（CliboDone.exe）から見た、アップデートスクリプトの相対パス
 4. アップデート実行時にチェックすべきプロセスの一覧（拡張子は不要）
 5. 対象アプリのGitHub所有者名
 6. 対象アプリのGitHubリポジトリ名

このように設定した上で、GitHubAutoUpdater.exeを実行することでアプリケーションのアップデートが可能になります。

## GitHubAutoUpdaterを使いたい場合にユーザーが用意するもの

**GitHub関連**
 - GitHubアカウント
 - GitHubでWindowsアプリケーションを公開するリポジトリ
  - GitHubのリリース機能でWindowsアプリケーションをリリースすること。リリース時にassetsファイルにアプリケーション一式をzipファイルで含めること。

**Windowsアプリケーションの内容**
 - Windowsアプリケーションをアップデートするためのアップデートスクリプトを用意すること（シェル実行できれば拡張子は問わない）。
 - アップデートスクリプト内で何かエラーが発生した場合は終了コードを0以外にして、正常の場合は終了コードを0にします。
 - Windowsアプリケーションのバージョン情報を格納するためのバージョンファイルを用意すること。こちらのファイルには、アップデートが実行されるたびに、GitHubのリリースタグの番号が更新される。
 - Windowsアプリケーション内にGitHubAutoUpdater.exeを呼び出すメニューなどが設置されていること。
