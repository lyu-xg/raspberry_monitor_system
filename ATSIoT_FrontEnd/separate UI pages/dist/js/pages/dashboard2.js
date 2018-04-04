$(function () {

  'use strict';

  /* ChartJS
   * -------
   * Here we will create a few charts using ChartJS
   */

  //-----------------------
  //- MONTHLY SALES CHART -
  //-----------------------

  // Get context with jQuery - using jQuery's .get() method.
  window.onload = function () {
  var chart = new CanvasJS.Chart("deviceChart",
  {
	title:{
	  text: "Converting in Local Time"
	},

	axisX:{
	  title: "time",
	  gridThickness: 2,
	  interval:2,
	  intervalType: "hour",
	  valueFormatString: "hh TT K",
	  labelAngle: -20
	},
	axisY:{
	  title: "distance"
	},
	data: [
	{
	  type: "line",
	  dataPoints: [//array
	  {x: new Date(Date.UTC (2012, 01, 1, 1,0) ), y: 26 },
	  {x: new Date( Date.UTC (2012, 01, 1,2,0) ), y: 38  },
	  {x: new Date( Date.UTC(2012, 01, 1,3,0) ), y: 43 },
	  {x: new Date( Date.UTC(2012, 01, 1,4,0) ), y: 29},
	  {x: new Date( Date.UTC(2012, 01, 1,5,0) ), y: 41},
	  {x: new Date( Date.UTC(2012, 01, 1,6,0) ), y: 54},
	  {x: new Date( Date.UTC(2012, 01, 1,7,0) ), y: 66},
	  {x: new Date( Date.UTC(2012, 01, 1,8,0) ), y: 60},
	  {x: new Date( Date.UTC(2012, 01, 1,9,0) ), y: 53},
	  {x: new Date( Date.UTC(2012, 01, 1,10,0) ), y: 60}
	  ]
	}
	]
  });

  chart.render();
}
});
