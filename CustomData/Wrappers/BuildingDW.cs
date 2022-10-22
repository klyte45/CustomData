using CustomData.Overrides;
using CustomData.Utils;
using CustomData.Xml;
using Kwytto.Utils;
using UnityEngine;

namespace CustomData.Wrappers
{
    public class BuildingDW : CSDataWrapperBase
    {
        protected override InstanceType RequiredType => (InstanceType)InstanceIdUtils.TYPE_BUILDING;
        protected override bool ExclusiveToIndex => false;
        protected override bool AnyButIndex => true;
        protected override int RefIndex { get; } = 0;
        public BuildingDW(InstanceDataExtensionXml xml) : base(xml)
        {
        }
        public Texture2D Logo
        {
            get => xml.Icon; set
            {
                GameObject.Destroy(xml.Icon);
                CDFacade.Instance.CallEventOnBuildingLogoChanged(xml.Id.Building);
                xml.Icon = value;
            }
        }

        public Color? VehiclesColor { get => xml.color1; set => xml.color1 = value; }
        public bool OverrideLineColor { get => xml.HasAnyFlag(1); set => _ = value ? xml.AddFlag(1) : xml.RemoveFlag(1); }
        public string PreferredSkin
        {
            get => xml.givenStringId; set
            {
                xml.givenStringId = value.TrimToNull();
                CDFacade.Instance.CallEventOnBuildingVehicleSkinChanged(Id.Building);
            }
        }
        public string OwnVehiclesIdFormatter
        {
            get => xml.SafeGetReference(0).qualifiedReference; set
            {
                xml.SafeGetReference(0).qualifiedReference = value;
                CDFacade.Instance.CallEventOnBuildingVehicleIdPatternChanged(Id.Building);
            }
        }

        internal uint GetNextVehicleId()
        {
            if (xml.sourceEnumeratorNextId is uint ui)
            {
                xml.sourceEnumeratorNextId = ui + 1;
                return ui;
            }
            xml.sourceEnumeratorNextId = 1;
            return 0;
        }
    }
}
