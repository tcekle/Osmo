// /// set the dimensions and margins of the graph
// const width = 450,
//     height = 450,
//     margin = 40;
//
// // The radius of the pieplot is half the width or half the height (smallest one). I subtract a bit of margin.
// const radius = Math.min(width, height) / 2 - margin;
//
// // append the svg object to the div called 'my_dataviz'
// const svg = d3.select("#my_pie_chart")
//     .append("svg")
//     .attr("width", width)
//     .attr("height", height)
//     .append("g")
//     .attr("transform", `translate(${width/2}, ${height/2})`);
//
// // create 2 data_set
// const data1 = {a: 9, b: 20, c:30, d:8, e:12}
// const data2 = {a: 6, b: 16, c:20, d:14, e:19, f:12}
//
// // set the color scale
// const color = d3.scaleOrdinal()
//     .domain(["a", "b", "c", "d", "e", "f"])
//     .range(d3.schemeDark2);
//
// // A function that create / update the plot for a given variable:
// function update(data) {
//
//     // Compute the position of each group on the pie:
//     const pie = d3.pie()
//         .value(function(d) {return d[1]; })
//         .sort(function(a, b) { return d3.ascending(a.key, b.key);} ) // This make sure that group order remains the same in the pie chart
//     const data_ready = pie(Object.entries(data))
//
//     // map to data
//     const u = svg.selectAll("path")
//         .data(data_ready)
//
//     // Build the pie chart: Basically, each part of the pie is a path that we build using the arc function.
//     u
//         .join('path')
//         .transition()
//         .duration(1000)
//         .attr('d', d3.arc()
//             .innerRadius(0)
//             .outerRadius(radius)
//         )
//         .attr('fill', function(d){ return(color(d.data[0])) })
//         .attr("stroke", "white")
//         .style("stroke-width", "2px")
//         .style("opacity", 1)
//
//
// }

const seriesColors = ["#7cb5ec", "#434348", "#90ed7d", "#f7a35c", "#8085e9", "#f15c80", "#e4d354", "#2b908f", "#f45b5b", "#91e8e1"];
const interpolatedColor = "#fcb0f1";

function tooltipPlugin({onclick, isInterpolated, data, shiftX = 10, shiftY = 10}) {
    let tooltipLeftOffset = 0;
    let tooltipTopOffset = 0;

    const tooltip = document.createElement("div");
    tooltip.className = "u-tooltip";

    let seriesIdx = null;
    let dataIdx = null;

    const fmtDate = uPlot.fmtDate("{M}/{D}/{YY} {h}:{mm}:{ss} {AA}");

    let over;

    let tooltipVisible = false;

    function showTooltip() {
        if (!tooltipVisible) {
            tooltip.style.display = "block";
            over.style.cursor = "pointer";
            tooltipVisible = true;
        }
    }

    function hideTooltip() {
        if (tooltipVisible) {
            tooltip.style.display = "none";
            over.style.cursor = null;
            tooltipVisible = false;
        }
    }

    function setTooltip(u) {
        showTooltip();

        let top = u.valToPos(u.data[seriesIdx][dataIdx], 'y');
        let lft = u.valToPos(u.data[        0][dataIdx], 'x');

        tooltip.style.top  = (tooltipTopOffset  + top + shiftX) + "px";
        tooltip.style.left = (tooltipLeftOffset + lft + shiftY) + "px";

        let message = "";
        let title = "";
        if (typeof u.data[seriesIdx + 1][dataIdx] !== 'undefined' && u.data[seriesIdx + 1][dataIdx].hasOwnProperty('message'))
        {
            message = u.data[seriesIdx + 1][dataIdx].message.message;
            title = u.data[seriesIdx + 1][dataIdx].message.title;
        }
        
        tooltip.style.borderColor = isInterpolated(dataIdx) ? interpolatedColor : seriesColors[seriesIdx - 1];
        let pctSinceStart = (((u.data[seriesIdx][dataIdx] - u.data[seriesIdx][0]) / u.data[seriesIdx][0]) * 100).toFixed(2);
        tooltip.textContent = (
            fmtDate(new Date(u.data[0][dataIdx] * 1e3)) + " - " + title + "\n" +
            message
            // uPlot.fmtNum(u.data[seriesIdx][dataIdx]) + " (" + pctSinceStart + "% since start)"
        );
    }

    return {
        hooks: {
            ready: [
                u => {
                    over = u.over;
                    tooltipLeftOffset = parseFloat(over.style.left);
                    tooltipTopOffset = parseFloat(over.style.top);
                    u.root.querySelector(".u-wrap").appendChild(tooltip);

                    let clientX;
                    let clientY;

                    over.addEventListener("mousedown", e => {
                        clientX = e.clientX;
                        clientY = e.clientY;
                    });

                    over.addEventListener("mouseup", e => {
                        // clicked in-place
                        if (e.clientX == clientX && e.clientY == clientY) {
                            if (seriesIdx != null && dataIdx != null) {
                                onclick(u, seriesIdx, dataIdx);
                            }
                        }
                    });
                }
            ],
            setCursor: [
                u => {
                    let c = u.cursor;

                    if (dataIdx != c.idx) {
                        dataIdx = c.idx;

                        if (seriesIdx != null)
                            setTooltip(u);
                    }
                }
            ],
            setSeries: [
                (u, sidx) => {
                    if (seriesIdx != sidx) {
                        seriesIdx = sidx;

                        if (sidx == null)
                            hideTooltip();
                        else if (dataIdx != null)
                            setTooltip(u);
                    }
                }
            ],
            // drawAxes: [
            //     u => {
            //         let { ctx } = u;
            //         let { left, top, width, height } = u.bbox;
            //
            //         const interpolatedColorWithAlpha = "#fcb0f17a";
            //
            //         ctx.save();
            //
            //         ctx.strokeStyle = interpolatedColorWithAlpha;
            //         ctx.beginPath();
            //
            //         let [ i0, i1 ] = u.series[0].idxs;
            //
            //         for (let j = i0; j <= i1; j++) {
            //             let v = u.data[0][j];
            //
            //             if (isInterpolated(j)) {
            //                 let cx = Math.round(u.valToPos(v, 'x', true));
            //                 ctx.moveTo(cx, top);
            //                 ctx.lineTo(cx, top + height);
            //             }
            //         }
            //
            //         ctx.closePath();
            //         ctx.stroke();
            //         ctx.restore();
            //     },
            // ],
        }
    };
}

