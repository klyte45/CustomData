using CustomData.Xml;

namespace CustomData.Wrappers
{
    public sealed class LocalTeamDW : TeamDW
    {
        protected override int MinIndex { get; } = 0x1;
        protected override int MaxIndex { get; } = 0xFFFF;
        public LocalTeamDW(InstanceDataExtensionXml xml) : base(xml) { }
        public override ushort StadiumId => (ushort)xml.Id.Index;
        public override int CityId { get => 0xFFFFFF; set { } }
        public override void AddModality(EventManager.EventType group) { }
        public override void RemoveModality(EventManager.EventType group) { }
        public override bool SupportsModality(EventManager.EventType group) => BuildingManager.instance.m_buildings.m_buffer[StadiumId].Info.m_buildingAI is MonumentAI ai && (ai.m_supportEvents & group) == group;

    }
}
