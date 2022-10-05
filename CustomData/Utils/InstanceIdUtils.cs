namespace CustomData.Utils
{
    public static class InstanceIdUtils
    {
        public const byte TYPE_BUILDING = 0x1;
        public const byte TYPE_VEHICLE = 0x2;
        public const byte TYPE_DISTRICT = 0x3;
        public const byte TYPE_CITIZEN = 0x4;
        public const byte TYPE_NETNODE = 0x5;
        public const byte TYPE_NETSEGMENT = 0x6;
        public const byte TYPE_PARKEDVEHICLE = 0x7;
        public const byte TYPE_TRANSPORTLINE = 0x8;
        public const byte TYPE_CITIZENINSTANCE = 0x9;
        public const byte TYPE_PROP = 0xa;
        public const byte TYPE_TREE = 0xb;
        public const byte TYPE_EVENT = 0xc;
        public const byte TYPE_NETLANE = 0xd;
        public const byte TYPE_BUILDINGPROPS = 0xe;
        public const byte TYPE_NETPROPS = 0xf;
        public const byte TYPE_DISASTER = 0x10;
        public const byte TYPE_LIGHTINING = 0x11;
        public const byte TYPE_RADIOCHANNEL = 0x12;
        public const byte TYPE_RADIOCONTENT = 0x13;
        public const byte TYPE_PARK = 0x14;

        public const byte TYPE_CD_REGIONCITIES = 0xDF;
        public const byte TYPE_CD_TEAMS = 0xDE;
        public const byte TYPE_CD_HIGHWAYKIND = 0xD9;
        public const byte TYPE_CD_HIGHWAYINSTANCE = 0xD8;

        private const uint CURRENT_CITY_INSTANCE = TYPE_CD_REGIONCITIES & 0xFF;
        public static readonly InstanceID CityInstance = new InstanceID
        {
            RawData = CURRENT_CITY_INSTANCE
        };
    }
}
