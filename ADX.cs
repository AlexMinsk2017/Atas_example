namespace ATAS.Indicators.Technical
{
	using System.ComponentModel;
	using System.Windows.Media;

	using ATAS.Indicators.Properties;

	using Utils.Common.Localization;

	using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

	[DisplayName("ADX")]
	[Description( "ADX")]
	public class ADX : Indicator
	{
		private readonly DX _dx = new DX();
		private readonly WMA _sma = new WMA();

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

				_sma.Period = _dx.Period = value;
				RecalculateValues();
			}
		}

		public ADX()
			: base(true)
		{
			Panel = IndicatorDataProvider.NewPanel;

			((ValueDataSeries)DataSeries[0]).Color = Colors.Green;

			DataSeries.Add(_dx.DataSeries[1]);
			DataSeries.Add(_dx.DataSeries[2]);

			Period = 10;

			Add(_dx);
		}

		protected override void OnCalculate(int bar, decimal value)
		{
			this[bar] = _sma.Calculate(bar, _dx[bar]);
		}
	}
}