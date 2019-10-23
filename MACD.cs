namespace ATAS.Indicators.Technical
{
	using System.ComponentModel;
	using System.Windows.Media;

	using ATAS.Indicators.Properties;

	using Utils.Common.Localization;

	using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

	[DisplayName("MACD")]
	[Description( "MACD")]
	public class MACD : Indicator
	{
		private readonly EMA _long = new EMA();
		private readonly EMA _short = new EMA();
		private readonly SMA _sma = new SMA();

		[Category( "Common")]
		[DisplayName( "LongPeriod")]
		[PropertyOrder(20)]
		[Parameter]
		public int LongPeriod
		{
			get => _long.Period;
			set
			{
				if (value <= 0)
					return;

				_long.Period = value;
				RecalculateValues();
			}
		}

		[Category( "Common")]
		[DisplayName( "ShortPeriod")]
		[PropertyOrder(20)]
		[Parameter]
		public int ShortPeriod
		{
			get => _short.Period;
			set
			{
				if (value <= 0)
					return;

				_short.Period = value;
				RecalculateValues();
			}
		}

		[Category( "Common")]
		[DisplayName( "SignalPeriod")]
		[PropertyOrder(20)]
		[Parameter]
		public int SignalPeriod
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

		public MACD()
		{
			Panel = IndicatorDataProvider.NewPanel;

			((ValueDataSeries)DataSeries[0]).VisualType = VisualMode.Histogram;
			((ValueDataSeries)DataSeries[0]).Color = Colors.CadetBlue;

			DataSeries.Add(new ValueDataSeries("Signal")
			{
				VisualType = VisualMode.Line,
				LineDashStyle = LineDashStyle.Dash
			});

			LongPeriod = 26;
			ShortPeriod = 12;
			SignalPeriod = 9;
		}

		protected override void OnCalculate(int bar, decimal value)
		{
			var macd = _short.Calculate(bar, value) - _long.Calculate(bar, value);
			var signal = _sma.Calculate(bar, macd);

			this[bar] = macd;
			DataSeries[1][bar] = signal;
		}
	}
}