var map;
var markers = [];

function addOpportunityMarker(opportunity) {
    var marker = map.addMarker({
        lat: opportunity.latitude,
        lng: opportunity.longitude,
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
    var baseUrl = "https://volly.azurewebsites.net/Opportunities/Details/";
    $("#OpportunityId").val(opportunity.id);
    $("#OpportunityModalTitle").html(opportunity.name);
    $("#OpportunityModalCategory").html(opportunity.categoryName);
    $("#OpportunityModalCause").html(opportunity.causeName);
    $("#OpportunityModalOrganization").html(opportunity.organizationName);
    $("#OpportunityModalOrganizationUrl").attr("href", opportunity.organizationLink);
    $("#ModalAddressHref").attr("href", "https://www.google.com/maps/search/?api=1&query=" + opportunity.address);
    $("#ModalAddress").html(opportunity.address);
    $("#ModalDescription").html(opportunity.description);
    $("#occurrencesInput").html(getOccurrenceSelectors(opportunity.occurrenceViews));
    $("#fb-share").attr("href", "https://www.facebook.com/sharer/sharer.php?u=" + baseUrl + opportunity.id);
    $("#tw-share").attr("href", "https://twitter.com/share?url=" + baseUrl + opportunity.id + "&text=Volly - " + opportunity.name);
    $("#ln-share").attr("href", baseUrl + opportunity.id);
    document.getElementById("ln-share").innerHTML = baseUrl + opportunity.id;
//    if (opportunity.externalSignUpUrl) {
//        $("#OpportunityModalExternalSignUpUrlHref").attr("href", opportunity.externalSignUpUrl);
//        $("#OpportunityModalExternalSignUpUrl").html(opportunity.externalSignUpUrl);
//        $("#OpportunityModalExternalSignUp").modal('show');
//    } else {
        $("#OpportunityModal").modal('show');
//    }
};

function prettyFormatDateTimes(d1, d2, breakline) {
    var dateTime = new Date(d1 + (moment(d1).isDST()?"-06:00":"-07:00"));
    var endDateTime = new Date(d2 + (moment(d2).isDST()?"-06:00":"-07:00"));
    var dateTimeString = "Coming soon!";
    if (dateTime.getFullYear() >= 1970) {
        if (endDateTime.getFullYear() >= 1970) {
            if (dateTime.getFullYear() === endDateTime.getFullYear()
                && dateTime.getMonth() === endDateTime.getMonth()
                && dateTime.getDay() === endDateTime.getDay()) {
                dateTimeString = moment(dateTime).format('ddd MMM D YYYY h:mm a') + " - " + moment(endDateTime).format('h:mm a');
            } else {
                dateTimeString = moment(dateTime).format('ddd MMM D YYYY h:mm a') + getSplit(breakline) + moment(endDateTime).format('ddd MMM D YYYY h:mm a')
            }
        } else {
            dateTimeString = moment(dateTime).format('ddd MMM D YYYY h:mm a');
        }
    }
    return dateTimeString;
}

function getSplit(breakline) {
    if (breakline) {
        return " -<br />";
    }
    return " - ";
}

function appendOpportunityPanel(opportunity, marker) {
    $("#opportunityList").append('<div id="opportunity-' + opportunity.id + '" class="col-xl-3 col-lg-4 col-md-6 col-sm-12 result-card"><div class="result-card-inner">' +
        '<div class="img-opp"><img src="' + opportunity.imageUrl + '" /></div>' +
        '<div class="result-details"><div class="result-address">' + opportunity.address + '</div>' +
        '<div class="result-org-name">' + opportunity.organizationName + '</div>' +
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

    if ($("#InitialOpportunity").html() == opportunity.id) {
        openOpportunityModal(opportunity);
    }
};

function getOccurrenceSelectors(occurrences) {
    var element = "";
    for (var i = 0; i < occurrences.length; i++) {
        var occurrence = occurrences[i];
        element = element + "<option value='" + occurrence.id +
            "'>" + prettyFormatDateTimes(occurrence.startTime, occurrence.endTime, false) +
            " (" + occurrence.openings + " spots remaining)</option>";
    }
    return element;
}

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
    $("#CommunityList").val("");
    $("#EventSort").prop("checked", true);
    $('#dateSelect').val("").datepicker("update");
    filterOpportunities();
});

$("#FilterOpportunities").click(filterOpportunities);

function filterOpportunities() {
    var categoryIds = $("#CategoryList").val();
    var causeIds = $("#CausesList").val();
    var organizationIds = $("#OrganizationList").val();
    var communityIds = $("#CommunityList").val();
    var dates = $('#dateSelect').datepicker("getDates");
    var sortBy = $('input[name="sortRadio"]:checked').val();

    var data = {
        "CategoryIds": categoryIds,
        "CauseIds": causeIds,
        "OrganizationIds": organizationIds,
        "CommunityIds": communityIds,
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
$("#causes-a").click(function(e){
    toggleFilterVisibility(e.target.id);
});
$("#categories-a").click(function(e){
    toggleFilterVisibility(e.target.id);
});
$("#organizations-a").click(function(e){
    toggleFilterVisibility(e.target.id);
});
$("#communities-a").click(function(e){
    toggleFilterVisibility(e.target.id);
});
function toggleFilterVisibility(filterid) {
    if ($("#" + filterid).hasClass("active")) {
        if ($("#filter-wrapper").hasClass("filter-wrapper-hide")) {
            $("#filter-wrapper").removeClass("filter-wrapper-hide")
            $("#filter-wrapper").addClass("filter-wrapper-show")
        } else {
            $("#filter-wrapper").removeClass("filter-wrapper-show")
            $("#filter-wrapper").addClass("filter-wrapper-hide")
        }
    } else {
        $("#filter-wrapper").removeClass("filter-wrapper-hide")
        $("#filter-wrapper").addClass("filter-wrapper-show")
    }
}
$("#toggle-map").click(function () {
    var dataShow = parseInt($('#toggle-map').attr('data-show'));
    if (dataShow === 1) {
        $('#map').css('height', '85vh');
    }
    $("#map").animate({
        opacity: dataShow
    }, 500, function () {
        if (dataShow === 1) {
            $("#toggle-map").attr('value', 'Hide Map');
            $("#toggle-map").attr('data-show', '0');
            $("#wrap-main").removeClass('col-lg-12');
            $("#wrap-main").addClass('col-lg-8');
            $("#wrap-main").removeClass('col-md-12');
            $("#wrap-main").addClass('col-md-8');
        } else {
            $("#toggle-map").attr('value', 'Show Map');
            $("#toggle-map").attr('data-show', '1');
            $("#wrap-main").removeClass('col-lg-8');
            $("#wrap-main").addClass('col-lg-12');
            $("#wrap-main").removeClass('col-md-8');
            $("#wrap-main").addClass('col-md-12');
            $('#map').css('height', 0);
        }
    });
});

(function () {
    $("#map").css("opacity", 0);
    $('#map').css('height', 0);
})();
