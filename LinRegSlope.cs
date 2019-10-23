namespace ATAS.Indicators.Technical
{
	using System.ComponentModel;

	using ATAS.Indicators.Properties;

	using Utils.Common.Localization;

	using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

	[DisplayName("LinRegSlope")]
	[Description("Linear Regression Slope")]
	public class LinRegSlope : Indicator
	{
		private int _period;

		[Category( "Common")]
		[DisplayName( "Period")]
		[PropertyOrder(20)]
		[Parameter]
		public int Period
		{
			get => _period;
			set
			{
				if (_period == value)
					return;

				if (value <= 1)
					return;

				_period = value;

				RaisePropertyChanged("Period");
				RecalculateValues();
			}
		}

		public LinRegSlope()
		{
			Panel = IndicatorDataProvider.NewPanel;
			Period = 14;
		}

		protected override void OnCalculate(int bar, decimal value)
		{
			if (bar < Period + 1)
				return;
			var sumX = Period * (Period - 1) * 0.5m;
			var divisor = sumX * sumX - Period * Period * (Period - 1m) * (2 * Period - 1) / 6;
			decimal sumXY = 0;
			for (var count = 0; count < Period && bar - count >= 0; count++)
			{
				if (bar - count < 0)
					continue;
				sumXY += count * (decimal)SourceDataSeries[bar - count];
			}

			var val = (Period * sumXY - sumX * SourceDataSeries.CalcSum(Period, bar)) / divisor;
			this[bar] = val;
		}
	}
}