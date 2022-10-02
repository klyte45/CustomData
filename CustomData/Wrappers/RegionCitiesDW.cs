using ColossalFramework;
using ColossalFramework.Globalization;
using CustomData.Utils;
using CustomData.Xml;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace CustomData.Wrappers
{
    public class RegionCitiesDW : CSDataWrapperBase
    {
        internal event Action OnAzimuthChanged;
        protected override InstanceType RequiredType => (InstanceType)InstanceIdUtils.TYPE_CD_REGIONCITIES;
        protected override bool ExclusiveToIndex => false;
        protected override bool AnyButIndex => true;
        protected override int RefIndex { get; } = 0;
        public RegionCitiesDW(InstanceDataExtensionXml xml) : base(xml)
        {
        }
        public float Azimuth
        {
            get => xml.generalFloatValue ?? 0; set
            {
                xml.generalFloatValue = ((value % 360) + 360) % 360;
                OnAzimuthChanged?.Invoke();
                SegmentUtils.UpdateSegmentNamesView();
            }
        }
        public string Name
        {
            get => xml.givenStringId; set
            {
                xml.givenStringId = value;
                m_safeName = null;
                SegmentUtils.UpdateSegmentNamesView();
            }
        }
        public Color CityColor
        {
            get => xml.color ?? GetColorForNumber(xml.Id.Index); set
            {
                xml.color = value;
                m_cachedColorTextureDirty = true;
            }
        }
        public int Seed
        {
            get
            {
                if (xml.flags is null)
                {
                    return (int)(xml.flags = (ulong)UnityEngine.Random.Range(int.MinValue, int.MaxValue));
                }
                return (int)(xml.flags.Value);
            }

            set
            {
                m_safeName = null;
                xml.flags = (ulong)value;
                SegmentUtils.UpdateSegmentNamesView();
            }
        }



        public Texture2D CachedColorTexture
        {
            get
            {
                UpdateCachedColorTexture();
                return m_cachedColorTexture;
            }
        }

        public string SafeName
        {
            get
            {
                UpdateSafeName();
                return m_safeName;
            }
        }
        private string m_safeName;

        private static readonly string[] m_patternsAvailable = new[]
        {
            "America",
            "Europe",
            "Mideast",
            "Spanish"
        };

        private void UpdateSafeName()
        {
            if (m_safeName is null)
            {
                if (xml.givenStringId.IsNullOrWhiteSpace())
                {
                    var nameGen = CDStorage.Instance.GetCommonCityConfig().NameGenerator;
                    UnityEngine.Random.InitState(Seed);
                    if (nameGen is null || !CDController.LoadedGeneralNames.TryGetValue(nameGen, out var namelist))
                    {
                        var targetPattern = m_patternsAvailable[UnityEngine.Random.Range(0, m_patternsAvailable.Length)];
                        var lengthPattern = (int)Locale.Count("CONNECTIONS_PATTERN", targetPattern);
                        var lengthName = (int)Locale.Count("CONNECTIONS_NAME", targetPattern);
                        m_safeName = string.Format(Locale.Get("CONNECTIONS_PATTERN", targetPattern, UnityEngine.Random.Range(0, lengthPattern)), Locale.Get("CONNECTIONS_NAME", targetPattern, UnityEngine.Random.Range(0, lengthName)));
                    }
                    else
                    {
                        m_safeName = namelist[UnityEngine.Random.Range(0, namelist.Length - 1)];
                    }
                }
                else
                {
                    m_safeName = xml.givenStringId;
                }
            }
        }


        private Texture2D m_cachedColorTexture;
        private bool m_cachedColorTextureDirty;

        private void UpdateCachedColorTexture()
        {
            if (m_cachedColorTexture is null)
            {
                m_cachedColorTexture = new Texture2D(1, 1);
                m_cachedColorTextureDirty = true;
            }
            if (m_cachedColorTextureDirty)
            {
                m_cachedColorTexture.SetPixels(new Color[] { CityColor });
                m_cachedColorTexture.Apply();
                m_cachedColorTextureDirty = false;
            }
        }

        private Color GetColorForNumber(uint num) =>
         num < 0
         ? Color.gray
         : Color.Lerp(m_colorOrder[(int)num % 11], Color.black, 0.1f + (num % 33 / 10 / 7.5f));

        private readonly List<Color> m_colorOrder = new List<Color>()
        {
            Color.red,
            Color.Lerp(Color.red,Color.yellow,0.5f),
            Color.yellow,
            Color.green,
            Color.cyan,
            Color.Lerp(Color.blue,Color.cyan,0.5f),
            Color.blue,
            Color.Lerp(Color.blue,Color.magenta,0.5f),
            Color.magenta,
            Color.Lerp(Color.red,Color.magenta,0.5f),
            Color.white,
        };

        ~RegionCitiesDW()
        {
            GameObject.Destroy(m_cachedColorTexture);
        }
    }
}
