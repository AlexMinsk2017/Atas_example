namespace ATAS.Indicators.Technical
{
	using System;
	using System.ComponentModel;

	using ATAS.Indicators.Properties;

	using Utils.Common.Localization;

	using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

	[DisplayName("WMA")]
	[Description( "WMA")]
	public class WMA : Indicator
	{
		private int _period;

		[Category( "Common")]
		[DisplayName( "Period")]
		[PropertyOrder(20)]
		[Parameter]
		public int Period
		{
			get { return _period; }
			set
			{
				if (value <= 0)
					return;

				_period = value;
				RecalculateValues();
			}
		}

		public WMA()
		{
			Period = 10;
		}

		protected override void OnCalculate(int bar, decimal value)
		{
			if (bar > 0)
			{
				var count = Math.Min(bar + 1, Period);
				var current = this[bar - 1];

				this[bar] = (current * (count - 1) + value) / count;
			}
			else
				this[bar] = value;
		}
	}
}