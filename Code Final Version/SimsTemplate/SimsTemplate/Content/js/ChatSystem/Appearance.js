var messageWindow;
var textbox;
var buttonbox;
var channels;
var currentChannel;
var oldChannelButton = null;

function channel(obj,buttonID)
{
	// save current content
	if (oldChannelButton)
	{
		$(oldChannelButton).attr("class", "");
		channels[oldChannelButton.attr("id")] = messageWindow.html();
	}
	//If this is undefined, it must be the first channel created. A message may be received in the websocket before the channels are ready.
	if(!currentChannel)
	{
		channels[buttonID] = messageWindow.html();
		currentChannel = obj;
	}
	else
	{
		messageWindow.html("");
		// load up its content
		currentChannel = $("#" + buttonID);
	}
    messageWindow.html(channels[buttonID]);

    currentChannel.attr("class", "SelectedButton");
    oldChannelButton = currentChannel;

    textbox.focus();
}

function constructList(sender, message)
{
	var msg = "<li> <div class=\"SenderName\"> <p>";
	msg += sender + ':</p></div> <div class="SenderMessage"> <p>';
	msg += message + "</p> </div> <div class=\"clear\"></div> </li>";
	return msg;
}

function constructButton(name,id)
{
	var b = "<li id=\"" + id + "\" onclick=\"channel(this,\'" + id + "\');\"><p id= \"" + name + "\">"
	b += name + "</p></li>";
	return b;
}

function removeButton(id)
{

	$(".ChannelButtonContainer ul li").each(function(index) {
			if($(this).attr("id") == id)
			{
				$(this).remove();
			}
		});
	
	if (currentChannel.attr("id") == id)
	{
		messageWindow.html("");
	}
	delete(channels[id]);
}

function addChannel(name,id)
{
	channels[id] = "";
	buttonbox.append(constructButton(name,id));
}

function messageReceived(channelID,from,msg)
{
	//If the channel that received the message is the current channel, append to message window. Else, add it to the channel List
	if (!currentChannel || channelID == currentChannel.attr("id"))
	{

		messageWindow.append(constructList(from,msg));
	}	
	else
	{
		channels[channelID] += constructList(from,msg);
		//Find the correct channel id
		$(".ChannelButtonContainer ul li").each(function(index) {
			if($(this).attr("id") == channelID)
			{
				$(this).toggleClass("MessageAvailable");
			}
		});
		
	}

}

/**
 * Removes all channel buttons, as well clears the message window when called
 *
 */
function shutdownClient()
{
	//Remove all buttons on the ChannelButtonContainer
	$(".ChannelButtonContainer ul li").each(function(index) {
		$(this).remove();
	});

	//Remove text from message window. Add a useful message
	messageWindow.html(constructList("Server","Closed"));
}
