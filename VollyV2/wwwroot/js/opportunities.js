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
    var dateTime = new Date(opportunity.dateTime);
    var dateTimeString = "Coming soon!";
    if (dateTime.getFullYear() >= 1970) {
        dateTimeString = dateTime.toDateString() + " " + dateTime.toLocaleTimeString();
    }
    $("#OpportunityId").val(opportunity.id);
    $("#OpportunityModalTitle").html(opportunity.name);
    $("#OpportunityModalCategory").html(opportunity.category.name);
    $("#OpportunityModalCause").html(causename);
    $("#OpportunityModalTime").html(dateTimeString);
    $("#OpportunityModalOrganization").html(opportunity.organization.name);
    $("#OpportunityModalAddress").html(opportunity.address);
    $("#OpportunityModalDescription").html(opportunity.description);
    $("#OpportunityModal").modal('show');
};

function appendOpportunityPanel(opportunity) {
    var dateTime = new Date(opportunity.dateTime);
    var dateTimeString = "Coming soon!";
    if (dateTime.getFullYear() >= 1970) {
        dateTimeString = dateTime.toDateString() + " " + dateTime.toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' });
    }
    $("#opportunityList").append('<div id="opportunity-' + opportunity.id + '" class="col-lg-4 col-md-6 col-sm-12 result-card"><div class="result-card-inner">' +
        '<img  src="' + opportunity.imageUrl + '" />' +
        '<div class="result-details"><div class="result-datetime">' + dateTimeString + '</div>' +
        '<div class="result-address">' + opportunity.address + '</div>' +
        '<div class="result-name">' + opportunity.name + '</div>' +
        '<div class="result-org-name">' + opportunity.organization.name + '</div>' +
        '</div></div></div>');
    $("#opportunity-" + opportunity.id).click(function (e) {
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
            initOpportunities(opportunities);
        });
};

function clearOpportunities() {
    deleteMarkers();
    $("#opportunityList").empty();
};

function initOpportunities(opportunities) {
    addOpportunityMarkers(opportunities);
    var categoryLookup = {};
    var causeLookup = {};
    for (var j = 0; j < opportunities.length; j++) {
        var opportunity = opportunities[j];
        if (!(opportunity.category.id in categoryLookup)) {
            categoryLookup[opportunity.category.id] = 1;
            $('#CategoryList').append(
                $('<option>', {
                    value: opportunity.category.id,
                    text: opportunity.category.name
                })
            );
        }
        if (!(opportunity.organization.cause.id in causeLookup)) {
            causeLookup[opportunity.organization.cause.id] = 1;
            $('#CausesList').append(
                $('<option>', {
                    value: opportunity.organization.cause.id,
                    text: opportunity.organization.cause.name
                })
            );
        }
    }
}

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