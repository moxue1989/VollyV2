
var map;
var markers = [];

function addOrganizationMarker(lat, lng, name, address, fullDescription, missionStatement, contactEmail, websiteLink, causename, id) {
    var icon = {
        url: "/images/Home.png"
    };
    var marker = map.addMarker({
        lat: lat,
        lng: lng,
        title: name,
        icon: icon,
        infoWindow: { content: name },
        click: function (e) {
            $("#DisplayModalTitle").html(name);
            $("#ModalAddress").html(address);
            $("#ModalCause").html(causename);
            $("#ModalDescription").html(fullDescription);
            $("#DisplayModal").modal('show');
        }
    });
    markers.push(marker);
    appendOrganizationPanel(name, address, fullDescription, missionStatement, contactEmail, websiteLink, causename, id);
};

function appendOrganizationPanel(name,
    address,
    fullDescription,
    missionStatement,
    contactEmail,
    websiteLink,
    causename,
    id) {
    $("#organizationList").append('<div class="col-lg-4 col-md-6 col-sm-12">' +
        '<br/>' + name +
        '<br/>' + address +
        '<br/>' + contactEmail +
        '<br/>' + websiteLink +
        '<br/>' +
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

        var causename = "";
        if (organization.cause) {
            causename = organization.cause.name;
        }
        addOrganizationMarker(organization.location.latitude,
            organization.location.longitude,
            organization.name,
            organization.address,
            organization.fullDescription,
            organization.missionStatement,
            organization.contactEmail,
            organization.websiteLink,
            causename,
            organization.id
        );
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