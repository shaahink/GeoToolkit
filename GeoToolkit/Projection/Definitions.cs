using DotSpatial.Projections;

namespace GeoToolkit.Projection
{
    public class Definitions
    {
        private const string British =
            "+proj=tmerc +lat_0=49 +lon_0=-2 +k=0.9996012717 +x_0=400000 +y_0=-100000 +ellps=airy +towgs84=446.448,-125.157,542.060,0.1502,0.2470,0.8421,-20.4894 +units=m +no_defs";

        public static ProjectionInfo WorldProjection = KnownCoordinateSystems.Geographic.World.WGS1984;
        public static ProjectionInfo BritishProjection = ProjectionInfo.FromProj4String(British);
    }
}