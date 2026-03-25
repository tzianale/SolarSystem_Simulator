using System.Collections.Generic;
using UnityEngine;


namespace Models
{
    /// <summary>
    /// Represents Keplerian orbital elements and rates for a planet.
    /// </summary>
    public class KeplerPlanetData
    {
        public float[] Elements { get; set; }
        public float[] Rates { get; set; }
        public float[] ExtraTerms { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="KeplerPlanetData"/> class.
        /// </summary>
        /// <param name="elements">The orbital elements of the planet.</param>
        /// <param name="rates">The rates of change of the orbital elements.</param>
        /// <param name="extraTerms">Additional terms for more accurate calculations (optional).</param>
        public KeplerPlanetData(float[] elements, float[] rates, float[] extraTerms = null)
        {
            Elements = elements;
            Rates = rates;
            ExtraTerms = extraTerms ?? new float[0];
        }
    }

    /// <summary>
    /// Static class containing a database of planetary data.
    /// </summary>
    public static class PlanetDatabase
    {
        /// <summary>
        /// Dictionary containing the Keplerian data for all planets.
        /// </summary>
        public static readonly Dictionary<string, KeplerPlanetData> Planets = new Dictionary<string, KeplerPlanetData>
    {
        {
            "Mercury", new KeplerPlanetData(
                new float[] { 0.38709843f, 0.20563661f, 7.00559432f, 252.25166724f, 77.45771895f, 48.33961819f },
                new float[] { 0.00000000f, 0.00002123f, -0.00590158f, 149472.67486623f, 0.15940013f, -0.12214182f }
            )
        },
        {
            "Venus", new KeplerPlanetData(
                new float[] { 0.72332102f, 0.00676399f, 3.39777545f, 181.97970850f, 131.76755713f, 76.67261496f },
                new float[] { -0.00000026f, -0.00005107f, 0.00043494f, 58517.81560260f, 0.05679648f, -0.27274174f }
            )
        },
        {
            "Earth", new KeplerPlanetData(
                new float[] { 1.00000018f, 0.01673163f, -0.00054346f, 100.46691572f, 102.93005885f, -5.11260389f },
                new float[] { -0.00000003f, -0.00003661f, -0.01337178f, 35999.37306329f, 0.31795260f, -0.24123856f }
            )
        },
        {
            "Mars", new KeplerPlanetData(
                new float[] { 1.52371243f, 0.09336511f, 1.85181869f, -4.56813164f, -23.91744784f, 49.71320984f },
                new float[] { 0.00000097f, 0.00009149f, -0.00724757f, 19140.29934243f, 0.45223625f, -0.26852431f }
            )
        },
        {
            "Jupiter", new KeplerPlanetData(
                new float[] { 5.20248019f, 0.04853590f, 1.29861416f, 34.33479152f, 14.27495244f, 100.29282654f },
                new float[] { -0.00002864f, 0.00018026f, -0.00322699f, 3034.90371757f, 0.18199196f, 0.13024619f },
                new float[] { -0.00012452f, 0.06064060f, -0.35635438f, 38.35125000f }
            )
        },
        {
            "Saturn", new KeplerPlanetData(
                new float[] { 9.54149883f, 0.05550825f, 2.49424102f, 50.07571329f, 92.86136063f, 113.63998702f },
                new float[] { -0.00003065f, -0.00032044f, 0.00451969f, 1222.11494724f, 0.54179478f, -0.25015002f },
                new float[] { 0.00025899f, -0.13434469f, 0.87320147f, 38.35125000f }
            )
        },
        {
            "Uranus", new KeplerPlanetData(
                new float[] { 19.18797948f, 0.04685740f, 0.77298127f, 314.20276625f, 172.43404441f, 73.96250215f },
                new float[] { -0.00020455f, -0.00001550f, -0.00180155f, 428.49512595f, 0.09266985f, 0.05739699f },
                new float[] { 0.00058331f, -0.97731848f, 0.17689245f, 7.67025000f }
            )
        },
        {
            "Neptune", new KeplerPlanetData(
                new float[] { 30.06952752f, 0.00895439f, 1.77005520f, 304.22289287f, 46.68158724f, 131.78635853f },
                new float[] { 0.00006447f, 0.00000818f, 0.00022400f, 218.46515314f, 0.01009938f, -0.00606302f },
                new float[] { -0.00041348f, 0.68346318f, -0.10162547f, 7.67025000f }
            )
        },
        {
            "Pluto", new KeplerPlanetData(
                new float[] { 39.48686035f, 0.24885238f, 17.14104260f, 238.96535011f, 224.09702598f, 110.30167986f },
                new float[] { 0.00449751f, 0.00006016f, 0.00000501f, 145.18042903f, -0.00968827f, -0.00809981f },
                new float[] { -0.01262724f, 0, 0, 0 }
            )
        }
    };

        /// <summary>
        /// Prints the keys of the planets dictionary to the console.
        /// </summary>
        public static void PrintKeys()
        {
            foreach (var key in Planets.Keys)
            {
                Debug.Log("Planet Key: " + key);
            }
        }
    }

}

