using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Data.Entity;
using SkyCommanderProcessor.Models;

namespace SkyCommanderProcessor.EXLogic {
  public class TaskExceptionInfo: EventArgs {
    public String TaskName { get; set; }
    public Exception TaskException { get; set; }
  }


  public static class WorkerProcess {
    public delegate void OnTaskErrorHandler(TaskExceptionInfo e);
    public static event OnTaskErrorHandler OnTaskError;
    public delegate void DbEntityException(System.Data.Entity.Validation.DbEntityValidationException e);
    public static event DbEntityException OnDbEntityException;

    //private static FlightProcessorConnection cn = new FlightProcessorConnection();
    private static List<DroneData> PendingDroneData = new List<DroneData>();
    private static Dictionary<String, DroneDataProcessor> ActiveFlights = new Dictionary<String, DroneDataProcessor>();
    private static List<int> RemovedDroneDataIDs = new List<int>();
    private static bool IsAppRunning = false;

    //variables used in the application
    private static readonly object _locker = new object();
    private static readonly object _lockFlight = new object();
    private static int _LastDroneDataID { get; set; } = 0;

    //variables to run threads
    private static Thread ThreadLoadPendingData { get; set; }
    private static Thread ThreadProcessing { get; set; }
    private static Thread ThreadUpdateDroneData { get; set; }

    public static DroneDataProcessor Flight(String KeyName) {
      lock(_lockFlight) { 
        return ActiveFlights[KeyName];
      }
    }

    public static bool RemoveFlight(String Key) {
      lock (_lockFlight) {
        ActiveFlights[Key] = null;
        return ActiveFlights.Remove(Key);

      }
    }

    public static List<String> Keys() {
      lock (_lockFlight) {
        return ActiveFlights.Keys.ToList();
      }
    }


    public static int GetPendingDataCount() {
      return PendingDroneData.Count;
    }

    public static void StartProcessor() {
      IsAppRunning = true;
      ThreadLoadPendingData = new Thread(new ThreadStart(fnLoadPendingDataMain));
      ThreadProcessing = new Thread(new ThreadStart(fnThreadProcessingMain));
      ThreadUpdateDroneData = new Thread(new ThreadStart(fnThreadUpdateDroneDataMain));


      ThreadLoadPendingData.IsBackground = true;
      ThreadProcessing.IsBackground = true;
      ThreadUpdateDroneData.IsBackground = true;

      ThreadLoadPendingData.Start();
      ThreadProcessing.Start();
      ThreadUpdateDroneData.Start();

    }

    public static void StopProcessor() {
      IsAppRunning = false;
    }

    public static void RemoveProximity(String Key) {
      int FlightIDToRemove = ActiveFlights[Key].GetFlightID();
      foreach(String TheKey in Keys()) {
        ActiveFlights[TheKey].ProximityRemove(FlightIDToRemove);
      }//foreach(String TheKey in AllKeys)
    }//public static RemoveProximity()

    public static async Task GenerateProximity() {
      var AllKeys = Keys();

      foreach(String FromKey in AllKeys) {
        DroneDataProcessor ProximityFromFlight = Flight(FromKey);
        GPSPoint FromPosition = ProximityFromFlight.GetPosition();
        int FromFlightID = ProximityFromFlight.GetFlightID();
        foreach (String ToKey in AllKeys) {         
          if(FromKey != ToKey) {
            DroneDataProcessor ProximityToFlight = Flight(ToKey);
            GPSPoint ToPosition = ProximityToFlight.GetPosition();
            int ToFlightID = ProximityToFlight.GetFlightID();
            Double Distance = GPSCalc.GetDistance(FromPosition, ToPosition);
            ProximityFromFlight.ProximityAdd(new Proximity {
              FlightID = ToFlightID,
              Lat = ToPosition.lat,
              Lng = ToPosition.lng,
              Distance = Distance,
              DroneName = ProximityToFlight.GetProperty("DroneName").ToString(),
              AccountID = (int)ProximityToFlight.GetProperty("AccountID"),
              AccountName = ProximityToFlight.GetProperty("AccountName").ToString(),
              Altitude = ProximityToFlight.Altitude,
              PilotID = ProximityToFlight.PilotID,
              PilotName = ProximityToFlight.PilotName
            });
            //Console.WriteLine($"From: {FromKey}({FromFlightID}) -> To: {ToKey}({ToFlightID}),  Distance: {Distance}");
          }//if(FromKey != ToKey)
        }//foreach (String ToKey in AllKeys)
        await ProximityFromFlight.GenerateProximityAlerts();
      }//foreach(String FromKey in AllKeys)



    }//public static void UpdateProximity() 

    public async static Task<bool> UpdateFlightsToDB() {
     
      foreach (String FromKey in Keys()) {
        try { 
        var Result = await Flight(FromKey).UpdateFlightsToDB();
        } catch (Exception ex) {
          OnTaskError?.Invoke(new TaskExceptionInfo {
            TaskException = ex,
            TaskName = $"Update Flight: FromKey={FromKey} - UpdateFlightsToDB()"
          });
        }
      }
      return true;
    }
    
