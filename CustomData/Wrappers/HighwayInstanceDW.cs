﻿using CustomData.Utils;
using CustomData.Xml;

namespace CustomData.Wrappers
{
    public class HighwayInstanceDW : CSDataWrapperBase
    {
        protected override InstanceType RequiredType => (InstanceType)InstanceIdUtils.TYPE_CD_HIGHWAYINSTANCE;
        protected override bool ExclusiveToIndex => false;
        protected override bool AnyButIndex => true;
        protected override int RefIndex { get; } = 0;
        public HighwayInstanceDW(InstanceDataExtensionXml xml) : base(xml)
        {
        }
        public InstanceID Type { get => new InstanceID { Index = (uint)(xml.genericId ?? 0), Type = (InstanceType)InstanceIdUtils.TYPE_CD_HIGHWAYKIND }; set => xml.genericId = (int)value.Index; }
        public string Identifier { get => xml.givenStringId; set => xml.givenStringId = value; }
        public string ForceName { get => xml.mainReference; set => xml.mainReference = value; }
        public float KmStart { get => xml.generalFloatValue ?? 0; set => xml.generalFloatValue = value; }
        public StoredMileageStartSource KmStartReference { get => (StoredMileageStartSource)xml.GetFromPattern(0xFF, 0); set => xml.SetToPattern((ulong)value, 0xff, 0); }
        public StoredAxis HighwayAxis { get => (StoredAxis)xml.GetFromPattern(0xFF00, 8); set => xml.SetToPattern((ulong)value, 0xff00, 8); }

        public enum StoredMileageStartSource
        {
            FROM_CITYCENTER,
            DEFAULT,
            FROM_N,
            FROM_NE,
            FROM_E,
            FROM_SE,
            FROM_S,
            FROM_SW,
            FROM_W,
            FROM_NW,
        }
        public enum StoredAxis
        {
            N_S,
            NE_SW,
            E_W,
            SE_NW,
        }
    }
}
