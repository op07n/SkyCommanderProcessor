//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SkyCommanderProcessor.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class PayloadTraffic
    {
        public Nullable<int> FlightID { get; set; }
        public Nullable<System.DateTime> ProcessTime { get; set; }
        public Nullable<decimal> MaxSpeed { get; set; }
        public Nullable<decimal> MinSpeed { get; set; }
        public Nullable<decimal> MediumSpeed { get; set; }
        public Nullable<int> NumberOfCar { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> FlightVideoID { get; set; }
        public int PTid { get; set; }
        public Nullable<int> VideoTime { get; set; }
    }
}