using CustomData.Overrides;
using CustomData.Utils;
using CustomData.Xml;
using UnityEngine;

namespace CustomData.Wrappers
{
    public class TransportLineDW : CSDataWrapperBase
    {
        protected override InstanceType RequiredType => (InstanceType)InstanceIdUtils.TYPE_TRANSPORTLINE;
        protected override bool ExclusiveToIndex => false;
        protected override bool AnyButIndex => true;
        protected override int RefIndex { get; } = 0;
        public TransportLineDW(InstanceDataExtensionXml xml) : base(xml)
        {
        }
        
        public Texture2D LineIcon
        {
            get => xml.Icon; set
            {
                GameObject.Destroy(xml.Icon);
                CDFacade.Instance.CallEventOnBuildingLogoChanged(xml.Id.Building);
                xml.Icon = value;
            }
        }

    }
}
