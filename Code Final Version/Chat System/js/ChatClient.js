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
		user.send(msg,currentChannel.val());
		
	};

	this.authenticate = function()
	{
		
		user.authenticate();
	};

	this.joinChannel = function(channelName)
	{
		//channel = $("#ch_box").val();
		user.joinChannel(channelName);
	};

	this.create = function(username)
	{
		user = new Chat(username);
	};
}

function Chat(username)
{	
	this.username = username;
	var msgType = {'auth':'auth','chat':'chat','join':'join','err':'err','admin':'admin'};
	this.ws = new WebSocket("ws://localhost:9000/");

	this.send = function(msg,channel)
	{	
		alert(chanel);
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
		//alert("Chat Client Closed");

	};

}

