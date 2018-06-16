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
    var baseUrl = "https%3A%2F%2Fvolly.azurewebsites.net%2FOpportunities%2FDetails%2F";
    var causename = "";
    if (opportunity.organization.cause) {
        causename = opportunity.organization.cause.name;
    }
    $("#OpportunityId").val(opportunity.id);
    $("#OpportunityModalTitle").html(opportunity.name);
    $("#OpportunityModalCategory").html(opportunity.category.name);
    $("#OpportunityModalCause").html(causename);
    $("#OpportunityModalTime").html(prettyFormatDateTimes(opportunity.dateTime, opportunity.endDateTime));
    $("#OpportunityModalOrganization").html(opportunity.organization.name);
    $("#OpportunityModalOrganizationUrl").attr("href", opportunity.organization.websiteLink);
    $("#ModalAddress").html(opportunity.address);
    $("#ModalDescription").html(opportunity.description);
    $("#OpportunityModalSpotsRemaining").html(opportunity.openings + ' spots remaining');
    $("#fb-share").attr("href", "https://www.facebook.com/sharer/sharer.php?u=" + baseUrl + opportunity.id);
    $("#tw-share").attr("href", "https://twitter.com/share?url=" + baseUrl + opportunity.id + "&text=Volly - " + opportunity.name);
    $("#OpportunityModal").modal('show');
};

function prettyFormatDateTimes(d1, d2, breakline) {
    var dateTime = new Date(d1);
    var endDateTime = new Date(d2);
    var dateTimeString = "Coming soon!";
    if (dateTime.getFullYear() >= 1970) {
        if (endDateTime.getFullYear() >= 1970) {
            if (dateTime.getFullYear() == endDateTime.getFullYear()
                && dateTime.getMonth() == endDateTime.getMonth()
                && dateTime.getDay() == endDateTime.getDay()) {
                dateTimeString = moment(dateTime).format('ddd MMM D YYYY h:mm a') + " - " + moment(endDateTime).format('h:mm a');
            } else {
                dateTimeString = moment(dateTime).format('ddd MMM D YYYY h:mm a') + getSplit(breakline) + moment(endDateTime).format('ddd MMM D YYYY h:mm a')
            }
        } else {
            dateTimeString = moment(dateTime).format('ddd MMM D YYYY h:mm a');
        }
    }
    return dateTimeString;
    function getSplit(breakline) {
        if (breakline) {
            return " -<br />";
        }
        return " - ";
    }
}

function appendOpportunityPanel(opportunity, marker) {
    $("#opportunityList").append('<div id="opportunity-' + opportunity.id + '" class="col-lg-3 col-md-4 col-sm-12 result-card"><div class="result-card-inner">' +
        '<div class="wrap-center"><div class="result-datetime">' + prettyFormatDateTimes(opportunity.dateTime, opportunity.endDateTime, true) + '</div></div>' +
        '<div class="img-opp"><img src="' + opportunity.imageUrl + '" /></div>' +
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
    enableDatePicker();
};

function enableDatePicker() {
    $('#dateSelect').datepicker({
        multidate: true,
        clearBtn: true,
        todayHighlight: true
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

$("#ClearFilters").click(function () {
    $("#CategoryList").val("");
    $("#CausesList").val("");
    $("#OrganizationList").val("");
    $("#EventSort").prop("checked", true);
    $('#dateSelect').val("").datepicker("update");
    filterOpportunities();
});

$("#FilterOpportunities").click(filterOpportunities);

function filterOpportunities() {
    var categoryIds = $("#CategoryList").val();
    var causeIds = $("#CausesList").val();
    var organizationIds = $("#OrganizationList").val();
    var dates = $('#dateSelect').datepicker("getDates");
    var sortBy = $('input[name="sortRadio"]:checked').val();

    var data = {
        "CategoryIds": categoryIds,
        "CauseIds": causeIds,
        "OrganizationIds": organizationIds,
        "Dates": dates,
        "Sort": sortBy
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
}

$("#toggle-map").click(function () {
    $("#map").animate({
        width: "toggle",
        height: "toggle"
    }, 500, function () {
        if (parseInt($("#toggle-map").attr('data-show')) === 0) {
            $("#toggle-map").attr('value', 'Hide Map');
            $("#toggle-map").attr('data-show', '1');
            $("#wrap-main").removeClass('col-lg-12');
            $("#wrap-main").addClass('col-lg-6');
            $("#wrap-main").removeClass('col-md-12');
            $("#wrap-main").addClass('col-md-6');
            $(".result-card").each(function () {
                $(this).removeClass('col-lg-3');
                $(this).addClass('col-lg-6');
                $(this).removeClass('col-md-4');
                $(this).addClass('col-md-6');
            });
        } else {
            $("#toggle-map").attr('value', 'Show Map');
            $("#toggle-map").attr('data-show', '0');
            $("#wrap-main").removeClass('col-lg-6');
            $("#wrap-main").addClass('col-lg-12');
            $("#wrap-main").removeClass('col-md-6');
            $("#wrap-main").addClass('col-md-12');
            $(".result-card").each(function () {
                $(this).removeClass('col-lg-6');
                $(this).addClass('col-lg-3');
                $(this).removeClass('col-md-6');
                $(this).addClass('col-md-4');
            });
        }
    });
});
(function () {
    $("#map").animate({
        width: "toggle",
        height: "toggle"
    }, 100, function () {
    })
})();