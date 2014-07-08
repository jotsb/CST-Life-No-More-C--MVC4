$(document).ready(function() {
	//ChatClient
	chatInterface = new Interface();

	//Create a chatter with the username Taylor
	chatInterface.create("");

});

function initializeClientWindow()
{
		
	chatInterface.authenticate();

	console.log("loaded");
    // instantiate
    messageWindow = $(".MessageWindow ul");
    textbox = $(".TextInputBox");
    buttonbox = $(".ChannelButtonContainer ul");
    channels = new Array();

    textbox.keyup(function(event){
	    if(event.keyCode == 13){
	        chatInterface.sendMessage();
	    }
	});


	//Placeholder. Creates channels. TODO -- FEATURE -- Load default channels

	// construct some default channels
	//createChannel("General",0);
	//currentChannel = $("#0");

	/**
	addChannel("Taylor");
	addChannel("Matt");
	
	
	//Set the selected channel to the first element
	obj = $(".ChannelButtonContainer ul li").first();
	$(obj).attr("class", "SelectedButton");
	oldChannelButton = obj;
	*/
	textbox.focus();

}
var idVal = 1;
//Temporary join channel until better UI is available
function tempJoinChannel()
{
	ch = $("#ch_box").val();
	$("#ch_box").val("");
	createChannel(ch,idVal);

	

	idVal++;
}

function createChannel(name,id)
{
	//If the name is in the channels, dont add again.
	if (id in channels)
	{
		return;
	}
	addChannel(name,id);

	//Select the channel added
	$(".ChannelButtonContainer ul li").each(function(index) {
		if($(this).attr("id") == id)
		{
			$(this).toggleClass("SelectedButton");
			channel($(this),id);
		}
	});
	//chatInterface.joinChannel(id);
}
