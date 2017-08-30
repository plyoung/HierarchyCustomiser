using System.Collections.Generic;
using UnityEngine;


namespace HierarchyCustomiser
{
	public class HierarchyCustomiserAsset : ScriptableObject, ISerializationCallbackReceiver
	{
		[HideInInspector] public HierarchyCustomiserOption[] layerOps;
		[HideInInspector] public List<HierarchyCustomiserOption> tagOps;
		public Dictionary<string, HierarchyCustomiserOption> TagCache = new Dictionary<string, HierarchyCustomiserOption>();

		public void InitNew()
		{
			tagOps = new List<HierarchyCustomiserOption>();
			layerOps = new HierarchyCustomiserOption[32];
			for (int i = 0; i < layerOps.Length; i++) layerOps[i] = new HierarchyCustomiserOption();
		}

		public void RefreshTagCache()
		{
			TagCache.Clear();
			foreach (HierarchyCustomiserOption op in tagOps)
			{
				if (!string.IsNullOrEmpty(op.tag) && !TagCache.ContainsKey(op.tag)) TagCache.Add(op.tag, op);
			}
		}

		void ISerializationCallbackReceiver.OnAfterDeserialize()
		{
			RefreshTagCache();
		}

		void ISerializationCallbackReceiver.OnBeforeSerialize()
		{
			RefreshTagCache();
		}

	}

	// ================================================================================================================

	[System.Serializable]
	public class HierarchyCustomiserOption
	{
		public string tag;
		public Texture2D icon;
		public bool iconLeft;
		public int iconOffs;
		public Color textColor;
		public Color backColor;
	}
}