namespace ATAS.Indicators.Technical
{
	using System.ComponentModel;

	using ATAS.Indicators.Properties;

	using Utils.Common.Localization;

	using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

	[DisplayName("EMA")]
	[Description( "EMA")]
	public class EMA : Indicator
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
				if (_period == value)
					return;

				if (value <= 0)
					return;

				_period = value;

				RaisePropertyChanged("Period");
				RecalculateValues();
			}
		}

		public EMA()
		{
			Period = 10;
		}

		protected override void OnCalculate(int bar, decimal value)
		{
			if (bar == 0)
				this[bar] = value;
			else
			{
				this[bar] = value * (2.0m / (1 + Period)) + (1 - (2.0m / (1 + Period))) * this[bar - 1];
			}
		}
	}
}