export function renderTimeSeries(elementId, trace, layout) {

    // var trace1 = {
    //     type: "scatter",
    //     mode: "lines",
    //     name: 'AAPL High',
    //     x: unpack(rows, 'Date'),
    //     y: unpack(rows, 'AAPL.High'),
    //     line: {color: '#17BECF'}
    // }
    //
    // var data = [trace1,trace2];
    //
    // var layout = {
    //     title: {text: 'Time Series with Rangeslider'},
    //     xaxis: {
    //         autorange: true,
    //         range: ['2015-02-17', '2017-02-16'],
    //         rangeslider: {range: ['2015-02-17', '2017-02-16']},
    //         type: 'date'
    //     },
    //     yaxis: {
    //         autorange: true,
    //         range: [86.8700008333, 138.870004167],
    //         type: 'linear'
    //     }
    // };
    
    var data = [trace];
    
    Plotly.newPlot(elementId, data, layout);
}