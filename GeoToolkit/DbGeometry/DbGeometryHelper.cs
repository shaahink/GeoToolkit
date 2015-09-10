using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using DotSpatial.Data;
using DotSpatial.Projections;
using DotSpatial.Topology;
using GeoAPI.Geometries;
using Microsoft.SqlServer.Types;
using NetTopologySuite.IO;
using GeometryCollection = NetTopologySuite.Geometries.GeometryCollection;
using IGeometry = DotSpatial.Topology.IGeometry;

namespace GeoToolkit.DbGeometry
{
    public class GeometryHelper
    {
        public static System.Data.Entity.Spatial.DbGeometry Buffer(System.Data.Entity.Spatial.DbGeometry location,
            double distance)
        {
            var sqlGeometry = SqlGeometry.STGeomFromWKB(new SqlBytes(location.AsBinary()), 0).STBuffer(distance);
            return System.Data.Entity.Spatial.DbGeometry.FromBinary(sqlGeometry.STAsBinary().Buffer);
        }

        public static System.Data.Entity.Spatial.DbGeometry Intersection(System.Data.Entity.Spatial.DbGeometry source,
            System.Data.Entity.Spatial.DbGeometry target)
        {
            return source.Intersection(target);
        }

        public static System.Data.Entity.Spatial.DbGeometry UnionAggregate(
            IEnumerable<System.Data.Entity.Spatial.DbGeometry> dbGeometrys)
        {
            var wktReader = new WKTReader();
            var geometries = dbGeometrys.Select(x => wktReader.Read(x.WellKnownValue.WellKnownText)).ToArray();
            var collection = new GeometryCollection(geometries);
            var wktWriter = new WKTWriter();
            return System.Data.Entity.Spatial.DbGeometry.FromText(wktWriter.Write(collection));
        }

        public static System.Data.Entity.Spatial.DbGeometry Project(System.Data.Entity.Spatial.DbGeometry source,
            ProjectionInfo pStart, ProjectionInfo pEnd)
        {
            var wkt = source.WellKnownValue.WellKnownText;
            var wktReader = new WKTReader();
            var geometry = wktReader.Read(wkt);
            var featureSet = new FeatureSet();
            featureSet.Features.Add(geometry.ToDotSpatial());
            featureSet.Projection = pStart;
            featureSet.Reproject(pEnd);
            var projected =
                (featureSet.Features.First().BasicGeometry as IGeometry).ToGeoAPI();
            var wktWriter = new WKTWriter();
            var projectedWkt = wktWriter.Write(projected);
            return System.Data.Entity.Spatial.DbGeometry.FromText(projectedWkt);
        }
    }
}