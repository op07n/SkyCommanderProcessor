using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SkyCommanderProcessor.Models;

namespace SkyCommanderProcessor.EXLogic {
  public class AlertSendStatus {
    public bool IsSend { get; set; } = false;
    public String ProviderMessage { get; set; }
  }

  public static class AlertQueue {
    private static List<String> GlobalEmailIDs = new List<String> {
      "Sojan Lawrence <sojan.lawrence@exponent-ts.com>",
      "Cathey <cathey.thattil@exponent-ts.com>",
      "Asam Khan <asam.khan@exponent-ts.com>"
    };

    //private static FlightProcessorConnection cn = new FlightProcessorConnection();
    private static List<PortalAlertEmail> alertQueue = new List<PortalAlertEmail>();
    private static List<PortalAlertEmail> alertQueueProcessed = new List<PortalAlertEmail>();

    private static List<String> _GlobalSmsNumbers = null;
    private static int _LastLoadedAlertID = 0;
    public static readonly object _lockerQueue = new object();
    public static readonly object _lockerResetQueue = new object();
    private static readonly object _lockSMSNumbers = new object();

    public static int QueueCount {
      get {
        return alertQueue.Count;
      }
    }

    public static int QueueProcessedCount {
      get {
        return alertQueueProcessed.Count;
      }
    }

    public static SourceGrid.Cells.Cell Cell(int Row, int Col, String QueueType = "Queue") {
      /*
      0: ID");
      1: Alert");
      2: To:");
      3: Body");
      4: Send?");
      5: Send On")
      */
      var Item = QueueType == "Queue" ? alertQueue[Row - 1] : alertQueueProcessed[Row - 1];
      SourceGrid.Cells.Cell cell = new SourceGrid.Cells.Cell();
      switch (Col) {
      case 0:
        cell.Value = Item.EmailID;
        break;
      case 1:
        cell.Value = Item.SendType;
        break;
      case 2:
        cell.Value = Item.ToAddress;
        break;
      case 3:
        cell.Value = Item.Body?.Substring(0, 30);
        break;
      case 4:
        cell.Value = (Item.IsSend == 1 ? "Yes" : "No");
        break;
      case 5:
        cell.Value = Item.SendOn == null ? "Pending" :
          ((DateTime)Item.SendOn).ToString("yyyy-MM-dd HH:mm:ss");
        break;
      case 6:
        cell.Value = Item.SendOn == null ? "Pending" :
          (DateTime.UtcNow - (DateTime)Item.SendOn).TotalSeconds.ToString();
        break;
      default:
        cell.Value = "N/A";
        break;
      }
      return cell;
    }

    public static async Task<bool> LoadQueue() {
      using(FlightProcessorConnection cn = new FlightProcessorConnection()) { 
        var Query = from alert in cn.PortalAlertEmail
                    where alert.EmailID > _LastLoadedAlertID && alert.IsSend == 0
                    orderby alert.EmailID ascending
                    select alert;
        if(await Query.AnyAsync()) {
          var QueItems = await Query.ToListAsync();
          lock(_lockerQueue) {
            QueItems.ForEach((item) => {
              _LastLoadedAlertID = item.EmailID;
              alertQueue.Add(item);
            });
          }
        }
      }
      return true;
    }

    public static bool ResetQueue() {
      List<int> ResetQue = new List<int>();
      alertQueueProcessed.ForEach((item) => {
        DateTime SendOn = (item.SendOn == null ? DateTime.UtcNow : (DateTime)item.SendOn);
        Double SpendTime = Math.Abs((Double)(SendOn - DateTime.UtcNow).TotalSeconds);
        if (SpendTime > 120)
          ResetQue.Add(alertQueueProcessed.IndexOf(item));
      });

      lock (_lockerResetQueue) {
        ResetQue.OrderByDescending(e => e).ToList().ForEach((index) => {
          alertQueueProcessed.RemoveAt(index);
        });
      }
      return true;
    }

