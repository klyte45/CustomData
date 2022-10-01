using CustomData.Utils;
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

        public static string GeneralNamesPath { get; } = FOLDER_PATH + Path.DirectorySeparatorChar + GENERAL_NAMING_FOLDER;
        public static string RoadPatternPath { get; } = FOLDER_PATH + Path.DirectorySeparatorChar + ROADPREFIX_SUBFOLDER_NAME;


        private static Dictionary<string, string[]> m_loadedGeneralNames;
        public static string[] LoadedGeneralNamesIdx { get; private set; }
        private static Dictionary<string, RoadPrefixFileIndexer> m_loadedRoadPatterns = new Dictionary<string, RoadPrefixFileIndexer>();
        public static string[] LoadedRoadPatternsIdx { get; private set; }

        public void Awake()
        {
            KFileUtils.EnsureFolderCreation(GeneralNamesPath);
            KFileUtils.EnsureFolderCreation(RoadPatternPath);
            LoadGeneralNames();
            LoadRoadPatternFiles();
        }
        public static void LoadGeneralNames() => LoadedGeneralNamesIdx = LoadSimpleNamesFiles(out m_loadedGeneralNames, GeneralNamesPath).Select(x => x.Key).ToArray();
        // public static void LoadRoadNamePatterns() => LoadSimpleNamesFiles(out m_loadedRoadPatterns, GeneralNamesPath);

        private static Dictionary<string, string[]> LoadSimpleNamesFiles(out Dictionary<string, string[]> result, string path)
        {
            result = new Dictionary<string, string[]>();
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
            m_loadedRoadPatterns.Clear();
            m_loadedRoadPatterns.AddRange(Directory.GetFiles(RoadPatternPath, "*.txt").Select(x =>
               {
                   var name = x.Split(Path.DirectorySeparatorChar).Last();
                   string fileContents = File.ReadAllText(x, Encoding.UTF8);
                   fileContents.Replace(Environment.NewLine, "\n");
                   var data = RoadPrefixFileIndexer.Parse(fileContents.Split('\n').Where(y => !string.IsNullOrEmpty(y) && !y.StartsWith("#")).ToArray());
                   LogUtils.DoLog("LOADED PREFIX NAMES ({0})", name);
                   return Tuple.New(name, data);
               }).ToDictionary(x => x.First, x => x.Second));
            LoadedRoadPatternsIdx = m_loadedRoadPatterns.Keys.ToArray();
        }

        protected override void StartActions()
        {
            base.StartActions();
        }
    }
}
