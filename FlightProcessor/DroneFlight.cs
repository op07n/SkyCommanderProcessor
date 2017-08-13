//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace FlightProcessor
{
    using System;
    using System.Collections.Generic;
    
    public partial class DroneFlight
    {
        public int ID { get; set; }
        public Nullable<int> PilotID { get; set; }
        public Nullable<int> GSCID { get; set; }
        public Nullable<System.DateTime> FlightDate { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<int> DroneID { get; set; }
        public Nullable<decimal> BBFlightID { get; set; }
        public Nullable<decimal> Latitude { get; set; }
        public Nullable<decimal> Longitude { get; set; }
        public string LogFrom { get; set; }
        public string LogTo { get; set; }
        public Nullable<System.DateTime> LogTakeOffTime { get; set; }
        public Nullable<System.DateTime> LogLandingTime { get; set; }
        public string LogBattery1ID { get; set; }
        public Nullable<decimal> LogBattery1StartV { get; set; }
        public Nullable<decimal> LogBattery1EndV { get; set; }
        public string LogBattery2ID { get; set; }
        public Nullable<decimal> LogBattery2StartV { get; set; }
        public Nullable<decimal> LogBattery2EndV { get; set; }
        public Nullable<byte> IsLogged { get; set; }
        public Nullable<System.DateTime> LogDateTime { get; set; }
        public Nullable<int> LogCreatedBy { get; set; }
        public string Descrepency { get; set; }
        public string ActionTaken { get; set; }
        public Nullable<bool> IsFlightOutside { get; set; }
        public string RecordedVideoURL { get; set; }
        public Nullable<int> IsFlightSoftFence { get; set; }
        public Nullable<int> LowerLimit { get; set; }
        public Nullable<int> HigherLimit { get; set; }
        public Nullable<decimal> GridLat { get; set; }
        public Nullable<decimal> GridLng { get; set; }
        public Nullable<int> FlightHours { get; set; }
        public Nullable<int> HasFlightInfo { get; set; }
        public Nullable<decimal> FlightDistance { get; set; }
        public Nullable<bool> IsInsideTimeFrame { get; set; }
        public Nullable<int> IsDistanceWarning { get; set; }
        public Nullable<int> IsDistanceCritical { get; set; }
        public int ApprovalID { get; set; }
        public Nullable<decimal> MaxSpeed { get; set; }
        public Nullable<decimal> MaxAltitude { get; set; }
        public Nullable<int> BoundaryCritical { get; set; }
        public Nullable<int> BoundaryHigh { get; set; }
        public Nullable<int> BoundaryWarning { get; set; }
        public Nullable<int> HeightCritical { get; set; }
        public Nullable<int> HeightHigh { get; set; }
        public Nullable<int> HeightWarning { get; set; }
        public Nullable<int> ProximityCritical { get; set; }
        public Nullable<int> ProximityHigh { get; set; }
        public Nullable<int> ProximityWarning { get; set; }
    }
}
