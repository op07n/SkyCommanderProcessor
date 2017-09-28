using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkyCommanderProcessor.Models;

namespace SkyCommanderProcessor.EXLogic {
  public class Alert {
    private String _AlertType { get; set; }
    private String _AlertCategory { get; set; }
    private String _AlertMessage { get; set; }
    private String _DroneName { get; set; }
    private String _AccountName { get; set; }

    private ExtDroneFight _Flight;

    public Alert(ExtDroneFight TheFlight) {
      _Flight = TheFlight;
      _DroneName = _Flight.DroneName;
      _AccountName = _Flight.AccountName;
    }

    public bool Generate(String AlertCategory, String AlertType = "High", Proximity ProximityFlight = null) {
      bool IsMessageGenerated = false;
      String sAltitude = ((Decimal)(_Flight.Altitude)).ToString("0.00");
      String sFlightTime = ((DateTime)_Flight.GetBase().FlightDate).ToString("dd-MMM-yyyy HH:mm:ss");
      _AlertCategory = AlertCategory;
      _AlertType = AlertType;
      switch (AlertCategory.ToLower()) {
      case "boundary":      
      case "altitude":
        _AlertMessage =
          AlertCategory + "\n" +
          _AccountName + "\n" +
          _Flight.PilotName + "\n" +
          GetLocation() + "\n" +
          "Altitude: " + sAltitude + " Meter\n" +
          "UTC " + sFlightTime + "\n" +
          "Ref: " + _Flight.GetBase().ID;
        IsMessageGenerated = true;
        break;
      case "proximity":
        _AlertMessage =
          AlertCategory + Environment.NewLine +
          _AccountName + Environment.NewLine +
          _Flight.PilotName + Environment.NewLine +
          _DroneName + Environment.NewLine +
          GetLocation() + Environment.NewLine +
          "Altitude: " + sAltitude + " Meter" + Environment.NewLine +
          "Ref: " + _Flight.GetBase().ID.ToString() + Environment.NewLine +
          Environment.NewLine +

          ProximityFlight.AccountName + Environment.NewLine +
          ProximityFlight.PilotName + Environment.NewLine +
          ProximityFlight.DroneName + Environment.NewLine +
          ProximityFlight.Location + Environment.NewLine +
          "Altitude: " + sAltitude + " Meter" + Environment.NewLine +
          "Ref: " + ProximityFlight.FlightID.ToString() + Environment.NewLine +
          Environment.NewLine +

          "Distance: " + ProximityFlight.Distance.ToString("0.00") + Environment.NewLine +
          "UTC " + sFlightTime;


          IsMessageGenerated = true;
        break;
      }
      return IsMessageGenerated;
    }//public String GetAlert

   
    public async Task<bool> Save() {
      using (FlightProcessorConnection CN = new FlightProcessorConnection()) { 
        //Add to Portal Alert table, to record SMS
        PortalAlert ThisAlert = new PortalAlert {
          AccountID = _Flight.AccountID,
          AlertCategory = _AlertCategory,
          AlertMessage = _AlertMessage,
          AlertType = _AlertType,
          Altitude = (int)_Flight.Altitude,
          ApprovalID = _Flight.GetBase().ApprovalID,
          CreatedOn = DateTime.UtcNow,
          DroneID = _Flight.GetBase().DroneID,
          FlightDataID = _Flight.LastProcessedID,
          FlightReadTime = _Flight.LastFlightDate,
          Latitude = _Flight.GetBase().Latitude,
          Longitude = _Flight.GetBase().Longitude,
          PilotID = _Flight.GetBase().PilotID,
          FlightID = _Flight.GetBase().ID
        };

        CN.PortalAlert.Add(ThisAlert);
        await CN.SaveChangesAsync();
        if(_AlertType == "Critical")
          await AlertQueue.AddToSmsQueue(ThisAlert);
      }
      return true;
    }

    public async Task<bool> SendSMS( List<String> SMSNumbers) {
      FlightProcessorConnection CN = new FlightProcessorConnection();

      if (_AlertType == "Critical") {
        foreach (String SMSNumber in SMSNumbers) {
          PortalAlertEmail Notification = new PortalAlertEmail {
            Attachments = null,
            Body = _AlertMessage,
            CreatedOn = DateTime.UtcNow,
            EmailID = 0,
            EmailSubject = $"SMS - {_AlertCategory}",
            EmailURL = null,
            FromAddress = null,
            IsSend = 0,
            SendOn = null,
            SendStatus = null,
            SendType = "SMS",
            ToAddress = SMSNumber,
            UserID = 0
          };
          CN.PortalAlertEmail.Add(Notification);
        }//foreach (String SMSNumber in SMSNumbers)
        await CN.SaveChangesAsync();
      }//if(_AlertType == "Critical")
      return true;
    }


    private String GetLocation() {
      Decimal Lat = (Decimal)(_Flight.GetBase().Latitude);
      Decimal Lng = (Decimal)(_Flight.GetBase().Longitude);
      return SufixTo(Lat, "N", "S") + " " + SufixTo(Lng, "E", "W");
    }

    private String SufixTo(Decimal Num, String Positive, String Negetive) {
      String Format = "0.000000";
      if (Num == 0)
        return Num.ToString(Format);
      else
        return Num.ToString(Format) + (Num > 0 ? Positive : Negetive);
    }

  }//public class Alert
}
