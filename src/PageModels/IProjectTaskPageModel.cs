using CommunityToolkit.Mvvm.Input;
using BalanceNalu.Models;

namespace BalanceNalu.PageModels;

public interface IProjectTaskPageModel
{
	IAsyncRelayCommand<ProjectTask> NavigateToTaskCommand { get; }
	bool IsBusy { get; }
}