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

        public override Enum[] AccessibleSubmenusEnum => Enum.GetValues(typeof(VariableBuildingSubType)).Cast<Enum>().Where(x => (VariableBuildingSubType)x != VariableBuildingSubType.None).ToArray();

        public override Dictionary<Enum, CommandLevel> CommandTree => CustomDataModBuildingVariables.ReadCommandTree();

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
                        && CustomDataModBuildingVariables.ReadData(tt, parameterPath.Skip(2).ToArray(), ref subtype, out paramContainer))
                    {
                        type = RootMenuEnumValueWithPrefix;
                        paramContainer.contentType = tt.GetContentType();
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
