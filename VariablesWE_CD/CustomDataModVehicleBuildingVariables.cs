extern alias WE;
using CustomData.Enums;
using CustomData.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using WE::WriteEverywhere.Font.Utility;
using WE::WriteEverywhere.Layout;
using WE::WriteEverywhere.Plugins;
using WE::WriteEverywhere.Plugins.Ext;
using WE::WriteEverywhere.Xml;

namespace VariablesWE_CD
{
    public class CustomDataModVehicleBuildingVariables : WEVariableExtensionEnum
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

        public override Enum RootMenuEnumValueWithPrefix => CDVars.SourceBuilding;

        public override string RootMenuDescription => RootMenuEnumValueWithPrefix.ValueToI18n();

        public override bool Supports(TextRenderingClass renderingClass) => renderingClass == TextRenderingClass.Vehicle;

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

        public override string GetTargetTextForVehicle(TextParameterVariableWrapper wrapper, ushort vehicleId, int secRefId, int tercRefId, TextToWriteOnXml textDescriptor, out IEnumerable<BasicRenderInformation> multipleOutput, out string[] preLoad)
        {
            preLoad = null;
            if (vehicleId == 0)
            {
                multipleOutput = null;
                return null;
            }
            return CustomDataModBuildingVariables.GetText(VehicleManager.instance.m_vehicles.m_buffer[vehicleId].m_sourceBuilding, wrapper.subtype, out multipleOutput);
        }
        public override string GetSubvalueDescription(Enum subRef) => subRef.ValueToI18n();
    }
}
