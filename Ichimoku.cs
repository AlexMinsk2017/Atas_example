namespace ATAS.Indicators.Technical
{
	using System;
	using System.ComponentModel;
	using System.Windows.Media;

	using ATAS.Indicators.Properties;

	using Utils.Common.Localization;

	[DisplayName("Ichimoku Kinko Hyo")]
	public class Ichimoku : Indicator
	{
		private readonly RangeDataSeries _senkouSpanBand = new RangeDataSeries("Senkou Span");
		private int _tenkan;
		private int _kijun;
		private int _senkou;
		private int _extBegin;

		[Category( "Settings")]
		[DisplayName("Tenkan-sen")]
		public int Tenkan
		{
			get => _tenkan;
			set
			{
				if (value <= 0)
					return;
				_tenkan = value;
				_extBegin = _kijun;
				if (_extBegin < _tenkan)
					_extBegin = _tenkan;
				DataSeries[0].Clear();
				DataSeries[1].Clear();
				DataSeries[2].Clear();
				DataSeries[3].Clear();
				RecalculateValues();
			}
		}

		[Category( "Settings")]
		[DisplayName("Kijun-sen")]
		public int Kijun
		{
			get => _kijun;
			set
			{
				if (value <= 0)
					return;
				_kijun = value;
				_extBegin = _kijun;
				if (_extBegin < _tenkan)
					_extBegin = _tenkan;
				DataSeries[0].Clear();
				DataSeries[1].Clear();
				DataSeries[2].Clear();
				DataSeries[3].Clear();
				RecalculateValues();
			}
		}

		[Category( "Settings")]
		[DisplayName("Senkou Span B")]
		public int Senkou
		{
			get => _senkou;
			set
			{
				if (value <= 0)
					return;
				_senkou = value;
				DataSeries[0].Clear();
				DataSeries[1].Clear();
				DataSeries[2].Clear();
				DataSeries[3].Clear();
				RecalculateValues();
			}
		}

		public Ichimoku()
			: base(true)
		{
			Panel = "Chart";
			_tenkan = 9;
			_kijun = 26;
			_senkou = 52;
			_extBegin = _kijun;
			if (_extBegin < _tenkan)
				_extBegin = _tenkan;
			((BaseDataSeries<Decimal>)DataSeries[0]).Name = "Tenkan-sen";
			((ValueDataSeries)DataSeries[0]).VisualType = VisualMode.Line;
			((ValueDataSeries)DataSeries[0]).LineDashStyle = LineDashStyle.Solid;
			((ValueDataSeries)DataSeries[0]).Color = Colors.Red;
			((ValueDataSeries)DataSeries[0]).Width = 1;
			DataSeries.Add(new ValueDataSeries("Kijun-sen")
			{
				VisualType = VisualMode.Line,
				LineDashStyle = LineDashStyle.Solid,
				Color = Colors.Blue,
				Width = 1
			});
			DataSeries.Add(new ValueDataSeries("Chikou Span")
			{
				VisualType = VisualMode.Line,
				LineDashStyle = LineDashStyle.Solid,
				Color = Colors.Lime,
				Width = 1
			});
			DataSeries.Add(new ValueDataSeries("Up Kumo")
			{
				VisualType = VisualMode.Line,
				LineDashStyle = LineDashStyle.Dot,
				Color = Colors.SandyBrown,
				Width = 1
			});
			DataSeries.Add(new ValueDataSeries("Down Kumo")
			{
				VisualType = VisualMode.Line,
				LineDashStyle = LineDashStyle.Dot,
				Color = Colors.Thistle,
				Width = 1
			});
			DataSeries.Add(_senkouSpanBand);
		}

		protected override void OnCalculate(int bar, Decimal value)
		{
			int bar1 = bar;
			int num = Math.Max(Math.Max(_tenkan, _kijun), _senkou) + 1;
			IndicatorCandle candle1 = GetCandle(bar1);
			Decimal val11 = candle1.Low;
			Decimal val12 = candle1.High;
			int bar2 = bar1 - 1;
			int currentBar = CurrentBar;
			if (bar + _kijun < currentBar)
				DataSeries[2][bar] = GetCandle(bar + _kijun).Close;
			else
				DataSeries[2][bar] = DataSeries[2][bar - 1];
			for (int index = 1; index < num & bar2 >= 0; --bar2)
			{
				IndicatorCandle candle2 = GetCandle(bar2);
				val11 = Math.Min(val11, candle2.Low);
				val12 = Math.Max(val12, candle2.High);
				if (bar >= _tenkan - 1 && index == _tenkan)
					DataSeries[0][bar] = (val12 + val11) / new Decimal(2);
				if (bar >= _kijun - 1 && index == _kijun)
					DataSeries[1][bar] = (val12 + val11) / new Decimal(2);
				if (bar >= _senkou - 1 && index == _senkou)
					DataSeries[4][bar] = (val12 + val11) / new Decimal(2);
				++index;
			}
			if (bar >= _extBegin)
				DataSeries[3][bar] = ((Decimal)DataSeries[0][bar - _kijun] + (Decimal)DataSeries[1][bar - _kijun]) / new Decimal(2);
			_senkouSpanBand[bar].Upper = (Decimal)DataSeries[4][bar];
			_senkouSpanBand[bar].Lower = (Decimal)DataSeries[3][bar];
		}
	}
}