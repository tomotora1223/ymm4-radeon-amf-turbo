# ビルドと検証

このリポジトリは[DisnanaのYMM4 AMFプラグイン](https://github.com/disnana/YMM4_AMF_Plugin)を基にした Radeon AMF Turbo のソースです。

## 開発環境

- Windows x64、PowerShell 7、Git
- Visual Studio 2022 または Build Tools の MSBuild、C++ デスクトップ開発ワークロード、MSVC v143、Windows SDK 10.0.26100.0
- .NET 10 SDK
- `IVideoFileWriter3` を含む .NET 10 対応の [YMM4 または YMM4 Lite](https://manjubox.net/ymm4/)
- ハードウェア出力の検証には、AMF対応Radeon、ドライバー、PATH上の `ffmpeg` と `ffprobe`

AMF SDK は Git サブモジュールにバージョンを固定しています。YMM4 の参照 DLL は手元の YMM4 フォルダーから読み込みます。

```powershell
git clone --recurse-submodules https://github.com/tomotora1223/ymm4-radeon-amf-turbo.git
Set-Location ymm4-radeon-amf-turbo
$ymm = 'C:\path\to\YukkuriMovieMaker_v4_Lite'
.\scripts\Build.ps1 -Configuration Release -IncludePlugin -Ymm4Directory $ymm
```

## テスト

```powershell
.\scripts\Run-Tests.ps1 -Suite Unit -Ymm4Directory $ymm
.\scripts\Run-Tests.ps1 -Suite GpuSmoke -Ymm4Directory $ymm
.\scripts\Run-Tests.ps1 -Suite Av1 -Ymm4Directory $ymm
```

`Unit` はビルド、ネイティブと管理側の契約、PowerShellの構文を検証します。`GpuSmoke` は H.264 と HEVC の出力、`Av1` は対応する GPU 上での AV1 出力を検証します。速度を比較する場合は、同じ素材・GPU・画質設定で複数回走らせ、MP4の確定までを測ってください。

## 配布

```powershell
.\scripts\Package-Plugin.ps1 -Configuration Release -Ymm4Directory $ymm
```

配布用 `.ymme` と SHA-256 が `artifacts\release` に作成されます。`.ymme` にはプラグインのDLLとライセンス文書が含まれます。コードの変更は、検証条件と結果を添えて[このリポジトリ](https://github.com/tomotora1223/ymm4-radeon-amf-turbo)の `main` 宛てに提出できます。[MIT License](LICENSE) と [第三者ライセンス](THIRD_PARTY_NOTICES.txt) の表記を維持してください。
