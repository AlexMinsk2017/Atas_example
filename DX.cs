namespace ATAS.Indicators.Technical
{
	using System;
	using System.ComponentModel;

	using ATAS.Indicators.Properties;

	using Utils.Common.Localization;

	using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

	[DisplayName("DX")]
	[Description( "DX")]
	public class DX : Indicator
	{
		private readonly DINeg _diNeg = new DINeg();
		private readonly DIPos _diPos = new DIPos();

		[Category( "Common")]
		[DisplayName( "Period")]
		[PropertyOrder(20)]
		[Parameter]
		public int Period
		{
			get => _diPos.Period;
			set
			{
				if (value <= 0)
					return;

				_diPos.Period = _diNeg.Period = value;
				RecalculateValues();
			}
		}

		public DX()
			: base(true)
		{
			Panel = IndicatorDataProvider.NewPanel;

			DataSeries.Add(_diPos.DataSeries[0]);
			DataSeries.Add(_diNeg.DataSeries[0]);

			Period = 10;

			Add(_diNeg);

			Add(_diPos);
		}

		protected override void OnCalculate(int bar, decimal value)
		{
			var pos = _diPos[bar];
			var neg = _diNeg[bar];

			var sum = pos + neg;
			var diff = Math.Abs(pos - neg);

			this[bar] = sum != 0m ? 100 * diff / sum : 0m;
		}
	}
}