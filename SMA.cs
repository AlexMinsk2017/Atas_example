namespace ATAS.Indicators.Technical
{
	using System.ComponentModel;

	using ATAS.Indicators.Properties;

	using Utils.Common.Localization;

	using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

	[DisplayName("SMA")]
	[Description( "SMA")]
	public class SMA : Indicator
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

		public SMA()
		{
			Period = 10;
		}

		protected override void OnCalculate(int bar, decimal value)
		{
			this[bar] = SourceDataSeries.CalcAverage(Period, bar);
		}
	}
}