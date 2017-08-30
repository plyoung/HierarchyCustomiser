using System.Linq;
using UnityEngine;
using UnityEditor;
using UnityEditor.IMGUI.Controls;


namespace HierarchyCustomiser
{
	[InitializeOnLoad]
	public class HierarchyCustomiserEd
	{
		public static class Styles
		{
			public static GUIStyle Label1;
			public static GUIStyle Label2;

			static Styles()
			{
				Label1 = new GUIStyle(TreeView.DefaultStyles.foldoutLabel) { richText = false };
				Label2 = new GUIStyle(Label1) { normal = { background = EditorGUIUtility.whiteTexture } };
			}
		}

		private static GUIContent GC_Temp = new GUIContent();

		private static HierarchyCustomiserAsset _settings = null;
		public static HierarchyCustomiserAsset Settings { get { return _settings ?? (_settings = LoadSettings()); } }

		// ------------------------------------------------------------------------------------------------------------

		static HierarchyCustomiserEd()
		{
			EditorApplication.hierarchyWindowItemOnGUI += HierarchyOnGUI;
		}

		private static void HierarchyOnGUI(int instanceID, Rect rect)
		{
			if (Event.current.type != EventType.Repaint) return;
			GameObject go = EditorUtility.InstanceIDToObject(instanceID) as GameObject;
			if (go == null) return;

			// get options
			HierarchyCustomiserOption opt1 = Settings.layerOps[go.layer];
			HierarchyCustomiserOption opt2 = null; Settings.TagCache.TryGetValue(go.tag, out opt2);

			// draw text
			if (!Selection.instanceIDs.Contains(instanceID))
			{
				Color textCol = (opt2 != null && opt2.textColor.a > 0f ? opt2.textColor : opt1.textColor);
				Color backColor = (opt2 != null && opt2.backColor.a > 0f ? opt2.backColor : opt1.backColor);

				if (backColor.a > 0f)
				{
					GUI.backgroundColor = backColor;
					Styles.Label2.normal.textColor = (textCol.a > 0f ? textCol : TreeView.DefaultStyles.foldoutLabel.normal.textColor);
					Styles.Label2.padding.left = (int)rect.x;
					Rect r = rect; r.width += r.width; r.x = 0;
					Styles.Label2.Draw(r, TempGUIContent(go.name), false, false, false, false);
					GUI.backgroundColor = Color.white;
				}

				else if (textCol.a > 0f)
				{
					Styles.Label1.normal.textColor = textCol;
					Styles.Label1.Draw(rect, TempGUIContent(go.name), false, false, false, false);
				}
			}

			// draw icons
			if (opt1.icon != null)
			{
				Rect r = rect;
				r.width = r.height;
				r.x = (opt1.iconLeft ? 1f + opt1.iconOffs : rect.xMax - (r.width + 3f + opt1.iconOffs));
				GUI.DrawTexture(r, opt1.icon);
			}

			if (opt2 != null && opt2.icon != null)
			{
				Rect r = rect;
				r.width = r.height;
				r.x = (opt2.iconLeft ? 1f + opt2.iconOffs : rect.xMax - (r.width + 3f + opt2.iconOffs));
				GUI.DrawTexture(r, opt2.icon);
			}
		}

		private static HierarchyCustomiserAsset LoadSettings()
		{
			string fn = GetPackageFolder() + "HierarchyCustomiserEdSettings.asset";
			HierarchyCustomiserAsset asset = AssetDatabase.LoadAssetAtPath<HierarchyCustomiserAsset>(fn);
			if (asset == null)
			{
				asset = ScriptableObject.CreateInstance<HierarchyCustomiserAsset>();
				asset.InitNew();
				AssetDatabase.CreateAsset(asset, fn);
				AssetDatabase.SaveAssets();
			}
			return asset;
		}

		private static GUIContent TempGUIContent(string label)
		{
			GC_Temp.text = label;
			return GC_Temp;
		}

		private static string GetPackageFolder()
		{
			string[] res = System.IO.Directory.GetFiles(Application.dataPath, "HierarchyCustomiserEd.cs", System.IO.SearchOption.AllDirectories);
			if (res.Length == 0) return null;
			return "Assets" + res[0].Replace(Application.dataPath, "").Replace("HierarchyCustomiserEd.cs", "").Replace("\\", "/");
		}

		// ------------------------------------------------------------------------------------------------------------
	}
}