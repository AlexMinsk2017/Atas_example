namespace ATAS.Indicators.Technical
{
	using System;
	using System.ComponentModel;
	using System.Windows.Media;

	using ATAS.Indicators.Properties;

	using Utils.Common.Localization;

	using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

	[DisplayName("Woodies CCI")]
    public class WoodiesCCI:Indicator
    {
        private readonly CCI _trendCci = new CCI { Name = "Trend CCI" };
        private readonly CCI _entryCci = new CCI { Name = "Entry CCI" };

		private readonly ValueDataSeries _cciNoTrend = new ValueDataSeries("No Trend") { VisualType = VisualMode.Histogram, Color = Colors.Gray, ShowCurrentValue = false, Width = 2 };
		private readonly ValueDataSeries _cciTimeBar = new ValueDataSeries("Time Bar") { VisualType = VisualMode.Histogram, Color = Colors.Gold, ShowCurrentValue = false, Width = 2 };
		private readonly ValueDataSeries _cciTrendUp = new ValueDataSeries("CCI Trend Up") { VisualType = VisualMode.Histogram, Color = Colors.Blue, ShowCurrentValue = false, Width = 2 };
		private readonly ValueDataSeries _cciTrendDown = new ValueDataSeries("CCI Trend Down") { VisualType = VisualMode.Histogram, Color = Colors.Maroon, ShowCurrentValue = false, Width = 2 };
		private readonly ValueDataSeries _negativeLsma = new ValueDataSeries("Negative LSMA") { VisualType = VisualMode.Block, Color = Colors.Red, ShowCurrentValue = false, ScaleIt = false, Width = 2 };
		private readonly ValueDataSeries _positiveLsma = new ValueDataSeries("Positive LSMA") { VisualType = VisualMode.Block, Color = Colors.Green, ShowCurrentValue = false, ScaleIt = false, Width = 2 };

        int _trendUp, _trendDown;

        #region Properties

        private int _lsmaPeriod = 25;

        [PropertyOrder(20)]
        [Parameter]
        [Category( "Common")]
        [DisplayName("LSMA Period")]
        public int LSMAPeriod
        {
            get { return _lsmaPeriod; }
            set
            {
                if (value <= 0)
                    return;

                _lsmaPeriod = value;
                RecalculateValues();
            }
        }

        private int _trendperiod = 5;

        [PropertyOrder(20)]
        [Parameter]
        [Category( "Common")]
        [DisplayName("Trend Period")]
        public int TrendPeriod
        {
            get { return _trendperiod; }
            set
            {
                if (value <= 0)
                    return;

                _trendperiod = value;
                RecalculateValues();
            }
        }

        [Category( "Common")]
        [DisplayName("Trend CCI Period")]
        [PropertyOrder(20)]
        [Parameter]
        public int TrendCCIPeriod
        {
            get { return _trendCci.Period; }
            set
            {
                if (value <= 0)
                    return;

                _trendCci.Period = value;
                RecalculateValues();
            }
        }

        [Category( "Common")]
        [DisplayName("Entry CCI Period")]
        [PropertyOrder(20)]
        [Parameter]
        public int EntryCCIPeriod
        {
            get { return _entryCci.Period; }
            set
            {
                if (value <= 0)
                    return;

                _entryCci.Period = value;
                RecalculateValues();
            }
        }

        #endregion

	    public WoodiesCCI()
		    : base(true)
	    {
		    TrendCCIPeriod = 14;
		    EntryCCIPeriod = 6;
		    _trendCci.DataSeries[0].Name = "Trend CCI";
		    _entryCci.DataSeries[0].Name = "Entry CCI";
		    Panel = IndicatorDataProvider.NewPanel;
		    ((ValueDataSeries)_entryCci.DataSeries[0]).Color = Colors.Orange;
		    ((ValueDataSeries)_trendCci.DataSeries[0]).Width = 2;
		    ((ValueDataSeries)_trendCci.DataSeries[0]).Color = Color.FromArgb(255, 69, 23, 69);

		    DataSeries.Add(_cciTrendUp);
		    DataSeries.Add(_cciTrendDown);
		    DataSeries.Add(_cciNoTrend);
		    DataSeries.Add(_cciTimeBar);

		    DataSeries.Add(_trendCci.DataSeries[0]);
		    DataSeries.Add(_entryCci.DataSeries[0]);

		    DataSeries.Add(_negativeLsma);
		    DataSeries.Add(_positiveLsma);

		    ((ValueDataSeries)DataSeries[0]).ShowCurrentValue = false;
		    ((ValueDataSeries)DataSeries[0]).Name = "Zero Line";
		    ((ValueDataSeries)DataSeries[0]).Color = Colors.Gray;
		    ((ValueDataSeries)DataSeries[0]).VisualType = VisualMode.Hide;

		    LineSeries.Add(new LineSeries("100")
		    {
			    Color = Colors.Gray,
			    LineDashStyle = LineDashStyle.Dash,
			    Value = 100,
			    Width = 1
		    });
		    LineSeries.Add(new LineSeries("200")
		    {
			    Color = Colors.Gray,
			    LineDashStyle = LineDashStyle.Dash,
			    Value = 200,
			    Width = 1
		    });
		    LineSeries.Add(new LineSeries("300")
		    {
			    Color = Colors.Gray,
			    LineDashStyle = LineDashStyle.Dash,
			    Value = 300,
			    Width = 1,
			    UseScale = true
		    });
		    LineSeries.Add(new LineSeries("-100")
		    {
			    Color = Colors.Gray,
			    LineDashStyle = LineDashStyle.Dash,
			    Value = -100,
			    Width = 1
		    });
		    LineSeries.Add(new LineSeries("-200")
		    {
			    Color = Colors.Gray,
			    LineDashStyle = LineDashStyle.Dash,
			    Value = -200,
			    Width = 1
		    });
		    LineSeries.Add(new LineSeries("-300")
		    {
			    Color = Colors.Gray,
			    LineDashStyle = LineDashStyle.Dash,
			    Value = -300,
			    Width = 1,
			    UseScale = true
		    });

			Add(_trendCci);
		    Add(_entryCci);
		}

		#region Overrides of Indicator

		/// <summary>
		/// Расчет значения индикатора
		/// </summary>
		/// <param name="bar">Номер бара</param>
		/// <param name="value">Значение входящей датасерии для данного бара</param>
		protected override void OnCalculate(int bar, decimal value)
        {
            try
            {
                this[bar] = 0;
                _cciNoTrend[bar] = 0;
                _cciTimeBar[bar] = 0;
                _cciTrendUp[bar] = 0;
                _cciTrendDown[bar] = 0;
                if (_trendCci[bar] > 0 && _trendCci[bar-1] < 0)
                {
                    if (_trendDown > TrendPeriod) _trendUp = 0;
                }
                if (_trendCci[bar] > 0)
                {
                    if (_trendUp < TrendPeriod)
                    {
                        _cciNoTrend[bar] = _trendCci[bar];
                        _trendUp++;
                    }
                    if (_trendUp == TrendPeriod)
                    {
                        _cciTimeBar[bar] = _trendCci[bar];
                        _trendUp++;
                    }
                    if (_trendUp > TrendPeriod)
                    {
                        _cciTrendUp[bar] = _trendCci[bar];
                    }
                }

                if (_trendCci[bar] < 0 && _trendCci[bar - 1] > 0)
                {
                    if (_trendUp > TrendPeriod) _trendDown = 0;
                }
                if (_trendCci[bar] < 0)
                {
                    if (_trendDown < TrendPeriod)
                    {
                        _cciNoTrend[bar] = _trendCci[bar];
                        _trendDown++;
                    }
                    if (_trendDown == TrendPeriod)
                    {
                        _cciTimeBar[bar] = _trendCci[bar];
                        _trendDown++;
                    }
                    if (_trendDown > TrendPeriod)
                    {
                        _cciTrendDown[bar] = _trendCci[bar];
                    }
                }


                decimal summ = 0;

                if (bar < LSMAPeriod + 2) return;
                
                for(int i=LSMAPeriod; i>=1; i--)
                {
                    decimal lengthvar = (decimal) ((LSMAPeriod + 1) / 3.0);
                    summ += (i - lengthvar) * GetCandle(bar - LSMAPeriod + i).Close;
                }

                var wt = summ * 6 / (LSMAPeriod * (LSMAPeriod + 1));
                _negativeLsma[bar] = 0.00001m;
                _positiveLsma[bar] = 0.00001m;

                if (wt > GetCandle(bar).Close) _negativeLsma[bar] = 0;
                if (wt < GetCandle(bar).Close) _positiveLsma[bar] = 0;

            }
            catch (Exception)
            {
                

            }
            
            //  _trendCCI.Calculate(bar, value);
            //  _entryCCI.Calculate(bar, value);
        }

        #endregion
    }
}