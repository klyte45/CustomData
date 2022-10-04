using CustomData.Utils;
using CustomData.Xml;

namespace CustomData.Wrappers
{
    public class CitizenGeneralDW : CSDataWrapperBase
    {
        protected override InstanceType RequiredType => (InstanceType)InstanceIdUtils.TYPE_CITIZEN;
        protected override bool ExclusiveToIndex => true;
        protected override bool AnyButIndex => false;
        protected override int RefIndex { get; } = 0;
        public CitizenGeneralDW(InstanceDataExtensionXml xml) : base(xml)
        {
        }
        public string MalesFile { get => xml.SafeGetReference(0).mainReference; set => xml.SafeGetReference(0).mainReference = value; }
        public string FemalesFile { get => xml.SafeGetReference(0).shortReference; set => xml.SafeGetReference(0).shortReference = value; }
        public string SurnamesFile { get => xml.SafeGetReference(0).qualifiedReference; set => xml.SafeGetReference(0).qualifiedReference = value; }
        public bool SurnameFirst { get => xml.HasAnyFlag(0x1); set => _ = value ? xml.AddFlag(0x1) : xml.RemoveFlag(0x1); }
    }
}
