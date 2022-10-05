using CustomData.Utils;
using CustomData.Xml;
using UnityEngine;

namespace CustomData.Wrappers
{
    public class DistrictDW : CSDataWrapperBase
    {
        protected override InstanceType RequiredType => (InstanceType)InstanceIdUtils.TYPE_DISTRICT;
        protected override bool ExclusiveToIndex => false;
        protected override bool AnyButIndex => false;
        protected override int RefIndex { get; } = 0;
        public DistrictDW(InstanceDataExtensionXml xml) : base(xml)
        {
        }
        public string RoadQualifierFile { get => xml.SafeGetReference(0).qualifiedReference; set => xml.SafeGetReference(0).qualifiedReference = value; }
        public string RoadNamesFile { get => xml.SafeGetReference(0).mainReference; set => xml.SafeGetReference(0).mainReference = value; }
        public int? DigitsPostalCode { get => xml.genericId is null ? (int)xml.Id.Index % 1000 : xml.genericId % 1000; set => xml.genericId = value is null ? null : value % 1000; }
        public Color? Color { get => xml.color1; set => xml.color1 = value; }

    }
}
