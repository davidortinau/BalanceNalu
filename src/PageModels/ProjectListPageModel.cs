#nullable disable
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using BalanceNalu.Data;
using BalanceNalu.Models;
using BalanceNalu.Services;

namespace BalanceNalu.PageModels;

public partial class ProjectListPageModel(INavigationService _navigationService, ProjectRepository _projectRepository) : ObservableObject
{
	[ObservableProperty]
	private List<Project> _projects = [];

	
	[RelayCommand]
	private async Task Appearing()
	{
		Projects = await _projectRepository.ListAsync();
	}

	[RelayCommand]
	Task NavigateToProject(Project project)
		=> _navigationService.GoToAsync(Navigation.Relative().Push<ProjectDetailPageModel>().WithIntent(project.ID));

	[RelayCommand]
	Task AddProject() 
		=> _navigationService.GoToAsync(Navigation.Relative().Push<ProjectDetailPageModel>());
}