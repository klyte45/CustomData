extern alias WE;
using CustomData.Xml;
using Kwytto.Interfaces;

namespace VariablesWE_CD
{
    public class VehicleItemCache : IIdentifiable
    {
        public ushort vehicleId;
        public long? Id { get => vehicleId; set => vehicleId = (ushort)(value ?? 0); }
        public string Identifier
        {
            get
            {
                if (identifier is null)
                {
                    identifier = CDStorage.Instance.GetVehicleSettings(vehicleId, true).GetVehicleIdentifier();
                }
                return identifier;
            }
        }
        private string identifier;
    }
}
