#if UNITY_EDITOR && !COMPILER_UDONSHARP
using UnityEngine;
using UnityEditor;
using VRC.Udon;
using VRC.Udon.Common.Interfaces;
using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using System.Collections.Generic;
using System.Threading.Tasks;

public class UdonChannelSetup : MonoBehaviour
{
    public UdonBehaviour udonBehaviour;
    public string YoutubeListID = "";
}

[CustomEditor(typeof(UdonChannelSetup))]
public class UdonChannelSetupEditor : Editor
{
    UdonChannelSetup UdonChannelSetup;

    private void OnEnable()
    {
        UdonChannelSetup = (UdonChannelSetup)target;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        EditorGUILayout.Space();
        EditorGUILayout.Space();

        // Symbolを取得出来なければボタンを無効化
        using (new EditorGUI.DisabledScope(!IsSetupReady(UdonChannelSetup)))
        {
            if (GUILayout.Button("Setup"))
            {
                var task = Youtubeset(UdonChannelSetup.udonBehaviour, UdonChannelSetup.YoutubeListID);
                Debug.Log("セット開始");
            }
        }
    }

    private void SetupUdonBehaviour(UdonBehaviour udonBehaviour, string exportedSymbol)
    {
        // PublicVariables取得
        IUdonVariableTable publicVariables = udonBehaviour.publicVariables;

        var audioSources = GameObject.FindObjectsOfType<AudioSource>();

        // Undo用意
        Undo.RecordObject(udonBehaviour, "Modify Public Variable");
        // 値をPublicVariablesに設定
        if (!publicVariables.TrySetVariableValue(exportedSymbol, audioSources))
        {
            Debug.Log("Error! Failed Setting Public Variables.");
            return;
        }

        Debug.Log("Setup Completed.");
    }

    private bool IsSetupReady(UdonChannelSetup UdonChannelSetup)
    {
        // Symbolテーブル取得
        IUdonSymbolTable symbolTable = UdonChannelSetup.udonBehaviour?.programSource?.SerializedProgramAsset?.RetrieveProgram()?.SymbolTable;
        if (symbolTable == null)
            return false;

        // Symbolが存在しなければFalse
        if (!symbolTable.HasExportedSymbol("titles"))
            return false;

        // Symbolの型がstring[]でなければFalse
        if (symbolTable.GetSymbolType("titles") != typeof(string[]))
            return false;

        return true;
    }
    static async Task Youtubeset(UdonBehaviour udonBehaviour,string youtubelistid)
    {
        Debug.Log("セット開始");
        var youtubeService = new YouTubeService(new BaseClientService.Initializer()
        {
            ApiKey = ""
        });

        var searchListRequest = youtubeService.PlaylistItems.List("snippet");
        searchListRequest.PlaylistId = youtubelistid;
        searchListRequest.MaxResults = 50;
        Debug.Log("YoutubeAPI");

        List<string> titles = new List<string>();
        List<string> urlString = new List<string>();
        do
        {
            var searchListResponse = await searchListRequest.ExecuteAsync();
            Debug.Log("取得");
            
            foreach (var searchResult in searchListResponse.Items)
            {

                Debug.Log($"{searchResult.Snippet.ResourceId.VideoId}, {searchResult.Snippet.Title}");
                titles.Add($"{searchResult.Snippet.Title.Normarize()}");
                urlString.Add("https://youtu.be/" + $"{searchResult.Snippet.ResourceId.VideoId}");

            }
            searchListRequest.PageToken = searchListResponse.NextPageToken;

        } while (searchListRequest.PageToken != null && searchListRequest.PageToken != "");
        int nagasa = titles.Count;

        // PublicVariables取得
        IUdonVariableTable publicVariables = udonBehaviour.publicVariables;

        // Undo用意
        Undo.RecordObject(udonBehaviour, "Modify Public Variable");
        // 値をPublicVariablesに設定
        if (!publicVariables.TrySetVariableValue("titles", titles.ToArray()))
        {
            Debug.Log("タイトルをセットできません");
            return;
        }
        if (!publicVariables.TrySetVariableValue("urlString", urlString.ToArray()))
        {
            Debug.Log("URLをセットできません");
            return;
        }
        if (!publicVariables.TrySetVariableValue("dummy", new string[nagasa]))
        {
            Debug.Log("長さをセットできません");
            return;
        }
        if (!publicVariables.TrySetVariableValue("descriptions", new string[nagasa]))
        {
            Debug.Log("長さをセットできません");
            return;
        }
        if (!publicVariables.TrySetVariableValue("playMode", new int[nagasa]))
        {
            Debug.Log("長さをセットできません");
            return;
        }
    }
}
#endif
