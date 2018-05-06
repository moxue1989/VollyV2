var map;
var markers = [];

function addOpportunityMarker(opportunity) {
    var marker = map.addMarker({
        lat: opportunity.location.latitude,
        lng: opportunity.location.longitude,
        title: opportunity.name,
        infoWindow: { content: opportunity.name },
        click: function (e) {
            openOpportunityModal(opportunity);
        }
    });
    markers.push(marker);
};

function openOpportunityModal(opportunity) {
    var causename = "";
    if (opportunity.organization.cause) {
        causename = opportunity.organization.cause.name;
    }
    var date = new Date(opportunity.dateTime);
    $("#OpportunityId").val(opportunity.id);
    $("#OpportunityModalTitle").html(opportunity.name);
    $("#OpportunityModalCategory").html(opportunity.category.name);
    $("#OpportunityModalCause").html(causename);
    $("#OpportunityModalTime").html(date.toDateString() + " " + date.toLocaleTimeString());
    $("#OpportunityModalOrganization").html(opportunity.organization.name);
    $("#OpportunityModalAddress").html(opportunity.address);
    $("#OpportunityModalDescription").html(opportunity.description);
    $("#OpportunityModal").modal('show');
};

function appendOpportunityPanel(opportunity) {
    $("#opportunityList").append('<div id="opportunity-' + opportunity.id + '" class="col-lg-4 col-md-6 col-sm-12 opportunity-card">' +
        '<img  src="' + opportunity.imageUrl + '" />' +
        '<br/>' + opportunity.name +
        '<br/>' + opportunity.organization.name +
        '<br/>' + opportunity.dateTime +
        '<br/>' + opportunity.address +
        '<br/>' + opportunity.category.name +
        '</div>');
    $("#opportunity-" + opportunity.id).click(function(e) {
        openOpportunityModal(opportunity);
    });
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
        addOpportunityMarker(opportunity);
        appendOpportunityPanel(opportunity);
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