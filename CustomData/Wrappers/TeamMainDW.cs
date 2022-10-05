using CustomData.Utils;
using CustomData.Xml;

namespace CustomData.Wrappers
{
    public abstract class TeamMainDW : CSDataWrapperBase
    {
        protected override InstanceType RequiredType { get; } = (InstanceType)InstanceIdUtils.TYPE_CD_TEAMS;
        protected override bool AnyButIndex { get; } = false;
        protected override bool ExclusiveToIndex { get; } = true;
        protected override int RefIndex { get; } = 0;
        protected TeamMainDW(InstanceDataExtensionXml xml) : base(xml) { }
        public string OpponentsGenerationFile { get => xml.SafeGetReference(0).mainReference; set => xml.SafeGetReference(0).mainReference = value; }
    }
}
