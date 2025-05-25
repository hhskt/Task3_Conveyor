using AksenovaConveyorLib.Models;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Input;

namespace AksenovaConveyorApp.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly Loader _loader;
        private readonly Random _random = new Random();
        private int _nextConveyorId = 1;

        public ObservableCollection<ConveyorViewModel> Conveyors { get; } = new ObservableCollection<ConveyorViewModel>();
        public ICommand AddConveyorCommand { get; }
        public ICommand StartAllCommand { get; }
        public ICommand StopAllCommand { get; }

        public event PropertyChangedEventHandler PropertyChanged;

        public MainViewModel()
        {
            _loader = new Loader(40);
            AddConveyorCommand = new RelayCommand(AddConveyor);
            StartAllCommand = new RelayCommand(async () => await StartAllAsync());
            StopAllCommand = new RelayCommand(StopAll);

            AddConveyor();
        }

        private void AddConveyor()
        {
            var mechanicTypes = Assembly.GetAssembly(typeof(IMechanic))
                .GetTypes()
                .Where(t => typeof(IMechanic).IsAssignableFrom(t) && !t.IsInterface)
                .ToArray();
            var mechanicType = mechanicTypes[_random.Next(mechanicTypes.Length)];
            var mechanic = (IMechanic)Activator.CreateInstance(mechanicType);

            var conveyorVm = new ConveyorViewModel(_nextConveyorId++, 40, _loader, mechanic);
            Conveyors.Add(conveyorVm);
        }

        private async Task StartAllAsync()
        {
            var tasks = Conveyors.Select(conveyor => conveyor.StartAsync()).ToArray();
            await Task.WhenAll(tasks);
        }

        private void StopAll()
        {
            foreach (var conveyor in Conveyors)
            {
                conveyor.Stop();
            }
        }
    }

    public class RelayCommand : ICommand
    {
        private readonly Action _execute;
        private readonly Func<bool> _canExecute;

        public RelayCommand(Action execute, Func<bool> canExecute = null)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object parameter) => _canExecute?.Invoke() ?? true;
        public void Execute(object parameter) => _execute();
    }

}
