using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using BalanceNalu.Data;
using BalanceNalu.Models;
using BalanceNalu.Services;

namespace BalanceNalu.PageModels;

public partial class ManageMetaPageModel(INavigationService _navigationService, CategoryRepository _categoryRepository, TagRepository _tagRepository, SeedDataService _seedDataService) : ObservableObject
{
	[ObservableProperty]
	private ObservableCollection<Category> _categories = [];

	[ObservableProperty]
	private ObservableCollection<Tag> _tags = [];

	private async Task LoadData()
	{
		var categoriesList = await _categoryRepository.ListAsync();
		Categories = new ObservableCollection<Category>(categoriesList);
		var tagsList = await _tagRepository.ListAsync();
		Tags = new ObservableCollection<Tag>(tagsList);
	}

	[RelayCommand]
	private Task Appearing()
		=> LoadData();

	[RelayCommand]
	private async Task SaveCategories()
	{
		foreach (var category in Categories)
		{
			await _categoryRepository.SaveItemAsync(category);
		}

		await AppShell.DisplayToastAsync("Categories saved");
	}

	[RelayCommand]
	private async Task DeleteCategory(Category category)
	{
		Categories.Remove(category);
		await _categoryRepository.DeleteItemAsync(category);
		await AppShell.DisplayToastAsync("Category deleted");
	}

	[RelayCommand]
	private async Task AddCategory()
	{
		var category = new Category();
		Categories.Add(category);
		await _categoryRepository.SaveItemAsync(category);
		await AppShell.DisplayToastAsync("Category added");
	}

	[RelayCommand]
	private async Task SaveTags()
	{
		foreach (var tag in Tags)
		{
			await _tagRepository.SaveItemAsync(tag);
		}

		await AppShell.DisplayToastAsync("Tags saved");
	}

	[RelayCommand]
	private async Task DeleteTag(Tag tag)
	{
		Tags.Remove(tag);
		await _tagRepository.DeleteItemAsync(tag);
		await AppShell.DisplayToastAsync("Tag deleted");
	}

	[RelayCommand]
	private async Task AddTag()
	{
		var tag = new Tag();
		Tags.Add(tag);
		await _tagRepository.SaveItemAsync(tag);
		await AppShell.DisplayToastAsync("Tag added");
	}

	[RelayCommand]
	private async Task Reset()
	{
		Preferences.Default.Remove("is_seeded");
		await _navigationService.GoToAsync(Navigation.Absolute().ShellContent<MainPageModel>());
	}
}