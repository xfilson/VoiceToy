using UnityEditor;
using UnityEngine;
using System.IO;
using System.Linq;

public class ExportSpriteContextMenu
{
    [MenuItem("Assets/导出切割后的子图", false, 30)]
    private static void ExportSelectedSprites()
    {
        // 获取选中的图集文件
        var selectedTextures = Selection.objects
            .Where(obj => obj is Texture2D)
            .Cast<Texture2D>()
            .ToList();

        if (selectedTextures.Count == 0)
        {
            EditorUtility.DisplayDialog("错误", "请选择已切割的图集文件", "确定");
            return;
        }

        // 选择导出路径
        string savePath = EditorUtility.SaveFolderPanel("选择导出目录", "Assets/", "");

        if (string.IsNullOrEmpty(savePath)) return;

        foreach (var texture in selectedTextures)
        {
            string assetPath = AssetDatabase.GetAssetPath(texture);
            var sprites = AssetDatabase.LoadAllAssetsAtPath(assetPath)
                .OfType<Sprite>()
                .ToList();

            if (sprites.Count == 0)
            {
                Debug.LogWarning($"图集 {texture.name} 未切割，跳过导出");
                continue;
            }

            // 创建子目录
            string targetDir = Path.Combine(savePath, texture.name);
            Directory.CreateDirectory(targetDir);

            // 导出所有子图
            foreach (var sprite in sprites)
            {
                var rect = sprite.rect;
                var pixels = sprite.texture.GetPixels((int)rect.x, (int)rect.y, (int)rect.width, (int)rect.height);
                
                Texture2D newTex = new Texture2D((int)rect.width, (int)rect.height);
                newTex.SetPixels(pixels);
                newTex.Apply();
                
                File.WriteAllBytes(Path.Combine(targetDir, $"{sprite.name}.png"), newTex.EncodeToPNG());
            }
        }

        AssetDatabase.Refresh();
        EditorUtility.DisplayDialog("完成", $"已导出到 {savePath}", "确定");
    }

    [MenuItem("Assets/导出切割后的子图", true)]
    private static bool ValidateExportSelectedSprites()
    {
        // 仅在选中Texture2D时显示菜单项
        return Selection.activeObject is Texture2D;
    }
}