export function uplotExample(incomingData) {

    
    const markerDrawHook = u => {
        const ctx = u.ctx;
        ctx.save();



        for (let timeIndex = 0; timeIndex < u.data[0].length; timeIndex++)
        {
            const timestamp = u.data[0][timeIndex];
            for (let yValueIndex = 1; yValueIndex < u.data.length; yValueIndex++)
            {
                if (typeof u.data[yValueIndex][timeIndex] === 'undefined')
                {
                    continue;
                }

                let val = u.data[yValueIndex][timeIndex];
                let height = 50;
                let width = 50;
                let cx = Math.round(u.valToPos(timestamp, 'x', true));
                let cy = Math.round(u.valToPos(val, 'y', true));

                ctx.beginPath();
                ctx.moveTo(cx, cy);

                // top left edge
                ctx.lineTo(cx - width / 2, cy + height / 2);

                // bottom left edge
                ctx.lineTo(cx, cy + height);

                // bottom right edge
                ctx.lineTo(cx + width / 2, cy + height / 2);

                // closing the path automatically creates
                // the top right edge
                ctx.closePath();

                ctx.fillStyle = seriesColors[yValueIndex - 1];
                ctx.fill();
            }
        }
        
        // for (let timeIndex = 0; timeIndex < u.data[0].length; timeIndex++)
        // {
        //     const timestamp = u.data[0][timeIndex];
        //     for (let yValueIndex = 1; yValueIndex < u.data.length; yValueIndex++)
        //     {
        //         if (typeof u.data[yValueIndex][timeIndex] === 'undefined')
        //         {
        //             continue;
        //         }
        //
        //         const left = u.valToPos(timestamp, 'x') + u.bbox.left + 1;
        //         const top = u.valToPos(u.data[yValueIndex][timeIndex], 'y') + u.bbox.top;
        //
        //         ctx.beginPath();
        //         ctx.moveTo(left, top);
        //         ctx.lineTo(left, u.bbox.height);
        //         ctx.strokeStyle = seriesColors[yValueIndex - 1];
        //         ctx.lineWidth = 2;
        //         ctx.stroke();
        //     }
        // }

        // for (let dataIndex = 1; dataIndex < u.data.length; dataIndex++)
        // {
        //     // Here we're just drawing a vertical line for each marker.
        //     // You can add as much complexity as you want.
        //     u.data[dataIndex].forEach((timestamp, i) => {
        //         const left = u.valToPos(timestamp, 'x') + u.bbox.left + 1;
        //         const top = u.valToPos(u.data[dataIndex][i], 'y') + u.bbox.top;
        //
        //         ctx.beginPath();
        //         ctx.moveTo(left, top);
        //         ctx.lineTo(left, u.bbox.height);
        //         // ctx.strokeStyle = "#ff0000"; // Red
        //         ctx.strokeStyle = seriesColors[dataIndex - 1];
        //         ctx.lineWidth = 2;
        //         ctx.stroke();
        //     });
        // }
        


        ctx.restore();
    };
    
    var container = document.getElementById('uplotExample');

    function isInterpolated(dataIdx) {
        return false;
    }

    let seriesOpts = [{}];
    let tables = [];
    
    for (let categoryIndex in incomingData)
    {
        seriesOpts.push({
            label: incomingData[categoryIndex][0].type,
            stroke: seriesColors[categoryIndex],
            points: {
                space: 0.5,
                fill: "red"
            },
            lineInterpolation: null,
            drawStyle: 2,
            paths: (u, seriesIdx, idx0, idx1) => {
                uPlot.orient(u, seriesIdx, (series, dataX, dataY, scaleX, scaleY, valToPosX, valToPosY, xOff, yOff, xDim, yDim) => {
                    let d = u.data[seriesIdx];
                });

                return null;
            }
        });
        
        tables.push([
            incomingData[categoryIndex].map(x => ( Math.floor(new Date(x.timestamp).getTime() / 1000 ))),
            incomingData[categoryIndex].map(x => categoryIndex + 1),
            incomingData[categoryIndex].map(x => ({message: x}))])
    }
    
    const opts = {
        width: container.clientWidth,
        height: 400,
        cursor: {
            focus: {
                prox: 5
            },
        },
        legend: {
            live: false,
        },
        series: seriesOpts,
        // series: [
        //     {},
        //     {
        //         label: "test Y",
        //         stroke: "red",
        //         lineInterpolation: null,
        //         drawStyle: 2,
        //         paths: (u, seriesIdx, idx0, idx1) => {
        //             uPlot.orient(u, seriesIdx, (series, dataX, dataY, scaleX, scaleY, valToPosX, valToPosY, xOff, yOff, xDim, yDim) => {
        //                 let d = u.data[seriesIdx];
        //
        //                 console.log(d);
        //             });
        //
        //             return null;
        //         }
        //     }
        // ],
        axes: [
            {},
            {
                label: "Y Label",
                labelGap: 8,
                labelSize: 28
            }
        ],
        hooks: {
            draw: [markerDrawHook]
        },
        plugins: [
            tooltipPlugin({
                onclick(u, seriesIdx, dataIdx) {
                    // let thisCommit = commits[dataIdx][1];
                    // let prevCommit = (commits[dataIdx-1] || [null,null])[1];
                    // window.open(`https://perf.rust-lang.org/compare.html?start=${prevCommit}&end=${thisCommit}&stat=${stat}`);
                },
                isInterpolated,
                data: incomingData
            }),
        ]
    };

    // opts.axes.forEach((axis) => {
    //     if (Object.keys(axis).length === 0)
    //     {
    //         return;
    //     }
    //
    //     axis.size = (self, values, axisIdx, cycleNum) => {
    //         let axis = self.axes[axisIdx];
    //
    //         // bail out, force convergence
    //         if (cycleNum > 1)
    //             return axis._size;
    //
    //         let axisSize = axis.ticks.size + axis.gap;
    //
    //         // find longest value
    //         let longestVal = (values ?? []).reduce((acc, val) => (
    //             val.length > acc.length ? val : acc
    //         ), "");
    //
    //         if (longestVal != "") {
    //             self.ctx.font = axis.font[0];
    //             axisSize += self.ctx.measureText(longestVal).width / devicePixelRatio;
    //         }
    //
    //         return Math.ceil(axisSize);
    //     }
    // });

    // const data = [
    //     // timestamps for X values
    //     [1597778400, 1597864800, 1597951200],
    //     // Y values
    //     [10, 20, 10],
    // ];
    
    // const data = [xValues, yValues, yValues];
    const data = uPlot.join(tables);
    // function fillArray(val) {
    //     let array = Array(val.length);
    //    
    //     for()
    //    
    //     return array;
    // }

    const uplotChart = new uPlot(opts, data, container);
    makeColorGradient(.3,.3,.3,0,2,4, 128,127);
}

