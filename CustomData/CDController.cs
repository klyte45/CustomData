using CustomData.Overrides;
using CustomData.UI;
using CustomData.Utils;
using CustomData.Xml;
using Kwytto.Interfaces;
using Kwytto.Utils;
using MonoMod.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace CustomData
{

    public class CDController : BaseController<ModInstance, CDController>
    {
        public static string FOLDER_PATH => ModInstance.ModSettingsRootFolder;

        public const string GENERAL_NAMING_FOLDER = "Namings";
        public const string ROADPREFIX_SUBFOLDER_NAME = "RoadsPatterns";
        public const string SIMPLEPATTERNS_SUBFOLDER_NAME = "SimplePatterns";

        public static string GeneralNamesPath { get; } = FOLDER_PATH + Path.DirectorySeparatorChar + GENERAL_NAMING_FOLDER;
        public static string RoadPatternPath { get; } = FOLDER_PATH + Path.DirectorySeparatorChar + ROADPREFIX_SUBFOLDER_NAME;
        public static string SimplePatternPath { get; } = FOLDER_PATH + Path.DirectorySeparatorChar + SIMPLEPATTERNS_SUBFOLDER_NAME;


        internal static Dictionary<string, string[]> LoadedGeneralNames { get; } = new Dictionary<string, string[]>();
        public static string[] LoadedGeneralNamesIdx { get; private set; }
        internal static Dictionary<string, RoadPrefixFileIndexer> LoadedRoadPatterns { get; } = new Dictionary<string, RoadPrefixFileIndexer>();
        public static string[] LoadedRoadPatternsIdx { get; private set; }
        internal static Dictionary<string, string[]> LoadedSimplePatterns { get; } = new Dictionary<string, string[]>();
        public static string[] LoadedSimplePatternsIdx { get; private set; }

        public void Awake()
        {
            KFileUtils.EnsureFolderCreation(GeneralNamesPath);
            KFileUtils.EnsureFolderCreation(RoadPatternPath);
            KFileUtils.EnsureFolderCreation(SimplePatternPath);
            LoadGeneralNames();
            LoadRoadPatternFiles();
            LoadSimplePatterns();

            _ = CDAddressWindow.Instance;

            BuildingManager.instance.EventBuildingReleased += (x) => CDStorage.Instance.RemoveBuilding(x);
        }
        public static void LoadGeneralNames()
        {
            LoadedGeneralNamesIdx = LoadSimpleNamesFiles(LoadedGeneralNames, GeneralNamesPath).Select(x => x.Key).ToArray();

            DistrictManager.instance.NamesModified();
            DistrictManager.instance.ParkNamesModified();
        }

        public static void LoadSimplePatterns() => LoadedSimplePatternsIdx = LoadSimpleNamesFiles(LoadedSimplePatterns, SimplePatternPath).Select(x => x.Key).ToArray();

        private static Dictionary<string, string[]> LoadSimpleNamesFiles(Dictionary<string, string[]> result, string path)
        {
            result.Clear();
            foreach (string filename in Directory.GetFiles(path, "*.txt").Select(x => x.Split(Path.DirectorySeparatorChar).Last()))
            {
                string fileContents = File.ReadAllText(path + Path.DirectorySeparatorChar + filename, Encoding.UTF8);
                result[filename] = fileContents.Split(Environment.NewLine.ToCharArray()).Select(x => x?.Trim()).Where(x => !string.IsNullOrEmpty(x)).ToArray();
                LogUtils.DoLog($"LOADED Files at {path} ({filename}) QTT: {result[filename].Length}");
            }
            return result;
        }
        public static void LoadRoadPatternFiles()
        {
            LoadedRoadPatterns.Clear();
            LoadedRoadPatterns.AddRange(Directory.GetFiles(RoadPatternPath, "*.txt").Select(x =>
               {
                   var name = x.Split(Path.DirectorySeparatorChar).Last();
                   string fileContents = File.ReadAllText(x, Encoding.UTF8);
                   fileContents.Replace(Environment.NewLine, "\n");
                   var data = RoadPrefixFileIndexer.Parse(fileContents.Split('\n').Where(y => !string.IsNullOrEmpty(y) && !y.StartsWith("#")).ToArray());
                   LogUtils.DoLog("LOADED PREFIX NAMES ({0})", name);
                   return Tuple.New(name, data);
               }).ToDictionary(x => x.First, x => x.Second));
            LoadedRoadPatternsIdx = LoadedRoadPatterns.Keys.ToArray();
            DistrictManager.instance.NamesModified();
            DistrictManager.instance.ParkNamesModified();
        }

        protected override void StartActions()
        {
            base.StartActions();
        }
    }
}
