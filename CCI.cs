namespace ATAS.Indicators.Technical
{
	using System;
	using System.ComponentModel;

	using ATAS.Indicators.Properties;

	using Utils.Common.Localization;

	using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

	[DisplayName("CCI")]
	[Description( "CCI")]
	public class CCI : Indicator
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

		public CCI()
			: base(true)
		{
			Panel = IndicatorDataProvider.NewPanel;
			Period = 10;
		}

		protected override void OnCalculate(int bar, decimal value)
		{
			var candle = GetCandle(bar);
			decimal mean = 0;
			var typical = (candle.High + candle.Low + candle.Close) / 3m;
			var sma0 = _sma.Calculate(bar, typical);
			for (var i = bar; i > bar - Period && i >= 0; i--)
			{
				var candleI = GetCandle(i);
				var typ = (candleI.High + candleI.Low + candleI.Close) / 3m;
				mean += Math.Abs(typ - sma0);
			}
			mean = 0.015m * (mean / Math.Min(Period, bar + 1));
			this[bar] = (typical - sma0) / (mean <= 0.000000001m ? 1 : mean);
		}
	}
}