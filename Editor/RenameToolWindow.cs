// Copyright (c) 2026 junya005. All rights reserved.

using UnityEditor;
using UnityEngine;

/// <summary>
/// 選択オブジェクトの一括リネームを行うエディターウィンドウ。
/// </summary>
/// <remarks>
/// インスタンスは直接作成せず、[MenuItem] で登録されたメニューから <c>ShowWindow</c> を通じて開く。
/// </remarks>
public class RenameToolWindow : EditorWindow
{
    private string _prefix = "Object_";

    private int _startNumber = 0;

    private bool _isNameSortOption = false;

    // UnityメニューのTools/Rename Toolsからウィンドウを開くためのエントリーポイント。
    // 手動で呼び出さず、メニューまたは Inspector のボタン経由で使用。
    [MenuItem("Tools/Rename Tools")]
    public static void ShowWindow()
    {
        // 将来的に初期設定を記述する予定があるため、返り値を保存しておく
        var window = GetWindow<RenameToolWindow>("Rename Tool");
    }

    private void OnGUI()
    {
        // 視認性を上げるため、EditorStyles.boldLabelを指定。
        GUILayout.Label("一括リネームツール", EditorStyles.boldLabel);

        _prefix = EditorGUILayout.TextField("接頭辞", _prefix);
        _startNumber = EditorGUILayout.IntField("開始番号", _startNumber);
        _isNameSortOption = EditorGUILayout.Toggle("名前順にしてからリネーム", _isNameSortOption);

        if (GUILayout.Button("リネーム実行"))
        {
            RenameSelectedObjects();
        }
    }

    /// <summary>
    /// 選択したオブジェクトの改名を実行する。
    /// </summary>
    private void RenameSelectedObjects()
    {
        GameObject[] selectedObjects = Selection.gameObjects;

        // 選択オブジェクトがない場合は処理をスキップ。
        if (selectedObjects.Length == 0)
        {
            Debug.LogWarning("オブジェクトが選択されていません");
            return;
        }

        if (_isNameSortOption)
        {
            // 選択オブジェクトを名前順にソート。
            System.Array.Sort(selectedObjects, (a, b) => a.name.CompareTo(b.name));
        }

        // オブジェクトの変更をUndoできるように登録しておく
        Undo.RecordObjects(selectedObjects, "一括リネーム");

        // 連番を付与するため、インデックスが扱えるforを使用。
        for (int i = 0; i < selectedObjects.Length; i++)
        {
            selectedObjects[i].name = _prefix + (_startNumber + i);

            // オブジェクト変更をUnityに通知し、上書き保存の対象に追加。(安全のため明示的に実行)
            EditorUtility.SetDirty(selectedObjects[i]);
        }

        Debug.Log($"リネーム完了！{selectedObjects.Length}個のオブジェクトをリネームしました。");
    }
}
