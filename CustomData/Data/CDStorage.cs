using CustomData.Wrappers;
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

        public DistrictDW GetDistrictData(byte districtId) => new DistrictDW(SafeGet(new InstanceID { District = districtId }));

        private InstanceDataExtensionXml SafeGet(InstanceID targetIdx)
        {
            if (!InstanceExtraData.TryGetValue(targetIdx.RawData, out var selectedDistrictData))
            {
                selectedDistrictData = CDStorage.Instance.InstanceExtraData[targetIdx.RawData] = new InstanceDataExtensionXml { Id = targetIdx };
            }

            return selectedDistrictData;
        }
    }
}
