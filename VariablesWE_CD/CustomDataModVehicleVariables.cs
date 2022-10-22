extern alias WE;
using CustomData.Enums;
using CustomData.Localization;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using WE::WriteEverywhere.Font.Utility;
using WE::WriteEverywhere.Layout;
using WE::WriteEverywhere.Plugins;
using WE::WriteEverywhere.Plugins.Ext;
using WE::WriteEverywhere.Xml;

namespace VariablesWE_CD
{
    public class CustomDataModVehicleVariables : WEVariableExtensionEnum
    {
        private static Dictionary<Enum, CommandLevel> ReadCommandTree()
        {
            Dictionary<Enum, CommandLevel> result = new Dictionary<Enum, CommandLevel>();
            foreach (var value in Enum.GetValues(typeof(VariableVehicleSubType)).Cast<VariableVehicleSubType>())
            {
                if (value == 0)
                {
                    continue;
                }

                result[value] = GetCommandLevel(value);
            }
            return result;
        }
        private static CommandLevel GetCommandLevel(VariableVehicleSubType var)
        {
            switch (var)
            {
                case VariableVehicleSubType.GeneratedId:
                    return CommandLevel.m_appendPrefix;
                default:
                    return null;
            }
        }
        private static bool ReadData(VariableVehicleSubType var, string[] relativeParams, ref Enum subtype, out VariableExtraParameterContainer extraParams)
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

        public override Enum[] AccessibleSubmenusEnum => Enum.GetValues(typeof(VariableVehicleSubType)).Cast<Enum>().Where(x => (VariableVehicleSubType)x != VariableVehicleSubType.None).ToArray();

        public override Dictionary<Enum, CommandLevel> CommandTree => ReadCommandTree();

        public override Enum DefaultValue => VariableVehicleSubType.None;

        public override Enum RootMenuEnumValueWithPrefix => CDVars.CurrentVehicle;

        public override string RootMenuDescription => RootMenuEnumValueWithPrefix.ValueToI18n();

        public override bool Supports(TextRenderingClass renderingClass) => renderingClass == TextRenderingClass.Vehicle;

        protected override void Validate_Internal(string[] parameterPath, ref Enum type, ref Enum subtype, ref byte index, ref VariableExtraParameterContainer paramContainer)
        {
            if (parameterPath.Length >= 2)
            {
                try
                {
                    if (Enum.Parse(typeof(VariableVehicleSubType), parameterPath[1]) is VariableVehicleSubType tt
                        && ReadData(tt, parameterPath.Skip(2).ToArray(), ref subtype, out paramContainer))
                    {
                        type = RootMenuEnumValueWithPrefix;
                        paramContainer.contentType = TextContent.ParameterizedText;
                    }
                }
                catch { }
            }
        }

        public override string GetTargetTextForVehicle(TextParameterVariableWrapper wrapper, ushort vehicleId, int secRefId, int tercRefId, TextToWriteOnXml textDescriptor, out IEnumerable<BasicRenderInformation> multipleOutput, out string[] preLoad)
        {
            preLoad = null;
            var controller = CDVarController.Instance;
            multipleOutput = null;
            if (vehicleId == 0 || controller is null)
            {
                return null;
            }
            switch (wrapper.subtype)
            {
                case VariableVehicleSubType.GeneratedId:
                    return CDVarController.Instance.SafeGetVehicle(vehicleId).Identifier;
            }
            return null;
        }
        public override string GetSubvalueDescription(Enum subRef) => subRef.ValueToI18n();
    }
}