function makeColorGradient(frequency1, frequency2, frequency3,
                           phase1, phase2, phase3,
                           center, width, len)
{
    if (center == undefined)   center = 128;
    if (width == undefined)    width = 127;
    if (len == undefined)      len = 50;

    for (var i = 0; i < len; ++i)
    {
        var red = Math.sin(frequency1*i + phase1) * width + center;
        var grn = Math.sin(frequency2*i + phase2) * width + center;
        var blu = Math.sin(frequency3*i + phase3) * width + center;
        // document.write( '<font color="' + RGB2Color(red,grn,blu) + '">&#9608;</font>');
        console.log(RGB2Color(red,grn,blu));
    }
}

function RGB2Color(r,g,b)
{
    return '#' + byte2Hex(r) + byte2Hex(g) + byte2Hex(b);
}

function byte2Hex(n)
{
    var nybHexString = "0123456789ABCDEF";
    return String(nybHexString.substr((n >> 4) & 0x0F,1)) + nybHexString.substr(n & 0x0F,1);
}

export function createTimeline(element, data, groups) {
    // DOM element where the Timeline will be attached
    var container = document.getElementById(element);
    
    var itemGroups = new vis.DataSet(groups);
    // Create a DataSet (allows two way data-binding)
    var items = new vis.DataSet(data);
    // var items = new vis.DataSet([
    //     {id: 1, content: 'item 1', start: '2014-04-20'},
    //     {id: 2, content: 'item 2', start: '2014-04-14'},
    //     {id: 3, content: 'item 3', start: '2014-04-18'},
    //     {id: 4, content: 'item 4', start: '2014-04-16', end: '2014-04-19'},
    //     {id: 5, content: 'item 5', start: '2014-04-25'},
    //     {id: 6, content: 'item 6', start: '2014-04-27', type: 'point'}
    // ]);

    // Configuration for the Timeline
    var options = {
        verticalScroll: true,
        zoomKey: 'ctrlKey',
        height: 1000,
        maxHeight: 1000,
        minHeight: 1000,
        groupOrder: 'content',
        tooltip: {
            followMouse: true
        }
    };

    // Create a Timeline
    var timeline = new vis.Timeline(container);
    timeline.setOptions(options);
    timeline.setGroups(itemGroups);
    timeline.setItems(data);
}

