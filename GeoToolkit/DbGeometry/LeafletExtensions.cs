//using System.Collections.Generic;
//using System.Data.Entity.Spatial;
//using Basemap.Spatial.Data.Leaflet;
//using Basemap.Spatial.Data.Projection;
//using DotSpatial.Projections;
//using GeoAPI.Geometries;
//using GeoToolkit.Projection;
//using NetTopologySuite.Geometries;
//using NetTopologySuite.IO;

//namespace Basemap.Spatial.Extensions
//{
//    public static class LeafletExtensions
//    {
//        public static DbGeometry ToRectangleDbGeometry(this LeafletBounds leafletBounds, ProjectionInfo pEnd)
//        {
//            var wktWriter = new WKTWriter();
//            var coordinates = new List<Coordinate>();
//            var p1 = ProjectLeafletPoint(leafletBounds._southWest);
//            var p2 = ProjectLeafletPoint(leafletBounds._northEast);

//            coordinates.Add(new Coordinate(p1.X, p1.Y));
//            coordinates.Add(new Coordinate(p2.X, p1.Y));
//            coordinates.Add(new Coordinate(p2.X, p2.Y));
//            coordinates.Add(new Coordinate(p1.X, p2.Y));


//            coordinates.Add(new Coordinate(p1.X, p1.Y));

//            var shell = new LinearRing(coordinates.ToArray());
//            IGeometry geometry = new Polygon(shell);
//            return DbGeometry.FromText(wktWriter.Write(geometry));
//        }

//        public static DbGeometry ToDbGeometry(this LeafletPoint leafletPoint, ProjectionInfo pEnd)
//        {
//            var coordinate = ProjectLeafletPoint(leafletPoint);
//            var wktWriter = new WKTWriter();
//            var point = new Point(coordinate.X, coordinate.Y);
//            var wellKnownText = wktWriter.Write(point);
//            return DbGeometry.FromText(wellKnownText);
//        }

//        private static Coordinate ProjectLeafletPoint(LeafletPoint leafletPoint)
//        {
//            var xy = new double[2];
//            xy[1] = leafletPoint.lat;
//            xy[0] = leafletPoint.lng;
//            //Defines the starting coordiante system
//            Reproject.ReprojectPoints(xy, null, Definitions.WorldProjection, Definitions.BritishProjection, 0, 1);

//            return new Coordinate(xy[0], xy[1]);
//        }
//    }
//}

