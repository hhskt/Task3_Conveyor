using AksenovaConveyorLib.Models;
using System;

namespace AksenovaConveyorLib.Models
{
    public interface IMechanic
    {
        void RepairConveyor(Conveyor conveyor);
        string MechanicName { get; }
    }
}
