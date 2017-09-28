using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkyCommanderProcessor.EXLogic {
  public class SimpleProximity {
    public Double Lat { get; set; }
    public Double Lng { get; set; }
    public Double Distance { get; set; }
    public Double Altitude { get; set; }
  }

  public class Proximity : SimpleProximity {
    public int FlightID { get; set; }
    public int PilotID { get; set; }
    public int AccountID { get; set; } = 0;
    public String DroneName { get; set; }
    public String AccountName { get; set; }
    public String PilotName { get; set; }

    public bool IsProximityWarning { get; set; } = false;
    public bool IsProximityCritical { get; set; } = false;


    public String Location {
      get {
        return SufixTo(this.Lat, "N", "S") + " " + SufixTo(this.Lng, "E", "W");
      }
    }

    private String SufixTo(Double Num, String Positive, String Negetive) {
      String Format = "0.000000";
      if (Num == 0)
        return Num.ToString(Format);
      else
        return Num.ToString(Format) + (Num > 0 ? Positive : Negetive);
    }

  }
}
