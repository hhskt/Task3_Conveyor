using System;
using System.Threading;
using System.Threading.Tasks;

namespace AksenovaConveyorLib.Models
{
    public class Conveyor
    {
        private readonly Random _random = new Random();
        private int _materialCount;
        private bool _isRunning;
        private bool _isBroken;
        private readonly int _id;

        public delegate void ConveyorEventHandler(object sender, ConveyorEventArgs e);
        public event ConveyorEventHandler MaterialDepleted;
        public event ConveyorEventHandler ConveyorBroken;
        public event ConveyorEventHandler ConveyorRepaired;
        public event ConveyorEventHandler MaterialProcessed;

        public Conveyor(int id, int initialMaterials)
        {
            _id = id;
            _materialCount = initialMaterials;
            _isRunning = false;
            _isBroken = false;
        }

        public int Id => _id;
        public int MaterialCount => _materialCount;
        public bool IsRunning => _isRunning;
        public bool IsBroken => _isBroken;

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _isRunning = true;
            await Task.Run(async () =>
            {
                while (_isRunning && !cancellationToken.IsCancellationRequested)
                {
                    if (_isBroken)
                    {
                        _isRunning = false;
                        ConveyorBroken?.Invoke(this, new ConveyorEventArgs(_id, "Conveyor broke down!"));
                        return;
                    }

                    if (_materialCount <= 0)
                    {
                        _isRunning = false;
                        MaterialDepleted?.Invoke(this, new ConveyorEventArgs(_id, "Materials depleted!"));
                        return;
                    }

                    _materialCount--;
                    MaterialProcessed?.Invoke(this, new ConveyorEventArgs(_id, $"Processed material. Remaining: {_materialCount}"));

                    if (_random.NextDouble() < 0.05)
                    {
                        _isBroken = true;
                        continue;
                    }

                    await Task.Delay(600);
                }
            }, cancellationToken);
        }

        public void Stop()
        {
            _isRunning = false;
        }

        public void RefillMaterials(int amount)
        {
            _materialCount += amount;
            MaterialProcessed?.Invoke(this, new ConveyorEventArgs(_id, $"Refilled {amount} materials. Total: {_materialCount}"));
        }

        public void Repair()
        {
            _isBroken = false;
            ConveyorRepaired?.Invoke(this, new ConveyorEventArgs(_id, "Conveyor repaired!"));
        }
    }

    public class ConveyorEventArgs : EventArgs
    {
        public int ConveyorId { get; }
        public string Message { get; }

        public ConveyorEventArgs(int conveyorId, string message)
        {
            ConveyorId = conveyorId;
            Message = message;
        }
    }
}
