using System.Collections;
using System.Collections.Generic;
using Content.Scripts.CraftsSystem;
using Content.Scripts.ItemsSystem;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public static class CustomIconsDrawer
{
    private static Dictionary<string, Texture2D> spriteAtlases = new Dictionary<string, Texture2D>();
    private static Texture2D backgroundTexture;
    private static Queue<PendingIconTask> iconProcessingQueue = new Queue<PendingIconTask>();

    static CustomIconsDrawer()
    {
        EditorApplication.projectWindowItemOnGUI += OnProjectWindowItemGUI;
        EditorApplication.update += ProcessIconQueue;
    }

    private static void OnProjectWindowItemGUI(string guid, Rect selectionRect)
    {
        string assetPath = AssetDatabase.GUIDToAssetPath(guid);

        CreateGrayTexture();

        bool isCraft = false;
        ItemObject item = AssetDatabase.LoadAssetAtPath<ItemObject>(assetPath);
        Sprite icon = null;
        
        
        if (item == null)
        {
            CraftObject craft = AssetDatabase.LoadAssetAtPath<CraftObject>(assetPath);

            if (craft != null)
            {
                item = craft.FinalItem.ResourceName;
                icon = craft.Icon;
                isCraft = true;
            }
            else
            {
                return;
            }
        }
        else
        {
            icon = item.ItemIcon;
        }

        if (item != null && icon != null)
        {
            if (item.EditorSprite == null && !IsTaskQueued(item))
            {
                iconProcessingQueue.Enqueue(new PendingIconTask
                {
                    Item = item,
                    Icon = icon
                });
            }

            if (item.EditorSprite != null)
            {
                DrawGUI(selectionRect, item, isCraft);
            }
        }
    }

    private static void DrawGUI(Rect selectionRect, ItemObject item, bool isCraft)
    {
        GUI.DrawTexture(
            new Rect(selectionRect.x, selectionRect.y, selectionRect.width, selectionRect.height / 1.3f),
            backgroundTexture,
            ScaleMode.ScaleToFit
        );

        GUI.DrawTexture(
            new Rect(selectionRect.x, selectionRect.y, selectionRect.width, selectionRect.height / 1.3f),
            item.EditorSprite,
            ScaleMode.ScaleToFit
        );

        if (isCraft)
        {
            var xOffect = 0f;
            var yOffcet = 0;
            if (selectionRect.height < 48)
            {
                xOffect += (selectionRect.width / 2f) + 50;
                yOffcet += 50;
            }

            GUI.Label(
                new Rect(selectionRect.x + 2f + xOffect, selectionRect.y + 2f, selectionRect.width,
                    (selectionRect.height / 2f) + yOffcet), "Craft", new GUIStyle(GUI.skin.label)
                {
                    alignment = TextAnchor.UpperLeft,
                    fontStyle = FontStyle.Bold,
                    normal = { textColor = Color.black }
                }
            );
            GUI.Label(
                new Rect(selectionRect.x + xOffect, selectionRect.y, selectionRect.width,
                    (selectionRect.height / 2f) + yOffcet), "Craft", new GUIStyle(GUI.skin.label)
                {
                    alignment = TextAnchor.UpperLeft,
                    fontStyle = FontStyle.Bold
                }
            );
        }
    }

    private static void CreateGrayTexture()
    {
        if (backgroundTexture == null)
        {
            backgroundTexture = new Texture2D(1, 1, TextureFormat.RGBA32, false, false);
            backgroundTexture.SetPixel(0, 0, new Color32(51, 51, 51, 255));
            backgroundTexture.Apply();
        }
    }

    private static bool IsTaskQueued(ItemObject item)
    {
        foreach (var task in iconProcessingQueue)
        {
            if (task.Item == item)
                return true;
        }
        return false;
    }

    private static void ProcessIconQueue()
    {
        if (iconProcessingQueue.Count > 0)
        {
            var task = iconProcessingQueue.Dequeue();

            // Выполняем обработку задачи
            if (task.Icon != null && task.Item != null)
            {
                if (!spriteAtlases.ContainsKey(task.Icon.texture.name)) 
                {
                    var clonedTexture = CloneTextureWithSettings(task.Icon.texture);
                    spriteAtlases.Add(task.Icon.texture.name, clonedTexture);
                }

                task.Item.EditorSprite = ExtractTextureFromSprite(task.Icon);
            }

            // Принудительно обновляем окно Project
            EditorApplication.RepaintProjectWindow();
        }
    }

    public static Texture2D CloneTextureWithSettings(Texture2D originalTexture)
    {
        if (!AssetDatabase.IsValidFolder("Assets/Editor"))
        {
            AssetDatabase.CreateFolder("Assets", "Editor");
        }

        var path = $"Assets/Editor/{originalTexture.name}.asset";
        AssetDatabase.DeleteAsset(path);

        var instance = Object.Instantiate(originalTexture);
        AssetDatabase.CreateAsset(instance, path);

        var importer = AssetImporter.GetAtPath(path) as TextureImporter;
        if (importer)
        {
            importer.isReadable = true;
            importer.crunchedCompression = false;
            EditorUtility.SetDirty(importer);
        }

        return instance;
    }

    public static Texture2D ExtractTextureFromSprite(Sprite sprite)
    {
        if (sprite == null)
        {
            Debug.LogError("Sprite is null!");
            return null;
        }

        Texture2D atlasTexture = spriteAtlases[sprite.texture.name];
        Rect spriteRect = sprite.textureRect;

        int width = Mathf.RoundToInt(spriteRect.width);
        int height = Mathf.RoundToInt(spriteRect.height);
        Texture2D extractedTexture = new Texture2D(width, height, TextureFormat.RGBA32, false);

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                int px = Mathf.Clamp((int)spriteRect.x + x, 0, atlasTexture.width - 1);
                int py = Mathf.Clamp((int)spriteRect.y + y, 0, atlasTexture.height - 1);

                Color pixelColor = atlasTexture.GetPixel(px, py);
                extractedTexture.SetPixel(x, y, pixelColor);
            }
        }

        extractedTexture.Apply();

        return extractedTexture;
    }

    private class PendingIconTask
    {
        public ItemObject Item;
        public Sprite Icon;
    }
}
