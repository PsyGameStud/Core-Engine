using UnityEditor;
using UnityEngine;

namespace Core
{
    public class ResourceLoader
    {
        private static string _path = "Assets/Resources/Configurations";
        private static string _pathForSearch = "Configurations";

        public static T GetConfiguration<T>() where T : Config
        {
            var nameDef = $"{_pathForSearch}/{typeof(T).Name}";
            var asset = Resources.Load<T>(nameDef);

            if (asset == null)
            {
                var name = $"{_path}/{typeof(T).Name}";
                asset = ScriptableObject.CreateInstance<T>();

#if UNITY_EDITOR
                AssetDatabase.CreateAsset(asset, $"{name}.asset");
                AssetDatabase.SaveAssets();
#endif
            }

            return asset;
        }
    }
}