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
    
    public partial class MSTR_Drone_Setup
    {
        public int DroneSetupId { get; set; }
        public int DroneId { get; set; }
        public Nullable<int> PilotUserId { get; set; }
        public Nullable<int> GroundStaffUserId { get; set; }
        public Nullable<decimal> BatteryVoltage { get; set; }
        public string Weather { get; set; }
        public string UasPhysicalCondition { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public Nullable<int> ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedOn { get; set; }
        public string NotificationEmails { get; set; }
    }
}
