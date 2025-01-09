using BalanceNalu.Models;
using BalanceNalu.PageModels;

namespace BalanceNalu.Pages;

public partial class MainPage : ContentPage
{
	public MainPage(MainPageModel model)
	{
		InitializeComponent();
		BindingContext = model;
	}
}