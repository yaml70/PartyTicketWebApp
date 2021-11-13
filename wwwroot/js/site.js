// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

$(document).ready(function () {
    $("#GenreStatsContainer").ready(function () {
        $.ajax({
            type: "GET",
            url: "/Manage/GenreStatistics",
            contentType: "application/json",
            dataType: "json",
            success: data => {
                createD3BarChart(data, "GenreStatsContainer");
            }
        });
    });

    $("#ClubStatsContainer").ready(function () {
        $.ajax({
            type: "GET",
            url: "/Manage/ClubStatistics",
            contentType: "application/json",
            dataType: "json",
            success: data => {

                createD3BarChart(data, "ClubStatsContainer");
            }
        });
    });


    $("AreaStatsContainer").ready(function () {
        $.ajax({
            type: "GET",
            url: "/Manage/AreaStatistics",
            contentType: "application/json",
            dataType: "json",
            success: data => {

                createD3BarChart(data, "AreaStatsContainer");
            }
        });
    });
});


function createD3BarChart(data, id) {
    const dataKeys = Object.keys(data);

    const dataToDisplay = dataKeys.map(key => ({ genre: key, number: data[key] }))

    var margin = { top: 20, right: 20, bottom: 100, left: 60 },
        width = 600 - margin.left - margin.right,
        height = 280;
    var x = d3.scale.ordinal().rangeRoundBands([0, width], 0.5);
    var y = d3.scale.linear().range([height, 0]);
 
    //draw axis
    var xAxis = d3.svg.axis()
        .scale(x)
        .orient("bottom");

    var yAxis = d3.svg.axis().tickFormat(d3.format("d"))
        .scale(y)
        .orient("left")
        .ticks(dataToDisplay.length)
        .innerTickSize(-width)
        .outerTickSize(0)
        .tickPadding(10);

    var svg = d3.select("#"+id)
        .append("svg")
        .attr("width", width + margin.left + margin.right)
        .attr("height", height + margin.top + margin.bottom)
        .append("g")
        .attr("transform", "translate(60,20)"); 

    x.domain(dataToDisplay.map(data => data.genre));

    y.domain([0, d3.max(dataToDisplay, data => data.number)]);

    svg.append("g")
        .attr("class", "x axis")
        .attr("transform", "translate(0,280)")
        .call(xAxis)
        .selectAll("text")
        .style("text-anchor", "middle")
        .attr("dx", "-2.5em")
        .attr("dy", "-.55em")
        .attr("y", 30)
        .attr("transform", "rotate(-40)")
        .style("stroke", "#69a3b2");

    svg.append("g")
        .attr("class", "y axis")
        .call(yAxis)
        .selectAll("text")
        .attr("transform", "rotate(-90)")
        .attr("y", 5)
        .attr("dy", "0.8em")
        .attr("text-anchor", "end")
        .style("stroke", "#69a3b2");

    svg.selectAll("bar")
        .data(dataToDisplay)
        .enter()
        .append("rect")
        .style({ "fill": " #00ffea", "shape-rendering": "crispEdges" })
        .attr("x", data => x(data.genre))
        .attr("width", x.rangeBand())
        .attr("y", data => y(data.number))
        .attr("height", data => (height - y(data.number)));
}





    $(document).ready(function () {
        // Add smooth scrolling to all links in navbar + footer link
        $(".navbar a, footer a[href='#myPage']").on('click', function (event) {
            // Make sure this.hash has a value before overriding default behavior
            if (this.hash !== "") {
                // Prevent default anchor click behavior
                event.preventDefault();
                // Store hash
                var hash = this.hash;
                // Using jQuery's animate() method to add smooth page scroll
                // The optional number (900) specifies the number of milliseconds it takes to scroll to the specified area
                $('html, body').animate({
                    scrollTop: $(hash).offset().top
                }, 900, function () {
                    // Add hash (#) to URL when done scrolling (default click behavior)
                    window.location.hash = hash;
                });
            } // End if
        });

        $(window).scroll(function () {
            $(".slideanim").each(function () {
                var pos = $(this).offset().top;
                var winTop = $(window).scrollTop();
                if (pos < winTop + 600) {
                    $(this).addClass("slide");
                }
            });
        });
    })

    $(document).ready(function () {
        $('#btnAdd').click(function () {
            var num = $('.clonedInput').length; // how many "duplicatable" input fields we currently have
            var newNum = new Number(num + 1);      // the numeric ID of the new input field being added
            // create the new element via clone(), and manipulate it's ID using newNum value
            var newElem = $('#input' + num).clone().attr('id', 'input' + newNum);
            // manipulate the name/id values of the input inside the new element
            newElem.children('input[type=text]:first').attr('id', 'name' + newNum).attr('name', 'name' + newNum);
            newElem.children('input[type=checkbox]:first').attr('id', 'chk' + newNum).attr('name', 'chk' + newNum);
            // insert the new element after the last "duplicatable" input field
            $('#input' + num).after(newElem);
            // enable the "remove" button
            $('#btnDel').attr('disabled', '');
            // business rule: you can only add 5 names
            if (newNum == 5)
                $('#btnAdd').attr('disabled', 'disabled');
        });

        $('#btnDel').click(function () {
            var num = $('.clonedInput').length; // how many "duplicatable" input fields we currently have
            $('#input' + num).remove();     // remove the last element
            // enable the "add" button
            $('#btnAdd').attr('disabled', '');
            // if only one element remains, disable the "remove" button
            if (num - 1 == 1)
                $('#btnDel').attr('disabled', 'disabled');
        });
        $('#btnDel').attr('disabled', 'disabled');
    });

