﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkyCommanderProcessor.Models;

namespace SkyCommanderProcessor.EXLogic {

  public class ExtDroneFight {
    public String DroneName { get; set; }
    public String AccountName { get; set; }
    public int AccountID { get; set; }
    public Double Altitude { get; set; } = 0;
    public DateTime LastFlightDate { get; set; }
    public int LastProcessedID { get; set; } = 0;
    public String PilotName { get; set; }
    private DroneFlight Base;

    public ExtDroneFight(DroneFlight BaseFlight) {
      Base = BaseFlight;
    }

    public DroneFlight GetBase() {
      return Base;
    }

  }
  
}
