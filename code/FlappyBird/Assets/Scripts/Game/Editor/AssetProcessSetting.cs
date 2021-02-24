using System;
using UnityEditor;
using UnityEngine;

public class AssetProcessSetting : AssetPostprocessor
{
    private void OnPreprocessTexture()
    {
        TextureImporter textureImporter = (TextureImporter)assetImporter;
        if (assetPath.Contains("Assets/Resources/sprites"))
        {
            textureImporter.textureType = TextureImporterType.Sprite;
            textureImporter.textureShape = TextureImporterShape.Texture2D;
            textureImporter.mipmapEnabled = false;
            textureImporter.isReadable = false;
        }
    }
}