    public async static void fnLoadPendingDataMain() {
      try { 
        while (IsAppRunning) {
          var TheTask = await fnLoadPendingData();
          await Task.Delay(10);
        }
      } catch(Exception ex) {
        OnTaskError?.Invoke(new TaskExceptionInfo {
        TaskException = ex,
        TaskName = "Load Pending Data - fnLoadPendingDataMain"
        });
      }
    }

    public async static void fnThreadProcessingMain() {
      try {
        while (IsAppRunning) {
          var TheTask = await fnThreadProcessing();
          await Task.Delay(10);
        }
      } catch (System.Data.Entity.Validation.DbEntityValidationException e) {
        OnDbEntityException?.Invoke(e);
      } catch (Exception ex) {
          OnTaskError?.Invoke(new TaskExceptionInfo {
            TaskException = ex,
            TaskName = "Main Thred Processing - fnThreadProcessingMain"
          });
      }
    }

    public async static void fnThreadUpdateDroneDataMain() {
      try { 
        while (IsAppRunning) {
          var TheTask = await fnThreadUpdateDroneData();        
        }
      } catch (System.Data.Entity.Validation.DbEntityValidationException e) {
        OnDbEntityException?.Invoke(e);
      } catch (Exception ex) {
        OnTaskError?.Invoke(new TaskExceptionInfo {
          TaskException = ex,
          TaskName = "Update Drone Data - fnThreadUpdateDroneDataMain"
        });
      }
    }

    private static void fnOnTaskError(TaskExceptionInfo exp) {
      OnTaskError?.Invoke(exp);
    }

    private static void fnOnDbEntityException(System.Data.Entity.Validation.DbEntityValidationException e) {
      OnDbEntityException?.Invoke(e);
    }

    private static async Task<bool> fnLoadPendingData() {
      using(FlightProcessorConnection cn = new FlightProcessorConnection()) { 
        var Query = from d in cn.DroneData
                    where (d.IsProcessed == null || d.IsProcessed == 0) && d.DroneDataId > _LastDroneDataID
                    orderby d.DroneDataId ascending
                    select d;
        if (await Query.AnyAsync()) {
          List<DroneData> Data = await Query.Take(50).ToListAsync();
          lock (_locker) {
            PendingDroneData.AddRange(Data);
          }
          _LastDroneDataID = Data.Last().DroneDataId;
          //a mininum pause for next read
          await Task.Delay(200);
        } else {
          //if no record pending, then wait for 2 second before next read
          await Task.Delay(2000);
        }//if(await Query.AnyAsync())
      }
      return true;
    }//private static async void fnLoadPendingData()


    private static async Task<bool> fnThreadProcessing() {
      DroneData ThisDroneData;
      DroneDataProcessor ThisFlight;

      //Get the First Data from Pending Data
      lock (_locker) {
        if (!PendingDroneData.Any()) {
          Thread.Sleep(100);
          return false;
        }          
        ThisDroneData = PendingDroneData.First();
        PendingDroneData.RemoveAt(0);
        RemovedDroneDataIDs.Add(ThisDroneData.DroneDataId);
      }

      String BBFlightID = ThisDroneData.BBFlightID;
      if(!ActiveFlights.ContainsKey(BBFlightID)) {
        DroneDataProcessor ThisDataProcessor = new DroneDataProcessor();
        ThisDataProcessor.OnTaskError += fnOnTaskError;
        ThisDataProcessor.OnDbEntityException += fnOnDbEntityException;

        ThisDataProcessor.SetDroneData(ThisDroneData);
        await ThisDataProcessor.GenerateFlight();
        ActiveFlights.Add(BBFlightID, ThisDataProcessor);
        ThisFlight = ActiveFlights[BBFlightID];
      } else {
        ThisFlight = ActiveFlights[BBFlightID];
        ThisFlight.SetDroneData(ThisDroneData);
      }
      
      ThisFlight.ProcessDroneData();
      await ThisFlight.GenerateAlerts();
      await ThisFlight.UpdateFlight();
      await ThisFlight.Update_MSTR_Drone();


      return true;
    }

    private async static Task<bool> fnThreadUpdateDroneData() {
      List<int> PendingRemovedDroneDataIDs;

      lock (_locker) {
        if (RemovedDroneDataIDs.Count < 1) return false;
        PendingRemovedDroneDataIDs = RemovedDroneDataIDs.ToList();
        RemovedDroneDataIDs.Clear();
      }

      using(var update = new FlightProcessorConnection()) { 
        var Records = update.DroneData.Where(e => PendingRemovedDroneDataIDs.Contains(e.DroneDataId)).ToList();
        Records.ForEach(e => e.IsProcessed = 1);
        await update.SaveChangesAsync();
      }

      return true;
    }//private async static Task<bool> fnThreadUpdateDroneData()

  }//public static class WorkerProcess
}//namespace SkyCommanderProcessor.EXLogic
