using SkyCommanderProcessor.EXLogic;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using SkyCommanderProcessor.Models;

namespace SkyCommanderProcessor {
  public partial class frmMain : Form {
    public bool IsTimerRunning = false;
    private bool IsAppRunning = true;
    System.Timers.Timer _timer1 = new System.Timers.Timer();
    System.Timers.Timer _timer2 = new System.Timers.Timer();
    private static Thread ThreadDBActions;
    private static Thread ThreadSendAlertQueue;
    private static readonly object _lockFileWrite = new object();
    public frmMain() {
      InitializeComponent();
    }
    private void frmMain_Load(object sender, EventArgs e) {

      using(FlightProcessorConnection cn = new FlightProcessorConnection()) {
        this.Text += " [" + cn.Database.Connection.Database + "]";
      }

      _timer1.Interval = 2000;
      _timer1.Elapsed += new System.Timers.ElapsedEventHandler(_timerEvent1);
      _timer1.Enabled = true;

      _timer2.Interval = 2000; //every 10 second
      _timer2.Elapsed += new System.Timers.ElapsedEventHandler(_timerEvent2);
      _timer2.Enabled = true;

      ThreadDBActions = new Thread(new ThreadStart(fnThreadDBActions));
      ThreadDBActions.IsBackground = true;
      ThreadDBActions.Start();

      ThreadSendAlertQueue = new Thread(new ThreadStart(fnSendAlertQueue));
      ThreadSendAlertQueue.IsBackground = true;
      ThreadSendAlertQueue.Start();

      ResetFlightsGrid();
      gridFlights.AutoSizeCells();
      ResetNotificationGrid();
      gridNotifications.AutoSizeCells();

      WorkerProcess.OnTaskError += ShowExceptions;
      WorkerProcess.OnDbEntityException += ShowEntityErrors;
      WorkerProcess.StartProcessor();

    }

    public async void fnSendAlertQueue() {
      while(IsAppRunning) {
        try {
          await AlertQueue.SendQueue();
        } catch(Exception ex) {
          ShowExceptions(new TaskExceptionInfo {
            TaskException = ex,
            TaskName = "Send Alert Queue"
          });
        }
        await Task.Delay(100);
      }//while(IsAppRunning)
    }

    public async void fnThreadDBActions() {
      while (IsAppRunning) {
        try {
          await fnThreadDBActionsAction();
        } catch (Exception ex) {
          ShowExceptions(new TaskExceptionInfo {
            TaskException = ex,
            TaskName = "Load Pending Data - fnLoadPendingDataMain"
          });
        }
        //Sleep for 4 second before do the proximity alearts
        await Task.Delay(4000);
      }

    }

    public async Task<bool> fnThreadDBActionsAction() {
      // Check any Flights that is more than 60 * 3 seconds old, then
      // Generate the report and remove the flights
      foreach (String Key in WorkerProcess.Keys()) {
        DroneDataProcessor TheFlight = WorkerProcess.Flight(Key);
        Double ElapsedTime = TheFlight.GetElapsed();

        if (ElapsedTime > 30) {
          //Generate the report
          await TheFlight.UpdateFlightSummary();
          await TheFlight.GenerateReport();
          WorkerProcess.RemoveProximity(Key);
          WorkerProcess.RemoveFlight(Key);
          TheFlight = null;
        }
      }
      //Update the Flight information back to Database
      await WorkerProcess.UpdateFlightsToDB();

      //Update Proximity of active flights
      await WorkerProcess.GenerateProximity();

      //Load list of alerts required to send
      await AlertQueue.LoadQueue();
      return true;
    }

    private void ShowEntityErrors(System.Data.Entity.Validation.DbEntityValidationException e) {
      StringBuilder Errors = new StringBuilder();
      foreach (var eve in e.EntityValidationErrors) {
        Errors.AppendLine($"Entity Type: \"{eve.Entry.Entity.GetType().Name}\", State: \"{eve.Entry.State}\"");
        foreach (var ve in eve.ValidationErrors) {
          Errors.AppendLine($"- Property: \"{ve.PropertyName}\", Error: \"{ve.ErrorMessage}\"");
        }
      }
      WriteToLog(Errors.ToString());
      txtExceptions.Invoke(new Action(() => {
        if (txtExceptions.Text.Length > 50000)
          txtExceptions.ResetText();
        txtExceptions.AppendText(Errors.ToString());
      }));
    }

    private void ShowExceptions(TaskExceptionInfo e) {
      WriteToLog(e.TaskException.ToString());
      txtExceptions.Invoke(new Action(() => {
        if (txtExceptions.Text.Length > 50000)
          txtExceptions.ResetText();
        txtExceptions.AppendText(e.TaskName + "\n\n" + e.TaskException.ToString());
      }));
    }
    
