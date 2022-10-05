using CustomData.Xml;
using System.Linq;

namespace CustomData.Wrappers
{
    public abstract class CSDataWrapperBase
    {
        protected InstanceDataExtensionXml xml;
        public InstanceID Id => xml.Id;
        protected abstract InstanceType RequiredType { get; }
        protected abstract bool ExclusiveToIndex { get; }
        protected abstract bool AnyButIndex { get; }
        protected virtual int RefIndex { get; } = 0;
        protected virtual uint[] DisallowedIndxes { get; } = new uint[0];
        public CSDataWrapperBase(InstanceDataExtensionXml xml)
        {
            this.xml = xml;
            ValidateXml();
        }

        protected void ValidateXml()
        {
            if (xml?.Id.Type != RequiredType)
            {
                throw new System.Exception($"Invalid xml for Wrapper! (found Type '{(((int)xml?.Id.Type).ToString("X2") ?? "null")}', required '{(int)RequiredType:X2}')");
            }
            if (ExclusiveToIndex && AnyButIndex)
            {
                throw new System.Exception("This class is misconfigured! Check it! (ExclusiveToIndex && AnyButIndex == true)");
            }
            if (ExclusiveToIndex && xml.Id.Index != RefIndex)
            {
                throw new System.Exception($"Invalid xml for Wrapper! (found Index '{xml.Id.Index:X6}', required '{RefIndex:X6}')");
            }
            if (AnyButIndex && xml.Id.Index == RefIndex)
            {
                throw new System.Exception($"Invalid xml for Wrapper! (found Index '{xml.Id.Index:X6}' that's not permitted to this wrapper!)");
            }
            if (DisallowedIndxes.Length > 0 && DisallowedIndxes.Contains(xml.Id.Index))
            {
                throw new System.Exception($"Invalid xml for Wrapper! (found Index '{xml.Id.Index:X6}' that's not permitted to this wrapper!)");
            }
        }
    }

}
