using FlightProcessor.EXLogic;
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

namespace FlightProcessor {
  public partial class frmMain : Form {
    public bool IsTimerRunning = false;
    System.Timers.Timer _timer1 = new System.Timers.Timer();
    System.Timers.Timer _timer2 = new System.Timers.Timer();

    public frmMain() {
      InitializeComponent();
    }
    private void frmMain_Load(object sender, EventArgs e) {
      _timer1.Interval = 1000;
      _timer1.Elapsed += new System.Timers.ElapsedEventHandler(_timerEvent1);
      _timer1.Enabled = true;

      _timer2.Interval = 1000 * 4; //every 10 second
      _timer2.Elapsed += new System.Timers.ElapsedEventHandler(_timerEvent2);
      _timer2.Enabled = true;


      ResetGrid();
      gridFlights.AutoSizeCells();
      WorkerProcess.OnTaskError += ShowExceptions;
      WorkerProcess.StartProcessor();

    }

    private void ShowExceptions(TaskExceptionInfo e) {
      txtExceptions.Invoke(new Action(() => {
        if (txtExceptions.Text.Length > 50000)
          txtExceptions.ResetText();
        txtExceptions.AppendText(e.TaskName + "\n\n" + e.TaskException.ToString());
      }));
    }
    

    private async void _timerEvent1(object Source, System.Timers.ElapsedEventArgs e ) {
      SetText(txtPendingRecords, WorkerProcess.GetPendingDataCount().ToString());
      
      gridFlights.Invoke(new Action(() => {
        ResetGrid();
        int Row = 0;
        foreach(String Key in WorkerProcess.Keys()) {
          Row++;
          gridFlights.Rows.Insert(Row);
          for(var i = 0; i <= 8; i++) { 
            gridFlights[Row, i] = WorkerProcess.Flight(Key).Cell(i);
          }
        }
        gridFlights.AutoSizeCells();
      }));

      // Check any Flights that is more than 60 * 3 seconds old, then
      // Generate the report and remove the flights
      foreach (String Key in WorkerProcess.Keys()) {
        DroneDataProcessor TheFlight = WorkerProcess.Flight(Key);
        Double ElapsedTime = TheFlight.GetElapsed();
        
        if (ElapsedTime > 30) {
          //Generate the report
          TheFlight.GenerateReport();
          TheFlight = null;
          WorkerProcess.RemoveProximity(Key);
          WorkerProcess.RemoveFlight(Key);
        } 
      }

      //Update Proximity of active flights
      await WorkerProcess.GenerateProximity();
    }


    private async void _timerEvent2(object Source, System.Timers.ElapsedEventArgs e) {
      //Update the Flight information back to Database
      var Result = await WorkerProcess.UpdateFlightsToDB();
    }

    private void ResetGrid() {
      gridFlights.Rows.Clear();
      gridFlights.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
      gridFlights.ColumnsCount = 9;
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

    }

    private void SetText(TextBox txtBox, String TextToShow) {
      if (txtBox.InvokeRequired) {
        txtBox.Invoke(new Action(() => { txtBox.Text = TextToShow; }));
        return;
      }
      txtBox.Text = TextToShow;
    }

    private void button1_Click(object sender, EventArgs e) {
      Environment.Exit(Environment.ExitCode);
    }
    

  }
}
