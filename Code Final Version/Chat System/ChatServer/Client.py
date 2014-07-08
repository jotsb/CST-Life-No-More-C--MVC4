import json, time, os
import urllib2, Cookie
from threading import Thread
from twisted.python import log
import Channel
from autobahn.websocket import WebSocketServerFactory, \
                               WebSocketServerProtocol, \
                               listenWS

#########################################################################################################################################
#																Client Protocol Class													#
# Represents a specific user that is using the chat system.				 																#
# Abstracts functionality towards the actual connection to the front end client.													 	#
#########################################################################################################################################
class ClientProtocol(WebSocketServerProtocol):

	clientsConnected = {}

	#Represents channels
	publicChannels = []
	
	#Creates default cha
	def createPublicChannels(self):

		#Add all the default channels here
		ClientProtocol.publicChannels.append(Channel.Channel("General"))

		#Let the factory know of public channels that have been created
		for ch in ClientProtocol.publicChannels:
			self.factory.joinChannel_ch(ch)
			
	#Initializes instance variables for Client
	def __init__(self):
		self.username = None
		self.authenticated = False
		self.admin = False
		self.BASE_URL = 'http://localhost:51548'
		self.URL_TO_VALIDATE = self.BASE_URL + '/Chat/Validate'
		self.URL_TO_BAN_USER = self.BASE_URL + '/ChatValidator/BanUser?Username='
		self.URL_TO_UNBAN_USER = self.BASE_URL + '/ChatValidator/UnbanUser?Username='
		self.URL_TO_RESTART = self.BASE_URL + '/ChatValidator/RestartChatServer?pid='


	
	#Occurs when the WebSocket is initially opened.
	#
	#First, it connects to the authentication server, ensuring the client is valid.
	#Second, it sets the client's relevant data returned from the authentication server, such as
	#	- If they are an administrator
	#	- If they are banned (NOTE if they are, they server will close the websocket)
	#	- The username of all the friends associated with the user
	#Next, it creates private channels for each of the user's friends that are currently using the chat server
	#Lastly, it joins the user to all public channels created
	def onOpen(self):
		log.msg("Client connected.")
		data = ""
		
		#If no public channels have been created
		if len(ClientProtocol.publicChannels) <= 0:
			self.createPublicChannels()

		request = urllib2.Request(self.URL_TO_VALIDATE)
		#Try to send -a request to the authentication server.
		try:
			#If there is a cookie to read. TODO: ADD ERROR HANDLING IF NO COOKIE EXISTS
			if 'cookie' in self.http_headers:
				request.add_header('Cookie', self.http_headers['cookie'])
			resp = urllib2.urlopen(request)
			content = resp.read()
			resp.close()
		#If there is no server.
		except urllib2.HTTPError:
			self.closeClientError('Cannot connect to authentication service.')
			return
		
		data = json.loads(content)

		if data['authenticated'] == False:
			self.closeClientError('Not Authenticated.')
			return
		
		#If the username field is in the message, set it as the clients username
		if 'username' in data:
			self.username = data['username']
			ClientProtocol.clientsConnected[self.username] = self
			#If the user is banned, disconnect. TODO -- Figure out how to do error codes
			if data['banned']:
				self.closeClientError('You are currently banned from the chat system.')
				return
			if data['admin']:
				self.admin = True
			if data['friendsList']:
				self.friends = data['friendsList']
				#Create private channels for each friend in friendslist
				for friend in self.friends:
					if friend in ClientProtocol.clientsConnected:
						#Create the first combination of username hash
						channelID1 = Channel.getHashOfUsernames([self,self.clientsConnected[friend]])
						#Create the second combination of username hash
						channelID2 = Channel.getHashOfUsernames([self.clientsConnected[friend],self])
						#If the first possibility exists
						if channelID1 in self.factory.channels:
							self.factory.channels[channelID1].reopenChannel([self,self.clientsConnected[friend]])
						#If the second possibility exists
						elif channelID2 in self.factory.channels:
							self.factory.channels[channelID2].reopenChannel([self,self.clientsConnected[friend]])
						else:
							privateChannel = Channel.PrivateChannel([self,self.clientsConnected[friend]])
							self.factory.joinChannel(privateChannel)

			
			for pubChannel in ClientProtocol.publicChannels:
				self.joinPublicChannel(pubChannel)
			
	#Join a public channel
	def joinPublicChannel(self,publicChannel):
		self.joinChannel(publicChannel)
		publicChannel.addUser(self)

	#When the client sends a message general to the chat server. Does NOT occur when a user opens and closes the connection.
	def onMessage(self, msg, binary):
		self.factory.dispatchMessage(msg, self)

	#When the Client closes the WebSocket connection
	def onClose(self, wasClean, code, reason):
		log.msg("Client disconnected.")

		#For each channel that the user has joined, quit that channel
		for channel in self.factory.channels:
			if isinstance(self.factory.channels[channel],Channel.PrivateChannel):
				if self in self.factory.channels[channel].usersConnected:
					self.factory.channels[channel].closeChannel()
		
		del(ClientProtocol.clientsConnected[self.username])
		del(self.factory.clients[self.username])
	#Close the websocket due to an error. Gives the client a useful message before terminating the connection.
	def closeClientError(self,msg):
		#Create an error message to send to chat client
		errorMsg = createErrorMsgJson(msg)
		self.sendMessage(errorMsg)
		#Close the websocket
		self.sendClose()
	
	#Send a request to the validation page to ban user selected. Assumes client has already been validated as an administrator.
	def banSpecificUser(self, userToBan):
		request = urllib2.Request(self.URL_TO_BAN_USER + userToBan)
		try:
			#If there is a cookie to read. TODO: ADD ERROR HANDLING IF NO COOKIE EXISTS
			if 'cookie' in self.http_headers:
				request.add_header('Cookie', self.http_headers['cookie'])
			resp = urllib2.urlopen(request)
			content = resp.read()
			resp.close()
		#If there is no server.
		except urllib2.HTTPError:
			return False
		
		data = json.loads(content)

		#ERROR -- Client does not always receive error message
		if data['banned']:
			#self.closeClientError('You have been banned from the chat system.')
			return True
		
		return False
	
	#Send a request to the validation page to unban user. Assumes client has already been validated as an administrator.
	def unbanUser(self, userToUnban):
		request = urllib2.Request(self.URL_TO_UNBAN_USER + userToUnban)
		try:
			#If there is a cookie to read. TODO: ADD ERROR HANDLING IF NO COOKIE EXISTS
			if 'cookie' in self.http_headers:
				request.add_header('Cookie', self.http_headers['cookie'])
			resp = urllib2.urlopen(request)
			content = resp.read()
			resp.close()
		#If there is no server.
		except urllib2.HTTPError:
			print "URL Error"
			return False
		
		data = json.loads(content)
		
		return not data['banned']
	
	#Make a request to URL_TO_RESTART url with current PID. DEPRECIATED
	#def restartServer(self):
	#	request = urllib2.Request(self.URL_TO_RESTART+str(os.getpid()))
	#	#If there is a cookie to read. TODO: ADD ERROR HANDLING IF NO COOKIE EXISTS
	#	if 'cookie' in self.http_headers:
	#		request.add_header('Cookie', self.http_headers['cookie'])
	#	resp = urllib2.urlopen(request)

	#Join client to specific channel
	def joinChannel(self,channel):
		self.openChannel(channel.getChannelName(self.username),channel.channelID)
	
	#Send a message to the client to create the channel in the UI.
	def openChannel(self,channelName, channelID):
		msg = {}
		msg['type'] = 'join'
		msg['payload'] = [channelName,channelID]
		self.sendMessage(json.dumps(msg))


#Create a json string representing representing an error from a dictionary message passed in.
def createErrorMsgJson(msg):
	errorMsg = {}
	errorMsg['type'] = 'err'
	errorMsg['payload'] = msg
	return json.dumps(errorMsg)
