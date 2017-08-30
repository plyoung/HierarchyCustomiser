using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.IMGUI.Controls;


namespace HierarchyCustomiser
{
	[InitializeOnLoad]
	public class HierarchyCustomiserEdWindow : EditorWindow
	{
		public static class Styles
		{
			public static GUIStyle Head;
			public static GUIStyle[] Row = new GUIStyle[2];

			static Styles()
			{
				Head = new GUIStyle() { margin = new RectOffset(3, 3, 0, 1), padding = new RectOffset(3, 5, 3, 3), richText = false };
				Row[0] = new GUIStyle("OL EntryBackEven") { margin = new RectOffset(3, 3, 0, 1), padding = new RectOffset(3, 5, 3, 3), richText = false };
				Row[1] = new GUIStyle("OL EntryBackOdd") { margin = new RectOffset(3, 3, 0, 1), padding = new RectOffset(3, 5, 3, 3), richText = false };
			}
		}

		private static readonly GUIContent GC_LayersHead = new GUIContent("Layers");
		private static readonly GUIContent GC_TagsHead = new GUIContent("Tags");
		private static readonly GUIContent[] GC_Head = { new GUIContent("Entry"), new GUIContent("Ico"), new GUIContent("L"), new GUIContent("Offs"), new GUIContent("Text"), new GUIContent("Back") };
		private static readonly GUIContent GC_ClearOpt = new GUIContent("X", "Clear/remove setting");
		private static readonly GUIContent GC_Add = new GUIContent("Add");
		private static GUIContent GC_Temp = new GUIContent("");

		private Vector2 scroll;
		private bool[] foldout = { true, true };

		// ------------------------------------------------------------------------------------------------------------

		[MenuItem("Window/Hierarchy Customiser")]
		public static void Show_Window()
		{
			GetWindow<HierarchyCustomiserEdWindow>("HierarchyCustomiser", true);
		}

		private void OnGUI()
		{
			scroll = EditorGUILayout.BeginScrollView(scroll);
			LayersOps();
			EditorGUILayout.Space();
			TagOps();
			EditorGUILayout.Space();
			EditorGUILayout.EndScrollView();
		}

		private void LayersOps()
		{
			if (!(foldout[0] = EditorGUILayout.Foldout(foldout[0], GC_LayersHead))) return;

			DrawHead(false);
			EditorGUI.BeginChangeCheck();
			for (int i = 0; i < 32; i++)
			{
				EditorGUILayout.BeginHorizontal(Styles.Row[i % 2 == 0 ? 0 : 1]);
				DrawOpt(TempGUIContent("{0:00}:{1}", i, LayerMask.LayerToName(i)), HierarchyCustomiserEd.Settings.layerOps[i]);
				EditorGUILayout.EndHorizontal();
			}

			if (EditorGUI.EndChangeCheck())
			{
				EditorUtility.SetDirty(HierarchyCustomiserEd.Settings);
				EditorApplication.RepaintHierarchyWindow();
			}
		}

		private void TagOps()
		{
			if (!(foldout[1] = EditorGUILayout.Foldout(foldout[1], GC_TagsHead))) return;

			bool dirty = false;
			int rem = -1;

			EditorGUILayout.Space();
			if (DrawHead(true))
			{
				dirty = true;
				HierarchyCustomiserEd.Settings.tagOps.Add(new HierarchyCustomiserOption());
			}

			;
			EditorGUI.BeginChangeCheck();
			for (int i = 0; i < HierarchyCustomiserEd.Settings.tagOps.Count; i++)
			{
				EditorGUILayout.BeginHorizontal(Styles.Row[i % 2 == 0 ? 0 : 1]);
				if (DrawOpt(null, HierarchyCustomiserEd.Settings.tagOps[i])) rem = i;
				EditorGUILayout.EndHorizontal();
			}

			if (rem >= 0)
			{
				HierarchyCustomiserEd.Settings.tagOps.RemoveAt(rem);
				dirty = true;
			}

			if (EditorGUI.EndChangeCheck() || dirty)
			{
				EditorUtility.SetDirty(HierarchyCustomiserEd.Settings);
				EditorApplication.RepaintHierarchyWindow();
			}
		}

		private static bool DrawHead(bool forTags)
		{
			bool res = false;
			EditorGUILayout.BeginHorizontal(Styles.Head);
			GUILayout.Label(GC_Head[0], EditorStyles.boldLabel);
			if (forTags) res = GUILayout.Button(GC_Add, EditorStyles.miniButton, GUILayout.Width(32));
			GUILayout.FlexibleSpace();
			GUILayout.Label(GC_Head[1], EditorStyles.boldLabel, GUILayout.Width(30));
			GUILayout.Label(GC_Head[2], EditorStyles.boldLabel, GUILayout.Width(20));
			GUILayout.Label(GC_Head[3], EditorStyles.boldLabel, GUILayout.Width(30));
			GUILayout.Space(5);
			GUILayout.Label(GC_Head[4], EditorStyles.boldLabel, GUILayout.Width(40));
			GUILayout.Label(GC_Head[5], EditorStyles.boldLabel, GUILayout.Width(40));
			EditorGUILayout.EndHorizontal();
			return res;
		}

		private static bool DrawOpt(GUIContent label, HierarchyCustomiserOption opt)
		{
			bool res = false;
			if (res = GUILayout.Button(GC_ClearOpt, EditorStyles.miniButton, GUILayout.Width(18)))
			{
				opt.icon = null;
				opt.iconLeft = false;
				opt.iconOffs = 0;
			}

			if (label == null)
			{   // is for tag when label = null
				opt.tag = EditorGUILayout.TagField(opt.tag, GUILayout.Width(100));
				GUILayout.FlexibleSpace();
			}
			else
			{
				GUILayout.Label(label, GUILayout.ExpandWidth(true));
			}

			opt.icon = (Texture2D)EditorGUILayout.ObjectField(opt.icon, typeof(Texture2D), false, GUILayout.Width(30));
			opt.iconLeft = EditorGUILayout.Toggle(opt.iconLeft, GUILayout.Width(20));
			opt.iconOffs = EditorGUILayout.IntField(opt.iconOffs, GUILayout.Width(30));
			GUILayout.Space(5);
			opt.textColor = EditorGUILayout.ColorField(opt.textColor, GUILayout.Width(40));
			opt.backColor = EditorGUILayout.ColorField(opt.backColor, GUILayout.Width(40));
			return res;
		}

		private static GUIContent TempGUIContent(string label, params object[] args)
		{
			GC_Temp.text = string.Format(label, args);
			return GC_Temp;
		}

		// ------------------------------------------------------------------------------------------------------------
	}
}