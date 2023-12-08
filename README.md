# City Rescue Multi Play
### PLATEAU-SDK-Toolkits-for-Unityを使ったサンプルアプリケーション

PLATEAU-SDK-Toolkits for Unityを用いたマルチプレイアプリケーションの作成方法についてご紹介します。

### 更新履歴

|  2023/12/13  |  City Rescue Multi Play　初回リリース|
| :--- | :--- |


# 目次

- [1. サンプルシーンの概要](#1-サンプルシーンの概要)
    * [1-1. 体験の概要](#1-1-体験の概要)
    * [1-2. Toolkitsの利用機能](#1-2-Toolkitsの利用機能)
- [2. 利用手順](#2-利用手順)
    * [2-1. 必要環境](#2-1-必要環境)
    * [2-2. Unity Gaming Servicesとは](#2-2-Unity-Gaming-Servicesとは)
    * [2-3. Unity Game Serviceを使うためのプロジェクトセットアップ](#2-3-Unity-Gaming-Servicesを使うためのプロジェクトセットアップ)
    * [2-4. サンプルシーンのビルド方法](#2-4-サンプルシーンのビルド方法)
    * [2-5. サンプルシーンの使い方](#2-5-サンプルシーンの使い方)
- [3. サンプルシーンのカスタマイズ方法](#3-サンプルシーンのカスタマイズ方法)
    * [3-1. PLATEAU都市モデルや配置物の変更](#3-1-PLATEAU都市モデルや配置物の変更)

 
# 1. サンプルシーンの概要
## 1-1. 体験の概要
複数人向けの災害シミュレータを例に取り、PLATEAUの3D都市モデルとマルチプレイ機能を組み合わせることで複数人で同時参加可能なアプリを構築する方法を紹介します。<br/>
このサンプルシーンは、3D都市モデルのメタデータを参照して洪水のシミュレーションを行い、被害状況や避難計画などを複数人でプランニングする体験となっています。<br/>
マルチプレイ機能の実装には様々な方法がありますが、このサンプルシーンでは例としてUnity Gaming Servicesを活用しています。<br/>
ロビー機能やボイスチャットやテキストチャットなどマルチプレイに必要な機能が一サービスとして揃っているのでマルチプレイ機能をシンプルに開発できるサービスです。
<br/>

## 1-2. 利用しているToolkitsの機能
このサンプルシーンでは、PLATEAU SDK-Toolkits for Unityの以下の機能を活用しています。
### Rendering Toolkit
- Auto Texturing
- 環境システム

### Maps Toolkit
- Cesiumを使ったPLATEAUモデルの位置合わせ
- Cesiumを使ったラスターオーバーレイ

  
# 2. 利用手順
## 2-1. 必要環境
### OS環境
- Windows11
- macOS Ventura 13.2

### Unity Version
- Unity 2021.3.30

### Rendering Pipeline
- HDRP
    - Built-in Rendering Pipeline、URPでは動作しません。<br>

### その他
- Unity Cloud アカウント, Unity Gaming Services
  - 本サンプルではUnityが公式に提供している「Unity Gaming Services」を使っています。
  - 本サンプルの利用では一定のデータ量を使用するまで無料で使うことができ、自動的に課金に移行されることはありません。
※クレジットカードなど決済手段の登録は不要で機能をお試しいただけます。


## 2-2. Unity Gaming Servicesとは
Unityが提供する、ゲームの機能開発からローンチ、運用に関わる機能を包括的に提供するソリューションです。<br/>
詳細は以下のドキュメントをご参照ください。<br/>
[Unity Gaming Services](https://unity.com/solutions/gaming-services)

このサンプルシーンではUnity Gaming Servicesを用いて以下の機能を実装しています。<br/>
- Lobby・ルーム機能
- チャット・ボイスチャット機能

## 2-3. Unity Game Serviceを使うためのプロジェクトセットアップ

本サンプルを利用するためには、Unity Gaming Servicesをセットアップする必要があります。
以下の手順に沿って、ご自身でUnity Cloudプロジェクトを作っていただき、IDを入力してください。

※Unity Editor内において事前にご自身のUnityアカウントでログインしておく必要がありますのでご注意ください。

1. [Unity Cloudのログイン](https://cloud.unity.com/home/login)ページにアクセスし. "Create account"を選びます。既にアカウントをお持ちの場合は"Sign in"を選択してください。
<img width="600" alt="multiplay_sample_ugs_createaccount" src="https://github.com/unity-takeuchi/PLATEAU-SDK-Toolkits-for-Unity-drafts/blob/main/SampleSceneReadmeImages/CityRescueMultiPlay/multiplay_sample_ugs_createaccount.png">

2. メールアドレス, パスワードなどの必要情報を入れて"Create Unity ID"を押下します。
<img width="600" alt="multiplay_sample_ugs_createaccount_inputinfo" src="https://github.com/unity-takeuchi/PLATEAU-SDK-Toolkits-for-Unity-drafts/blob/main/SampleSceneReadmeImages/CityRescueMultiPlay/multiplay_sample_ugs_createaccount_inputinfo.png">

3. メール確認画面が表示されるので届いたメールの確認リンクをクリックします。
<img width="600" alt="multiplay_sample_ugs_mailconfirm" src="https://github.com/unity-takeuchi/PLATEAU-SDK-Toolkits-for-Unity-drafts/blob/main/SampleSceneReadmeImages/CityRescueMultiPlay/multiplay_sample_ugs_mailconfirm.png">

4. ダッシュボード画面に遷移するので、"Create Project"ボタンを押下します。

<img width="600" alt="multiplay_sample_ugs_mailconfirm" src="https://github.com/unity-takeuchi/PLATEAU-SDK-Toolkits-for-Unity-drafts/blob/main/SampleSceneReadmeImages/CityRescueMultiPlay/multiplay_sample_ugs_dashboard_aftercreate.png">

5. 任意のプロジェクト名を入力しプロジェクトを作成します。
<img width="600" alt="multiplay_sample_ugs_prpjectname" src="https://github.com/unity-takeuchi/PLATEAU-SDK-Toolkits-for-Unity-drafts/blob/main/SampleSceneReadmeImages/CityRescueMultiPlay/multiplay_sample_ugs_prpjectname.png">

6. "Products"メニューを選び、"Vivox Text Chat"をLaunchします。
<img width="600" alt="スクリーンショット 2023-12-04 16 22 17" src="https://github.com/unity-shimizu/PLATEAU-Toolkits-Sample-CityRescueMultiPlay/assets/137732437/7322eb66-4cc0-4b36-921c-7b9035866abe">


7. プロジェクトに戻り、"Project ID"をコピーします。
 <img width="600" alt="multiplay_sample_projectidcopy" src="https://github.com/unity-takeuchi/PLATEAU-SDK-Toolkits-for-Unity-drafts/blob/main/SampleSceneReadmeImages/CityRescueMultiPlay/multiplay_sample_projectidcopy.png">

サンプルシーンを利用する前のUnity Gaming Servicesをセットアップする手順は以上です。<br/>



## 2-4. サンプルシーンのビルド方法

ここからはUnity Editorでサンプルシーンのファイルを開いてアプリケーションのビルドを行います。

1. Unityのサンプルプロジェクトに戻り、Assets/Scenes/Sample01_Lobby.unityを開きます。<br>
<img width="600" alt="multiplay_sample_scene" src="https://github.com/unity-takeuchi/PLATEAU-SDK-Toolkits-for-Unity-drafts/blob/main/SampleSceneReadmeImages/CityRescueMultiPlay/multiplay_sample_scene.png">

2. 最初にHDRPに関してのウィザードが表示されることがありますが、閉じてください。
<img width="600" alt="multiplay_sample_hdrpwizard" src="https://github.com/unity-takeuchi/PLATEAU-SDK-Toolkits-for-Unity-drafts/blob/main/SampleSceneReadmeImages/CityRescueMultiPlay/multiplay_sample_hdrpwizard.png">

3. メニューよりFile > Build Settingsを選択します。Windows, Mac以外になっている場合は、Windows, Macを選択して、画面下部にある「Switch Platform」ボタンを押下し、Platformを切り替えます。<br/>
そして、Build Settingsの中から左下にある"Player Settings"を選択を押下してください。<br>
<img width="600" alt="multiplay_sample_buildsettings" src="https://github.com/unity-takeuchi/PLATEAU-SDK-Toolkits-for-Unity-drafts/blob/main/SampleSceneReadmeImages/CityRescueMultiPlay/multiplay_sample_buildsettings.png">


4. "Services"メニューを選択し、"Services General Settings"の中で"New Link"を選択します。
<img width="600" alt="multiplay_sample_ugs_servicesnewlink" src="https://github.com/unity-takeuchi/PLATEAU-SDK-Toolkits-for-Unity-drafts/blob/main/SampleSceneReadmeImages/CityRescueMultiPlay/multiplay_sample_ugs_servicesnewlink.png">


5. "Create a Unity Project id"の中にプルダウンメニューが表示されますので、表示される候補の中から、2-3.で作成したプロジェクトを選択してください。<br/>
これで作成されたUnity Cloud Projectと紐付けがされ、本サンプルシーンでマルチプレイヤー機能を使うことができます。
<img width="600" alt="multiplay_sample_ugs_services_selectprj" src="https://github.com/unity-takeuchi/PLATEAU-SDK-Toolkits-for-Unity-drafts/blob/main/SampleSceneReadmeImages/CityRescueMultiPlay/multiplay_sample_ugs_services_selectprj.png">
   
6. Build Settings画面に戻り、下部にある「Build」ボタンを押下します。<br>
出力先を選択してビルドを開始します。
<img width="600" alt="multiplay_sample_buildsettings" src="https://github.com/unity-takeuchi/PLATEAU-SDK-Toolkits-for-Unity-drafts/blob/main/SampleSceneReadmeImages/CityRescueMultiPlay/multiplay_sample_buildsettings.png">


## 2-5. サンプルシーンの使い方

### ビルドしたアプリケーションの操作方法

#### マルチプレイのホスト側
①ビルドしたアプリケーションを開くと、オープニング画面が表示されます。<br>
「始めましょう」ボタンを押下してください。<br>

<img width="600" alt="miniature_sample_02_markerscan" src="https://github.com/unity-takeuchi/PLATEAU-SDK-Toolkits-for-Unity-drafts/assets/137732437/58a75f71-8f1b-4839-abb2-75c6425e8247">


②入出可能なロビー一覧が表示されます。この時点では何も表示されないので、作成タブを押下してください。
<img width="600" alt="miniature_sample_02_markerscan" src="https://github.com/unity-takeuchi/PLATEAU-SDK-Toolkits-for-Unity-drafts/blob/main/SampleSceneReadmeImages/CityRescueMultiPlay/multiplay_sample_enterlobby.png">

③ルームの作成画面が表示されますので、任意のロビー名を入力して、作成ボタンを押下してください。

<img width="600" alt="multiplay_sample_make_room" src="https://github.com/unity-takeuchi/PLATEAU-SDK-Toolkits-for-Unity-drafts/blob/main/SampleSceneReadmeImages/CityRescueMultiPlay/multiplay_sample_make_room.png">

④ルームメニューが表示されます。他のユーザーが参加するのを待ってから開始ボタンを押下してください。
<img width="600" alt="multiplay_sample_roomrobby" src="https://github.com/unity-takeuchi/PLATEAU-SDK-Toolkits-for-Unity-drafts/blob/main/SampleSceneReadmeImages/CityRescueMultiPlay/multiplay_sample_roomrobby.png">

⑤ローディング画面に遷移した後、メイン画面に遷移します。プレイヤーは飛んでいるドローンに変身します。<br>

<img width="600" alt="multiplay_sample_dialog1" src="https://github.com/unity-takeuchi/PLATEAU-SDK-Toolkits-for-Unity-drafts/assets/137732437/1828e0c1-7a8a-4b58-a6c5-8aad7d536c79">

<img width="600" alt="multiplay_sample_dialog2" src="https://github.com/unity-takeuchi/PLATEAU-SDK-Toolkits-for-Unity-drafts/assets/137732437/44e9c8d4-7c3d-4136-a1ec-226efcfebdc5">

⑥地物を右クリックで選択すると周囲の浸水深が表示されます。<br>

<img width="600" alt="multiplay_sample_dialog1_findbuild" src="https://github.com/unity-takeuchi/PLATEAU-SDK-Toolkits-for-Unity-drafts/assets/137732437/1369fba0-c5d9-4f6e-afe6-efeb6add8a1f">

⑥テキストチャットやボイスチャットを使って、グループメンバーに情報を共有しましょう。<br>
<img width="600" alt="multiplay_sample_chat" src="https://github.com/unity-takeuchi/PLATEAU-SDK-Toolkits-for-Unity-drafts/assets/137732437/10f0c9f9-1faa-46f2-8813-1ca83cfdc176">


⑥下記の操作により、プレイヤー（ドローン）を操作できます。
|  項目 |  操作方法|
| :--- | :--- |
|  前へ進む |  矢印上キーもしくは「W」キー|
|  後へ進む |  矢印下キーもしくは「Z」キー|
|  左へ進む |  矢印左キーもしくは「A」キー|
|  右へ進む |  矢印右キーもしくは「S」キー|
|  上昇する |  「Q」キー|
|  下降する |  「E」キー|
|  向きの変更 |  向きたい方向へ画面上をドラッグ|
|  右クリック |  情報を表示したい地物を選択|

#### マルチプレイのクライアント側

①ビルドしたアプリケーションを開くと、オープニング画面が表示されます。<br>
「始めましょう」ボタンを押下してください。<br>

<img width="600" alt="miniature_sample_02_markerscan" src="https://github.com/unity-takeuchi/PLATEAU-SDK-Toolkits-for-Unity-drafts/assets/137732437/58a75f71-8f1b-4839-abb2-75c6425e8247">

②入出可能なロビー一覧が表示されます。既にホストがルームを作成している場合はルームが表示されるので選択し、「参加」を押下してください。<br>
<img width="600" alt="multiplay_sample_memberjoin" src="https://github.com/unity-takeuchi/PLATEAU-SDK-Toolkits-for-Unity-drafts/blob/main/SampleSceneReadmeImages/CityRescueMultiPlay/multiplay_sample_memberjoin.png">

③同じルームに参加しているメンバーが表示されます。問題なければ「開始」ボタンを押下してください。
<img width="600" alt="multiplay_sample_athermember_join" src="https://github.com/unity-takeuchi/PLATEAU-SDK-Toolkits-for-Unity-drafts/blob/main/SampleSceneReadmeImages/CityRescueMultiPlay/multiplay_sample_athermember_join.png">

④ローディング画面に遷移した後、ゲーム画面に遷移します。プレイヤーは飛んでいるドローンに変身します。
<img width="600" alt="multiplay_sample_dialog1" src="https://github.com/unity-takeuchi/PLATEAU-SDK-Toolkits-for-Unity-drafts/assets/137732437/1828e0c1-7a8a-4b58-a6c5-8aad7d536c79">



# ライセンス
- 本リポジトリはMITライセンスで提供されています。
- 本システムの開発はユニティ・テクノロジーズ・ジャパン株式会社が行っています。
- ソースコードおよび関連ドキュメントの著作権は国土交通省に帰属します。

# 注意事項/利用規約
- 本ツールはベータバージョンです。バグ、動作不安定、予期せぬ挙動等が発生する可能性があり、動作保証はできかねますのでご了承ください。
- 処理をしたあとにToolkitsをアンインストールした場合、建物の表示が壊れるなど挙動がおかしくなる場合がございます。
- 本ツールをアップデートした際は、一度Unity エディタを再起動してご利用ください。
- パフォーマンスの観点から、3D都市モデルをダウンロード・インポートする際は、3㎞2範囲内とすることを推奨しています。
- インポートエリアの広さや地物の種類（建物、道路、災害リスクなど）が増えると処理負荷が高くなる可能性があります。
- 本リポジトリの内容は予告なく変更・削除する可能性があります。
- 本リポジトリの利用により生じた損失及び損害等について、国土交通省はいかなる責任も負わないものとします。


