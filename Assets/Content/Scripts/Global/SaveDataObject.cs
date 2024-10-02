using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Content.Scripts.Boot;
using Content.Scripts.ItemsSystem;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

namespace Content.Scripts.Global
{
    [CreateAssetMenu(menuName = "Scriptable/SaveData", fileName = "SaveDataObject", order = 0)]
    public partial class SaveDataObject : ScriptableObjectInstaller
    {
        [SerializeField] private CharactersData charactersData = new CharactersData();
        [SerializeField] private RaftsData raftsData = new RaftsData();
        [SerializeField] private MapData mapData = new MapData();
        [SerializeField] private GlobalData globalData = new GlobalData();
        [SerializeField] private TutorialsData tutorialsData = new TutorialsData();
        [SerializeField] private DungeonsData dungeonsData = new DungeonsData();
        [SerializeField] private PlayerInventoryData playerInventory = new PlayerInventoryData();
        [SerializeField] private CrossGameData crossGameData = new CrossGameData();

        public CharactersData Characters => charactersData;

        public RaftsData Rafts => raftsData;

        public MapData Map => mapData;

        public GlobalData Global => globalData;

        public TutorialsData Tutorials => tutorialsData;

        public DungeonsData Dungeons => dungeonsData;

        public CrossGameData CrossGame => crossGameData;

        public PlayerInventoryData PlayerInventory => playerInventory;


        public override void InstallBindings()
        {
            Container.Bind<SaveDataObject>().FromInstance(this).AsSingle();
            LoadFile();
        }
        
        public MapData.IslandData GetTargetIsland()
        {
            return mapData.GetIslandData(globalData.IslandSeed);
        }

        protected string GetPathFolder()
        {
#if UNITY_EDITOR || PLATFORM_STANDALONE_WIN
            return Directory.GetParent(Application.dataPath)?.FullName;
#endif
#if UNITY_ANDROID
            return Application.persistentDataPath;
#else
            return null;
#endif
        }

        protected virtual string GetFilePath() => GetPathFolder() + @"\data.dat";
        
        [Button]
        public virtual void SaveFile()
        {
            string json = "";
            
#if UNITY_EDITOR || UNITY_ANDROID || PLATFORM_STANDALONE_WIN
            if (!string.IsNullOrEmpty(GetPathFolder()))
            {
                json = JsonUtility.ToJson(this);
                File.WriteAllText(GetFilePath(), json, Encoding.Unicode);
                #if UNITY_EDITOR
                    File.WriteAllText(GetPathFolder() + @"\Backups\backup" + DateTime.Now.ToString("yyyy-M-d dddd-HH-mm-ss") + ".dat", json, Encoding.Unicode);
                #endif
            }
            else
            {
                Debug.LogError("Path error!");
            }
#endif
#if UNITY_WEBGL
            json = JsonUtility.ToJson(this);
            PlayerPrefs.SetString(PrefsSaveKey, json);
#endif
        }

        [Button]
        public virtual void LoadFile()
        {
#if UNITY_EDITOR || UNITY_ANDROID || PLATFORM_STANDALONE_WIN
            var file = GetFilePath();
            if (File.Exists(file))
            {
                try
                {
                    JsonUtility.FromJsonOverwrite(File.ReadAllText(file), this);
                    return;
                }
                catch (Exception e)
                {
                    Debug.LogError("Save Parse Error");
                }
            }
#endif
#if UNITY_WEBGL
            var json = PlayerPrefs.GetString(PrefsSaveKey);
            if (!string.IsNullOrEmpty(json))
            {
                JsonUtility.FromJsonOverwrite(json, this);
                return;
            }
#endif
            
            JsonUtility.FromJsonOverwrite(JsonUtility.ToJson(CreateInstance<SaveDataObject>()), this);
            
            SaveFile();

        }

        [Button]
        public void DeleteFile(bool saveCrossGame)
        {
            string crossGameJson = string.Empty;
            if (saveCrossGame)
            {
                crossGameJson = JsonUtility.ToJson(crossGameData);
            }
            
#if UNITY_EDITOR || UNITY_ANDROID || PLATFORM_STANDALONE_WIN
            var file = GetFilePath();
            if (File.Exists(file))
            {
                File.Delete(file);
            }
#endif
#if UNITY_WEBGL
            PlayerPrefs.DeleteAll();
#endif
            LoadFile();

            if (saveCrossGame)
            {
                this.crossGameData = JsonUtility.FromJson<CrossGameData>(crossGameJson);
            }

            SaveFile();
        }

        [Button]
        public void OpenFile()
        {
            Application.OpenURL(Directory.GetParent(GetFilePath())?.FullName);
        }

        public void SetRaftsData(RaftsData newRaftsData)
        {
            raftsData = newRaftsData;
        }
        
        [Button]
        public void DebugButton(){
            foreach (var isl in Map.Islands)
            {
                var tmp = Map.Islands.FindAll(x => x.IslandSeed == isl.IslandSeed);

                if (tmp.Count >= 2)
                {
                    Debug.LogError(tmp.Count);

                    foreach (var t in tmp)
                    {
                        Debug.LogError(t.IslandSeed);
                    }
                    return;
                }
            }
            Debug.LogError("empty");
        }

        public async Task SaveToCloud(CloudService cloudService)
        {
            var file = GetFilePath();
            if (File.Exists(file))
            {
                await cloudService.SaveJson(File.ReadAllText(file));
            }
        }

        public void ReplaceJson(string json)
        {
            if (!string.IsNullOrEmpty(json))
            {
                var file = GetFilePath();
                if (File.Exists(file))
                {
                    File.Delete(file);
                }
                File.WriteAllText(file, json);
                LoadFile();
            }
        }
    }
    
}