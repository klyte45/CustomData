using CustomData.Utils;
using CustomData.Xml;

namespace CustomData.Wrappers
{
    public class HighwayTypeDW : CSDataWrapperBase
    {
        private const ulong FLAG_SIMPLE_SPACE = 0x1;
        private const ulong FLAG_SIMPLE_BEFORE = 0x2;
        private const ulong FLAG_EXTENSIVE_SPACE = 0x10;
        private const ulong FLAG_EXTENSIVE_BEFORE = 0x20;
        protected override InstanceType RequiredType => (InstanceType)InstanceIdUtils.TYPE_CD_HIGHWAYKIND;
        protected override bool ExclusiveToIndex => false;
        protected override bool AnyButIndex => true;
        protected override int RefIndex { get; } = 0;
        public HighwayTypeDW(InstanceDataExtensionXml xml) : base(xml)
        {
        }
        public string SimpleQualifier { get => xml.SafeGetReference(0).shortReference; set => xml.SafeGetReference(0).shortReference = value; }
        public string ExtensiveQualifier { get => xml.SafeGetReference(0).qualifiedReference; set => xml.SafeGetReference(0).qualifiedReference = value; }
        public string Name { get => xml.SafeGetReference(0).mainReference; set => xml.SafeGetReference(0).mainReference = value; }
        public bool AddSpaceSimple { get => xml.HasAnyFlag(FLAG_SIMPLE_SPACE); set => _ = value ? xml.AddFlag(FLAG_SIMPLE_SPACE) : xml.RemoveFlag(FLAG_SIMPLE_SPACE); }
        public bool AddSpaceExtensive { get => xml.HasAnyFlag(FLAG_EXTENSIVE_SPACE); set => _ = value ? xml.AddFlag(FLAG_EXTENSIVE_SPACE) : xml.RemoveFlag(FLAG_EXTENSIVE_SPACE); }
        public bool SimpleIsBeforeId { get => xml.HasAnyFlag(FLAG_SIMPLE_BEFORE); set => _ = value ? xml.AddFlag(FLAG_SIMPLE_BEFORE) : xml.RemoveFlag(FLAG_SIMPLE_BEFORE); }
        public bool ExtensiveIsBeforeId { get => xml.HasAnyFlag(FLAG_EXTENSIVE_BEFORE); set => _ = value ? xml.AddFlag(FLAG_EXTENSIVE_BEFORE) : xml.RemoveFlag(FLAG_EXTENSIVE_BEFORE); }

    }
}
