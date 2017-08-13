using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightProcessor.EXLogic {
  public class FlightApproval {
    private GPSPolygon BoundaryPolygon { get; set; } = new GPSPolygon();
    private GPSPolygon SoftBoundaryPolygon { get; set; } = new GPSPolygon();

    public int ApprovalId { get; set; } = 0;
    public String ApprovalName { get; set; }
    public String Boundary { set {
        BoundaryPolygon.SetPolygon(value);
      }
    }
    public String SoftBoundary {
      set {
        SoftBoundaryPolygon.SetPolygon(value);
      }
    }
    public Decimal MinAltitude { get; set; } = 0;
    public Decimal MaxAltitude { get; set; } = 0;

    public FlightApproval() {
    }




    public int GetApprovalId() {
      return ApprovalId;
    }
    

    public bool IsInBoundary(GPSPoint Point) {
      return BoundaryPolygon.InPolygon(Point);
    }

    public bool IsInSoftBoundary(GPSPoint Point) {
      return SoftBoundaryPolygon.InPolygon(Point);
    }

    public bool IsInAltitudeLimit(Decimal Altitude) {
      if (Altitude >= MinAltitude && Altitude <= MaxAltitude)
        return true;
      return false;
    }

  }

}
