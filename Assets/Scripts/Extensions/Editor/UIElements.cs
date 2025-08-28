using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Extensions.Editor
{
	public static class UIElements
	{
		public static string PropertyToField(string propertyName) => $"<{propertyName}>k__BackingField";

		public static VisualElement GetVerticalProperty(SerializedProperty property)
		{
			VisualElement ret = new();
			ret.Add(new Label(property.displayName));
			ret.Add(new PropertyField(property, string.Empty));
			return ret;
		}

		public static IEnumerable<T> LoadAllAssets<T>(string filter = null) where T : Object
		{
			filter ??= string.Empty;
			return AssetDatabase.FindAssets($"t:{typeof(T).Name} " + filter).Select(guid => AssetDatabase.LoadAssetAtPath<T>(AssetDatabase.GUIDToAssetPath(guid)));
		}

		public static Texture2D GetAssetPreviewBlocking(Object asset)
		{
			if (!asset)
				return null;
			Texture2D icon = AssetPreview.GetAssetPreview(asset);
			if (icon)
				return icon;
			while (AssetPreview.IsLoadingAssetPreview(asset.GetInstanceID()))
				icon = AssetPreview.GetAssetPreview(asset);
			return icon;
		}
	}
}
