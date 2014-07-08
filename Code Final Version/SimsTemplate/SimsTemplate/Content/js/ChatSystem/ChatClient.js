var chatInterface;
var user;
var socketOpen;

function Message(type, payload) {
	this.type = type;
	this.payload = payload;
}

function ChatMessage(type,channel,payload)
{
	this.type = type;
	this.channel = channel;
	this.payload = payload
}


function Interface() {
	this.sendMessage = function()
	{
		//Get the value from the text box. Clear value afterward
		msg = $("#msg_box").val();
		$("#msg_box").val("");
		//channel = $("#ch_box").val(); 
		//alert(currentChannel);
		user.send(msg,currentChannel.attr("id"));
		
	};

	this.authenticate = function()
	{
		
		user.authenticate();
	};

	this.joinChannel = function(channelID)
	{
		//channel = $("#ch_box").val();
		user.joinChannel(channelID);
	};

	this.create = function(username)
	{
		user = new Chat(username);
	};
}

var channelsReady = false;

function Chat(username)
{	
	this.username = username;
	var msgType = {'auth':'auth','chat':'chat','join':'join','err':'err','admin':'admin','quit':'quit','ack':'ack'};
	this.ws = new WebSocket("ws://142.232.17.225:9000/");

	this.send = function(msg,channel)
	{	
		chatMsg = new ChatMessage(msgType['chat'],channel, msg);

		var JSONmsg = $.toJSON(chatMsg);
		this.ws.send(JSONmsg);
	};

	/**
	 * Sends a message to authenticate the client on the server.
	 */
	this.authenticate = function()
	{
		var JSONmsg = $.toJSON(new Message(msgType['auth'],this.username));
		this.ws.send(JSONmsg);
	};

	/**
	 * Sends a message to the server to add client to channel.
	 */
	this.joinChannel = function(channel) {

		var jsonstr = $.toJSON(new Message(msgType['join'], channel));
		this.ws.send(jsonstr);
		
	};
	/**
	 * Occurs when the websocket receives a message. 
	 * Decodes the JSON and appends it to the current Channel
	 * TODO!! FIX -- Appends to current channel selected, not desired channel
	 */
	this.ws.onmessage = function(e) {
		var jsonMsg = $.evalJSON(e.data);
		
		//If the message received is a chat message, append to channel messages
		if (jsonMsg['type'] == msgType['chat'])
		{
			messageReceived(jsonMsg['channel'],jsonMsg['from'],jsonMsg['payload']);
		}
		//If the message is an error, display message in alert box.
		else if (jsonMsg['type'] == msgType['err'])
		{
			alert(jsonMsg['payload']);
		}
		else if(jsonMsg['type'] == msgType['join'])
		{
			createChannel(jsonMsg['payload'][0],jsonMsg['payload'][1]);
			jsonMsg['type'] = msgType['ack'];

			//Send an acknowledgement to the server that the channel has been created
			this.send($.toJSON(jsonMsg));
		}
		else if(jsonMsg['type'] == msgType['quit'])
		{
			removeButton(jsonMsg['payload']);
		}

	};

   /**
	 * Occurs when the websocket is initially opened.
	 * Initializes the chat window.
	 * TODO!! FEATURE -- Add some sort of loading message
	 */
	this.ws.onopen = function() {
		initializeClientWindow();
	};

	/**
	 * Occurs when there is an error in the websocket
	 * TODO!! FEATURE -- Add some useful messages
	 */
	this.ws.onerror = function(err) {
		alert(err.data);
	};

	/**
	 * Occurs when the websocket is closed on the server side.
	 * TODO!! Dismantle chat UI
	 */
	this.ws.onclose = function(code) {
		alert("Chat System is not operational at this time. Please try again later." + "\n" + "If this problem persists, contact an administrator.");
		//Set client window to closed state
		shutdownClient();
	};

}

