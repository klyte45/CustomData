using ColossalFramework;
using CustomData.Xml;
using System;

namespace CustomData.Wrappers
{
    public class VehicleDW : CSDataWrapperBase
    {
        protected override InstanceType RequiredType => InstanceType.Vehicle;
        protected override bool ExclusiveToIndex => false;
        protected override bool AnyButIndex => true;
        protected override int RefIndex { get; } = 0;
        public VehicleDW(InstanceDataExtensionXml xml) : base(xml)
        {
        }

        private string cachedIdentifier;
        private string formatCachedIdentifier;

        public uint GetSeqIdFromDepot()
        {
            if (xml.sourceEnumeratorReceivedId is uint i)
            {
                return i;
            }
            var vehicleId = Id.Vehicle;
            var srcBuilding = VehicleManager.instance.m_vehicles.m_buffer[vehicleId].m_sourceBuilding;
            if (srcBuilding == 0)
            {
                return (xml.sourceEnumeratorReceivedId = vehicleId).Value;
            }
            var buildingConfig = CDStorage.Instance.GetBuildingSettings(VehicleManager.instance.m_vehicles.m_buffer[vehicleId].m_sourceBuilding, true);
            xml.sourceEnumeratorReceivedId = buildingConfig.GetNextVehicleId();
            return xml.sourceEnumeratorReceivedId.Value;
        }

        public string GetVehicleIdentifier()
        {
            var vehicleId = Id.Vehicle;
            var srcBuilding = VehicleManager.instance.m_vehicles.m_buffer[vehicleId].m_sourceBuilding;
            if (srcBuilding == 0)
            {
                return vehicleId.ToString("00000");
            }
            var buildingConfig = CDStorage.Instance.GetBuildingSettings(VehicleManager.instance.m_vehicles.m_buffer[vehicleId].m_sourceBuilding, false);
            if (!(buildingConfig?.OwnVehiclesIdFormatter is string identifierFormat) || identifierFormat.Length == 0)
            {
                return vehicleId.ToString("00000");
            }
            if (formatCachedIdentifier == identifierFormat)
            {
                return cachedIdentifier;
            }
            formatCachedIdentifier = identifierFormat;

            var firstVehicle = VehicleManager.instance.m_vehicles.m_buffer[vehicleId].GetFirstVehicle(vehicleId);
            ref Vehicle vehicle = ref VehicleManager.instance.m_vehicles.m_buffer[firstVehicle];

            var tlId = vehicle.m_transportLine;
            ref TransportLine tl = ref TransportManager.instance.m_lines.m_buffer[tlId];

            string result = "";

            string vehicleString = null;
            string vehicleNthDepot = null;
            string vehicleNthTrailer = null;

            string GetVehicleInstanceString()
            {
                if (vehicleString == null)
                {
                    vehicleString = vehicleId.ToString().PadLeft(5, '\0');
                }
                return vehicleString;
            }

            string GetVehicleNthDepot()
            {
                if (vehicleNthDepot == null)
                {
                    vehicleNthDepot = GetSeqIdFromDepot().ToString().PadLeft(5, '\0');
                }
                return vehicleNthDepot;
            }

            string GetVehicleNthTrailer()
            {
                if (vehicleNthTrailer == null)
                {
                    int counter = 0;
                    ref Vehicle[] vBuffer = ref VehicleManager.instance.m_vehicles.m_buffer;
                    var nextVehicle = firstVehicle;
                    while (nextVehicle != vehicleId)
                    {
                        nextVehicle = vBuffer[nextVehicle].m_trailingVehicle;
                        counter++;
                        if (nextVehicle == 0)
                        {
                            counter = -1;
                            break;
                        }
                        if (counter > 16384)
                        {
                            CODebugBase<LogChannel>.Error(LogChannel.Core, "Invalid list detected!B\n" + Environment.StackTrace);
                            break;
                        }
                    }
                    vehicleNthTrailer = counter.ToString().PadLeft(3, '\0'); ;
                }
                return vehicleNthTrailer;
            }


            char GetLetter(char item)
            {
                switch (item)
                {
                    case 'J': return GetVehicleNthTrailer().Replace('\0', '0')[0];
                    case 'K': return GetVehicleNthTrailer().Replace('\0', '0')[1];
                    case 'L': return GetVehicleNthTrailer().Replace('\0', '0')[2];

                    case 'A': return GetVehicleNthDepot().Replace('\0', '0')[0];
                    case 'B': return GetVehicleNthDepot().Replace('\0', '0')[1];
                    case 'C': return GetVehicleNthDepot().Replace('\0', '0')[2];
                    case 'D': return GetVehicleNthDepot().Replace('\0', '0')[3];
                    case 'E': return GetVehicleNthDepot().Replace('\0', '0')[4];

                    case 'V': return GetVehicleInstanceString().Replace('\0', '0')[0];
                    case 'W': return GetVehicleInstanceString().Replace('\0', '0')[1];
                    case 'X': return GetVehicleInstanceString().Replace('\0', '0')[2];
                    case 'Y': return GetVehicleInstanceString().Replace('\0', '0')[3];
                    case 'Z': return GetVehicleInstanceString().Replace('\0', '0')[4];

                    case 'j': return GetVehicleNthTrailer()[0];
                    case 'k': return GetVehicleNthTrailer()[1];
                    case 'l': return GetVehicleNthTrailer()[2];

                    case 'a': return GetVehicleNthDepot()[0];
                    case 'b': return GetVehicleNthDepot()[1];
                    case 'c': return GetVehicleNthDepot()[2];
                    case 'd': return GetVehicleNthDepot()[3];
                    case 'e': return GetVehicleNthDepot()[4];

                    case 'v': return GetVehicleInstanceString()[0];
                    case 'w': return GetVehicleInstanceString()[1];
                    case 'x': return GetVehicleInstanceString()[2];
                    case 'y': return GetVehicleInstanceString()[3];
                    case 'z': return GetVehicleInstanceString()[4];

                    default: return item;
                };
            }

            bool escapeNext = false;
            foreach (char item in identifierFormat)
            {
                if (escapeNext)
                {
                    result += item;
                    escapeNext = false;
                }
                else if (item == '\\')
                {
                    escapeNext = true;
                }
                else
                {
                    result += GetLetter(item);
                }
            }
            return cachedIdentifier = result.Replace("\0", "").Trim();
        }



    }
}
