using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Device;
using System.Device.Location;

namespace SkyCommanderProcessor.EXLogic {
  public class GPSPoint {
    public float lat { get; set; }
    public float lng { get; set; }

    public float Y {
      get { return lat; }
    }

    public float X {
      get { return lng; }
    }

    public GPSPoint(float lat, float lng) {
      this.lat = lat;
      this.lng = lng;
    }

    public Double GetDistanceTo(GPSPoint Point) {
      var sCoord = new GeoCoordinate(lat, lng);
      var eCoord = new GeoCoordinate(Point.lat, Point.lng);

      return Math.Abs(sCoord.GetDistanceTo(eCoord));
    }
  }

  public class GPSPolygon {
    private List<GPSPoint> Points { get; set; } = new List<GPSPoint>();

    public int Count {
      get { return Points.Count; }
    }

    public void SetPolygon(String Coordinates) {
      Points.Clear();
      foreach (String Coordinate in Coordinates.Split(',')) {
        String[] XY = Coordinate.Split(' ');
        float lng = 0;
        float lat = 0;
        float.TryParse(XY[0], out lat);
        float.TryParse(XY[1], out lng);
        Points.Add(new GPSPoint(lat, lng));
      }
    }

    public GPSPoint this[int i] {
      get { return Points[i]; }
    }

    public Double GetDistanceTo(GPSPoint Point) {
      return Point.GetDistanceTo(Points[0]);
    }

    // Return True if the point is in the polygon.
    public bool InPolygon(GPSPoint Point) {
      // Get the angle between the point and the
      // first and last vertices.
      int max_point = Points.Count - 1;
      float total_angle = GetAngle(
          Points[max_point].X, Points[max_point].Y,
          Point.X, Point.Y,
          Points[0].X, Points[0].Y);

      // Add the angles from the point
      // to each other pair of vertices.
      for (int i = 0; i < max_point; i++) {
        total_angle += GetAngle(
            Points[i].X, Points[i].Y,
            Point.X, Point.Y,
            Points[i + 1].X, Points[i + 1].Y);
      }

      // The total angle should be 2 * PI or -2 * PI if
      // the point is in the polygon and close to zero
      // if the point is outside the polygon.
      return (Math.Abs(total_angle) > 0.000001);
    }

    // Return the angle ABC.
    // Return a value between PI and -PI.
    // Note that the value is the opposite of what you might
    // expect because Y coordinates increase downward.
    public static float GetAngle(float Ax, float Ay,
        float Bx, float By, float Cx, float Cy) {
      // Get the dot product.
      float dot_product = DotProduct(Ax, Ay, Bx, By, Cx, Cy);

      // Get the cross product.
      float cross_product = CrossProductLength(Ax, Ay, Bx, By, Cx, Cy);

      // Calculate the angle.
      return (float)Math.Atan2(cross_product, dot_product);
    }

    // Return the dot product AB · BC.
    // Note that AB · BC = |AB| * |BC| * Cos(theta).
    private static float DotProduct(float Ax, float Ay,
        float Bx, float By, float Cx, float Cy) {
      // Get the vectors' coordinates.
      float BAx = Ax - Bx;
      float BAy = Ay - By;
      float BCx = Cx - Bx;
      float BCy = Cy - By;

      // Calculate the dot product.
      return (BAx * BCx + BAy * BCy);
    }

    // Return the cross product AB x BC.
    // The cross product is a vector perpendicular to AB
    // and BC having length |AB| * |BC| * Sin(theta) and
    // with direction given by the right-hand rule.
    // For two vectors in the X-Y plane, the result is a
    // vector with X and Y components 0 so the Z component
    // gives the vector's length and direction.
    public static float CrossProductLength(float Ax, float Ay,
        float Bx, float By, float Cx, float Cy) {
      // Get the vectors' coordinates.
      float BAx = Ax - Bx;
      float BAy = Ay - By;
      float BCx = Cx - Bx;
      float BCy = Cy - By;

      // Calculate the Z coordinate of the cross product.
      return (BAx * BCy - BAy * BCx);
    }

  }
  public  class GPS {
    private GPSPolygon Polygon = null;

    public GPS() {
      
    }
    public GPS(GPSPolygon Polygon) {
      this.Polygon = Polygon;
    }

    public void SetPolygon(String Coordinates) {
      if (Polygon == null)
        Polygon = new GPSPolygon();
      Polygon.SetPolygon(Coordinates);
    }

    public bool InPolygon(GPSPoint Point) {
      if (Polygon != null)
        return Polygon.InPolygon(Point);
      return false;
    }

  }

  public static class GPSCalc {

    public static Double GetDistance(GPSPoint Point1, GPSPoint Point2) {
      var sCoord = new GeoCoordinate(Point1.lat, Point1.lng);
      var eCoord = new GeoCoordinate(Point2.lat, Point2.lng);
      return sCoord.GetDistanceTo(eCoord);    
      
    }

    public static Double GetAngle(GPSPoint Point1, GPSPoint Point2) {
      //stackoverflow.com/questions/3932502/calculate-angle-between-two-latitude-longitude-points
      double long1 = Point1.lng, long2 = Point2.lng;
      double lat1 = Point1.lat, lat2 = Point2.lat;

      double dLon = (long2 - long1);

      double y = Math.Sin(dLon) * Math.Cos(lat2);
      double x = Math.Cos(lat1) * Math.Sin(lat2) - Math.Sin(lat1)
              * Math.Cos(lat2) * Math.Cos(dLon);

      double brng = Math.Atan2(y, x);

      brng = brng * (180.0 / Math.PI); //convert to degree
      brng = (brng + 360) % 360;
      brng = 360 - brng; // count degrees counter-clockwise - remove to make clockwise

      return brng;
    }


  }
}
