using Kwytto.Data;
using Kwytto.Utils;
using System.Xml.Serialization;

namespace CustomData.Xml
{

    public class CDStorage : DataExtensionBase<CDStorage>
    {
        public override string SaveId => "K45_CD_CDStorage";

        [XmlElement("InstanceExtraData")]
        public NonSequentialList<InstanceDataExtensionXml> InstanceExtraData { get; set; } = new NonSequentialList<InstanceDataExtensionXml>();
    }
}
