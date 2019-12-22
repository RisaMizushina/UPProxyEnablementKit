# 概要

このツールは、UiPath Studioおよび、UiPath Robotを使う上で、プロキシサーバーを使用して、Orchestratorに接続するための、設定を作成します。

Basic認証など、ID・パスワードを使用した、認証にも対応しています。
（シングルサインオンには、原理的に対応できません）


# 重要な注意事項

このソリューションは、認証つきのプロキシサーバーに対応した、設定ファイル・必要なライブラリを、生成させることができます。ライブラリおよび、設定ファイルの中には、暗号化された状態で、プロキシサーバーの認証情報が、含まれます。

格納された認証情報は、テキストエディタや、バイナリエディタでは、即座に見ることは、できません。
しかし、Windowsプログラミングや、.NET Frameworkの、ある程度の知識があれば、動作を解析することで、認証情報を取り出せます。

これは、認証処理の代行を、ソフトウェアに行わせる上で、不可避な仕様となります。

そのため、以下の点に、ご注意の上、使用してください。
- 生成したファイルの漏洩が、そのまま、認証情報の漏洩につながります。
- 組織の規則との、適合性を確認する必要が、あります。
  - パスワードの保存を、禁止している場合。
  - 保存する際に一定の要件を満たすことを要求される場合。


# 使用する前の準備

NuGetクライアントツールを、ダウンロードして、ProxyEnablementKit.exeと、同じフォルダに置いてください。

入手方法は、以下のドキュメントを、参照してください。

https://docs.microsoft.com/ja-jp/nuget/install-nuget-client-tools

# 使い方
ProxyEnablementKit.exe を、起動します。

## 最初に、Enterprise Editionか、Community Editionかを、選択します。（半角数字の、0 または 1 です）

UiPathの、インストール先が、通常と異なるときは、次に、入力を求められますので、設定してください。

## その後、次の順番で、入力します。

- プロキシサーバー
http://example.com:8080 のような、ポート番号も、含む形式で、入力してください。
- ユーザー名
ユーザー名を、入力します。認証がないときは、空欄のまま、Enterを押してください。
- パスワード
ユーザー名を、入力したら、パスワードも入力します。（確認のため、2回、入力してください）
パスワードは、画面には表示されません。バックスペースキーでの修正はできます。

入力が終わると、いくつかのウィンドウが、一瞬出たり、消えたりします。

処理完了メッセージが出たら、ファイルの生成が、完了しています。

## 生成されたファイルを、コピーして適用します
アプリケーション（ProxyEnablementKit）の、フォルダに、ProxyKitOutput_年月日_日付 という、フォルダが作成されています。

中には、backupと、outputの、フォルダがあります。
- backupの中身は、書き換え対象のファイルを、コピーしただけのものです。
- outputの中身は、UiPathでプロキシを使うための、設定を書き換えた、設定ファイルです。

DirectoryList.txtに、UiPathのディレクトリが、記録されています。

そのディレクトリに、outputフォルダの、中身を上書きしてください。（Enterprise Editionでは、管理者権限が、必須になります）

**ファイルを書き換えたら、Windowsを再起動してください。**
機能が適用されます。


# 謝辞
Masatomi KINO さん（Twitter: @masatomix ）
バグ報告と、説明書の不備（Windows再起動が、必要なことが、わかりにくいこと）のご指摘、ありがとうございました。
