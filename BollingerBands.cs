namespace ATAS.Indicators.Technical
{
	using System.ComponentModel;
	using System.Windows.Media;

	using ATAS.Indicators.Properties;

	using Utils.Common.Localization;

	using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

	[DisplayName("BollingerBands")]
	[Description( "BollingerBands")]
	public class BollingerBands : Indicator
	{
		private readonly RangeDataSeries _band = new RangeDataSeries("BackGround");
		private readonly StdDev _dev = new StdDev();

		private readonly SMA _sma = new SMA();
		private decimal _width;

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

				_sma.Period = _dev.Period = value;
				RecalculateValues();
			}
		}

		[Category( "Common")]
		[DisplayName( "BBandsWidth")]
		[PropertyOrder(22)]
		[Parameter]
		public decimal Width
		{
			get => _width;
			set
			{
				if (value <= 0)
					return;

				_width = value;
				RecalculateValues();
			}
		}

		public BollingerBands()
		{
			((ValueDataSeries)DataSeries[0]).Color = Colors.Green;

			DataSeries.Add(new ValueDataSeries("Up")
			{
				VisualType = VisualMode.Line
			});
			DataSeries.Add(new ValueDataSeries("Down")
			{
				VisualType = VisualMode.Line
			});

			DataSeries.Add(_band);
			Period = 10;
			Width = 1;
		}

		protected override void OnCalculate(int bar, decimal value)
		{
			var sma = _sma.Calculate(bar, value);
			var dev = _dev.Calculate(bar, value);

			this[bar] = sma;

			DataSeries[1][bar] = sma + dev * Width;
			DataSeries[2][bar] = sma - dev * Width;

			_band[bar].Upper = sma + dev * Width;
			_band[bar].Lower = sma - dev * Width;
		}
	}
}