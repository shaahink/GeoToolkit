//using System.IO;
//using System.Linq;
//using NetTopologySuite.IO;
//using NetTopologySuite.IO.Converters;
//using Newtonsoft.Json;

//namespace GeoToolkit.GeoJson
//{
//    public class BasemapGeoJsonReader
//    {
//        /// <summary>
//        ///     Reads the specified json.
//        /// </summary>
//        /// <typeparam name="TObject">The type of the object.</typeparam>
//        /// <param name="json">The json.</param>
//        /// <returns></returns>
//        public TObject Read<TObject>(string json)
//            where TObject : class
//        {
//            JsonSerializer geoJsonSerializer = new GeoJsonSerializer();
//            geoJsonSerializer.Converters.Remove(
//                geoJsonSerializer.Converters.FirstOrDefault(jsonConverter => jsonConverter is FeatureConverter));
//            geoJsonSerializer.Converters.Remove(
//                geoJsonSerializer.Converters.FirstOrDefault(converter => converter is FeatureCollectionConverter));

//            geoJsonSerializer.Converters.Insert(0, new TempFeatureCollectionConverter());
//            geoJsonSerializer.Converters.Insert(0, new TempFeatureConverter());

//            using (var stringReader = new StringReader(json))
//            {
//                return geoJsonSerializer.Deserialize<TObject>(new JsonTextReader(stringReader));
//            }
//        }
//    }
//}