    public static async Task<bool> SendQueue() {
      PortalAlertEmail portalAlertEmail;
      AlertSendStatus stat = null;
      if (!alertQueue.Any())
        return false;

      //Lock que items for safe threading 
      lock (_lockerQueue) {
        portalAlertEmail = alertQueue[0];
        alertQueue.RemoveAt(0);
      }

      switch (portalAlertEmail.SendType.ToLower()) {
      case "sms":
        stat = await SendSMS(portalAlertEmail);
        break;
      case "email":
        stat = SendEmail(portalAlertEmail);
        break;
      }

      if(stat != null) {
        portalAlertEmail.IsSend =  (byte)(stat.IsSend ? 1 : 0);
        portalAlertEmail.SendOn = DateTime.UtcNow;
        portalAlertEmail.SendStatus = stat.ProviderMessage;
        using (FlightProcessorConnection cn = new FlightProcessorConnection()) {
          var entry = cn.Entry(portalAlertEmail);
          entry.State = EntityState.Modified;
          await cn.SaveChangesAsync();
        }
        lock(_lockerResetQueue) { 
          alertQueueProcessed.Add(portalAlertEmail);
        }
        return true;
      }
      return false;
    }


    private static bool GetSmsNumbers() {
      List<String> cellNumbers = new List<String>();
      using (FlightProcessorConnection cn = new FlightProcessorConnection()) {
        var Query = from sms in cn.SMSTable
                    select sms.CellNumber;
        if (Query.Any()) {
          cellNumbers = Query.ToList();
          _GlobalSmsNumbers = new List<String>();
          cellNumbers.ForEach((num) => _GlobalSmsNumbers.Add(FixCellNumber(num)));
          return true;
        }
      }
      return false;
    }

    private static string FixCellNumber(String Number) {
      Number = Number.Trim();
      if (Number.StartsWith("+"))
        Number = Number.Substring(1);
      if(Number.StartsWith("05"))
        Number = "9715" + Number.Substring(2);
      if(Number.StartsWith("00"))
        Number = Number.Substring(2);
      return Number;
    }

    public static async Task<bool> AddToEmailQueue(PortalAlertEmail portalAlertEmail) {
      if (!String.IsNullOrEmpty(portalAlertEmail.ToAddress))
        portalAlertEmail.ToAddress = portalAlertEmail.ToAddress + ";";
      portalAlertEmail.ToAddress += String.Join(";",GlobalEmailIDs);
      using (FlightProcessorConnection cn = new FlightProcessorConnection()) {
        cn.PortalAlertEmail.Add(portalAlertEmail);
        await cn.SaveChangesAsync();
      }
      return true;
    }

    public static async Task<bool> AddToSmsQueue(PortalAlert alert) {
      //Load default Numbers, if not loaded already
      lock(_lockSMSNumbers) { 
        if (_GlobalSmsNumbers == null) { 
        if (!GetSmsNumbers())
          _GlobalSmsNumbers = new List<String>();
        }
      }
      //Add the SMS Number of local pilot
      List<String> LocalSmsNumbers = _GlobalSmsNumbers.ToList();
      int PilotID = (int)alert.PilotID;
      using (FlightProcessorConnection cn = new FlightProcessorConnection()) {
        String PilotSMSNumber = await cn.MSTR_User
        .Where(w => w.UserId == PilotID)
        .Select(s => s.MobileNo)
        .FirstOrDefaultAsync();
        if (!String.IsNullOrWhiteSpace(PilotSMSNumber))
          LocalSmsNumbers.Add(FixCellNumber(PilotSMSNumber));

        foreach (var CellNumber in LocalSmsNumbers) {
          PortalAlertEmail portalAlertEmail = new PortalAlertEmail {
            CreatedOn = DateTime.UtcNow,
            Attachments = null,
            Body = alert.AlertMessage,
            SendType = "SMS",
            ToAddress = CellNumber,
            IsSend = 0,
            SendStatus = "Waiting",
            UserID = PilotID,
            SendOn = null
          };
          cn.PortalAlertEmail.Add(portalAlertEmail);
        }

        try {
          await cn.SaveChangesAsync();
          return true;
        } catch {

        }
      }
      return false;
    }

    private static async Task<AlertSendStatus> SendSMS(PortalAlertEmail portalAlertEmail) {
      SMS sms = new SMS(portalAlertEmail.ToAddress, portalAlertEmail.Body);
      return await sms.Send();
    }
    private static AlertSendStatus SendEmail(PortalAlertEmail portalAlertEmail) {
      AlertSendStatus alertSendStatus = new AlertSendStatus {
        IsSend = false
      };
      EmailMessage email = new EmailMessage(portalAlertEmail);

      Task taskA = Task.Factory.StartNew(() => { alertSendStatus = email.Send(); });
      taskA.Wait();

      return alertSendStatus;
    }

  }
}
