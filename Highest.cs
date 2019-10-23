namespace ATAS.Indicators.Technical
{
	using System;
	using System.ComponentModel;

	using ATAS.Indicators.Properties;

	using Utils.Common.Localization;

	using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

	[DisplayName("Highest")]
	[Description( "Highest")]
	public class Highest : Indicator
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

		public Highest()
		{
			Period = 10;
		}

		protected override void OnCalculate(int bar, decimal value)
		{
			var start = Math.Max(0, bar - Period + 1);
			var count = Math.Min(bar + 1, Period);

			var max = (decimal)SourceDataSeries[start];

			for (var i = start + 1; i < start + count; i++)
				max = Math.Max(max, (decimal)SourceDataSeries[i]);

			this[bar] = max;
		}
	}
}