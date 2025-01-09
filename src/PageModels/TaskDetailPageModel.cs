using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using BalanceNalu.Data;
using BalanceNalu.Models;
using BalanceNalu.Services;

namespace BalanceNalu.PageModels;

public record TaskDetailIntent(Project? Project, int TaskId);

public partial class TaskDetailPageModel(ProjectRepository _projectRepository, TaskRepository _taskRepository, ModalErrorHandler _errorHandler, INavigationService _navigationService) : ObservableObject, IEnteringAware<TaskDetailIntent>
{
	public const string ProjectQueryKey = "project";
	private ProjectTask? _task;
	private bool _canDelete;
	
	[ObservableProperty]
	private string _title = string.Empty;

	[ObservableProperty]
	private bool _isCompleted;

	[ObservableProperty]
	private List<Project> _projects = [];

	[ObservableProperty]
	private Project? _project;

	[ObservableProperty]
	private int _selectedProjectIndex = -1;


	[ObservableProperty]
	private bool _isExistingProject;

	private async Task LoadTaskAsync(Project project, int taskId)
	{
		Project = project;

		if (taskId > 0)
		{
			_task = await _taskRepository.GetAsync(taskId);

			if (_task is null)
			{
				_errorHandler.HandleError(new Exception($"Task Id {taskId} isn't valid."));
				return;
			}

			Project = await _projectRepository.GetAsync(_task.ProjectID);
		}
		else
		{
			_task = new ProjectTask();
		}

		// If the project is new, we don't need to load the project dropdown
		if (Project?.ID == 0)
        {
            IsExistingProject = false;
		}
		else
        {
            Projects = await _projectRepository.ListAsync();
            IsExistingProject = true;
		}

		if (Project is not null)
			SelectedProjectIndex = Projects.FindIndex(p => p.ID == Project.ID);
		else if (_task?.ProjectID > 0)
			SelectedProjectIndex = Projects.FindIndex(p => p.ID == _task.ProjectID);

		if (taskId > 0)
		{
			if (_task is null)
			{
				_errorHandler.HandleError(new Exception($"Task with id {taskId} could not be found."));
				return;
			}

			Title = _task.Title;
			IsCompleted = _task.IsCompleted;
			CanDelete = true;
		}
		else
		{
			_task = new ProjectTask()
			{
				ProjectID = Project?.ID ?? 0
			};
		}
	}

	public bool CanDelete
	{
		get => _canDelete;
		set
		{
			_canDelete = value;
			DeleteCommand.NotifyCanExecuteChanged();
		}
	}

	[RelayCommand]
	private async Task Save()
	{
		if (_task is null)
		{
			_errorHandler.HandleError(
				new Exception("Task or project is null. The task could not be saved."));

			return;
		}

		_task.Title = Title;

		int projectId = Project?.ID ?? 0;

		if (Projects.Count > SelectedProjectIndex && SelectedProjectIndex >= 0)
			_task.ProjectID = projectId = Projects[SelectedProjectIndex].ID;

		_task.IsCompleted = IsCompleted;

		if (Project?.ID == projectId && !Project.Tasks.Contains(_task))
			Project.Tasks.Add(_task);

		if (_task.ProjectID > 0)
			_ = _taskRepository.SaveItemAsync(_task);

		await _navigationService.GoToAsync(Navigation.Relative().Pop());

		if (_task.ID > 0)
			await AppShell.DisplayToastAsync("Task saved");
	}

	[RelayCommand(CanExecute = nameof(CanDelete))]
	private async Task Delete()
	{
		if (_task is null || Project is null)
		{
			_errorHandler.HandleError(
				new Exception("Task is null. The task could not be deleted."));

			return;
		}

		if (Project.Tasks.Contains(_task))
			Project.Tasks.Remove(_task);

		if (_task.ID > 0)
			await _taskRepository.DeleteItemAsync(_task);

		await _navigationService.GoToAsync(Navigation.Relative().Pop());
		await AppShell.DisplayToastAsync("Task deleted");
	}

	public async ValueTask OnEnteringAsync(TaskDetailIntent intent)
    {
		LoadTaskAsync(intent.Project, intent.TaskId).FireAndForgetSafeAsync(_errorHandler);
    }
}