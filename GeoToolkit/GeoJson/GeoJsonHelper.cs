using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using DotSpatial.Data;
using DotSpatial.Projections;
using GeoAPI.Geometries;
using GeoToolkit.DbGeometry;
using GeoToolkit.Projection;
using NetTopologySuite.Features;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;
using Feature = NetTopologySuite.Features.Feature;
using GeometryConverter = DotSpatial.Topology.GeometryConverter;

namespace GeoToolkit.GeoJson
{
    public class GeoJsonHelper
    {
        /// <summary>
        ///     convert geoJson feature or featureCollection to Geometry
        /// </summary>
        /// <param name="geoJson"></param>
        /// <returns></returns>
        public static System.Data.Entity.Spatial.DbGeometry ToGeometry(string geoJson)
        {
            var featureCollection = FeatureCollection(geoJson);
            var geometries = featureCollection.Features.Select(feature => feature.Geometry).ToList();

            var wktWriter = new WKTWriter();
            var geoemetryColleciton = new GeometryCollection(geometries.ToArray());

            var wkt = wktWriter.Write(geoemetryColleciton);
            var geometry = System.Data.Entity.Spatial.DbGeometry.FromText(wkt);
            return geometry;
        }

        /// <summary>
        ///     geoJson should be feature or featureCollection. convert and project geoJson to DbGeometry
        /// </summary>
        /// <param name="geoJson"></param>
        /// <param name="pStart"></param>
        /// <param name="pEnd"></param>
        /// <returns></returns>
        public static System.Data.Entity.Spatial.DbGeometry ToGeometry(string geoJson, ProjectionInfo pStart,
            ProjectionInfo pEnd)
        {
            var featureCollection = FeatureCollection(geoJson);


            var geometries =
                Project(featureCollection.Features.Select(feature => feature.Geometry).ToList(), pStart, pEnd);

            var wktWriter = new WKTWriter();
            var geoemetryColleciton = new GeometryCollection(geometries.ToArray());

            var wkt = wktWriter.Write(geoemetryColleciton);
            var geometry = System.Data.Entity.Spatial.DbGeometry.FromText(wkt);
            return geometry;
        }

        private static FeatureCollection FeatureCollection(string geoJson)
        {
            var reader = new GeoJsonReader();

            var featureCollection = new FeatureCollection();

            try
            {
                featureCollection =
                    reader.Read<FeatureCollection>(geoJson);
                if (featureCollection.Count == 0)
                {
                    var feature =
                        reader.Read<Feature>(geoJson);
                    featureCollection.Add(feature);
                }
            }
            catch (Exception)
            {
                var feature =
                    reader.Read<Feature>(geoJson);
                featureCollection.Add(feature);
            }
            return featureCollection;
        }


        public static List<IGeometry> Project(List<IGeometry> toList, ProjectionInfo pStart, ProjectionInfo pEnd)
        {
            var geometryCollection = new GeometryCollection(toList.ToArray());
            var collection = geometryCollection.ToDotSpatial();
            var featureSet = new FeatureSet();
            foreach (var geo in collection.Geometries)
            {
                featureSet.Features.Add(geo);
            }
            featureSet.Projection = pStart;
            featureSet.Reproject(pEnd);
            var dotSpatialProjectedGeos =
                featureSet.Features.Select(x => x.BasicGeometry as DotSpatial.Topology.IGeometry).ToList();
            var result =
                dotSpatialProjectedGeos.Select(
                    dotSpatialProjectedGeo => GeometryConverter.ToGeoAPI((dotSpatialProjectedGeo)))
                    .ToList();
            return result;
        }

        public static IGeometry Project(IGeometry geometry, ProjectionInfo pStart, ProjectionInfo pEnd)
        {
            var featureSet = new FeatureSet();
            featureSet.AddFeature(geometry.ToDotSpatial());
            featureSet.Projection = pStart;
            featureSet.Reproject(pEnd);
            return
                GeometryConverter.ToGeoAPI(
                    ((featureSet.Features[0].BasicGeometry as DotSpatial.Topology.IGeometry)));
        }

        public static string ToGeoJson(System.Data.Entity.Spatial.DbGeometry location)
        {
            var wktReader = new WKTReader();
            var geometry = wktReader.Read(location.WellKnownValue.WellKnownText);
            var geoJsonWriter = new GeoJsonWriter();
            return geoJsonWriter.Write(geometry);
        }

        public static string ToGeoJson(System.Data.Entity.Spatial.DbGeometry location, ProjectionInfo pStart,
            ProjectionInfo pEnd)
        {
            var wktReader = new WKTReader();
            var geometry = wktReader.Read(location.WellKnownValue.WellKnownText);
            geometry = Project(geometry, pStart, pEnd);
            var geoJsonWriter = new GeoJsonWriter();
            return geoJsonWriter.Write(geometry);
        }

        public static string ToGeoJson(IEnumerable<System.Data.Entity.Spatial.DbGeometry> dbGeometrys,
            ProjectionInfo pStart)
        {
            var pEnd = Definitions.WorldProjection;
            var enumerable = dbGeometrys as IList<System.Data.Entity.Spatial.DbGeometry> ?? dbGeometrys.ToList();
            var reader = new WKTReader();
            var geometryCollection =
                new GeometryCollection(
                    enumerable.Select(x => GeometryHelper.Project(x.MakeValid(), pStart, pEnd))
                        .Select(dbGeometry => reader.Read(dbGeometry.WellKnownValue.WellKnownText))
                        .ToArray());
            var geoJsonWriter = new GeoJsonWriter();
            return geoJsonWriter.Write(geometryCollection);
        }

        public static string ToGeoJson(IEnumerable<System.Data.Entity.Spatial.DbGeometry> dbGeometrys,
            ProjectionInfo pStart, DataTable dataTable)
        {
            var pEnd = Definitions.WorldProjection;
            var dbGeometrys1 = dbGeometrys as IList<System.Data.Entity.Spatial.DbGeometry> ?? dbGeometrys.ToList();
            var reader = new WKTReader();
            var featureCollection = new FeatureCollection();
            var columns = (from DataColumn column in dataTable.Columns select column.ColumnName).ToList();
            for (var i = 0; i < dbGeometrys1.Count(); i++)
            {
                var geometry = GeometryHelper.Project(dbGeometrys1[i].MakeValid(), pStart, pEnd);
                var read = reader.Read(geometry.WellKnownValue.WellKnownText);
                var table = new AttributesTable();
                foreach (var column in columns)
                {
                    table.AddAttribute(column, dataTable.Rows[i][column]);
                }
                featureCollection.Add(new Feature(read, table));
            }
            var geoJsonWriter = new GeoJsonWriter();
            return geoJsonWriter.Write(featureCollection);
        }
    }
}