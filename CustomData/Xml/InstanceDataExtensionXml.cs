using Kwytto.Interfaces;
using Kwytto.Utils;
using System;
using System.Xml.Serialization;
using UnityEngine;

namespace CustomData.Xml
{
    [XmlRoot("InstanceEntry")]
    public class InstanceDataExtensionXml : IIdentifiable
    {
        [XmlIgnore]
        public InstanceID Id { get; set; }
        [XmlAttribute("InstanceId")]
        public string IdHex
        {
            get
            {
                return Id.RawData.ToString("X8");
            }
            set
            {
                Id = new InstanceID
                {
                    RawData = Convert.ToUInt32(value, 16)
                };
            }
        }

        long? IIdentifiable.Id
        {
            get => Id.RawData; set => Id = new InstanceID()
            {
                RawData = value is null ? 0 : (uint)value
            };
        }
        private ulong? m_randomSeed;
        [XmlAttribute("seedCD")]
        public ulong RandomSeed
        {
            get => m_randomSeed ?? (m_randomSeed = BitConverter.ToUInt64(Guid.NewGuid().ToByteArray(), 0)) ?? 0L;
            set => m_randomSeed = value;
        }
        [XmlAttribute("generatedId")]
        public string generatedId;
        [XmlAttribute("mainReference")]
        public string mainReference;
        [XmlAttribute("qualifiedReference")]
        public string qualifiedReference;
        [XmlAttribute("shortReference")]
        public string shortReference;
        [XmlAttribute("flags")]
        public ulong flags;
        [XmlAttribute("generalFloatValue")]
        public float generalFloatValue;
        [XmlAttribute("sourceEnumeratorReceivedId")]
        public uint sourceEnumeratorReceivedId;
        [XmlAttribute("sourceEnumeratorNextId")]
        public uint sourceEnumeratorNextId;
        [XmlElement("color")]
        public Color color;
        [XmlIgnore]
        public Texture2D icon;
        [XmlElement("icon")]
        public string IconBase64
        {
            get => icon?.ToBase64();
            set => icon = value is null ? null : TextureUtils.Base64ToTexture2D(value);
        }
    }
}