export function initialize() {
    update(data1);
}

function createChart(data) {
    const root = partition(data);

    //const root = partition(sunBurstData);
    var color = d3.scaleOrdinal(d3.quantize(d3.interpolateRainbow, data.children.length + 1));

    root.each(d => d.current = d);

    const svg = d3.select("#my_pie_chart")
        .append("svg")
        .attr("viewBox", [0, 0, width, width])
        .style("font", "20px sans-serif");
    // const svg = d3.create("svg")
    //     .attr("viewBox", [0, 0, width, width])
    //     .style("font", "10px sans-serif");

    const g = svg.append("g")
        .attr("transform", `translate(${width / 2},${width / 2})`);

    const path = g.append("g")
        .selectAll("path")
        .data(root.descendants().slice(1))
        .join("path")
        .attr("fill", d => { while (d.depth > 1) d = d.parent; return color(d.data.name); })
        .attr("fill-opacity", d => arcVisible(d.current) ? (d.children ? 0.6 : 0.4) : 0)
        .attr("pointer-events", d => arcVisible(d.current) ? "auto" : "none")

        .attr("d", d => arc(d.current));

    path.filter(d => d.children)
        .style("cursor", "pointer")
        .on("click", clicked);

    path.append("title")
        .text(d => `${d.ancestors().map(d => d.data.name).reverse().join("/")}\n${format(d.value)}`);

    const label = g.append("g")
        .attr("pointer-events", "none")
        .attr("text-anchor", "middle")
        .style("user-select", "none")
        .selectAll("text")
        .data(root.descendants().slice(1))
        .join("text")
        .attr("dy", "0.35em")
        .attr("fill-opacity", d => +labelVisible(d.current))
        .attr("transform", d => labelTransform(d.current))
        .text(d => d.data.name);

    const parent = g.append("circle")
        .datum(root)
        .attr("r", radius)
        .attr("fill", "none")
        .attr("pointer-events", "all")
        .on("click", clicked);

    function clicked(event, p) {
    parent.datum(p.parent || root);

    root.each(d => d.target = {
        x0: Math.max(0, Math.min(1, (d.x0 - p.x0) / (p.x1 - p.x0))) * 2 * Math.PI,
        x1: Math.max(0, Math.min(1, (d.x1 - p.x0) / (p.x1 - p.x0))) * 2 * Math.PI,
        y0: Math.max(0, d.y0 - p.depth),
        y1: Math.max(0, d.y1 - p.depth)
    });

    const t = g.transition().duration(750);

    // Transition the data on all arcs, even the ones that aren’t visible,
    // so that if this transition is interrupted, entering arcs will start
    // the next transition from the desired position.
    path.transition(t)
        .tween("data", d => {
            const i = d3.interpolate(d.current, d.target);
            return t => d.current = i(t);
        })
        .filter(function(d) {
            return +this.getAttribute("fill-opacity") || arcVisible(d.target);
        })
        .attr("fill-opacity", d => arcVisible(d.target) ? (d.children ? 0.6 : 0.4) : 0)
        .attr("pointer-events", d => arcVisible(d.target) ? "auto" : "none")

        .attrTween("d", d => () => arc(d.current));

    label.filter(function(d) {
        return +this.getAttribute("fill-opacity") || labelVisible(d.target);
    }).transition(t)
        .attr("fill-opacity", d => +labelVisible(d.target))
        .attrTween("transform", d => () => labelTransform(d.current));

}

function arcVisible(d) {
    return d.y1 <= 3 && d.y0 >= 1 && d.x1 > d.x0;
}

function labelVisible(d) {
    return d.y1 <= 3 && d.y0 >= 1 && (d.y1 - d.y0) * (d.x1 - d.x0) > 0.03;
}

function labelTransform(d) {
    const x = (d.x0 + d.x1) / 2 * 180 / Math.PI;
    const y = (d.y0 + d.y1) / 2 * radius;
    return `rotate(${x - 90}) translate(${y},0) rotate(${x < 180 ? 0 : 180})`;
}

    function partition(data) {
        const root = d3.hierarchy(data)
            .sum(d => d.value)
            .sort((a, b) => b.value - a.value);
        return d3.partition()
            .size([2 * Math.PI, root.height + 1])
            (root);
    }

    
    
    function format() {
        return d3.format(",d");
    }
    
return svg.node();
}

