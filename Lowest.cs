namespace ATAS.Indicators.Technical
{
	using System;
	using System.ComponentModel;

	using ATAS.Indicators.Properties;

	using Utils.Common.Localization;

	using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

	[DisplayName("Lowest")]
	[Description( "Lowest")]
	public class Lowest : Indicator
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
				if (value <= 0)
					return;

				_period = value;
				RecalculateValues();
			}
		}

		public Lowest()
		{
			Period = 10;
		}

		protected override void OnCalculate(int bar, decimal value)
		{
			var start = Math.Max(0, bar - Period + 1);
			var count = Math.Min(bar + 1, Period);

			var min = (decimal)SourceDataSeries[start];

			for (var i = start + 1; i < start + count; i++)
				min = Math.Min(min, (decimal)SourceDataSeries[i]);

			this[bar] = min;
		}
	}
}