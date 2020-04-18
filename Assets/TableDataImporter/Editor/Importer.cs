using System.IO;
using UnityEditor;

namespace TableDataImporter.Editor {
    public class Importer {
        private const string MENU_ITEM = "Assets/Import TableData";

        [MenuItem(MENU_ITEM, true)]
        public static bool IsTableData() {
            var path = AssetDatabase.GetAssetPath(Selection.activeInstanceID);
            return TableDataFactory.CanParse(path);
        }

        [MenuItem(MENU_ITEM)]
        public static void Import() {
            var inputPath = AssetDatabase.GetAssetPath(Selection.activeInstanceID);
            var parser = TableDataFactory.CreateParser(inputPath);
            var ast = parser.Parse();
            var builder = new TableDataBuilder(ast);
            var data = builder.Build();
            var outputPath = Path.ChangeExtension(inputPath, ".asset");
            var asset = AssetDatabase.LoadAssetAtPath<TableData>(outputPath);
            if (asset == null) {
                AssetDatabase.CreateAsset(data, outputPath);
            }
            else {
                EditorUtility.CopySerialized(data, asset);
                AssetDatabase.SaveAssets();
            }
        }
    }
}