    private void WriteToLog(String ErrorString) {
      try { 
        String FileName = System.IO.Path.Combine(Application.StartupPath, "SkyCommanderProcessor.Log");
        lock(_lockFileWrite) {
          using (System.IO.StreamWriter file = new System.IO.StreamWriter(FileName, true)) {
            file.WriteLine("------------------------------------------------------------------");
            file.WriteLine(DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"));
            file.WriteLine("------------------------------------------------------------------");
            file.WriteLine(ErrorString);
            file.Close();
          }
        }
      } catch {
        //nothing to do here.
      }
    }

    private  void _timerEvent1(object Source, System.Timers.ElapsedEventArgs e ) {
      SetText(txtPendingRecords, WorkerProcess.GetPendingDataCount().ToString());

      gridFlights.Invoke(new Action(() => {
        ResetFlightsGrid();
        int Row = 0;
        foreach(String Key in WorkerProcess.Keys()) {
          Row++;
          gridFlights.Rows.Insert(Row);
          for(var i = 0; i <= 9; i++) { 
            gridFlights[Row, i] = WorkerProcess.Flight(Key).Cell(i);
          }
        }
        gridFlights.AutoSizeCells();
      }));


    }


    private  void _timerEvent2(object Source, System.Timers.ElapsedEventArgs e) {
      SetText(txtPendingNotifications, AlertQueue.QueueCount.ToString());

      gridNotifications.Invoke(new Action(() => {
        ResetNotificationGrid();
        lock(AlertQueue._lockerQueue) { 
          for(int Row = 1; Row <= AlertQueue.QueueCount; Row++) {
            gridNotifications.Rows.Insert(Row);
            for(var i = 0; i <= 6; i++) {
              gridNotifications[Row, i] = AlertQueue.Cell(Row, i);
            }
          }
        }

        lock (AlertQueue._lockerResetQueue) {
          int GridRow = AlertQueue.QueueCount;
          for(int Row = 1; Row <= AlertQueue.QueueProcessedCount; Row++) {
            gridNotifications.Rows.Insert(GridRow + Row);
            for (var i = 0; i <= 6; i++) {
              gridNotifications[Row, i] = AlertQueue.Cell(Row, i, "Processed");
            }
          }
        }

        gridNotifications.AutoSizeCells();
      }));

      AlertQueue.ResetQueue();
    }

    private void ResetFlightsGrid() {
      gridFlights.Rows.Clear();
      gridFlights.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
      gridFlights.ColumnsCount = 10;
      gridFlights.FixedRows = 1;
      gridFlights.Rows.Insert(0);
      gridFlights[0, 0] = new SourceGrid.Cells.ColumnHeader("BB ID");
      gridFlights[0, 1] = new SourceGrid.Cells.ColumnHeader("Flight#");
      gridFlights[0, 2] = new SourceGrid.Cells.ColumnHeader("Received Date");
      gridFlights[0, 3] = new SourceGrid.Cells.ColumnHeader("Speed");
      gridFlights[0, 4] = new SourceGrid.Cells.ColumnHeader("Altitude");
      gridFlights[0, 5] = new SourceGrid.Cells.ColumnHeader("Satellite");
      gridFlights[0, 6] = new SourceGrid.Cells.ColumnHeader("Total Time");
      gridFlights[0, 7] = new SourceGrid.Cells.ColumnHeader("Distance");
      gridFlights[0, 8] = new SourceGrid.Cells.ColumnHeader("Last Updated");
      gridFlights[0, 9] = new SourceGrid.Cells.ColumnHeader("Elapsed");
    }

    private void ResetNotificationGrid() {
      gridNotifications.Rows.Clear();
      gridNotifications.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
      gridNotifications.ColumnsCount = 7;
      gridNotifications.FixedRows = 1;
      gridNotifications.Rows.Insert(0);
      gridNotifications[0, 0] = new SourceGrid.Cells.ColumnHeader("ID");
      gridNotifications[0, 1] = new SourceGrid.Cells.ColumnHeader("Alert");
      gridNotifications[0, 2] = new SourceGrid.Cells.ColumnHeader("To:");
      gridNotifications[0, 3] = new SourceGrid.Cells.ColumnHeader("Body");
      gridNotifications[0, 4] = new SourceGrid.Cells.ColumnHeader("Send?");
      gridNotifications[0, 5] = new SourceGrid.Cells.ColumnHeader("Send On");
      gridNotifications[0, 6] = new SourceGrid.Cells.ColumnHeader("Elapsed");
    }

    private void SetText(TextBox txtBox, String TextToShow) {
      if (txtBox.InvokeRequired) {
        txtBox.Invoke(new Action(() => { txtBox.Text = TextToShow; }));
        return;
      }
      txtBox.Text = TextToShow;
    }

    private void button1_Click(object sender, EventArgs e) {
      IsAppRunning = false;
      WorkerProcess.StopProcessor();
      Environment.Exit(Environment.ExitCode);
    }

    private void btnPDF_Click(object sender, EventArgs e) {
      SelectPdf.HtmlToPdf converter = new SelectPdf.HtmlToPdf();
      converter.Options.MarginBottom = 10;
      converter.Options.MarginLeft = 10;
      converter.Options.MarginRight = 10;
      converter.Options.MarginTop = 10;
      converter.Options.EmbedFonts = true;
      converter.Options.DrawBackground = true;
      converter.Options.JavaScriptEnabled = true;
      converter.Options.KeepImagesTogether = true;
      SelectPdf.PdfDocument doc = converter.ConvertUrl("http://portal.exponent-ts.com/Report/FlightReport/236");
      doc.Save($"C:\\007\\Test-{DateTime.Now.Ticks}.pdf");
      doc.Close();
    }
  }
}
