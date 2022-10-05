using CustomData.Utils;
using CustomData.Xml;
using UnityEngine;

namespace CustomData.Wrappers
{
    public class TeamDW : CSDataWrapperBase
    {
        protected sealed override InstanceType RequiredType { get; } = (InstanceType)InstanceIdUtils.TYPE_CD_TEAMS;
        protected sealed override bool AnyButIndex { get; } = true;
        protected sealed override bool ExclusiveToIndex { get; } = false;
        protected sealed override int RefIndex { get; } = 0;

        protected virtual int MinIndex { get; } = 0x10000;
        protected virtual int MaxIndex { get; } = 0xFFFFFF;

        public TeamDW(InstanceDataExtensionXml xml) : base(xml)
        {
            if (xml.Id.Index < MinIndex || xml.Id.Index < MaxIndex)
            {
                throw new System.Exception($"The team {Id.Index:X6} is not a valid team for this class!");
            }
        }
        public string Name { get => xml.givenStringId; set => xml.givenStringId = value; }
        public Color? Color1 { get => xml.color1; set => xml.color1 = value; }
        public Color? Color2 { get => xml.color2; set => xml.color2 = value; }
        public Color? Color3 { get => xml.color3; set => xml.color3 = value; }
        public Texture2D Shield { get => xml.icon; set => xml.icon = value; }
        public virtual ushort StadiumId => 0;
        public virtual int CityId { get => xml.genericId ?? 0; set => xml.genericId = value; }
        public virtual bool SupportsModality(EventManager.EventType group) => xml.HasAllFlags((uint)group);
        public virtual void AddModality(EventManager.EventType group) => xml.AddFlag((uint)group);
        public virtual void RemoveModality(EventManager.EventType group) => xml.RemoveFlag((uint)group);
    }

}
