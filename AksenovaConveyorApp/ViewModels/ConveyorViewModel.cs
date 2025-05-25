using AksenovaConveyorLib.Models;

using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;

namespace AksenovaConveyorApp.ViewModels
{
    public class ConveyorViewModel : INotifyPropertyChanged
    {
        private readonly Conveyor _conveyor;
        private readonly Loader _loader;
        private readonly IMechanic _mechanic;
        private CancellationTokenSource _cts;
        private double _position;
        private string _status;
        private bool _triggerAnimation;

        public event PropertyChangedEventHandler PropertyChanged;

        public ConveyorViewModel(int id, int initialMaterials, Loader loader, IMechanic mechanic)
        {
            _conveyor = new Conveyor(id, initialMaterials);
            _loader = loader;
            _mechanic = mechanic;
            _cts = new CancellationTokenSource();
            _status = "Stopped";
            _position = 0;
            _triggerAnimation = false;

            _conveyor.MaterialDepleted += async (s, e) =>
            {
                Status = e.Message;
                await _loader.RefillAsync(_conveyor);
                await StartAsync();
            };
            _conveyor.ConveyorBroken += (s, e) =>
            {
                Status = $"{e.Message} Repairing by {MechanicName}...";
                _mechanic.RepairConveyor(_conveyor);
            };
            _conveyor.ConveyorRepaired += async (s, e) =>
            {
                Status = $"{e.Message} Repaired by {MechanicName}";
                await StartAsync();
            };
            _conveyor.MaterialProcessed += (s, e) =>
            {
                Status = e.Message;
                Position = 0;
                TriggerAnimation = true;

                Task.Delay(550).ContinueWith(_ =>
                {
                    if (IsRunning)
                        TriggerAnimation = false;
                });
            };
        }

        public int Id => _conveyor.Id;

        public double Position
        {
            get => _position;
            set
            {
                _position = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Position)));
            }
        }

        public string Status
        {
            get => _status;
            set
            {
                _status = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Status)));
            }
        }

        public bool IsRunning
        {
            get => _conveyor.IsRunning;
            set
            {
                if (_conveyor.IsRunning != value)
                {
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsRunning)));
                }
            }
        }

        public string MechanicName => _mechanic.MechanicName;

        public bool TriggerAnimation
        {
            get => _triggerAnimation;
            set
            {
                _triggerAnimation = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(TriggerAnimation)));
            }
        }

        public async Task StartAsync()
        {
            _cts = new CancellationTokenSource();
            Status = "Running";
            await _conveyor.StartAsync(_cts.Token);
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsRunning)));
        }

        public void Stop()
        {
            _cts.Cancel();
            _conveyor.Stop();
            Status = "Stopped";
            TriggerAnimation = false;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsRunning)));
        }
    }
}
