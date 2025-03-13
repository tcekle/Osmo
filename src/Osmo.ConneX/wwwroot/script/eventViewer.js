export function renderEventViewer(elementName, incomingData, frequencyData) {

    var container = document.getElementById(elementName);
    let symbolMap = {
        SystemStartup: 'triangle-up',
        SystemShutdown: 'triangle-down'
    }

    let data = [];

    for (let categoryIndex in incomingData)
    {
        data.push({
            x: incomingData[categoryIndex].map(x => x.timestamp),
            y: incomingData[categoryIndex].map(y => y.type),
            visible: incomingData[categoryIndex].length <= 1000 ? 'legend' : 'legendonly',
            mode: 'markers',
            type: 'scattergl',
            name: incomingData[categoryIndex][0].type + " (" + incomingData[categoryIndex].length + ")",
            text: incomingData[categoryIndex].map(t => t.title),
            marker: {
                size: 12,
                symbol: symbolMap.hasOwnProperty(incomingData[categoryIndex][0].type) ? symbolMap[incomingData[categoryIndex][0].type] : 'diamond'
            },
            hovertemplate:
                "<b>%{x|%Y-%m-%d %H:%M:%S} - %{text}</b><br><br>" +
                "%{customdata.message}<extra></extra>",
            customdata: incomingData[categoryIndex].map(t => ({message: t.message == null ? "<none>" : t.message.replace(/(?:\r\n|\r|\n)/g, '<br>')}))
        });
    }

    data.push({
        x: frequencyData.map(x => x.time),
        y: frequencyData.map(y => y.value),
        type: 'bar',
        yaxis: 'y2',
        name: "Event Frequency",
        marker: {
            color: 'rgba(0, 0, 0, 0.1)', // Specify the color with reduced opacity
        },
        hoverinfo: 'none', // Disable hoverinfo for the faded bars
    })

    var layout = {
        height: container.clientHeight,
        xaxis: {
            showgrid: true,
            showline: true,
            linecolor: "rgb(102, 102, 102)",
            titlefont: { font: { color: "rgb(204, 204, 204)" } },
            title: "Time of the event",
            type: 'date',
            rangeslider: {
                visible: true
            }
        },
        xaxis2: {
            matches: 'x',
            rangeslider: {
                visible: true
            }
        },
        yaxis: {
            showgrid: true,
            showline: true,
            fixedrange: true,
            linecolor: "rgb(102, 102, 102)",
            titlefont: { font: { color: "rgb(204, 204, 204)" } },
            tickfont: { font: { color: "rgb(102, 102, 102)" } },
            title: "ConneX Event Type",
        },
        yaxis2: {
            title: 'frequency',
            overlaying: 'y',
            side: 'right'

        },
        title:'ConneX events',
        margin: { l: 140, r: 40, b: 100, t: 50 },
        hovermode: "closest",
        hoverlabel: { bgcolor: "#FFF" }
    };

    Plotly.newPlot(elementName, data, layout);
}