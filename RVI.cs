namespace ATAS.Indicators.Technical
{
	using System.ComponentModel;
	using System.Windows.Media;

	using ATAS.Indicators.Properties;

	using Utils.Common.Localization;

	[DisplayName("RVI")]
	[Description( "RVI")]
	public class RVI : Indicator
	{
		public RVI()
			: base(true)
		{
			Panel = IndicatorDataProvider.NewPanel;

			((ValueDataSeries)DataSeries[0]).Color = Colors.Green;

			DataSeries.Add(new ValueDataSeries("Signal")
			{
				VisualType = VisualMode.Line
			});
		}

		protected override void OnCalculate(int bar, decimal value)
		{
			if (bar > 3)
			{
				var c0 = GetCandle(bar - 3);
				var c1 = GetCandle(bar - 2);
				var c2 = GetCandle(bar - 1);
				var c3 = GetCandle(bar - 0);

				var valueNum = (c0.Close - c0.Open + 2 * (c1.Close - c1.Open) + 2 * (c2.Close - c2.Open) + (c3.Close - c3.Open)) / 6m;
				var valueDenum = (c0.High - c0.Low + 2 * (c1.High - c1.Low) + 2 * (c2.High - c2.Low) + (c3.High - c3.Low)) / 6m;

				DataSeries[0][bar] = valueDenum != 0 ? valueNum / valueDenum : valueNum;
				DataSeries[1][bar] = (this[bar - 3] + 2 * this[bar - 2] + 2 * this[bar - 1] + this[bar]) / 6m;
			}
			else
			{
				DataSeries[0][bar] = 0m;
				DataSeries[1][bar] = 0m;
			}
		}
	}
}