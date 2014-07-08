from twisted.internet import reactor
from twisted.python import log
from collections import defaultdict, deque
import sys,json, re, cgi, subprocess, os
import urllib2
import Channel
from Client import *
from autobahn.websocket import WebSocketServerFactory, \
                               WebSocketServerProtocol, \
                               listenWS

DEBUG = False
SERVER_RESTART_UTILITY = 'Restart.exe'

INVALID_COMMAND_MESSAGE = 'Invalid Public Channel Command. Type /admin help all to get current admin commands'

ADMIN_HELP_MESSAGE =	'Here is the list of all admin commands currently supported' +\
						'<br />'


def constructAdminHelp(command, usage, returns, note = None):
	boldOpen 	= '<b>'
	boldClose 	= '</b>'
	newline		= '<br />'
	tab 		= '    - '
	message 	= ""


	message += boldOpen + command[0] + boldClose + ': ' + command[1] + newline
	message += tab + boldOpen + 'Usage: ' + boldClose + usage + newline
	message += tab + boldOpen + 'Returns: ' + boldClose + returns + newline
	if note:
		message += tab + boldOpen + 'Note: ' + boldClose + note + newline
	message += newline
	return message

#########################################################################################################################################
#														Admin command messages														 	#
#########################################################################################################################################	

#Add the Ban Public Channel Help Command
ADMIN_HELP_MESSAGE += constructAdminHelp([	'ban','Ban a specific user from the entire website'],
											'/admin ban [USER] where [USER] is a specific user to ban',
											'Whether the user was successfully banned.',
											'If the user was not banned successfully, check to make sure userame is correct'
										)
#Add the Unban Public Channel Help Command
ADMIN_HELP_MESSAGE += constructAdminHelp([	'unban','Unban a specific user from the entire website'],
											'/admin unban [USER] where [USER] is a specific user to unban',
											'Whether the user was successfully unbanned.',
											'If the user was not unbanned successfully, check to make sure userame is correct'
										)
#Add the Create Public Channel Help Command
ADMIN_HELP_MESSAGE += constructAdminHelp([	'publicchannel create','Creates a new public channel for all users to see'],
											'/admin publicchannel create [CHANNEL] where [CHANNEL] is the new channel name',
											'Nothing'
										)
#Add the Chatserver stop Help Command
ADMIN_HELP_MESSAGE += constructAdminHelp([	'chatserver stop','Stops the chat server'],
											'/admin chatserver stop',
											'Nothing'
										)
#Add the Chatserver restart Help Command
ADMIN_HELP_MESSAGE += constructAdminHelp([	'chatserver restart','Restarts the chat server'],
											'/admin chatserver restart',
											'Nothing'
										)


