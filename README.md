# KinelPlaylistにYoutubeのプレイリストを追加するやつ

[VRChat] VideoPlayer For VRChat (SDK3)  
https://booth.pm/ja/items/2758684  
のプレイリストにYoutubeのプレイリストを追加するやつです  

Youtube API v3が必要です Unityで使うにはNugetとかで入れといてください  

# 前提
1. VRChatSDK3-Worlds  
https://vrchat.com/home/download  
2. UdonSharp  
https://github.com/MerlinVR/UdonSharp  
3. [VRChat] VideoPlayer For VRChat (SDK3)  
https://booth.pm/ja/items/2758684  

4. NuGetForUnity  
https://github.com/GlitchEnzo/NuGetForUnity  
5. YouTube Data API v3  
NuGetForUnityを使って入れる  

6. YouTube Data API v3のAPIキー  
https://console.cloud.google.com/apis/dashboard  
プロジェクトを立てて、ライブラリからYouTube Data API v3を有効化し「認証情報」からAPIキーを取得  


# 使い方
86行目にあなたのAPIキーを入れる  

適当なゲームオブジェクトにアタッチ  
KinelPlaylist_tabのListを入れる  
YoutubeのリストIDを入れてセットアップ
