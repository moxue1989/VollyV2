
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
            openOrganizationModal(organization.id);
        }
    });
    markers.push(marker);
}

function openOrganizationModal(id) {
    $.getJSON('/api/Organizations/' + id)
        .then(function (organization) {
            var causename = "";
            if (organization.cause) {
                causename = organization.cause.name;
            }
            $("#DisplayModalTitle").html(organization.name);
            $("#ModalAddress").html(organization.address);
            $("#ModalCause").html(causename);
            $("#ModalDescription").html(organization.fullDescription);
            $("#ModalContactEmail").html(organization.contactEmail);
            $("#ModalWebsiteLink").html(organization.websiteLink);
            $("#AModalWebsiteLink").attr("href", organization.websiteLink);
            $("#ModalContactEmail").html(organization.contactEmail);
            $("#AModalContactEmail").attr("href", 'mailto:' + organization.contactEmail);
            $("#img-organization").attr("src", organization.imageUrl);
            $("#DisplayModal").modal('show');
        });
}
function appendOrganizationPanel(organization) {
    var causename = "";
    if (organization.cause) {
        causename = organization.cause.name;
    }
    $("#OrganizationTable tbody").append('<tr id="organization-' + organization.id + '"><td>' +
        organization.name + '</td><td>' +
        causename + '</td></tr>');
    $("#organization-" + organization.id).click(function (e) {
        openOrganizationModal(organization.id);
    });
}

function initMap() {
    map = new GMaps({
        div: '#map2',
        lat: 51.044308,
        lng: -114.0652801,
        zoom: 14
    });
    getAllOrganizations();
    $('#searchNearMe').click(function () {
        if (navigator.geolocation) {
            navigator.geolocation.getCurrentPosition(
                function (p) {
                    map.panTo(new google.maps.LatLng(p.coords.latitude, p.coords.longitude));
                }, function (e) {
                    alert(e.message);
                }, {
                    timeout: 10000,
                }
            );
        } else {
            alert('Geolocation services must be enabled.')
        }
    })
};

function getAllOrganizations() {
    $.getJSON(
        '/api/Organizations',
        function (organizations) {
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
    $("#OrganizationTable tbody").empty();
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