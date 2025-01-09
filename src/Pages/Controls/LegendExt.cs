using Syncfusion.Maui.Toolkit.Charts;

namespace BalanceNalu.Pages.Controls;

public class LegendExt : ChartLegend
{
	protected override double GetMaximumSizeCoefficient()
	{
		return 0.5;
	}
}
