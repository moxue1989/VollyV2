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
    return marker;
};

function openOpportunityModal(opportunity) {
    var causename = "";
    if (opportunity.organization.cause) {
        causename = opportunity.organization.cause.name;
    }
    var dateTime = new Date(opportunity.dateTime);
    var dateTimeString = "Coming soon!";
    if (dateTime.getFullYear() >= 1970) {
        dateTimeString = moment(opportunity.dateTime).format('ddd MMM D YYYY h:mm a');
    }
    $("#OpportunityId").val(opportunity.id);
    $("#OpportunityModalTitle").html(opportunity.name);
    $("#OpportunityModalCategory").html(opportunity.category.name);
    $("#OpportunityModalCause").html(causename);
    $("#OpportunityModalTime").html(dateTimeString);
    $("#OpportunityModalOrganization").html(opportunity.organization.name);
    $("#OpportunityModalOrganizationUrl").attr("href", opportunity.organization.websiteLink);
    $("#OpportunityModalAddress").html(opportunity.address);
    $("#OpportunityModalDescription").html(opportunity.description);
    $("#OpportunityModalSpotsRemaining").html(opportunity.openings + ' spots remaining');
    $("#fb-share").attr("href", "https://www.facebook.com/sharer/sharer.php?u=https%3A%2F%2Fvolly.azurewebsites.net%2FOpportunities%2FDetails%2F" + opportunity.id)
    $("#tw-share").attr("href", "https://twitter.com/share?url=https%3A%2F%2Fvolly.azurewebsites.net%2FOpportunities%2FDetails%2F" + opportunity.id + "&text=Volly - " + opportunity.name);
    $("#OpportunityModal").modal('show');
};

function appendOpportunityPanel(opportunity, marker) {
    var dateTime = new Date(opportunity.dateTime);
    var dateTimeString = "Coming soon!";
    if (dateTime.getFullYear() >= 1970) {
        dateTimeString = moment(opportunity.dateTime).format('ddd MMM D YYYY h:mm a');
    }
    $("#opportunityList").append('<div id="opportunity-' + opportunity.id + '" class="col-lg-4 col-md-6 col-sm-12 result-card"><div class="result-card-inner">' +
        '<div class="wrap-center"><div class="result-datetime">' + dateTimeString + '</div></div>' +
        '<img src="' + opportunity.imageUrl + '" />' +
        '<div class="result-details"><div class="result-address">' + opportunity.address + '</div>' +
        '<div class="result-org-name">' + opportunity.organization.name + '</div>' +
        '<div class="result-name">' + opportunity.name + '</div>' +
        '</div></div></div>');
    $("#opportunity-" + opportunity.id).click(function (e) {
        openOpportunityModal(opportunity);
    });
    $("#opportunity-" + opportunity.id)
        .hover(function (e) {
            marker.setAnimation(google.maps.Animation.BOUNCE);
        }, function (e) {
            marker.setAnimation(null);
        });
};

function initMap() {
    map = new GMaps({
        div: '#map',
        lat: 51.044308,
        lng: -114.0652801,
        zoom: 10
    });
    $('#nothingFoundAlert').hide();
    getAllOpportunities();
    enableDickPicker();
};

function enableDickPicker() {
    $('#dateSelect').datepicker({
        multidate: true,
        clearBtn: true
    });
}

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
        var marker = addOpportunityMarker(opportunity);
        appendOpportunityPanel(opportunity, marker);
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
    var dates = $('#dateSelect').datepicker("getDates");

    var data = {
        "CategoryIds": categoryIds,
        "CauseIds": causeIds,
        "Dates": dates
    };

    $.ajax({
        type: "POST",
        contentType: "application/json",
        url: '/api/Opportunities/Search',
        data: JSON.stringify(data),
        dataType: "json",
        success: function (opportunities) {
            clearOpportunities();
            if (opportunities.length === 0) {
                $('#nothingFoundAlert').show();
            } else {
                $('#nothingFoundAlert').hide();
                addOpportunityMarkers(opportunities);
            }
        }
    });
});