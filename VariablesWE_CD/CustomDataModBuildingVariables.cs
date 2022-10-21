extern alias WE;
using CustomData.Enums;
using CustomData.Localization;
using CustomData.Xml;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using WE::WriteEverywhere.Font.Utility;
using WE::WriteEverywhere.Layout;
using WE::WriteEverywhere.Plugins;
using WE::WriteEverywhere.Plugins.Ext;
using WE::WriteEverywhere.Xml;

namespace VariablesWE_CD
{
    public class CustomDataModBuildingVariables : WEVariableExtensionEnum
    {
        private static Dictionary<Enum, CommandLevel> ReadCommandTree()
        {
            Dictionary<Enum, CommandLevel> result = new Dictionary<Enum, CommandLevel>();
            foreach (var value in Enum.GetValues(typeof(VariableBuildingSubType)).Cast<VariableBuildingSubType>())
            {
                if (value == 0)
                {
                    continue;
                }

                result[value] = GetCommandLevel(value);
            }
            return result;
        }
        private static CommandLevel GetCommandLevel(VariableBuildingSubType var)
        {
            switch (var)
            {
                case VariableBuildingSubType.ImageLogo:
                    return CommandLevel.m_endLevel;
                default:
                    return null;
            }
        }
        private static bool ReadData(VariableBuildingSubType var, string[] relativeParams, ref Enum subtype, out VariableExtraParameterContainer extraParams)
        {
            var cmdLevel = GetCommandLevel(var);
            if (cmdLevel is null)
            {
                extraParams = default;
                return false;
            }

            cmdLevel.ParseFormatting(relativeParams, out extraParams);
            subtype = var;
            return true;
        }

        public override Enum[] AccessibleSubmenusEnum => Enum.GetValues(typeof(VariableBuildingSubType)).Cast<Enum>().Where(x => (VariableBuildingSubType)x != VariableBuildingSubType.None).ToArray();

        public override Dictionary<Enum, CommandLevel> CommandTree => ReadCommandTree();

        public override Enum DefaultValue => VariableBuildingSubType.None;

        public override Enum RootMenuEnumValueWithPrefix => CDVars.CurrentBuilding;

        public override string RootMenuDescription => RootMenuEnumValueWithPrefix.ValueToI18n();

        public override bool Supports(TextRenderingClass renderingClass)
        {
            return renderingClass == TextRenderingClass.Buildings;
        }

        protected override void Validate_Internal(string[] parameterPath, ref Enum type, ref Enum subtype, ref byte index, ref VariableExtraParameterContainer paramContainer)
        {
            if (parameterPath.Length >= 2)
            {
                try
                {
                    if (Enum.Parse(typeof(VariableBuildingSubType), parameterPath[1]) is VariableBuildingSubType tt
                        && ReadData(tt, parameterPath.Skip(2).ToArray(), ref subtype, out paramContainer))
                    {
                        type = RootMenuEnumValueWithPrefix;
                        paramContainer.contentType = TextContent.ParameterizedSpriteSingle;
                    }
                }
                catch { }
            }
        }



        public override string GetTargetTextForBuilding(TextParameterVariableWrapper wrapper, WriteOnBuildingXml propGroupDescriptor, WriteOnBuildingPropXml buildingDescriptor, ushort buildingId, int secRefId, int tercRefId, TextToWriteOnXml textDescriptor, out IEnumerable<BasicRenderInformation> multipleOutput, out string[] preLoad)
        {
            preLoad = null;
            return GetText(buildingId, wrapper.subtype, out multipleOutput);
        }

        internal static string GetText(ushort buildingId, Enum subtype, out IEnumerable<BasicRenderInformation> multipleOutput)
        {
            var controller = CDVarController.Instance;
            if (buildingId == 0 || controller is null)
            {
                multipleOutput = null;
                return null;
            }
            switch (subtype)
            {
                case VariableBuildingSubType.ImageLogo:
                    if (!controller.CachedBuildingImages.ContainsKey(buildingId))
                    {
                        var data = CDStorage.Instance.GetBuildingSettings(buildingId);
                        controller.CachedBuildingImages[buildingId] = data.Logo is Texture2D tex
                            ? WERenderingHelper.GenerateBri(tex)
                            : null;
                    }
                    multipleOutput = new[] { controller.CachedBuildingImages[buildingId] };
                    break;
                default:
                    multipleOutput = null;
                    break;
            }
            return null;
        }

        public override string GetSubvalueDescription(Enum subRef) => subRef.ValueToI18n();
    }
}
