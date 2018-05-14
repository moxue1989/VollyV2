const connection = new signalR.HubConnectionBuilder()
    .withUrl("/chatHub")
    .build();

connection.on("ReceiveMessage", (user, message) => {
    const encodedMsg = user + ": " + message;
    const div = document.createElement("div");
    div.textContent = encodedMsg;
    $("#messages").append(div);
});

connection.on("ListOpportunityLinks", (opportunities) => {
    for (var i = 0; i < opportunities.length; i++) {
        var opportunity = opportunities[i];
        const div = document.createElement("div");
        var link = document.createElement('a');
        link.textContent = opportunity.name;
        link.title = opportunity.name;
        link.href = '/Opportunities/Details/' + opportunity.id;

        div.appendChild(link);
        $("#messages").append(div);
    }
});

$('#message').keypress(e => {
    if (e.keyCode === 13)
        $('#send').click();
});


$("#send").click(event => {
    var message = $("#message").val();
    $('#message').val('').focus();
    connection.invoke("SendMessage", message).catch(err => console.error(err.toString()));
    event.preventDefault();
});

connection.start()
    .then(() => {
        const encodedMsg = "VollyBot: " + "Welcome to Volly! Let me help you find some opportunities!\n" +
            "Tell me a category or cause that you are interested in.";
        const div = document.createElement("div");
        div.textContent = encodedMsg;
        $("#messages").append(div);
    })
    .catch(err => console.error(err.toString()));