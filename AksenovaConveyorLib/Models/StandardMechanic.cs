using AksenovaConveyorLib.Models;
using System;

namespace AksenovaConveyorLib.Models
{
    public class StandardMechanic : IMechanic
    {
        public string MechanicName => "Standard Mechanic";

        public async void RepairConveyor(Conveyor conveyor)
        {
            await Task.Delay(3000);
            conveyor.Repair();
        }
    }
}
