using AksenovaConveyorLib.Models;
using System;

namespace AksenovaConveyorLib.Models
{
    public class EmergencyMechanic : IMechanic
    {
        public string MechanicName => "Emergency Mechanic";

        public async void RepairConveyor(Conveyor conveyor)
        {
            await Task.Delay(1000);
            conveyor.Repair();
        }
    }
}
