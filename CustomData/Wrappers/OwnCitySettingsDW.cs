using CustomData.Enums;
using CustomData.Utils;
using CustomData.Xml;
using Kwytto.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using static CustomData.Utils.SegmentUtils;
using static CustomData.Xml.InstanceDataExtensionXml;

namespace CustomData.Wrappers
{
    public class OwnCitySettingsDW : CSDataWrapperBase
    {
        protected override InstanceType RequiredType => (InstanceType)InstanceIdUtils.TYPE_CD_REGIONCITIES;
        protected override bool ExclusiveToIndex => true;
        protected override bool AnyButIndex => false;
        protected override int RefIndex { get; } = 0xFFFFFF;
        public OwnCitySettingsDW(InstanceDataExtensionXml xml) : base(xml)
        {
        }
        public ReferenceData GetDistrictGeneratorFile(DistrictAreaType type) => xml.SafeGetReference((long)type) ?? (xml.references[(long)type] = new ReferenceData());
        public int PostalCodeDigits { get => (xml.genericId ?? 0) % 1000; set => xml.genericId = value % 1000; }
        public string PostalCodeFormat
        {
            get => xml.givenStringId ?? "LMNOP-BCE"; set
            {
                xml.givenStringId = value;
                m_cachedPostalCodeTokens = null;
            }
        }
        public string AddressLine1 { get => xml.SafeGetReference(long.MaxValue).mainReference ?? "B A( - F)"; set => xml.SafeGetReference(0).mainReference = value; }
        public string AddressLine2 { get => xml.SafeGetReference(long.MaxValue).qualifiedReference ?? "[D - ]C"; set => xml.SafeGetReference(0).qualifiedReference = value; }
        public string AddressLine3 { get => xml.SafeGetReference(long.MaxValue).shortReference ?? "E"; set => xml.SafeGetReference(0).shortReference = value; }


        #region Addresses

