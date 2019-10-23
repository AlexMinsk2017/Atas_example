namespace ATAS.Indicators.Technical
{
	using System;
	using System.ComponentModel;

	using ATAS.Indicators.Properties;

	using Utils.Common.Localization;

	using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

	[DisplayName("MeanDev")]
	[Description( "MeanDev")]
	public class MeanDev : Indicator
	{
		private readonly SMA _sma = new SMA();

		[Category( "Common")]
		[DisplayName( "Period")]
		[PropertyOrder(20)]
		[Parameter]
		public int Period
		{
			get => _sma.Period;
			set
			{
				if (value <= 0)
					return;

				_sma.Period = value;
				RecalculateValues();
			}
		}

		public MeanDev()
		{
			Panel = IndicatorDataProvider.NewPanel;

			//DataSeries.Add(_sma.DataSeries[0]);
			//((ValueDataSeries)DataSeries[1]).VisualType = VisualMode.Hide;

			Period = 10;
		}

		protected override void OnCalculate(int bar, decimal value)
		{
			var sma = _sma.Calculate(bar, value);

			var start = Math.Max(0, bar - Period + 1);
			var count = Math.Min(bar + 1, Period);

			var sum = 0m;

			for (var i = start; i < start + count; i++)
				sum += Math.Abs((decimal)SourceDataSeries[i] - sma);

			this[bar] = sum / count;
		}
	}
}