#########################################################################################################################################
#																Chat Factory Class													 	#
# Handles the processing of all messages received from the clients.																	 	#
#########################################################################################################################################
class ChatFactory(WebSocketServerFactory):
	protocol = ClientProtocol

	def __init__(self, host, server, debug):
		WebSocketServerFactory.__init__(self, host, server=server, debug=debug)

		#Used to get a dictionary of all clients currently connected.
		#Key 	- Username as a string
		#Value 	- Client protocol
		self.clients = {}

		#Used to get a dictionary of all channels currently created.
		#Key 	- Channel hash as a string
		#Value 	- Channel or Private Channel object
		self.channels = dict()

	#Add the given channel to the dictionary of channels if it is not already there
	def joinChannel(self,channel):
		#Get the channel name to add from the join payload
		self.channels[channel.channelID] = channel
	
	def joinChannel_ch(self,channel):
		log.msg('ChatFactory joining channel: ' + str(channel.channelName))

		channelID = channel.channelID
		self.channels[channelID] = channel
	

	
	#Create private channel with the two friends. DEPRECIATED - Nov 30
	#def addChannel(self,users):
		
		#print privateChannel
		#ClientProtocol.clientsConnected[firstUser].openChannel(secondUser,12345)
		#ClientProtocol.clientsConnected[secondUser].openChannel(firstUser,12345)

	#Remove passed in user from all channels
	def quitChannels(self,client):
		for channel in self.channels:
			self.channels[channel].removeUser(client)
    
	#Dispatches message based on the type field of the messaged passed in. Called from the client protocol.
	def dispatchMessage(self,msg,client):
		#If there is no message, there is nothing to dispatch
		if msg == None:
			return
		
		payload = json.loads(msg)
		msgType = payload['type']
		#Add from Column to json message
		payload['from'] = client.username

		if DEBUG:
			log.msg('Chat Factory dispatching message: ' + msg)
		
		if msgType == 'chat':
			self.channelSend(payload,client)
		elif msgType == 'ack':
			#Index 1 of payload is the channel id
			self.channels[payload['payload'][1]].sendArchivedMessages(client)
		
	#Handles admin functionality. IMPORTANT -- All commands are translated into lower case. 
	def adminFunction(self,msg,client):
		statement = msg['payload'].split()
		
		#If the command  is missing some arguments
		if len(statement) < 3:
			self.adminCommandMessage(client,msg,INVALID_COMMAND_MESSAGE)
			return
		
		#If the command was ban
		if statement[1].lower() == 'ban':
			#Prepare the user message
			banMsg = 'User ' + statement[2]
			#If the ban was successful
			if client.banSpecificUser(statement[2]):
				banMsg += ' was successfully banned.'
			else:
				banMsg += ' was not banned. Please try again later or contact an administrator.'
			self.adminCommandMessage(client,msg,banMsg)

			#If the user to be banned is currently using the chat system, disconnect them with a useful message
			if statement[2] in self.clients:
				self.clients[statement[2]].closeClientError('You have been banned on the chat server')
		#If the command was unban
		elif statement[1].lower() == 'unban':
			#If the ban was successful
			banMsg = 'User ' + statement[2]
			#Server returns users current ban state
			if client.unbanUser(statement[2]):
				banMsg += ' was successfully unbanned.'
			else:
				banMsg += ' was not unbanned. Please try again later or contact an administrator.'
			self.adminCommandMessage(client,msg,banMsg)
		#If the command was to do with a public channel
		elif statement[1].lower() == 'publicchannel':
			self.publicChannelAdminCommand(statement)
		elif statement[1].lower() == 'chatserver':
			self.chatServerAdminCommand(statement,client)
		elif statement[1].lower() == 'help':
			self.adminCommandMessage(client,msg,ADMIN_HELP_MESSAGE)
		else:
			self.adminCommandMessage(client,msg,INVALID_COMMAND_MESSAGE)
			return
	
	#Admin commands that have to do the chatserver itself. Currently supports Shutdown and restarts.
	def chatServerAdminCommand(self,statement,client):
		if statement[2] == 'shutdown':
			reactor.stop()
		elif statement[2] == 'restart':
			#Create a new process to restart the server
			subprocess.Popen([SERVER_RESTART_UTILITY,str(os.getpid()), os.getcwd()+'\server.py'])
			#Stop the server, killing the current process
			reactor.stop()
		else:
			self.adminCommandMessage(client,msg,INVALID_COMMAND_MESSAGE)

	#Admin commands that have to do with public channels. Currently supports create new channel.
	def publicChannelAdminCommand(self,statement):
		channelMsg = 'Channel'
		if len(statement) >= 4:
			if statement[2] == 'create':
				#Create a new public channel
				newPublicChannel = Channel.Channel(statement[3])
				ClientProtocol.publicChannels.append(newPublicChannel)
				self.joinChannel_ch(newPublicChannel)

				#For each client that is connected, 
				for client in ClientProtocol.clientsConnected:
					ClientProtocol.clientsConnected[client].joinPublicChannel(newPublicChannel)
				
			if statement[2] == 'delete':
				#Delete an existing public channel
				pass
		else:
			self.adminCommandMessage(client,msg,INVALID_COMMAND_MESSAGE)
			return
	
	#Send an error message from the server to the specified client in the specified channel
	def adminCommandMessage(self,client,msg,messageText):
		msg['payload'] = messageText
		msg['type'] = 'chat'
		msg['from'] = 'Server'
		client.sendMessage(json.dumps(msg))

	#Sends a message to all users subscrbed to specified channed
	def channelSend(self,inboundMsg,client):
		#decodedMsg = inboundMsg['payload']
		channel = inboundMsg['channel']

		if re.search(r'^/admin\s',inboundMsg['payload']) and client.admin:
			self.adminFunction(inboundMsg,client)
			return

		inboundMsg['payload'] = cgi.escape(inboundMsg['payload'])
		#Convert json message back to string
		outboundMsg = json.dumps(inboundMsg)

		try:
			self.channels[channel].chatSend(outboundMsg)
		except KeyError:
			print 'Channel: ' + str(channel) + ' not found.'
