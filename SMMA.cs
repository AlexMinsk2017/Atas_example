namespace ATAS.Indicators.Technical
{
	using System.ComponentModel;

	using ATAS.Indicators.Properties;

	using Utils.Common.Localization;

	using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

	[DisplayName("SMMA")]
	[Description( "SMMA")]
	public class SMMA : Indicator
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
				if (value <= 0)
					return;

				_period = value;
				RecalculateValues();
			}
		}

		public SMMA()
		{
			Period = 10;
		}

		protected override void OnCalculate(int bar, decimal value)
		{
			if (bar == 0)
				this[bar] = value;
			else
				this[bar] = (this[bar - 1] * (Period - 1) + value) / Period;
		}
	}
}