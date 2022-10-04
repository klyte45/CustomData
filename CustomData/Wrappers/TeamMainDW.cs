using CustomData.Utils;
using CustomData.Xml;

namespace CustomData.Wrappers
{
    public abstract class TeamMainDW : CSDataWrapperBase
    {
        protected override bool AnyButIndex { get; } = false;
        protected override bool ExclusiveToIndex { get; } = true;
        protected override int RefIndex { get; } = 0;
        protected TeamMainDW(InstanceDataExtensionXml xml) : base(xml) { }

        public string OpponentsGenerationFile { get => xml.SafeGetReference(0).mainReference; set => xml.SafeGetReference(0).mainReference = value; }
    }

    public class SoccerMainTeamDW : TeamMainDW { public SoccerMainTeamDW(InstanceDataExtensionXml xml) : base(xml) { } protected override InstanceType RequiredType { get; } = (InstanceType)InstanceIdUtils.TYPE_CD_SOCCERTEAMS; }
    public class BaseballMainTeamDW : TeamMainDW { public BaseballMainTeamDW(InstanceDataExtensionXml xml) : base(xml) { } protected override InstanceType RequiredType { get; } = (InstanceType)InstanceIdUtils.TYPE_CD_BASEBALLTEAMS; }
    public class BasketballMainTeamDW : TeamMainDW { public BasketballMainTeamDW(InstanceDataExtensionXml xml) : base(xml) { } protected override InstanceType RequiredType { get; } = (InstanceType)InstanceIdUtils.TYPE_CD_BASKETBALLTEAMS; }
    public class FootballMainTeamDW : TeamMainDW { public FootballMainTeamDW(InstanceDataExtensionXml xml) : base(xml) { } protected override InstanceType RequiredType { get; } = (InstanceType)InstanceIdUtils.TYPE_CD_FOOTBALLTEAMS; }
}
