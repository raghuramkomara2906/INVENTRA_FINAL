// charts.js
document.addEventListener('DOMContentLoaded', function() {
    // common options for all charts
    const commonOptions = {
      responsive: true,
      maintainAspectRatio: false,
      animation: {
        duration: 800,
        easing: 'easeOutQuart'
      },
      legend: {
        display: true,
        position: 'top',
        labels: { fontSize: 14 },
        // toggle dataset on legend click
        onClick: function(e, legendItem) {
          const chart = this.chart;
          const idx = legendItem.datasetIndex;
          chart.getDatasetMeta(idx).hidden = !chart.isDatasetVisible(idx);
          chart.update();
        }
      },
      tooltips: {
        enabled: true,
        mode: 'index',
        intersect: false,
        callbacks: {
          // you can customize tooltip labels here
          label: function(tooltipItem, data) {
            return data.datasets[tooltipItem.datasetIndex].label + 
                   ': ' + tooltipItem.yLabel;
          }
        }
      },
      hover: {
        mode: 'nearest',
        intersect: true
      }
    };
  
    // 1) Bar chart
    new Chart(document.getElementById('turnoverChart').getContext('2d'), {
      type: 'bar',
      data: {
        labels: ['Jan','Feb','Mar','Apr','May','Jun'],
        datasets: [{
          label: 'Inventory Turnover',
          data: [4.2,3.8,4.5,4.0,4.7,5.1],
          backgroundColor: 'rgba(78,137,174,0.6)'
        }]
      },
      options: Object.assign({
        title: { display: true, text: 'Monthly Inventory Turnover', fontSize: 18 },
        scales: {
          yAxes: [{ ticks: { beginAtZero: true }, gridLines: { color: '#eee' } }],
          xAxes: [{ gridLines: { display: false } }]
        }
      }, commonOptions)
    });
  
    // 2) Pie chart
    new Chart(document.getElementById('categoryChart').getContext('2d'), {
      type: 'pie',
      data: {
        labels: ['Electronics','Apparel','Home Goods','Office Supplies','Other'],
        datasets: [{
          data: [30,20,25,15,10],
          backgroundColor: [
            'rgba(78,137,174,0.7)',
            'rgba(242,177,52,0.7)',
            'rgba(136,78,174,0.7)',
            'rgba(174,78,112,0.7)',
            'rgba(78,174,104,0.7)'
          ]
        }]
      },
      options: Object.assign({
        title: { display: true, text: 'In-Stock Items by Category', fontSize: 18 },
        legend: { position: 'bottom' }
      }, commonOptions)
    });
  
    // 3) Line chart
    new Chart(document.getElementById('forecastChart').getContext('2d'), {
      type: 'line',
      data: {
        labels: ['Week 1','Week 2','Week 3','Week 4','Week 5','Week 6'],
        datasets: [
          {
            label: 'Forecast Demand',
            data: [150,170,160,180,175,190],
            borderColor: 'rgba(242,177,52,1)',
            backgroundColor: 'rgba(242,177,52,0.2)',
            pointRadius: 5,
            fill: true
          },
          {
            label: 'Actual Sales',
            data: [140,165,155,185,170,200],
            borderColor: 'rgba(78,137,174,1)',
            backgroundColor: 'rgba(78,137,174,0.2)',
            pointRadius: 5,
            fill: true
          }
        ]
      },
      options: Object.assign({
        title: { display: true, text: 'Weekly Forecast vs Actual Sales', fontSize: 18 },
        scales: {
          yAxes: [{ ticks: { beginAtZero: false }, gridLines: { color: '#eee' } }],
          xAxes: [{ gridLines: { color: '#eee' } }]
        }
      }, commonOptions)
    });
  });
  