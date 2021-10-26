# TouhouDCG
## 作品ページ

https://unityroom.com/games/touhou_dcg
## 目次

1. [ゲーム概要](#abstract)
1. [基本設計](#basic)
   1. [概要(名前空間)](#abstract_namespace)
   1. [概要(シーケンス)](#abstract_sequence)
1. [詳細設計](#detail)
   1. [ゲームのセットアップ](#setup)
   1. [カードの移動](#move)
   1. [攻撃](#attack)
   1. [カードのアビリティー](#ability)

<a id="abstract"></a>

## ゲーム概要

このゲームは1対1のカードゲームです。両者のターンが交代に訪れ、自分のターンには以下のアクションを行うことができます。
- コストを支払いカードをフィールドに出す。
- 1コストを支払い、ボムカードを手に入れる
- ボムカードを使用して、フィールドのカードの特殊能力を発動させる
- 自身のフィールドのカードで、相手のカードまたは相手のヒーローを攻撃する。

先に相手のヒーローのHPを0にしたプレイヤーが勝利となります。

<img src="https://framari.org/SunnyMilk/github/TouhouDCG/Images/GameScene.png" width="500">
<br><br>

## 注意
本作品で使用した外部のアセット(素材など)は以下の通りです
- スクリプトを含むアセット
  - Photon Unity Networking 2
  - DOTween (HOTween v2)
  - UniRx
  - Unmask For UGUI
- 画像、音楽、3Dモデル、フォント
  - <a href="https://commons.nicovideo.jp/">ニコニコモンズ</a>
  - <a href="https://twitter.com/takurosusrt">たくろす様</a>
  - <a href="https://pocket-se.info/">ポケットサウンド/効果音素材</a>
  - <a href="https://seiga.nicovideo.jp/seiga/im7755763">はるか様</a>
  - <a href="https://www.ac-illust.com/">イラストAC</a>
  - <a href="https://www.nicovideo.jp/watch/sm17615327">東方のパワーアップアイテム</a>
  - Parisienne
  - Noto Sans Japanese
  - 4500 Cartoon GUI Elements


以上のアセット(または素材)はプロジェクトから取り除いています。

<a id="basic"></a>

## 基本設計

<a id="abstract_namespace"></a>

### 概略(名前空間)

このゲームはコマンドパターンをベースに作成しています。機能(クラス)を大きく分けると”プレイヤー”、”コマンド”、”デバイス”の3つに分割できます。

プレイヤー
このゲーム中には様々な選択肢(どのカードをプレイするか、どのカードを攻撃するかなど)が存在します。これらの中から1つ(または複数)選択する機能を”プレイヤー”に帰属させます。例えばユーザーの入力を受け付けるクラス、COMなどが該当します。

デバイス
プレイヤーの決定を受け、適切にカードに状態変化を引き起こす機能をここに分類します。つまり、ゲームの進行はすべてデバイスが担当します。

コマンド
プレイヤーの決定を一つのクラスのインスタンスで表現し、デバイスに伝達する機能です。例えば手札のカードをフィールドにドラッグアンドドロップ(以下D&D)された際は```Command_PlayCard```クラスのインスタンスが生成され、これをもとにデバイスが処理を行います。

これら三つの関係を図で表現すると下図のようになります。

<img src="https://framari.org/SunnyMilk/github/TouhouDCG/Images/Abstract.png" width="300">

はじめにプレイヤーの意思決定を表現するコマンドクラスのインスタンスが生成されます。これを```CommandManager```クラスに渡します。```CommandManager```は渡されたコマンドの中身をもとに、デバイスを動かします。

最も重要な点はプレイヤーがデバイスと結合していない点です。これによる利点は2つ存在します。
オンラインの通信対戦を容易に実現できる
片方のプレイヤーが入力を行いコマンドクラスのインスタンスが生成されたとします。これをシリアル化して対戦相手に送信し、両端末で同じコマンドを共有すれば、両プレイヤーのデバイスを同様に操作することができます。
AIの作成が容易になる
コマンドが無ければ、デバイスの操作の組み合わせは非常に多くなります。コマンドを間に挟み選択肢を適切に絞ることで、COMが考慮すべき操作を減らすことができます。
<br><br>

<a id="abstract_sequence"></a>

### 概略(シーケンス)

ここではプレイヤーAとBがオンライン対戦を行っている状況を考えます。プレイヤーAがある入力を行った後の処理の流れを説明します。シーケンス図に表すと下図のようになります。

<img src="https://framari.org/SunnyMilk/github/TouhouDCG/Images/Abst_Sequence.png" width="700">

分類子の名前の最後がA(B)のものは、プレイヤーA(B)の端末上に存在するクラスを指します。つまりA-B間のやり取りはオンライン上で行われることになります。

Aの入力はコマンドに直され```CommandManagerA```に渡されます。このコマンドの中身をもとにデバイスが操作されます。この後```NetworkManagerA```がコマンドをシリアル化し、```NetworkManagerB```に送信します。受信したコマンドはデシリアライズされ```CommandManagerB```に渡され、コマンドの中身をもとにデバイスを操作します。


<br><br>

<a id="detail"></a>

## 詳細設計

ゲーム中に行われる主要な動作である"ゲームのセットアップ"、"カードの移動"、"攻撃"、"スキルの発動"の詳細を説明します。

<br>

<a id="setup"></a>

### ゲームのセットアップ

実際にゲームを開始する前に行う作業がいくつか存在します。主要なものとしてデッキを生成、初期手札の決定(デッキから6枚のカードを引きそのうち3枚を初期手札とし、残りをデッキに戻してシャッフル)が挙げられます。オンライン対戦の場合は自分の手札と山札を相手の端末に送信する必要もあります。これらの作業を実現させた設計に関して説明します。まずクラス図とシーケンス図を以下に並べます。

<img src="https://framari.org/SunnyMilk/github/TouhouDCG/Images/Initialize_class.png" width="300">　　<img src="https://framari.org/SunnyMilk/github/TouhouDCG/Images/Initialize_seq_1.png" width="524">

シーケンス図に沿って説明します。ゲームの準備は、ゲームシーンへの遷移完了のコールバックメソッドで行われます。コールバックには二種類あり、一つは「シーン遷移直後(フェードインの前)」もう一つは「フェードイン直後」です。このパッケージは自作したものであり、[github](https://github.com/FraMari495/WBSceneTransition "シーン遷移パッケージ")もしくは[YouTube](https://www.youtube.com/watch?v=I-JgXVaOkEw&t=41s "パッケージの解説動画")にて詳細を説明しています。まずシーン遷移直後のコールバックで、カードのインスタンスを作成します。ユーザーが作成したデッキはカードのデータベースIDを用いて表現されており、この情報をもとにカードオブジェクトを作成します。これが完了するとフェードインが行われゲーム画面が表示されます。この直後に初期手札の選択が開始します。山札の中から6枚のカードが表示されるので、そのうち3枚を選択して初期手札とします。選択が終了すると、選択されなかったカードは山札に戻されシャッフルします。オフラインでのプレイの場合、これで準備が終了です。

オンライン対戦の場合はこの作業を行ったのち、お互いのデッキや初期手札の送受信が必要になります。そのため下図の操作を行います。分類子の名前の最後がA(B)のものは、プレイヤーA(B)の端末上に存在するクラスを指します。


<img src="https://framari.org/SunnyMilk/github/TouhouDCG/Images/Initialize_seq_2.png" width="700">

デッキの交換は、プレイヤー2人のうち片方(シーケンス図では"A"のプレイヤー)が主導して行います。Aのマリガンが終了後、```ConnectionManagerA```にデッキ送信を行うよう要請します。```ConnectionManagerB```が受信すると、```DeckB```に情報を格納します。次にプレイヤーBのデッキを送信します。この時点でプレイヤーBのマリガンが終了していない場合は、Bのマリガン終了を待機したのちに送信します。プレイヤーBはここで準備が終了します。プレイヤーAは受信したデッキを```DeckA```に格納した時点で準備が終了します。

<br><br>

<a id="move"></a>

### カードの移動

関連するクラス図とシーケンス図は下図のようになります。

<img src="https://framari.org/SunnyMilk/github/TouhouDCG/Images/PlayCard_class.png" width="800">

<img src="https://framari.org/SunnyMilk/github/TouhouDCG/Images/CardPlay.png" width="600">

カードが手札にある際はD&Dによってプレイすることが可能です。カードがフィールドに存在する際は、D&Dによって攻撃することが可能です。このようにカードの状態により、D&Dをした際の処理が異なります。そのためカードの入力受付を実装する際にステートパターンを用いています。カードが手札にある場合の例を上図に示しています。カードがD&Dされた場合は```CardPosition```(全てのカードが持っているコンポーネント)の```State.OnEndDrag()```が呼ばれます。ここでは```State```に```State_Hand```のインスタンスが代入されているため、カードが手札にある場合の処理が実行されます。具体的には、```Command_PlayCard```のインスタンスを作成し、```CommandManager```に渡します。すると```Command.Run()```が実行され、```Hand```(手札クラス)の```PlayCard()```が呼ばれます。すると```Hand.Cards```からプレイしたカードが削除され、```Field.Cards```に追加されます。```Hand```は```Status```にイベントを発行させ、その通知を受けて```CardInputHandler```は手札用の見た目に変更します。オンライン対戦の場合、作成されたコマンドをシリアル化し相手に送信します。

<br><br>

<a id="attack"></a>

### カードの攻撃

実装時の考え方は、"カードの移動"とほとんど同じです。そのためクラス図の内容の多くが重複しています。

<img src="https://framari.org/SunnyMilk/github/TouhouDCG/Images/Attack_class.png" width="800">

<img src="https://framari.org/SunnyMilk/github/TouhouDCG/Images/Attack_Sequence.png" width="600">

シーケンス図の内容に沿って説明します。攻撃はフィールドのカードが他のフィールドのカードにD&Dされた際に行われます。ドロップされたカードが、ドラッグ側に自身の情報を渡します。ドラッグ側のカードは```Command_Attack```のインスタンスを生成して```CommandManager```に渡します。```CommandManager```がコマンドの中身を実行します。これにより```Field```クラスの```Attack()```メソッドが呼ばれ、攻撃が開始します。```Field```クラスはダメージ計算を行い、2つのカードの```Status```を更新し、表示内容(カードに掛かれている数値)を変更するようイベントを発行します。

<a id="ability"></a>

### カードのアビリティー

カードのアビリティーが発動するタイミングは4つ存在し、「カードのプレイ時」「ターン終了時」「ボムカードを使用時」「死亡時」です。特にカードのプレイ時のアビリティーに注目して設計の説明を行います。アビリティー周りのクラス図が下図になります。


<img src="https://framari.org/SunnyMilk/github/TouhouDCG/Images/Ability_class.png" width="600">

```OnPlayAbility```クラスに注目します。大切なのは具体的なスキルの効果を表現する```abstract```な```RunAbility(発動者,ターゲット)```メソッドです。新しくアビリティーを実装したい場合は、このクラスを継承したクラスを作成し、```RunAbility()```の中に自由に記述すれば完成します。

オンライン対戦において、カードのアビリティーが発動する流れを説明します。


<img src="https://framari.org/SunnyMilk/github/TouhouDCG/Images/Ability_Sequence.png" width="600">

[カードのプレイに関するシーケンス図](#カードの移動)とほとんど同じですが、```Command_PlayCard```クラスのインスタンスを生成する直前に、アビリティーのターゲットを決定する手順が挟まります("AoE"や"ドロー"のような、ターゲットを必要としないアビリティーの場合はこの手順は踏みません)。手札からカードをプレイした直後に、ターゲットの選択を行う機能をもつクラス(```TargetSelector```)のインスタンスを生成します。選択が完了したのちに```Command_PlayCard```のインスタンスを生成します。この時選択されたターゲットの情報をコマンドに伝えます。このコマンドが実行された際、カードをフィールドに移動させるだけではなく、```OnPlayAbility.Run(発動者,ターゲット)```も行われます。オンライン戦の場合は、このコマンドをシリアル化して対戦相手の端末に送信します。
## Author

白黒Unity

<br>

## License

WBSceneTransition is under [MIT license](https://en.wikipedia.org/wiki/MIT_License).
