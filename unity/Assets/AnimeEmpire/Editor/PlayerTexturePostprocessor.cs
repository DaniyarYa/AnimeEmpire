using UnityEditor;

namespace AnimeEmpire.Editor
{
    public class PlayerTexturePostprocessor : AssetPostprocessor
    {
        const string PlayerFolder = "Assets/AnimeEmpire/Art/Characters/PlayerAvatar";

        void OnPreprocessTexture()
        {
            if (assetPath == null || !assetPath.StartsWith(PlayerFolder)) return;
            var importer = (TextureImporter)assetImporter;
            importer.textureType = TextureImporterType.Default;
            importer.sRGBTexture = true;
            importer.alphaSource = TextureImporterAlphaSource.FromInput;
            importer.maxTextureSize = 2048;
            importer.mipmapEnabled = true;
            importer.streamingMipmaps = true;

            var android = importer.GetPlatformTextureSettings("Android");
            android.overridden = true;
            android.maxTextureSize = 2048;
            android.format = TextureImporterFormat.ASTC_6x6;
            importer.SetPlatformTextureSettings(android);

            var iOS = importer.GetPlatformTextureSettings("iPhone");
            iOS.overridden = true;
            iOS.maxTextureSize = 2048;
            iOS.format = TextureImporterFormat.ASTC_6x6;
            importer.SetPlatformTextureSettings(iOS);
        }
    }
}
