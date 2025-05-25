using AksenovaConveyorLib.Models;
using System;

namespace AksenovaConveyorLib.Models
{
    public class AdvancedMechanic : IMechanic
    {
        public string MechanicName => "Advanced Mechanic";

        public async void RepairConveyor(Conveyor conveyor)
        {
            await Task.Delay(1500);
            conveyor.Repair();
        }
    }
}
