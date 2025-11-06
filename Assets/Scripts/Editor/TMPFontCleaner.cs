using System;
using System.Collections;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using TMPro;

public class TMPFontCleaner : EditorWindow {
    private TMP_FontAsset _fontAsset;
    private Vector2 _scroll;

    [MenuItem("Window/TMP Font Cleaner")]
    public static void ShowWindow() {
        var w = GetWindow<TMPFontCleaner>("TMP Font Cleaner");
        w.minSize = new Vector2(380, 120);
    }

    private void OnGUI() {
        GUILayout.Label("Удалить Ligatures и Adjustment Tables из TMP Font Asset", EditorStyles.boldLabel);

        EditorGUILayout.BeginHorizontal();
        _fontAsset = (TMP_FontAsset)EditorGUILayout.ObjectField("TMP Font Asset", _fontAsset, typeof(TMP_FontAsset), false);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();

        using (new EditorGUI.DisabledScope(_fontAsset == null)) {
            if (GUILayout.Button("Remove Ligatures & Adjustments", GUILayout.Height(34))) {
                if (_fontAsset == null) return;
                if (!EditorUtility.DisplayDialog("Confirm",
                        $"Remove ligatures and adjustment records from '{_fontAsset.name}'? This will modify the asset on disk.", "Yes, remove",
                        "Cancel")) {
                    return;
                }

                try {
                    Undo.RegisterCompleteObjectUndo(_fontAsset, "TMP: Remove Ligatures & Adjustments");
                    int changed = RemoveLigaturesAndAdjustments(_fontAsset);
                    EditorUtility.SetDirty(_fontAsset);
                    AssetDatabase.SaveAssets();
                    EditorUtility.DisplayDialog("Done", $"Cleared {changed} matching list(s) on font '{_fontAsset.name}'.", "OK");
                } catch (Exception ex) {
                    Debug.LogException(ex);
                    EditorUtility.DisplayDialog("Error", "An error occurred. See console for details.", "OK");
                }
            }
        }

        EditorGUILayout.Space();
        GUILayout.Label("How to use:");
        EditorGUILayout.LabelField("1. Place this script into an 'Editor' folder.");
        EditorGUILayout.LabelField("2. Open: Window -> TMP Font Cleaner.");
        EditorGUILayout.LabelField("3. Select a TMP_FontAsset and press the button.");
    }

    private int RemoveLigaturesAndAdjustments(TMP_FontAsset fontAsset) {
        int clearedCount = 0;

        // Look for fields on the font asset that are likely candidates to contain ligature/adjustment data
        BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
        Type t = fontAsset.GetType();

        FieldInfo[] fields = t.GetFields(flags);
        foreach (var fi in fields) {
            string name = fi.Name.ToLowerInvariant();
            if (IsTargetFieldName(name)) {
                object value = fi.GetValue(fontAsset);
                if (TryClearListField(fi, fontAsset, ref value)) {
                    clearedCount++;
                    continue;
                }
            }
        }

        // Also attempt to inspect the public 'fontFeatureTable' property if present
        PropertyInfo[] props = t.GetProperties(flags);
        foreach (var pi in props) {
            string name = pi.Name.ToLowerInvariant();
            if (IsTargetFieldName(name)) {
                object obj = null;
                try {
                    obj = pi.GetValue(fontAsset, null);
                } catch {
                    obj = null;
                }

                if (obj != null) {
                    // Try to clear fields inside that object
                    if (ClearNestedLists(obj)) clearedCount++;
                }
            }
        }

        // As a fallback, scan nested objects for fields containing target names
        foreach (var fi in fields) {
            try {
                object value = fi.GetValue(fontAsset);
                if (value != null) {
                    if (ClearNestedLists(value)) clearedCount++;
                }
            } catch { }
        }

        return clearedCount;
    }

    private bool IsTargetFieldName(string lowerName) {
        // target typical substrings that indicate ligatures/adjustments/feature tables
        return lowerName.Contains("ligatur") || lowerName.Contains("adjust") || lowerName.Contains("feature") || lowerName.Contains("pair") ||
               lowerName.Contains("glyphpair");
    }

    private bool TryClearListField(FieldInfo fi, object owner, ref object fieldValue) {
        if (fieldValue == null) {
            // create an empty instance of the field type if possible
            try {
                var empty = Activator.CreateInstance(fi.FieldType);
                fi.SetValue(owner, empty);
                return true;
            } catch {
                return false;
            }
        }

        // If it implements System.Collections.IList, try to clear it
        if (fieldValue is IList list) {
            try {
                list.Clear();
                return true;
            } catch { }
        }

        // If it's an array, replace with empty array of same element type
        if (fieldValue != null && fieldValue.GetType().IsArray) {
            Type elem = fieldValue.GetType().GetElementType();
            var emptyArr = Array.CreateInstance(elem, 0);
            fi.SetValue(owner, emptyArr);
            return true;
        }

        return false;
    }

    private bool ClearNestedLists(object obj) {
        bool anyCleared = false;
        if (obj == null) return false;

        Type t = obj.GetType();
        BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
        FieldInfo[] fields = t.GetFields(flags);
        foreach (var fi in fields) {
            string name = fi.Name.ToLowerInvariant();
            if (!IsTargetFieldName(name)) continue;

            object val = fi.GetValue(obj);
            if (val == null) {
                try {
                    var empty = Activator.CreateInstance(fi.FieldType);
                    fi.SetValue(obj, empty);
                    anyCleared = true;
                } catch { }
            } else if (val is IList list) {
                try {
                    list.Clear();
                    anyCleared = true;
                } catch { }
            } else if (val.GetType().IsArray) {
                Type elem = val.GetType().GetElementType();
                var emptyArr = Array.CreateInstance(elem, 0);
                fi.SetValue(obj, emptyArr);
                anyCleared = true;
            }
        }

        // Also check properties with target names
        PropertyInfo[] props = t.GetProperties(flags);
        foreach (var pi in props) {
            string name = pi.Name.ToLowerInvariant();
            if (!IsTargetFieldName(name)) continue;

            object val = null;
            try {
                val = pi.GetValue(obj, null);
            } catch {
                val = null;
            }

            if (val == null) {
                // skip setting properties (some are read-only)
                continue;
            }

            if (val is IList list) {
                try {
                    list.Clear();
                    anyCleared = true;
                } catch { }
            } else if (val.GetType().IsArray) {
                Type elem = val.GetType().GetElementType();
                var emptyArr = Array.CreateInstance(elem, 0);
                // can't set property in general; try to find a backing field
                FieldInfo backing = t.GetField("_" + pi.Name, flags) ?? t.GetField("m_" + pi.Name, flags);
                if (backing != null) {
                    backing.SetValue(obj, emptyArr);
                    anyCleared = true;
                }
            }
        }

        return anyCleared;
    }
}