        public void GetAddressLines(Vector3 sidewalk, Vector3 midPosBuilding, out string[] addressLines)
        {
            addressLines = null;
            string line1 = AddressLine1;
            string line2 = AddressLine2;
            string line3 = AddressLine3;
            string format = (line1 + "≠" + line2 + "≠" + line3);

            Regex regexEscape = new Regex(@"\\([ABCDEF[\]\\()])", RegexOptions.Compiled);
            foreach (Match escapeMatch in regexEscape.Matches(format))
            {
                format = format.Replace($"\\{escapeMatch.Groups[1].Value}", char.ConvertFromUtf32(0xFF00 + escapeMatch.Groups[1].Value[0]));
            }


            string a = "";//street
            int b = 0;//number
            string c = SimulationManager.instance.m_metaData.m_CityName;//city
            string d = "";//district
            string e = "";//zipcode
            string f = "";//area name

            if (format.ToCharArray().Intersect("AB".ToCharArray()).Count() > 0)
            {
                if (!GetStreetAndNumber(sidewalk, midPosBuilding, out a, out b))
                {
                    return;
                }
            }

            if (format.ToCharArray().Intersect("D[]".ToCharArray()).Count() > 0)
            {
                int districtId = DistrictManager.instance.GetDistrict(midPosBuilding);
                if (districtId > 0)
                {
                    d = DistrictManager.instance.GetDistrictName(districtId);
                    format = Regex.Replace(format, @"\]|\[", "", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                }
                else
                {
                    format = Regex.Replace(format, @"\[[^]]*\]", "", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                }

            }

            if (format.Contains("E"))
            {
                e = TokenToPostalCode(sidewalk);
            }
            if (format.Intersect("F()".ToCharArray()).Count() > 0)
            {
                int parkId = DistrictManager.instance.GetPark(midPosBuilding);
                if (parkId > 0)
                {
                    f = DistrictManager.instance.GetParkName(parkId);
                    format = Regex.Replace(format, @"\)|\(", "", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                }
                else
                {
                    format = Regex.Replace(format, @"\([^\)]*\)", "", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                }
            }
            ParseToFormatableString(ref format, 6);

            addressLines = new string(string.Format(format, a, b, c, d, e, f).Select(x => x > 0xff00 && x < 0xffff ? (char)(x - 0xFF00) : x).ToArray()).Split("≠".ToCharArray());
        }

        internal static bool GetStreetAndNumber(Vector3 sidewalk, Vector3 midPosBuilding, out string streetName, out int number)
        {
            SegmentUtils.GetNearestSegment(sidewalk, out Vector3 targetPosition, out float targetLength, out ushort targetSegmentId);
            if (targetSegmentId == 0)
            {
                streetName = string.Empty;
                number = 0;
                return false;
            }
            var seed = NetManager.instance.m_segments.m_buffer[targetSegmentId].m_nameSeed;
            var startSrc = MileageStartSource.DEFAULT;
            var offsetMeters = 0;
            //if (AdrNameSeedDataXml.Instance.NameSeedConfigs.TryGetValue(seed, out AdrNameSeedConfig seedConf))
            //{
            //    startSrc = seedConf.MileageStartSrc;
            //    offsetMeters = (int)seedConf.MileageOffset;
            //}

            return SegmentUtils.GetAddressStreetAndNumber(targetPosition, targetSegmentId, targetLength, midPosBuilding, startSrc, offsetMeters, out number, out streetName);
        }

        private static void ParseToFormatableString(ref string input, byte count)
        {
            for (int i = 0; i < count; i++)
            {
                var targetChar = ((char)('A' + i)).ToString();
                input = input.Replace($"\\{targetChar}", "☆").Replace(targetChar, $"{{{i}}}").Replace("☆", targetChar);
            }
        }
        #endregion




        #region Postal Code

        private PostalCodeTokenContainer[] m_cachedPostalCodeTokens;

        public string TokenToPostalCode(Vector3 position)
        {
            string result = "";

            string cachedSeedName = null;
            string cachedSegmentId = null;
            string cachedCityPre = null;
            string cachedDistrictPre = null;
            string cachedCx = null;
            string cachedCy = null;

            string seedName()
            {
                if (cachedSeedName == null)
                {
                    SegmentUtils.GetNearestSegment(position, out _, out _, out ushort targetSegmentId);
                    cachedSeedName = NetManager.instance.m_segments.m_buffer[targetSegmentId].m_nameSeed.ToString("00000");
                }
                return cachedSeedName;
            }
            string segmentId()
            {
                if (cachedSegmentId == null)
                {
                    SegmentUtils.GetNearestSegment(position, out _, out _, out ushort targetSegmentId);
                    cachedSegmentId = targetSegmentId.ToString("00000");
                }
                return cachedSegmentId;
            }
            string cityPre() => cachedCityPre ?? (cachedCityPre = PostalCodeDigits.ToString("000"));
            string distPre()
            {
                if (cachedDistrictPre == null)
                {
                    int district = DistrictManager.instance.GetDistrict(position) & 0xff;
                    cachedDistrictPre = (CDStorage.Instance.GetDistrictData((byte)district).DigitsPostalCode).Value.ToString("000");
                }
                return cachedDistrictPre;
            }

            string cx()
            {
                if (cachedCx == null)
                {
                    Vector2 tilePos = MapUtils.GetMapTile(position);
                    cachedCx = Math.Floor(tilePos.x * 10).ToString("00");
                    cachedCy = Math.Floor(tilePos.y * 10).ToString("00");
                }
                return cachedCx;
            }
            string cy()
            {
                if (cachedCy == null)
                {
                    Vector2 tilePos = MapUtils.GetMapTile(position);
                    cachedCx = Math.Floor(tilePos.x * 10).ToString("00");
                    cachedCy = Math.Floor(tilePos.y * 10).ToString("00");
                }
                return cachedCy;
            }

            if (m_cachedPostalCodeTokens is null)
            {
                m_cachedPostalCodeTokens = PostalCodeTokenContainer.TokenizeAsPostalCode(PostalCodeFormat);
            }
            foreach (var token in m_cachedPostalCodeTokens)
            {
                switch (token.token)
                {
                    case PostalCodeToken.NONE: result += token.fixedText; break;
                    case PostalCodeToken.NAMESEED_D5: result += seedName()[0]; break;
                    case PostalCodeToken.NAMESEED_D4: result += seedName()[1]; break;
                    case PostalCodeToken.NAMESEED_D3: result += seedName()[2]; break;
                    case PostalCodeToken.NAMESEED_D2: result += seedName()[3]; break;
                    case PostalCodeToken.NAMESEED_D1: result += seedName()[4]; break;
                    case PostalCodeToken.SEGMENT_D5: result += segmentId()[0]; break;
                    case PostalCodeToken.SEGMENT_D4: result += segmentId()[1]; break;
                    case PostalCodeToken.SEGMENT_D3: result += segmentId()[2]; break;
                    case PostalCodeToken.SEGMENT_D2: result += segmentId()[3]; break;
                    case PostalCodeToken.SEGMENT_D1: result += segmentId()[4]; break;
                    case PostalCodeToken.CITY_PRE_D3: result += cityPre()[0]; break;
                    case PostalCodeToken.CITY_PRE_D2: result += cityPre()[1]; break;
                    case PostalCodeToken.CITY_PRE_D1: result += cityPre()[2]; break;
                    case PostalCodeToken.DISTRICT_PRE_D3: result += distPre()[0]; break;
                    case PostalCodeToken.DISTRICT_PRE_D2: result += distPre()[1]; break;
                    case PostalCodeToken.DISTRICT_PRE_D1: result += distPre()[2]; break;
                    case PostalCodeToken.CX_I: result += cx()[0]; break;
                    case PostalCodeToken.CX_D: result += cx()[1]; break;
                    case PostalCodeToken.CY_I: result += cy()[0]; break;
                    case PostalCodeToken.CY_D: result += cy()[1]; break;
                }
            }

            return result;
        }
        public string AsPostalCodeString()
        {
            if (m_cachedPostalCodeTokens is null)
            {
                m_cachedPostalCodeTokens = PostalCodeTokenContainer.TokenizeAsPostalCode(PostalCodeFormat);
            }
            var result = "";
            foreach (var token in m_cachedPostalCodeTokens)
            {
                if (token.token == PostalCodeToken.NONE)
                {
                    if ("ABCDEFGHIJKLMNOPXxYy\"\\".ToCharArray().Intersect(token.fixedText.ToCharArray()).Any())
                    {
                        result += token.fixedText.Length == 1 ? $"\\{token.fixedText}" : $"\"{token.fixedText.Replace("\\", "\\\\").Replace("\"", "\\\"")}\"";
                    }
                    else
                    {
                        result += token.fixedText;
                    }

                }
                else
                {
                    result += token.AsString();
                }
            }
            return result;
        }
        private class PostalCodeTokenContainer
        {
            public PostalCodeToken token = PostalCodeToken.NONE;
            public string fixedText;

            public string AsString()
            {
                switch (token)
                {
                    default:
                    case PostalCodeToken.NONE: return "";
                    case PostalCodeToken.NAMESEED_D5: return "A";
                    case PostalCodeToken.NAMESEED_D4: return "B";
                    case PostalCodeToken.NAMESEED_D3: return "C";
                    case PostalCodeToken.NAMESEED_D2: return "D";
                    case PostalCodeToken.NAMESEED_D1: return "E";
                    case PostalCodeToken.SEGMENT_D5: return "F";
                    case PostalCodeToken.SEGMENT_D4: return "G";
                    case PostalCodeToken.SEGMENT_D3: return "H";
                    case PostalCodeToken.SEGMENT_D2: return "I";
                    case PostalCodeToken.SEGMENT_D1: return "J";
                    case PostalCodeToken.CITY_PRE_D3: return "K";
                    case PostalCodeToken.CITY_PRE_D2: return "L";
                    case PostalCodeToken.CITY_PRE_D1: return "M";
                    case PostalCodeToken.DISTRICT_PRE_D3: return "N";
                    case PostalCodeToken.DISTRICT_PRE_D2: return "O";
                    case PostalCodeToken.DISTRICT_PRE_D1: return "P";
                    case PostalCodeToken.CX_I: return "X";
                    case PostalCodeToken.CX_D: return "x";
                    case PostalCodeToken.CY_I: return "Y";
                    case PostalCodeToken.CY_D: return "y";
                }
            }

            public static PostalCodeTokenContainer[] TokenizeAsPostalCode(string format)
            {
                bool intoQuote = false;
                bool escapeNext = false;
                string currentQuoteData = null;
                List<PostalCodeTokenContainer> result = new List<PostalCodeTokenContainer>();
                foreach (var letter in format)
                {
                    if (escapeNext)
                    {
                        escapeNext = false;
                        if (result[result.Count - 1].token == PostalCodeToken.NONE)
                        {
                            result[result.Count - 1].fixedText += letter;
                        }
                        else
                        {
                            result.Add(new PostalCodeTokenContainer
                            {
                                fixedText = letter.ToString()
                            });
                        }
                        continue;
                    }
                    if (intoQuote)
                    {
                        if (letter == '\\')
                        {
                            escapeNext = true;
                            continue;
                        }
                        if (letter == '"')
                        {
                            intoQuote = false;
                            result.Add(new PostalCodeTokenContainer
                            {
                                fixedText = currentQuoteData,
                                token = PostalCodeToken.NONE
                            });
                            currentQuoteData = null;
                            continue;
                        }
                        currentQuoteData += letter;
                        continue;
                    }
                    switch (letter)
                    {
                        case '"':
                            currentQuoteData = "";
                            intoQuote = true;
                            continue;
                        case '\\':
                            escapeNext = true;
                            continue;
                        case 'A': result.Add(new PostalCodeTokenContainer { token = PostalCodeToken.NAMESEED_D5 }); break;
                        case 'B': result.Add(new PostalCodeTokenContainer { token = PostalCodeToken.NAMESEED_D4 }); break;
                        case 'C': result.Add(new PostalCodeTokenContainer { token = PostalCodeToken.NAMESEED_D3 }); break;
                        case 'D': result.Add(new PostalCodeTokenContainer { token = PostalCodeToken.NAMESEED_D2 }); break;
                        case 'E': result.Add(new PostalCodeTokenContainer { token = PostalCodeToken.NAMESEED_D1 }); break;
                        case 'F': result.Add(new PostalCodeTokenContainer { token = PostalCodeToken.SEGMENT_D5 }); break;
                        case 'G': result.Add(new PostalCodeTokenContainer { token = PostalCodeToken.SEGMENT_D4 }); break;
                        case 'H': result.Add(new PostalCodeTokenContainer { token = PostalCodeToken.SEGMENT_D3 }); break;
                        case 'I': result.Add(new PostalCodeTokenContainer { token = PostalCodeToken.SEGMENT_D2 }); break;
                        case 'J': result.Add(new PostalCodeTokenContainer { token = PostalCodeToken.SEGMENT_D1 }); break;
                        case 'K': result.Add(new PostalCodeTokenContainer { token = PostalCodeToken.CITY_PRE_D3 }); break;
                        case 'L': result.Add(new PostalCodeTokenContainer { token = PostalCodeToken.CITY_PRE_D2 }); break;
                        case 'M': result.Add(new PostalCodeTokenContainer { token = PostalCodeToken.CITY_PRE_D1 }); break;
                        case 'N': result.Add(new PostalCodeTokenContainer { token = PostalCodeToken.DISTRICT_PRE_D3 }); break;
                        case 'O': result.Add(new PostalCodeTokenContainer { token = PostalCodeToken.DISTRICT_PRE_D2 }); break;
                        case 'P': result.Add(new PostalCodeTokenContainer { token = PostalCodeToken.DISTRICT_PRE_D1 }); break;
                        case 'X': result.Add(new PostalCodeTokenContainer { token = PostalCodeToken.CX_I }); break;
                        case 'x': result.Add(new PostalCodeTokenContainer { token = PostalCodeToken.CX_D }); break;
                        case 'Y': result.Add(new PostalCodeTokenContainer { token = PostalCodeToken.CY_I }); break;
                        case 'y': result.Add(new PostalCodeTokenContainer { token = PostalCodeToken.CY_D }); break;
                        default:
                            if (result[result.Count - 1].token == PostalCodeToken.NONE)
                            {
                                result[result.Count - 1].fixedText += letter;
                            }
                            else
                            {
                                result.Add(new PostalCodeTokenContainer
                                {
                                    fixedText = letter.ToString()
                                });
                            }
                            break;
                    }
                }
                return result.ToArray();
            }
        }
        private enum PostalCodeToken
        {
            NONE,
            NAMESEED_D5,
            NAMESEED_D4,
            NAMESEED_D3,
            NAMESEED_D2,
            NAMESEED_D1,
            SEGMENT_D5,
            SEGMENT_D4,
            SEGMENT_D3,
            SEGMENT_D2,
            SEGMENT_D1,
            CITY_PRE_D3,
            CITY_PRE_D2,
            CITY_PRE_D1,
            DISTRICT_PRE_D3,
            DISTRICT_PRE_D2,
            DISTRICT_PRE_D1,
            CX_I,
            CX_D,
            CY_I,
            CY_D,
        }
        #endregion
    }

}

