using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightProcessor.EXLogic {
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
      case "height":
        _AlertMessage = $"UAS {_DroneName} of {_AccountName} is unauthorised at {GetLocation()} at an altitude of {sAltitude} Meter on UTC {sFlightTime} - Ref: {_Flight.GetBase().ID}";
        IsMessageGenerated = true;
        break;
      case "proximity":
        _AlertMessage =
          $"UAS {_DroneName} of {_AccountName} [{_Flight.GetBase().ID}] and " +
          $"{ProximityFlight.DroneName} of {ProximityFlight.AccountName} [{ProximityFlight.FlightID}] " +
          $" are close to {ProximityFlight.Distance.ToString("0.00")} Meter";
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
      String Format = "0.0000";
      if (Num == 0)
        return Num.ToString(Format);
      else
        return Num.ToString(Format) + (Num > 0 ? Positive : Negetive);
    }

  }//public class Alert
}
