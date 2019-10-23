namespace ATAS.Indicators.Technical
{
	using System.ComponentModel;

	using ATAS.Indicators.Properties;

	using CustomIndicatorSet.Properties;

	using Utils.Common.Localization;

	[DisplayName("AD")]
	[Description( "AD_Description")]
	public class AD : Indicator
	{
		public AD()
			: base(true)
		{
			Panel = IndicatorDataProvider.NewPanel;
		}

		protected override void OnCalculate(int bar, decimal value)
		{
			var candle = GetCandle(bar);
			var prev = bar == 0 ? 0m : this[bar - 1];

			var diff = candle.High - candle.Low;

			this[bar] = diff == 0
				? prev
				: candle.Close - candle.Low - (candle.High - candle.Close) * candle.Volume / diff + prev;
		}
	}
}