var width = 1000;
var radius = width / 6;


var arc = d3.arc()
    .startAngle(d => d.x0)
    .endAngle(d => d.x1)
    .padAngle(d => Math.min((d.x1 - d.x0) / 2, 0.005))
    .padRadius(radius * 1.5)
    .innerRadius(d => d.y0 * radius)
    .outerRadius(d => Math.max(d.y0 * radius, d.y1 * radius - 1))


function sunBurstChart(data) {
    function autoBox() {
        document.body.appendChild(this);
        const {x, y, width, height} = this.getBBox();
        document.body.removeChild(this);
        return [x, y, width, height];
    }
    
    // Specify the chart’s colors and approximate radius (it will be adjusted at the end).
    const color = d3.scaleOrdinal(d3.quantize(d3.interpolateRainbow, data.children.length + 1));
    const radius = 928 / 2;

    // Prepare the layout.
    const partition = data => d3.partition()
        .size([2 * Math.PI, radius])
        (d3.hierarchy(data)
            .sum(d => d.value)
            .sort((a, b) => b.value - a.value));

    const arc = d3.arc()
        .startAngle(d => d.x0)
        .endAngle(d => d.x1)
        .padAngle(d => Math.min((d.x1 - d.x0) / 2, 0.005))
        .padRadius(radius / 2)
        .innerRadius(d => d.y0)
        .outerRadius(d => d.y1 - 1);

    const root = partition(data);

    // Create the container SVG.
    //const svg = d3.create("svg");
    const svg = d3.select("#my_pie_chart")
        .append("svg");

    // Add an arc for each element, with a title for tooltips.
    const format = d3.format(",d");
    svg.append("g")
        .attr("fill-opacity", 0.6)
        .selectAll("path")
        .data(root.descendants().filter(d => d.depth))
        .join("path")
        .attr("fill", d => { while (d.depth > 1) d = d.parent; return color(d.data.name); })
        .attr("d", arc)
        .append("title")
        .text(d => `${d.ancestors().map(d => d.data.name).reverse().join("/")}\n${format(d.value)}`);

    // Add a label for each element.
    svg.append("g")
        .attr("pointer-events", "none")
        .attr("text-anchor", "middle")
        .attr("font-size", 20)
        .attr("font-family", "sans-serif")
        .selectAll("text")
        .data(root.descendants().filter(d => d.depth && (d.y0 + d.y1) / 2 * (d.x1 - d.x0) > 10))
        .join("text")
        .attr("transform", function(d) {
            const x = (d.x0 + d.x1) / 2 * 180 / Math.PI;
            const y = (d.y0 + d.y1) / 2;
            return `rotate(${x - 90}) translate(${y},0) rotate(${x < 180 ? 0 : 180})`;
        })
        .attr("dy", "0.35em")
        .text(d => d.data.name);

    // The autoBox function adjusts the SVG’s viewBox to the dimensions of its contents.
    return svg.attr("viewBox", [-radius - 50, -radius - 50, width, width]).node();
}
export function createSunBurst(data) {
    //var chart = createChart(sunBurstData);
    //createChart(data);
    sunBurstChart(data);

    // d3.select("#my_pie_chart")
    //     .append(chart);
}

