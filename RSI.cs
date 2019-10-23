namespace ATAS.Indicators.Technical
{
	using System.ComponentModel;
	using System.Windows.Media;

	using ATAS.Indicators.Properties;

	using Utils.Common.Localization;

	using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

	[DisplayName("RSI")]
	[Description( "RSI")]
	public class RSI : Indicator
	{
		private readonly SMMA _negative;
		private readonly SMMA _positive;

		[Category( "Common")]
		[DisplayName( "Period")]
		[PropertyOrder(20)]
		[Parameter]
		public int Period
		{
			get => _positive.Period;
			set
			{
				if (value <= 0)
					return;

				_positive.Period = _negative.Period = value;
				RecalculateValues();
			}
		}

		public RSI()
		{
			Panel = IndicatorDataProvider.NewPanel;

			LineSeries.Add(new LineSeries("Down")
			{
				Color = Colors.Orange,
				LineDashStyle = LineDashStyle.Dash,
				Value = 30,
				Width = 1
			});
			LineSeries.Add(new LineSeries("Up")
			{
				Color = Colors.Orange,
				LineDashStyle = LineDashStyle.Dash,
				Value = 70,
				Width = 1
			});

			_positive = new SMMA();
			_negative = new SMMA();

			Period = 10;
		}

		protected override void OnCalculate(int bar, decimal value)
		{
			if (bar == 0)
			{
				this[bar] = 0;
			}
			else
			{
				var diff = (decimal)SourceDataSeries[bar] - (decimal)SourceDataSeries[bar - 1];
				var pos = _positive.Calculate(bar, diff > 0 ? diff : 0);
				var neg = _negative.Calculate(bar, diff < 0 ? -diff : 0);

				if (neg != 0)
				{
					var div = pos / neg;

					this[bar] = div == 1
						? 0m
						: 100m - 100m / (1m + div);
				}
				else
				{
					this[bar] = 100m;
				}
			}
		}
	}
}