namespace ATAS.Indicators.Technical
{
	using System.ComponentModel;
	using System.Windows.Media;

	using ATAS.Indicators.Properties;

	using Utils.Common.Localization;

	using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

	[DisplayName("WPR")]
	[Description("Williams’ Percent Range")]
	public class WPR : Indicator
	{
		private readonly Highest _highest = new Highest();
		private readonly Lowest _lowest = new Lowest();
		private int _period = 14;
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

				_period = _highest.Period=_lowest.Period= value;
				RecalculateValues();
			}
		}

		public WPR()
			: base(true)
		{
			Panel = IndicatorDataProvider.NewPanel;
			Period = 14;
			LineSeries.Add(new LineSeries("-20") { Color = Colors.Gray, Width = 1,LineDashStyle = LineDashStyle.Dot,Value = -20});
			LineSeries.Add(new LineSeries("-80") { Color = Colors.Gray, Width = 1, LineDashStyle = LineDashStyle.Dot, Value = -80 });
		}


		protected override void OnCalculate(int bar, decimal value)
		{
			var candle = GetCandle(bar);

			var highest = _highest.Calculate(bar, candle.High);
			var lowest = _lowest.Calculate(bar, candle.Low);

			if(highest - lowest!=0)
				this[bar] = -100 * (highest - candle.Close) / (highest - lowest);
			else
			{
				this[bar] = bar > 0 ? this[bar - 1] : 0;
			}
		}
	}
}