using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using ReeveUnionManager.Models;
using System.Text.Json;
using JetBrains.Annotations;

namespace ReeveUnionManager.ViewModels
{
    public class ChecklistViewModel : INotifyPropertyChanged
    {
        private readonly string _checklistFilePath;
        private readonly ChecklistType _checklistType;
        private double _progress;
        private string _progressText;

        public ObservableCollection<ChecklistItem> Items { get; set; }
        public string Title { get; set; }
        public double Progress
        {
            get => _progress;
            set
            {
                _progress = value;
                OnPropertyChanged();
            }
        }

        public string ProgressText
        {
            get => _progressText;
            set
            {
                _progressText = value;
                OnPropertyChanged();
            }
        }

        public ICommand AddTaskCommand { get; }
        public ICommand ResetAllCommand { get; }
        public ICommand DeleteTaskCommand { get; }
        public ICommand EditTaskCommand { get; }
        public ChecklistViewModel(ChecklistType checklistType)
        {
            _checklistType = checklistType;
            _checklistFilePath = Path.Combine(FileSystem.AppDataDirectory, $"{checklistType}_checklist.json");

            Title = checklistType == ChecklistType.Opening ? "Opening Checklist" : "Closing Checklist";
            // Load existing checklist or create default
            LoadChecklist();

            // Subscribe to property changes on all items
            foreach (var item in Items)
            {
                item.PropertyChanged += Item_PropertyChanged;
            }

            AddTaskCommand = new Command(OnAddTask);
            ResetAllCommand = new Command(OnResetAll);
            DeleteTaskCommand = new Command<ChecklistItem>(OnDeleteTask);
            EditTaskCommand = new Command<ChecklistItem>(OnEditTask);

            UpdateProgress();
        }

        private void LoadChecklist()
        {
            if (File.Exists(_checklistFilePath))
            {
                try
                {
                    string json = File.ReadAllText(_checklistFilePath);
                    var loadedItems = JsonSerializer.Deserialize<List<ChecklistItem>>(json);
                    Items = new ObservableCollection<ChecklistItem>(loadedItems ?? GetDefaultTasks());
                }
                catch
                {
                    Items = new ObservableCollection<ChecklistItem>(GetDefaultTasks());
                }
            }
            else
            {
                Items = new ObservableCollection<ChecklistItem>(GetDefaultTasks());
            }
        }

        private void SaveChecklist()
        {
            try
            {
                string json = JsonSerializer.Serialize(Items.ToList());
                File.WriteAllText(_checklistFilePath, json);
            }
            catch (Exception ex)
            {
                // Handle error - maybe show alert
                System.Diagnostics.Debug.WriteLine($"Error saving checklist: {ex.Message}");
            }
        }
        private List<ChecklistItem> GetDefaultTasks()
        {
            if (_checklistType == ChecklistType.Closing)
            {
                return new List<ChecklistItem>
                {
                    new ChecklistItem { TaskName = "Turn off all lights" },
                    new ChecklistItem { TaskName = "Lock main entrance doors" },
                    new ChecklistItem { TaskName = "Check restrooms and clean if necessary" },
                };
            }   
            // Default opening tasks
            return new List<ChecklistItem>
            {
                new ChecklistItem { TaskName = "Unlock main entrance doors" },
                new ChecklistItem { TaskName = "Turn on all lights" },
                new ChecklistItem { TaskName = "Check restrooms and restock supplies" },
            };
        }
        private void Item_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ChecklistItem.IsCompleted))
            {
                UpdateProgress();
                SaveChecklist();
            }
        }

        private void UpdateProgress()
        {
            int completed = Items.Count(item => item.IsCompleted);
            int total = Items.Count;

            Progress = total > 0 ? (double)completed / total : 0;
            ProgressText = $"{completed} of {total} completed";
        }

        private async void OnAddTask()
        {
            string result = await Application.Current.MainPage.DisplayPromptAsync(
                "Add Task",
                "Enter task name:",
                "Add",
                "Cancel");

            if (!string.IsNullOrWhiteSpace(result))
            {
                var newItem = new ChecklistItem { TaskName = result };
                newItem.PropertyChanged += Item_PropertyChanged;
                Items.Add(newItem);
                UpdateProgress();
                SaveChecklist();
            }
        }

        private void OnResetAll()
        {
            foreach (var item in Items)
            {
                item.IsCompleted = false;
            }
        }

        private async void OnDeleteTask(ChecklistItem item)
        {
            bool confirm = await Application.Current.MainPage.DisplayAlert(
                "Delete Task",
                $"Are you sure you want to delete this task?",
                "Delete",
                "Cancel");

            if (confirm)
            {
                item.PropertyChanged -= Item_PropertyChanged;
                Items.Remove(item);
                UpdateProgress();
                SaveChecklist();
            }
        }

        private async void OnEditTask(ChecklistItem item)
        {
            string result = await Application.Current.MainPage.DisplayPromptAsync(
                "Edit Task",
                "Update task name:",
                "Save",
                "Cancel",
                initialValue: item.TaskName);

            if (!string.IsNullOrWhiteSpace(result))
            {
                item.TaskName = result;
                SaveChecklist();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
public enum ChecklistType
{
    Opening,
    Closing
}