using System;
using System.Threading;
using System.Threading.Tasks;

namespace AksenovaConveyorLib.Models
{
    public class Loader
    {
        private readonly int _refillAmount;

        public Loader(int refillAmount)
        {
            _refillAmount = refillAmount;
        }

        public async Task RefillAsync(Conveyor conveyor)
        {
            await Task.Delay(2000);
            conveyor.RefillMaterials(_refillAmount);
        }
    }
}
