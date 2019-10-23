namespace ATAS.Indicators.Technical
{
	using System;
	using System.ComponentModel;

	using ATAS.Indicators.Properties;

	using Utils.Common.Localization;

	using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

	[DisplayName("ATR")]
	[Description( "ATR")]
	public class ATR : Indicator
	{
		private readonly WMA _wma = new WMA();

		[Category("Common")]
		[DisplayName( "Period")]
		[PropertyOrder(20)]
		[Parameter]
		public int Period
		{
			get => _wma.Period;
			set
			{
				if (value <= 0)
					return;

				_wma.Period = value;
				RecalculateValues();
			}
		}

		public ATR()
			: base(true)
		{
			Panel = IndicatorDataProvider.NewPanel;
			Period = 10;
			((ValueDataSeries)DataSeries[0]).StringFormat = StringFormat;
		}

		protected override void OnCalculate(int bar, decimal value)
		{
			var candle = GetCandle(bar);

			if (bar > 0)
			{
				var last = GetCandle(bar - 1);

				var v1 = Math.Abs(candle.High - candle.Low);
				var v2 = Math.Abs(last.Close - candle.High);
				var v3 = Math.Abs(last.Close - candle.Low);

				this[bar] = _wma.Calculate(bar, Math.Max(v1, Math.Max(v2, v3)));
			}
			else
			{
				this[bar] = candle.High - candle.Low;
				((ValueDataSeries)DataSeries[0]).StringFormat = StringFormat;
			}
		}
	}
}