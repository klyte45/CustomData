using ColossalFramework.Globalization;
using ColossalFramework.Math;
using CustomData.Xml;
using Kwytto.Utils;
using System.Reflection;

namespace CustomData.Overrides
{
    public class CitizenAIOVerrides : Redirector, IRedirectable
    {
        #region Mod
        public static bool GenerateCitizenName(uint citizenID, byte family, ref string __result)
        {
            Randomizer randomizer = new Randomizer(citizenID);
            Randomizer randomizer2 = new Randomizer(family);
            bool isMale = Citizen.GetGender(citizenID) == Citizen.Gender.Male;

            string name, surname;
            var configObj = CDStorage.Instance.GetCitizenData();
            if (isMale)
            {
                string filenameFirst = configObj.MalesFile ?? "";
                if (CDController.LoadedGeneralNames.TryGetValue(filenameFirst, out var firstNameArray))
                {
                    int arrLen = firstNameArray.Length;
                    name = firstNameArray[randomizer.Int32((uint)arrLen)];

                }
                else
                {
                    name = Locale.Get("NAME_MALE_FIRST", randomizer.Int32(Locale.Count("NAME_MALE_FIRST")));
                }

            }
            else
            {
                string filenameFirst = configObj.FemalesFile ?? "";
                if (CDController.LoadedGeneralNames.TryGetValue(filenameFirst, out var firstNameArray))
                {
                    int arrLen = firstNameArray.Length;
                    name = firstNameArray[randomizer.Int32((uint)arrLen)];

                }
                else
                {
                    name = Locale.Get("NAME_FEMALE_FIRST", randomizer.Int32(Locale.Count("NAME_FEMALE_FIRST")));
                }

            }
            string filenameLast = configObj.SurnamesFile ?? "";

            if (CDController.LoadedGeneralNames.TryGetValue(filenameLast, out var surnameArray))
            {
                int arrLen = surnameArray.Length;
                surname = surnameArray[randomizer2.Int32((uint)arrLen)];
                __result = configObj.SurnameFirst ? $"{surname} {name}" : $"{name} {surname}";
            }
            else
            {
                if (isMale)
                {
                    surname = Locale.Get("NAME_MALE_LAST", randomizer2.Int32(Locale.Count("NAME_MALE_LAST")));
                }
                else
                {
                    surname = Locale.Get("NAME_FEMALE_LAST", randomizer2.Int32(Locale.Count("NAME_FEMALE_LAST")));
                }
                __result = StringUtils.SafeFormat(surname, name);
            }

            return false;
        }

        #endregion

        #region Hooking
        public void Awake()
        {
            LogUtils.DoLog("Loading CitizenAI Overrides");
            #region CitizenAI Hooks
            MethodInfo preRename = typeof(CitizenAIOVerrides).GetMethod("GenerateCitizenName", RedirectorUtils.allFlags);
            MethodInfo GetNameMethod = typeof(CitizenAI).GetMethod("GenerateCitizenName", RedirectorUtils.allFlags);
            LogUtils.DoLog($"Overriding GetName ({GetNameMethod} => {preRename})");
            AddRedirect(GetNameMethod, preRename);
            #endregion
        }

        #endregion
    }
}
