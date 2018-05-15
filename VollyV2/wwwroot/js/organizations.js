
var map;
var markers = [];

function addOrganizationMarker(organization) {
    var icon = {
        url: "/images/Home.png"
    }; 
    var marker = map.addMarker({
        lat: organization.location.latitude,
        lng: organization.location.longitude,
        title: organization.name,
        icon: icon,
        infoWindow: { content: organization.name },
        click: function (e) {
            openOrganizationModal(organization);
        }
    });
    markers.push(marker);
}

function openOrganizationModal(organization) {
    var causename = "";
    if (organization.cause) {
        causename = organization.cause.name;
    }
    $("#DisplayModalTitle").html(organization.name);
    $("#ModalAddress").html(organization.address);
    $("#ModalCause").html(causename);
    $("#ModalDescription").html(organization.fullDescription);
    $("#DisplayModal").modal('show');
}
function appendOrganizationPanel(organization) {
    $("#organizationList").append('<div id="organization-' + organization.id + '" class="col-lg-4 col-md-6 col-sm-12 result-card">' +
        '<br/>' + organization.name +
        '<br/>' + organization.address +
        '<br/>' + organization.contactEmail +
        '<br/>' + organization.websiteLink +
        '</div>');
    $("#organization-" + organization.id).click(function (e) {
        openOrganizationModal(organization);
    });
}

function initMap() {
    map = new GMaps({
        div: '#map',
        lat: 51.044308,
        lng: -114.0652801,
        zoom: 10
    });
    getAllOrganizations();
};

function getAllOrganizations() {
    $.getJSON(
        '/api/Organizations',
        function (organizations) {
            clearOrganizations();
            addOrganizationMarkers(organizations);
        });
};

function addOrganizationMarkers(organizations) {
    for (var i = 0; i < organizations.length; i++) {
        var organization = organizations[i];
        if (!organization.location) {
            continue;
        }
        addOrganizationMarker(organization);
        appendOrganizationPanel(organization);
    }
};

$("#FilterOrganizations").click(function () {
    var causeIds = $("#CausesList").val();

    var data = {
        "CauseIds": causeIds
    };

    $.ajax({
        type: "POST",
        contentType: "application/json",
        url: '/api/Organizations/Search',
        data: JSON.stringify(data),
        dataType: "json",
        success: function (organizations) {
            clearOrganizations();
            addOrganizationMarkers(organizations);
        }
    });
});

function clearOrganizations() {
    deleteMarkers();
    $("#organizationList").empty();
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