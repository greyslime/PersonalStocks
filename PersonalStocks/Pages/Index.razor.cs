using Microsoft.AspNetCore.Components;
using PersonalStocks.Data;
using ChartJs.Blazor.LineChart;
using ChartJs.Blazor.Common;
using ChartJs.Blazor.Util;
using ChartJs.Blazor.Common.Enums;
using System.Drawing;

namespace PersonalStocks.Pages
{
    public partial class Index
    {
        [Inject]
        private PersonalStocksService Service { get; set; }
        private LineConfig _config;

        protected override async Task OnInitializedAsync()
        {
            _config = new LineConfig
            {
                Options = new LineOptions
                {
                    Responsive = true,
                    Title = new OptionsTitle
                    {
                        Display = true,
                        Text = "Personal Stocks"
                    },
                    Tooltips = new Tooltips
                    {
                        Mode = InteractionMode.Nearest,
                        Intersect = true
                    },
                    Hover = new Hover
                    {
                        Mode = InteractionMode.Nearest,
                        Intersect = true
                    }
                }
            };

            var stocks = await Service.GetStocks();

            foreach (var stock in stocks)
            {
                await Service.AlignMovementsOfStock(stock);

                StockDataset dataset = new(stock.Name); 
                if(!_config.Data.XLabels.Contains("Start")) _config.Data.XLabels.Add("Start");
                stock.CurrentValue = stock.StartingValue;
                dataset.Add(stock.StartingValue);

                foreach (var m in await Service.GetMovements(stock))
                {
                    if (!_config.Data.XLabels.Contains(m.Date.ToShortDateString())) _config.Data.XLabels.Add(m.Date.ToShortDateString());
                        stock.ApplyMovement(m);
                    dataset.Add(stock.CurrentValue);
                }

                _config.Data.Datasets.Add(dataset);
            }
        }
    }

    class StockDataset : LineDataset<double>
    {
        public StockDataset(string name)
        {
            Random random = new();
            Color randomColor = Color.FromArgb((byte)random.Next(0, 255), (byte)random.Next(0, 255), (byte)random.Next(0, 255));

            Label = name;
            BackgroundColor = ColorUtil.FromDrawingColor(randomColor);
            BorderColor = ColorUtil.FromDrawingColor(randomColor);
            Fill = FillingMode.Disabled;
            LineTension = 0;
            PointStyle = ChartJs.Blazor.Common.Enums.PointStyle.Rect;
            SpanGaps = false;
        }
    }
}




