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
    
    public partial class M2M_DroneServiceParts
    {
        public int Id { get; set; }
        public Nullable<int> PartsGroupId { get; set; }
        public string Description { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<int> ModifiedBy { get; set; }
        public Nullable<int> ApprovedBy { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public Nullable<System.DateTime> ModifiedOn { get; set; }
        public Nullable<System.DateTime> ApprovedOn { get; set; }
        public string Remarks { get; set; }
        public Nullable<int> RecordType { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public Nullable<int> ServiceId { get; set; }
        public Nullable<int> PartsId { get; set; }
        public string ServicePartsType { get; set; }
        public Nullable<int> QtyCount { get; set; }
    
        public virtual M2M_DroneServiceParts M2M_DroneServiceParts1 { get; set; }
        public virtual M2M_DroneServiceParts M2M_DroneServiceParts2 { get; set; }
    }
}
