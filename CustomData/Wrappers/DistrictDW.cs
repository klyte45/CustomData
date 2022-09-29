using CustomData.Utils;
using CustomData.Xml;
using UnityEngine;

namespace CustomData.Wrappers
{
    public class DistrictDW : CSDataWrapperBase
    {
        protected override InstanceType RequiredType => (InstanceType)InstanceIdUtils.TYPE_DISTRICT;
        protected override bool ExclusiveToIndex => false;
        protected override bool AnyButIndex => true;
        protected override int RefIndex { get; } = 0;
        public DistrictDW(InstanceDataExtensionXml xml) : base(xml)
        {
        }
        public string RoadQualifierFile { get => xml.qualifiedReference; set => xml.qualifiedReference = value; }
        public string RoadNamesFile { get => xml.mainReference; set => xml.mainReference = value; }
        public uint DigitsPostalCode { get => (uint)xml.flags % 1000; set => xml.flags = value % 1000; }
        public Color Color { get => xml.color ?? Color.white; set => xml.color = value; }

    }
}
