var map;
var markers = [];

function addOpportunityMarker(lat, lng, name, desc, dateTime, address, organizationName, category, cause, imageUrl, id) {
    var marker = map.addMarker({
        lat: lat,
        lng: lng,
        title: name,
        infoWindow: { content: name },
        click: function (e) {
            $("#OpportunityId").val(id);
            $("#OpportunityModalTitle").html(name);
            $("#OpportunityModalCategory").html(category);
            $("#OpportunityModalCause").html(cause);
            $("#OpportunityModalTime").html(dateTime);
            $("#OpportunityModalOrganization").html(organizationName);
            $("#OpportunityModalAddress").html(address);
            $("#OpportunityModalDescription").html(desc);
            $("#OpportunityModal").modal('show');
        }
    });
    markers.push(marker);
    appendOpportunityPanel(name, dateTime, address, organizationName, category, cause, imageUrl, id);
};

function appendOpportunityPanel(name, dateTime, address, organizationName, category, cause, imageUrl, id) {
    $("#opportunityList").append('<div class="col-lg-4 col-md-6 col-sm-12">' +
        '<img  src="' + imageUrl + '" />' +
        '<br/>' + name +
        '<br/>' + organizationName +
        '<br/>' + dateTime +
        '<br/>' + address +
        '<br/>' + category +
        '<br/>' +
        '<br/>' +
        '</div>');
};

function initMap() {
    map = new GMaps({
        div: '#map',
        lat: 51.044308,
        lng: -114.0652801,
        zoom: 12
    });
    getAllOpportunities();
};

function getAllOpportunities() {
    $.getJSON(
        '/api/Opportunities',
        function (opportunities) {
            addOpportunityMarkers(opportunities);
        });
};

function clearOpportunities() {
    deleteMarkers();
    $("#opportunityList").empty();
};

function addOpportunityMarkers(opportunities) {
    for (var i = 0; i < opportunities.length; i++) {
        var opportunity = opportunities[i];
        if (!opportunity.location) {
            continue;
        }

        var date = new Date(opportunity.dateTime);

        var causename = "";
        if (opportunity.organization.cause) {
            causename = opportunity.organization.cause.name;
        }
        addOpportunityMarker(opportunity.location.latitude,
            opportunity.location.longitude,
            opportunity.name,
            opportunity.description,
            date.toDateString() + " " + date.toLocaleTimeString(),
            opportunity.address,
            opportunity.organization.name,
            opportunity.category.name,
            causename,
            opportunity.imageUrl,
            opportunity.id
        );
    }
};

function deleteMarkers() {
    clearMarkers();
    markers = [];
};

function clearMarkers() {
    setMapOnAll(null);
};

function setMapOnAll(map) {
    for (var i = 0; i < markers.length; i++) {
        markers[i].setMap(map);
    }
};

$("#FilterOpportunities").click(function () {
    var categoryIds = $("#CategoryList").val();
    var causeIds = $("#CausesList").val();
    var startDate = $("#StartDate").val();
    var endDate = $("#EndDate").val();

    if (!startDate) {
        startDate = undefined;
    }

    if (!endDate) {
        endDate = undefined;
    }

    var data = {
        "CategoryIds": categoryIds,
        "CauseIds": causeIds,
        "StartDate": startDate,
        "EndDate": endDate
    };

    $.ajax({
        type: "POST",
        contentType: "application/json",
        url: '/api/Opportunities/Search',
        data: JSON.stringify(data),
        dataType: "json",
        success: function (opportunities) {
            clearOpportunities();
            addOpportunityMarkers(opportunities);
        }
    });
});