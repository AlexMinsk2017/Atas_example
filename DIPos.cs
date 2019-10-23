namespace ATAS.Indicators.Technical
{
	using System.ComponentModel;
	using System.Windows.Media;

	using ATAS.Indicators.Properties;

	using Utils.Common.Localization;

	using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

	[DisplayName("DI+")]
	[Description( "DIPos")]
	public class DIPos : Indicator
	{
		private readonly ATR _atr = new ATR();
		private readonly WMA _wma = new WMA();

		[Category( "Common")]
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

				_wma.Period = _atr.Period = value;
				RecalculateValues();
			}
		}

		public DIPos()
			: base(true)
		{
			Panel = IndicatorDataProvider.NewPanel;

			((ValueDataSeries)DataSeries[0]).Color = Colors.Blue;

			Period = 10;

			Add(_atr);
		}

		
		protected override void OnCalculate(int bar, decimal value)
		{
			if (bar > 0)
			{
				var atr = _atr[bar];

				var currentCandle = GetCandle(bar);
				var prevCandle = GetCandle(bar - 1);

				var val = currentCandle.High > prevCandle.High && currentCandle.High - prevCandle.High > prevCandle.Low - currentCandle.Low
					? currentCandle.High - prevCandle.High
					: 0m;

				var wma = _wma.Calculate(bar, val);

				this[bar] = atr != 0m ? 100m * wma / atr : 0m;
			}
			else
			{
				this[bar] = 0;
			}
		}
	}
}