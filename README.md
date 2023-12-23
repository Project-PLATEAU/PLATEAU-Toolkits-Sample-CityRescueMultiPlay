# City Rescue Multi Play
### PLATEAU-SDK-Toolkits-for-Unityを使ったサンプルプロジェクト
<img width="1080" alt="cityrescue_kv" src="/Documentation~/Images/cityrescue_kv.png">



### 更新履歴

|  2023/12/25  |  City Rescue Multi Play　初回リリース|
| :--- | :--- |


# 目次

- [1. サンプルプロジェクトの概要](#1-サンプルプロジェクトの概要)
  - [1-1. サンプルプロジェクトで体験できること](#1-1-サンプルプロジェクトで体験できること)
  - [1-2. Unityプロジェクト情報](#1-2-unityプロジェクト情報)
  - [1-3. 利用している PLATEAU SDK-Toolkits の機能](#1-3-利用している-plateau-sdk-toolkits-の機能)
- [2. 利用手順](#2-利用手順)
    * [2-1. 推奨環境](#2-1-推奨環境)
    * [2-2. Unity Gaming Servicesとは](#2-2-Unity-Gaming-Servicesとは)
    * [2-3. Unity Game Serviceを使うためのプロジェクトセットアップ](#2-3-Unity-Gaming-Servicesを使うためのプロジェクトセットアップ)
    * [2-4. サンプルプロジェクトのビルド方法](#2-4-サンプルプロジェクトのビルド方法)
    * [2-5. サンプルプロジェクトの使い方](#2-5-サンプルプロジェクトの使い方)
- [3. サンプルプロジェクトのカスタマイズ方法](#3-サンプルプロジェクトのカスタマイズ方法)
    * [3-1. 3D都市モデルを差し替える方法](#3-1-3D都市モデルを差し替える方法)
    * [3-2. 環境の設定変更](#3-2-環境の設定変更)
    * [3-3. 夜間用テクスチャの設定方法](#3-3-夜間用テクスチャの設定方法)
 
# 1. サンプルプロジェクトの概要
## 1-1. サンプルプロジェクトで体験できること
複数人向けの災害シミュレータを例に取り、PLATEAUの3D都市モデルとマルチプレイ機能を組み合わせることで複数人で同時参加可能なアプリを構築する方法を紹介します。<br/>
このサンプルプロジェクトは、3D都市モデルのメタデータを参照して洪水のシミュレーションを行い、被害状況や避難計画などを複数人でプランニングする体験となっています。<br/>
マルチプレイ機能の実装には様々な方法がありますが、このサンプルプロジェクトでは例としてUnity Gaming Servicesを活用しています。<br/>
ロビー機能やボイスチャット、テキストチャットなどマルチプレイに必要な機能を簡単に実装することができます。
<br/>

## 1-2. Unityプロジェクト情報
### Unity Version
- Unity 2021.3.30

### Rendering Pipeline
- HDRP

※Built-in Rendering Pipeline、URPでは動作しません。<br>

## 1-3. 利用している PLATEAU SDK-Toolkits の機能
このサンプルプロジェクトでは、PLATEAU SDK-Toolkits for Unityの以下の機能を活用しています。
### Rendering Toolkit
- Auto Texturing
- 環境システム

### Maps Toolkit
- Cesiumを使ったPLATEAUモデルの位置合わせ
- Cesiumを使ったラスターオーバーレイ

  
# 2. 利用手順
## 2-1. 推奨環境
### OS環境
以下は本プロジェクトに使用した開発環境です。
- Windows11
- macOS Ventura 13.2

### その他
- Unity Cloud アカウント, Unity Gaming Services
  - 本サンプルではUnityが公式に提供している「Unity Gaming Services」を使っています。
  - 本サンプルの利用では一定のデータ量を使用するまで無料で使うことができ、自動的に課金に移行されることはありません。
※クレジットカードなど決済手段の登録は不要で機能をお試しいただけます。


## 2-2. Unity Gaming Servicesとは
Unityが提供する、ゲームの機能開発からローンチ、運用に関わる機能を包括的に提供するソリューションです。<br/>
詳細は以下のドキュメントをご参照ください。<br/>
[Unity Gaming Services](https://unity.com/solutions/gaming-services)

このサンプルプロジェクトではUnity Gaming Servicesを用いて以下の機能を実装しています。<br/>
- Lobby・ルーム機能
- チャット・ボイスチャット機能

## 2-3. Unity Game Serviceを使うためのプロジェクトセットアップ

本サンプルを利用するためには、Unity Gaming Servicesをセットアップする必要があります。
以下の手順に沿って、ご自身でUnity Cloudプロジェクトを作っていただき、IDを入力してください。

※Unity Editor内において事前にご自身のUnityアカウントでログインしておく必要がありますのでご注意ください。

1. [Unity Cloudのログイン](https://cloud.unity.com/home/login)ページにアクセスし. "Create account"を選びます。既にアカウントをお持ちの場合は"Sign in"を選択してください。
<img width="600" alt="multiplay_sample_ugs_createaccount" src="/Documentation~/Images/multiplay_sample_ugs_createaccount.png">

2. メールアドレス, パスワードなどの必要情報を入れて"Create Unity ID"を押下します。
<img width="600" alt="multiplay_sample_ugs_createaccount_inputinfo" src="/Documentation~/Images/multiplay_sample_ugs_createaccount_inputinfo.png">

3. メール確認画面が表示されるので届いたメールの確認リンクをクリックします。
<img width="600" alt="multiplay_sample_ugs_mailconfirm" src="/Documentation~/Images/multiplay_sample_ugs_mailconfirm.png">

4. ダッシュボード画面に遷移するので、"Create Project"ボタンを押下します。

<img width="600" alt="multiplay_sample_ugs_mailconfirm" src="/Documentation~/Images/multiplay_sample_ugs_dashboard_aftercreate.png">

5. 任意のプロジェクト名を入力しプロジェクトを作成します。
<img width="600" alt="multiplay_sample_ugs_prpjectname" src="/Documentation~/Images/multiplay_sample_ugs_prpjectname.png">

6. "Products"メニューを選び、"Vivox Text Chat"をLaunchします。
<img width="600" alt="multiplay_sample_vivoxlaunch" src="/Documentation~/Images/multiplay_sample_vivoxlaunch.png">


7. プロジェクトに戻り、"Project ID"をコピーします。
 <img width="600" alt="multiplay_sample_projectidcopy" src="/Documentation~/Images/multiplay_sample_projectidcopy.png">

サンプルプロジェクトを利用する前のUnity Gaming Servicesをセットアップする手順は以上です。<br/>



## 2-4. サンプルプロジェクトのビルド方法

ここからはUnity Editorでサンプルプロジェクトのファイルを開いてアプリケーションのビルドを行います。

1. Unityのサンプルプロジェクトに戻り、Assets/Scenes/Sample01_Lobby.unityを開きます。<br>
<img width="600" alt="multiplay_sample_scene" src="/Documentation~/Images/multiplay_sample_scene.png">

2. 最初にHDRPに関してのウィザードが表示されることがありますが、閉じてください。
<img width="600" alt="multiplay_sample_hdrpwizard" src="/Documentation~/Images/multiplay_sample_hdrpwizard.png">

3. メニューよりFile > Build Settingsを選択します。Windows, Mac以外になっている場合は、Windows, Macを選択して、画面下部にある「Switch Platform」ボタンを押下し、Platformを切り替えます。<br/>
そして、Build Settingsの中から左下にある"Player Settings"を選択を押下してください。<br>
<img width="600" alt="multiplay_sample_buildsettings" src="/Documentation~/Images/multiplay_sample_buildsettings.png">


4. "Services"メニューを選択し、"Services General Settings"の中で"New Link"を選択します。
<img width="600" alt="multiplay_sample_ugs_servicesnewlink" src="/Documentation~/Images/multiplay_sample_ugs_servicesnewlink.png">


5. "Create a Unity Project id"の中にプルダウンメニューが表示されますので、表示される候補の中から、2-3.で作成したプロジェクトを選択してください。<br/>
これで作成されたUnity Cloud Projectと紐付けがされ、本サンプルプロジェクトでマルチプレイヤー機能を使うことができます。
<img width="600" alt="multiplay_sample_ugs_services_selectprj" src="/Documentation~/Images/multiplay_sample_ugs_services_selectprj.png">
   
6. Build Settings画面に戻り、下部にある「Build」ボタンを押下します。<br>
出力先を選択してビルドを開始します。
<img width="600" alt="multiplay_sample_buildsettings" src="/Documentation~/Images/multiplay_sample_buildsettings.png">


## 2-5. サンプルプロジェクトの使い方

### ビルドしたアプリケーションの操作方法

#### マルチプレイのホスト側
①ビルドしたアプリケーションを開くと、オープニング画面が表示されます。<br>
「始めましょう」ボタンを押下してください。<br>

<img width="600" alt="multiplay_sample_start" src="/Documentation~/Images/multiplay_sample_start.png">


②入出可能なロビー一覧が表示されます。この時点では何も表示されないので、作成タブを押下してください。
<img width="600" alt="multiplay_sample_enterlobby" src="/Documentation~/Images/multiplay_sample_enterlobby.png">

③ルームの作成画面が表示されますので、任意のロビー名を入力して、作成ボタンを押下してください。

<img width="600" alt="multiplay_sample_make_room" src="/Documentation~/Images/multiplay_sample_make_room.png">

④ルームメニューが表示されます。他のユーザーが参加するのを待ってから開始ボタンを押下してください。
<img width="600" alt="multiplay_sample_roomrobby" src="/Documentation~/Images/multiplay_sample_roomrobby.png">

⑤ローディング画面に遷移した後、メイン画面に遷移します。プレイヤーは飛んでいるドローンに変身します。<br>

<img width="600" alt="multiplay_sample_dialog1" src="/Documentation~/Images/multiplay_sample_dialog1.png">

<img width="600" alt="multiplay_sample_dialog2" src="/Documentation~/Images/multiplay_sample_dialog2.png">

⑥地物を右クリックで選択すると周囲の浸水深が表示されます。<br>

<img width="600" alt="multiplay_sample_dialog1_findbuild" src="/Documentation~/Images/multiplay_sample_dialog1_findbuild.png">

⑥テキストチャットやボイスチャットを使って、グループメンバーに情報を共有しましょう。<br>
<img width="600" alt="multiplay_sample_chat" src="/Documentation~/Images/multiplay_sample_chat.png">


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

<img width="600" alt="multiplay_sample_clientstart" src="/Documentation~/Images/multiplay_sample_clientstart.png">

②入出可能なロビー一覧が表示されます。既にホストがルームを作成している場合はルームが表示されるので選択し、「参加」を押下してください。<br>
<img width="600" alt="multiplay_sample_memberjoin" src="/Documentation~/Images/multiplay_sample_memberjoin.png">

③同じルームに参加しているメンバーが表示されます。問題なければ「開始」ボタンを押下してください。
<img width="600" alt="multiplay_sample_athermember_join" src="/Documentation~/Images/multiplay_sample_athermember_join.png">

④ローディング画面に遷移した後、ゲーム画面に遷移します。プレイヤーは飛んでいるドローンに変身します。
<img width="600" alt="multiplay_sample_dialog1" src="/Documentation~/Images/multiplay_sample_dialog1.png">


# 3. サンプルプロジェクトのカスタマイズ方法
## 3-1. 3D都市モデルを差し替える方法
このサンプルプロジェクトでは、例として沼津市の3D都市モデルを配置しています。<br>
PLATEAU SDKを用いて3D都市モデルを読み込むことで、別の地域向けにサンプルプロジェクトをカスタマイズすることができます。<br>
(※浸水区域情報があるモデルについては3D都市モデルの差し替えで情報が反映されますが、浸水区域情報がないモデルを選択すると正しく表示されないのでご注意ください。)

### シーンの構成
このサンプルプロジェクトでは、Githubのファイル容量の制約上、1シーン当たり100MBを超えないように地物の数を限定しています。<br>
ランタイムで複数のサブシーンを重ねてロードすることで広域のモデル表示を実現しています。シーンファイルの構成は以下の通りです。<br>
<img width="600" alt="multiplay_scene_structure" src="/Documentation~/Images/multiplay_scene_structure.png"> <br>
1. Sample01＿Lobby：マルチプレイの準備、ロビー作成などを行うシーンです。
2. FloodSimulation ：マルチプレイでロードされるゲームレベルです。その中には地形の航空写真と浸水領域モデルが配置されています。
3. Buildings 1~5：1シーンあたり100MBを超えないように地物の数を限定しています。この5つのシーンをランライム中に重ねてロードしています。

### サブシーン読み込みの無効化
3D都市モデルを差し替えて利用される際、サブシーンの分割は不要のため、以下の手順でサブシーンの読み込みを無効化してください。<br>
`Assets/Scripts/GameLobby/NGO/PlayerCam.cs` 
を開き、112~115行目の以下のコードを削除してください。<br>
<img width="600" alt="multiplay_sample_customize_playercam" src="/Documentation~/Images/multiplay_sample_customize_playercam.png"> <br>
こちらのコードがサブシーンのロードを行う部分なので、削除するとサブシーンがロードされなくなります。

### 3D都市モデルの配置
FloodSimulationシーンを開き、PLATEAU SDK for Unityを用いて利用したいエリアの3D都市モデルを読み込みます。
具体的な使い方は[PLATEAU SDK for Unityのマニュアル](https://project-plateau.github.io/PLATEAU-SDK-for-Unity/)をご参照ください。

読み込んだ3D都市モデルを図のようにCesiumGeoreferenceのチャイルドオブジェクトにします。<br>
<img width="300" alt="multiplay_sample_customize_hierarchy" src="/Documentation~/Images/multiplay_sample_customize_hierarchy.png"> <br>

Cesium Globe Anchorコンポーネントを追加します。<br>
<img width="300" alt="multiplay_sample_customize_cga" src="/Documentation~/Images/multiplay_sample_customize_cga.png"> <br>

Maps Toolkitを開き、配置した3D都市モデルをアタッチして「PLATEAUモデルの位置を合せる」を選択すると、3D都市モデルの位置が補正されます。<br>
<img width="600" alt="multiplay_sample_customize_alignment" src="/Documentation~/Images/multiplay_sample_customize_alignment.png"> <br>

Maps Toolkitの詳しい使い方は[Maps ToolkitのReadme](https://github.com/PLATEAU-Toolkits-Internal/PLATEAU-SDK-Maps-Toolkit-for-Unity)をご参照ください。

## 3-2. 環境の設定変更
デフォルトでは晴れ・日中の環境に設定されています。<br>
Rendering Toolkitの環境システムを利用することで天候条件や時間帯を変更することが出来ます。<br>
メニューからPLATEAU>PLATEAU Toolkit>Rendering Toolkitを開いてください。<br>
<img width="400" alt="multiplay_customize_renderingmenu" src="/Documentation~/Images/multiplay_customize_renderingmenu.png"> <br>

Time of Dayの値を変更することで時間帯を変更出来ます。<br>
<img width="600" alt="multiplay_customize_time_dawn" src="/Documentation~/Images/multiplay_customize_time_dawn.png"> 
<img width="600" alt="multiplay_customize_time_noon" src="/Documentation~/Images/multiplay_customize_time_noon.png"> <br>

Rain/Snow/Cloudy/Fog Color/FogDistanceの値を変更することで天気を調整出来ます。<br>
<img width="600" alt="multiplay_customize_weather_fog" src="/Documentation~/Images/multiplay_customize_weather_fog.png"> 

環境システムの各設定項目の内容や、具体的な操作方法は[Rendering ToolkitのReadme](https://github.com/PLATEAU-Toolkits-Internal/PLATEAU-SDK-Toolkits-for-Unity/tree/main/PlateauToolkit.Rendering)をご参照ください。

## 3-3. 夜間用のテクスチャの設定方法
3-2. 環境の設定変更で時間帯を夜間に設定すると、デフォルトの3D都市モデルでは建物に光源がないため、建物は真っ暗に表示されてしまいます。<br>
<img width="600" alt="multiplay_customize_time_midnight" src="/Documentation~/Images/multiplay_customize_time_midnight.png"> <br>

そのような場合には、Rendering ToolkitのAuto Texturing機能を使用することで、夜景を再現することが出来ます。<br>
まず、Auto Textureを適用したい建物を選択します。<br>
<img width="600" alt="multiplay_customize_select_a_bldg" src="/Documentation~/Images/multiplay_customize_select_a_bldg.png"> <br>

Rendering Toolkitを開いて「テクスチャ生成」を選択すると夜景用のテクスチャが生成されます。<br>
<img width="600" alt="multiplay_customize_autotexture" src="/Documentation~/Images/multiplay_customize_autotexture.png"> <br>

ヒエラルキー内の全建物を一括選択して反映したい場合は、検索バーで「bldg t:MeshRenderer p(m_IsActive)=true」と検索すると一括でシーン内の建物を選択出来ます*。<br>
<img width="600" alt="multiplay_customize_select_all_bldgs" src="/Documentation~/Images/multiplay_customize_select_all_bldgs.png"> <br>
<img width="600" alt="multiplay_customize_select_all_bldgs_light" src="/Documentation~/Images/multiplay_customize_select_all_bldgs_light.png"> <br>

*この検索にはUnity Editor上でのカスタム選択機能が必要となります。カスタム選択機能を使用するには メニュー > Edit > Preferences の SearchタブでSceneの項目をAdvancedに設定する必要があります。<br>
<img width="600" alt="multiplay_customize_custom_search" src="/Documentation~/Images/multiplay_customize_custom_search.png"> 

Auto Texture適用後、窓の明かりをOFFにしたい場合は対象となる建物を選択し、Rendering Toolkitを開いて「窓の表示の切り替え」を選択してください。<br>
<img width="600" alt="multiplay_customize_select_a_bldg_dark" src="/Documentation~/Images/multiplay_customize_select_a_bldg_dark.png"> <br>

設定方法の詳細については[Rendering ToolkitのReadme](https://github.com/PLATEAU-Toolkits-Internal/PLATEAU-SDK-Toolkits-for-Unity/tree/main/PlateauToolkit.Rendering)をご参照ください。


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


