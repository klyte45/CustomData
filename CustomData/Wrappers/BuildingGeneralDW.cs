﻿using CustomData.Utils;
using CustomData.Xml;

namespace CustomData.Wrappers
{
    public class BuildingGeneralDW : CSDataWrapperBase
    {
        protected override InstanceType RequiredType => (InstanceType)InstanceIdUtils.TYPE_BUILDING;
        protected override bool ExclusiveToIndex => true;
        protected override bool AnyButIndex => false;
        protected override int RefIndex { get; } = 0;
        public BuildingGeneralDW(InstanceDataExtensionXml xml) : base(xml)
        {
        }
        public string AddressLine1 { get => xml.mainReference; set => xml.mainReference = value; }
        public string AddressLine2 { get => xml.qualifiedReference; set => xml.qualifiedReference = value; }
        public string AddressLine3 { get => xml.shortReference; set => xml.shortReference = value; }
        public bool GetIsAutogen(UseGeneratedNameCategories cat) => xml.HasAnyFlag(1ul << (int)cat);
        public void SetAutogen(UseGeneratedNameCategories cat) => xml.AddFlag(1ul << (int)cat);
        public void UnsetAutogen(UseGeneratedNameCategories cat) => xml.RemoveFlag(1ul << (int)cat);

        public enum UseGeneratedNameCategories
        {
            Bus,
            IntercityBus,
            Trolleybus,
            Tram,
            TrainPassenger,
            Monorail,
            Metro,
            CableCar,
            Ferry,
            ShipPassenger,
            Helicopters,
            Blimps,
            AirplanePassenger,
            ShipCargo = 0x14,
            TrainCargo,
            AirplaneCargo,
            ResidentialZone = 0x24,
            CommercialZone,
            IndustrialZone,
            OfficeZone
        }


    }
}