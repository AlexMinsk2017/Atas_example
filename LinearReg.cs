namespace ATAS.Indicators.Technical
{
	using System;
	using System.ComponentModel;

	using ATAS.Indicators.Properties;

	using Utils.Common.Localization;

	using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

	[DisplayName("LinearReg")]
	[Description( "LinearReg")]
	public class LinearReg : Indicator
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

		public LinearReg()
		{
			Period = 10;
		}

		protected override void OnCalculate(int bar, decimal value)
		{
			var start = Math.Max(0, bar - Period + 1);
			var count = Math.Min(bar + 1, Period);

			var x = 0m;
			var y = 0m;
			var xy = 0m;
			var x2 = 0m;

			for (var i = start; i < start + count; i++)
			{
				var val = (decimal)SourceDataSeries[i];

				x += i;
				x2 += i * i;

				y += val;
				xy += i * val;
			}

			var k = count * x2 - x * x;

			k = k == 0
				? 0
				: (count * xy - x * y) / k;

			this[bar] = k * bar + (y - k * x) / count;
		}
	}
}