# YMM4 Radeon AMF Turbo

Radeonのハードウェアエンコーダーを使うYMM4向け動画出力プラグインです。H.264・HEVC・AV1の映像とAAC音声をMP4に書き出します。SDR・8ビットに対応します。[DisnanaのYMM4 AMFプラグイン v0.2.2](https://github.com/disnana/YMM4_AMF_Plugin)を基にした改変版です。

[最新版のダウンロード（GitHub Releases）](https://github.com/tomotora1223/ymm4-radeon-amf-turbo/releases/latest)

## インストールと使い方

1. プロジェクトを保存してYMM4を終了します。
2. `YMM4-Radeon-AMF-Turbo-v0.4.1-turbo.ymme`を開いてインストールします。関連付けが未設定の場合は、YMM4の「ヘルプ」→「YMM4用拡張子の関連付け」で登録できます。[公式手順](https://manjubox.net/ymm4/faq/plugin/how_to_use/)
3. YMM4の「ファイル」→「動画出力」を開き、「動画出力」の一覧から「Radeon AMF Turbo（MP4）」を選びます。
4. 「コーデック」でH.264・H.265（HEVC）・AV1を選べます。AV1にはGPUとドライバーのハードウェアエンコード対応が必要です。

対象はWindows 11 x64、`IVideoFileWriter3`に対応した.NET 10版YMM4、AMF対応のRadeon GPUとそのドライバーです。幅・高さは偶数にしてください。Visual C++ランタイムが必要な場合はMicrosoftのx64版を使います。[YMM4の配布ページ](https://manjubox.net/ymm4/)・[Visual C++ランタイム](https://learn.microsoft.com/en-us/cpp/windows/latest-supported-vc-redist?view=msvc-170)

手動導入する場合は、`.ymme`をZIPとして展開し、中の`AMFVideoWriterPlugin`フォルダーをYMM4の`user\plugin`へ配置します。Disnana版と同じインストール先を使うため、両版を切り替える場合はフォルダーごと差し替えます。

## 高速化の内容

- GPU直渡しを既定で有効にし、D3D11 VideoProcessorでYMM4の描画面から直接NV12へ変換します。AMFには8枚の所有テクスチャプールから渡します。
- 描画面を直接変換できない場合はRGBの作業用テクスチャを経由します。NV12の初期化に対応しない環境ではAMFのRGB入力経路を使います。
- AACの変換・圧縮を32件の有界キューと専用スレッドへ移し、映像の回収と並行処理します。書き出し完了時にキューを空にしてからMP4を確定します。
- AMFの圧縮バッファから直接NALを読み取り、MP4用の映像サンプルを作成します。

「詳細設定」の「GPU色変換（NV12）」で経路を比較できます。「プロファイリング結果を書き出す」をオンにすると、MP4と同じ場所に`.amf_profile.json`が保存されます。

v0.4.1では、NV12と専用スレッドでのAMF投入を併用するとパイプラインの診断に失敗する不具合を修正しました。

AV1の符号化面にGPUがパディングを加えた場合は、MP4に目的の表示範囲を記録します。Radeon 780Mでは1080pを1082行で符号化するため、表示範囲に対応する再生アプリは下端2行を除いた1920×1080で表示します。

## Radeon 780Mでの測定

同一の合成D2D素材と48 kHzステレオ音声を、60 fps・H.264・標準・12 Mbps VBRで出力しました。1080pは2,400フレーム、4Kは900フレームです。各条件を交互に3回ずつ走らせ、描画からMP4確定までの中央値を比較しました。

| 解像度 | Turbo v0.3 | Turbo v0.4 | 処理速度の増加 |
| --- | ---: | ---: | ---: |
| 1920×1080 | 519.70 fps / 4.62秒 | 609.48 fps / 3.94秒 | 17.3% |
| 3840×2160 | 133.47 fps / 6.74秒 | 184.92 fps / 4.87秒 | 38.6% |

1080pの連続負荷では、同じH.264設定でWindowsのGPU Engineカウンター `Video Codec 0` が平均99.6%、ピーク約100%でした。書き出し中の1秒間隔8サンプルです。実プロジェクトでは、描画や素材のデコード負荷によって書き出し速度とGPU使用率が変わります。

## ソースとライセンス

ソースをビルドする場合はAMFサブモジュールを初期化し、`scripts/Package-Plugin.ps1 -Ymm4Directory <YMM4フォルダー>`を実行します。ビルド環境と検証手順は[CONTRIBUTING.md](https://github.com/tomotora1223/ymm4-radeon-amf-turbo/blob/main/CONTRIBUTING.md)を参照してください。

[MIT License](LICENSE)および[第三者ライセンス](THIRD_PARTY_NOTICES.txt)を同梱しています。原実装は[tarutaru247のYMM4_NVEncPlugin](https://github.com/tarutaru247/YMM4_NVEncPlugin)です。
