using CustomData.Utils;
using CustomData.Xml;
using UnityEngine;

namespace CustomData.Wrappers
{
    public abstract class TeamDW : CSDataWrapperBase
    {
        protected override bool AnyButIndex { get; } = true;
        protected override bool ExclusiveToIndex { get; } = false;
        protected override int RefIndex { get; } = 0;
        protected TeamDW(InstanceDataExtensionXml xml) : base(xml) { }
        public string Name { get => xml.givenStringId; set => xml.givenStringId = value; }
        public Color? AdditionalColor { get => xml.color; set => xml.color = value; }
        public Texture2D Shield { get => xml.icon; set => xml.icon = value; }
        public ushort StadiumId { get => (ushort)(xml.genericId ?? 0); set => xml.genericId = value; }
        public byte CityId { get => (byte)xml.GetFromPattern(0xFF, 0); set => xml.SetToPattern(value, 0xFF, 0); }

    }
    public class SoccerTeamDW : TeamDW { public SoccerTeamDW(InstanceDataExtensionXml xml) : base(xml) { } protected override InstanceType RequiredType { get; } = (InstanceType)InstanceIdUtils.TYPE_CD_SOCCERTEAMS; }
    public class BaseballTeamDW : TeamDW { public BaseballTeamDW(InstanceDataExtensionXml xml) : base(xml) { } protected override InstanceType RequiredType { get; } = (InstanceType)InstanceIdUtils.TYPE_CD_BASEBALLTEAMS; }
    public class BasketballTeamDW : TeamDW { public BasketballTeamDW(InstanceDataExtensionXml xml) : base(xml) { } protected override InstanceType RequiredType { get; } = (InstanceType)InstanceIdUtils.TYPE_CD_BASKETBALLTEAMS; }
    public class FootballTeamDW : TeamDW { public FootballTeamDW(InstanceDataExtensionXml xml) : base(xml) { } protected override InstanceType RequiredType { get; } = (InstanceType)InstanceIdUtils.TYPE_CD_FOOTBALLTEAMS; }
}
