var messageWindow;
var textbox;
var buttonbox;
var channels;
var currentChannel;
var oldChannelButton = null;

function channel(obj,buttonID)
{
	// save current content
	channels[currentChannel] = messageWindow.html();
	if (oldChannelButton)
		$(oldChannelButton).attr("class", "");

	var instance = jQuery("li", obj);

	alert("ID: " + $("5"));

	// load up its content
	currentChannel = instance.html();
    messageWindow.html(channels[currentChannel]);

    $(obj).attr("class", "SelectedButton");
    oldChannelButton = obj;

    textbox.focus();
}
/*
function send()
{
	console.log("send");

	if (textbox.val() != "")
		messageWindow.append(constructList("Me", textbox.val()))
	// clear the text box
	textbox.val("");
}
*/

function constructList(sender, message)
{
	var msg = "<li> <div class=\"SenderName\"> <p>";
	msg += sender + ':</p></div> <div class="SenderMessage"> <p>';
	msg += message + "</p> </div> <div class=\"clear\"></div> </li>";
	return msg;
}

function constructButton(name,id)
{
	var b = "<li id=\"" + id + "\" onclick=\"channel(this," + id + ");\"><p id= \"" + name + "\">"
	b += name + "</p></li>";
	return b;
}

function addChannel(name)
{
	channels[name] = "";
	buttonbox.append(constructButton(name,1));
}

function messageReceived(channelName,from,msg)
{
	//If the channel that received the message is the current channel, append to message window. Else, add it to the channel List
	if (channelName == currentChannel)
	{
		messageWindow.append(constructList(from,msg));
	}	
	else
	{
		channels[channelName] += constructList(from,msg);
		
		$(".ChannelButtonContainer ul li").each(function(index) {
			if($(this).text() == channelName)
			{
				$(this).toggleClass("MessageAvailable");
			}
		});
		
	}

}