const sunBurstData = {
    "name": "flare",
    "children": [
        {
            "name": "analytics",
            "children": [
                {
                    "name": "cluster",
                    "children": [
                        {"name": "AgglomerativeCluster", "value": 3938},
                        {"name": "CommunityStructure", "value": 3812},
                        {"name": "HierarchicalCluster", "value": 6714},
                        {"name": "MergeEdge", "value": 743}
                    ]
                },
                {
                    "name": "graph",
                    "children": [
                        {"name": "BetweennessCentrality", "value": 3534},
                        {"name": "LinkDistance", "value": 5731},
                        {"name": "MaxFlowMinCut", "value": 7840},
                        {"name": "ShortestPaths", "value": 5914},
                        {"name": "SpanningTree", "value": 3416}
                    ]
                },
                {
                    "name": "optimization",
                    "children": [
                        {"name": "AspectRatioBanker", "value": 7074}
                    ]
                }
            ]
        },
        {
            "name": "animate",
            "children": [
                {"name": "Easing", "value": 17010},
                {"name": "FunctionSequence", "value": 5842},
                {
                    "name": "interpolate",
                    "children": [
                        {"name": "ArrayInterpolator", "value": 1983},
                        {"name": "ColorInterpolator", "value": 2047},
                        {"name": "DateInterpolator", "value": 1375},
                        {"name": "Interpolator", "value": 8746},
                        {"name": "MatrixInterpolator", "value": 2202},
                        {"name": "NumberInterpolator", "value": 1382},
                        {"name": "ObjectInterpolator", "value": 1629},
                        {"name": "PointInterpolator", "value": 1675},
                        {"name": "RectangleInterpolator", "value": 2042}
                    ]
                },
                {"name": "ISchedulable", "value": 1041},
                {"name": "Parallel", "value": 5176},
                {"name": "Pause", "value": 449},
                {"name": "Scheduler", "value": 5593},
                {"name": "Sequence", "value": 5534},
                {"name": "Transition", "value": 9201},
                {"name": "Transitioner", "value": 19975},
                {"name": "TransitionEvent", "value": 1116},
                {"name": "Tween", "value": 6006}
            ]
        },
        {
            "name": "data",
            "children": [
                {
                    "name": "converters",
                    "children": [
                        {"name": "Converters", "value": 721},
                        {"name": "DelimitedTextConverter", "value": 4294},
                        {"name": "GraphMLConverter", "value": 9800},
                        {"name": "IDataConverter", "value": 1314},
                        {"name": "JSONConverter", "value": 2220}
                    ]
                },
                {"name": "DataField", "value": 1759},
                {"name": "DataSchema", "value": 2165},
                {"name": "DataSet", "value": 586},
                {"name": "DataSource", "value": 3331},
                {"name": "DataTable", "value": 772},
                {"name": "DataUtil", "value": 3322}
            ]
        },
        {
            "name": "display",
            "children": [
                {"name": "DirtySprite", "value": 8833},
                {"name": "LineSprite", "value": 1732},
                {"name": "RectSprite", "value": 3623},
                {"name": "TextSprite", "value": 10066}
            ]
        },
        {
            "name": "flex",
            "children": [
                {"name": "FlareVis", "value": 4116}
            ]
        },
        {
            "name": "physics",
            "children": [
                {"name": "DragForce", "value": 1082},
                {"name": "GravityForce", "value": 1336},
                {"name": "IForce", "value": 319},
                {"name": "NBodyForce", "value": 10498},
                {"name": "Particle", "value": 2822},
                {"name": "Simulation", "value": 9983},
                {"name": "Spring", "value": 2213},
                {"name": "SpringForce", "value": 1681}
            ]
        },
        {
            "name": "query",
            "children": [
                {"name": "AggregateExpression", "value": 1616},
                {"name": "And", "value": 1027},
                {"name": "Arithmetic", "value": 3891},
                {"name": "Average", "value": 891},
                {"name": "BinaryExpression", "value": 2893},
                {"name": "Comparison", "value": 5103},
                {"name": "CompositeExpression", "value": 3677},
                {"name": "Count", "value": 781},
                {"name": "DateUtil", "value": 4141},
                {"name": "Distinct", "value": 933},
                {"name": "Expression", "value": 5130},
                {"name": "ExpressionIterator", "value": 3617},
                {"name": "Fn", "value": 3240},
                {"name": "If", "value": 2732},
                {"name": "IsA", "value": 2039},
                {"name": "Literal", "value": 1214},
                {"name": "Match", "value": 3748},
                {"name": "Maximum", "value": 843},
                {
                    "name": "methods",
                    "children": [
                        {"name": "add", "value": 593},
                        {"name": "and", "value": 330},
                        {"name": "average", "value": 287},
                        {"name": "count", "value": 277},
                        {"name": "distinct", "value": 292},
                        {"name": "div", "value": 595},
                        {"name": "eq", "value": 594},
                        {"name": "fn", "value": 460},
                        {"name": "gt", "value": 603},
                        {"name": "gte", "value": 625},
                        {"name": "iff", "value": 748},
                        {"name": "isa", "value": 461},
                        {"name": "lt", "value": 597},
                        {"name": "lte", "value": 619},
                        {"name": "max", "value": 283},
                        {"name": "min", "value": 283},
                        {"name": "mod", "value": 591},
                        {"name": "mul", "value": 603},
                        {"name": "neq", "value": 599},
                        {"name": "not", "value": 386},
                        {"name": "or", "value": 323},
                        {"name": "orderby", "value": 307},
                        {"name": "range", "value": 772},
                        {"name": "select", "value": 296},
                        {"name": "stddev", "value": 363},
                        {"name": "sub", "value": 600},
                        {"name": "sum", "value": 280},
                        {"name": "update", "value": 307},
                        {"name": "variance", "value": 335},
                        {"name": "where", "value": 299},
                        {"name": "xor", "value": 354},
                        {"name": "_", "value": 264}
                    ]
                },
                {"name": "Minimum", "value": 843},
                {"name": "Not", "value": 1554},
                {"name": "Or", "value": 970},
                {"name": "Query", "value": 13896},
                {"name": "Range", "value": 1594},
                {"name": "StringUtil", "value": 4130},
                {"name": "Sum", "value": 791},
                {"name": "Variable", "value": 1124},
                {"name": "Variance", "value": 1876},
                {"name": "Xor", "value": 1101}
            ]
        },
        {
            "name": "scale",
            "children": [
                {"name": "IScaleMap", "value": 2105},
                {"name": "LinearScale", "value": 1316},
                {"name": "LogScale", "value": 3151},
                {"name": "OrdinalScale", "value": 3770},
                {"name": "QuantileScale", "value": 2435},
                {"name": "QuantitativeScale", "value": 4839},
                {"name": "RootScale", "value": 1756},
                {"name": "Scale", "value": 4268},
                {"name": "ScaleType", "value": 1821},
                {"name": "TimeScale", "value": 5833}
            ]
        },
        {
            "name": "util",
            "children": [
                {"name": "Arrays", "value": 8258},
                {"name": "Colors", "value": 10001},
                {"name": "Dates", "value": 8217},
                {"name": "Displays", "value": 12555},
                {"name": "Filter", "value": 2324},
                {"name": "Geometry", "value": 10993},
                {
                    "name": "heap",
                    "children": [
                        {"name": "FibonacciHeap", "value": 9354},
                        {"name": "HeapNode", "value": 1233}
                    ]
                },
                {"name": "IEvaluable", "value": 335},
                {"name": "IPredicate", "value": 383},
                {"name": "IValueProxy", "value": 874},
                {
                    "name": "math",
                    "children": [
                        {"name": "DenseMatrix", "value": 3165},
                        {"name": "IMatrix", "value": 2815},
                        {"name": "SparseMatrix", "value": 3366}
                    ]
                },
                {"name": "Maths", "value": 17705},
                {"name": "Orientation", "value": 1486},
                {
                    "name": "palette",
                    "children": [
                        {"name": "ColorPalette", "value": 6367},
                        {"name": "Palette", "value": 1229},
                        {"name": "ShapePalette", "value": 2059},
                        {"name": "SizePalette", "value": 2291}
                    ]
                },
                {"name": "Property", "value": 5559},
                {"name": "Shapes", "value": 19118},
                {"name": "Sort", "value": 6887},
                {"name": "Stats", "value": 6557},
                {"name": "Strings", "value": 22026}
            ]
        },
        {
            "name": "vis",
            "children": [
                {
                    "name": "axis",
                    "children": [
                        {"name": "Axes", "value": 1302},
                        {"name": "Axis", "value": 24593},
                        {"name": "AxisGridLine", "value": 652},
                        {"name": "AxisLabel", "value": 636},
                        {"name": "CartesianAxes", "value": 6703}
                    ]
                },
                {
                    "name": "controls",
                    "children": [
                        {"name": "AnchorControl", "value": 2138},
                        {"name": "ClickControl", "value": 3824},
                        {"name": "Control", "value": 1353},
                        {"name": "ControlList", "value": 4665},
                        {"name": "DragControl", "value": 2649},
                        {"name": "ExpandControl", "value": 2832},
                        {"name": "HoverControl", "value": 4896},
                        {"name": "IControl", "value": 763},
                        {"name": "PanZoomControl", "value": 5222},
                        {"name": "SelectionControl", "value": 7862},
                        {"name": "TooltipControl", "value": 8435}
                    ]
                },
                {
                    "name": "data",
                    "children": [
                        {"name": "Data", "value": 20544},
                        {"name": "DataList", "value": 19788},
                        {"name": "DataSprite", "value": 10349},
                        {"name": "EdgeSprite", "value": 3301},
                        {"name": "NodeSprite", "value": 19382},
                        {
                            "name": "render",
                            "children": [
                                {"name": "ArrowType", "value": 698},
                                {"name": "EdgeRenderer", "value": 5569},
                                {"name": "IRenderer", "value": 353},
                                {"name": "ShapeRenderer", "value": 2247}
                            ]
                        },
                        {"name": "ScaleBinding", "value": 11275},
                        {"name": "Tree", "value": 7147},
                        {"name": "TreeBuilder", "value": 9930}
                    ]
                },
                {
                    "name": "events",
                    "children": [
                        {"name": "DataEvent", "value": 2313},
                        {"name": "SelectionEvent", "value": 1880},
                        {"name": "TooltipEvent", "value": 1701},
                        {"name": "VisualizationEvent", "value": 1117}
                    ]
                },
                {
                    "name": "legend",
                    "children": [
                        {"name": "Legend", "value": 20859},
                        {"name": "LegendItem", "value": 4614},
                        {"name": "LegendRange", "value": 10530}
                    ]
                },
                {
                    "name": "operator",
                    "children": [
                        {
                            "name": "distortion",
                            "children": [
                                {"name": "BifocalDistortion", "value": 4461},
                                {"name": "Distortion", "value": 6314},
                                {"name": "FisheyeDistortion", "value": 3444}
                            ]
                        },
                        {
                            "name": "encoder",
                            "children": [
                                {"name": "ColorEncoder", "value": 3179},
                                {"name": "Encoder", "value": 4060},
                                {"name": "PropertyEncoder", "value": 4138},
                                {"name": "ShapeEncoder", "value": 1690},
                                {"name": "SizeEncoder", "value": 1830}
                            ]
                        },
                        {
                            "name": "filter",
                            "children": [
                                {"name": "FisheyeTreeFilter", "value": 5219},
                                {"name": "GraphDistanceFilter", "value": 3165},
                                {"name": "VisibilityFilter", "value": 3509}
                            ]
                        },
                        {"name": "IOperator", "value": 1286},
                        {
                            "name": "label",
                            "children": [
                                {"name": "Labeler", "value": 9956},
                                {"name": "RadialLabeler", "value": 3899},
                                {"name": "StackedAreaLabeler", "value": 3202}
                            ]
                        },
                        {
                            "name": "layout",
                            "children": [
                                {"name": "AxisLayout", "value": 6725},
                                {"name": "BundledEdgeRouter", "value": 3727},
                                {"name": "CircleLayout", "value": 9317},
                                {"name": "CirclePackingLayout", "value": 12003},
                                {"name": "DendrogramLayout", "value": 4853},
                                {"name": "ForceDirectedLayout", "value": 8411},
                                {"name": "IcicleTreeLayout", "value": 4864},
                                {"name": "IndentedTreeLayout", "value": 3174},
                                {"name": "Layout", "value": 7881},
                                {"name": "NodeLinkTreeLayout", "value": 12870},
                                {"name": "PieLayout", "value": 2728},
                                {"name": "RadialTreeLayout", "value": 12348},
                                {"name": "RandomLayout", "value": 870},
                                {"name": "StackedAreaLayout", "value": 9121},
                                {"name": "TreeMapLayout", "value": 9191}
                            ]
                        },
                        {"name": "Operator", "value": 2490},
                        {"name": "OperatorList", "value": 5248},
                        {"name": "OperatorSequence", "value": 4190},
                        {"name": "OperatorSwitch", "value": 2581},
                        {"name": "SortOperator", "value": 2023}
                    ]
                },
                {"name": "Visualization", "value": 16540}
            ]
        }
    ]
}

export function plotlyExample(incomingData, frequencyData) {

    var container = document.getElementById('plotlyExample');
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
    
    // var trace1 = {
    //     x: incomingData[0].map(x => x.timestamp),
    //     y: incomingData[0].map(y => y.type),
    //     mode: 'markers',
    //     type: 'scatter',
    //     name: incomingData[0][0].type,
    //     text: incomingData[0].map(t => t.title),
    //     marker: { 
    //         size: 12,
    //         symbol: 'diamond'
    //     }
    // };

    // var trace2 = {
    //     x: [1.5, 2.5, 3.5, 4.5, 5.5],
    //     y: [4, 1, 7, 1, 4],
    //     mode: 'markers',
    //     type: 'scatter',
    //     name: 'Team B',
    //     text: ['B-a', 'B-b', 'B-c', 'B-d', 'B-e'],
    //     marker: { size: 12 }
    // };

    // var data = [ trace1 ];

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
    
    Plotly.newPlot("plotlyExample", data, layout);
}