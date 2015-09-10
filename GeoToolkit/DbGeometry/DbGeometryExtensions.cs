using System.Data.SqlTypes;
using DotSpatial.Projections;
using GeoToolkit.GeoJson;
using GeoToolkit.Projection;
using Microsoft.SqlServer.Types;

namespace GeoToolkit.DbGeometry
{
    public static class DbGeometryExtensions
    {
        public static string ToGeoJson(this System.Data.Entity.Spatial.DbGeometry geometry)
        {
            return GeoJsonHelper.ToGeoJson(geometry);
        }

        /// <summary>
        ///     This will convert geometry and project it from sourceEpsg to EPSG 4326
        /// </summary>
        /// <param name="geometry"></param>
        /// <param name="pStart"></param>
        /// <returns></returns>
        public static string ToGeoJson(this System.Data.Entity.Spatial.DbGeometry geometry, ProjectionInfo pStart)
        {
            geometry = geometry.MakeValid();
            return GeoJsonHelper.ToGeoJson(geometry, pStart,
                Definitions.WorldProjection);
        }

        public static System.Data.Entity.Spatial.DbGeometry MakeValid(
            this System.Data.Entity.Spatial.DbGeometry geometry)
        {
            var coordinateSystemId = 0;

            return
                System.Data.Entity.Spatial.DbGeometry.FromText(
                    SqlGeometry.STGeomFromText(new SqlChars(geometry.AsText()), coordinateSystemId)
                        .MakeValid()
                        .STAsText()
                        .ToSqlString()
                        .ToString(), coordinateSystemId);
        }

        public static SqlGeometry ToSqlGeometry(this System.Data.Entity.Spatial.DbGeometry dbGeometry)
        {
            return SqlGeometry.STGeomFromWKB(new SqlBytes(dbGeometry.AsBinary()), 0);
        }

        public static System.Data.Entity.Spatial.DbGeometry Reverse(
            this System.Data.Entity.Spatial.DbGeometry linestring)
        {
            var fromWkb = SqlGeometry.STGeomFromWKB(new SqlBytes(linestring.AsBinary()), 0);

            // Create a new Geometry Builder  
            var gb = new SqlGeometryBuilder();
            // Set the Spatial Reference ID equal to the supplied linestring  
            gb.SetSrid((int) (fromWkb.STSrid));
            // Start the linestring  
            gb.BeginGeometry(OpenGisGeometryType.LineString);
            // Add the first point using BeginFigure()  
            gb.BeginFigure((double) fromWkb.STEndPoint().STX, (double) fromWkb.STEndPoint().STY);
            // Loop through remaining points in reverse order  
            for (var x = (int) fromWkb.STNumPoints() - 1; x > 0; --x)
            {
                gb.AddLine((double) fromWkb.STPointN(x).STX, (double) fromWkb.STPointN(x).STY);
            }
            // End the figure  
            gb.EndFigure();
            // End the geometry  
            gb.EndGeometry();
            // Return that as a SqlGeometry instance  
            return System.Data.Entity.Spatial.DbGeometry.FromBinary(gb.ConstructedGeometry.STAsBinary().Buffer);
        }
    }
}