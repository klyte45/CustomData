using CustomData.Utils;
using CustomData.Xml;
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
                xml.Icon = value;
            }
        }

    }
}
